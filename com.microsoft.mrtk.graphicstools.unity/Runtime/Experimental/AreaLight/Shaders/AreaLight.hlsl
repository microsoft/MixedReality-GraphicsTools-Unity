// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef GT_AREA_LIGHT
#define GT_AREA_LIGHT

// Based off work from: https://github.com/Unity-Technologies/VolumetricLighting

#include "HLSLSupport.cginc"

#pragma multi_compile _ _AREA_LIGHT_ACTIVE _AREA_LIGHTS_ACTIVE

#define AREA_LIGHT_COUNT 2
#define AREA_LIGHT_DATA_SIZE 1
//#define AREA_LIGHT_ENABLE_DIFFUSE

/// <summary>
/// Global properties.
/// </summary>

// LUTs are atlased in the following order:
//   Diffuse    Specular    Fresnel
// +----------+----------+----------+
// |          |          |          |
// |    XX    |    XX    |    XX    |
// |          |          |          |
// +----------+----------+----------+
UNITY_DECLARE_TEX2D(_AreaLightLUTAtlas);

// Shader.SetGlobalTexture…Array(…) does not exist, so this is the best we can do.
UNITY_DECLARE_TEX2D(_AreaLightCookie0);
UNITY_DECLARE_TEX2D_NOSAMPLER(_AreaLightCookie1); // Reuse _AreaLightCookie0's sampler.

float4 _AreaLightData[AREA_LIGHT_COUNT * AREA_LIGHT_DATA_SIZE];
float4x4 _AreaLightVerts[AREA_LIGHT_COUNT];

/// <summary>
/// Lighting methods.
/// </summary>

bool RayPlaneIntersect(float3 direction, float3 origin, float4 plane, out float t)
{
	t = -dot(plane, float4(origin, 1.0)) / dot(plane.xyz, direction);
	return t > 0.0;
}

half4 SampleAreaLightCookie(in int lightIndex, in float2 uv)
{
#if defined(_AREA_LIGHTS_ACTIVE)
	[forcecase]
	switch (lightIndex)
	{
		case 0:
			return UNITY_SAMPLE_TEX2D(_AreaLightCookie0, uv);
		case 1:
			return UNITY_SAMPLE_TEX2D_SAMPLER(_AreaLightCookie1, _AreaLightCookie0, uv);
		default:
			return half4(1, 1, 1, 1);
	}
#else
	return UNITY_SAMPLE_TEX2D(_AreaLightCookie0, uv);
#endif // _AREA_LIGHTS_ACTIVE
}

half3 SampleDiffuseFilteredTexture(in int lightIndex, in float4x3 L, float3 direction)
{
	float3 p1 = L[0];
	float3 p2 = L[1];
	float3 p3 = L[2];
	float3 p4 = L[3];

	// Area light plane basis.
	float3 V1 = p2 - p1;
	float3 V2 = p4 - p1;
	float3 planeOrtho = cross(V1, V2);
	float planeAreaSquared = dot(planeOrtho, planeOrtho);

	float4 plane = float4(planeOrtho, -dot(planeOrtho, p1));
	float planeDist;
	RayPlaneIntersect(direction, 0, plane, planeDist);

	float3 P = planeDist * direction - p1;

	// Find tex coords of P.
	float dot_V1_V2 = dot(V1, V2);
	float inv_dot_V1_V1 = 1.0 / dot(V1, V1);
	float3 V2_ = V2 - V1 * dot_V1_V2 * inv_dot_V1_V1;
	float2 uv;
	uv.x = dot(V2_, P) / dot(V2_, V2_);
	uv.y = abs(_AreaLightData[lightIndex].a - (dot(V1, P) * inv_dot_V1_V1 - dot_V1_V2 * inv_dot_V1_V1 * uv.x));

	return SampleAreaLightCookie(lightIndex, uv).rgb;
}

float3 IntegrateEdge(in float3 v1, in float3 v2)
{
	float x = dot(v1, v2);
	float y = abs(x);

	float a = 0.8543985 + (0.4965155 + 0.0145206 * y) * y;
	float b = 3.4175940 + (4.1616724 + y) * y;
	float v = a / b;

	float theta_sintheta = (x > 0.0) ? v : 0.5 * rsqrt(max(1.0 - x * x, 1e-7)) - v;

	return cross(v1, v2) * theta_sintheta;
}

float3 PolygonRadiance(in int lightIndex, in float4x3 L)
{
	// Baum's equation
	// Expects non-normalized vertex positions

	float4x3 unclippedL = L;

	// Detect clipping config.
	uint config = 0;
	if (L[0].z > 0) { config += 1; }
	if (L[1].z > 0) { config += 2; }
	if (L[2].z > 0) { config += 4; }
	if (L[3].z > 0) { config += 8; }
			
	// The fifth vertex for cases when clipping cuts off one corner.
	// Due to a compiler bug, copying L into a vector array with 5 rows
	// messes something up, so we need to stick with the matrix + the L4 vertex.
	float3 L4 = L[3];
			
	// This switch is surprisingly fast. Tried replacing it with a lookup array of vertices.
	// Even though that replaced the switch with just some indexing and no branches, it became
	// way, way slower - mem fetch stalls?
	
	// Clip.
	uint n = 0;
	switch (config)
	{
		case 0: // clip all
			break;
						
		case 1: // V1 clip V2 V3 V4
			n = 3;
			L[1] = -L[1].z * L[0] + L[0].z * L[1];
			L[2] = -L[3].z * L[0] + L[0].z * L[3];
			break;
						
		case 2: // V2 clip V1 V3 V4
			n = 3;
			L[0] = -L[0].z * L[1] + L[1].z * L[0];
			L[2] = -L[2].z * L[1] + L[1].z * L[2];
			break;
						
		case 3: // V1 V2 clip V3 V4
			n = 4;
			L[2] = -L[2].z * L[1] + L[1].z * L[2];
			L[3] = -L[3].z * L[0] + L[0].z * L[3];
			break;
						
		case 4: // V3 clip V1 V2 V4
			n = 3;
			L[0] = -L[3].z * L[2] + L[2].z * L[3];
			L[1] = -L[1].z * L[2] + L[2].z * L[1];
			break;
						
		case 5: // V1 V3 clip V2 V4: impossible
			break;
						
		case 6: // V2 V3 clip V1 V4
			n = 4;
			L[0] = -L[0].z * L[1] + L[1].z * L[0];
			L[3] = -L[3].z * L[2] + L[2].z * L[3];
			break;
						
		case 7: // V1 V2 V3 clip V4
			n = 5;
			L4 = -L[3].z * L[0] + L[0].z * L[3];
			L[3] = -L[3].z * L[2] + L[2].z * L[3];
			break;
						
		case 8: // V4 clip V1 V2 V3
			n = 3;
			L[0] = -L[0].z * L[3] + L[3].z * L[0];
			L[1] = -L[2].z * L[3] + L[3].z * L[2];
			L[2] = L[3];
			break;
						
		case 9: // V1 V4 clip V2 V3
			n = 4;
			L[1] = -L[1].z * L[0] + L[0].z * L[1];
			L[2] = -L[2].z * L[3] + L[3].z * L[2];
			break;
						
		case 10: // V2 V4 clip V1 V3: impossible
			break;
						
		case 11: // V1 V2 V4 clip V3
			n = 5;
			L[3] = -L[2].z * L[3] + L[3].z * L[2];
			L[2] = -L[2].z * L[1] + L[1].z * L[2];
			break;
						
		case 12: // V3 V4 clip V1 V2
			n = 4;
			L[1] = -L[1].z * L[2] + L[2].z * L[1];
			L[0] = -L[0].z * L[3] + L[3].z * L[0];
			break;
						
		case 13: // V1 V3 V4 clip V2
			n = 5;
			L[3] = L[2];
			L[2] = -L[1].z * L[2] + L[2].z * L[1];
			L[1] = -L[1].z * L[0] + L[0].z * L[1];
			break;
						
		case 14: // V2 V3 V4 clip V1
			n = 5;
			L4 = -L[0].z * L[3] + L[3].z * L[0];
			L[0] = -L[0].z * L[1] + L[1].z * L[0];
			break;
						
		case 15: // V1 V2 V3 V4
			n = 4;
			break;
	}
			
	if (n == 0)
	{
		return 0;
	}
			
	// Normalize.
	L[0] = normalize(L[0]);
	L[1] = normalize(L[1]);
	L[2] = normalize(L[2]);

	if (n == 3)
	{
		L[3] = L[0];
	}
	else
	{
		L[3] = normalize(L[3]);
		if (n == 4)
		{
			L4 = L[0];
		}
		else
		{
			L4 = normalize(L4);
		}
	}

	// Integrate.
	float3 sum = 0;
	sum += IntegrateEdge(L[0], L[1]);
	sum += IntegrateEdge(L[1], L[2]);
	sum += IntegrateEdge(L[2], L[3]);

	if (n >= 4)
	{
		sum += IntegrateEdge(L[3], L4);
	}

	if (n == 5)
	{
		sum += IntegrateEdge(L4, L[0]);
	}

	float3 direction = normalize(sum);
	sum.z = max(0, sum.z * 0.15915); // 1 / (2 * Pi).
	return SampleDiffuseFilteredTexture(lightIndex, unclippedL, direction) * sum.z;
}

half4 SampleAtlas(in float2 uv, in half index)
{
	return UNITY_SAMPLE_TEX2D(_AreaLightLUTAtlas, float2(uv.x / 3.0 + (index / 3.0), uv.y));
}

half3 TransformedPolygonRadiance(in int lightIndex, 
								 in float4x3 L, 
								 in float2 uv, 
								 in half transformIndex,
								 in float amplitude)
{
	// Get the inverse LTC matrix M.
	float3x3 Minv = 0;
	Minv._m22 = 1;
	Minv._m00_m02_m11_m20 = SampleAtlas(uv, transformIndex);
						
	// Transform light vertices into diffuse configuration.
	float4x3 LTransformed = mul(L, Minv);
			
	// Polygon radiance in transformed configuration - specular.
	return PolygonRadiance(lightIndex, LTransformed) * amplitude.xxx;
}

void CalculateAreaLight(in float3 worldPosition,
						in float3 worldCameraPosition,
						in float3 worldNormal,
						in half3 baseColor,
						in half3 specularColor, 
						in half smoothness,
						in int lightIndex,
						out half3 output)
{
	// TODO - [Cameron-Micka] larger and smaller values cause artifacts - why?
	smoothness = clamp(smoothness, 0.01, 0.93);
	half roughness = 1 - smoothness;
	float3 V = normalize(worldCameraPosition - worldPosition);
			
	// Construct orthonormal basis around worldNormal, aligned with V.
	float3x3 basis;
	basis[0] = normalize(V - worldNormal * dot(V, worldNormal));
	basis[1] = normalize(cross(worldNormal, basis[0]));
	basis[2] = worldNormal;
					
	// Transform light vertices into that space.
	float4x3 L;
	L = (float4x3)_AreaLightVerts[lightIndex] - float4x3(worldPosition, worldPosition, worldPosition, worldPosition);
	L = mul(L, transpose(basis));
			
	// UVs for sampling the LUTs.
	float theta = acos(dot(V, worldNormal));
	half2 uv = half2(roughness, theta / 1.57); // Half Pi.
			
	half3 AmpDiffAmpSpecFresnel = SampleAtlas(uv, 2).rgb;
			
	half3 result = 0;
#if defined(AREA_LIGHT_ENABLE_DIFFUSE)
	half3 diffuseTerm = TransformedPolygonRadiance(lightIndex, L, uv, 0, AmpDiffAmpSpecFresnel.x);
	result = diffuseTerm * baseColor;
#endif // AREA_LIGHT_ENABLE_DIFFUSE
			
	half3 specularTerm = TransformedPolygonRadiance(lightIndex, L, uv, 1, AmpDiffAmpSpecFresnel.y);
	half3 fresnelTerm = (half) (specularColor + (1.0 - specularColor) * AmpDiffAmpSpecFresnel.z);
	result += specularTerm * fresnelTerm * 3.14159265359; // Pi.

	output = result * _AreaLightData[lightIndex].rgb;
}

/// <summary>
/// Entry point, call this from your handwritten shader.
/// </summary>
void CalculateAreaLights(in float3 worldPosition,
						 in float3 worldCameraPosition,
						 in float3 worldNormal,
						 in half3 baseColor,
						 in half3 specularColor,
						 in half smoothness,
						 out half3 output)
{
	output = baseColor;

#if defined(_AREA_LIGHT_ACTIVE)
	half3 light;
	CalculateAreaLight(worldPosition, 
					   worldCameraPosition, 
					   worldNormal, 
					   baseColor, 
					   specularColor, 
					   smoothness,
					   0,
					   light);
	output += light;
#elif defined(_AREA_LIGHTS_ACTIVE)
	for (int i = 0; i < AREA_LIGHT_COUNT; ++i)
	{
		half3 light;
		CalculateAreaLight(worldPosition, 
						   worldCameraPosition, 
						   worldNormal, 
						   baseColor, 
						   specularColor, 
						   smoothness,
						   i,
						   light);
		output += light;
	}
#endif // _AREA_LIGHTS_ACTIVE
}

/// <summary>
/// Entry point, call this from Shader Graph (half precision).
/// </summary>
void CalculateAreaLights_half(in float3 worldPosition,
							  in float3 worldCameraPosition,
							  in float3 worldNormal,
							  in half3 baseColor,
							  in half3 specularColor,
							  in half smoothness,
							  out half3 output)
{
	CalculateAreaLights(worldPosition,
						worldCameraPosition,
						worldNormal,
						baseColor,
						specularColor,
						smoothness,
						output);
}

/// <summary>
/// Entry point, call this from Shader Graph (full precision).
/// </summary>
void CalculateAreaLights_float(in float3 worldPosition,
							   in float3 worldCameraPosition,
							   in float3 worldNormal,
							   in half3 baseColor,
							   in half3 specularColor,
							   in half smoothness,
							   out half3 output)
{
	CalculateAreaLights(worldPosition,
						worldCameraPosition,
						worldNormal,
						baseColor,
						specularColor,
						smoothness,
						output);
}

#endif // GT_AREA_LIGHT

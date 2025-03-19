// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef GT_AREA_LIGHT
#define GT_AREA_LIGHT

// Based off work from: https://github.com/Unity-Technologies/VolumetricLighting

/// <summary>
/// Defines.
/// </summary>

#define AREA_LIGHT_COUNT 2
#define AREA_LIGHT_DATA_SIZE 1
#define AREA_LIGHT_ENABLE_DIFFUSE 0

/// <summary>
/// Global properties.
/// </summary>

#if AREA_LIGHT_ENABLE_DIFFUSE
sampler2D _TransformInv_Diffuse;
#endif
sampler2D _TransformInv_Specular;
sampler2D _AmpDiffAmpSpecFresnel;

// Shader.SetGlobalTexture…Array(…) does not exist, so this is the best we can do.
sampler2D _AreaLightCookie0;
sampler2D _AreaLightCookie1;

float4 _AreaLightData[AREA_LIGHT_COUNT * AREA_LIGHT_DATA_SIZE];
float4x4 _AreaLightVerts[AREA_LIGHT_COUNT];

/// <summary>
/// Lighting methods.
/// </summary>

half IntegrateEdge(in half3 v1, in half3 v2)
{
	float cosTheta = dot(v1, v2);
	float theta = acos(cosTheta);
	float cross = (v1.x * v2.y - v1.y * v2.x);
	return cross * ((theta > 0.001) ? theta / sin(theta) : 1.0);
}
			
half PolygonRadiance(in half4x3 L)
{
	// Baum's equation
	// Expects non-normalized vertex positions

	// Detect clipping config.
	uint config = 0;
	if (L[0].z > 0) { config += 1; }
	if (L[1].z > 0) { config += 2; }
	if (L[2].z > 0) { config += 4; }
	if (L[3].z > 0) { config += 8; }
			
	// The fifth vertex for cases when clipping cuts off one corner.
	// Due to a compiler bug, copying L into a vector array with 5 rows
	// messes something up, so we need to stick with the matrix + the L4 vertex.
	half3 L4 = L[3];
			
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
	half sum = 0;
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
				
	sum *= 0.15915; // 1/2pi
			
	return max(0, sum);
}
			
half TransformedPolygonRadiance(in half4x3 L, in half2 uv, in sampler2D transformInv, in half amplitude)
{
	// Get the inverse LTC matrix M.
	half3x3 Minv = 0;
	Minv._m22 = 1;
	Minv._m00_m02_m11_m20 = tex2D(transformInv, uv);
						
	// Transform light vertices into diffuse configuration.
	half4x3 LTransformed = mul(L, Minv);
			
	// Polygon radiance in transformed configuration - specular.
	return PolygonRadiance(LTransformed) * amplitude;
}

half4 SampleAreaLightCookie(in int lightIndex, in float2 uv)
{
	[forcecase]
	switch (lightIndex)
	{
		case 0:
			return tex2D(_AreaLightCookie0, uv);
		case 1:
			return tex2D(_AreaLightCookie1, uv);
		default:
			return half4(1, 1, 1, 1);
	}
}

half3 SampleDiffuseFilteredTexture(in int lightIndex, in half4x3 L)
{
	float3 p1_ = L[0];
	float3 p2_ = L[1];
	float3 p3_ = L[2];
	float3 p4_ = L[3];

	// Area light plane basis.
	float3 V1 = p2_ - p1_;
	float3 V2 = p4_ - p1_;
	float3 planeOrtho = (cross(V1, V2));
	float planeAreaSquared = dot(planeOrtho, planeOrtho);
	float planeDistxPlaneArea = dot(planeOrtho, p1_);

	// Orthonormal projection of (0,0,0) in area light space.
	float3 P = planeDistxPlaneArea * planeOrtho / planeAreaSquared - p1_;
				
	// Find tex coords of P.
	float dot_V1_V2 = dot(V1, V2);
	float inv_dot_V1_V1 = 1.0 / dot(V1, V1);
	float3 V2_ = V2 - V1 * dot_V1_V2 * inv_dot_V1_V1;
	float2 Puv;
	Puv.x = dot(V2_, P) / dot(V2_, V2_);
	Puv.y = 1 - (dot(V1, P) * inv_dot_V1_V1 - dot_V1_V2 * inv_dot_V1_V1 * Puv.x);
	float2 uv = float2(0.125, 0.125) + 0.75 * Puv;

	// TODO, calculate mip level based on distance to area light if the texture has pre-filtered mip levels.
	//float d = abs(planeDistxPlaneArea) / pow(planeAreaSquared, 0.75);
	//float w = log(1024.0 * d) / log(6.0); // TODO get texture size.
	//return tex2Dlod(texLightFiltered, float4(uv.x, uv.y, 0, w)).rgb;

	return SampleAreaLightCookie(lightIndex, uv).rgb;
}

void CalculateAreaLight(in half3 worldPosition,
						in half3 worldCameraPosition,
						in half3 worldNormal,
						in half3 diffuseColor,
						in half3 specularColor, 
						in half smoothness,
						in int lightIndex,
						out half3 output)
{
	// TODO: larger and smaller values cause artifacts - why?
	smoothness = clamp(smoothness, 0.01, 0.93);
	half roughness = 1 - smoothness;
	half3 V = normalize(worldCameraPosition - worldPosition);
			
	// Construct orthonormal basis around worldNormal, aligned with V.
	half3x3 basis;
	basis[0] = normalize(V - worldNormal * dot(V, worldNormal));
	basis[1] = normalize(cross(worldNormal, basis[0]));
	basis[2] = worldNormal;
					
	// Transform light vertices into that space.
	half4x3 L;
	L = (half4x3)_AreaLightVerts[lightIndex] - half4x3(worldPosition, worldPosition, worldPosition, worldPosition);
	L = mul(L, transpose(basis));

	// Texture. TODO, disable if no texture.
	half3 textureColor = SampleDiffuseFilteredTexture(lightIndex, L);
			
	// UVs for sampling the LUTs.
	half theta = acos(dot(V, worldNormal));
	half2 uv = half2(roughness, theta / 1.57);
			
	half3 AmpDiffAmpSpecFresnel = tex2D(_AmpDiffAmpSpecFresnel, uv).rgb;
			
	half3 result = 0;
#if AREA_LIGHT_ENABLE_DIFFUSE
	half diffuseTerm = TransformedPolygonRadiance(L, uv, _TransformInv_Diffuse, AmpDiffAmpSpecFresnel.x);
	result = diffuseTerm * diffuseColor;
#endif
			
	half specularTerm = TransformedPolygonRadiance(L, uv, _TransformInv_Specular, AmpDiffAmpSpecFresnel.y);
	half fresnelTerm = specularColor + (1.0 - specularColor) * AmpDiffAmpSpecFresnel.z;
	result += specularTerm * fresnelTerm * 3.14159265359; // Pi.

	output = result * _AreaLightData[lightIndex].rgb * textureColor;
}

void CalculateAreaLights(in half3 worldPosition,
						 in half3 worldCameraPosition,
						 in half3 worldNormal,
						 in half3 diffuseColor,
						 in half3 specularColor,
						 in half smoothness,
						 out half3 output)
{
	output = 0;

	for (int i = 0; i < AREA_LIGHT_COUNT; ++i)
	{
		half3 light;
		CalculateAreaLight(worldPosition, 
						   worldCameraPosition, 
						   worldNormal, 
						   diffuseColor, 
						   specularColor, 
						   smoothness,
						   i,
						   light);
		output += light;
	}
}

// Shader Graph full precision version.
void CalculateAreaLights_float(in half3 worldPosition,
							   in half3 worldCameraPosition,
							   in half3 worldNormal,
							   in half3 diffuseColor,
							   in half3 specularColor,
							   in half smoothness,
							   out half3 output)
{
	CalculateAreaLights(worldPosition,
						worldCameraPosition,
						worldNormal,
						diffuseColor,
						specularColor,
						smoothness,
						output);
}

#endif // GT_AREA_LIGHT

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Experimental/Area Light"
{
	Properties 
	{
		_Color("Color", Color) = (1,1,1,1)
		_SpecColor("Specular", Color) = (0.5, 0.5, 0.5)
		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Comment in to help with RenderDoc debugging.
			//#pragma enable_d3d11_debug_symbols

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				half3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 worldPosition : TEXCOORD0;
				half3 worldNormal : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPosition = mul(UNITY_MATRIX_M, v.vertex).xyz;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}

			CBUFFER_START(UnityPerMaterial)
			fixed4 _Color;
			fixed4 _SpecColor;
			fixed _Smoothness;
			CBUFFER_END

			#define AREA_LIGHT_ENABLE_DIFFUSE 1

			/// <summary>
			/// Global properties.
			/// </summary>
			#if AREA_LIGHT_ENABLE_DIFFUSE
			sampler2D _TransformInv_Diffuse;
			#endif
			sampler2D _TransformInv_Specular;
			sampler2D _AmpDiffAmpSpecFresnel;

			#define AREA_LIGHT_COUNT 2
			#define AREA_LIGHT_DATA_SIZE 1
			float4 _AreaLightData[AREA_LIGHT_COUNT * AREA_LIGHT_DATA_SIZE];
			float4x4 _AreaLightVerts[AREA_LIGHT_COUNT];

			/// <summary>
			/// Lighting methods.
			/// </summary>
			half IntegrateEdge(half3 v1, half3 v2)
			{
				half theta = acos(max(-0.9999, dot(v1,v2)));
				half theta_sintheta = theta / sin(theta);
				return theta_sintheta * (v1.x*v2.y - v1.y*v2.x);
			}
			
			// Baum's equation
			// Expects non-normalized vertex positions
			half PolygonRadiance(half4x3 L)
			{
				// detect clipping config	
				uint config = 0;
				if (L[0].z > 0) config += 1;
				if (L[1].z > 0) config += 2;
				if (L[2].z > 0) config += 4;
				if (L[3].z > 0) config += 8;
			
			
				// The fifth vertex for cases when clipping cuts off one corner.
				// Due to a compiler bug, copying L into a vector array with 5 rows
				// messes something up, so we need to stick with the matrix + the L4 vertex.
				half3 L4 = L[3];
			
				// This switch is surprisingly fast. Tried replacing it with a lookup array of vertices.
				// Even though that replaced the switch with just some indexing and no branches, it became
				// way, way slower - mem fetch stalls?
			
				// clip
				uint n = 0;
				switch(config)
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
						L[2] =  L[3];
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
					return 0;
			
				// normalize
				L[0] = normalize(L[0]);
				L[1] = normalize(L[1]);
				L[2] = normalize(L[2]);
				if(n == 3)
					L[3] = L[0];
				else
				{
					L[3] = normalize(L[3]);
					if (n == 4)
						L4 = L[0];
					else
						L4 = normalize(L4);
				}
				
				// integrate
				half sum = 0;
				sum += IntegrateEdge(L[0], L[1]);
				sum += IntegrateEdge(L[1], L[2]);
				sum += IntegrateEdge(L[2], L[3]);
				if(n >= 4)	
					sum += IntegrateEdge(L[3], L4);
				if(n == 5)
					sum += IntegrateEdge(L4, L[0]);
				
				sum *= 0.15915; // 1/2pi
			
				return max(0, sum);
			}
			
			half TransformedPolygonRadiance(half4x3 L, half2 uv, sampler2D transformInv, half amplitude)
			{
				// Get the inverse LTC matrix M
				half3x3 Minv = 0;
				Minv._m22 = 1;
				Minv._m00_m02_m11_m20 = tex2D(transformInv, uv);
						
				// Transform light vertices into diffuse configuration
				half4x3 LTransformed = mul(L, Minv);
			
				// Polygon radiance in transformed configuration - specular
				return PolygonRadiance(LTransformed) * amplitude;
			}
			
			half3 CalculateLight (half3 position, half3 diffColor, half3 specColor, half oneMinusRoughness, half3 N, int lightIndex)
			{
				// TODO: larger and smaller values cause artifacts - why?
				oneMinusRoughness = clamp(oneMinusRoughness, 0.01, 0.93);
				half roughness = 1 - oneMinusRoughness;
				half3 V = normalize(_WorldSpaceCameraPos - position);
			
				// Construct orthonormal basis around N, aligned with V
				half3x3 basis;
				basis[0] = normalize(V - N * dot(V, N));
				basis[1] = normalize(cross(N, basis[0]));
				basis[2] = N;
					
				// Transform light vertices into that space
				half4x3 L;
				L = _AreaLightVerts[lightIndex] - half4x3(position, position, position, position);
				L = mul(L, transpose(basis));
			
				// UVs for sampling the LUTs
				half theta = acos(dot(V, N));
				half2 uv = half2(roughness, theta/1.57);
			
				half3 AmpDiffAmpSpecFresnel = tex2D(_AmpDiffAmpSpecFresnel, uv).rgb;
			
				half3 result = 0;
			#if AREA_LIGHT_ENABLE_DIFFUSE
				half diffuseTerm = TransformedPolygonRadiance(L, uv, _TransformInv_Diffuse, AmpDiffAmpSpecFresnel.x);
				result = diffuseTerm * diffColor;
			#endif
			
				half specularTerm = TransformedPolygonRadiance(L, uv, _TransformInv_Specular, AmpDiffAmpSpecFresnel.y);
				half fresnelTerm = specColor + (1.0 - specColor) * AmpDiffAmpSpecFresnel.z;
				result += specularTerm * fresnelTerm * UNITY_PI;

				return result * _AreaLightData[lightIndex];
			}

			fixed4 frag (v2f i) : SV_Target
			{
				half3 worldPos = i.worldPosition;
				half3 baseColor = _Color;
				half3 specColor = _SpecColor;
				half oneMinusRoughness = _Smoothness;
				half3 normalWorld = normalize(i.worldNormal);

				fixed3 output = _Color;
				for (int i = 0; i < AREA_LIGHT_COUNT; ++i)
				{
					output += CalculateLight(worldPos, 
											 baseColor, 
											 specColor, 
											 oneMinusRoughness, 
											 normalWorld,
											 i);
				}

				return fixed4(output, 1);
			}
			ENDCG
		}
	}
}

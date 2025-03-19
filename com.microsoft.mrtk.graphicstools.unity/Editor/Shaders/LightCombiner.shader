// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/// <summary>
/// Shader used by the LightCombinerWindow for GPU accelerated lightmap packing.
/// </summary>
Shader "Hidden/Graphics Tools/Light Combiner"
{
	Properties
	{
		_AlbedoColor("Albedo Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_AlbedoMap("Albedo", 2D) = "white" {}
		_AlbedoMapScaleOffset("Albedo Map Scale Offset", Vector)  = (1, 1, 0, 0)

		_RemappedAlbedoMap("Remapped Albedo", 2D) = "white" {}
		_RemappedAlbedoMaskMap("Remapped Albedo Mask", 2D) = "black" {}
		_AlbedoMapSize("Albedo Map Size", Vector)  = (256, 256, 0, 0)
		_LightMap("Light Map", 2D) = "black" {}
		_LightMapScaleOffset("Light Map Scale Offset", Vector)  = (1, 1, 0, 0)
		_DilationSteps("Dilation Steps", Float) = 16
	}
	SubShader
	{
		Pass
		{
			Name "Albedo"

			Cull Off

			HLSLPROGRAM

			#pragma multi_compile_local _ USE_LIGHTMAP_SCALE_OFFSET

			#pragma vertex VertexStage
			#pragma fragment PixelStage

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv1 : TEXCOORD0;
			};

			TEXTURE2D(_AlbedoMap);
			SAMPLER(sampler_AlbedoMap);

			half4 _AlbedoMapScaleOffset;
			half4 _LightMapScaleOffset;

			v2f VertexStage(appdata v)
			{
				v2f o;
#if defined(USE_LIGHTMAP_SCALE_OFFSET)
				float2 uv = v.uv  * _LightMapScaleOffset.xy + _LightMapScaleOffset.zw;
#else
				float2 uv = v.uv;
				uv.y = 1 - uv.y;
#endif
				o.vertex = float4(uv * 2 - 1, 0, 1);
				o.uv1 = v.uv1;

				return o;
			}

			half4 PixelStage(v2f i) : SV_Target
			{
				float2 albedoUV = i.uv1  * _AlbedoMapScaleOffset.xy + _AlbedoMapScaleOffset.zw;
				return SAMPLE_TEXTURE2D(_AlbedoMap, sampler_AlbedoMap, albedoUV);
			}

			ENDHLSL
		}

		Pass
		{
			Name "AlbedoMask"

			Cull Off

			HLSLPROGRAM

			#pragma multi_compile_local _ USE_LIGHTMAP_SCALE_OFFSET

			#pragma vertex VertexStage
			#pragma fragment PixelStage

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			half4 _LightMapScaleOffset;

			v2f VertexStage(appdata v)
			{
				v2f o;
#if defined(USE_LIGHTMAP_SCALE_OFFSET)
				float2 uv = v.uv  * _LightMapScaleOffset.xy + _LightMapScaleOffset.zw;
#else
				float2 uv = v.uv;
				uv.y = 1 - uv.y;
#endif
				o.vertex = float4(uv * 2 - 1, 0, 1);

				return o;
			}

			half4 PixelStage(v2f i) : SV_Target
			{
				return half4(1, 0, 0, 1);
			}

			ENDHLSL
		}

		Pass
		{
			Name "Lightmap"

			HLSLPROGRAM

			#pragma multi_compile_local _ USE_LIGHTMAP_SCALE_OFFSET
			#pragma multi_compile_local _ CONVERT_TO_SRGB
			#pragma multi_compile_local _ DILATE

			#pragma vertex VertexStage
			#pragma fragment PixelStage

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			half4 _AlbedoColor;

			TEXTURE2D(_RemappedAlbedoMap);
			SAMPLER(sampler_RemappedAlbedoMap);

			TEXTURE2D(_RemappedAlbedoMaskMap);
			SAMPLER(sampler_RemappedAlbedoMaskMap);

			half4 _AlbedoMapSize;

			TEXTURE2D(_LightMap);
			SAMPLER(sampler_LightMap);

			half4 _LightMapScaleOffset;
			float _DilationSteps;

			v2f VertexStage(appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.uv = v.uv;

				return o;
			}

			// Based on this technique: https://shaderbits.com/blog/uv-dilation
			float3 UVPositionalDilation(float2 uv)
			{
				float texelsize = 1 / min(_AlbedoMapSize.x, _AlbedoMapSize.y);
				float minDistance = 10000000;
				float2 offsets[8] = {float2(-1,0), float2(1,0), float2(0,1), float2(0,-1), 
									 float2(-1,1), float2(1,1), float2(1,-1), float2(-1,-1)};
				
				float3 sampleMask = SAMPLE_TEXTURE2D(_RemappedAlbedoMaskMap, sampler_RemappedAlbedoMaskMap, uv).rgb;
				float3 currentMinSample = SAMPLE_TEXTURE2D(_RemappedAlbedoMap, sampler_RemappedAlbedoMap, uv).rgb;
				
				if (sampleMask.r == 0)
				{
					int i = 0;
					[loop]
					while (i < _DilationSteps)
					{ 
						++i;
						int j = 0;
						
						[unroll]
						while (j < 8)
						{
							float2 currentUV = uv + offsets[j] * texelsize * i;
							float3 offsetSampleMask = SAMPLE_TEXTURE2D(_RemappedAlbedoMaskMap, sampler_RemappedAlbedoMaskMap, currentUV).rgb;
							float3 offsetSample = SAMPLE_TEXTURE2D(_RemappedAlbedoMap, sampler_RemappedAlbedoMap, currentUV).rgb;

							if (offsetSampleMask.r != 0)
							{
								float currentDistance = length(uv - currentUV);
				
								if (currentDistance < minDistance)
								{
									float2 projectUV = currentUV + offsets[j] * texelsize * i * 0.25;
									float3 directionMask = SAMPLE_TEXTURE2D(_RemappedAlbedoMaskMap, sampler_RemappedAlbedoMaskMap, projectUV).rgb;
									float3 direction = SAMPLE_TEXTURE2D(_RemappedAlbedoMap, sampler_RemappedAlbedoMap, projectUV).rgb;
									minDistance = currentDistance;
				
									if (directionMask.r != 0 || directionMask.g != 0 || directionMask.b != 0)
									{
										float3 delta = offsetSample - direction;
										currentMinSample = offsetSample + delta * 4;
									}
									else
									{
										currentMinSample = offsetSample;
									}
								}
							}
							++j;
						}
					}
				}
				
				return currentMinSample;
			}

			half4 PixelStage(v2f i) : SV_Target
			{
				float2 albedoUV = i.uv;
#if defined(USE_LIGHTMAP_SCALE_OFFSET)
				albedoUV.y = 1.0 - albedoUV.y;
#endif
				half4 albedo = SAMPLE_TEXTURE2D(_RemappedAlbedoMap, sampler_RemappedAlbedoMap, albedoUV);
#if defined(DILATE)
				albedo = half4(UVPositionalDilation(albedoUV), albedo.a);
#endif

#if defined(USE_LIGHTMAP_SCALE_OFFSET)
				float2 lightmapUV = i.uv;
#else
				float2 lightmapUV = i.uv * _LightMapScaleOffset.xy + _LightMapScaleOffset.zw;
#endif

#if defined(UNITY_LIGHTMAP_FULL_HDR)
				half3 lightmap = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, lightmapUV).rgb;
#else
				half4 decodeInstructions = half4(LIGHTMAP_HDR_MULTIPLIER, LIGHTMAP_HDR_EXPONENT, 0.0h, 0.0h);
				float4 encodedIlluminance = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, lightmapUV).rgba;
				half3 lightmap = DecodeLightmap(encodedIlluminance, decodeInstructions);
#endif

				half4 output = half4(_AlbedoColor.rgb * albedo.rgb * lightmap.rgb, _AlbedoColor.a * albedo.a);
#if defined(CONVERT_TO_SRGB)
				output.rgb = LinearToSRGB(output.rgb);
#endif
				return output;
			}

			ENDHLSL
		}

		Pass
		{
			Name "LightmapCopy"

			Cull Off

			HLSLPROGRAM

			#pragma multi_compile_local _ CONVERT_TO_SRGB

			#pragma vertex VertexStage
			#pragma fragment PixelStage

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			TEXTURE2D(_LightMap);
			SAMPLER(sampler_LightMap);

			v2f VertexStage(appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.uv = v.uv;

				return o;
			}

			half4 PixelStage(v2f i) : SV_Target
			{
				float2 lightmapUV = i.uv;

#if defined(UNITY_LIGHTMAP_FULL_HDR)
				half3 lightmap = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, lightmapUV).rgb;
#else
				half4 decodeInstructions = half4(LIGHTMAP_HDR_MULTIPLIER, LIGHTMAP_HDR_EXPONENT, 0.0h, 0.0h);
				float4 encodedIlluminance = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, lightmapUV).rgba;
				half3 lightmap = DecodeLightmap(encodedIlluminance, decodeInstructions);
#endif

				half4 output = half4(lightmap.rgb, 1.0h);
#if defined(CONVERT_TO_SRGB)
				output.rgb = LinearToSRGB(output.rgb);
#endif
				return output;
			}

			ENDHLSL
		}
	}
}

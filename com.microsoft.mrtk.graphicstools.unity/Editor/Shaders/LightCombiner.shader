// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/// <summary>
/// Shader used by the LightCombinerWindow for GPU accelerated lightmap packing.
/// </summary>
Shader "Hidden/Graphics Tools/Light Combiner"
{
	Properties
	{
		_AlbedoMap("Albedo Map", 2D) = "white" {}
		_AlbedoMapScaleOffset("Albedo Map Scale Offset", Vector)  = (1, 1, 0, 0)
		_LightMap("Light Map", 2D) = "black" {}
		_LightMapScaleOffset("Light Map Scale Offset", Vector)  = (1, 1, 0, 0)
	}
	SubShader
	{
		Pass
		{
			Name "Albedo"

			Cull Off

			HLSLPROGRAM

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
				float2 uv : TEXCOORD0;
			};

			TEXTURE2D(_AlbedoMap);
			SAMPLER(sampler_AlbedoMap);
			half4 _AlbedoMapScaleOffset;

			v2f VertexStage(appdata v)
			{
				v2f o;
				o.vertex = float4(v.uv * 2 - 1, 0, 1);
				o.uv = v.uv1;

				return o;
			}

			half4 PixelStage(v2f i) : SV_Target
			{
				half4 output = half4(1, 0, 0, 1);

				float2 albedoUV = i.uv  * _AlbedoMapScaleOffset.xy + _AlbedoMapScaleOffset.zw;
				output = SAMPLE_TEXTURE2D(_AlbedoMap, sampler_AlbedoMap, albedoUV);

				return output;
			}

			ENDHLSL
		}

		Pass
		{
			Name "Lightmap"

			Blend DstColor Zero

			HLSLPROGRAM

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
			half4 _LightMapScaleOffset;

			v2f VertexStage(appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.uv = v.uv;

				return o;
			}

			half4 PixelStage(v2f i) : SV_Target
			{
				half4 output = half4(0, 0, 0, 1);

				float2 lightmapUV = i.uv  * _LightMapScaleOffset.xy + _LightMapScaleOffset.zw;
#ifdef UNITY_LIGHTMAP_FULL_HDR
				output.rgb = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, lightmapUV).rgb;
#else
				half4 decodeInstructions = half4(LIGHTMAP_HDR_MULTIPLIER, LIGHTMAP_HDR_EXPONENT, 0.0h, 0.0h);
				float4 encodedIlluminance = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, lightmapUV).rgba;
				output.rgb = DecodeLightmap(encodedIlluminance, decodeInstructions);
#endif
				return output;
			}

			ENDHLSL
		}
	}
}

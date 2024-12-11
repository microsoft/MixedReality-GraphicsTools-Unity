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
		_AlbedoMap("Albedo Map", 2D) = "white" {}
		_AlbedoMapScaleOffset("Albedo Map Scale Offset", Vector)  = (1, 1, 0, 0)

		_RemappedAlbedoMap("Remapped Albedo Map", 2D) = "white" {}
		_RemappedAlbedoMapScaleOffset("Remapped Albedo Map Scale Offset", Vector)  = (1, 1, 0, 0)
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
				float2 uv1 : TEXCOORD0;
			};

			TEXTURE2D(_AlbedoMap);
			SAMPLER(sampler_AlbedoMap);
			half4 _AlbedoMapScaleOffset;

			v2f VertexStage(appdata v)
			{
				v2f o;
				float2 uv = v.uv;
				uv.y = 1 - uv.y;
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
			Name "Lightmap"

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

			half4 _AlbedoColor;
			TEXTURE2D(_RemappedAlbedoMap);
			SAMPLER(sampler_RemappedAlbedoMap);
			half4 _RemappedAlbedoMapScaleOffset;
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
				float2 remappedAlbedoUV = i.uv  * _RemappedAlbedoMapScaleOffset.xy + _RemappedAlbedoMapScaleOffset.zw;
				half4 albedo = SAMPLE_TEXTURE2D(_RemappedAlbedoMap, sampler_RemappedAlbedoMap, remappedAlbedoUV);

				float2 lightmapUV = i.uv  * _LightMapScaleOffset.xy + _LightMapScaleOffset.zw;
				half3 lightmap = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, lightmapUV).rgb;

				return half4(_AlbedoColor.rgb * albedo.rgb * lightmap.rgb, _AlbedoColor.a * albedo.a);
			}

			ENDHLSL
		}
	}
}

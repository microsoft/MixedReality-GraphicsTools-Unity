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
			HLSLPROGRAM

			#pragma vertex VertexStage
			#pragma fragment PixelStage

			#include "UnityCG.cginc"

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

			sampler2D _AlbedoMap;
			half4 _AlbedoMapScaleOffset;
			sampler2D _LightMap;
			half4 _LightMapScaleOffset;

			v2f VertexStage(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			half4 PixelStage(v2f i) : SV_Target
			{
				half4 output;

				output = tex2D(_AlbedoMap, i.uv  * _AlbedoMapScaleOffset.xy + _AlbedoMapScaleOffset.zw);
				// TODO lightmap combine.

				return output;
			}

			ENDHLSL
		}
	}
}

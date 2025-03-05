// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Experimental/Area Light Example"
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
			#include "AreaLight.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				half3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 worldPosition : TEXCOORD1;
				half3 worldNormal : TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
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

			fixed4 frag (v2f i) : SV_Target
			{
				half3 worldPosition = i.worldPosition;
				half3 worldNormal = normalize(i.worldNormal);

				half3 lightOutput;
				CalculateAreaLights(worldPosition, worldNormal, _Color, _SpecColor, _Smoothness, lightOutput);

				return fixed4(lightOutput + _Color, 1);
			}
			ENDCG
		}
	}
}

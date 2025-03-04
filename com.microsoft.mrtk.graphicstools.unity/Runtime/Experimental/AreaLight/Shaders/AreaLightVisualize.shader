// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Hidden/Graphics Tools/Experimental/Area Light Visualize"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
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
	
			#include "UnityCG.cginc"
	
			struct appdata_t 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
	
			struct v2f 
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};
	
			sampler2D _MainTex;

			CBUFFER_START(UnityPerMaterial)
			fixed4 _Color;
			float4 _MainTex_ST;
			CBUFFER_END

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
	
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 output = tex2D(_MainTex, i.texcoord);
				UNITY_OPAQUE_ALPHA(output.a);
				return output * _Color;
			}
			ENDCG
		}
	}
}

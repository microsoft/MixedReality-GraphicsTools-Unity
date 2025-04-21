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
		Cull Off
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
			float _Facing;
			float _UVStartsAtTop;
			CBUFFER_END

			v2f vert (appdata_t input)
			{
				v2f output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				output.vertex = UnityObjectToClipPos(input.vertex);
				output.texcoord = TRANSFORM_TEX(input.texcoord, _MainTex);
				return output;
			}
	
			fixed4 frag (v2f input, bool facing : SV_IsFrontFace) : SV_Target
			{
				facing = _Facing ? !facing : facing;
				float2 texcoord = input.texcoord;
				texcoord.x = abs(_Facing - texcoord.x);
				texcoord.y = abs(_UVStartsAtTop - texcoord.y);
				fixed4 output = facing ? tex2D(_MainTex, texcoord) : fixed4(0.05, 0.05, 0.05, 1.0);
				UNITY_OPAQUE_ALPHA(output.a);
				return output * _Color;
			}
			ENDCG
		}
	}
}

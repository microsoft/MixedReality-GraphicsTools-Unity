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
			#include_with_pragmas "Packages/com.microsoft.mrtk.graphicstools.unity/Runtime/Experimental/AreaLight/Shaders/AreaLight.hlsl"

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

			v2f vert (appdata input)
			{
				v2f output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				output.vertex = UnityObjectToClipPos(input.vertex);
				output.worldPosition = mul(UNITY_MATRIX_M, input.vertex).xyz;
				output.worldNormal = UnityObjectToWorldNormal(input.normal);
				return output;
			}

			CBUFFER_START(UnityPerMaterial)
			fixed4 _Color;
			fixed4 _SpecColor;
			fixed _Smoothness;
			CBUFFER_END

			fixed4 frag (v2f input) : SV_Target
			{
				half3 worldPosition = input.worldPosition;
				half3 worldCameraPosition = _WorldSpaceCameraPos;
				half3 worldNormal = normalize(input.worldNormal);

				half3 output;
				CalculateAreaLights(worldPosition, worldCameraPosition, worldNormal, _Color, _SpecColor, _Smoothness, output);

				return fixed4(output, 1);
			}
			ENDCG
		}
	}
}

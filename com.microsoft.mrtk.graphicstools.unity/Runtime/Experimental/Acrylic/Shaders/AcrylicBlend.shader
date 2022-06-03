// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Experimental/Acrylic/Blend"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _AcrylicBlendTexture;

            half _AcrylicBlendFraction;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 color;                
                color.rgb = tex2D(_MainTex, i.uv).rgb * (1.0 - _AcrylicBlendFraction) + _AcrylicBlendFraction * tex2D(_AcrylicBlendTexture, i.uv).rgb;
                color.a = 1.0;
                return color;
            }
            ENDCG
        }
    }
}

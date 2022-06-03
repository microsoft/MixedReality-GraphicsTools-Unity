// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Experimental/Acrylic/Blit Copy" 
{
    Properties
    {
        _MainTex("Texture", any) = "" {}
        _Color("Base Color", Color) = (0.0, 0.0, 0.0, 1.0)
    }
    SubShader
    {
        Pass 
        {
            ZTest Always 
            Cull Off 
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
            uniform float4 _MainTex_ST;
            uniform float4 _Color;

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

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                fixed4 texColor = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.texcoord);
                // Traditional transparency (Blend SrcAlpha OneMinusSrcAlpha)
                fixed3 blend = (texColor.rgb * texColor.a) + (_Color.rgb * (1.0 - texColor.a));
                return fixed4(blend, _Color.a);
            }
            ENDCG
        }
    }

    Fallback Off
}

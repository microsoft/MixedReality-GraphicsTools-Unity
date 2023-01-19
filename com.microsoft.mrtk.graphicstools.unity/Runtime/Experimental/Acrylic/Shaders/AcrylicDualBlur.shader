// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Experimental/Acrylic/Dual Blur"
{
    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.universal": "12.1.0"
        }

        Tags { "RenderType"="Opaque" }
        LOD 100
        ZTest Always
        Cull Off
        ZWrite Off
        Blend Off

        HLSLINCLUDE
            //#include "UnityCG.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D_X(_AcrylicBlurSource);
            sampler sampler_AcrylicBlurSource;
            float2 _AcrylicBlurOffset;
            float2 _AcrylicHalfPixel;

            v2f vert (Attributes vertex)
            {
                v2f o;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = float4(vertex.positionOS.xyz, 1);
                o.uv = vertex.uv;
#if UNITY_UV_STARTS_AT_TOP
                o.uv.y = 1.0 - o.uv.y;
#endif
                return o;
            }

            half4 fragDown (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                half4 color;                
                color.rgb = SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv).rgb * 4.0;
                color.rgb += SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv + _AcrylicHalfPixel * _AcrylicBlurOffset).rgb;
                color.rgb += SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv - _AcrylicHalfPixel * _AcrylicBlurOffset).rgb;
                color.rgb += SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv + float2(-_AcrylicHalfPixel.x, _AcrylicHalfPixel.y) * _AcrylicBlurOffset).rgb;
                color.rgb += SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv + float2(_AcrylicHalfPixel.x, -_AcrylicHalfPixel.y) * _AcrylicBlurOffset).rgb;

                color.rgb *= 0.125;
                color.a = 1.0;
                return color;
            }

            half4 fragUp(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                half4 color;
                color.rgb = SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv + float2(-_AcrylicHalfPixel.x * 2.0, 0.0) * _AcrylicBlurOffset).rgb;
                color.rgb += SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv + float2(-_AcrylicHalfPixel.x, _AcrylicHalfPixel.y) * _AcrylicBlurOffset).rgb * 2.0;
                color.rgb += SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv + float2(0.0, _AcrylicHalfPixel.y * 2.0) * _AcrylicBlurOffset).rgb;
                color.rgb += SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv + float2(_AcrylicHalfPixel.x, _AcrylicHalfPixel.y) * _AcrylicBlurOffset).rgb * 2.0;
                color.rgb += SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv + float2(_AcrylicHalfPixel.x * 2.0, 0.0) * _AcrylicBlurOffset).rgb;
                color.rgb += SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv + float2(_AcrylicHalfPixel.x, -_AcrylicHalfPixel.y) * _AcrylicBlurOffset).rgb * 2.0;
                color.rgb += SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv + float2(0.0, -_AcrylicHalfPixel.y * 2.0) * _AcrylicBlurOffset).rgb;
                color.rgb += SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, i.uv + float2(-_AcrylicHalfPixel.x, -_AcrylicHalfPixel.y) * _AcrylicBlurOffset).rgb * 2.0;
                color.rgb *= (1.0 / 12.0);
                color.a = 1.0;
                return color;
            }

        ENDHLSL

        Pass // 0: sample down to lower res
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragDown
            ENDHLSL
        }

        Pass // 1: sample up to higher res
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragUp
            ENDHLSL
        }
    }

    Fallback "Hidden/InternalErrorShader"
}

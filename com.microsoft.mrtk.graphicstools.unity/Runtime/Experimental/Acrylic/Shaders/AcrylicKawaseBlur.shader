// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Experimental/Acrylic/Kawase Blur"
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

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            half4 frag(v2f input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 color;
                color.rgb = SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, input.uv + _AcrylicBlurOffset).rgb
                          + SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, input.uv + float2(-_AcrylicBlurOffset.x, _AcrylicBlurOffset.y)).rgb
                          + SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, input.uv + float2(_AcrylicBlurOffset.x, -_AcrylicBlurOffset.y)).rgb
                          + SAMPLE_TEXTURE2D_X(_AcrylicBlurSource, sampler_AcrylicBlurSource, input.uv - _AcrylicBlurOffset).rgb;
                color.rgb *= 0.25;
                color.a = 1.0;
                return color;
            }
            ENDHLSL
        }
    }

    Fallback "Hidden/InternalErrorShader"
}

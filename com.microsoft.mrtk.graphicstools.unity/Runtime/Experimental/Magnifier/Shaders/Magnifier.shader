// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Magnifier"
{
    Properties
    {
    }
    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.universal": "12.1.0"
        }

        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            half MagnifierMagnification;
            float4 MagnifierCenter;

            TEXTURE2D_X(MagnifierTexture);
            SAMPLER(samplerMagnifierTexture);

            float2 ZoomIn(float2 uv, float zoomAmount, float2 zoomCenter)
            {
                return ((uv - zoomCenter) * zoomAmount) + zoomCenter;
            }

            v2f vert(appdata v)
            {
                v2f o = (v2f)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = TransformObjectToHClip(v.vertex.xyz);

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.vertex);
                float2 normalizedScreenSpaceUVStereo = UnityStereoTransformScreenSpaceTex(normalizedScreenSpaceUV);
                float2 zoomedUv = ZoomIn(normalizedScreenSpaceUVStereo, MagnifierMagnification, MagnifierCenter.xy);

                return SAMPLE_TEXTURE2D_X(MagnifierTexture, samplerMagnifierTexture, zoomedUv);
            }
           ENDHLSL
        }
    }

    Fallback "Hidden/InternalErrorShader"
}
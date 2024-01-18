// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Magnifier"
{
    Properties
    {
        [Toggle(_ROUND_CORNERS)] _RoundCorners("Round Corners", Float) = 1.0
        _RoundCornerRadius("Round Corner Radius", Float) = 0.05
        _RoundCornerMargin("Round Corner Margin", Float) = 0.005
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
            "DisableBatching" = "True"
        }

        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha, One One

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define _URP
            #define _TRANSPARENT
            #define _EDGE_SMOOTHING_AUTOMATIC

            #pragma shader_feature_local _ROUND_CORNERS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.microsoft.mrtk.graphicstools.unity/Runtime/Shaders/GraphicsToolsCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _RoundCornerRadius;
                float _RoundCornerMargin;
            CBUFFER_END

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 scale : TEXCOORD3;

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
                o.uv0 = v.uv0;
                o.scale = GTGetWorldScale();

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Rounded corner clipping.
#if defined(_ROUND_CORNERS)
                half2 distanceToEdge;
                distanceToEdge.x = abs(i.uv0.x - 0.5h) * 2.0h;
                distanceToEdge.y = abs(i.uv0.y - 0.5h) * 2.0h;

                float2 halfScale = i.scale.xy * 0.5;
                float2 cornerPosition = distanceToEdge * halfScale;
                float cornerCircleRadius = max(_RoundCornerRadius, GT_MIN_CORNER_VALUE);
                float2 cornerCircleDistance = halfScale - _RoundCornerMargin - cornerCircleRadius;
                float roundCornerClip = GTRoundCorners(cornerPosition, cornerCircleDistance, cornerCircleRadius, 0.0);
#endif // _ROUND_CORNERS

                float2 normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.vertex);
                float2 normalizedScreenSpaceUVStereo = UnityStereoTransformScreenSpaceTex(normalizedScreenSpaceUV);
                float2 zoomedUv = ZoomIn(normalizedScreenSpaceUVStereo, MagnifierMagnification, MagnifierCenter.xy);

                half4 output = SAMPLE_TEXTURE2D_X(MagnifierTexture, samplerMagnifierTexture, zoomedUv);

#if defined(_ROUND_CORNERS)
                output.a = roundCornerClip;
#endif // _ROUND_CORNERS

                return output;
            }
           ENDHLSL
        }
    }

    Fallback "Hidden/InternalErrorShader"
}
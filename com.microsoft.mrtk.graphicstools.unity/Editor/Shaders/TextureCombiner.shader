// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/// <summary>
/// Shader used by the TextureCombinerWindow for GPU accelerated channel packing.
/// </summary>
Shader "Hidden/Graphics Tools/Texture Combiner"
{
    Properties
    {
        _MetallicMap("Metallic Map", 2D) = "black" {}
        _MetallicMapChannel("Metallic Map Channel", Int) = 0 // Red.
        _MetallicUniform("Metallic Uniform", Float) = -0.01
        _OcclusionMap("Occlusion Map", 2D) = "white" {}
        _OcclusionMapChannel("Occlusion Map Channel", Int) = 1 // Green.
        _OcclusionUniform("Occlusion Uniform", Float) = -0.01
        _EmissionMap("Emission Map", 2D) = "black" {}
        _EmissionMapChannel("Emission Map Channel", Int) = 4 // RGBAverage.
        _EmissionUniform("Emission Uniform", Float) = -0.01
        _SmoothnessMap("Smoothness Map", 2D) = "gray" {}
        _SmoothnessMapChannel("Smoothness Map Channel", Int) = 3 // Alpha.
        _SmoothnessUniform("Smoothness Uniform", Float) = -0.01
    }
    SubShader
    {
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertexStage
            #pragma fragment PixelStage

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MetallicMap;
            int _MetallicMapChannel;
            float _MetallicUniform;
            sampler2D _OcclusionMap;
            int _OcclusionMapChannel;
            float _OcclusionUniform;
            sampler2D _EmissionMap;
            int _EmissionMapChannel;
            float _EmissionUniform;
            sampler2D _SmoothnessMap;
            int _SmoothnessMapChannel;
            float _SmoothnessUniform;

            v2f VertexStage(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            half4 ToGrayScale(half4 color)
            {
                return color.r * 0.21 + color.g * 0.71 + color.b * 0.08;
            }

            half Sample(half4 color, int channel, float uniformValue)
            {
                if (uniformValue >= 0.0)
                {
                    return uniformValue;
                }

                if (channel == 4)
                {
                    return ToGrayScale(color);
                }

                return color[channel];
            }

            half4 PixelStage(v2f i) : SV_Target
            {
                half4 output;

                output.r = Sample(tex2D(_MetallicMap, i.uv), _MetallicMapChannel, _MetallicUniform);
                output.g = Sample(tex2D(_OcclusionMap, i.uv), _OcclusionMapChannel, _OcclusionUniform);
                output.b = Sample(tex2D(_EmissionMap, i.uv), _EmissionMapChannel, _EmissionUniform);
                output.a = Sample(tex2D(_SmoothnessMap, i.uv), _SmoothnessMapChannel, _SmoothnessUniform);

                return output;
            }

            ENDHLSL
        }
    }
}

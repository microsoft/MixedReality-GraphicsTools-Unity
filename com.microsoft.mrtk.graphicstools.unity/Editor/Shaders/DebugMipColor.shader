// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/// <summary>
/// Tints a pixel based on the texel to pixel ratio using Unity's GetDebugMipColor method.
///  - Original color means it’s a perfect match (1:1 texels to pixels ratio at the current distance and resolution).
///  - Red indicates that the texture is larger than necessary.
///  - Blue indicates that the texture could be larger.
/// Note, the ideal texture sizes depend on the resolution at which your application will run and how close the camera can get to a surface.
/// 
/// Note, the signature of GetDebugMipColor changed in 12.1.0 so we need two sub shaders.
/// </summary>
Shader "Hidden/Graphics Tools/DebugMipColor"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.universal": "[10.6.0,12.1.0)"
        }

        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertexStage
            #pragma fragment PixelStage

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Debug.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            CBUFFER_END

            Varyings VertexStage(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.position = TransformObjectToHClip(input.vertex.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);

                return output;
            }

            half4 PixelStage(Varyings input) : SV_Target
            {
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                return half4(GetDebugMipColor(albedo.rgb, _MainTex, _MainTex_TexelSize, input.uv), albedo.a);
            }

            ENDHLSL
        }
    }
    SubShader
        {
            PackageRequirements
            {
                "com.unity.render-pipelines.universal": "12.1.0"
            }
    
            Tags { "RenderType" = "Opaque" }
            LOD 100
    
            Pass
            {
                HLSLPROGRAM
    
                #pragma vertex VertexStage
                #pragma fragment PixelStage
    
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Debug.hlsl"
    
                struct Attributes
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };
    
                struct Varyings
                {
                    float4 position : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };
    
                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
    
                CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                CBUFFER_END
    
                Varyings VertexStage(Attributes input)
                {
                    Varyings output;
                    UNITY_SETUP_INSTANCE_ID(input);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                    output.position = TransformObjectToHClip(input.vertex.xyz);
                    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
    
                    return output;
                }
    
                half4 PixelStage(Varyings input) : SV_Target
                {
                    half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                    return half4(GetDebugMipColor(albedo.rgb, _MainTex_TexelSize, input.uv), albedo.a);
                }
    
                ENDHLSL
            }
        }

    Fallback "Hidden/InternalErrorShader"
}

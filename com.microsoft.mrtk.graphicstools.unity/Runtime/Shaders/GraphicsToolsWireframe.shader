// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/// <summary>
/// Displays a mesh's triagles based on the approach described in Shader-Based Wireframe Drawing (2008)
/// http://orbit.dtu.dk/en/publications/id(13e2122d-bec7-48de-beca-03ce6ea1c3f1).html
/// </summary>
Shader "Graphics Tools/Wireframe"
{
    Properties
    {
        // Rendering options.
        _BaseColor("Base color", Color) = (0.0, 0.0, 0.0, 1.0)
        _WireColor("Wire color", Color) = (1.0, 1.0, 1.0, 1.0)
        _WireThickness("Wire thickness", Range(0, 800)) = 100

        // Advanced options.
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.RenderingMode)] _Mode("Rendering Mode", Float) = 0 // "Opaque"
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.RenderingMode)] _CustomMode("Mode", Float) = 0     // "Opaque"
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Float) = 1                         // "One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend", Float) = 0                    // "Zero"
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlendAlpha("Source Blend Alpha", Float) = 1              // "One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlendAlpha("Destination Blend Alpha", Float) = 1         // "One"
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("Blend Operation", Float) = 0                         // "Add"
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Depth Test", Float) = 4                        // "LessEqual"
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.DepthWrite)] _ZWrite("Depth Write", Float) = 1     // "On"
        _ZOffsetFactor("Depth Offset Factor", Float) = 0                                                     // "Zero"
        _ZOffsetUnits("Depth Offset Units", Float) = 0                                                       // "Zero"
        [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask("Color Write Mask", Float) = 15         // "All"
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Float) = 2                             // "Back"
        _RenderQueueOverride("Render Queue Override", Range(-1.0, 5000)) = -1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            Name "Main"
            Blend[_SrcBlend][_DstBlend],[_SrcBlendAlpha][_DstBlendAlpha]
            BlendOp[_BlendOp]
            ZTest[_ZTest]
            ZWrite[_ZWrite]
            Cull[_CullMode]
            Offset[_ZOffsetFactor],[_ZOffsetUnits]
            ColorMask[_ColorWriteMask]

            HLSLPROGRAM

            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #pragma multi_compile_local _ _CLIPPING_PLANE _CLIPPING_SPHERE _CLIPPING_BOX

            #if defined(_CLIPPING_PLANE) || defined(_CLIPPING_SPHERE) || defined(_CLIPPING_BOX)
                #define _CLIPPING_PRIMITIVE
            #else
                #undef _CLIPPING_PRIMITIVE
            #endif

            #include "UnityCG.cginc"
            #include "GraphicsToolsCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float4 _WireColor;
            float _WireThickness;
            CBUFFER_END

            struct v2g
            {
                float4 viewPos : SV_POSITION;
#if defined(_CLIPPING_PRIMITIVE)
                float3 worldPos : TEXCOORD0;
#endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2g vert(appdata_base v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2g o;
                o.viewPos = UnityObjectToClipPos(v.vertex);
#if defined(_CLIPPING_PRIMITIVE)
                o.worldPos = mul(UNITY_MATRIX_M, v.vertex).xyz;
#endif
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                return o;
            }

            // inverseW is to counteract the effect of perspective-correct interpolation so that the lines
            // look the same thickness regardless of their depth in the scene.
            struct g2f
            {
                float4 viewPos : SV_POSITION;
                float inverseW : TEXCOORD0;
                float3 dist : TEXCOORD1;
#if defined(_CLIPPING_PRIMITIVE)
                float3 worldPos : TEXCOORD2;
#endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            [maxvertexcount(3)]
            void geom(triangle v2g i[3], inout TriangleStream<g2f> triStream)
            {
                // Calculate the vectors that define the triangle from the input points.
                float2 point0 = i[0].viewPos.xy / i[0].viewPos.w;
                float2 point1 = i[1].viewPos.xy / i[1].viewPos.w;
                float2 point2 = i[2].viewPos.xy / i[2].viewPos.w;

                // Calculate the area of the triangle.
                float2 vector0 = point2 - point1;
                float2 vector1 = point2 - point0;
                float2 vector2 = point1 - point0;
                float area = abs(vector1.x * vector2.y - vector1.y * vector2.x);

                float3 distScale[3];
                distScale[0] = float3(area / length(vector0), 0, 0);
                distScale[1] = float3(0, area / length(vector1), 0);
                distScale[2] = float3(0, 0, area / length(vector2));

                float wireScale = 800 - _WireThickness;

                // Output each original vertex with its distance to the opposing line defined
                // by the other two vertices.
                g2f o;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                [unroll]
                for (uint idx = 0; idx < 3; ++idx)
                {
                   o.viewPos = i[idx].viewPos;
                   o.inverseW = 1.0 / o.viewPos.w;
                   o.dist = distScale[idx] * o.viewPos.w * wireScale;
#if defined(_CLIPPING_PRIMITIVE)
                   o.worldPos = i[idx].worldPos;
#endif
                   UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(i[idx], o);
                   triStream.Append(o);
                }
            }

            float4 frag(g2f i) : COLOR
            {
#if defined(_CLIPPING_PRIMITIVE)
                ClipAgainstPrimitive(i.worldPos);
#endif

                // Calculate  minimum distance to one of the triangle lines, making sure to correct
                // for perspective-correct interpolation.
                float dist = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.inverseW;

                // Make the intensity of the line very bright along the triangle edges but fall-off very
                // quickly.
                float I = exp2(-2 * dist * dist);

                return I * _WireColor + (1 - I) * _BaseColor;
            }

            ENDHLSL
        }
    }

    CustomEditor "Microsoft.MixedReality.GraphicsTools.Editor.WireframeShaderGUI"
}

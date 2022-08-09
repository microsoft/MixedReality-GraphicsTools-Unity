// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

Shader "Graphics Tools/Samples/Fish"
{
    Properties
    {
        [Header(Material)]
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex("Albedo", 2D) = "white" {}

        [Header(Animation)]
         _SwimSpeed("Swim Speed", Float) = 200.0
         _SwimOffset("Swim Offset", Float) = 20.0
         _SwimMagnitude("Swim Magnitude", Float) = 0.1
         _SwimAttenuation("Swim Attenuation", Float) = 0.1
    }

    SubShader
    {
        Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertexStage
            #pragma fragment PixelStage

            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            /// <summary>
            /// Vertex attributes passed into the vertex shader from the app.
            /// </summary>
            struct Attributes
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            /// <summary>
            /// Vertex attributes interpolated across a triangle and sent from the vertex shader to the fragment shader.
            /// </summary>
            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            /// <summary>
            /// Properties.
            /// </summary>
            CBUFFER_START(UnityPerMaterial)
#if defined(UNITY_INSTANCING_ENABLED)
                half4 _ColorUnused;     // Defined in the PerMaterialInstanced constant buffer.
                float _SwimSpeedUnused; // Defined in the PerMaterialInstanced constant buffer.
#else
                half4 _Color;
                float _SwimSpeed;
#endif
                float _SwimOffset;
                float _SwimMagnitude;
                float _SwimAttenuation;
            CBUFFER_END

#if defined(UNITY_INSTANCING_ENABLED)
            UNITY_INSTANCING_BUFFER_START(PerMaterialInstanced)
                UNITY_DEFINE_INSTANCED_PROP(half4, _Color)
                UNITY_DEFINE_INSTANCED_PROP(float, _SwimSpeed)
            UNITY_INSTANCING_BUFFER_END(PerMaterialInstanced)
#endif

            sampler2D _MainTex;

            /// <summary>
            /// Vertex shader.
            /// </summary>
            Varyings VertexStage(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                // Swiming animation.
                float phase = _Time * UNITY_ACCESS_INSTANCED_PROP(PerMaterialInstanced, _SwimSpeed);
                float offset = input.vertex.z * _SwimOffset;
                input.vertex.x += (sin(phase + offset) * _SwimMagnitude) * saturate(_SwimAttenuation - input.vertex.z);

                output.position = UnityObjectToClipPos(input.vertex);
                output.texcoord = input.texcoord;
                output.color = UNITY_ACCESS_INSTANCED_PROP(PerMaterialInstanced, _Color);

                return output;
            }

            /// <summary>
            /// Pixel shader.
            /// </summary>
            half4 PixelStage(Varyings input) : SV_Target
            {
                half4 albedo = tex2D(_MainTex, input.texcoord);
                return half4(lerp(albedo.rgb, albedo.rgb * input.color.rgb, albedo.a), 1.0);
            }

            ENDHLSL
        }
    }
}

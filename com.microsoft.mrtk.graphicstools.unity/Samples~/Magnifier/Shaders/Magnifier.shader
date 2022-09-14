// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Magnifier"
{
    Properties
    { 
        _Magnification("Magnification", Float) = 0.5
        [ShowAsVector2]  Center("Center", Vector) = (0.5,0.5,0,0)
    }
    SubShader
    {
        // thmicka: Tagged the material as "Transparent" so that it renders later in the frame.
        // This isn't technically needed, just helped me debug via the Unity Frame Debugger. Later we will want to control this via a render feature.      
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

            struct v2f  //vertex to fragment
            {
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            half _Magnification;
            float2 Center;
           
            TEXTURE2D_X(_MagnifierTexture);
            SAMPLER(sampler_MagnifierTexture);
           
            float2 zoomIn(float2 uv, float zoomAmount, float2 zoomCenter)
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

                float zoomAmount = _Magnification;

                // zoomCenter expects normalized coordinates (between 0 and 1)
                float2 zoomCenter = Center;

                float2 zoomedUv = zoomIn(normalizedScreenSpaceUVStereo, zoomAmount, zoomCenter);
               
                float4 output = SAMPLE_TEXTURE2D_X(_MagnifierTexture, sampler_MagnifierTexture, zoomedUv);
                                
              return output;
            }
           ENDHLSL
        }
    }
}
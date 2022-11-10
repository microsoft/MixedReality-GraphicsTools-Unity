// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Magnifier"
{
    Properties
    { 
        Magnification("Magnification", Float) = 0.5
        [ShowAsVector2]  Center("Center", Vector) = (0.5,0.5,0,0)
    }
    SubShader
    {
       
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
            
            half Magnification;
            float2 Center;
           
            TEXTURE2D_X(MagnifierTexture);
            SAMPLER(samplerMagnifierTexture);
           
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

                // zoomCenter expects normalized coordinates (between 0 and 1)
                float2 zoomCenter = Center;

                float2 zoomedUv = zoomIn(normalizedScreenSpaceUVStereo, Magnification, zoomCenter);
               
                float4 output = SAMPLE_TEXTURE2D_X(MagnifierTexture, samplerMagnifierTexture, zoomedUv);
                                
              return output;
            }
           ENDHLSL
        }
    }
}
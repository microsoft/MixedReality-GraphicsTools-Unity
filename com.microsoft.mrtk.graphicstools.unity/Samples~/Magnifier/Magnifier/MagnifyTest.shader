Shader "Unlit/MagnifyTest"
{
    Properties
    { 
        _Magnification("Magnification", Float) = 0.5
      [ShowAsVector2]  Center("Center", Vector) = (0,0,0,0)
    }
    SubShader
    {
        // thmicka: Tagged the material as "Transparent" so that it renders later in the frame.
        // This isn't technically needed, just helped me debug via the Unity Frame Debugger. Later we will want to control this via a render feature.
        // TODO: Setup system to render the magnifier at the end of the frame, likely via a "Render Objects Renderer Feature"
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
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
           // TEXTURE2D_X(_CameraOpaqueTexture);
            TEXTURE2D_X(_GrabPassTransparent);
           // SAMPLER(sampler_CameraOpaqueTexture);
            SAMPLER(sampler_GrabPassTransparent);
            float2 zoomIn(float2 uv, float zoomAmount, float2 zoomCenter)
            {
                return ((uv - zoomCenter) * zoomAmount) + zoomCenter;
            }

            v2f vert(appdata v)
            {
                v2f o = (v2f)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = TransformObjectToHClip(v.vertex);

                return o;
            }

           

            half4 frag(v2f i) : SV_Target
            {
                // thmicka: I didn't know Unity had this method. But it does the clip space to screen space transforamtion for us: https://github.com/Unity-Technologies/Graphics/blob/632f80e011f18ea537ee6e2f0be3ff4f4dea6a11/Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ShaderVariablesFunctions.hlsl#L266
                float2 normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.vertex);

                // thmicka: Call UnityStereoTransformScreenSpaceTex too ennsure this works with single pass stereo rendering: https://github.com/Unity-Technologies/Graphics/blob/223d8105e68c5a808027d78f39cb4e2545fd6276/Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl#L499
                float2 normalizedScreenSpaceUVStereo = UnityStereoTransformScreenSpaceTex(normalizedScreenSpaceUV);

                float zoomAmount = _Magnification;

                // comment this to disable zoom animation
                //zoomAmount = abs(sin(iTime));

                // zoomCenter expects normalized coordinates (between 0 and 1)
                float2 zoomCenter = Center;

                float2 zoomedUv = zoomIn(normalizedScreenSpaceUVStereo, zoomAmount, zoomCenter);

                // thmicka: Sample the _CameraOpaqueTexture like we were. Chuks: swapped _CameraOpaqueTexture for the _GrabPassTransparent texture array
                // TODO: Sample a custom camera texture where we can control when it gets generated.
                float4 output = SAMPLE_TEXTURE2D_X(_GrabPassTransparent, sampler_GrabPassTransparent, zoomedUv);
               
                // thmicka: Just tinting the output "red" so that I can see what is rendered. Else, it's hard to see the quad. 
                // In the future this won't be an issue because we will be zooming in/out. TODO: just return "output"
              return output;
            }
           ENDHLSL
        }
    }
}
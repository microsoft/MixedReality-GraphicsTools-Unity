Shader "Unlit/MagnifyTest"
{
    Properties
    {
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
       

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
                float4 uv : TEXCOORD0;
               
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

           

            v2f vert (appdata v)
            {   
                v2f o = (v2f)0;
                UNITY_SETUP_INSTANCE_ID(v); 
                
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
       
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = ComputeScreenPos(o.vertex);
               
                return o;
            }

TEXTURE2D_X(_CameraOpaqueTexture);
SAMPLER(sampler_CameraOpaqueTexture);

            half4 frag (v2f i) : SV_Target
            {
                float2 uvScreen = i.uv.xy / i.uv.w;
                return SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uvScreen);
                //fixed4(i.uv.x,i.uv.y,0,1.0);
            }
           ENDHLSL
        }
    }
}

Shader "ColorBlit"
{Properties {
		[HideInInspector] 
		_MainTex ("Main", 2D) = "white" {}
        CenterRadial ("Radial", Vector) = (0, 0, 0, 0)
		Amount      ("Amount", Float) = 0.85
		RadiusInner  ("Radius Inner", Float) = 0.2
		RadiusOuter  ("Radius Outer", Float) = 0.27
		
	}
  // HLSLPROGRAM
   // #include "UnityCG.cginc"
	
	//ENDHLSL
        SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off ZTest Off Blend Off
        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag_Single
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionHCS   : POSITION;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                float2  uv          : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                // Note: The pass is setup with a mesh already in clip
                // space, that's why, it's enough to just output vertex
                // positions
                output.positionCS = float4(input.positionHCS.xyz, 1.0);

                #if UNITY_UV_STARTS_AT_TOP
                output.positionCS.y *= -1;
                #endif

                output.uv = input.uv;
                return output;
            }

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float _Intensity;
             
    sampler2D _MainTex;	
	float4 CenterRadial;
	float RadiusInner;
	float RadiusOuter;
	float Amount;
    bool MagnifyingGlassIsInCircle(float2 uv, float4 cr, float radiusOuter)
{
	float2 ray = uv - cr.xy;
	float len = length(ray / cr.zw);
	if (len < radiusOuter)
		return true;
	else
		return false;
}

float4 MagnifyingGlassSampleTexture(float2 uv, float4 cr, float amount, float radiusInner, float radiusOuter)
{
	float2 ray = uv - cr.xy;
	float len = length(ray / cr.zw);
	
	float2 newUV;
	float amt = 1 - amount;
	if (len < radiusInner)
	{
		newUV = cr.xy + (ray * amt);
	}
	else
	{
		float diff = radiusOuter - radiusInner;
		float ratio = (len - radiusInner) / diff;   // 0 ~ 1
		ratio = ratio * 3.14159;                    // -pi/2 ~ pi/2
		float c = cos(ratio);                       // -1 ~ 1
		c = c + 1;                                  // 0 ~ 2
		c = c / 2;                                  // 0 ~ 1
		newUV = ((cr.xy + ray * amt) * c) + (uv * (1 - c));
	}
	return SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture,newUV);
      //UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex,newUV);
   
}
		
float4 frag_Single(Varyings i) : SV_Target
		{
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i)
			float4 c;
			if (MagnifyingGlassIsInCircle (i.uv, CenterRadial, RadiusOuter))
				c = MagnifyingGlassSampleTexture (i.uv, CenterRadial, Amount, RadiusInner, RadiusOuter);
			else
				c = tex2D(_MainTex, i.uv);
			return c;
       	}
		
          /*  half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float4 color = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, input.uv);
                return color * float4(0, _Intensity, 0, 1);
            }*/
            ENDHLSL
        }
    }
}
Shader "Magnifying Glass/Circle" {
	Properties {
		[HideInInspector] 
		_MainTex ("Main", 2D) = "white" {}
        CenterRadial ("Radial", Vector) = (0, 0, 0, 0)
		Amount      ("Amount", Float) = 0.85
		RadiusInner  ("Radius Inner", Float) = 0.2
		RadiusOuter  ("Radius Outer", Float) = 0.27
		
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _MainTex;	
	float4 CenterRadial;
	float RadiusInner;
	float RadiusOuter;
	float Amount;
bool MagnifyingGlassIsInCircle (float2 uv, float4 cr, float radiusOuter)
{
	float2 ray = uv - cr.xy;
	float len = length(ray / cr.zw);
	if (len < radiusOuter)
		return true;
	else
		return false;
}
float4 MagnifyingGlassSampleTexture (float2 uv, float4 cr, float amount, float radiusInner, float radiusOuter)
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
	return  UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, newUV);
}
		
float4 frag_Single (v2f_img i) : SV_Target
		{
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i)
			float4 c;
			if (MagnifyingGlassIsInCircle (i.uv, CenterRadial, RadiusOuter))
				c = MagnifyingGlassSampleTexture (i.uv, CenterRadial, Amount, RadiusInner, RadiusOuter);
			else
				c = tex2D(_MainTex, i.uv);
			return c;
       	}
		
	ENDCG
	SubShader {
		ZTest Off Cull Off ZWrite Off Blend Off
	  	Fog { Mode off }	
		Pass {  
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag_Single

			ENDCG
		}
		
	}
	FallBack Off
}
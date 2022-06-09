#ifndef UTILS_CGINC
#define UTILS_CGINC

#include "UnityCG.cginc"

//tex2D _MainTex;
UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);

#define MG_DECLARE_UNIFORM(n)\
	
	float4 _ComplicatedCenterRadial##n;\
	float  _ComplicatedAmount##n;\
	float  _ComplicatedRadiusInner##n;\
	float  _ComplicatedRadiusOuter##n;



bool MagnifyingGlassComplicated_IsInCircle (float2 uv, float4 cr, float radiusOuter)
{
	float2 ray = uv - cr.xy;
	float len = length(ray / cr.zw);
	if (len < radiusOuter)
		return true;
	else
		return false;
}
float4 MagnifyingGlassComplicated_SampleTexture (float2 uv, float4 cr, float amount, float radiusInner, float radiusOuter)
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
	return UNITY_SAMPLE_SCREENSPACE_TEXTURE (_MainTex, newUV);
	//return tex2D(_MainTex, newUV);
}

#endif
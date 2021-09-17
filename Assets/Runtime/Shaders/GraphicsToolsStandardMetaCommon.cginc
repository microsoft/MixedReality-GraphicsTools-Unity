// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef GRAPHICS_TOOLS_STANDARD_META_COMMON
#define GRAPHICS_TOOLS_STANDARD_META_COMMON

#pragma vertex vert
#pragma fragment frag

/// <summary>
/// Features.
/// </summary>

#pragma shader_feature EDITOR_VISUALIZATION
#pragma shader_feature _EMISSION
#pragma shader_feature _CHANNEL_MAP

/// <summary>
/// Inludes.
/// </summary>
/// 
#include "UnityCG.cginc"
#include "UnityMetaPass.cginc"

/// <summary>
/// Vertex attributes interpolated across a triangle and sent from the vertex shader to the fragment shader.
/// </summary>
struct v2f
{
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0;
};

/// <summary>
/// Properties.
/// </summary>
sampler2D _MainTex;
sampler2D _ChannelMap;
float4 _MainTex_ST;

fixed4 _Color;
fixed4 _EmissiveColor;

#if defined(_RENDER_PIPELINE)
CBUFFER_START(_LightBuffer)
float4 _MainLightPosition;
half4 _MainLightColor;
CBUFFER_END
#else
fixed4 _LightColor0;
#endif

/// <summary>
/// Vertex shader entry point.
/// </summary>
v2f vert(appdata_full v)
{
    v2f o;
    o.vertex = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

    return o;
}

/// <summary>
/// Fragment (pixel) shader entry point.
/// </summary>
half4 frag(v2f i) : SV_Target
{
    UnityMetaInput output;
    UNITY_INITIALIZE_OUTPUT(UnityMetaInput, output);

    output.Albedo = tex2D(_MainTex, i.uv) * _Color;
#if defined(_EMISSION)
#if defined(_CHANNEL_MAP)
    output.Emission += tex2D(_ChannelMap, i.uv).b * _EmissiveColor;
#else
    output.Emission += _EmissiveColor;
#endif
#endif
#if defined(_RENDER_PIPELINE)
    output.SpecularColor = _MainLightColor.rgb;
#else
    output.SpecularColor = _LightColor0.rgb;
#endif

    return UnityMetaFragment(output);
}

#endif // GRAPHICS_TOOLS_STANDARD_META_COMMON

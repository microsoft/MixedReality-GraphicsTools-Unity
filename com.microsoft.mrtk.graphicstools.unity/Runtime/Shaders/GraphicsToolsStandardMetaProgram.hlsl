// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef GT_STANDARD_META_PROGRAM
#define GT_STANDARD_META_PROGRAM

#pragma vertex VertexStage
#pragma fragment PixelStage

/// <summary>
/// Features.
/// </summary>

#pragma shader_feature EDITOR_VISUALIZATION
#pragma shader_feature_local_fragment _EMISSION
#pragma shader_feature_local _CHANNEL_MAP

/// <summary>
/// Inludes.
/// </summary>

#if defined(_URP)
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
#else
#include "UnityCG.cginc"
#include "UnityMetaPass.cginc"
#endif

#include "GraphicsToolsStandardInput.hlsl"

/// <summary>
/// Vertex attributes passed into the vertex shader from the app.
/// </summary>
struct MetaAttributes
{
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
    float4 texcoord2 : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

/// <summary>
/// Vertex attributes interpolated across a triangle and sent from the vertex shader to the fragment shader.
/// </summary>
struct MetaVaryings
{
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0;
};

/// <summary>
/// Vertex shader entry point.
/// </summary>
MetaVaryings VertexStage(MetaAttributes input)
{
    MetaVaryings output;
#if defined(_URP)
    output.vertex = MetaVertexPosition(input.vertex, input.texcoord1.xy, input.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#else
    output.vertex = UnityMetaVertexPosition(input.vertex, input.texcoord1.xy, input.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
    output.vertex = UnityObjectToClipPos(input.vertex);
#endif
    output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);

    return output;
}

/// <summary>
/// Fragment (pixel) shader entry point.
/// </summary>
half4 PixelStage(MetaVaryings input) : SV_Target
{
#if defined(_URP)
    MetaInput output = (MetaInput)0;
    output.Albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb * _Color.rgb;
#if defined(_EMISSION)
#if defined(_CHANNEL_MAP)
    output.Emission = SAMPLE_TEXTURE2D(_ChannelMap, sampler_ChannelMap, input.uv).rgb * _EmissiveColor.rgb;
#else
    output.Emission = SAMPLE_TEXTURE2D(_EmissiveMap, sampler_EmissiveMap, input.uv).rgb * _EmissiveColor.rgb;
#endif
#endif

    return MetaFragment(output);
#else
    UnityMetaInput output = (UnityMetaInput)0;
    output.Albedo = tex2D(_MainTex, input.uv) * _Color;
#if defined(_EMISSION)
#if defined(_CHANNEL_MAP)
    output.Emission += tex2D(_ChannelMap, input.uv).b * _EmissiveColor;
#else
    output.Emission = tex2D(_EmissiveMap, input.uv) * _EmissiveColor;
#endif
#endif
    output.SpecularColor = _LightColor0.rgb;

    return UnityMetaFragment(output);
#endif
}

#endif // GT_STANDARD_META_PROGRAM

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef GT_SHADOW_PASS
#define GT_SHADOW_PASS

// Used by GraphicsToolsLighting.hlsl to get light direction?
#define _SHADOW_PASS

// Uncomment to help with RenderDoc debugging.
#pragma enable_d3d11_debug_symbols

#pragma vertex ShadowPassVertexStage
#pragma fragment ShadowPassPixelStage
#pragma shader_feature_local _USE_WORLD_SCALE

/// <summary>
/// Features.
/// </summary>

//#pragma shader_feature EDITOR_VISUALIZATION
//#pragma shader_feature_local_fragment _EMISSION
//#pragma shader_feature_local _CHANNEL_MAP

#pragma shader_feature_local _ROUND_CORNERS
#pragma shader_feature_local_fragment _INDEPENDENT_CORNERS

/// <summary>
/// Inludes.
/// </summary>

#if defined(_URP)
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
#else
    #include "UnityCG.cginc"
    #include "UnityMetaPass.cginc"
#endif

#if defined(_ROUND_CORNERS)
    #define _NORMAL
#endif

#if defined(_VERTEX_EXTRUSION) || defined(_ROUND_CORNERS) || defined(_BORDER_LIGHT)
    #define _SCALE
#endif

#include "GraphicsToolsCommon.hlsl"
#include "GraphicsToolsStandardInput.hlsl"
#include "GraphicsToolsLighting.hlsl"
#include "GraphicsToolsRoundCorners.hlsl"

/// <summary>
/// Vertex attributes passed into the vertex shader from the app.
/// </summary>
struct ShadowPassAttributes
{
    float4 position : POSITION;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
    float4 texcoord2 : TEXCOORD2;
    half3 normal : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

/// <summary>
/// Vertex attributes interpolated across a triangle and sent from the vertex shader to the fragment shader.
/// </summary>
struct ShadowPassVaryings
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD0;
    half3 normal : NORMAL;
    
    #if defined(_SCALE)
        half3 scale : TEXCOORD3;
    #endif
};

float4 GT_GetShadowPositionHClip(ShadowPassAttributes input)
{
    float3 positionWS = TransformObjectToWorld(input.position.xyz);
    
    #if defined(_URP)
        half3 normalWS = TransformObjectToWorldNormal(input.normal);
    #else
        half3 normalWS = UnityObjectToWorldNormal(input.normal);
    #endif

    GTMainLight light = GTGetMainLight();
    
    #if _CASTING_PUNCTUAL_LIGHT_SHADOW
        float3 lightDirectionWS = normalize(light.direction - positionWS);
    #else
        float3 lightDirectionWS = light.direction;
    #endif

    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

    #if UNITY_REVERSED_Z
        positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #else
        positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #endif

    return positionCS;
}

float3 GT_ApplyShadowBias(float3 positionWS, float3 normalWS, float3 lightDirection)
{
    float invNdotL = 1.0 - saturate(dot(lightDirection, normalWS));
    float scale = invNdotL * _ShadowBias.y;

    // normal bias is negative since we want to apply an inset normal offset
    positionWS = lightDirection * _ShadowBias.xxx + positionWS;
    positionWS = normalWS * scale.xxx + positionWS;
    return positionWS;
}

/// <summary>
/// Vertex shader entry point.
/// </summary>
ShadowPassVaryings ShadowPassVertexStage(ShadowPassAttributes input)
{
    ShadowPassVaryings output;

    #if defined(_URP)
        output.position = MetaVertexPosition(input.position, input.texcoord1.xy, input.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
    #else
        output.position = UnityMetaVertexPosition(input.position, input.texcoord1.xy, input.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
        output.position = UnityObjectToClipPos(input.position);
    #endif

    output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
    output.position = GT_GetShadowPositionHClip(input);
    
    //
    // Normal
    half3 localNormal = input.normal;

    #if defined(_NORMAL) || defined(_VERTEX_EXTRUSION)
        #if defined(_URP)
            half3 worldNormal = TransformObjectToWorldNormal(localNormal);
        #else
            half3 worldNormal = UnityObjectToWorldNormal(localNormal);
        #endif
    #endif
            
    output.normal = localNormal;

    //
    // Scale
    #if defined(_SCALE)
        output.scale = GTGetWorldScale();
        half minScaleWS = min(min(output.scale.x, output.scale.y), output.scale.z);
        // Scale XY is used by canvas
        #if defined(_CANVAS_RENDERED)
            output.scale.x *= input.uv2.x;
            output.scale.y *= input.uv2.y;
            output.scale.z = input.uv3.x;
        #endif

        //#if defined(_USE_WORLD_SCALE)
        //    output.scale.z = minScaleWS;
        //#endif

        #if defined(_CANVAS_RENDERED)
            if (abs(localNormal.x) == 1.0) // Y,Z plane.
            {
                output.scale.x = output.scale.z;
            }
            else if (abs(localNormal.y) == 1.0) // X,Z plane.
            {
                output.scale.y = output.scale.x;
            }
            // Else X,Y plane.
        #endif

        #if defined(_USE_WORLD_SCALE)
            output.scale.z = 1;
        #else
            output.scale.z = minScaleWS;
        #endif
    #endif

    return output;
}

/// <summary>
/// Fragment (pixel) shader entry point.
/// </summary>
half4 ShadowPassPixelStage(ShadowPassVaryings input) : SV_Target
{
    half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
    half clipval = tex.a - _Cutoff;
    
    #if defined(_ROUND_CORNERS)
        half2 distanceToEdge = abs(input.uv - half2(0.5, 0.5)) * half(2.0);
        half2 halfScale = input.scale.xy * half(0.5);
        half2 cornerPosition = distanceToEdge * halfScale;
        // Store results from corner rounding
        float currentCornerRadius;
        float cornerCircleRadius;
        float2 cornerCircleDistance;
        half cornerClip;

        float minScaleWS = input.scale.z;
        float rad = _RoundCornerRadius;

       RoundCorners(
            cornerPosition.xy, input.uv.xy, minScaleWS, halfScale, _EdgeSmoothingValue, rad, _RoundCornerMargin,
            // The rest are written to...
            currentCornerRadius, cornerCircleRadius, cornerCircleDistance, cornerClip);

       // Combine rounded corners with texture alpha
       clipval -= (half(1.0) - cornerClip);

       clip(clipval);
    #endif

    return 0;
}

#endif // GT_SHADOW_PASS

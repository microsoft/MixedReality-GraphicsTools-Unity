// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef GRAPHICS_TOOLS_COMMON
#define GRAPHICS_TOOLS_COMMON

/// <summary>
/// Constants
/// </summary>

#define GRAPHICS_TOOLS_PI                 3.14159265359
#define GRAPHICS_TOOLS_TWO_PI             6.28318530718
#define GRAPHICS_TOOLS_FOUR_PI            12.56637061436
#define GRAPHICS_TOOLS_INV_PI             0.31830988618
#define GRAPHICS_TOOLS_INV_TWO_PI         0.15915494309
#define GRAPHICS_TOOLS_INV_FOUR_PI        0.07957747155
#define GRAPHICS_TOOLS_HALF_PI            1.57079632679
#define GRAPHICS_TOOLS_INV_HALF_PI        0.636619772367
#define GRAPHICS_TOOLS_DEGREES_TO_RADIANS (GRAPHICS_TOOLS_PI / 180.0)

#define GRAPHICS_TOOLS_MIN_CORNER_VALUE      1e-3
#define GRAPHICS_TOOLS_MIN_CORNER_VALUE_RECT 1e-1
#define GRAPHICS_TOOLS_MAX_NEAR_LIGHT_DIST   10.0

/// <summary>
/// Transformation methods.
/// </summary>

inline float3 GTGetWorldScale()
{
    float3 scale;
    scale.x = length(mul(UNITY_MATRIX_M, float4(1.0, 0.0, 0.0, 0.0)));
    scale.y = length(mul(UNITY_MATRIX_M, float4(0.0, 1.0, 0.0, 0.0)));
    scale.z = length(mul(UNITY_MATRIX_M, float4(0.0, 0.0, 1.0, 0.0)));
    return scale;
}

inline float GTGetWorldScaleMinAxis()
{
    float3 scale = GTGetWorldScale();
    return min(min(scale.x, scale.y), scale.z);
}

/// <summary>
/// Color space methods.
/// </summary>

inline half3 GTLinearTosRGB(half3 linRGB)
{
    linRGB = max(linRGB, half3(0.h, 0.h, 0.h));
    return max(1.055h * pow(linRGB, 0.416666667h) - 0.055h, 0.h);
}

inline half4 GTLinearTosRGB(half4 linRGB)
{
    return half4(GTLinearTosRGB(linRGB.rgb), linRGB.a);
}

inline half3 GTsRGBToLinear(half3 sRGB)
{
    return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);
}

inline half4 GTsRGBToLinear(half4 sRGB)
{
    return half4(GTsRGBToLinear(sRGB.rgb), sRGB.a);
}

/// <summary>
/// Lighting methods.
/// </summary>

inline float GTNearLightDistance(float4 light, float3 worldPosition)
{
    return distance(worldPosition, light.xyz) + ((1.0 - light.w) * GRAPHICS_TOOLS_MAX_NEAR_LIGHT_DIST);
}

inline float GTHoverLight(float4 hoverLight, float inverseRadius, float3 worldPosition)
{
    return (1.0 - saturate(length(hoverLight.xyz - worldPosition) * inverseRadius)) * hoverLight.w;
}

inline float GTProximityLight(float4 proximityLight, float4 proximityLightParams, float4 proximityLightPulseParams, float3 worldPosition, float3 worldNormal, out half colorValue)
{
    float proximityLightDistance = dot(proximityLight.xyz - worldPosition, worldNormal);
#if defined(_PROXIMITY_LIGHT_TWO_SIDED)
    worldNormal = proximityLightDistance < 0.0 ? -worldNormal : worldNormal;
    proximityLightDistance = abs(proximityLightDistance);
#endif
    float normalizedProximityLightDistance = saturate(proximityLightDistance * proximityLightParams.y);
    float3 projectedProximityLight = proximityLight.xyz - (worldNormal * abs(proximityLightDistance));
    float projectedProximityLightDistance = length(projectedProximityLight - worldPosition);
    float attenuation = (1.0 - normalizedProximityLightDistance) * proximityLight.w;
    colorValue = saturate(projectedProximityLightDistance * proximityLightParams.z);
    float pulse = step(proximityLightPulseParams.x, projectedProximityLightDistance) * proximityLightPulseParams.y;

    return smoothstep(1.0, 0.0, projectedProximityLightDistance / (proximityLightParams.x * max(pow(normalizedProximityLightDistance, 0.25), proximityLightParams.w))) * pulse * attenuation;
}

inline half3 GTMixProximityLightColor(half4 centerColor, half4 middleColor, half4 outerColor, half t)
{
    half3 color = lerp(centerColor.rgb, middleColor.rgb, smoothstep(centerColor.a, middleColor.a, t));
    return lerp(color, outerColor.rgb, smoothstep(middleColor.a, outerColor.a, t));
}

/// <summary>
/// SDF methods.
/// </summary>

inline float GTPointVsPlane(float3 worldPosition, float4 plane)
{
    float3 planePosition = plane.xyz * plane.w;
    return dot(worldPosition - planePosition, plane.xyz);
}

inline float GTPointVsSphere(float3 worldPosition, float4x4 sphereInverseTransform)
{
    return length(mul(sphereInverseTransform, float4(worldPosition, 1.0)).xyz) - 0.5;
}

inline float GTPointVsBox(float3 worldPosition, float4x4 boxInverseTransform)
{
    float3 distance = abs(mul(boxInverseTransform, float4(worldPosition, 1.0)).xyz) - 0.5;
    return length(max(distance, 0.0)) + min(max(distance.x, max(distance.y, distance.z)), 0.0);
}

inline float GTPointVsRoundedBox(in float2 position, in float2 cornerCircleDistance, in float cornerCircleRadius)
{
    return length(max(abs(position) - cornerCircleDistance, 0.0)) - cornerCircleRadius;
}

inline float FilterDistance(in float distance)
{
    float filterWidth = fwidth(distance);
    return smoothstep(filterWidth, -filterWidth, distance);
}

inline float GTRoundCornersSmooth(in float2 position, in float2 cornerCircleDistance, in float cornerCircleRadius, in float smoothingValue)
{
    float distance = GTPointVsRoundedBox(position, cornerCircleDistance, cornerCircleRadius);
#if defined(_EDGE_SMOOTHING_AUTOMATIC)
    return FilterDistance(distance);
#else
    return smoothstep(1.0, 0.0, distance / smoothingValue);
#endif
}

inline float GTRoundCorners(in float2 position, in float2 cornerCircleDistance, in float cornerCircleRadius, in float smoothingValue)
{
#if defined(_TRANSPARENT)
    return GTRoundCornersSmooth(position, cornerCircleDistance, cornerCircleRadius, smoothingValue);
#else
    return (GTPointVsRoundedBox(position, cornerCircleDistance, cornerCircleRadius) < 0.0);
#endif
}

inline float GTFindCornerRadius(in float2 offset, in float4 radii)
{
    if (offset.x < 0)
    {
        if (offset.y > 0) { return radii.x; }
        else { return radii.z; }
    }
    else
    {
        if (offset.y > 0) { return radii.y; }
        else { return radii.w; }
    }
}

/// <summary>
/// UnityUI methods.
/// </summary>

inline float GTGet2DClippingRounded(in float2 position, in float4 clipRect, in float radius)
{
    float2 halfSize = (clipRect.zw - clipRect.xy) * 0.5;
    float2 center = clipRect.xy + halfSize;
    float2 offset = position - center;

    return GTPointVsRoundedBox(offset, halfSize - radius, radius);
}

inline float GTGet2DClippingRoundedSoft(in float2 position, in float4 clipRect, in float radius)
{
    return saturate(FilterDistance(GTGet2DClippingRounded(position, clipRect, radius)));
}

inline float GTGet2DClippingRoundedIndependent(in float2 position, in float4 clipRect, in float4 radii)
{
    float2 halfSize = (clipRect.zw - clipRect.xy) * 0.5;
    float2 center = clipRect.xy + halfSize;
    float2 offset = position - center;
    float radius = GTFindCornerRadius(offset, radii);

    return GTPointVsRoundedBox(offset, halfSize - radius, radius);
}

inline float GTGet2DClippingRoundedIndependentSoft(in float2 position, in float4 clipRect, in float4 radii)
{
    return saturate(FilterDistance(GTGet2DClippingRoundedIndependent(position, clipRect, radii)));
}

inline float GTUnityUIClipRect(in float2 position, in float4 clipRect, in float4 radii)
{
#if defined(UNITY_UI_ALPHACLIP)
#if defined(_UI_CLIP_RECT_ROUNDED)
    radii.x = max(radii.x, GRAPHICS_TOOLS_MIN_CORNER_VALUE_RECT);
    return GTGet2DClippingRounded(position, clipRect, radii.x) <= 0.0;
#elif defined(_UI_CLIP_RECT_ROUNDED_INDEPENDENT)
    radii = max(radii, GRAPHICS_TOOLS_MIN_CORNER_VALUE_RECT);
    return GTGet2DClippingRoundedIndependent(position, clipRect, radii) <= 0.0;
#else
    return GTGet2DClippingRounded(position, clipRect, GRAPHICS_TOOLS_MIN_CORNER_VALUE_RECT) <= 0.0;
#endif
#else
#if defined(_UI_CLIP_RECT_ROUNDED)
    radii.x = max(radii.x, GRAPHICS_TOOLS_MIN_CORNER_VALUE_RECT);
    return GTGet2DClippingRoundedSoft(position, clipRect, radii.x);
#elif defined(_UI_CLIP_RECT_ROUNDED_INDEPENDENT)
    radii = max(radii, GRAPHICS_TOOLS_MIN_CORNER_VALUE_RECT);
    return GTGet2DClippingRoundedIndependentSoft(position, clipRect, radii);
#else
    return GTGet2DClippingRoundedSoft(position, clipRect, GRAPHICS_TOOLS_MIN_CORNER_VALUE_RECT);
#endif
#endif
}

/// <summary>
/// Gradient methods.
/// </summary>

#if defined(_URP)
half3 GTIridescence(float tangentDotIncident, TEXTURE2D_PARAM(spectrumMap, sampler_spectrumMap), float threshold, float2 uv, float angle, float intensity)
#else
half3 GTIridescence(float tangentDotIncident, sampler2D spectrumMap, float threshold, float2 uv, float angle, float intensity)
#endif
{
    float k = tangentDotIncident * 0.5 + 0.5;

#if defined(_URP)
    float4 left = SAMPLE_TEXTURE2D_GRAD(spectrumMap, sampler_spectrumMap, float2(lerp(0.0, 1.0 - threshold, k), 0.5), float2(0.0, 0.0), float2(0.0, 0.0));
    float4 right = SAMPLE_TEXTURE2D_GRAD(spectrumMap, sampler_spectrumMap, float2(lerp(threshold, 1.0, k), 0.5), float2(0.0, 0.0), float2(0.0, 0.0));
#else
    float4 left = tex2D(spectrumMap, float2(lerp(0.0, 1.0 - threshold, k), 0.5), float2(0.0, 0.0), float2(0.0, 0.0));
    float4 right = tex2D(spectrumMap, float2(lerp(threshold, 1.0, k), 0.5), float2(0.0, 0.0), float2(0.0, 0.0));
#endif

    float2 XY = uv - float2(0.5, 0.5);
    float s = (cos(angle) * XY.x - sin(angle) * XY.y) / cos(angle);
    return (left.rgb + s * (right.rgb - left.rgb)) * intensity;
}

half4 GTFourPointGradient(half4 topLeft, half4 topRight, half4 bottomLeft, half4 bottomRight, half2 uv)
{
    half4 top = topLeft + (topRight - topLeft) * uv.x;
    half4 bottom = bottomLeft + (bottomRight - bottomLeft) * uv.x;

    return bottom + (top - bottom) * uv.y;
}

half4 GTLinearGradient(half4 color0, half4 color1, half4 color2, half4 color3, half4 alpha, half4 alphaTime, float t)
{
    half4 output;

    output.rgb = lerp(color0.rgb, color1.rgb, smoothstep(color0.a, color1.a, t));
    output.rgb = lerp(output.rgb, color2.rgb, smoothstep(color1.a, color2.a, t));
    output.rgb = lerp(output.rgb, color3.rgb, smoothstep(color2.a, color3.a, t));

    output.a = lerp(alpha.r, alpha.g, smoothstep(alphaTime.r, alphaTime.g, t));
    output.a = lerp(output.a, alpha.b, smoothstep(alphaTime.g, alphaTime.b, t));
    output.a = lerp(output.a, alpha.a, smoothstep(alphaTime.b, alphaTime.a, t));

    return output;
}

#endif // GRAPHICS_TOOLS_COMMON
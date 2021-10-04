// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef GRAPHICS_TOOLS_COMMON
#define GRAPHICS_TOOLS_COMMON

/// <summary>
/// Point in primitive methods.
/// </summary>

inline float PointVsPlane(float3 worldPosition, float4 plane)
{
    float3 planePosition = plane.xyz * plane.w;
    return dot(worldPosition - planePosition, plane.xyz);
}

inline float PointVsSphere(float3 worldPosition, float4x4 sphereInverseTransform)
{
    return length(mul(sphereInverseTransform, float4(worldPosition, 1.0)).xyz) - 0.5;
}

inline float PointVsBox(float3 worldPosition, float4x4 boxInverseTransform)
{
    float3 distance = abs(mul(boxInverseTransform, float4(worldPosition, 1.0))) - 0.5;
    return length(max(distance, 0.0)) + min(max(distance.x, max(distance.y, distance.z)), 0.0);
}

/// <summary>
/// Lighting methods.
/// </summary>

static const float _MaxNearLightDistance = 10.0;

inline float NearLightDistance(float4 light, float3 worldPosition)
{
    return distance(worldPosition, light.xyz) + ((1.0 - light.w) * _MaxNearLightDistance);
}

inline float HoverLight(float4 hoverLight, float inverseRadius, float3 worldPosition)
{
    return (1.0 - saturate(length(hoverLight.xyz - worldPosition) * inverseRadius)) * hoverLight.w;
}

inline float ProximityLight(float4 proximityLight, float4 proximityLightParams, float4 proximityLightPulseParams, float3 worldPosition, float3 worldNormal, out fixed colorValue)
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

inline fixed3 MixProximityLightColor(fixed4 centerColor, fixed4 middleColor, fixed4 outerColor, fixed t)
{
    fixed3 color = lerp(centerColor.rgb, middleColor.rgb, smoothstep(centerColor.a, middleColor.a, t));
    return lerp(color, outerColor, smoothstep(middleColor.a, outerColor.a, t));
}

/// <summary>
/// SDF methods.
/// </summary>

inline float PointVsRoundedBox(float2 position, float2 cornerCircleDistance, float cornerCircleRadius)
{
    return length(max(abs(position) - cornerCircleDistance, 0.0)) - cornerCircleRadius;
}

inline float RoundCornersSmooth(float2 position, float2 cornerCircleDistance, float cornerCircleRadius, float smoothingValue)
{
    return smoothstep(1.0, 0.0, PointVsRoundedBox(position, cornerCircleDistance, cornerCircleRadius) / smoothingValue);
}

inline float RoundCorners(float2 position, float2 cornerCircleDistance, float cornerCircleRadius, float smoothingValue)
{
#if defined(_TRANSPARENT)
    return RoundCornersSmooth(position, cornerCircleDistance, cornerCircleRadius, smoothingValue);
#else
    return (PointVsRoundedBox(position, cornerCircleDistance, cornerCircleRadius) < 0.0);
#endif
}

/// <summary>
/// Gradient methods.
/// </summary>

fixed3 Iridescence(float tangentDotIncident, sampler2D spectrumMap, float threshold, float2 uv, float angle, float intensity)
{
    float k = tangentDotIncident * 0.5 + 0.5;
    float4 left = tex2D(spectrumMap, float2(lerp(0.0, 1.0 - threshold, k), 0.5), float2(0.0, 0.0), float2(0.0, 0.0));
    float4 right = tex2D(spectrumMap, float2(lerp(threshold, 1.0, k), 0.5), float2(0.0, 0.0), float2(0.0, 0.0));

    float2 XY = uv - float2(0.5, 0.5);
    float s = (cos(angle) * XY.x - sin(angle) * XY.y) / cos(angle);
    return (left.rgb + s * (right.rgb - left.rgb)) * intensity;
}

fixed3 FourPointGradient(half4 gradientColor, half4 topLeft, half4 topRight, half4 bottomLeft, half4 bottomRight, half2 uv)
{
    half3 top = topLeft.rgb + (topRight.rgb - topLeft.rgb) * uv.x;
    half3 bottom = bottomLeft.rgb + (bottomRight.rgb - bottomLeft.rgb) * uv.x;

    return gradientColor.rgb * (bottom + (top - bottom) * uv.y);
}

fixed3 LinearGradient(half4 color0, half4 color1, half4 color2, half4 color3, float t)
{
    fixed3 color = lerp(color0.rgb, color1.rgb, smoothstep(color0.a, color1.a, t));
    color = lerp(color, color2, smoothstep(color1.a, color2.a, t));
    return lerp(color, color3, smoothstep(color2.a, color3.a, t));
}

#endif // GRAPHICS_TOOLS_COMMON
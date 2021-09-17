﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef GRAPHICS_TOOLS_COMMON
#define GRAPHICS_TOOLS_COMMON

/// <summary>
/// Point in primitive tests.
/// </summary>

#if defined(_CLIPPING_PLANE)
inline float PointVsPlane(float3 worldPosition, float4 plane)
{
    float3 planePosition = plane.xyz * plane.w;
    return dot(worldPosition - planePosition, plane.xyz);
}
#endif

#if defined(_CLIPPING_SPHERE)
inline float PointVsSphere(float3 worldPosition, float4x4 sphereInverseTransform)
{
    return length(mul(sphereInverseTransform, float4(worldPosition, 1.0)).xyz) - 0.5;
}
#endif

#if defined(_CLIPPING_BOX)
inline float PointVsBox(float3 worldPosition, float4x4 boxInverseTransform)
{
    float3 distance = abs(mul(boxInverseTransform, float4(worldPosition, 1.0))) - 0.5;
    return length(max(distance, 0.0)) + min(max(distance.x, max(distance.y, distance.z)), 0.0);
}
#endif

#endif // GRAPHICS_TOOLS_COMMON
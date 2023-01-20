// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Round corner clipping
// Shared by gt standard and gt shadowpass

#ifndef GT_ROUND_CORNERS
#define GT_ROUND_CORNERS
void RoundCorners(
    half2 cornerPosition, 
    float2 st,
    half zscale,
    half halfScale,
    half edgeSmoothingValue,
    
    inout half roundCornerRadius, 
    inout half roundCornerMargin, 

    out half currentCornerRadius,
    out half cornerCircleRadius,
    out half2 cornerCircleDistance, 
    out half cornerClip)
{
    #if defined(_INDEPENDENT_CORNERS)
        #if !defined(_USE_WORLD_SCALE)
            roundCornerRadius = clamp(roundCornerRadius, 0, 0);
        #endif
            currentCornerRadius = GTFindCornerRadius(st, roundCornerRadius);
    #else 
        currentCornerRadius = roundCornerRadius;
    #endif

    #if defined(_USE_WORLD_SCALE)
        cornerCircleRadius = max(currentCornerRadius, GT_MIN_CORNER_VALUE) * zscale;
    #else
        cornerCircleRadius = saturate(max(currentCornerRadius - roundCornerMargin, GT_MIN_CORNER_VALUE)) * zscale;
    #endif

    cornerCircleDistance = halfScale - (roundCornerMargin * zscale) - cornerCircleRadius;

    #if defined(_ROUND_CORNERS_HIDE_INTERIOR)
        cornerClip = (cornerClip < 1) ? cornerClip : 0;
    #else
        cornerClip = GTRoundCorners(cornerPosition, cornerCircleDistance, cornerCircleRadius, _EdgeSmoothingValue * zscale);
    #endif
}
#endif

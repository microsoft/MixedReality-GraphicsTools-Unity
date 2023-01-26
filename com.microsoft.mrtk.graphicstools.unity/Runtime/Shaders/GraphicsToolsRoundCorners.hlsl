// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Round corner clipping
// Shared by GT standard and GT shadowpass

#ifndef GT_ROUND_CORNERS
#define GT_ROUND_CORNERS
void RoundCorners(
    half2 cornerPosition,
    float2 st,
    half minScaleWS,
    half2 halfScale,
    half edgeSmoothingValue, // * minScaleWS
    half roundCornerRadius,
    half roundCornerMargin, // * minScaleWS
    out float currentCornerRadius,
    out float cornerCircleRadius, // * minScaleWS
    out float2 cornerCircleDistance,
    out half cornerClip)
{
    #if defined(_INDEPENDENT_CORNERS)
        #if !defined(_USE_WORLD_SCALE)
            currentCornerRadius = GTFindCornerRadius(st, clamp(roundCornerRadius, 0, .5);
        #endif
        currentCornerRadius = GTFindCornerRadius(st, roundCornerRadius);
    #else 
        currentCornerRadius = roundCornerRadius;
    #endif

    #if defined(_USE_WORLD_SCALE)
        cornerCircleRadius = max(currentCornerRadius, GT_MIN_CORNER_VALUE);
    #else
        cornerCircleRadius = saturate(max(currentCornerRadius - roundCornerMargin, GT_MIN_CORNER_VALUE));
    #endif
        
    cornerCircleRadius *= minScaleWS;
    
    cornerCircleDistance = halfScale - (roundCornerMargin * minScaleWS) - cornerCircleRadius;

    #if defined(_ROUND_CORNERS_HIDE_INTERIOR)
        cornerClip = (cornerClip < half(1.0)) ? cornerClip : half(0.0);
    #else
    cornerClip = GTRoundCorners(cornerPosition, cornerCircleDistance, cornerCircleRadius, edgeSmoothingValue * minScaleWS);
#endif
}
#endif

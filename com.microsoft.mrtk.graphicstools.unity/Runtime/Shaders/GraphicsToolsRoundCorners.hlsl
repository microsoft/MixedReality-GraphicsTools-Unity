// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Round corner clipping
// Shared by GT standard and GT shadowpass

#ifndef GT_ROUND_CORNERS
#define GT_ROUND_CORNERS
half CurrentCornerRadius()
{
    #if defined(_INDEPENDENT_CORNERS)
        #if !defined(_USE_WORLD_SCALE)
            return GTFindCornerRadius(input.uv.xy, clamp(_RoundCornerRadius, 0, .5));
        #endif
        return GTFindCornerRadius(input.uv.xy, _RoundCornerRadius);
    #else 
        return _RoundCornerRadius;
    #endif
}

half CornerCircleRadius(half radius, half margin)
{
    half result;
    #if defined(_USE_WORLD_SCALE)
        result = max(radius, GT_MIN_CORNER_VALUE);
    #else
        result = saturate(max(radius - margin, GT_MIN_CORNER_VALUE));
    #endif
    return result;
}

half CornerClip(half2 position, half2 distance, half radius, half smoothing)
{
    half result = GTRoundCorners(position, distance, radius, smoothing);
    #if defined(_ROUND_CORNERS_HIDE_INTERIOR)
        return (result < half(1.0)) ? result : half(0.0);
    #endif
    return result;
}
void RoundCorners(
    half2 cornerPosition,
    float2 st,
    half minScaleWS,
    half2 halfScale,
    half edgeSmoothingValue, // * minScaleWS
    half roundCornerRadius, // why no *minScaleWS?
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
            // BUG this writes over the above, no matter what!!!
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

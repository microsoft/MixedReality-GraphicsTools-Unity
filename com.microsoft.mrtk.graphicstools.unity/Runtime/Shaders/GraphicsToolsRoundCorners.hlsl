// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Round corner clipping
// Shared by GT standard and GT shadowpass

#ifndef GT_ROUND_CORNERS
#define GT_ROUND_CORNERS
half CurrentCornerRadius(half2 uv)
{
    #if defined(_INDEPENDENT_CORNERS)
        #if !defined(_USE_WORLD_SCALE)
            return clamp(_RoundCornerRadius, half(0), half(.5);
        #endif
        return GTFindCornerRadius(uv, _RoundCornerRadius);
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
#endif

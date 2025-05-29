// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef GT_SCALABLE_COMMON
#define GT_SCALABLE_COMMON

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"

half3 GTAlphaModulate(half3 albedo, half alpha)
{
#if UNITY_VERSION >= 202200
	return AlphaModulate(albedo, alpha);
#else
	// Fake alpha for multiply blend by lerping albedo towards 1 (white) using alpha.
	// Manual adjustment for "lighter" multiply effect (similar to "premultiplied alpha")
	// would be painting whiter pixels in the texture.
	// This emulates that procedure in shader, so it should be applied to the base/source color.
#if defined(_ALPHAMODULATE_ON)
	return lerp(half3(1.0, 1.0, 1.0), albedo, alpha);
#else
	return albedo;
#endif
#endif
}

#endif // GT_SCALABLE_COMMON

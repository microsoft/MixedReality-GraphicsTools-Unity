// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef GT_GRID
#define GT_GRID

// Based off: https://bgolus.medium.com/the-best-darn-grid-shader-yet-727f9278b9d8

void Grid_float(in float3 worldPosition,
				in float3 worldSpaceCameraPos,
				in float gridScale,
				in float3 lineWidth,
				in float4 baseColor,
				in float4 lineColor,
				out float4 output)
{
	// Trick to reduce visual artifacts when far from the world origin.
	float3 cameraCenteringOffset = floor(worldSpaceCameraPos * gridScale);
	float3 uvw = worldPosition * gridScale - cameraCenteringOffset;

	float3 blendNormal = abs(normalize(cross(ddy(uvw), ddx(uvw))));

	// Adjust line width based on surface angle this is a cos to sin conversion.
	lineWidth *= sqrt(saturate(1.0 - blendNormal * blendNormal));

	float3 uvwDDX = ddx(uvw);
	float3 uvwDDY = ddy(uvw);
	float3 uvwDeriv = float3(
					length(float2(uvwDDX.x, uvwDDY.x)),
					length(float2(uvwDDX.y, uvwDDY.y)),
					length(float2(uvwDDX.z, uvwDDY.z))
					);
	uvwDeriv = max(uvwDeriv, 0.00001);

	bool3 invertLine = lineWidth > 0.5;
	float3 targetWidth = invertLine ? 1.0 - lineWidth : lineWidth;
	float3 drawWidth = clamp(targetWidth, uvwDeriv, 0.5);
	float3 lineAA = uvwDeriv * 1.5;
	float3 gridUV = abs(frac(uvw) * 2.0 - 1.0);
	gridUV = invertLine ? gridUV : 1.0 - gridUV;
	float3 grid3 = smoothstep(drawWidth + lineAA, drawWidth - lineAA, gridUV);
	grid3 *= saturate(targetWidth / drawWidth);
	grid3 = lerp(grid3, targetWidth, saturate(uvwDeriv * 2.0 - 1.0));
	grid3 = invertLine ? 1.0 - grid3 : grid3;

	float3 blendFactor = blendNormal / dot(float3(1, 1, 1), blendNormal);
	float3 blendFwidth = max(fwidth(blendFactor), 0.0001);

	// 0.8 is an arbitrary offset to hide the line when the surface is
	// almost axis aligned.
	float3 blendEdge = 0.8;

	grid3 *= saturate((blendEdge - blendFactor) / blendFwidth + 1.0);

	float grid = lerp(lerp(grid3.x, 1.0, grid3.y), 1.0, grid3.z);

	// Lerp between base and line color.
	output = lerp(baseColor, lineColor, grid * lineColor.a);
}

#endif // GT_GRID

// Copyright © 2020 Unity Technologies ApS
// Licensed under the Unity Companion License for Unity-dependent projects--see https://unity3d.com/legal/licenses/Unity_Companion_License
// Unless expressly provided otherwise, the Software under this license is made available strictly on an “AS IS” BASIS WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED. Please review the license for details on these and other terms and conditions.

#ifndef GT_LIGHTING
#define GT_LIGHTING

/// <summary>
/// Light accessors.
/// </summary>

struct GTMainLight
{
    half3 direction;
    half3 color;
};

GTMainLight GTGetMainLight()
{
    GTMainLight light;
#if defined(_DIRECTIONAL_LIGHT) || defined(_DISTANT_LIGHT)
#if defined(_DISTANT_LIGHT)
    light.direction = _DistantLightData[0].xyz;
    light.color = _DistantLightData[1].xyz;
#else
#if defined(_URP)
    Light directionalLight = GetMainLight();
    light.direction = directionalLight.direction;
    light.color = directionalLight.color;
#else
    light.direction = _WorldSpaceLightPos0.xyz;
    light.color = _LightColor0.rgb;
#endif
#endif
#endif
    return light;
}

/// <summary>
/// Physically based rendering methods forked from https://github.com/Unity-Technologies/Graphics to provide mixed reality specific adjustments.
/// </summary>

// Standard dielectric reflectivity coef at incident angle (= 4%).
#define GTDielectricSpec half4(0.04, 0.04, 0.04, 1.0 - 0.04)

// A good ambient global illumination value when spherical harmonics are not available.
#define GTDefaultAmbientGI (glstate_lightmodel_ambient.rgb + half3(0.25h, 0.25h, 0.25h))

struct GTBRDFData
{
    half3 albedo;
    half3 diffuse;
    half3 specular;
    half reflectivity;
    half perceptualRoughness;
    half roughness;
    half roughness2;
    half grazingTerm;

    // We save some light invariant BRDF terms so we don't have to recompute
    // them in the light loop. Take a look at DirectBRDF function for detailed explaination.
    half normalizationTerm;     // roughness * 4.0 + 2.0
    half roughness2MinusOne;    // roughness^2 - 1.0
};

half GTOneMinusReflectivityMetallic(half metallic)
{
    // We'll need oneMinusReflectivity, so
    //   1-reflectivity = 1-lerp(dielectricSpec, 1, metallic) = lerp(1-dielectricSpec, 0, metallic)
    // store (1-dielectricSpec) in kDielectricSpec.a, then
    //   1-reflectivity = lerp(alpha, 0, metallic) = alpha + metallic*(0 - alpha) =
    //                  = alpha - metallic * alpha
    half oneMinusDielectricSpec = GTDielectricSpec.a;
    return oneMinusDielectricSpec - metallic * oneMinusDielectricSpec;
}

half GTPerceptualRoughnessToRoughness(half perceptualRoughness)
{
    return perceptualRoughness * perceptualRoughness;
}

half GTPerceptualSmoothnessToPerceptualRoughness(half perceptualSmoothness)
{
    return (1.0h - perceptualSmoothness);
}

inline void GTInitializeBRDFDataDirect(half3 albedo, half3 diffuse, half3 specular, half reflectivity, half oneMinusReflectivity, half smoothness, inout half alpha, out GTBRDFData outBRDFData)
{
    outBRDFData = (GTBRDFData)0;
    outBRDFData.albedo = albedo;
    outBRDFData.diffuse = diffuse;
    outBRDFData.specular = specular;
    outBRDFData.reflectivity = reflectivity;

    outBRDFData.perceptualRoughness = GTPerceptualSmoothnessToPerceptualRoughness(smoothness);
    outBRDFData.roughness = max(GTPerceptualRoughnessToRoughness(outBRDFData.perceptualRoughness), GT_HALF_MIN_SQRT);
    outBRDFData.roughness2 = max(outBRDFData.roughness * outBRDFData.roughness, GT_HALF_MIN);
    outBRDFData.grazingTerm = saturate(smoothness + reflectivity);
    outBRDFData.normalizationTerm = outBRDFData.roughness * 4.0h + 2.0h;
    outBRDFData.roughness2MinusOne = outBRDFData.roughness2 - 1.0h;

#ifdef _ALPHAPREMULTIPLY_ON
    outBRDFData.diffuse *= alpha;
    alpha = alpha * oneMinusReflectivity + reflectivity; // NOTE: alpha modified and propagated up.
#endif
}

inline void GTInitializeBRDFData(half3 albedo, half metallic, half3 specular, half smoothness, inout half alpha, out GTBRDFData outBRDFData)
{
    half oneMinusReflectivity = GTOneMinusReflectivityMetallic(metallic);
    half reflectivity = 1.0h - oneMinusReflectivity;
    half3 brdfDiffuse = albedo * oneMinusReflectivity;
    half3 brdfSpecular = lerp(GTDielectricSpec.rgb, albedo, metallic);

    GTInitializeBRDFDataDirect(albedo, brdfDiffuse, brdfSpecular, reflectivity, oneMinusReflectivity, smoothness, alpha, outBRDFData);
}

// The *approximated* version of the non-linear remapping. It works by
// approximating the cone of the specular lobe, and then computing the MIP map level
// which (approximately) covers the footprint of the lobe with a single texel.
// Improves the perceptual roughness distribution.
half GTPerceptualRoughnessToMipmapLevel(half perceptualRoughness)
{
    perceptualRoughness = perceptualRoughness * (1.7 - 0.7 * perceptualRoughness);

    return perceptualRoughness * UNITY_SPECCUBE_LOD_STEPS;
}

half3 GTGlossyEnvironmentReflection(half3 reflectVector, half perceptualRoughness, half occlusion)
{
#if defined(_REFLECTIONS)
    half mip = GTPerceptualRoughnessToMipmapLevel(perceptualRoughness);

#if defined(_URP)
    half4 encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflectVector, mip);
#else
    half4 encodedIrradiance = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectVector, mip);
#endif

#if defined(UNITY_USE_NATIVE_HDR)
    half3 irradiance = encodedIrradiance.rgb;
#else
#if defined(_URP)
    half3 irradiance = DecodeHDREnvironment(encodedIrradiance, unity_SpecCube0_HDR);
#else
    half3 irradiance = DecodeHDR(encodedIrradiance, unity_SpecCube0_HDR);
#endif
#endif

    return irradiance * occlusion;
#endif

#if defined(_URP)
    return _GlossyEnvironmentColor.rgb * occlusion;
#else
    return unity_IndirectSpecColor.rgb * occlusion;
#endif
}

// Computes the specular term for EnvironmentBRDF
half3 GTEnvironmentBRDFSpecular(GTBRDFData brdfData, half fresnelTerm)
{
    float surfaceReduction = 1.0 / (brdfData.roughness2 + 1.0);
    return surfaceReduction * lerp(brdfData.specular, brdfData.grazingTerm, fresnelTerm);
}

half3 GTEnvironmentBRDF(GTBRDFData brdfData, half3 indirectDiffuse, half3 indirectSpecular, half fresnelTerm)
{
    half3 c = indirectDiffuse * brdfData.diffuse;
    c += indirectSpecular * GTEnvironmentBRDFSpecular(brdfData, fresnelTerm);
    return c;
}

half3 GTGlobalIllumination(GTBRDFData brdfData, half3 bakedGI, half occlusion, half3 normalWS, half3 viewDirectionWS)
{
    half3 reflectVector = reflect(-viewDirectionWS, normalWS);
    half NoV = saturate(dot(normalWS, viewDirectionWS));
    half fresnelTerm = GTPow4(1.0 - NoV);

    half3 indirectDiffuse = bakedGI * occlusion;
    half3 indirectSpecular = GTGlossyEnvironmentReflection(reflectVector, brdfData.perceptualRoughness, occlusion);

    return GTEnvironmentBRDF(brdfData, indirectDiffuse, indirectSpecular, fresnelTerm);;
}

// Computes the scalar specular term for Minimalist CookTorrance BRDF
// NOTE: needs to be multiplied with reflectance f0, i.e. specular color to complete
half GTDirectBRDFSpecular(GTBRDFData brdfData, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS)
{
    float3 halfDir = GTSafeNormalize(float3(lightDirectionWS) + float3(viewDirectionWS));

    float NoH = saturate(dot(normalWS, halfDir));
    half LoH = saturate(dot(lightDirectionWS, halfDir));

    // GGX Distribution multiplied by combined approximation of Visibility and Fresnel
    // BRDFspec = (D * V * F) / 4.0
    // D = roughness^2 / ( NoH^2 * (roughness^2 - 1) + 1 )^2
    // V * F = 1.0 / ( LoH^2 * (roughness + 0.5) )
    // See "Optimizing PBR for Mobile" from Siggraph 2015 moving mobile graphics course
    // https://community.arm.com/events/1155

    // Final BRDFspec = roughness^2 / ( NoH^2 * (roughness^2 - 1) + 1 )^2 * (LoH^2 * (roughness + 0.5) * 4.0)
    // We further optimize a few light invariant terms
    // brdfData.normalizationTerm = (roughness + 0.5) * 4.0 rewritten as roughness * 4.0 + 2.0 to a fit a MAD.
    float d = NoH * NoH * brdfData.roughness2MinusOne + 1.00001f;

    half LoH2 = LoH * LoH;
    half specularTerm = brdfData.roughness2 / ((d * d) * max(0.1h, LoH2) * brdfData.normalizationTerm);

    // On platforms where half actually means something, the denominator has a risk of overflow
    // clamp below was added specifically to "fix" that, but dx compiler (we convert bytecode to metal/gles)
    // sees that specularTerm have only non-negative terms, so it skips max(0,..) in clamp (leaving only min(100,...))
#if defined (SHADER_API_MOBILE)
    specularTerm = specularTerm - GT_HALF_MIN;
    specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif

    return specularTerm;
}

half3 GTLightingPhysicallyBased(GTBRDFData brdfData,
    half3 lightColor, half3 lightDirectionWS,
    half3 normalWS, half3 viewDirectionWS)
{
    half NdotL = saturate(dot(normalWS, lightDirectionWS));
    half3 radiance = lightColor * NdotL;

    half3 brdf = brdfData.diffuse;
#if defined(_SPECULAR_HIGHLIGHTS)
    brdf += brdfData.specular * GTDirectBRDFSpecular(brdfData, normalWS, lightDirectionWS, viewDirectionWS);
#endif

    return brdf * radiance;
}

#endif // GT_LIGHTING

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

PackedVaryings vert(Attributes input)
{
    Varyings output = (Varyings)0;
    output = BuildVaryings(input);
    PackedVaryings packedOutput = (PackedVaryings)0;
    packedOutput = PackVaryings(output);
    return packedOutput;
}
  
#if defined(MATERIAL_QUALITY_LOW)

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#if defined(LOD_FADE_CROSSFADE)
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
#endif

#include "ScalableCommon.hlsl"

void InitializeInputData(Varyings input, SurfaceDescription surfaceDescription, out InputData inputData)  
{  
    inputData = (InputData)0;  
    
    inputData.positionWS = input.positionWS;  
    inputData.normalWS = input.normalWS; 
    inputData.viewDirectionWS = half3(0, 0, 1);
    inputData.shadowCoord = 0;
    inputData.fogCoord = 0; 
    inputData.vertexLighting = half3(0, 0, 0);
    #if defined(LIGHTMAP_ON)
        inputData.bakedGI = SampleLightmap(input.staticLightmapUV, input.positionWS, inputData.normalWS);
    #else
        inputData.bakedGI = half3(1, 1, 1);
    #endif
        inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);  
        inputData.shadowMask = half4(1, 1, 1, 1);
  
    #if defined(DEBUG_DISPLAY) 
        #if defined(LIGHTMAP_ON)  
            inputData.staticLightmapUV = input.staticLightmapUV;  
        #else  
            inputData.vertexSH = input.sh;  
        #endif  
    #endif  
}  

void frag(  
    PackedVaryings packedInput,  
    out half4 outColor : SV_Target0  
#ifdef _WRITE_RENDERING_LAYERS  
    , out float4 outRenderingLayers : SV_Target1  
#endif  
)  
{  
    Varyings unpacked = UnpackVaryings(packedInput);  
    UNITY_SETUP_INSTANCE_ID(unpacked);  
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(unpacked);  
    SurfaceDescription surfaceDescription = BuildSurfaceDescription(unpacked);  
  
    #if defined(_SURFACE_TYPE_TRANSPARENT)  
        bool isTransparent = true;  
    #else  
        bool isTransparent = false;  
    #endif  
  
    #if defined(_ALPHATEST_ON)  
        half alpha = AlphaDiscard(surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold);  
    #elif defined(_SURFACE_TYPE_TRANSPARENT)  
        half alpha = surfaceDescription.Alpha;  
    #else  
        half alpha = half(1.0);  
    #endif  

    #if defined(LOD_FADE_CROSSFADE) && USE_UNITY_CROSSFADE
        LODFadeCrossFade(unpacked.positionCS);
    #endif

    #if defined(_ALPHAMODULATE_ON)
        surfaceDescription.BaseColor = GTAlphaModulate(surfaceDescription.BaseColor, alpha);
    #endif

    #if defined(_DBUFFER)
        ApplyDecalToBaseColor(unpacked.positionCS, surfaceDescription.BaseColor);
    #endif
  
    InputData inputData;  
    InitializeInputData(unpacked, surfaceDescription, inputData);  
  
    half4 albedo = float4(surfaceDescription.BaseColor, saturate(alpha));  
    half3 normalTS = half3(0, 0, 1);
    half4 finalColor = UniversalFragmentBakedLit(inputData, albedo.rgb, alpha, normalTS);
    finalColor.a = OutputAlpha(finalColor.a, isTransparent);
    outColor = finalColor;  

    #ifdef _WRITE_RENDERING_LAYERS  
        uint renderingLayers = GetMeshRenderingLayer();  
        outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);  
    #endif  
} 

#else

#include "ScalableCommon.hlsl"

void InitializeInputData(Varyings input, SurfaceDescription surfaceDescription, out InputData inputData)
{
    inputData = (InputData)0;

    inputData.positionWS = input.positionWS;

    #ifdef _NORMALMAP
        // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
        float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
        float3 bitangent = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);

        inputData.tangentToWorld = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);
        #if _NORMAL_DROPOFF_TS
            inputData.normalWS = TransformTangentToWorld(surfaceDescription.NormalTS, inputData.tangentToWorld);
        #elif _NORMAL_DROPOFF_OS
            inputData.normalWS = TransformObjectToWorldNormal(surfaceDescription.NormalOS);
        #elif _NORMAL_DROPOFF_WS
            inputData.normalWS = surfaceDescription.NormalWS;
        #endif
    #else
        inputData.normalWS = input.normalWS;
    #endif
    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        inputData.shadowCoord = input.shadowCoord;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
    #else
        inputData.shadowCoord = float4(0, 0, 0, 0);
    #endif

    inputData.fogCoord = InitializeInputDataFog(float4(input.positionWS, 1.0), input.fogFactorAndVertexLight.x);
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
#if defined(DYNAMICLIGHTMAP_ON)
    inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV.xy, input.sh, inputData.normalWS);
#else
    inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.sh, inputData.normalWS);
#endif
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);

    #if defined(DEBUG_DISPLAY)
    #if defined(DYNAMICLIGHTMAP_ON)
    inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
    #endif
    #if defined(LIGHTMAP_ON)
    inputData.staticLightmapUV = input.staticLightmapUV;
    #else
    inputData.vertexSH = input.sh;
    #endif
    #endif
}

void frag(
    PackedVaryings packedInput
    , out half4 outColor : SV_Target0
#ifdef _WRITE_RENDERING_LAYERS
    , out float4 outRenderingLayers : SV_Target1
#endif
)
{
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(unpacked);
    SurfaceDescription surfaceDescription = BuildSurfaceDescription(unpacked);

#if defined(_SURFACE_TYPE_TRANSPARENT)
    bool isTransparent = true;
#else
    bool isTransparent = false;
#endif

#if defined(_ALPHATEST_ON)
    half alpha = AlphaDiscard(surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold);
#elif defined(_SURFACE_TYPE_TRANSPARENT)
    half alpha = surfaceDescription.Alpha;
#else
    half alpha = half(1.0);
#endif

    #if defined(LOD_FADE_CROSSFADE) && USE_UNITY_CROSSFADE
        LODFadeCrossFade(unpacked.positionCS);
    #endif

    InputData inputData;
    InitializeInputData(unpacked, surfaceDescription, inputData);
    // TODO: Mip debug modes would require this, open question how to do this on ShaderGraph.
    //SETUP_DEBUG_TEXTURE_DATA(inputData, unpacked.texCoord1.xy, _MainTex);

    #ifdef _SPECULAR_SETUP
        float3 specular = surfaceDescription.Specular;
    #if defined(MATERIAL_QUALITY_MEDIUM)
    #else
        float metallic = 1;
    #endif//MATERIAL_QUALITY_MEDIUM
    #else
        float3 specular = 0;
    #if defined(MATERIAL_QUALITY_MEDIUM)
    #else
        float metallic = surfaceDescription.Metallic;
    #endif//MATERIAL_QUALITY_MEDIUM
    #endif

    half3 normalTS = half3(0, 0, 0);
    #if defined(_NORMALMAP) && defined(_NORMAL_DROPOFF_TS)
        normalTS = surfaceDescription.NormalTS;
    #endif

    SurfaceData surface;
    surface.albedo              = surfaceDescription.BaseColor;
#if defined(MATERIAL_QUALITY_MEDIUM)
    surface.metallic            = 0.0;
#else
    surface.metallic            = saturate(metallic);
#endif//MATERIAL_QUALITY_MEDIUM
    surface.specular            = specular;
    surface.smoothness          = saturate(surfaceDescription.Smoothness),
#if defined(MATERIAL_QUALITY_MEDIUM)
    surface.occlusion           = 1.0;
#else
    surface.occlusion           = surfaceDescription.Occlusion,
#endif//MATERIAL_QUALITY_MEDIUM
    surface.emission            = surfaceDescription.Emission,
    surface.alpha               = saturate(alpha);
    surface.normalTS            = normalTS;
    surface.clearCoatMask       = 0;
    surface.clearCoatSmoothness = 1;

    #ifdef _CLEARCOAT
        surface.clearCoatMask       = saturate(surfaceDescription.CoatMask);
        surface.clearCoatSmoothness = saturate(surfaceDescription.CoatSmoothness);
    #endif

    surface.albedo = GTAlphaModulate(surface.albedo, surface.alpha);

#ifdef _DBUFFER
    ApplyDecalToSurfaceData(unpacked.positionCS, surface, inputData);
#endif

#if defined(MATERIAL_QUALITY_MEDIUM)
    half4 color = UniversalFragmentBlinnPhong(inputData, surface);
#else
    half4 color = UniversalFragmentPBR(inputData, surface);
#endif//MATERIAL_QUALITY_MEDIUM
    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    color.a = OutputAlpha(color.a, isTransparent);

    outColor = color;

#ifdef _WRITE_RENDERING_LAYERS
    uint renderingLayers = GetMeshRenderingLayer();
    outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
#endif
}
#endif
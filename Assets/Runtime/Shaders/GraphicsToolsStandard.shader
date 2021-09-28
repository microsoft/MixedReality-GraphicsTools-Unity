// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Standard"
{
    Properties
    {
        // Main maps.
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex("Albedo", 2D) = "white" {}
        [Enum(AlbedoAlphaMode)] _AlbedoAlphaMode("Albedo Alpha Mode", Float) = 0 // "Transparency"
        [Toggle] _AlbedoAssignedAtRuntime("Albedo Assigned at Runtime", Float) = 0.0
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        [Toggle(_CHANNEL_MAP)] _EnableChannelMap("Enable Channel Map", Float) = 0.0
        [NoScaleOffset] _ChannelMap("Channel Map", 2D) = "white" {}
        [Toggle(_NORMAL_MAP)] _EnableNormalMap("Enable Normal Map", Float) = 0.0
        [NoScaleOffset] _NormalMap("Normal Map", 2D) = "bump" {}
        _NormalMapScale("Scale", Float) = 1.0
        [Toggle(_EMISSION)] _EnableEmission("Enable Emission", Float) = 0.0
        [HDR]_EmissiveColor("Emissive Color", Color) = (0.0, 0.0, 0.0, 1.0)
        [Toggle(_TRIPLANAR_MAPPING)] _EnableTriplanarMapping("Triplanar Mapping", Float) = 0.0
        [Toggle(_USE_SSAA)] _EnableSSAA("Super Sample Anti Aliasing", Float) = 0.0
        _MipmapBias("Mipmap Bias", Range(-5.0, 0.0)) = -2.0
        [Toggle(_LOCAL_SPACE_TRIPLANAR_MAPPING)] _EnableLocalSpaceTriplanarMapping("Local Space", Float) = 0.0
        _TriplanarMappingBlendSharpness("Blend Sharpness", Range(1.0, 16.0)) = 4.0

        // Rendering options.
        [Toggle(_DIRECTIONAL_LIGHT)] _DirectionalLight("Directional Light", Float) = 1.0
        [Toggle(_SPECULAR_HIGHLIGHTS)] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [Toggle(_SPHERICAL_HARMONICS)] _SphericalHarmonics("Spherical Harmonics", Float) = 0.0
        [Toggle(_REFLECTIONS)] _Reflections("Reflections", Float) = 0.0
        [Toggle(_REFRACTION)] _Refraction("Refraction", Float) = 0.0
        _RefractiveIndex("Refractive Index", Range(0.0, 3.0)) = 0.0
        [Toggle(_RIM_LIGHT)] _RimLight("Rim Light", Float) = 0.0
        _RimColor("Rim Color", Color) = (0.5, 0.5, 0.5, 1.0)
        _RimPower("Rim Power", Range(0.0, 8.0)) = 0.25
        [Toggle(_VERTEX_COLORS)] _VertexColors("Vertex Colors", Float) = 0.0
        [Toggle(_VERTEX_EXTRUSION)] _VertexExtrusion("Vertex Extrusion", Float) = 0.0
        _VertexExtrusionValue("Vertex Extrusion Value", Float) = 0.0
        [Toggle(_VERTEX_EXTRUSION_SMOOTH_NORMALS)] _VertexExtrusionSmoothNormals("Vertex Extrusion Smooth Normals", Float) = 0.0
        _BlendedClippingWidth("Blended Clipping With", Range(0.0, 10.0)) = 1.0
        [Toggle(_CLIPPING_BORDER)] _ClippingBorder("Clipping Border", Float) = 0.0
        _ClippingBorderWidth("Clipping Border Width", Range(0.0, 1.0)) = 0.025
        _ClippingBorderColor("Clipping Border Color", Color) = (1.0, 0.2, 0.0, 1.0)
        [Toggle(_NEAR_PLANE_FADE)] _NearPlaneFade("Near Plane Fade", Float) = 0.0
        [Toggle(_NEAR_LIGHT_FADE)] _NearLightFade("Near Light Fade", Float) = 0.0
        _FadeBeginDistance("Fade Begin Distance", Range(0.0, 10.0)) = 0.85
        _FadeCompleteDistance("Fade Complete Distance", Range(0.0, 10.0)) = 0.5
        _FadeMinValue("Fade Min Value", Range(0.0, 1.0)) = 0.0

        // Fluent options.
        [Toggle(_HOVER_LIGHT)] _HoverLight("Hover Light", Float) = 1.0
        [Toggle(_HOVER_COLOR_OVERRIDE)] _EnableHoverColorOverride("Hover Color Override", Float) = 0.0
        _HoverColorOverride("Hover Color Override", Color) = (1.0, 1.0, 1.0, 1.0)
        [Toggle(_PROXIMITY_LIGHT)] _ProximityLight("Proximity Light", Float) = 0.0
        [Toggle(_PROXIMITY_LIGHT_COLOR_OVERRIDE)] _EnableProximityLightColorOverride("Proximity Light Color Override", Float) = 0.0
        [HDR]_ProximityLightCenterColorOverride("Proximity Light Center Color Override", Color) = (1.0, 0.0, 0.0, 0.0)
        [HDR]_ProximityLightMiddleColorOverride("Proximity Light Middle Color Override", Color) = (0.0, 1.0, 0.0, 0.5)
        [HDR]_ProximityLightOuterColorOverride("Proximity Light Outer Color Override", Color) = (0.0, 0.0, 1.0, 1.0)
        [Toggle(_PROXIMITY_LIGHT_SUBTRACTIVE)] _ProximityLightSubtractive("Proximity Light Subtractive", Float) = 0.0
        [Toggle(_PROXIMITY_LIGHT_TWO_SIDED)] _ProximityLightTwoSided("Proximity Light Two Sided", Float) = 0.0
        _FluentLightIntensity("Fluent Light Intensity", Range(0.0, 1.0)) = 1.0
        [Toggle(_ROUND_CORNERS)] _RoundCorners("Round Corners", Float) = 0.0
        _RoundCornerRadius("Round Corner Radius", Range(0.0, 0.5)) = 0.25
        _RoundCornerMargin("Round Corner Margin", Range(0.0, 0.5)) = 0.01
        [Toggle(_INDEPENDENT_CORNERS)] _IndependentCorners("Independent Corners", Float) = 0.0
        _RoundCornersRadius("Round Corners Radius", Vector) = (0.5 ,0.5, 0.5, 0.5)
        [Toggle(_BORDER_LIGHT)] _BorderLight("Border Light", Float) = 0.0
        [Toggle(_BORDER_LIGHT_REPLACES_ALBEDO)] _BorderLightReplacesAlbedo("Border Light Replaces Albedo", Float) = 0.0
        [Toggle(_BORDER_LIGHT_OPAQUE)] _BorderLightOpaque("Border Light Opaque", Float) = 0.0
        _BorderWidth("Border Width", Range(0.0, 1.0)) = 0.1
        [Enum(BorderColorMode)] _BorderColorMode("_BorderColor Mode", Float) = 0 // "Brightness"
        _BorderMinValue("Border Min Value", Range(0.0, 1.0)) = 0.1
        _BorderColor("Border Color", Color) = (1.0, 1.0, 1.0, 0.0)
        _EdgeSmoothingValue("Edge Smoothing Value", Range(0.0, 1.0)) = 0.002
        _BorderLightOpaqueAlpha("Border Light Opaque Alpha", Range(0.0, 1.0)) = 1.0
        [Toggle(_INNER_GLOW)] _InnerGlow("Inner Glow", Float) = 0.0
        _InnerGlowColor("Inner Glow Color (RGB) and Intensity (A)", Color) = (1.0, 1.0, 1.0, 0.75)
        _InnerGlowPower("Inner Glow Power", Range(2.0, 32.0)) = 4.0
        [Toggle(_IRIDESCENCE)] _Iridescence("Iridescence", Float) = 0.0
        [NoScaleOffset] _IridescentSpectrumMap("Iridescent Spectrum Map", 2D) = "white" {}
        _IridescenceIntensity("Iridescence Intensity", Range(0.0, 1.0)) = 0.5
        _IridescenceThreshold("Iridescence Threshold", Range(0.0, 1.0)) = 0.05
        _IridescenceAngle("Iridescence Angle", Range(-0.78, 0.78)) = -0.78
        [Toggle(_ENVIRONMENT_COLORING)] _EnvironmentColoring("Environment Coloring", Float) = 0.0
        _EnvironmentColorThreshold("Environment Color Threshold", Range(0.0, 3.0)) = 1.5
        _EnvironmentColorIntensity("Environment Color Intensity", Range(0.0, 1.0)) = 0.5
        _EnvironmentColorX("Environment Color X (RGB)", Color) = (1.0, 0.0, 0.0, 1.0)
        _EnvironmentColorY("Environment Color Y (RGB)", Color) = (0.0, 1.0, 0.0, 1.0)
        _EnvironmentColorZ("Environment Color Z (RGB)", Color) = (0.0, 0.0, 1.0, 1.0)

        // Advanced options.
        [Enum(RenderingMode)] _Mode("Rendering Mode", Float) = 0                                     // "Opaque"
        [Enum(CustomRenderingMode)] _CustomMode("Mode", Float) = 0                                   // "Opaque"
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Float) = 1                 // "One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend", Float) = 0            // "Zero"
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("Blend Operation", Float) = 0                 // "Add"
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Depth Test", Float) = 4                // "LessEqual"
        [Enum(DepthWrite)] _ZWrite("Depth Write", Float) = 1                                         // "On"
        _ZOffsetFactor("Depth Offset Factor", Float) = 0                                             // "Zero"
        _ZOffsetUnits("Depth Offset Units", Float) = 0                                               // "Zero"
        [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask("Color Write Mask", Float) = 15 // "All"
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Float) = 2                     // "Back"
        _RenderQueueOverride("Render Queue Override", Range(-1.0, 5000)) = -1
        [Toggle(_USE_WORLD_SCALE)] _UseWorldScale("Use World Scale", Float) = 0.0
        [Toggle(_STENCIL)] _Stencil("Enable Stencil Testing", Float) = 0.0
        _StencilReference("Stencil Reference", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComparison("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOperation("Stencil Operation", Int) = 0
    }

    /// <summary>
    /// Sub Shader for the Universal Render Pipeline (URP).
    /// </summary>
    SubShader
    {
        // Default pass (only pass outside of the editor).
        Pass
        {
            Name "Main"
            Tags{ "RenderType" = "Opaque" "LightMode" = "UniversalForward" }
            LOD 100
            Blend[_SrcBlend][_DstBlend]
            BlendOp[_BlendOp]
            ZTest[_ZTest]
            ZWrite[_ZWrite]
            Cull[_CullMode]
            Offset[_ZOffsetFactor],[_ZOffsetUnits]
            ColorMask[_ColorWriteMask]

            Stencil
            {
                Ref[_StencilReference]
                Comp[_StencilComparison]
                Pass[_StencilOperation]
            }

            CGPROGRAM

            #define _RENDER_PIPELINE

            #include "GraphicsToolsStandardCommon.cginc"

            ENDCG
        }

        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "Meta"
            Tags { "LightMode" = "Meta" }

            CGPROGRAM

            #define _RENDER_PIPELINE

            #include "GraphicsToolsStandardMetaCommon.cginc"
            
            ENDCG
        }
    }

    /// <summary>
    /// Sub Shader for the Lightweight Render Pipeline (LWRP).
    /// </summary>
    SubShader
    {
        // Default pass (only pass outside of the editor).
        Pass
        {
            Name "Main"
            Tags{ "RenderType" = "Opaque" "LightMode" = "LightweightForward" }
            LOD 100
            Blend[_SrcBlend][_DstBlend]
            BlendOp[_BlendOp]
            ZTest[_ZTest]
            ZWrite[_ZWrite]
            Cull[_CullMode]
            Offset[_ZOffsetFactor],[_ZOffsetUnits]
            ColorMask[_ColorWriteMask]

            Stencil
            {
                Ref[_StencilReference]
                Comp[_StencilComparison]
                Pass[_StencilOperation]
            }

            CGPROGRAM

            #define _RENDER_PIPELINE

            #include "GraphicsToolsStandardCommon.cginc"

            ENDCG
        }

        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "Meta"
            Tags { "LightMode" = "Meta" }

            CGPROGRAM

            #define _RENDER_PIPELINE

            #include "GraphicsToolsStandardMetaCommon.cginc"

            ENDCG
        }
    }
    
    /// <summary>
    /// Sub Shader for the Built-in Render Pipeline.
    /// </summary>
    SubShader
    {
        // Default pass (only pass outside of the editor).
        Pass
        {
            Name "Main"
            Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
            LOD 100
            Blend[_SrcBlend][_DstBlend]
            BlendOp[_BlendOp]
            ZTest[_ZTest]
            ZWrite[_ZWrite]
            Cull[_CullMode]
            Offset[_ZOffsetFactor],[_ZOffsetUnits]
            ColorMask[_ColorWriteMask]

            Stencil
            {
                Ref[_StencilReference]
                Comp[_StencilComparison]
                Pass[_StencilOperation]
            }

            CGPROGRAM

            #include "GraphicsToolsStandardCommon.cginc"

            ENDCG
        }

        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "Meta"
            Tags { "LightMode" = "Meta" }

            CGPROGRAM

            #include "GraphicsToolsStandardMetaCommon.cginc"

            ENDCG
        }
    }
    
    Fallback "Hidden/InternalErrorShader"
    CustomEditor "Microsoft.MixedReality.GraphicsTools.Editor.MixedRealityStandardShaderGUI"
}

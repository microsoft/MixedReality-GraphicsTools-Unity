// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/// <summary>
/// Standard shader configured to work best with UnityUI (uGUI) CanvasRenderers.
/// See "Graphics Tools/Standard" for improved Mesh Renderer support.
/// </summary>
Shader "Graphics Tools/Standard Canvas"
{
    Properties
    {
        // Main maps.
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        [PerRendererData] _MainTex("Albedo", 2D) = "white" {}
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.AlbedoAlphaMode)] _AlbedoAlphaMode("Albedo Alpha Mode", Float) = 0 // "Transparency"
        [Toggle] _AlbedoAssignedAtRuntime("Albedo Assigned at Runtime", Float) = 0.0
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _Fade("Alpha Fade", Range(0.0, 1.0)) = 1.0
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        [Toggle(_CHANNEL_MAP)] _EnableChannelMap("Enable Channel Map", Float) = 0.0
        [NoScaleOffset] _ChannelMap("Channel Map", 2D) = "white" {}
        [Toggle(_NORMAL_MAP)] _EnableNormalMap("Enable Normal Map", Float) = 0.0
        [NoScaleOffset] _NormalMap("Normal Map", 2D) = "bump" {}
        _NormalMapScale("Scale", Float) = 1.0
        [Toggle(_EMISSION)] _EnableEmission("Enable Emission", Float) = 0.0
        [HDR]_EmissiveColor("Emissive Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _EmissiveMap("Emissive Map",2D) = "white" {}
        [Toggle(_TRIPLANAR_MAPPING)] _EnableTriplanarMapping("Triplanar Mapping", Float) = 0.0
        [Toggle(_LOCAL_SPACE_TRIPLANAR_MAPPING)] _EnableLocalSpaceTriplanarMapping("Local Space", Float) = 0.0
        _TriplanarMappingBlendSharpness("Blend Sharpness", Range(1.0, 16.0)) = 4.0
        [Toggle(_USE_SSAA)] _EnableSSAA("Super Sample Anti Aliasing", Float) = 0.0
        _MipmapBias("Mipmap Bias", Range(-5.0, 0.0)) = -2.0

        // Rendering options.
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.LightMode)] _DirectionalLight("Light Mode", Float) = 0.0 // "Unlit"
        [Toggle(_NON_PHOTOREALISTIC)] _NPR("Non-Photorealistic Rendering", Float) = 0.0
        [Toggle(_SPECULAR_HIGHLIGHTS)] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [Toggle(_SPHERICAL_HARMONICS)] _SphericalHarmonics("Spherical Harmonics", Float) = 0.0
        [Toggle(_REFLECTIONS)] _Reflections("Reflections", Float) = 0.0
        [Toggle(_RIM_LIGHT)] _RimLight("Rim Light", Float) = 0.0
        _RimColor("Rim Color", Color) = (0.5, 0.5, 0.5, 1.0)
        _RimPower("Rim Power", Range(0.0, 8.0)) = 0.25
        [Toggle(_VERTEX_COLORS)] _VertexColors("Vertex Colors", Float) = 1.0
        [Toggle(_VERTEX_EXTRUSION)] _VertexExtrusion("Vertex Extrusion", Float) = 0.0
        _VertexExtrusionValue("Vertex Extrusion Value", Float) = 0.0
        [Toggle(_VERTEX_EXTRUSION_SMOOTH_NORMALS)] _VertexExtrusionSmoothNormals("Vertex Extrusion Smooth Normals", Float) = 0.0
        [Toggle(_VERTEX_EXTRUSION_CONSTANT_WIDTH)] _VertexExtrusionConstantWidth("Vertex Extrusion Constant Width", Float) = 0.0
        _BlendedClippingWidth("Clipping Alpha Falloff", Range(0.0, 10.0)) = 0.0
        [Toggle(_CLIPPING_BORDER)] _ClippingBorder("Clipping Border", Float) = 0.0
        _ClippingBorderWidth("Clipping Border Width", Range(0.0, 1.0)) = 0.025
        _ClippingBorderColor("Clipping Border Color", Color) = (1.0, 0.2, 0.0, 1.0)
        [Toggle(_NEAR_PLANE_FADE)] _NearPlaneFade("Near Plane Fade", Float) = 0.0
        [Toggle(_NEAR_LIGHT_FADE)] _NearLightFade("Near Light Fade", Float) = 0.0
        _FadeBeginDistance("Fade Begin Distance", Range(0.0, 10.0)) = 0.85
        _FadeCompleteDistance("Fade Complete Distance", Range(0.0, 10.0)) = 0.5
        _FadeMinValue("Fade Min Value", Range(0.0, 1.0)) = 0.0

        // Fluent options.
        [Toggle(_HOVER_LIGHT)] _HoverLight("Hover Light", Float) = 0.0
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
        [Toggle(_ROUND_CORNERS_HIDE_INTERIOR)] _RoundCornersHideInterior("Hide Interior", Float) = 0.0
        [Toggle(_BORDER_LIGHT)] _BorderLight("Border Light", Float) = 0.0
        [Toggle(_BORDER_LIGHT_REPLACES_ALBEDO)] _BorderLightReplacesAlbedo("Border Light Replaces Albedo", Float) = 0.0
        [Toggle(_BORDER_LIGHT_OPAQUE)] _BorderLightOpaque("Border Light Opaque", Float) = 0.0
        _BorderWidth("Border Width", Range(0.0, 1.0)) = 0.1
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.BorderColorMode)] _BorderColorMode("Border Color Mode", Float) = 0 // "Brightness"
        _BorderMinValue("Border Min Value", Range(0.0, 1.0)) = 0.1
        _BorderColor("Border Color", Color) = (1.0, 1.0, 1.0, 0.0)
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.EdgeSmoothingMode)] _EdgeSmoothingMode("Edge Smoothing Mode", Float) = 0 // "Manual"
        _EdgeSmoothingValue("Edge Smoothing Value", Float) = 0.002
        _BorderLightOpaqueAlpha("Border Light Opaque Alpha", Range(0.0, 1.0)) = 1.0
        [Toggle(_INNER_GLOW)] _InnerGlow("Inner Glow", Float) = 0.0
        _InnerGlowColor("Inner Glow Color (RGB) and Intensity (A)", Color) = (1.0, 1.0, 1.0, 0.75)
        _InnerGlowPower("Inner Glow Power", Range(2.0, 32.0)) = 4.0
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.GradientMode)] _GradientMode("Gradient Mode", Float) = 0 // "None"
        [NoScaleOffset] _IridescentSpectrumMap("Iridescent Spectrum Map", 2D) = "white" {}
        _IridescenceIntensity("Iridescence Intensity", Range(0.0, 1.0)) = 0.5
        _IridescenceThreshold("Iridescence Threshold", Range(0.0, 1.0)) = 0.05
        _IridescenceAngle("Iridescence Angle", Range(-0.78, 0.78)) = -0.78
        _GradientAngle("Gradient Angle", Range(0, 360.0)) = 180
        _GradientColor0("Gradient Color 0", Color) = (0.631373, 0.631373, 0.631373, 0.0)
        _GradientColor1("Gradient Color 1", Color) = (1.0, 0.690196, 0.976471, 0.25)
        _GradientColor2("Gradient Color 2", Color) = (0.0, 0.33, 0.88, 0.5)
        _GradientColor3("Gradient Color 3", Color) = (0.0, 0.33, 0.88, 1.0)
        _GradientColor4("Gradient Color 4", Color) = (1.0, 1.0, 1.0, 1.0)
        _GradientAlpha("Gradient Alpha", Vector) = (1.0, 1.0, 1.0, 1.0)
        _GradientAlphaTime("Gradient Alpha Time", Vector) = (0.0, 0.0, 0.0, 0.0)
        [Toggle(_ENVIRONMENT_COLORING)] _EnvironmentColoring("Environment Coloring", Float) = 0.0
        _EnvironmentColorThreshold("Environment Color Threshold", Range(0.0, 3.0)) = 1.5
        _EnvironmentColorIntensity("Environment Color Intensity", Range(0.0, 1.0)) = 0.5
        _EnvironmentColorX("Environment Color X (RGB)", Color) = (1.0, 0.0, 0.0, 1.0)
        _EnvironmentColorY("Environment Color Y (RGB)", Color) = (0.0, 1.0, 0.0, 1.0)
        _EnvironmentColorZ("Environment Color Z (RGB)", Color) = (0.0, 0.0, 1.0, 1.0)
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.BlurMode)] _BlurMode("Blur Mode", Float) = 0 // "None"
        _BlurTextureIntensity("Blur Texture Intensity", Range(0.0, 1.0)) = 1.0
        _BlurBorderIntensity("Blur Border Intensity", Range(0.0, 1.0)) = 0.0
        _BlurBackgroundRect("Blur Background Rect", Vector) = (0.0, 0.0, 1.0, 1.0)

        // Advanced options.
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.RenderingMode)] _Mode("Rendering Mode", Float) = 3          // "Transparent"
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.RenderingMode)] _CustomMode("Mode", Float) = 2              // "Fade"
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Float) = 1                                  // "One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend", Float) = 10                            // "OneMinusSrcAlpha"
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlendAlpha("Source Blend Alpha", Float) = 1                       // "One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlendAlpha("Destination Blend Alpha", Float) = 1                  // "One"
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("Blend Operation", Float) = 0                                  // "Add"
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Depth Test", Float) = 4                                 // "LessEqual"
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.DepthWrite)] _ZWrite("Depth Write", Float) = 0              // "Off"
        _ZOffsetFactor("Depth Offset Factor", Float) = 0                                                              // "Zero"
        _ZOffsetUnits("Depth Offset Units", Float) = 0                                                                // "Zero"
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.ColorWriteMask)] _ColorMask("Color Write Mask", Float) = 15 // "All"
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Float) = 0                                      // "Off"
        _RenderQueueOverride("Render Queue Override", Range(-1.0, 5000)) = -1
        [Toggle(_USE_WORLD_SCALE)] _UseWorldScale("Absolute Size", Float) = 1.0
        [Toggle(_STENCIL)] _EnableStencil("Enable Stencil Testing", Float) = 0.0
        _Stencil("Stencil Reference", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOp("Stencil Operation", Int) = 0
        _StencilWriteMask("Stencil Write Mask", Range(0, 255)) = 255
        _StencilReadMask("Stencil Read Mask", Range(0, 255)) = 255
        _ClipRect("Clip Rect", Vector) = (-32767.0, -32767.0, 32767.0, 32767.0)
        _ClipRectRadii("Clip Rect Radii", Vector) = (10.0, 10.0, 10.0, 10.0)
    }

    /// <summary>
    /// Sub Shader for the Universal Render Pipeline (URP).
    /// </summary>
    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.universal": "12.1.0"
        }

        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Fade"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        // Default pass (no meta pass needed for UI).
        Pass
        {
            Name "Main"
            Tags{ "LightMode" = "UniversalForward" }
            LOD 100
            Blend[_SrcBlend][_DstBlend],[_SrcBlendAlpha][_DstBlendAlpha]
            BlendOp[_BlendOp]
            ZTest[_ZTest]
            ZWrite[_ZWrite]
            Cull[_CullMode]
            Offset[_ZOffsetFactor],[_ZOffsetUnits]
            ColorMask[_ColorMask]

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            HLSLPROGRAM

            #define _URP
            #define _CANVAS_RENDERED

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ _UI_CLIP_RECT_ROUNDED _UI_CLIP_RECT_ROUNDED_INDEPENDENT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include_with_pragmas "GraphicsToolsStandardProgram.hlsl"

            ENDHLSL
        }
    }
    
    /// <summary>
    /// Sub Shader for the Built-in Render Pipeline.
    /// </summary>
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Fade"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        // Default pass (no meta pass needed for UI).
        Pass
        {
            Name "Main"
            Tags{ "LightMode" = "ForwardBase" }
            LOD 100
            Blend[_SrcBlend][_DstBlend],[_SrcBlendAlpha][_DstBlendAlpha]
            BlendOp[_BlendOp]
            ZTest[_ZTest]
            ZWrite[_ZWrite]
            Cull[_CullMode]
            Offset[_ZOffsetFactor],[_ZOffsetUnits]
            ColorMask[_ColorMask]

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            HLSLPROGRAM

            #define _CANVAS_RENDERED

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ _UI_CLIP_RECT_ROUNDED _UI_CLIP_RECT_ROUNDED_INDEPENDENT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            
            #include_with_pragmas "GraphicsToolsStandardProgram.hlsl"

            ENDHLSL
        }
    }
    
    CustomEditor "Microsoft.MixedReality.GraphicsTools.Editor.StandardShaderGUI"
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// How to treat the alpha channel of the albedo texture.
    /// </summary>
    public enum AlbedoAlphaMode
    {
        Transparency = 0,
        Metallic = 1,
        Smoothness = 2
    }

    /// <summary>
    /// What type of direct light affects the surface.
    /// </summary>
    public enum LightMode
    {
        Unlit = 0,
        LitDirectional = 1,
        LitDistant = 2
    }

    /// <summary>
    /// What type of gradient to generate.
    /// </summary>
    public enum GradientMode
    {
        None = 0,
        Iridescence = 1,
        FourPoint = 2,
        Linear = 3
    }

    /// <summary>
    /// How the border color should be calculated.
    /// </summary>
    public enum BorderColorMode
    {
        Brightness = 0,
        HoverColor = 1,
        Color = 2,
        Gradient = 3
    }

    /// <summary>
    /// Is edge smoothing controlled by a user defined value or programmatically.
    /// </summary>
    public enum EdgeSmoothingMode
    {
        Manual = 0,
        Automatic = 1
    }

    /// <summary>
    /// How to sample the blur texture.
    /// </summary>
    public enum BlurMode
    {
        None = 0,
        Layer1 = 1,
        Layer2 = 2,
        PrebakedBackground = 3
    }

    /// <summary>
    /// A custom shader inspector for the "Graphics Tools/Standard" and "Graphics Tools/Standard Canvas" shaders.
    /// </summary>
    public class StandardShaderGUI : BaseShaderGUI
    {
        /// <summary>
        /// Common names, keywords, and tooltips.
        /// </summary>
        protected static class Styles
        {
            public static readonly string primaryMapsTitle = "Main Maps";
            public static readonly string renderingOptionsTitle = "Rendering Options";
            public static readonly string advancedOptionsTitle = "Advanced Options";
            public static readonly string fluentOptionsTitle = "Fluent Options";
            public static readonly string stencilComparisonName = "_StencilComparison";
            public static readonly string stencilOperationName = "_StencilOperation";
            public static readonly string disableAlbedoMapName = "_DISABLE_ALBEDO_MAP";
            public static readonly string albedoMapAlphaMetallicName = "_METALLIC_TEXTURE_ALBEDO_CHANNEL_A";
            public static readonly string albedoMapAlphaSmoothnessName = "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A";
            public static readonly string propertiesComponentHelp = "Use the {0} component(s) to control {1} properties.";
            public static readonly GUIContent albedo = new GUIContent("Albedo", "Albedo (RGB) and Transparency (Alpha)");
            public static readonly GUIContent albedoAssignedAtRuntime = new GUIContent("Assigned at Runtime", "As an optimization albedo operations are disabled when no albedo texture is specified. If a albedo texture will be specified at runtime enable this option.");
            public static readonly GUIContent alphaCutoff = new GUIContent("Alpha Cutoff", "Threshold for Alpha Cutoff");
            public static readonly GUIContent alphaFade = new GUIContent("Alpha Fade", "Fade value which can be altered separately from the albedo color.");
            public static readonly GUIContent metallic = new GUIContent("Metallic", "Metallic Value");
            public static readonly GUIContent smoothness = new GUIContent("Smoothness", "Smoothness Value");
            public static readonly GUIContent enableChannelMap = new GUIContent("Channel Map", "Enable Channel Map, a Channel Packing Texture That Follows Unity's Standard Channel Setup");
            public static readonly GUIContent channelMap = new GUIContent("Channel Map", "Metallic (Red), Occlusion (Green), Emission (Blue), Smoothness (Alpha)");
            public static readonly GUIContent enableNormalMap = new GUIContent("Normal Map", "Enable Normal Map");
            public static readonly GUIContent normalMap = new GUIContent("Normal Map"); 
            public static readonly GUIContent enableEmission = new GUIContent("Emission", "Enable Emission");
            public static readonly GUIContent emissiveColor = new GUIContent("Color");
            public static readonly GUIContent emissiveMap = new GUIContent("EmissionMap");
            public static readonly GUIContent enableTriplanarMapping = new GUIContent("Triplanar Mapping", "Enable Triplanar Mapping, a technique which programmatically generates UV coordinates");
            public static readonly GUIContent enableLocalSpaceTriplanarMapping = new GUIContent("Local Space", "If True Triplanar Mapping is Calculated in Local Space");
            public static readonly GUIContent triplanarMappingBlendSharpness = new GUIContent("Blend Sharpness", "The Power of the Blend with the Normal");
            public static readonly GUIContent enableSSAA = new GUIContent("Super Sample Anti-Aliasing", "Enable Super Sample Anti-Aliasing, a technique improves texture clarity at long distances");
            public static readonly GUIContent mipmapBias = new GUIContent("Mipmap Bias", "Degree to bias the mip map. A larger negative value reduces aliasing and improves clarity, but may decrease performance");
            public static readonly GUIContent lightMode = new GUIContent("Light Mode", "What Type of Direct Light Affects the Surface");
            public static readonly string[] lightModeNames = new string[] { "Unlit", "Lit - Directional", "Lit - Distant" };
            public static readonly string lightModeLitDirectional = "_DIRECTIONAL_LIGHT";
            public static readonly string lightModeLitDistant = "_DISTANT_LIGHT";
            public static readonly GUIContent nonPhotorealisticRendering = new GUIContent("Non-Photorealistic Rendering","Non-Photorealistic Rendering");
            public static readonly GUIContent specularHighlights = new GUIContent("Specular Highlights", "Calculate Specular Highlights");
            public static readonly GUIContent sphericalHarmonics = new GUIContent("Spherical Harmonics", "Read From Spherical Harmonics Data for Ambient Light");
            public static readonly GUIContent reflections = new GUIContent("Reflections", "Calculate Glossy Reflections");
            public static readonly GUIContent rimLight = new GUIContent("Rim Light", "Enable Rim (Fresnel) Lighting");
            public static readonly GUIContent rimColor = new GUIContent("Color", "Rim Highlight Color");
            public static readonly GUIContent rimPower = new GUIContent("Power", "Rim Highlight Saturation");
            public static readonly GUIContent vertexColors = new GUIContent("Vertex Colors", "Enable Vertex Color Tinting");
            public static readonly GUIContent vertexExtrusion = new GUIContent("Vertex Extrusion", "Enable Vertex Extrusion Along the Vertex Normal");
            public static readonly GUIContent vertexExtrusionValue = new GUIContent("Extrusion Value", "How Far to Extrude the Vertex Along the Vertex Normal");
            public static readonly GUIContent vertexExtrusionSmoothNormals = new GUIContent("Use Smooth Normals", "Should Vertex Extrusion use the Smooth Normals in UV3, or Default Normals");
            public static readonly GUIContent vertexExtrusionConstantWidth = new GUIContent("Constant Width", "Should the Extrusion Value be scaled by the pixels distance from the camera?");
            public static readonly GUIContent blendedClippingWidth = new GUIContent("Clipping Alpha Falloff", "Controls The Width of the Fade Region on Alpha Blended Materials. 0 Equals No Falloff.");
            public static readonly GUIContent clippingBorder = new GUIContent("Clipping Border", "Enable a Border Along the Clipping Primitive's Edge");
            public static readonly GUIContent clippingBorderWidth = new GUIContent("Width", "Width of the Clipping Border");
            public static readonly GUIContent clippingBorderColor = new GUIContent("Color", "Interpolated Color of the Clipping Border");
            public static readonly GUIContent nearPlaneFade = new GUIContent("Near Fade", "Objects Disappear (Turn to Black/Transparent) as the Camera (or Hover/Proximity Light) Nears Them");
            public static readonly GUIContent nearLightFade = new GUIContent("Use Light", "A Hover or Proximity Light (Rather Than the Camera) Determines Near Fade Distance");
            public static readonly GUIContent fadeBeginDistance = new GUIContent("Fade Begin", "Distance From Camera (or Hover/Proximity Light) to Begin Fade In");
            public static readonly GUIContent fadeCompleteDistance = new GUIContent("Fade Complete", "Distance From Camera (or Hover/Proximity Light) When Fade is Fully In");
            public static readonly GUIContent fadeMinValue = new GUIContent("Fade Min Value", "Clamps the Fade Amount to a Minimum Value");
            public static readonly GUIContent hoverLight = new GUIContent("Hover Light", "Enable utilization of Hover Light(s)");
            public static readonly GUIContent enableHoverColorOverride = new GUIContent("Override Color", "Override Global Hover Light Color for this Material");
            public static readonly GUIContent hoverColorOverride = new GUIContent("Color", "Override Hover Light Color");
            public static readonly GUIContent proximityLight = new GUIContent("Proximity Light", "Enable utilization of Proximity Light(s)");
            public static readonly GUIContent enableProximityLightColorOverride = new GUIContent("Override Color", "Override Global Proximity Light Color for this Material");
            public static readonly GUIContent proximityLightCenterColorOverride = new GUIContent("Center Color", "The Override Color of the ProximityLight Gradient at the Center (RGB) and (A) is Gradient Extent");
            public static readonly GUIContent proximityLightMiddleColorOverride = new GUIContent("Middle Color", "The Override Color of the ProximityLight Gradient at the Middle (RGB) and (A) is Gradient Extent");
            public static readonly GUIContent proximityLightOuterColorOverride = new GUIContent("Outer Color", "The Override Color of the ProximityLight Gradient at the Outer Edge (RGB) and (A) is Gradient Extent");
            public static readonly GUIContent proximityLightSubtractive = new GUIContent("Subtractive", "Proximity Lights Remove Light from a Surface, Used to Mimic a Shadow");
            public static readonly GUIContent proximityLightTwoSided = new GUIContent("Two Sided", "Proximity Lights Apply to Both Sides of a Surface");
            public static readonly GUIContent fluentLightIntensity = new GUIContent("Light Intensity", "Intensity Scaler for All Hover and Proximity Lights");
            public static readonly GUIContent roundCorners = new GUIContent("Round Corners", "(Assumes UVs Specify Borders of Surface, Works Best on Unity Cube, Quad, and Plane)");
            public static readonly GUIContent roundCornerRadius = new GUIContent("Unit Radius", "Rounded Rectangle Corner Unit Sphere Radius");
            public static readonly GUIContent roundCornerRadiusWorldScale = new GUIContent("Radius", "Rounded Rectangle Corner Sphere Radius in World Units");
            public static readonly GUIContent roundCornersRadius = new GUIContent("Corners Radius", "UpLeft-UpRight-BottomRight-BottomLeft");
            public static readonly GUIContent roundCornerMargin = new GUIContent("Margin %", "Distance From Geometry Edge");
            public static readonly GUIContent roundCornerMarginWorldScale = new GUIContent("Margin", "Distance From Geometry Edge in World Units");
            public static readonly GUIContent independentCorners = new GUIContent("Independent Corners", "Manage Each Corner Separately");
            public static readonly GUIContent roundCornersHideInterior = new GUIContent("Hide Interior", "Pixels Within the Rounded Rect Should be Discarded");
            public static readonly GUIContent borderLight = new GUIContent("Border Light", "Enable Border Lighting (Assumes UVs Specify Borders of Surface, Works Best on Unity Cube, Quad, and Plane)");
            public static readonly GUIContent borderLightReplacesAlbedo = new GUIContent("Replace Albedo", "Border Light Replaces Albedo (Replacement Rather Than Additive)");
            public static readonly GUIContent borderLightOpaque = new GUIContent("Opaque Borders", "Borders Override Alpha Value to Appear Opaque");
            public static readonly GUIContent borderWidth = new GUIContent("Width %", "Uniform Width Along Border as a % of the Smallest XYZ Dimension");
            public static readonly GUIContent borderWidthWorldScale = new GUIContent("Width", "Uniform Width Along Border in World Units");
            public static readonly GUIContent borderColorMode = new GUIContent("Color Mode", "How the Border is Colored");
            public static readonly string borderColorModeHoverColorName = "_BORDER_LIGHT_USES_HOVER_COLOR";
            public static readonly string borderColorModeColorName = "_BORDER_LIGHT_USES_COLOR";
            public static readonly string borderColorModeGradientName = "_BORDER_LIGHT_USES_GRADIENT";
            public static readonly GUIContent borderMinValue = new GUIContent("Brightness", "Brightness Scaler");
            public static readonly GUIContent borderColor = new GUIContent("Border Color", "Border Color");
            public static readonly GUIContent edgeSmoothingMode = new GUIContent("Edge Smoothing Mode", "Manual, specified anti aliasing value will be used. Automatic, the shader will calculate a anti aliasing value.");
            public static readonly string edgeSmoothingModeAutomaticName = "_EDGE_SMOOTHING_AUTOMATIC";
            public static readonly GUIContent edgeSmoothingValue = new GUIContent("Value", "Smoothing Factor Applied to SDF Edges");
            public static readonly GUIContent borderLightOpaqueAlpha = new GUIContent("Alpha", "Alpha value of \"opaque\" borders.");
            public static readonly GUIContent innerGlow = new GUIContent("Inner Glow", "Enable Inner Glow (Assumes UVs Specify Borders of Surface, Works Best on Unity Cube, Quad, and Plane)");
            public static readonly GUIContent innerGlowColor = new GUIContent("Color", "Inner Glow Color (RGB) and Intensity (A)");
            public static readonly GUIContent innerGlowPower = new GUIContent("Power", "Power Exponent to Control Glow");
            public static readonly GUIContent gradientMode = new GUIContent("Gradient Mode", "Specifies the Type of Color Gradient to Apply in UV Space");
            public static readonly string gradientModeIridescence = "_IRIDESCENCE";
            public static readonly string gradientModeFourPoint = "_GRADIENT_FOUR_POINT";
            public static readonly string gradientModeLinear = "_GRADIENT_LINEAR";
            public static readonly GUIContent iridescentSpectrumMap = new GUIContent("Spectrum Map", "Spectrum of Colors to Apply (Usually a Texture with ROYGBIV from Left to Right)");
            public static readonly GUIContent iridescenceIntensity = new GUIContent("Intensity", "Intensity of Iridescence");
            public static readonly GUIContent iridescenceThreshold = new GUIContent("Threshold", "Threshold Window to Sample From the Spectrum Map");
            public static readonly GUIContent iridescenceAngle = new GUIContent("Angle", "Surface Angle");
            public static readonly GUIContent fourPointGradientTopLeftColor = new GUIContent("Top Left", "Top Left Color at UV (0, 0)");
            public static readonly GUIContent fourPointGradientTopRightColor = new GUIContent("Top Right", "Top Right Color at UV (1, 0)");
            public static readonly GUIContent fourPointGradientBottomLeftColor = new GUIContent("Bottom Left", "Bottom Left Color at UV (0, 1)");
            public static readonly GUIContent fourPointGradientBottomRightColor = new GUIContent("Bottom Right", "Bottom Right Color at UV (1, 1)");
            public static readonly GUIContent fourPointGradientAutoFillButton = new GUIContent("Auto Fill", "Generates a Visually Pleasing Gradient Based on the Top Left Color");
            public static readonly GUIContent linearGradientHelp = new GUIContent("Note: Only the first 4 color (and first 4 alpha) keys will be used from the gradient editor.");
            public static readonly GUIContent linearGradientColor = new GUIContent("Color Keys", "Specify up to 4 Colors and 4 Alpha Values to Evaluate.");
            public static readonly GUIContent linearGradientAngle = new GUIContent("Angle", "By Default Gradients Go Top to Bottom the Angle Adjusts That Direction");
            public static readonly GUIContent linearCssGradient = new GUIContent("Import From CSS Gradient", "For Example Type: linear-gradient(red, yellow);");
            public static readonly GUIContent environmentColoring = new GUIContent("Environment Coloring", "Change Color Based on View");
            public static readonly GUIContent environmentColorThreshold = new GUIContent("Threshold", "Threshold When Environment Coloring Should Appear Based on Surface Normal");
            public static readonly GUIContent environmentColorIntensity = new GUIContent("Intensity", "Intensity (or Brightness) of the Environment Coloring");
            public static readonly GUIContent environmentColorX = new GUIContent("X-Axis Color", "Color Along the World Space X-Axis");
            public static readonly GUIContent environmentColorY = new GUIContent("Y-Axis Color", "Color Along the World Space Y-Axis");
            public static readonly GUIContent environmentColorZ = new GUIContent("Z-Axis Color", "Color Along the World Space Z-Axis");
            public static readonly GUIContent blurMode = new GUIContent("Blur Mode", "None, no blur texture is sampled. Layer1, _blurTexture is sampled from the AcrylicLayerManager. Layer2, _blurTexture2 is sampled from the AcrylicLayerManager.");
            public static readonly string blurModeLayer1Name = "_BLUR_TEXTURE";
            public static readonly string blurModeLayer2Name = "_BLUR_TEXTURE_2";
            public static readonly string blurModePrebakedBackgroundName = "_BLUR_TEXTURE_PREBAKED_BACKGROUND";
            public static readonly GUIContent blurTextureIntensity = new GUIContent("Blur Texture Intensity", "Scaler applied to the blur texture.");
            public static readonly GUIContent blurBorderIntensity = new GUIContent("Blur Border Intensity", "Scaler applied to the blur texture in conjunction with the Border Light feature.");
            public static readonly GUIContent blurBackgroundRect = new GUIContent("Blur Background Rect", "The rect specified by the CanvasRectProvider component to calculate adjusted texture coordinates.");
            public static readonly GUIContent stencil = new GUIContent("Enable Stencil Testing", "Enabled Stencil Testing Operations");
            public static readonly GUIContent stencilReference = new GUIContent("Stencil Reference", "Value to Compared Against (if Comparison is Anything but Always) and/or the Value to be Written to the Buffer (if Either Pass, Fail or ZFail is Set to Replace)");
            public static readonly GUIContent stencilComparison = new GUIContent("Stencil Comparison", "Function to Compare the Reference Value to");
            public static readonly GUIContent stencilOperation = new GUIContent("Stencil Operation", "What to do When the Stencil Test Passes");
            public static readonly GUIContent stencilWriteMask = new GUIContent("Stencil Write Mask", "Specifies Which Bits are Included in the Write Operation");
            public static readonly GUIContent stencilReadMask = new GUIContent("Stencil Read Mask", "Specifies Which Bits are Included in the Read Operation");
            public static readonly GUIContent useWorldScale = new GUIContent("Absolute Size", "Features That Require Object Scale (Round Corners, Border Light, etc.), Default to Using Absolute (World) Scale when Enabled");
        }

        protected MaterialProperty albedoMap;
        protected MaterialProperty albedoColor;
        protected MaterialProperty albedoAlphaMode;
        protected MaterialProperty albedoAssignedAtRuntime;
        protected MaterialProperty alphaCutoff;
        protected MaterialProperty alphaFade;
        protected MaterialProperty enableChannelMap;
        protected MaterialProperty channelMap;
        protected MaterialProperty enableNormalMap;
        protected MaterialProperty normalMap;
        protected MaterialProperty normalMapScale;
        protected MaterialProperty enableEmission;
        protected MaterialProperty emissiveColor;
        protected MaterialProperty emissiveMap;
        protected MaterialProperty enableTriplanarMapping;
        protected MaterialProperty enableLocalSpaceTriplanarMapping;
        protected MaterialProperty triplanarMappingBlendSharpness;
        protected MaterialProperty enableSSAA;
        protected MaterialProperty mipmapBias;
        protected MaterialProperty metallic;
        protected MaterialProperty smoothness;
        protected MaterialProperty lightMode;
        protected MaterialProperty specularHighlights;
        protected MaterialProperty sphericalHarmonics;
        protected MaterialProperty nonPhotorealisticRendering;
        protected MaterialProperty reflections;
        protected MaterialProperty rimLight;
        protected MaterialProperty rimColor;
        protected MaterialProperty rimPower;
        protected MaterialProperty vertexColors;
        protected MaterialProperty vertexExtrusion;
        protected MaterialProperty vertexExtrusionValue;
        protected MaterialProperty vertexExtrusionSmoothNormals;
        protected MaterialProperty vertexExtrusionConstantWidth;
        protected MaterialProperty blendedClippingWidth;
        protected MaterialProperty clippingBorder;
        protected MaterialProperty clippingBorderWidth;
        protected MaterialProperty clippingBorderColor;
        protected MaterialProperty nearPlaneFade;
        protected MaterialProperty nearLightFade;
        protected MaterialProperty fadeBeginDistance;
        protected MaterialProperty fadeCompleteDistance;
        protected MaterialProperty fadeMinValue;
        protected MaterialProperty hoverLight;
        protected MaterialProperty enableHoverColorOverride;
        protected MaterialProperty hoverColorOverride;
        protected MaterialProperty proximityLight;
        protected MaterialProperty enableProximityLightColorOverride;
        protected MaterialProperty proximityLightCenterColorOverride;
        protected MaterialProperty proximityLightMiddleColorOverride;
        protected MaterialProperty proximityLightOuterColorOverride;
        protected MaterialProperty proximityLightSubtractive;
        protected MaterialProperty proximityLightTwoSided;
        protected MaterialProperty fluentLightIntensity;
        protected MaterialProperty roundCorners;
        protected MaterialProperty roundCornerRadius;
        protected MaterialProperty roundCornerMargin;
        protected MaterialProperty independentCorners;
        protected MaterialProperty roundCornersRadius;
        protected MaterialProperty roundCornersHideInterior;
        protected MaterialProperty borderLight;
        protected MaterialProperty borderLightReplacesAlbedo;
        protected MaterialProperty borderLightOpaque;
        protected MaterialProperty borderWidth;
        protected MaterialProperty borderColorMode;
        protected MaterialProperty borderMinValue;
        protected MaterialProperty borderColor;
        protected MaterialProperty edgeSmoothingMode;
        protected MaterialProperty edgeSmoothingValue;
        protected MaterialProperty borderLightOpaqueAlpha;
        protected MaterialProperty innerGlow;
        protected MaterialProperty innerGlowColor;
        protected MaterialProperty innerGlowPower;
        protected MaterialProperty gradientMode;
        protected MaterialProperty iridescentSpectrumMap;
        protected MaterialProperty iridescenceIntensity;
        protected MaterialProperty iridescenceThreshold;
        protected MaterialProperty iridescenceAngle;
        protected MaterialProperty gradientAngle;
        protected MaterialProperty gradientColor0;
        protected MaterialProperty gradientColor1;
        protected MaterialProperty gradientColor2;
        protected MaterialProperty gradientColor3;
        protected MaterialProperty gradientColor4;
        protected MaterialProperty gradientAlpha;
        protected MaterialProperty gradientAlphaTime;
        protected MaterialProperty environmentColoring;
        protected MaterialProperty environmentColorThreshold;
        protected MaterialProperty environmentColorIntensity;
        protected MaterialProperty environmentColorX;
        protected MaterialProperty environmentColorY;
        protected MaterialProperty environmentColorZ;
        protected MaterialProperty blurMode;
        protected MaterialProperty blurTextureIntensity;
        protected MaterialProperty blurBorderIntensity;
        protected MaterialProperty blurBackgroundRect;
        protected MaterialProperty enableStencil;
        protected MaterialProperty stencilReference;
        protected MaterialProperty stencilComparison;
        protected MaterialProperty stencilOperation;
        protected MaterialProperty stencilWriteMask;
        protected MaterialProperty stencilReadMask;
        protected MaterialProperty useWorldScale;

        protected string cssGradient = string.Empty;
        protected bool cssGradientValid = false;

        /// <summary>
        /// Looks for  properties associated with various render features.
        /// </summary>
        /// <param name="props">Material properties to search.</param>
        protected override void FindProperties(MaterialProperty[] props)
        {
            base.FindProperties(props);

            albedoMap = FindProperty("_MainTex", props);
            albedoColor = FindProperty("_Color", props);
            albedoAlphaMode = FindProperty("_AlbedoAlphaMode", props);
            albedoAssignedAtRuntime = FindProperty("_AlbedoAssignedAtRuntime", props);
            alphaCutoff = FindProperty("_Cutoff", props);
            alphaFade = FindProperty("_Fade", props);
            metallic = FindProperty("_Metallic", props);
            smoothness = FindProperty("_Smoothness", props);
            enableChannelMap = FindProperty("_EnableChannelMap", props);
            channelMap = FindProperty("_ChannelMap", props);
            enableNormalMap = FindProperty("_EnableNormalMap", props);
            normalMap = FindProperty("_NormalMap", props);
            normalMapScale = FindProperty("_NormalMapScale", props);
            enableEmission = FindProperty("_EnableEmission", props);
            emissiveMap = FindProperty("_EmissiveMap", props);
            emissiveColor = FindProperty("_EmissiveColor", props);
            enableTriplanarMapping = FindProperty("_EnableTriplanarMapping", props);
            enableLocalSpaceTriplanarMapping = FindProperty("_EnableLocalSpaceTriplanarMapping", props);
            triplanarMappingBlendSharpness = FindProperty("_TriplanarMappingBlendSharpness", props);
            enableSSAA = FindProperty("_EnableSSAA", props);
            mipmapBias = FindProperty("_MipmapBias", props);
            lightMode = FindProperty("_DirectionalLight", props);
            specularHighlights = FindProperty("_SpecularHighlights", props);
            sphericalHarmonics = FindProperty("_SphericalHarmonics", props);
            nonPhotorealisticRendering = FindProperty("_NPR", props);
            reflections = FindProperty("_Reflections", props);
            rimLight = FindProperty("_RimLight", props);
            rimColor = FindProperty("_RimColor", props);
            rimPower = FindProperty("_RimPower", props);
            vertexColors = FindProperty("_VertexColors", props);
            vertexExtrusion = FindProperty("_VertexExtrusion", props);
            vertexExtrusionValue = FindProperty("_VertexExtrusionValue", props);
            vertexExtrusionSmoothNormals = FindProperty("_VertexExtrusionSmoothNormals", props);
            vertexExtrusionConstantWidth = FindProperty("_VertexExtrusionConstantWidth", props);
            blendedClippingWidth = FindProperty("_BlendedClippingWidth", props);
            clippingBorder = FindProperty("_ClippingBorder", props);
            clippingBorderWidth = FindProperty("_ClippingBorderWidth", props);
            clippingBorderColor = FindProperty("_ClippingBorderColor", props);
            nearPlaneFade = FindProperty("_NearPlaneFade", props);
            nearLightFade = FindProperty("_NearLightFade", props);
            fadeBeginDistance = FindProperty("_FadeBeginDistance", props);
            fadeCompleteDistance = FindProperty("_FadeCompleteDistance", props);
            fadeMinValue = FindProperty("_FadeMinValue", props);
            hoverLight = FindProperty("_HoverLight", props);
            enableHoverColorOverride = FindProperty("_EnableHoverColorOverride", props);
            hoverColorOverride = FindProperty("_HoverColorOverride", props);
            proximityLight = FindProperty("_ProximityLight", props);
            enableProximityLightColorOverride = FindProperty("_EnableProximityLightColorOverride", props);
            proximityLightCenterColorOverride = FindProperty("_ProximityLightCenterColorOverride", props);
            proximityLightMiddleColorOverride = FindProperty("_ProximityLightMiddleColorOverride", props);
            proximityLightOuterColorOverride = FindProperty("_ProximityLightOuterColorOverride", props);
            proximityLightSubtractive = FindProperty("_ProximityLightSubtractive", props);
            proximityLightTwoSided = FindProperty("_ProximityLightTwoSided", props);
            fluentLightIntensity = FindProperty("_FluentLightIntensity", props);
            roundCorners = FindProperty("_RoundCorners", props);
            roundCornerRadius = FindProperty("_RoundCornerRadius", props);
            roundCornersRadius = FindProperty("_RoundCornersRadius", props);
            roundCornerMargin = FindProperty("_RoundCornerMargin", props);
            independentCorners = FindProperty("_IndependentCorners", props);
            roundCornersHideInterior = FindProperty("_RoundCornersHideInterior", props);
            borderLight = FindProperty("_BorderLight", props);
            borderLightReplacesAlbedo = FindProperty("_BorderLightReplacesAlbedo", props);
            borderLightOpaque = FindProperty("_BorderLightOpaque", props);
            borderWidth = FindProperty("_BorderWidth", props);
            borderColorMode = FindProperty("_BorderColorMode", props);
            borderMinValue = FindProperty("_BorderMinValue", props);
            borderColor = FindProperty("_BorderColor", props);
            edgeSmoothingMode = FindProperty("_EdgeSmoothingMode", props);
            edgeSmoothingValue = FindProperty("_EdgeSmoothingValue", props);
            borderLightOpaqueAlpha = FindProperty("_BorderLightOpaqueAlpha", props);
            innerGlow = FindProperty("_InnerGlow", props);
            innerGlowColor = FindProperty("_InnerGlowColor", props);
            innerGlowPower = FindProperty("_InnerGlowPower", props);
            gradientMode = FindProperty("_GradientMode", props);
            iridescentSpectrumMap = FindProperty("_IridescentSpectrumMap", props);
            iridescenceIntensity = FindProperty("_IridescenceIntensity", props);
            iridescenceThreshold = FindProperty("_IridescenceThreshold", props);
            iridescenceAngle = FindProperty("_IridescenceAngle", props);
            gradientAngle = FindProperty("_GradientAngle", props);
            gradientColor0 = FindProperty("_GradientColor0", props);
            gradientColor1 = FindProperty("_GradientColor1", props);
            gradientColor2 = FindProperty("_GradientColor2", props);
            gradientColor3 = FindProperty("_GradientColor3", props);
            gradientColor4 = FindProperty("_GradientColor4", props);
            gradientAlpha = FindProperty("_GradientAlpha", props);
            gradientAlphaTime = FindProperty("_GradientAlphaTime", props);
            environmentColoring = FindProperty("_EnvironmentColoring", props);
            environmentColorThreshold = FindProperty("_EnvironmentColorThreshold", props);
            environmentColorIntensity = FindProperty("_EnvironmentColorIntensity", props);
            environmentColorX = FindProperty("_EnvironmentColorX", props);
            environmentColorY = FindProperty("_EnvironmentColorY", props);
            environmentColorZ = FindProperty("_EnvironmentColorZ", props);
            blurMode = FindProperty("_BlurMode", props);
            blurTextureIntensity = FindProperty("_BlurTextureIntensity", props);
            blurBorderIntensity = FindProperty("_BlurBorderIntensity", props);
            blurBackgroundRect = FindProperty("_BlurBackgroundRect", props);
            enableStencil = FindProperty("_EnableStencil", props);
            stencilReference = FindProperty("_StencilReference", props, false);
            stencilComparison = FindProperty(Styles.stencilComparisonName, props, false);
            stencilOperation = FindProperty(Styles.stencilOperationName, props, false);
            // Handle Standard Canvas naming.
            stencilReference = stencilReference == null ? FindProperty("_Stencil", props) : stencilReference;
            stencilComparison = stencilComparison == null ? FindProperty("_StencilComp", props) : stencilComparison;
            stencilOperation = stencilOperation == null ? FindProperty("_StencilOp", props) : stencilOperation;
            stencilWriteMask = FindProperty("_StencilWriteMask", props);
            stencilReadMask = FindProperty("_StencilReadMask", props);
            useWorldScale = FindProperty("_UseWorldScale", props);
        }

        /// <summary>
        /// Renders the material inspector in a few different categories.
        /// </summary>
        /// <param name="materialEditor">Current material editor in use.</param>
        /// <param name="props">Material properties to search.</param>
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            Material material = (Material)materialEditor.target;

            base.OnGUI(materialEditor, props);

            MainMapOptions(materialEditor, material);
            RenderingOptions(materialEditor, material);
            FluentOptions(materialEditor, material);
            AdvancedOptions(materialEditor, material);
        }

        /// <summary>
        /// Attempts to copy material properties from other shaders to the Graphics Tools/Standard and Graphics Tools/Standard Canvas shaders.
        /// </summary>
        /// <param name="material">Current material.</param>
        /// <param name="oldShader">Previous shader.</param>
        /// <param name="newShader">Current shader.</param>
        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            // Cache old shader properties with potentially different names than the new shader.
            float? smoothness = GetFloatProperty(material, "_Glossiness");
            float? diffuse = GetFloatProperty(material, "_UseDiffuse");
            float? specularHighlights = GetFloatProperty(material, "_SpecularHighlights");
            float? normalMap = null;
            Texture normalMapTexture = material.HasProperty("_BumpMap") ? material.GetTexture("_BumpMap") : null;
            float? normalMapScale = GetFloatProperty(material, "_BumpScale");
            float? emission = null;
            Color? emissionColor = GetColorProperty(material, "_EmissionColor");
            Texture emissionMapTexture = material.HasProperty("_EmissionMap") ? material.GetTexture("_EmissionMap") : null;
            float? reflections = null;
            float? rimLighting = null;
            Vector4? textureScaleOffset = null;
            float? cullMode = GetFloatProperty(material, "_Cull");
            bool newShaderIsStandardCanvas = newShader.name == StandardShaderUtility.GraphicsToolsStandardCanvasShaderName;

            if (oldShader)
            {
                if (oldShader.name.Contains("Standard"))
                {
                    normalMap = material.IsKeywordEnabled("_NORMALMAP") ? 1.0f : 0.0f;
                    emission = material.IsKeywordEnabled("_EMISSION") ? 1.0f : 0.0f;
                    reflections = GetFloatProperty(material, "_GlossyReflections");
                }
                else if (oldShader.name.Contains("Fast Configurable"))
                {
                    normalMap = material.IsKeywordEnabled("_USEBUMPMAP_ON") ? 1.0f : 0.0f;
                    emission = GetFloatProperty(material, "_UseEmissionColor");
                    reflections = GetFloatProperty(material, "_UseReflections");
                    rimLighting = GetFloatProperty(material, "_UseRimLighting");
                    textureScaleOffset = GetVectorProperty(material, "_TextureScaleOffset");
                }
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            // Apply old shader properties to the new shader.
            SetShaderFeatureActive(material, null, "_Smoothness", smoothness);

            if (!newShaderIsStandardCanvas)
            {
                SetShaderFeatureActive(material, "_DIRECTIONAL_LIGHT", "_DirectionalLight", diffuse);
                SetShaderFeatureActive(material, "_SPECULAR_HIGHLIGHTS", "_SpecularHighlights", specularHighlights);
            }

            SetShaderFeatureActive(material, "_NORMAL_MAP", "_EnableNormalMap", normalMap);

            if (normalMapTexture)
            {
                material.SetTexture("_NormalMap", normalMapTexture);
            }
            
            SetShaderFeatureActive(material, null, "_NormalMapScale", normalMapScale);

            if (emissionMapTexture)
            {
                material.SetTexture("_EmissiveMap", emissionMapTexture);
            }

            SetShaderFeatureActive(material, "_EMISSION", "_EnableEmission", emission);
            SetColorProperty(material, "_EmissiveColor", emissionColor);

            if (!newShaderIsStandardCanvas)
            {
                SetShaderFeatureActive(material, "_REFLECTIONS", "_Reflections", reflections);
            }

            SetShaderFeatureActive(material, "_RIM_LIGHT", "_RimLight", rimLighting);
            SetVectorProperty(material, "_MainTex_ST", textureScaleOffset);
            SetShaderFeatureActive(material, null, "_CullMode", cullMode);

            // Setup the rendering mode based on the old shader.
            if (oldShader == null || !oldShader.name.Contains(LegacyShadersPath))
            {
                SetupMaterialWithRenderingMode(material, (RenderingMode)material.GetFloat(BaseStyles.renderingModeName), RenderingMode.Opaque, -1);
            }
            else
            {
                RenderingMode mode = RenderingMode.Opaque;

                if (oldShader.name.Contains(TransparentCutoutShadersPath))
                {
                    mode = RenderingMode.Cutout;
                }
                else if (oldShader.name.Contains(TransparentShadersPath))
                {
                    mode = RenderingMode.Fade;
                }

                material.SetFloat(BaseStyles.renderingModeName, (float)mode);

                MaterialChanged(material);
            }

            // Clear the main texture when going to the Standard Canvas shader since this will be specified by an image component. 
            if (newShaderIsStandardCanvas)
            {
                material.SetTexture("_MainTex", null);
            }
        }

        /// <inheritdoc/>
        protected override void MaterialChanged(Material material)
        {
            SetupMaterialWithAlbedo(material, albedoMap, albedoAlphaMode, albedoAssignedAtRuntime);

            // Ensure old materials with "_IRIDESCENCE" enabled also turn on the Iridescence gradient mode.
            if (material.IsKeywordEnabled("_IRIDESCENCE"))
            {
                SetShaderFeatureActive(material, Styles.gradientModeIridescence, "_GradientMode", (float)GradientMode.Iridescence);
            }

            base.MaterialChanged(material);
        }

        /// <summary>
        /// Displays inspectors settings that deal with texture sampling.
        /// </summary>
        /// <param name="materialEditor">Current material editor in use.</param>
        /// <param name="material">Current material in use.</param>
        protected void MainMapOptions(MaterialEditor materialEditor, Material material)
        {
            GUILayout.Label(Styles.primaryMapsTitle, EditorStyles.boldLabel);

            materialEditor.TexturePropertySingleLine(Styles.albedo, albedoMap, albedoColor);

            if (albedoMap.textureValue == null)
            {
                materialEditor.ShaderProperty(albedoAssignedAtRuntime, Styles.albedoAssignedAtRuntime, 2);
            }

            RenderingMode mode = (RenderingMode)renderingMode.floatValue;
            RenderingMode customMode = (RenderingMode)customRenderingMode.floatValue;

            if (mode == RenderingMode.Cutout || (mode == RenderingMode.Custom && customMode == RenderingMode.Cutout))
            {
                materialEditor.ShaderProperty(alphaCutoff, Styles.alphaCutoff);
            }

            if (mode != RenderingMode.Opaque && mode != RenderingMode.Cutout)
            {
                materialEditor.ShaderProperty(alphaFade, Styles.alphaFade);
            }

            materialEditor.ShaderProperty(enableChannelMap, Styles.enableChannelMap);

            if (PropertyEnabled(enableChannelMap))
            {
                EditorGUI.indentLevel += 2;
                materialEditor.TexturePropertySingleLine(Styles.channelMap, channelMap);
                GUILayout.Box("Metallic (Red), Occlusion (Green), Emission (Blue), Smoothness (Alpha)", EditorStyles.helpBox, Array.Empty<GUILayoutOption>());
                EditorGUI.indentLevel -= 2;
            }

            if (!PropertyEnabled(enableChannelMap))
            {
                EditorGUI.indentLevel += 2;

                materialEditor.ShaderProperty(albedoAlphaMode, albedoAlphaMode.displayName);

                if ((AlbedoAlphaMode)albedoAlphaMode.floatValue != AlbedoAlphaMode.Metallic)
                {
                    materialEditor.ShaderProperty(metallic, Styles.metallic);
                }

                if ((AlbedoAlphaMode)albedoAlphaMode.floatValue != AlbedoAlphaMode.Smoothness)
                {
                    materialEditor.ShaderProperty(smoothness, Styles.smoothness);
                }

                SetupMaterialWithAlbedo(material, albedoMap, albedoAlphaMode, albedoAssignedAtRuntime);

                EditorGUI.indentLevel -= 2;
            }

            if ((LightMode)lightMode.floatValue != LightMode.Unlit ||
                PropertyEnabled(reflections) ||
                PropertyEnabled(rimLight) ||
                PropertyEnabled(environmentColoring))
            {
                materialEditor.ShaderProperty(enableNormalMap, Styles.enableNormalMap);

                if (PropertyEnabled(enableNormalMap))
                {
                    EditorGUI.indentLevel += 2;
                    materialEditor.TexturePropertySingleLine(Styles.normalMap, normalMap, normalMapScale);
                    EditorGUI.indentLevel -= 2;
                }
            }

            materialEditor.ShaderProperty(enableEmission, Styles.enableEmission);

            if (PropertyEnabled(enableEmission))
            {
                EditorGUI.indentLevel += 2;
                materialEditor.TexturePropertySingleLine(Styles.emissiveColor, emissiveMap, emissiveColor);
                EditorGUI.indentLevel -= 2;
            }

            GUI.enabled = !PropertyEnabled(enableSSAA);
            materialEditor.ShaderProperty(enableTriplanarMapping, Styles.enableTriplanarMapping);

            if (PropertyEnabled(enableTriplanarMapping))
            {
                materialEditor.ShaderProperty(enableLocalSpaceTriplanarMapping, Styles.enableLocalSpaceTriplanarMapping, 2);
                materialEditor.ShaderProperty(triplanarMappingBlendSharpness, Styles.triplanarMappingBlendSharpness, 2);
            }
            GUI.enabled = true;

            GUI.enabled = !PropertyEnabled(enableTriplanarMapping);
            // SSAA implementation based off this article: https://medium.com/@bgolus/sharper-mipmapping-using-shader-based-supersampling-ed7aadb47bec
            materialEditor.ShaderProperty(enableSSAA, Styles.enableSSAA);

            if (PropertyEnabled(enableSSAA))
            {
                materialEditor.ShaderProperty(mipmapBias, Styles.mipmapBias, 2);
            }
            GUI.enabled = true;

            EditorGUILayout.Space();
            materialEditor.TextureScaleOffsetProperty(albedoMap);
        }

        /// <summary>
        /// Displays inspectors settings for more generic render features.
        /// </summary>
        /// <param name="materialEditor">Current material editor in use.</param>
        /// <param name="material">Current material in use.</param>
        protected void RenderingOptions(MaterialEditor materialEditor, Material material)
        {
            EditorGUILayout.Space();
            GUILayout.Label(Styles.renderingOptionsTitle, EditorStyles.boldLabel);

            lightMode.floatValue = EditorGUILayout.Popup(Styles.lightMode, (int)lightMode.floatValue, Styles.lightModeNames);

            switch ((LightMode)lightMode.floatValue)
            {
                default:
                case LightMode.Unlit:
                    {
                        material.DisableKeyword(Styles.lightModeLitDirectional);
                        material.DisableKeyword(Styles.lightModeLitDistant);
                    }
                    break;
                case LightMode.LitDirectional:
                    {
                        material.EnableKeyword(Styles.lightModeLitDirectional);
                        material.DisableKeyword(Styles.lightModeLitDistant);
                    }
                    break;
                case LightMode.LitDistant:
                    {
                        material.DisableKeyword(Styles.lightModeLitDirectional);
                        material.EnableKeyword(Styles.lightModeLitDistant);

                        GUILayout.Box(string.Format(Styles.propertiesComponentHelp, nameof(DistantLight), Styles.lightModeNames[(int)LightMode.LitDistant]), EditorStyles.helpBox, Array.Empty<GUILayoutOption>());
                    }
                    break;
            }

            if ((LightMode)lightMode.floatValue != LightMode.Unlit)
            {
                materialEditor.ShaderProperty(specularHighlights, Styles.specularHighlights, 2);
                materialEditor.ShaderProperty(sphericalHarmonics, Styles.sphericalHarmonics, 2);
                materialEditor.ShaderProperty(nonPhotorealisticRendering, Styles.nonPhotorealisticRendering, 2);
            }

            materialEditor.ShaderProperty(reflections, Styles.reflections);

            materialEditor.ShaderProperty(rimLight, Styles.rimLight);

            if (PropertyEnabled(rimLight))
            {
                materialEditor.ShaderProperty(rimColor, Styles.rimColor, 2);
                materialEditor.ShaderProperty(rimPower, Styles.rimPower, 2);
            }

            materialEditor.ShaderProperty(vertexColors, Styles.vertexColors);

            materialEditor.ShaderProperty(vertexExtrusion, Styles.vertexExtrusion);

            if (PropertyEnabled(vertexExtrusion))
            {
                materialEditor.ShaderProperty(vertexExtrusionValue, Styles.vertexExtrusionValue, 2);
                materialEditor.ShaderProperty(vertexExtrusionSmoothNormals, Styles.vertexExtrusionSmoothNormals, 2);
                materialEditor.ShaderProperty(vertexExtrusionConstantWidth, Styles.vertexExtrusionConstantWidth, 2);
            }

            if ((RenderingMode)renderingMode.floatValue != RenderingMode.Opaque &&
                (RenderingMode)renderingMode.floatValue != RenderingMode.Cutout)
            {
                materialEditor.ShaderProperty(blendedClippingWidth, Styles.blendedClippingWidth);
                GUILayout.Box(string.Format(Styles.propertiesComponentHelp, nameof(ClippingPrimitive), "other clipping"), EditorStyles.helpBox, Array.Empty<GUILayoutOption>());
            }

            materialEditor.ShaderProperty(clippingBorder, Styles.clippingBorder);

            if (PropertyEnabled(clippingBorder))
            {
                materialEditor.ShaderProperty(clippingBorderWidth, Styles.clippingBorderWidth, 2);
                materialEditor.ShaderProperty(clippingBorderColor, Styles.clippingBorderColor, 2);
                GUILayout.Box(string.Format(Styles.propertiesComponentHelp, nameof(ClippingPrimitive), "other clipping"), EditorStyles.helpBox, Array.Empty<GUILayoutOption>());
            }

            materialEditor.ShaderProperty(nearPlaneFade, Styles.nearPlaneFade);

            if (PropertyEnabled(nearPlaneFade))
            {
                materialEditor.ShaderProperty(nearLightFade, Styles.nearLightFade, 2);
                materialEditor.ShaderProperty(fadeBeginDistance, Styles.fadeBeginDistance, 2);
                materialEditor.ShaderProperty(fadeCompleteDistance, Styles.fadeCompleteDistance, 2);
                materialEditor.ShaderProperty(fadeMinValue, Styles.fadeMinValue, 2);
            }
        }

        /// <summary>
        /// Displays inspectors settings for user interface related features.
        /// </summary>
        /// <param name="materialEditor">Current material editor in use.</param>
        /// <param name="material">Current material in use.</param>
        protected void FluentOptions(MaterialEditor materialEditor, Material material)
        {
            /// TODO - [Cameron-Micka] this function has grown quite large. Might make sense to break up into more logical sections?
            EditorGUILayout.Space();
            GUILayout.Label(Styles.fluentOptionsTitle, EditorStyles.boldLabel);
            RenderingMode mode = (RenderingMode)renderingMode.floatValue;
            RenderingMode customMode = (RenderingMode)customRenderingMode.floatValue;

            materialEditor.ShaderProperty(hoverLight, Styles.hoverLight);

            if (PropertyEnabled(hoverLight))
            {
                GUILayout.Box(string.Format(Styles.propertiesComponentHelp, nameof(HoverLight), Styles.hoverLight.text), EditorStyles.helpBox, Array.Empty<GUILayoutOption>());

                materialEditor.ShaderProperty(enableHoverColorOverride, Styles.enableHoverColorOverride, 2);

                if (PropertyEnabled(enableHoverColorOverride))
                {
                    materialEditor.ShaderProperty(hoverColorOverride, Styles.hoverColorOverride, 4);
                }
            }

            materialEditor.ShaderProperty(proximityLight, Styles.proximityLight);

            if (PropertyEnabled(proximityLight))
            {
                materialEditor.ShaderProperty(enableProximityLightColorOverride, Styles.enableProximityLightColorOverride, 2);

                if (PropertyEnabled(enableProximityLightColorOverride))
                {
                    materialEditor.ShaderProperty(proximityLightCenterColorOverride, Styles.proximityLightCenterColorOverride, 4);
                    materialEditor.ShaderProperty(proximityLightMiddleColorOverride, Styles.proximityLightMiddleColorOverride, 4);
                    materialEditor.ShaderProperty(proximityLightOuterColorOverride, Styles.proximityLightOuterColorOverride, 4);
                }

                materialEditor.ShaderProperty(proximityLightSubtractive, Styles.proximityLightSubtractive, 2);
                materialEditor.ShaderProperty(proximityLightTwoSided, Styles.proximityLightTwoSided, 2);
                GUILayout.Box(string.Format(Styles.propertiesComponentHelp, nameof(ProximityLight), Styles.proximityLight.text), EditorStyles.helpBox, Array.Empty<GUILayoutOption>());
            }

            materialEditor.ShaderProperty(borderLight, Styles.borderLight);

            if (PropertyEnabled(borderLight))
            {
                if (PropertyEnabled(useWorldScale))
                {
                    // Hide range.
                    EditorGUI.indentLevel += 2;
                    borderWidth.floatValue = EditorGUILayout.FloatField(Styles.borderWidthWorldScale, borderWidth.floatValue);
                    EditorGUI.indentLevel -= 2;
                }
                else
                {
                    materialEditor.ShaderProperty(borderWidth, Styles.borderWidth, 2);
                }

                materialEditor.ShaderProperty(borderColorMode, Styles.borderColorMode, 2);

                /// TODO - [Cameron-Micka] Could switch to using the KeywordEnum property drawer in the future.
                switch ((BorderColorMode)borderColorMode.floatValue)
                {
                    default:
                    case BorderColorMode.Brightness:
                        {
                            materialEditor.ShaderProperty(borderMinValue, Styles.borderMinValue, 2);

                            material.DisableKeyword(Styles.borderColorModeHoverColorName);
                            material.DisableKeyword(Styles.borderColorModeColorName);
                            material.DisableKeyword(Styles.borderColorModeGradientName);
                        }
                        break;
                    case BorderColorMode.HoverColor:
                        {
                            materialEditor.ShaderProperty(borderMinValue, Styles.borderMinValue, 2);
                            GUILayout.Box(string.Format("Enable the {0} property and {1} property to alter the color.", Styles.hoverLight.text, Styles.enableHoverColorOverride.text), EditorStyles.helpBox, Array.Empty<GUILayoutOption>());

                            material.EnableKeyword(Styles.borderColorModeHoverColorName);
                            material.DisableKeyword(Styles.borderColorModeColorName);
                            material.DisableKeyword(Styles.borderColorModeGradientName);
                        }
                        break;
                    case BorderColorMode.Color:
                        {
                            materialEditor.ShaderProperty(borderColor, Styles.borderColor, 2);

                            material.DisableKeyword(Styles.borderColorModeHoverColorName);
                            material.EnableKeyword(Styles.borderColorModeColorName);
                            material.DisableKeyword(Styles.borderColorModeGradientName);
                        }
                        break;
                    case BorderColorMode.Gradient:
                        {
                            GUILayout.Box(string.Format("Adjust the {0} property to alter the color.", Styles.gradientMode.text), EditorStyles.helpBox, Array.Empty<GUILayoutOption>());

                            material.DisableKeyword(Styles.borderColorModeHoverColorName);
                            material.DisableKeyword(Styles.borderColorModeColorName);
                            material.EnableKeyword(Styles.borderColorModeGradientName);
                        }
                        break;
                }

                materialEditor.ShaderProperty(borderLightReplacesAlbedo, Styles.borderLightReplacesAlbedo, 2);

                if (mode == RenderingMode.Cutout || mode == RenderingMode.Fade || mode == RenderingMode.Transparent ||
                    (mode == RenderingMode.Custom && customMode == RenderingMode.Cutout) ||
                    (mode == RenderingMode.Custom && customMode == RenderingMode.Fade))
                {
                    materialEditor.ShaderProperty(borderLightOpaque, Styles.borderLightOpaque, 2);

                    if (PropertyEnabled(borderLightOpaque))
                    {
                        materialEditor.ShaderProperty(borderLightOpaqueAlpha, Styles.borderLightOpaqueAlpha, 4);
                    }
                }
            }
            
            if (!PropertyEnabled(roundCorners) && PropertyEnabled(borderLight))
            {
                materialEditor.ShaderProperty(edgeSmoothingMode, Styles.edgeSmoothingMode, 2);

                switch ((EdgeSmoothingMode)edgeSmoothingMode.floatValue)
                {
                    default:
                    case EdgeSmoothingMode.Manual:
                    {
                        material.DisableKeyword(Styles.edgeSmoothingModeAutomaticName);
                        materialEditor.ShaderProperty(edgeSmoothingValue, Styles.edgeSmoothingValue, 3);
                    }
                        break;
                    case EdgeSmoothingMode.Automatic:
                    {
                        material.EnableKeyword(Styles.edgeSmoothingModeAutomaticName);
                    }
                        break;
                }
            }

            if (PropertyEnabled(hoverLight) || PropertyEnabled(proximityLight) || PropertyEnabled(borderLight))
            {
                materialEditor.ShaderProperty(fluentLightIntensity, Styles.fluentLightIntensity, 2);
            }

            materialEditor.ShaderProperty(roundCorners, Styles.roundCorners);

            if (PropertyEnabled(roundCorners))
            {
                materialEditor.ShaderProperty(independentCorners, Styles.independentCorners, 2);

                if (PropertyEnabled(independentCorners))
                {
                    materialEditor.ShaderProperty(roundCornersRadius, Styles.roundCornersRadius, 2);
                }
                else
                {
                    if (PropertyEnabled(useWorldScale))
                    {
                        // Hide range.
                        EditorGUI.indentLevel += 2;
                        roundCornerRadius.floatValue = EditorGUILayout.FloatField(Styles.roundCornerRadiusWorldScale, roundCornerRadius.floatValue);
                        EditorGUI.indentLevel -= 2;
                    }
                    else
                    {
                        materialEditor.ShaderProperty(roundCornerRadius, Styles.roundCornerRadius, 2);
                    }
                }

                if (PropertyEnabled(useWorldScale))
                {
                    // Hide range.
                    EditorGUI.indentLevel += 2;
                    roundCornerMargin.floatValue = EditorGUILayout.FloatField(Styles.roundCornerMarginWorldScale, roundCornerMargin.floatValue);
                    EditorGUI.indentLevel -= 2;
                }
                else
                {
                    materialEditor.ShaderProperty(roundCornerMargin, Styles.roundCornerMargin, 2);
                }

                if ((RenderingMode)renderingMode.floatValue != RenderingMode.Opaque &&
                    (RenderingMode)renderingMode.floatValue != RenderingMode.Cutout)
                {
                    materialEditor.ShaderProperty(roundCornersHideInterior, Styles.roundCornersHideInterior, 2);
                }
            }

            if (PropertyEnabled(roundCorners))
            {
                materialEditor.ShaderProperty(edgeSmoothingMode, Styles.edgeSmoothingMode, 2);

                switch ((EdgeSmoothingMode)edgeSmoothingMode.floatValue)
                {
                    default:
                    case EdgeSmoothingMode.Manual:
                        {
                            material.DisableKeyword(Styles.edgeSmoothingModeAutomaticName);
                            materialEditor.ShaderProperty(edgeSmoothingValue, Styles.edgeSmoothingValue, 3);
                        }
                        break;
                    case EdgeSmoothingMode.Automatic:
                        {
                            material.EnableKeyword(Styles.edgeSmoothingModeAutomaticName);
                        }
                        break;
                }
            }

            materialEditor.ShaderProperty(innerGlow, Styles.innerGlow);

            if (PropertyEnabled(innerGlow))
            {
                materialEditor.ShaderProperty(innerGlowColor, Styles.innerGlowColor, 2);
                materialEditor.ShaderProperty(innerGlowPower, Styles.innerGlowPower, 2);
            }

            materialEditor.ShaderProperty(gradientMode, Styles.gradientMode);

            switch ((GradientMode)gradientMode.floatValue)
            {
                default:
                case GradientMode.None:
                    {
                        material.DisableKeyword(Styles.gradientModeIridescence);
                        material.DisableKeyword(Styles.gradientModeFourPoint);
                        material.DisableKeyword(Styles.gradientModeLinear);
                    }
                    break;
                case GradientMode.Iridescence:
                    {
                        EditorGUI.indentLevel += 2;
                        materialEditor.TexturePropertySingleLine(Styles.iridescentSpectrumMap, iridescentSpectrumMap);
                        EditorGUI.indentLevel -= 2;
                        materialEditor.ShaderProperty(iridescenceIntensity, Styles.iridescenceIntensity, 2);
                        materialEditor.ShaderProperty(iridescenceThreshold, Styles.iridescenceThreshold, 2);
                        materialEditor.ShaderProperty(iridescenceAngle, Styles.iridescenceAngle, 2);

                        material.EnableKeyword(Styles.gradientModeIridescence);
                        material.DisableKeyword(Styles.gradientModeFourPoint);
                        material.DisableKeyword(Styles.gradientModeLinear);
                    }
                    break;
                case GradientMode.FourPoint:
                    {
                        materialEditor.ShaderProperty(gradientColor1, Styles.fourPointGradientTopLeftColor, 2);
                        materialEditor.ShaderProperty(gradientColor2, Styles.fourPointGradientTopRightColor, 2);
                        materialEditor.ShaderProperty(gradientColor3, Styles.fourPointGradientBottomLeftColor, 2);
                        materialEditor.ShaderProperty(gradientColor4, Styles.fourPointGradientBottomRightColor, 2);

                        if (GUILayout.Button(Styles.fourPointGradientAutoFillButton))
                        {
                            Color topLeft, topRight, bottomLeft, bottomRight, stroke;
                            StandardShaderUtility.AutofillFourPointGradient(gradientColor1.colorValue, out topLeft, out topRight, out bottomLeft, out bottomRight, out stroke);

                            gradientColor1.colorValue = topLeft;
                            gradientColor2.colorValue = topRight;
                            gradientColor3.colorValue = bottomLeft;
                            gradientColor4.colorValue = bottomRight;
                            borderColor.colorValue = stroke;
                        }

                        material.DisableKeyword(Styles.gradientModeIridescence);
                        material.EnableKeyword(Styles.gradientModeFourPoint);
                        material.DisableKeyword(Styles.gradientModeLinear);
                    }
                    break;

                case GradientMode.Linear:
                    {
                        const int maxKeys = 4;

                        GradientColorKey ToGradientColorKey(MaterialProperty property)
                        {
                            Color color = property.colorValue;
                            return new GradientColorKey(color, color.a);
                        }

                        GradientAlphaKey[] ToGradientAlphaKey(MaterialProperty propertyA,
                                                              MaterialProperty propertyT)
                        {
                            Vector4 alphas = propertyA.vectorValue;
                            Vector4 times = propertyT.vectorValue;
                            return new GradientAlphaKey[] { new GradientAlphaKey(alphas.x, times.x),
                                                            new GradientAlphaKey(alphas.y, times.y),
                                                            new GradientAlphaKey(alphas.z, times.z),
                                                            new GradientAlphaKey(alphas.w, times.w) };
                        }

                        EditorGUI.BeginChangeCheck();
                        {
                            Gradient gradient = new Gradient();
                            gradient.SetKeys(new GradientColorKey[] { ToGradientColorKey(gradientColor0),
                                                                      ToGradientColorKey(gradientColor1),
                                                                      ToGradientColorKey(gradientColor2),
                                                                      ToGradientColorKey(gradientColor3) },
                                             ToGradientAlphaKey(gradientAlpha, gradientAlphaTime));
                            EditorGUI.indentLevel += 2;
                            EditorGUILayout.HelpBox(Styles.linearGradientHelp);
                            gradient = EditorGUILayout.GradientField(Styles.linearGradientColor, gradient);
                            EditorGUI.indentLevel -= 2;
                            if (EditorGUI.EndChangeCheck())
                            {
                                List<GradientColorKey> colorKeys = new List<GradientColorKey>(gradient.colorKeys);

                                // Ensure we always have 'maxKeys' colors keys.
                                while (colorKeys.Count < maxKeys)
                                {
                                    colorKeys.Add(colorKeys[colorKeys.Count - 1]);
                                }

                                Color ToColorTime(GradientColorKey colorKey)
                                {
                                    return new Color(colorKey.color.r, colorKey.color.g, colorKey.color.b, colorKey.time);
                                }

                                gradientColor0.colorValue = ToColorTime(colorKeys[0]);
                                gradientColor1.colorValue = ToColorTime(colorKeys[1]);
                                gradientColor2.colorValue = ToColorTime(colorKeys[2]);
                                gradientColor3.colorValue = ToColorTime(colorKeys[3]);

                                List<GradientAlphaKey> alphaKeys = new List<GradientAlphaKey>(gradient.alphaKeys);

                                // Ensure we always have 'maxKeys' alpha keys.
                                while (alphaKeys.Count < maxKeys)
                                {
                                    alphaKeys.Add(alphaKeys[alphaKeys.Count - 1]);
                                }

                                gradientAlpha.vectorValue = new Vector4(alphaKeys[0].alpha, alphaKeys[1].alpha, alphaKeys[2].alpha, alphaKeys[3].alpha);
                                gradientAlphaTime.vectorValue = new Vector4(alphaKeys[0].time, alphaKeys[1].time, alphaKeys[2].time, alphaKeys[3].time);
                            }
                        }

                        materialEditor.ShaderProperty(gradientAngle, Styles.linearGradientAngle, 2);

                        EditorGUI.indentLevel += 2;
                        EditorGUILayout.LabelField(Styles.linearCssGradient, EditorStyles.boldLabel);

                        EditorGUI.BeginChangeCheck();
                        {
                            EditorGUILayout.BeginHorizontal("Label");
                            {
                                cssGradient = EditorGUILayout.TextField(cssGradient);
                                GUILayout.Box(string.IsNullOrEmpty(cssGradient) ?
                                   EditorGUIUtility.IconContent("orangeLight") :
                                   cssGradientValid ?
                                   EditorGUIUtility.IconContent("greenLight") :
                                   EditorGUIUtility.IconContent("redLight"),
                                   GUILayout.Height(16));
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUI.indentLevel -= 2;

                            if (EditorGUI.EndChangeCheck())
                            {
                                Color[] gradientColors;
                                float[] gradientKeys;
                                float angle;

                                if (StandardShaderUtility.TryParseCSSGradient(cssGradient, out gradientColors, out gradientKeys, out angle))
                                {
                                    List<Color> colors = new List<Color>(gradientColors);
                                    List<float> keys = new List<float>(gradientKeys);

                                    // Ensure we always have 'maxKeys' colors/keys.
                                    while (colors.Count < maxKeys)
                                    {
                                        colors.Add(colors[colors.Count - 1]);
                                        keys.Add(keys[keys.Count - 1]);
                                    }

                                    Color ToColorTime(Color color, float key)
                                    {
                                        return new Color(color.r, color.g, color.b, key);
                                    }

                                    gradientColor0.colorValue = ToColorTime(colors[0], keys[0]);
                                    gradientColor1.colorValue = ToColorTime(colors[1], keys[1]);
                                    gradientColor2.colorValue = ToColorTime(colors[2], keys[2]);
                                    gradientColor3.colorValue = ToColorTime(colors[3], keys[3]);
                                    gradientAlpha.vectorValue = new Color(colors[0].a, colors[1].a, colors[2].a, colors[3].a);
                                    gradientAlphaTime.vectorValue = new Color(keys[0], keys[1], keys[2], keys[3]);
                                    gradientAngle.floatValue = angle;

                                    cssGradientValid = true;
                                }
                                else
                                {
                                    cssGradientValid = false;
                                }
                            }
                        }

                        material.DisableKeyword(Styles.gradientModeIridescence);
                        material.DisableKeyword(Styles.gradientModeFourPoint);
                        material.EnableKeyword(Styles.gradientModeLinear);
                    }
                    break;
            }

            materialEditor.ShaderProperty(environmentColoring, Styles.environmentColoring);

            if (PropertyEnabled(environmentColoring))
            {
                materialEditor.ShaderProperty(environmentColorThreshold, Styles.environmentColorThreshold, 2);
                materialEditor.ShaderProperty(environmentColorIntensity, Styles.environmentColorIntensity, 2);
                materialEditor.ShaderProperty(environmentColorX, Styles.environmentColorX, 2);
                materialEditor.ShaderProperty(environmentColorY, Styles.environmentColorY, 2);
                materialEditor.ShaderProperty(environmentColorZ, Styles.environmentColorZ, 2);
            }

            materialEditor.ShaderProperty(blurMode, Styles.blurMode);
            BlurMode currentBlurMode = (BlurMode)blurMode.floatValue;

            switch (currentBlurMode)
            {
                default:
                case BlurMode.None:
                    {
                        material.DisableKeyword(Styles.blurModeLayer1Name);
                        material.DisableKeyword(Styles.blurModeLayer2Name);
                        material.DisableKeyword(Styles.blurModePrebakedBackgroundName);
                    }
                    break;
                case BlurMode.Layer1:
                    {
                        material.EnableKeyword(Styles.blurModeLayer1Name);
                        material.DisableKeyword(Styles.blurModeLayer2Name);
                        material.DisableKeyword(Styles.blurModePrebakedBackgroundName);
                    }
                    break;
                case BlurMode.Layer2:
                    {
                        material.DisableKeyword(Styles.blurModeLayer1Name);
                        material.EnableKeyword(Styles.blurModeLayer2Name);
                        material.DisableKeyword(Styles.blurModePrebakedBackgroundName);
                    }
                    break;
                case BlurMode.PrebakedBackground:
                    {
                        material.DisableKeyword(Styles.blurModeLayer1Name);
                        material.DisableKeyword(Styles.blurModeLayer2Name);
                        material.EnableKeyword(Styles.blurModePrebakedBackgroundName);
                    }
                    break;
            }

            if (currentBlurMode != BlurMode.None)
            {
                materialEditor.ShaderProperty(blurTextureIntensity, Styles.blurTextureIntensity, 2);

                if (PropertyEnabled(borderLight))
                {
                    materialEditor.ShaderProperty(blurBorderIntensity, Styles.blurBorderIntensity, 2);
                }

                if (currentBlurMode == BlurMode.PrebakedBackground)
                {
                    GUI.enabled = false;
                    materialEditor.ShaderProperty(blurBackgroundRect, Styles.blurBackgroundRect, 2);
                    GUI.enabled = true;
                }
            }
        }

        /// <summary>
        /// Displays inspectors settings for features users don't need to alter often.
        /// </summary>
        /// <param name="materialEditor">Current material editor in use.</param>
        /// <param name="material">Current material in use.</param>
        protected void AdvancedOptions(MaterialEditor materialEditor, Material material)
        {
            EditorGUILayout.Space();
            GUILayout.Label(Styles.advancedOptionsTitle, EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            materialEditor.ShaderProperty(renderQueueOverride, BaseStyles.renderQueueOverride);

            if (EditorGUI.EndChangeCheck())
            {
                MaterialChanged(material);
            }

            // Show the RenderQueueField but do not allow users to directly manipulate it. That is done via the renderQueueOverride.
            GUI.enabled = false;
            materialEditor.RenderQueueField();
            GUI.enabled = true;

            materialEditor.EnableInstancingField();

            materialEditor.ShaderProperty(enableStencil, Styles.stencil);

            if (PropertyEnabled(enableStencil))
            {
                materialEditor.ShaderProperty(stencilReference, Styles.stencilReference, 2);
                materialEditor.ShaderProperty(stencilComparison, Styles.stencilComparison, 2);
                materialEditor.ShaderProperty(stencilOperation, Styles.stencilOperation, 2);
                materialEditor.ShaderProperty(stencilWriteMask, Styles.stencilWriteMask, 2);
                materialEditor.ShaderProperty(stencilReadMask, Styles.stencilReadMask, 2);
            }
            else
            {
                // When stencil is disable, revert to the default stencil operations. Note, when tested on D3D11 hardware the stencil state 
                // is still set even when the CompareFunction.Disabled is selected, but this does not seem to affect performance.
                material.SetInt(Styles.stencilComparisonName, (int)CompareFunction.Disabled);
                material.SetInt(Styles.stencilOperationName, (int)StencilOp.Keep);
            }

            bool scaleRequired = ScaleRequired();

            if (scaleRequired)
            {
                materialEditor.ShaderProperty(useWorldScale, Styles.useWorldScale);
            }

            // Static and dynamic batching will normalize the object scale, which breaks features which utilize object scale.
            material.SetOverrideTag("DisableBatching", scaleRequired ? "True" : "False");
        }

        /// <summary>
        /// Returns true if the material has a feature enabled that requires world scale of the object be known.
        /// </summary>
        /// <returns>True if scale is required to render properly.</returns>
        protected bool ScaleRequired()
        {
            return PropertyEnabled(vertexExtrusion) ||
                   PropertyEnabled(roundCorners) ||
                   PropertyEnabled(borderLight) ||
                   ((GradientMode)gradientMode.floatValue == GradientMode.Linear) ||
                   (PropertyEnabled(enableTriplanarMapping) && PropertyEnabled(enableLocalSpaceTriplanarMapping));
        }

        /// <summary>
        /// Enables/disables features based on the albedo settings.
        /// </summary>
        /// <param name="material">Current material in use.</param>
        /// <param name="albedoMap">Albedo texture.</param>
        /// <param name="albedoAlphaMode">Albedo alpha channel usage.</param>
        /// <param name="albedoAssignedAtRuntime">Flag if the albedo map can be added at runtime, disables optimizations if true.</param>
        protected static void SetupMaterialWithAlbedo(Material material, MaterialProperty albedoMap, MaterialProperty albedoAlphaMode, MaterialProperty albedoAssignedAtRuntime)
        {
            if (albedoMap.textureValue || PropertyEnabled(albedoAssignedAtRuntime))
            {
                material.DisableKeyword(Styles.disableAlbedoMapName);
            }
            else
            {
                material.EnableKeyword(Styles.disableAlbedoMapName);
            }

            switch ((AlbedoAlphaMode)albedoAlphaMode.floatValue)
            {
                case AlbedoAlphaMode.Transparency:
                    {
                        material.DisableKeyword(Styles.albedoMapAlphaMetallicName);
                        material.DisableKeyword(Styles.albedoMapAlphaSmoothnessName);
                    }
                    break;

                case AlbedoAlphaMode.Metallic:
                    {
                        material.EnableKeyword(Styles.albedoMapAlphaMetallicName);
                        material.DisableKeyword(Styles.albedoMapAlphaSmoothnessName);
                    }
                    break;

                case AlbedoAlphaMode.Smoothness:
                    {
                        material.DisableKeyword(Styles.albedoMapAlphaMetallicName);
                        material.EnableKeyword(Styles.albedoMapAlphaSmoothnessName);
                    }
                    break;
            }
        }
    }
}

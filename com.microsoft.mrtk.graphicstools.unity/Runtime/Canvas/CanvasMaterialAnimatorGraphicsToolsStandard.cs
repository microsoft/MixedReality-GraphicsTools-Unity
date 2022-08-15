// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// This class was auto generated via Assets > Graphics Tools > Generate Canvas Material Animator.
    /// Use Unity's animation system to animate fields on this class to drive material properties on CanvasRenderers.
    /// Version=0.1.0
    /// </summary>
    public class CanvasMaterialAnimatorGraphicsToolsStandard : CanvasMaterialAnimatorBase
    {
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _Color = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ColorID = Shader.PropertyToID("_Color");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Texture2D _MainTex = null;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _MainTexID = Shader.PropertyToID("_MainTex");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _AlbedoAlphaMode = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _AlbedoAlphaModeID = Shader.PropertyToID("_AlbedoAlphaMode");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _AlbedoAssignedAtRuntime = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _AlbedoAssignedAtRuntimeID = Shader.PropertyToID("_AlbedoAssignedAtRuntime");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _Cutoff = 0.5f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _CutoffID = Shader.PropertyToID("_Cutoff");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _Fade = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _FadeID = Shader.PropertyToID("_Fade");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _Metallic = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _MetallicID = Shader.PropertyToID("_Metallic");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _Smoothness = 0.5f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _SmoothnessID = Shader.PropertyToID("_Smoothness");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _EnableChannelMap = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnableChannelMapID = Shader.PropertyToID("_EnableChannelMap");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Texture2D _ChannelMap = null;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ChannelMapID = Shader.PropertyToID("_ChannelMap");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _EnableNormalMap = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnableNormalMapID = Shader.PropertyToID("_EnableNormalMap");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Texture2D _NormalMap = null;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _NormalMapID = Shader.PropertyToID("_NormalMap");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _NormalMapScale = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _NormalMapScaleID = Shader.PropertyToID("_NormalMapScale");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _EnableEmission = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnableEmissionID = Shader.PropertyToID("_EnableEmission");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _EmissiveColor = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EmissiveColorID = Shader.PropertyToID("_EmissiveColor");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Texture2D _EmissiveMap = null;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EmissiveMapID = Shader.PropertyToID("_EmissiveMap");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _EnableTriplanarMapping = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnableTriplanarMappingID = Shader.PropertyToID("_EnableTriplanarMapping");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _EnableLocalSpaceTriplanarMapping = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnableLocalSpaceTriplanarMappingID = Shader.PropertyToID("_EnableLocalSpaceTriplanarMapping");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(1f, 16f)] public float _TriplanarMappingBlendSharpness = 4f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _TriplanarMappingBlendSharpnessID = Shader.PropertyToID("_TriplanarMappingBlendSharpness");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _EnableSSAA = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnableSSAAID = Shader.PropertyToID("_EnableSSAA");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(-5f, 0f)] public float _MipmapBias = -2f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _MipmapBiasID = Shader.PropertyToID("_MipmapBias");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _DirectionalLight = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _DirectionalLightID = Shader.PropertyToID("_DirectionalLight");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _SpecularHighlights = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _SpecularHighlightsID = Shader.PropertyToID("_SpecularHighlights");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _SphericalHarmonics = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _SphericalHarmonicsID = Shader.PropertyToID("_SphericalHarmonics");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _Reflections = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ReflectionsID = Shader.PropertyToID("_Reflections");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _RimLight = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _RimLightID = Shader.PropertyToID("_RimLight");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _RimColor = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _RimColorID = Shader.PropertyToID("_RimColor");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 8f)] public float _RimPower = 0.25f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _RimPowerID = Shader.PropertyToID("_RimPower");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _VertexColors = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _VertexColorsID = Shader.PropertyToID("_VertexColors");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _VertexExtrusion = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _VertexExtrusionID = Shader.PropertyToID("_VertexExtrusion");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _VertexExtrusionValue = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _VertexExtrusionValueID = Shader.PropertyToID("_VertexExtrusionValue");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _VertexExtrusionSmoothNormals = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _VertexExtrusionSmoothNormalsID = Shader.PropertyToID("_VertexExtrusionSmoothNormals");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 10f)] public float _BlendedClippingWidth = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BlendedClippingWidthID = Shader.PropertyToID("_BlendedClippingWidth");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _ClippingBorder = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ClippingBorderID = Shader.PropertyToID("_ClippingBorder");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _ClippingBorderWidth = 0.025f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ClippingBorderWidthID = Shader.PropertyToID("_ClippingBorderWidth");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _ClippingBorderColor = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ClippingBorderColorID = Shader.PropertyToID("_ClippingBorderColor");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _NearPlaneFade = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _NearPlaneFadeID = Shader.PropertyToID("_NearPlaneFade");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _NearLightFade = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _NearLightFadeID = Shader.PropertyToID("_NearLightFade");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 10f)] public float _FadeBeginDistance = 0.85f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _FadeBeginDistanceID = Shader.PropertyToID("_FadeBeginDistance");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 10f)] public float _FadeCompleteDistance = 0.5f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _FadeCompleteDistanceID = Shader.PropertyToID("_FadeCompleteDistance");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _FadeMinValue = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _FadeMinValueID = Shader.PropertyToID("_FadeMinValue");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _HoverLight = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _HoverLightID = Shader.PropertyToID("_HoverLight");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _EnableHoverColorOverride = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnableHoverColorOverrideID = Shader.PropertyToID("_EnableHoverColorOverride");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _HoverColorOverride = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _HoverColorOverrideID = Shader.PropertyToID("_HoverColorOverride");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _ProximityLight = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ProximityLightID = Shader.PropertyToID("_ProximityLight");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _EnableProximityLightColorOverride = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnableProximityLightColorOverrideID = Shader.PropertyToID("_EnableProximityLightColorOverride");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _ProximityLightCenterColorOverride = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ProximityLightCenterColorOverrideID = Shader.PropertyToID("_ProximityLightCenterColorOverride");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _ProximityLightMiddleColorOverride = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ProximityLightMiddleColorOverrideID = Shader.PropertyToID("_ProximityLightMiddleColorOverride");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _ProximityLightOuterColorOverride = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ProximityLightOuterColorOverrideID = Shader.PropertyToID("_ProximityLightOuterColorOverride");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _ProximityLightSubtractive = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ProximityLightSubtractiveID = Shader.PropertyToID("_ProximityLightSubtractive");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _ProximityLightTwoSided = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ProximityLightTwoSidedID = Shader.PropertyToID("_ProximityLightTwoSided");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _FluentLightIntensity = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _FluentLightIntensityID = Shader.PropertyToID("_FluentLightIntensity");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _RoundCorners = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _RoundCornersID = Shader.PropertyToID("_RoundCorners");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 0.5f)] public float _RoundCornerRadius = 0.25f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _RoundCornerRadiusID = Shader.PropertyToID("_RoundCornerRadius");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 0.5f)] public float _RoundCornerMargin = 0.01f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _RoundCornerMarginID = Shader.PropertyToID("_RoundCornerMargin");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _IndependentCorners = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _IndependentCornersID = Shader.PropertyToID("_IndependentCorners");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Vector4 _RoundCornersRadius = Vector4.zero;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _RoundCornersRadiusID = Shader.PropertyToID("_RoundCornersRadius");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _RoundCornersHideInterior = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _RoundCornersHideInteriorID = Shader.PropertyToID("_RoundCornersHideInterior");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _BorderLight = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BorderLightID = Shader.PropertyToID("_BorderLight");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _BorderLightReplacesAlbedo = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BorderLightReplacesAlbedoID = Shader.PropertyToID("_BorderLightReplacesAlbedo");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _BorderLightOpaque = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BorderLightOpaqueID = Shader.PropertyToID("_BorderLightOpaque");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _BorderWidth = 0.1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BorderWidthID = Shader.PropertyToID("_BorderWidth");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _BorderColorMode = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BorderColorModeID = Shader.PropertyToID("_BorderColorMode");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _BorderMinValue = 0.1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BorderMinValueID = Shader.PropertyToID("_BorderMinValue");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _BorderColor = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BorderColorID = Shader.PropertyToID("_BorderColor");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _EdgeSmoothingMode = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EdgeSmoothingModeID = Shader.PropertyToID("_EdgeSmoothingMode");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _EdgeSmoothingValue = 0.002f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EdgeSmoothingValueID = Shader.PropertyToID("_EdgeSmoothingValue");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _BorderLightOpaqueAlpha = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BorderLightOpaqueAlphaID = Shader.PropertyToID("_BorderLightOpaqueAlpha");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _InnerGlow = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _InnerGlowID = Shader.PropertyToID("_InnerGlow");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _InnerGlowColor = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _InnerGlowColorID = Shader.PropertyToID("_InnerGlowColor");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(2f, 32f)] public float _InnerGlowPower = 4f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _InnerGlowPowerID = Shader.PropertyToID("_InnerGlowPower");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _GradientMode = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _GradientModeID = Shader.PropertyToID("_GradientMode");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Texture2D _IridescentSpectrumMap = null;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _IridescentSpectrumMapID = Shader.PropertyToID("_IridescentSpectrumMap");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _IridescenceIntensity = 0.5f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _IridescenceIntensityID = Shader.PropertyToID("_IridescenceIntensity");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _IridescenceThreshold = 0.05f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _IridescenceThresholdID = Shader.PropertyToID("_IridescenceThreshold");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(-0.78f, 0.78f)] public float _IridescenceAngle = -0.78f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _IridescenceAngleID = Shader.PropertyToID("_IridescenceAngle");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 360f)] public float _GradientAngle = 180f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _GradientAngleID = Shader.PropertyToID("_GradientAngle");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _GradientColor0 = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _GradientColor0ID = Shader.PropertyToID("_GradientColor0");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _GradientColor1 = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _GradientColor1ID = Shader.PropertyToID("_GradientColor1");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _GradientColor2 = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _GradientColor2ID = Shader.PropertyToID("_GradientColor2");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _GradientColor3 = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _GradientColor3ID = Shader.PropertyToID("_GradientColor3");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _GradientColor4 = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _GradientColor4ID = Shader.PropertyToID("_GradientColor4");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Vector4 _GradientAlpha = Vector4.zero;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _GradientAlphaID = Shader.PropertyToID("_GradientAlpha");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Vector4 _GradientAlphaTime = Vector4.zero;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _GradientAlphaTimeID = Shader.PropertyToID("_GradientAlphaTime");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _EnvironmentColoring = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnvironmentColoringID = Shader.PropertyToID("_EnvironmentColoring");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 3f)] public float _EnvironmentColorThreshold = 1.5f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnvironmentColorThresholdID = Shader.PropertyToID("_EnvironmentColorThreshold");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _EnvironmentColorIntensity = 0.5f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnvironmentColorIntensityID = Shader.PropertyToID("_EnvironmentColorIntensity");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _EnvironmentColorX = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnvironmentColorXID = Shader.PropertyToID("_EnvironmentColorX");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _EnvironmentColorY = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnvironmentColorYID = Shader.PropertyToID("_EnvironmentColorY");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _EnvironmentColorZ = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnvironmentColorZID = Shader.PropertyToID("_EnvironmentColorZ");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _BlurMode = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BlurModeID = Shader.PropertyToID("_BlurMode");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _BlurTextureIntensity = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BlurTextureIntensityID = Shader.PropertyToID("_BlurTextureIntensity");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _BlurBorderIntensity = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BlurBorderIntensityID = Shader.PropertyToID("_BlurBorderIntensity");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Vector4 _BlurBackgroundRect = Vector4.zero;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BlurBackgroundRectID = Shader.PropertyToID("_BlurBackgroundRect");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _Mode = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ModeID = Shader.PropertyToID("_Mode");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _CustomMode = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _CustomModeID = Shader.PropertyToID("_CustomMode");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _SrcBlend = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _SrcBlendID = Shader.PropertyToID("_SrcBlend");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _DstBlend = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _DstBlendID = Shader.PropertyToID("_DstBlend");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _SrcBlendAlpha = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _SrcBlendAlphaID = Shader.PropertyToID("_SrcBlendAlpha");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _DstBlendAlpha = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _DstBlendAlphaID = Shader.PropertyToID("_DstBlendAlpha");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _BlendOp = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BlendOpID = Shader.PropertyToID("_BlendOp");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _ZTest = 4f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ZTestID = Shader.PropertyToID("_ZTest");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _ZWrite = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ZWriteID = Shader.PropertyToID("_ZWrite");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _ZOffsetFactor = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ZOffsetFactorID = Shader.PropertyToID("_ZOffsetFactor");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _ZOffsetUnits = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ZOffsetUnitsID = Shader.PropertyToID("_ZOffsetUnits");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _ColorWriteMask = 15f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ColorWriteMaskID = Shader.PropertyToID("_ColorWriteMask");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _CullMode = 2f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _CullModeID = Shader.PropertyToID("_CullMode");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(-1f, 5000f)] public float _RenderQueueOverride = -1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _RenderQueueOverrideID = Shader.PropertyToID("_RenderQueueOverride");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _UseWorldScale = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _UseWorldScaleID = Shader.PropertyToID("_UseWorldScale");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _EnableStencil = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _EnableStencilID = Shader.PropertyToID("_EnableStencil");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 255f)] public float _StencilReference = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _StencilReferenceID = Shader.PropertyToID("_StencilReference");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _StencilComparison = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _StencilComparisonID = Shader.PropertyToID("_StencilComparison");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _StencilOperation = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _StencilOperationID = Shader.PropertyToID("_StencilOperation");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 255f)] public float _StencilWriteMask = 255f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _StencilWriteMaskID = Shader.PropertyToID("_StencilWriteMask");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 255f)] public float _StencilReadMask = 255f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _StencilReadMaskID = Shader.PropertyToID("_StencilReadMask");

        /// <inheritdoc/>
        public override void InitializeFromMaterial(Material material)
        {
            _Color = material.GetColor(_ColorID);
            _MainTex = (Texture2D)material.GetTexture(_MainTexID);
            _AlbedoAlphaMode = material.GetFloat(_AlbedoAlphaModeID);
            _AlbedoAssignedAtRuntime = material.GetFloat(_AlbedoAssignedAtRuntimeID);
            _Cutoff = material.GetFloat(_CutoffID);
            _Fade = material.GetFloat(_FadeID);
            _Metallic = material.GetFloat(_MetallicID);
            _Smoothness = material.GetFloat(_SmoothnessID);
            _EnableChannelMap = material.GetFloat(_EnableChannelMapID);
            _ChannelMap = (Texture2D)material.GetTexture(_ChannelMapID);
            _EnableNormalMap = material.GetFloat(_EnableNormalMapID);
            _NormalMap = (Texture2D)material.GetTexture(_NormalMapID);
            _NormalMapScale = material.GetFloat(_NormalMapScaleID);
            _EnableEmission = material.GetFloat(_EnableEmissionID);
            _EmissiveColor = material.GetColor(_EmissiveColorID);
            _EmissiveMap = (Texture2D)material.GetTexture(_EmissiveMapID);
            _EnableTriplanarMapping = material.GetFloat(_EnableTriplanarMappingID);
            _EnableLocalSpaceTriplanarMapping = material.GetFloat(_EnableLocalSpaceTriplanarMappingID);
            _TriplanarMappingBlendSharpness = material.GetFloat(_TriplanarMappingBlendSharpnessID);
            _EnableSSAA = material.GetFloat(_EnableSSAAID);
            _MipmapBias = material.GetFloat(_MipmapBiasID);
            _DirectionalLight = material.GetFloat(_DirectionalLightID);
            _SpecularHighlights = material.GetFloat(_SpecularHighlightsID);
            _SphericalHarmonics = material.GetFloat(_SphericalHarmonicsID);
            _Reflections = material.GetFloat(_ReflectionsID);
            _RimLight = material.GetFloat(_RimLightID);
            _RimColor = material.GetColor(_RimColorID);
            _RimPower = material.GetFloat(_RimPowerID);
            _VertexColors = material.GetFloat(_VertexColorsID);
            _VertexExtrusion = material.GetFloat(_VertexExtrusionID);
            _VertexExtrusionValue = material.GetFloat(_VertexExtrusionValueID);
            _VertexExtrusionSmoothNormals = material.GetFloat(_VertexExtrusionSmoothNormalsID);
            _BlendedClippingWidth = material.GetFloat(_BlendedClippingWidthID);
            _ClippingBorder = material.GetFloat(_ClippingBorderID);
            _ClippingBorderWidth = material.GetFloat(_ClippingBorderWidthID);
            _ClippingBorderColor = material.GetColor(_ClippingBorderColorID);
            _NearPlaneFade = material.GetFloat(_NearPlaneFadeID);
            _NearLightFade = material.GetFloat(_NearLightFadeID);
            _FadeBeginDistance = material.GetFloat(_FadeBeginDistanceID);
            _FadeCompleteDistance = material.GetFloat(_FadeCompleteDistanceID);
            _FadeMinValue = material.GetFloat(_FadeMinValueID);
            _HoverLight = material.GetFloat(_HoverLightID);
            _EnableHoverColorOverride = material.GetFloat(_EnableHoverColorOverrideID);
            _HoverColorOverride = material.GetColor(_HoverColorOverrideID);
            _ProximityLight = material.GetFloat(_ProximityLightID);
            _EnableProximityLightColorOverride = material.GetFloat(_EnableProximityLightColorOverrideID);
            _ProximityLightCenterColorOverride = material.GetColor(_ProximityLightCenterColorOverrideID);
            _ProximityLightMiddleColorOverride = material.GetColor(_ProximityLightMiddleColorOverrideID);
            _ProximityLightOuterColorOverride = material.GetColor(_ProximityLightOuterColorOverrideID);
            _ProximityLightSubtractive = material.GetFloat(_ProximityLightSubtractiveID);
            _ProximityLightTwoSided = material.GetFloat(_ProximityLightTwoSidedID);
            _FluentLightIntensity = material.GetFloat(_FluentLightIntensityID);
            _RoundCorners = material.GetFloat(_RoundCornersID);
            _RoundCornerRadius = material.GetFloat(_RoundCornerRadiusID);
            _RoundCornerMargin = material.GetFloat(_RoundCornerMarginID);
            _IndependentCorners = material.GetFloat(_IndependentCornersID);
            _RoundCornersRadius = material.GetVector(_RoundCornersRadiusID);
            _RoundCornersHideInterior = material.GetFloat(_RoundCornersHideInteriorID);
            _BorderLight = material.GetFloat(_BorderLightID);
            _BorderLightReplacesAlbedo = material.GetFloat(_BorderLightReplacesAlbedoID);
            _BorderLightOpaque = material.GetFloat(_BorderLightOpaqueID);
            _BorderWidth = material.GetFloat(_BorderWidthID);
            _BorderColorMode = material.GetFloat(_BorderColorModeID);
            _BorderMinValue = material.GetFloat(_BorderMinValueID);
            _BorderColor = material.GetColor(_BorderColorID);
            _EdgeSmoothingMode = material.GetFloat(_EdgeSmoothingModeID);
            _EdgeSmoothingValue = material.GetFloat(_EdgeSmoothingValueID);
            _BorderLightOpaqueAlpha = material.GetFloat(_BorderLightOpaqueAlphaID);
            _InnerGlow = material.GetFloat(_InnerGlowID);
            _InnerGlowColor = material.GetColor(_InnerGlowColorID);
            _InnerGlowPower = material.GetFloat(_InnerGlowPowerID);
            _GradientMode = material.GetFloat(_GradientModeID);
            _IridescentSpectrumMap = (Texture2D)material.GetTexture(_IridescentSpectrumMapID);
            _IridescenceIntensity = material.GetFloat(_IridescenceIntensityID);
            _IridescenceThreshold = material.GetFloat(_IridescenceThresholdID);
            _IridescenceAngle = material.GetFloat(_IridescenceAngleID);
            _GradientAngle = material.GetFloat(_GradientAngleID);
            _GradientColor0 = material.GetColor(_GradientColor0ID);
            _GradientColor1 = material.GetColor(_GradientColor1ID);
            _GradientColor2 = material.GetColor(_GradientColor2ID);
            _GradientColor3 = material.GetColor(_GradientColor3ID);
            _GradientColor4 = material.GetColor(_GradientColor4ID);
            _GradientAlpha = material.GetVector(_GradientAlphaID);
            _GradientAlphaTime = material.GetVector(_GradientAlphaTimeID);
            _EnvironmentColoring = material.GetFloat(_EnvironmentColoringID);
            _EnvironmentColorThreshold = material.GetFloat(_EnvironmentColorThresholdID);
            _EnvironmentColorIntensity = material.GetFloat(_EnvironmentColorIntensityID);
            _EnvironmentColorX = material.GetColor(_EnvironmentColorXID);
            _EnvironmentColorY = material.GetColor(_EnvironmentColorYID);
            _EnvironmentColorZ = material.GetColor(_EnvironmentColorZID);
            _BlurMode = material.GetFloat(_BlurModeID);
            _BlurTextureIntensity = material.GetFloat(_BlurTextureIntensityID);
            _BlurBorderIntensity = material.GetFloat(_BlurBorderIntensityID);
            _BlurBackgroundRect = material.GetVector(_BlurBackgroundRectID);
            _Mode = material.GetFloat(_ModeID);
            _CustomMode = material.GetFloat(_CustomModeID);
            _SrcBlend = material.GetFloat(_SrcBlendID);
            _DstBlend = material.GetFloat(_DstBlendID);
            _SrcBlendAlpha = material.GetFloat(_SrcBlendAlphaID);
            _DstBlendAlpha = material.GetFloat(_DstBlendAlphaID);
            _BlendOp = material.GetFloat(_BlendOpID);
            _ZTest = material.GetFloat(_ZTestID);
            _ZWrite = material.GetFloat(_ZWriteID);
            _ZOffsetFactor = material.GetFloat(_ZOffsetFactorID);
            _ZOffsetUnits = material.GetFloat(_ZOffsetUnitsID);
            _ColorWriteMask = material.GetFloat(_ColorWriteMaskID);
            _CullMode = material.GetFloat(_CullModeID);
            _RenderQueueOverride = material.GetFloat(_RenderQueueOverrideID);
            _UseWorldScale = material.GetFloat(_UseWorldScaleID);
            _EnableStencil = material.GetFloat(_EnableStencilID);
            _StencilReference = material.GetFloat(_StencilReferenceID);
            _StencilComparison = material.GetFloat(_StencilComparisonID);
            _StencilOperation = material.GetFloat(_StencilOperationID);
            _StencilWriteMask = material.GetFloat(_StencilWriteMaskID);
            _StencilReadMask = material.GetFloat(_StencilReadMaskID);
        }

        /// <inheritdoc/>
        public override void ApplyToMaterial(Material material)
        {
            material.SetColor(_ColorID, _Color);
            material.SetTexture(_MainTexID, (Texture2D)_MainTex);
            material.SetFloat(_AlbedoAlphaModeID, _AlbedoAlphaMode);
            material.SetFloat(_AlbedoAssignedAtRuntimeID, _AlbedoAssignedAtRuntime);
            material.SetFloat(_CutoffID, _Cutoff);
            material.SetFloat(_FadeID, _Fade);
            material.SetFloat(_MetallicID, _Metallic);
            material.SetFloat(_SmoothnessID, _Smoothness);
            material.SetFloat(_EnableChannelMapID, _EnableChannelMap);
            material.SetTexture(_ChannelMapID, (Texture2D)_ChannelMap);
            material.SetFloat(_EnableNormalMapID, _EnableNormalMap);
            material.SetTexture(_NormalMapID, (Texture2D)_NormalMap);
            material.SetFloat(_NormalMapScaleID, _NormalMapScale);
            material.SetFloat(_EnableEmissionID, _EnableEmission);
            material.SetColor(_EmissiveColorID, _EmissiveColor);
            material.SetTexture(_EmissiveMapID, (Texture2D)_EmissiveMap);
            material.SetFloat(_EnableTriplanarMappingID, _EnableTriplanarMapping);
            material.SetFloat(_EnableLocalSpaceTriplanarMappingID, _EnableLocalSpaceTriplanarMapping);
            material.SetFloat(_TriplanarMappingBlendSharpnessID, _TriplanarMappingBlendSharpness);
            material.SetFloat(_EnableSSAAID, _EnableSSAA);
            material.SetFloat(_MipmapBiasID, _MipmapBias);
            material.SetFloat(_DirectionalLightID, _DirectionalLight);
            material.SetFloat(_SpecularHighlightsID, _SpecularHighlights);
            material.SetFloat(_SphericalHarmonicsID, _SphericalHarmonics);
            material.SetFloat(_ReflectionsID, _Reflections);
            material.SetFloat(_RimLightID, _RimLight);
            material.SetColor(_RimColorID, _RimColor);
            material.SetFloat(_RimPowerID, _RimPower);
            material.SetFloat(_VertexColorsID, _VertexColors);
            material.SetFloat(_VertexExtrusionID, _VertexExtrusion);
            material.SetFloat(_VertexExtrusionValueID, _VertexExtrusionValue);
            material.SetFloat(_VertexExtrusionSmoothNormalsID, _VertexExtrusionSmoothNormals);
            material.SetFloat(_BlendedClippingWidthID, _BlendedClippingWidth);
            material.SetFloat(_ClippingBorderID, _ClippingBorder);
            material.SetFloat(_ClippingBorderWidthID, _ClippingBorderWidth);
            material.SetColor(_ClippingBorderColorID, _ClippingBorderColor);
            material.SetFloat(_NearPlaneFadeID, _NearPlaneFade);
            material.SetFloat(_NearLightFadeID, _NearLightFade);
            material.SetFloat(_FadeBeginDistanceID, _FadeBeginDistance);
            material.SetFloat(_FadeCompleteDistanceID, _FadeCompleteDistance);
            material.SetFloat(_FadeMinValueID, _FadeMinValue);
            material.SetFloat(_HoverLightID, _HoverLight);
            material.SetFloat(_EnableHoverColorOverrideID, _EnableHoverColorOverride);
            material.SetColor(_HoverColorOverrideID, _HoverColorOverride);
            material.SetFloat(_ProximityLightID, _ProximityLight);
            material.SetFloat(_EnableProximityLightColorOverrideID, _EnableProximityLightColorOverride);
            material.SetColor(_ProximityLightCenterColorOverrideID, _ProximityLightCenterColorOverride);
            material.SetColor(_ProximityLightMiddleColorOverrideID, _ProximityLightMiddleColorOverride);
            material.SetColor(_ProximityLightOuterColorOverrideID, _ProximityLightOuterColorOverride);
            material.SetFloat(_ProximityLightSubtractiveID, _ProximityLightSubtractive);
            material.SetFloat(_ProximityLightTwoSidedID, _ProximityLightTwoSided);
            material.SetFloat(_FluentLightIntensityID, _FluentLightIntensity);
            material.SetFloat(_RoundCornersID, _RoundCorners);
            material.SetFloat(_RoundCornerRadiusID, _RoundCornerRadius);
            material.SetFloat(_RoundCornerMarginID, _RoundCornerMargin);
            material.SetFloat(_IndependentCornersID, _IndependentCorners);
            material.SetVector(_RoundCornersRadiusID, _RoundCornersRadius);
            material.SetFloat(_RoundCornersHideInteriorID, _RoundCornersHideInterior);
            material.SetFloat(_BorderLightID, _BorderLight);
            material.SetFloat(_BorderLightReplacesAlbedoID, _BorderLightReplacesAlbedo);
            material.SetFloat(_BorderLightOpaqueID, _BorderLightOpaque);
            material.SetFloat(_BorderWidthID, _BorderWidth);
            material.SetFloat(_BorderColorModeID, _BorderColorMode);
            material.SetFloat(_BorderMinValueID, _BorderMinValue);
            material.SetColor(_BorderColorID, _BorderColor);
            material.SetFloat(_EdgeSmoothingModeID, _EdgeSmoothingMode);
            material.SetFloat(_EdgeSmoothingValueID, _EdgeSmoothingValue);
            material.SetFloat(_BorderLightOpaqueAlphaID, _BorderLightOpaqueAlpha);
            material.SetFloat(_InnerGlowID, _InnerGlow);
            material.SetColor(_InnerGlowColorID, _InnerGlowColor);
            material.SetFloat(_InnerGlowPowerID, _InnerGlowPower);
            material.SetFloat(_GradientModeID, _GradientMode);
            material.SetTexture(_IridescentSpectrumMapID, (Texture2D)_IridescentSpectrumMap);
            material.SetFloat(_IridescenceIntensityID, _IridescenceIntensity);
            material.SetFloat(_IridescenceThresholdID, _IridescenceThreshold);
            material.SetFloat(_IridescenceAngleID, _IridescenceAngle);
            material.SetFloat(_GradientAngleID, _GradientAngle);
            material.SetColor(_GradientColor0ID, _GradientColor0);
            material.SetColor(_GradientColor1ID, _GradientColor1);
            material.SetColor(_GradientColor2ID, _GradientColor2);
            material.SetColor(_GradientColor3ID, _GradientColor3);
            material.SetColor(_GradientColor4ID, _GradientColor4);
            material.SetVector(_GradientAlphaID, _GradientAlpha);
            material.SetVector(_GradientAlphaTimeID, _GradientAlphaTime);
            material.SetFloat(_EnvironmentColoringID, _EnvironmentColoring);
            material.SetFloat(_EnvironmentColorThresholdID, _EnvironmentColorThreshold);
            material.SetFloat(_EnvironmentColorIntensityID, _EnvironmentColorIntensity);
            material.SetColor(_EnvironmentColorXID, _EnvironmentColorX);
            material.SetColor(_EnvironmentColorYID, _EnvironmentColorY);
            material.SetColor(_EnvironmentColorZID, _EnvironmentColorZ);
            material.SetFloat(_BlurModeID, _BlurMode);
            material.SetFloat(_BlurTextureIntensityID, _BlurTextureIntensity);
            material.SetFloat(_BlurBorderIntensityID, _BlurBorderIntensity);
            material.SetVector(_BlurBackgroundRectID, _BlurBackgroundRect);
            material.SetFloat(_ModeID, _Mode);
            material.SetFloat(_CustomModeID, _CustomMode);
            material.SetFloat(_SrcBlendID, _SrcBlend);
            material.SetFloat(_DstBlendID, _DstBlend);
            material.SetFloat(_SrcBlendAlphaID, _SrcBlendAlpha);
            material.SetFloat(_DstBlendAlphaID, _DstBlendAlpha);
            material.SetFloat(_BlendOpID, _BlendOp);
            material.SetFloat(_ZTestID, _ZTest);
            material.SetFloat(_ZWriteID, _ZWrite);
            material.SetFloat(_ZOffsetFactorID, _ZOffsetFactor);
            material.SetFloat(_ZOffsetUnitsID, _ZOffsetUnits);
            material.SetFloat(_ColorWriteMaskID, _ColorWriteMask);
            material.SetFloat(_CullModeID, _CullMode);
            material.SetFloat(_RenderQueueOverrideID, _RenderQueueOverride);
            material.SetFloat(_UseWorldScaleID, _UseWorldScale);
            material.SetFloat(_EnableStencilID, _EnableStencil);
            material.SetFloat(_StencilReferenceID, _StencilReference);
            material.SetFloat(_StencilComparisonID, _StencilComparison);
            material.SetFloat(_StencilOperationID, _StencilOperation);
            material.SetFloat(_StencilWriteMaskID, _StencilWriteMask);
            material.SetFloat(_StencilReadMaskID, _StencilReadMask);
        }

        /// <inheritdoc/>
        public override string GetTargetShaderName()
        {
            return "Graphics Tools/Standard";
        }
    }
}

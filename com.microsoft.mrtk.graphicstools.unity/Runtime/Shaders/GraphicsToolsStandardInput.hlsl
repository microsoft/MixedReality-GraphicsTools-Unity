// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef GT_STANDARD_INPUT
#define GT_STANDARD_INPUT

/// <summary>
/// Vertex attributes passed into the vertex shader from the app.
/// </summary>
struct Attributes
{
    float4 vertex : POSITION;
    // The default UV channel used for texturing.
    float2 uv : TEXCOORD0;
#if defined(LIGHTMAP_ON)
    // Reserved for Unity's light map UVs.
    float2 uv1 : TEXCOORD1;
#endif
    // Used for smooth normal data (or UGUI scaling data).
    float4 uv2 : TEXCOORD2;
    // Used for UGUI scaling data.
    float2 uv3 : TEXCOORD3;
#if defined(_VERTEX_COLORS)
    half4 color : COLOR0;
#endif
    half3 normal : NORMAL;
#if defined(_NORMAL_MAP)
    half4 tangent : TANGENT;
#endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

/// <summary>
/// Vertex attributes interpolated across a triangle and sent from the vertex shader to the fragment shader.
/// </summary>
struct Varyings
{
    float4 position : SV_POSITION;
#if defined(_UV)
    float2 uv : TEXCOORD0;
#endif
#if defined(_BLUR_TEXTURE) || defined(_BLUR_TEXTURE_2)
    float4 uvScreen : TEXCOORD4;
#elif defined(_BLUR_TEXTURE_PREBAKED_BACKGROUND)
    float2 uvBackgroundRect : TEXCOORD4;
#endif
#if defined(LIGHTMAP_ON)
    float2 lightMapUV : TEXCOORD1;
#endif
    half4 color : COLOR0;
#if defined(_SPHERICAL_HARMONICS)
    half3 ambient : COLOR1;
#endif
#if defined(_IRIDESCENCE)
    half3 gradient : COLOR2;
#elif defined(_GRADIENT_LINEAR)
    float gradient : COLOR2;
#endif
#if defined(_WORLD_POSITION)
#if defined(_NEAR_PLANE_FADE)
    float4 worldPosition : TEXCOORD2;
#else
    float3 worldPosition : TEXCOORD2;
#endif
#endif
#if defined(UNITY_UI_CLIP_RECT)
    float3 localPosition : TEXCOORD7;
#endif
#if defined(_SCALE)
    float3 scale : TEXCOORD3;
#endif
#if defined(_NORMAL)
#if defined(_TRIPLANAR_MAPPING)
    half3 worldNormal : COLOR3;
    half3 triplanarNormal : COLOR4;
    float3 triplanarPosition : TEXCOORD6;
#elif defined(_NORMAL_MAP)
    half3 tangentX : COLOR3;
    half3 tangentY : COLOR4;
    half3 tangentZ : COLOR5;
#else
    half3 worldNormal : COLOR3;
#endif
#endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

/// <summary>
/// Textures and samplers.
/// </summary>

#if defined(_URP)
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
#if defined(_CHANNEL_MAP)
TEXTURE2D(_ChannelMap);
SAMPLER(sampler_ChannelMap);
#endif
#if defined(_NORMAL_MAP)
TEXTURE2D(_NormalMap);
SAMPLER(sampler_NormalMap);
#endif
#if defined(_EMISSION)
TEXTURE2D(_EmissiveMap);
SAMPLER(sampler_EmissiveMap);
#endif
#if defined(_IRIDESCENCE)
TEXTURE2D(_IridescentSpectrumMap);
SAMPLER(sampler_IridescentSpectrumMap);
#endif
#if defined(_BLUR_TEXTURE)
TEXTURE2D_X(_blurTexture);
SAMPLER(sampler_blurTexture);
#elif defined(_BLUR_TEXTURE_2)
TEXTURE2D_X(_blurTexture2);
SAMPLER(sampler_blurTexture2);
#elif defined(_BLUR_TEXTURE_PREBAKED_BACKGROUND)
TEXTURE2D(_blurTexture);
SAMPLER(sampler_blurTexture);
#endif
#else
sampler2D _MainTex;
#if defined(_CHANNEL_MAP)
sampler2D _ChannelMap;
#endif
#if defined(_NORMAL_MAP)
sampler2D _NormalMap;
#endif
#if defined(_EMISSION)
sampler2D _EmissiveMap;
#endif
#if defined(_IRIDESCENCE)
sampler2D _IridescentSpectrumMap;
#endif
#if defined(_BLUR_TEXTURE)
UNITY_DECLARE_SCREENSPACE_TEXTURE(_blurTexture);
#elif defined(_BLUR_TEXTURE_2)
UNITY_DECLARE_SCREENSPACE_TEXTURE(_blurTexture2);
#elif defined(_BLUR_TEXTURE_PREBAKED_BACKGROUND)
sampler2D _blurTexture;
#endif
#endif

/// <summary>
/// Global properties.
/// </summary>

#if defined(_URP)
// Empty.
#else
half4 _LightColor0;
#endif

#if defined(_DISTANT_LIGHT)
#define DISTANT_LIGHT_COUNT 1
#define DISTANT_LIGHT_DATA_SIZE 2
float4 _DistantLightData[DISTANT_LIGHT_COUNT * DISTANT_LIGHT_DATA_SIZE];
#endif

#if defined(_HOVER_LIGHT) || defined(_NEAR_LIGHT_FADE)
#define HOVER_LIGHT_COUNT 4
#define HOVER_LIGHT_DATA_SIZE 2
float4 _HoverLightData[HOVER_LIGHT_COUNT * HOVER_LIGHT_DATA_SIZE];
#endif

#if defined(_PROXIMITY_LIGHT) || defined(_NEAR_LIGHT_FADE)
#define PROXIMITY_LIGHT_COUNT 2
#define PROXIMITY_LIGHT_DATA_SIZE 6
float4 _ProximityLightData[PROXIMITY_LIGHT_COUNT * PROXIMITY_LIGHT_DATA_SIZE];
#endif

#if defined(_CLIPPING_PLANE)
half _ClipPlaneSide;
float4 _ClipPlane;
#endif

#if defined(_CLIPPING_SPHERE)
half _ClipSphereSide;
float4x4 _ClipSphereInverseTransform;
#endif

#if defined(_CLIPPING_BOX)
half _ClipBoxSide;
float4x4 _ClipBoxInverseTransform;
#endif

/// <summary>
/// Per material properties.
/// </summary>

CBUFFER_START(UnityPerMaterial)

#if defined(UNITY_INSTANCING_ENABLED)
    half4 _ColorUnused; // Color is defined in the PerMaterialInstanced constant buffer.
#else
    half4 _Color;
#endif

    half4 _MainTex_ST;
    half _Metallic;
    half _Smoothness;

    half _NormalMapScale;
    // #if defined(_ALPHA_CLIP)
    half _Cutoff;

    // #if defined(_ALPHABLEND_ON) || defined(_ALPHABLEND_TRANS_ON) || defined(_ADDITIVE_ON)
    half _Fade;

    // #if defined(UNITY_UI_CLIP_RECT)
    float4 _ClipRect;

    // #if defined(_UI_CLIP_RECT_ROUNDED)|| defined(_UI_CLIP_RECT_ROUNDED_INDEPENDENT)
    float4 _ClipRectRadii;

    // #if defined(_EMISSION)
    half4 _EmissiveColor;

    // #if defined(_USE_SSAA)
    float _MipmapBias;

    // #if defined(_TRIPLANAR_MAPPING)
    float _TriplanarMappingBlendSharpness;

    // #if defined(_RIM_LIGHT)
    half3 _RimColor;
    half _RimPower;

    // #if defined(_VERTEX_EXTRUSION)
    float _VertexExtrusionValue;

    // #if defined(_CLIPPING_PRIMITIVE)
    float _BlendedClippingWidth;

    // #if defined(_CLIPPING_BORDER)
    half _ClippingBorderWidth;
    half3 _ClippingBorderColor;

    // #if defined(_NEAR_PLANE_FADE)
    float _FadeBeginDistance;
    float _FadeCompleteDistance;
    half _FadeMinValue;

    // #if defined(_HOVER_COLOR_OVERRIDE)
    half3 _HoverColorOverride;

    // #if defined(_PROXIMITY_LIGHT_COLOR_OVERRIDE)
    float4 _ProximityLightCenterColorOverride;
    float4 _ProximityLightMiddleColorOverride;
    float4 _ProximityLightOuterColorOverride;

    // #if defined(_HOVER_LIGHT) || defined(_PROXIMITY_LIGHT) || defined(_BORDER_LIGHT)
    half _FluentLightIntensity;
    
    // #if defined(_ROUND_CORNERS) || defined(_BORDER_LIGHT)
    // #if defined(_INDEPENDENT_CORNERS)
    float4 _RoundCornersRadius;
    float _RoundCornerRadius;
    float _RoundCornerMargin;

    // #if defined(_BORDER_LIGHT)
    half _BorderWidth;
    half _BorderMinValue;
    // #if defined(_BORDER_LIGHT_USES_COLOR)
    half3 _BorderColor;

    // #if defined(_BORDER_LIGHT_OPAQUE)
    half _BorderLightOpaqueAlpha;

    // #if defined(_ROUND_CORNERS) || defined(_BORDER_LIGHT)
    float _EdgeSmoothingValue;

    // #if defined(_INNER_GLOW)
    half4 _InnerGlowColor;
    half _InnerGlowPower;

    // #if defined(_IRIDESCENCE)
    half _IridescenceIntensity;
    half _IridescenceThreshold;
    half _IridescenceAngle;

    // #if defined(_GRADIENT_FOUR_POINT) || defined(_GRADIENT_LINEAR)
    float _GradientAngle;
    half4 _GradientColor0;
    half4 _GradientColor1;
    half4 _GradientColor2;
    half4 _GradientColor3;
    half4 _GradientColor4;
    half4 _GradientAlpha;
    half4 _GradientAlphaTime;

    // #if defined(_ENVIRONMENT_COLORING)
    half _EnvironmentColorThreshold;
    half _EnvironmentColorIntensity;
    half3 _EnvironmentColorX;
    half3 _EnvironmentColorY;
    half3 _EnvironmentColorZ;

    // #if defined(_BLUR_TEXTURE) || defined(_BLUR_TEXTURE_2) || defined(_BLUR_TEXTURE_PREBAKED_BACKGROUND)
    half _BlurTextureIntensity;
    half _BlurBorderIntensity;
    float4 _BlurBackgroundRect;

CBUFFER_END

#if defined(UNITY_INSTANCING_ENABLED)
UNITY_INSTANCING_BUFFER_START(PerMaterialInstanced)

    UNITY_DEFINE_INSTANCED_PROP(half4, _Color)

UNITY_INSTANCING_BUFFER_END(PerMaterialInstanced)
#endif

#endif // GT_STANDARD_INPUT

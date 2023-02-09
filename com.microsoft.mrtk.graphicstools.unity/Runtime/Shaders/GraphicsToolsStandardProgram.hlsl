// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef GT_STANDARD_PROGRAM
#define GT_STANDARD_PROGRAM

#pragma vertex VertexStage
#pragma fragment PixelStage

// Comment in to help with RenderDoc debugging.
//#pragma enable_d3d11_debug_symbols

/// <summary>
/// Features.
/// </summary>

#pragma shader_feature_local _ _ALPHATEST_ON
#pragma shader_feature_local _DISABLE_ALBEDO_MAP
#pragma shader_feature_local_fragment _ _METALLIC_TEXTURE_ALBEDO_CHANNEL_A _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature_local _CHANNEL_MAP
#pragma shader_feature_local _ _DIRECTIONAL_LIGHT _DISTANT_LIGHT
#pragma shader_feature_local _VERTEX_COLORS
#pragma shader_feature_local _VERTEX_EXTRUSION
#pragma shader_feature_local_vertex _VERTEX_EXTRUSION_SMOOTH_NORMALS
#pragma shader_feature_local_vertex _VERTEX_EXTRUSION_CONSTANT_WIDTH
#pragma shader_feature_local _NEAR_PLANE_FADE
#pragma shader_feature_local_vertex _NEAR_LIGHT_FADE
#pragma shader_feature_local _ROUND_CORNERS
#pragma shader_feature_local_fragment _INDEPENDENT_CORNERS
#pragma shader_feature_local_fragment _ROUND_CORNERS_HIDE_INTERIOR
#pragma shader_feature_local_fragment _ _EDGE_SMOOTHING_AUTOMATIC
#pragma shader_feature_local _USE_WORLD_SCALE

#if !defined(_SHADOW_PASS)
#pragma shader_feature_local _ _ALPHABLEND_ON _ALPHABLEND_TRANS_ON _ADDITIVE_ON
#pragma shader_feature_local _NORMAL_MAP
#pragma shader_feature_local _EMISSION
#pragma shader_feature_local _TRIPLANAR_MAPPING
#pragma shader_feature_local _LOCAL_SPACE_TRIPLANAR_MAPPING
#pragma shader_feature_local_fragment _USE_SSAA
#pragma shader_feature_local _NON_PHOTOREALISTIC
#pragma shader_feature_local_fragment _SPECULAR_HIGHLIGHTS
#pragma shader_feature_local _SPHERICAL_HARMONICS
#pragma shader_feature_local _REFLECTIONS
#pragma shader_feature_local _RIM_LIGHT
#pragma shader_feature_local _HOVER_LIGHT
#pragma shader_feature_local_fragment _HOVER_COLOR_OVERRIDE
#pragma shader_feature_local _PROXIMITY_LIGHT
#pragma shader_feature_local_fragment _PROXIMITY_LIGHT_COLOR_OVERRIDE
#pragma shader_feature_local_fragment _PROXIMITY_LIGHT_SUBTRACTIVE
#pragma shader_feature_local _PROXIMITY_LIGHT_TWO_SIDED
#pragma shader_feature_local _BORDER_LIGHT
#pragma shader_feature_local_fragment _ _BORDER_LIGHT_USES_HOVER_COLOR _BORDER_LIGHT_USES_COLOR _BORDER_LIGHT_USES_GRADIENT
#pragma shader_feature_local_fragment _BORDER_LIGHT_REPLACES_ALBEDO
#pragma shader_feature_local_fragment _BORDER_LIGHT_OPAQUE
#pragma shader_feature_local _INNER_GLOW
#pragma shader_feature_local _ _IRIDESCENCE _GRADIENT_FOUR_POINT _GRADIENT_LINEAR
#pragma shader_feature_local _ENVIRONMENT_COLORING
#pragma shader_feature_local _ _BLUR_TEXTURE _BLUR_TEXTURE_2 _BLUR_TEXTURE_PREBAKED_BACKGROUND
#endif

/// <summary>
///  Defines and includes.
/// </summary>

#if defined(_TRIPLANAR_MAPPING) || defined(_DIRECTIONAL_LIGHT) || defined(_DISTANT_LIGHT) || defined(_SPHERICAL_HARMONICS) || defined(_REFLECTIONS) || defined(_RIM_LIGHT) || defined(_PROXIMITY_LIGHT) || defined(_ENVIRONMENT_COLORING) || defined(LIGHTMAP_ON)
#define _NORMAL
#else
#undef _NORMAL
#endif

#if defined(_CLIPPING_PLANE) || defined(_CLIPPING_SPHERE) || defined(_CLIPPING_BOX)
#define _CLIPPING_PRIMITIVE
#else
#undef _CLIPPING_PRIMITIVE
#endif

#if defined(_NORMAL) || defined(_CLIPPING_PRIMITIVE) || defined(_NEAR_PLANE_FADE) || defined(_HOVER_LIGHT) || defined(_PROXIMITY_LIGHT)
#define _WORLD_POSITION
#else
#undef _WORLD_POSITION
#endif

#if defined(_ALPHATEST_ON) || defined(UNITY_UI_ALPHACLIP) || defined(_CLIPPING_PRIMITIVE) || defined(_ROUND_CORNERS)
#define _ALPHA_CLIP
#if !defined(UNITY_UI_ALPHACLIP)
#define UNITY_UI_ALPHACLIP
#endif
#else
#undef _ALPHA_CLIP
#undef UNITY_UI_ALPHACLIP
#endif

#if defined(_ALPHABLEND_ON) || defined(_ALPHABLEND_TRANS_ON) || defined(_ADDITIVE_ON)
#define _TRANSPARENT
#undef _ALPHA_CLIP
#undef UNITY_UI_ALPHACLIP
#else
#undef _TRANSPARENT
#endif

#if defined(_VERTEX_EXTRUSION) || defined(_ROUND_CORNERS) || defined(_BORDER_LIGHT) || defined(_GRADIENT_LINEAR)
#define _SCALE
#else
#undef _SCALE
#endif

#if defined(_ROUND_CORNERS) || defined(_BORDER_LIGHT) || defined(_INNER_GLOW)
#define _DISTANCE_TO_EDGE
#else
#undef _DISTANCE_TO_EDGE
#endif

#if defined(_IRIDESCENCE) || defined(_GRADIENT_FOUR_POINT) || defined(_GRADIENT_LINEAR)
#define _GRADIENT
#else
#undef _GRADIENT
#endif

#if !defined(_DISABLE_ALBEDO_MAP) || defined(_TRIPLANAR_MAPPING) || defined(_CHANNEL_MAP) || defined(_NORMAL_MAP) || defined(_DISTANCE_TO_EDGE) || defined(_GRADIENT) || defined(_EMISSION)
#define _UV
#else
#undef _UV
#endif

#if defined(_BLUR_TEXTURE) || defined(_BLUR_TEXTURE_2)
#define _UV_SCREEN
#else
#undef _UV_SCREEN
#endif

#if defined(_URP)
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#else
#include "UnityCG.cginc"
#include "UnityStandardConfig.cginc"
#include "UnityStandardUtils.cginc"
#endif 

#include "GraphicsToolsCommon.hlsl"
#include "GraphicsToolsStandardInput.hlsl"
#include "GraphicsToolsLighting.hlsl"

/// <summary>
/// Vertex shader entry point.
/// </summary>
Varyings VertexStage(Attributes input)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    float4 vertexPosition = input.vertex;

#if defined(_WORLD_POSITION) || defined(_VERTEX_EXTRUSION)
    float3 worldVertexPosition = mul(UNITY_MATRIX_M, vertexPosition).xyz;
#endif

#if defined(_SCALE)
    output.scale = GTGetWorldScale();
    float canvasScale = 1.0;
#if !defined(_VERTEX_EXTRUSION_SMOOTH_NORMALS)
#if defined(_CANVAS_RENDERED)
    canvasScale = min(min(output.scale.x, output.scale.y), output.scale.z);
    output.scale.x *= input.uv2.x;
    output.scale.y *= input.uv2.y;
    output.scale.z *= input.uv3.x;
#endif
#endif
#endif
    
    half3 localNormal = input.normal;

#if defined(_NORMAL) || defined(_VERTEX_EXTRUSION)
#if defined(_URP)
    half3 worldNormal = TransformObjectToWorldNormal(localNormal);
#else
    half3 worldNormal = UnityObjectToWorldNormal(localNormal);
#endif
#endif

#if (defined(_VERTEX_EXTRUSION) && defined(_VERTEX_EXTRUSION_CONSTANT_WIDTH)) || defined(_NEAR_PLANE_FADE) && !defined(_NEAR_LIGHT_FADE)
    float cameraDistance = GetDistanceToCamera(vertexPosition);
#endif

#if defined(_VERTEX_EXTRUSION)
#if defined(_VERTEX_EXTRUSION_CONSTANT_WIDTH)
    _VertexExtrusionValue *= cameraDistance;
#endif
#if defined(_VERTEX_EXTRUSION_SMOOTH_NORMALS)
#if defined(_URP)
    worldVertexPosition += TransformObjectToWorldNormal(input.uv2.xyz * output.scale) * _VertexExtrusionValue;
#else
    worldVertexPosition += UnityObjectToWorldNormal(input.uv2.xyz * output.scale) * _VertexExtrusionValue;
#endif
#else
    worldVertexPosition += worldNormal * _VertexExtrusionValue;
#endif
#if defined(_URP)
    vertexPosition = mul(UNITY_MATRIX_I_M, float4(worldVertexPosition, 1.0));
#else
    vertexPosition = mul(unity_WorldToObject, float4(worldVertexPosition, 1.0));
#endif
#endif

#if defined(_URP)
    output.position = TransformObjectToHClip(vertexPosition.xyz);
#else
    output.position = UnityObjectToClipPos(vertexPosition);
#endif

#if defined(_WORLD_POSITION)
    output.worldPosition.xyz = worldVertexPosition;
#endif

#if defined(UNITY_UI_CLIP_RECT)
    output.localPosition.xyz = vertexPosition.xyz;
#endif

#if defined(_NEAR_PLANE_FADE)
    float rangeInverse = 1.0 / (_FadeBeginDistance - _FadeCompleteDistance);
#if defined(_NEAR_LIGHT_FADE)
    float fadeDistance = GT_MAX_NEAR_LIGHT_DIST;

    [unroll]
    for (int hoverLightIndex = 0; hoverLightIndex < HOVER_LIGHT_COUNT; ++hoverLightIndex)
    {
        int dataIndex = hoverLightIndex * HOVER_LIGHT_DATA_SIZE;
        fadeDistance = min(fadeDistance, GTNearLightDistance(_HoverLightData[dataIndex], output.worldPosition.xyz));
    }

    [unroll]
    for (int proximityLightIndex = 0; proximityLightIndex < PROXIMITY_LIGHT_COUNT; ++proximityLightIndex)
    {
        int dataIndex = proximityLightIndex * PROXIMITY_LIGHT_DATA_SIZE;
        fadeDistance = min(fadeDistance, GTNearLightDistance(_ProximityLightData[dataIndex], output.worldPosition.xyz));
    }
#else
    float fadeDistance = cameraDistance;
#endif
    output.worldPosition.w = max(saturate(mad(fadeDistance, rangeInverse, -_FadeCompleteDistance * rangeInverse)), _FadeMinValue);
#endif

#if defined(_BORDER_LIGHT) || defined(_ROUND_CORNERS)
    output.uv = input.uv;
    
    if (abs(localNormal.x) == 1.0) // Y,Z plane.
    {
        output.scale.x = output.scale.z;
    }
    else if (abs(localNormal.y) == 1.0) // X,Z plane.
    {
        output.scale.y = output.scale.z;
    } // Else X,Y plane.
        
    #if defined(_USE_WORLD_SCALE)
        output.scale.z = canvasScale;
    #else
        output.scale.z = min(min(output.scale.x, output.scale.y), output.scale.z);
    #endif

#elif defined(_UV)
    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
#endif

#if defined(_UV_SCREEN)
    output.uvScreen = ComputeScreenPos(output.position);
    // Flip vertical UV for orthographic projections (if not already flipped) to ensure the image is not upside down.
#if defined(UNITY_UV_STARTS_AT_TOP)
    output.uvScreen.y = unity_OrthoParams.w ? (1.0 - output.uvScreen.y) : output.uvScreen.y;
#else
    output.uvScreen.y = unity_OrthoParams.w ? output.uvScreen.y : (1.0 - output.uvScreen.y);
#endif
#elif (_BLUR_TEXTURE_PREBAKED_BACKGROUND)
    output.uvBackgroundRect = float2((vertexPosition.x - _BlurBackgroundRect.x) / (_BlurBackgroundRect.z - _BlurBackgroundRect.x), 
                                     (vertexPosition.y - _BlurBackgroundRect.y) / (_BlurBackgroundRect.w - _BlurBackgroundRect.y));
#endif 

#if defined(LIGHTMAP_ON)
    output.lightMapUV.xy = input.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

    output.color = UNITY_ACCESS_INSTANCED_PROP(PerMaterialInstanced, _Color);

#if defined(_VERTEX_COLORS)
    output.color *= input.color;
#endif

#if defined(_SPHERICAL_HARMONICS)
#if defined(_URP)
    float4 coefficients[7];
    coefficients[0] = unity_SHAr;
    coefficients[1] = unity_SHAg;
    coefficients[2] = unity_SHAb;
    coefficients[3] = unity_SHBr;
    coefficients[4] = unity_SHBg;
    coefficients[5] = unity_SHBb;
    coefficients[6] = unity_SHC;
    output.ambient = max(0.0, SampleSH9(coefficients, worldNormal));
#else
    output.ambient = ShadeSH9(float4(worldNormal, 1.0));
#endif
#endif

#if defined(_IRIDESCENCE)
    float3 rightTangent = normalize(mul((float3x3)UNITY_MATRIX_M, float3(1.0, 0.0, 0.0)));
    float3 incidentWithCenter = normalize(mul(UNITY_MATRIX_M, float4(0.0, 0.0, 0.0, 1.0)).xyz - _WorldSpaceCameraPos);
    float tangentDotIncident = dot(rightTangent, incidentWithCenter);
#if defined(_URP)
    output.gradient = GTIridescence(tangentDotIncident, 
                                  TEXTURE2D_ARGS(_IridescentSpectrumMap, sampler_IridescentSpectrumMap), 
                                  _IridescenceThreshold, 
                                  input.uv, 
                                  _IridescenceAngle, 
                                  _IridescenceIntensity);
#else
    output.gradient = GTIridescence(tangentDotIncident, 
                                  _IridescentSpectrumMap, 
                                  _IridescenceThreshold, 
                                  input.uv, 
                                  _IridescenceAngle, 
                                  _IridescenceIntensity);
#endif
 #elif defined(_GRADIENT_LINEAR)
    // Reference: https://patrickbrosset.medium.com/do-you-really-understand-css-linear-gradients-631d9a895caf
    // Translate the angle from degress to radians and default pointing up along the unit circle.
    float angle = _GradientAngle * GT_DEGREES_TO_RADIANS;
    float cosA = cos(angle);
    float sinA = sin(angle);

    // Calculate the direction vector of the gradient line.
    float2 direction = mul(float2(0.0, 1.0), float2x2(cosA, -sinA, sinA, cosA));

    // Calculate the length of the gradient line for this rect.
    float width = output.scale.x;
    float height = output.scale.y;
    float length = abs(width * sinA) + abs(height * cosA);

    // Calculate start point of the gradient (which can lie outside of the rect).
    float2 center = float2(width * 0.5, height * 0.5);
    float2 start = center - (direction * (length * 0.5));

    // Project the vector from the start point to the current texel onto the gradient direction. This will 
    // tell us how far this texel is along the gradient.
    float2 texel = float2(output.uv.x * width, output.uv.y * height);
    float t = dot(texel - start, direction / length);
    output.gradient = t;
#endif

#if defined(_NORMAL)
#if defined(_TRIPLANAR_MAPPING)
    output.worldNormal = worldNormal;
#if defined(_LOCAL_SPACE_TRIPLANAR_MAPPING)
    output.triplanarNormal = localNormal;
    output.triplanarPosition = vertexPosition.xyz;
#else
    output.triplanarNormal = worldNormal;
    output.triplanarPosition = output.worldPosition;
#endif
#elif defined(_NORMAL_MAP)
#if defined(_URP)
    half3 worldTangent = TransformObjectToWorldDir(input.tangent.xyz);
#else
    half3 worldTangent = UnityObjectToWorldDir(input.tangent.xyz);
#endif
    half tangentSign = input.tangent.w * unity_WorldTransformParams.w;
    half3 worldBitangent = cross(worldNormal, worldTangent) * tangentSign;
    output.tangentX = half3(worldTangent.x, worldBitangent.x, worldNormal.x);
    output.tangentY = half3(worldTangent.y, worldBitangent.y, worldNormal.y);
    output.tangentZ = half3(worldTangent.z, worldBitangent.z, worldNormal.z);
#else
    output.worldNormal = worldNormal;
#endif
#endif

    return output;
}

/// <summary>
/// Fragment (pixel) shader entry point.
/// </summary>
half4 PixelStage(Varyings input, bool facing : SV_IsFrontFace) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

#if defined(_TRIPLANAR_MAPPING)
    // Calculate triplanar uvs and apply texture scale and offset values like TRANSFORM_TEX.
    half3 triplanarBlend = pow(abs(input.triplanarNormal), _TriplanarMappingBlendSharpness);
    triplanarBlend /= dot(triplanarBlend, half3(1.0h, 1.0h, 1.0h));
    float2 uvX = input.triplanarPosition.zy * _MainTex_ST.xy + _MainTex_ST.zw;
    float2 uvY = input.triplanarPosition.xz * _MainTex_ST.xy + _MainTex_ST.zw;
    float2 uvZ = input.triplanarPosition.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    
    // Ternary operator is 2 instructions faster than sign() when we don't care about zero returning a zero sign.
    float3 axisSign = input.triplanarNormal < 0 ? -1 : 1;
    uvX.x *= axisSign.x;
    uvY.x *= axisSign.y;
    uvZ.x *= -axisSign.z;
#endif

// Texturing.
#if defined(_DISABLE_ALBEDO_MAP)
    half4 albedo = half4(1.0h, 1.0h, 1.0h, 1.0h);
#else
#if defined(_TRIPLANAR_MAPPING)
#if defined(_URP)
    half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvX) * triplanarBlend.x +
                   SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvY) * triplanarBlend.y +
                   SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvZ) * triplanarBlend.z;
#else
    half4 albedo = tex2D(_MainTex, uvX) * triplanarBlend.x +
                   tex2D(_MainTex, uvY) * triplanarBlend.y +
                   tex2D(_MainTex, uvZ) * triplanarBlend.z;
#endif
#else
#if defined(_USE_SSAA)
    // Does SSAA on the texture, implementation based off this article: https://medium.com/@bgolus/sharper-mipmapping-using-shader-based-supersampling-ed7aadb47bec
    // per pixel screen space partial derivatives
    float2 dx = ddx(input.uv) * 0.25; // horizontal offset
    float2 dy = ddy(input.uv) * 0.25; // vertical offset
    // supersampled 2x2 ordered grid
    half4 albedo = half4(0.0h, 0.0h, 0.0h, 0.0h);
#if defined(_URP)
    albedo += SAMPLE_TEXTURE2D_BIAS(_MainTex, sampler_MainTex, float2(input.uv + dx + dy), _MipmapBias);
    albedo += SAMPLE_TEXTURE2D_BIAS(_MainTex, sampler_MainTex, float2(input.uv - dx + dy), _MipmapBias);
    albedo += SAMPLE_TEXTURE2D_BIAS(_MainTex, sampler_MainTex, float2(input.uv + dx - dy), _MipmapBias);
    albedo += SAMPLE_TEXTURE2D_BIAS(_MainTex, sampler_MainTex, float2(input.uv - dx - dy), _MipmapBias);
#else
    albedo += tex2Dbias(_MainTex, float4(input.uv + dx + dy, 0.0, _MipmapBias));
    albedo += tex2Dbias(_MainTex, float4(input.uv - dx + dy, 0.0, _MipmapBias));
    albedo += tex2Dbias(_MainTex, float4(input.uv + dx - dy, 0.0, _MipmapBias));
    albedo += tex2Dbias(_MainTex, float4(input.uv - dx - dy, 0.0, _MipmapBias));
#endif
    albedo *= 0.25;
#else
#if defined(_URP)
    half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
#else
    half4 albedo = tex2D(_MainTex, input.uv);
#endif
#endif
#endif
#endif

#if defined(_CHANNEL_MAP)
#if defined(_URP)
    half4 channel = SAMPLE_TEXTURE2D(_ChannelMap, sampler_ChannelMap, input.uv);
#else
    half4 channel = tex2D(_ChannelMap, input.uv);
#endif
    _Metallic = channel.r;
    // TODO - [Cameron-Micka] should occlusion be applied to albedo or just lighting?
    albedo.rgb *= channel.g;
    _Smoothness = channel.a;
#else
#if defined(_METALLIC_TEXTURE_ALBEDO_CHANNEL_A)
    _Metallic = albedo.a;
    albedo.a = 1.0h;
#elif defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A)
    _Smoothness = albedo.a;
    albedo.a = 1.0h;
#endif 
#endif

#if defined(_EMISSION)
#if defined(_URP)
    half3 emissionMap = SAMPLE_TEXTURE2D(_EmissiveMap, sampler_EmissiveMap, input.uv).xyz;
#else
    half3 emissionMap = tex2D(_EmissiveMap, input.uv).xyz;
#endif
#endif
    
    // Primitive clipping.
#if defined(_CLIPPING_PRIMITIVE)
    float primitiveDistance = 1.0;
#if defined(_CLIPPING_PLANE)
    primitiveDistance = min(primitiveDistance, GTPointVsPlane(input.worldPosition.xyz, _ClipPlane) * _ClipPlaneSide);
#endif
#if defined(_CLIPPING_SPHERE)
    primitiveDistance = min(primitiveDistance, GTPointVsSphere(input.worldPosition.xyz, _ClipSphereInverseTransform) * _ClipSphereSide);
#endif
#if defined(_CLIPPING_BOX)
    primitiveDistance = min(primitiveDistance, GTPointVsBox(input.worldPosition.xyz, _ClipBoxInverseTransform) * _ClipBoxSide);
#endif
#if defined(_CLIPPING_BORDER)
    half3 primitiveBorderColor = lerp(_ClippingBorderColor, half3(0.0h, 0.0h, 0.0h), primitiveDistance / _ClippingBorderWidth);
    albedo.rgb += primitiveBorderColor * (primitiveDistance < _ClippingBorderWidth ? 1.0h : 0.0h);
#endif
#endif

#if defined(_DISTANCE_TO_EDGE)
    half2 distanceToEdge;
    distanceToEdge.x = abs(input.uv.x - 0.5h) * 2.0h;
    distanceToEdge.y = abs(input.uv.y - 0.5h) * 2.0h;
#endif

#if defined(_BORDER_LIGHT) || defined(_ROUND_CORNERS)
    float2 halfScale = input.scale.xy * 0.5;
    float2 cornerPosition = distanceToEdge * halfScale;

    half currentCornerRadius;

    // Rounded corner clipping.
#if defined(_ROUND_CORNERS)
#if defined(_INDEPENDENT_CORNERS)
#if !defined(_USE_WORLD_SCALE)
    _RoundCornersRadius = clamp(_RoundCornersRadius, 0.0h, 0.5h);
#endif
    currentCornerRadius = GTFindCornerRadius(input.uv.xy, _RoundCornersRadius);
#else
    currentCornerRadius = _RoundCornerRadius;
#endif
#else
    currentCornerRadius = 0.0h;
    _RoundCornerMargin = 0.0h;
#endif
#if defined(_USE_WORLD_SCALE)
    float cornerCircleRadius = max(currentCornerRadius, GT_MIN_CORNER_VALUE) * input.scale.z;
#else
    float cornerCircleRadius = saturate(max(currentCornerRadius - _RoundCornerMargin, GT_MIN_CORNER_VALUE)) * input.scale.z;
#endif
    float2 cornerCircleDistance = halfScale - (_RoundCornerMargin * input.scale.z) - cornerCircleRadius;
#if defined(_ROUND_CORNERS)
    float roundCornerClip = GTRoundCorners(cornerPosition, cornerCircleDistance, cornerCircleRadius, _EdgeSmoothingValue * input.scale.z);
#if defined(_ROUND_CORNERS_HIDE_INTERIOR)
    roundCornerClip = (roundCornerClip < 1.0) ? roundCornerClip : 0.0;
#endif
#endif
#endif

    albedo *= input.color;
#if defined(_ADDITIVE_ON)
    albedo.rgb *= input.color.a;
#endif

#if defined(_GRADIENT)
#if defined(_IRIDESCENCE)
    half4 gradientColor = half4(input.gradient, 1.0h);
#elif defined(_GRADIENT_FOUR_POINT)
    half4 gradientColor = GTFourPointGradient(_GradientColor1, _GradientColor2, _GradientColor3, _GradientColor4, input.uv);
#elif defined(_GRADIENT_LINEAR)
    half4 gradientColor = GTLinearGradient(_GradientColor0, _GradientColor1, _GradientColor2, _GradientColor3, _GradientAlpha, _GradientAlphaTime, input.gradient);
#endif

#if !defined(_BORDER_LIGHT_USES_GRADIENT)
    albedo.rgb += gradientColor.rgb;
    albedo.a *= gradientColor.a;
#endif
#endif

    // Normal calculation.
#if defined(_NORMAL)
#if defined(_URP)
    half3 worldViewDir = normalize(GetWorldSpaceViewDir(input.worldPosition.xyz));
#else
    half3 worldViewDir = normalize(UnityWorldSpaceViewDir(input.worldPosition.xyz));
 #endif
#if defined(_REFLECTIONS) || defined(_ENVIRONMENT_COLORING)
    half3 incident = -worldViewDir;
#endif
    half3 worldNormal;

#if defined(_NORMAL_MAP)
#if defined(_TRIPLANAR_MAPPING)
#if defined(_URP)
    half3 tangentNormalX = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uvX), _NormalMapScale);
    half3 tangentNormalY = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uvY), _NormalMapScale);
    half3 tangentNormalZ = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uvZ), _NormalMapScale);
#else
    half3 tangentNormalX = UnpackScaleNormal(tex2D(_NormalMap, uvX), _NormalMapScale);
    half3 tangentNormalY = UnpackScaleNormal(tex2D(_NormalMap, uvY), _NormalMapScale);
    half3 tangentNormalZ = UnpackScaleNormal(tex2D(_NormalMap, uvZ), _NormalMapScale);
#endif
    tangentNormalX.x *= axisSign.x;
    tangentNormalY.x *= axisSign.y;
    tangentNormalZ.x *= -axisSign.z;

    // Swizzle world normals to match tangent space and apply Whiteout normal blend.
    tangentNormalX = half3(tangentNormalX.xy + input.worldNormal.zy, tangentNormalX.z * input.worldNormal.x);
    tangentNormalY = half3(tangentNormalY.xy + input.worldNormal.xz, tangentNormalY.z * input.worldNormal.y);
    tangentNormalZ = half3(tangentNormalZ.xy + input.worldNormal.xy, tangentNormalZ.z * input.worldNormal.z);

    // Swizzle tangent normals to match world normal and blend together.
    worldNormal = normalize(tangentNormalX.zyx * triplanarBlend.x +
                            tangentNormalY.xzy * triplanarBlend.y +
                            tangentNormalZ.xyz * triplanarBlend.z);
#else
#if defined(_URP)
    half3 tangentNormal = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv), _NormalMapScale);
#else
    half3 tangentNormal = UnpackScaleNormal(tex2D(_NormalMap, input.uv), _NormalMapScale);
#endif
    worldNormal.x = dot(input.tangentX, tangentNormal);
    worldNormal.y = dot(input.tangentY, tangentNormal);
    worldNormal.z = dot(input.tangentZ, tangentNormal);
    worldNormal = normalize(worldNormal) * (facing ? 1.0h : -1.0h);
#endif
#else
    worldNormal = normalize(input.worldNormal) * (facing ? 1.0h : -1.0h);
#endif
#endif

    // Static lighting support.
#ifdef LIGHTMAP_ON
#if defined(_URP)
    albedo.rgb *= SampleLightmap(input.lightMapUV, worldNormal);
#else
    albedo.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, input.lightMapUV));
#endif
#endif

    half pointToLight = 1.0;
    half3 fluentLightColor = half3(0.0h, 0.0h, 0.0h);

    // Hover light.
#if defined(_HOVER_LIGHT)
    pointToLight = 0.0;

    [unroll]
    for (int hoverLightIndex = 0; hoverLightIndex < HOVER_LIGHT_COUNT; ++hoverLightIndex)
    {
        int dataIndex = hoverLightIndex * HOVER_LIGHT_DATA_SIZE;
        half hoverValue = GTHoverLight(_HoverLightData[dataIndex], _HoverLightData[dataIndex + 1].w, input.worldPosition.xyz);
        pointToLight += hoverValue;
#if !defined(_HOVER_COLOR_OVERRIDE)
        fluentLightColor += lerp(half3(0.0h, 0.0h, 0.0h), _HoverLightData[dataIndex + 1].rgb, hoverValue);
#endif
    }
#if defined(_HOVER_COLOR_OVERRIDE)
    fluentLightColor = _HoverColorOverride.rgb * pointToLight;
#endif
#endif

    // Proximity light.
#if defined(_PROXIMITY_LIGHT)
#if !defined(_HOVER_LIGHT)
    pointToLight = 0.0;
#endif
    [unroll]
    for (int proximityLightIndex = 0; proximityLightIndex < PROXIMITY_LIGHT_COUNT; ++proximityLightIndex)
    {
        int dataIndex = proximityLightIndex * PROXIMITY_LIGHT_DATA_SIZE;
        half colorValue;
        half proximityValue = GTProximityLight(_ProximityLightData[dataIndex], _ProximityLightData[dataIndex + 1], _ProximityLightData[dataIndex + 2], input.worldPosition.xyz, worldNormal, colorValue);
        pointToLight += proximityValue;
#if defined(_PROXIMITY_LIGHT_COLOR_OVERRIDE)
        half3 proximityColor = GTMixProximityLightColor(_ProximityLightCenterColorOverride, _ProximityLightMiddleColorOverride, _ProximityLightOuterColorOverride, colorValue);
#else
        half3 proximityColor = GTMixProximityLightColor(_ProximityLightData[dataIndex + 3], _ProximityLightData[dataIndex + 4], _ProximityLightData[dataIndex + 5], colorValue);
#endif  
#if defined(_PROXIMITY_LIGHT_SUBTRACTIVE)
        fluentLightColor -= lerp(half3(0.0h, 0.0h, 0.0h), proximityColor, proximityValue);
#else
        fluentLightColor += lerp(half3(0.0h, 0.0h, 0.0h), proximityColor, proximityValue);
#endif    
    }
#endif

#if defined(_BLUR_TEXTURE) || defined(_BLUR_TEXTURE_2) || defined(_BLUR_TEXTURE_PREBAKED_BACKGROUND)
    half3 blurColor = half3(0.0h, 0.0h, 0.0h);
#if defined(_UV_SCREEN)
    float2 uvScreen = input.uvScreen.xy / input.uvScreen.w;
#if defined(_BLUR_TEXTURE)
#if defined(_URP)
    blurColor = SAMPLE_TEXTURE2D_X(_blurTexture, sampler_blurTexture, uvScreen).rgb;
#else
    blurColor = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_blurTexture, uvScreen).rgb;
#endif
#elif defined(_BLUR_TEXTURE_2)
#if defined(_URP)
    blurColor = SAMPLE_TEXTURE2D_X(_blurTexture2, sampler_blurTexture2, uvScreen).rgb;
#else
    blurColor = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_blurTexture2, uvScreen).rgb;
#endif
#endif
#elif(_BLUR_TEXTURE_PREBAKED_BACKGROUND)
#if defined(_URP)
    blurColor = SAMPLE_TEXTURE2D(_blurTexture, sampler_blurTexture, input.uvBackgroundRect).rgb;
#else
    blurColor = tex2D(_blurTexture, input.uvBackgroundRect).rgb;
#endif
#endif
    albedo.rgb += blurColor * _BlurTextureIntensity;
#endif

    // Border light.
#if defined(_BORDER_LIGHT)
#if defined(_USE_WORLD_SCALE)
    half borderMargin = _RoundCornerMargin + _BorderWidth;
    cornerCircleRadius = max(currentCornerRadius - _BorderWidth, GT_MIN_CORNER_VALUE) * input.scale.z;
#else
    half borderMargin = _RoundCornerMargin + _BorderWidth * 0.5h;
    cornerCircleRadius = saturate(max(currentCornerRadius - borderMargin, GT_MIN_CORNER_VALUE)) * input.scale.z;
#endif
    cornerCircleDistance = halfScale - (borderMargin * input.scale.z) - cornerCircleRadius;

    half borderValue = 1.0 - GTRoundCornersSmooth(cornerPosition, cornerCircleDistance, cornerCircleRadius, _EdgeSmoothingValue * input.scale.z);

#if defined(_BORDER_LIGHT_USES_HOVER_COLOR) && defined(_HOVER_LIGHT) && defined(_HOVER_COLOR_OVERRIDE)
    half3 borderColor = _HoverColorOverride.rgb * _BorderMinValue;
#elif defined(_BORDER_LIGHT_USES_COLOR)
    half3 borderColor = _BorderColor;
#elif defined(_BORDER_LIGHT_USES_GRADIENT) && defined(_GRADIENT)
    half3 borderColor = gradientColor.rgb;
#else
    half3 borderColor = half3(_BorderMinValue, _BorderMinValue, _BorderMinValue);
#endif

    half3 borderContribution = borderColor * _FluentLightIntensity;

#if defined(_BLUR_TEXTURE) || defined(_BLUR_TEXTURE_2) || defined(_BLUR_TEXTURE_PREBAKED_BACKGROUND)
    borderContribution += blurColor * _BlurBorderIntensity;
#endif

#if defined(_BORDER_LIGHT_REPLACES_ALBEDO)
    albedo.rgb = lerp(albedo.rgb, borderContribution, borderValue);
#else
    albedo.rgb += lerp(half3(0.0h, 0.0h, 0.0h), borderContribution, borderValue);
#endif
#if defined(_HOVER_LIGHT) || defined(_PROXIMITY_LIGHT)
    albedo.rgb += (fluentLightColor * borderValue * pointToLight * _FluentLightIntensity) * 2.0h;
#endif
#if defined(_BORDER_LIGHT_OPAQUE)
    albedo.a = max(albedo.a, borderValue * _BorderLightOpaqueAlpha);
#endif
#endif

#if defined(_ROUND_CORNERS)
#if defined(_ALPHABLEND_TRANS_ON)
    albedo *= roundCornerClip;
#else
    albedo.a *= roundCornerClip;
#endif
    pointToLight *= roundCornerClip;
#endif

#ifdef UNITY_UI_CLIP_RECT
    half clipRect = GTUnityUIClipRect(input.localPosition.xy, _ClipRect, _ClipRectRadii);
#if defined(_ALPHABLEND_TRANS_ON)
    albedo *= clipRect;
#else
    albedo.a *= clipRect;
#endif
#endif

#if defined(_ALPHA_CLIP)
#if !defined(_ALPHATEST_ON)
    _Cutoff = 0.5;
#endif
#if defined(_CLIPPING_PRIMITIVE)
    albedo *= (primitiveDistance > 0.0);
#endif
    clip(albedo.a - _Cutoff);
    albedo.a = 1.0;
#endif

#if defined(_SHADOW_PASS)
	// Return early to avoid unnecessary calculations for shadow pass.
    return 0;
#endif
    
    // Final lighting mix.
    half4 output = albedo;

#if defined(_DIRECTIONAL_LIGHT) || defined(_DISTANT_LIGHT) || defined(_REFLECTIONS)
#if defined(_CHANNEL_MAP)
    half occlusion = channel.g;
#else
    half occlusion = 1.0h;
#endif
#endif

#if defined(_DIRECTIONAL_LIGHT) || defined(_DISTANT_LIGHT) || defined(_NPR_Rendering)
    GTBRDFData brdfData;
    GTInitializeBRDFData(albedo.rgb, _Metallic, half3(1.0h, 1.0h, 1.0h), _Smoothness, albedo.a, brdfData);

 #if defined(_SPHERICAL_HARMONICS)
    half3 bakedGI = input.ambient;
#else
    half3 bakedGI = GTDefaultAmbientGI;
#endif

    // Indirect lighting.
    output.rgb = GTGlobalIllumination(brdfData, bakedGI, occlusion, worldNormal, worldViewDir);

    // Direct lighting.
    GTMainLight light = GTGetMainLight();
    // Non Photorealistic
#if defined(_NON_PHOTOREALISTIC)
    output.rgb += GTLightingNonPhotorealistic(brdfData, light.color, light.direction, worldNormal, worldViewDir);
#else
    output.rgb += GTLightingPhysicallyBased(brdfData, light.color, light.direction, worldNormal, worldViewDir);
#endif
    // No lighting, but show reflections.
#elif defined(_REFLECTIONS) 
    half3 reflectVector = reflect(-worldViewDir, worldNormal);
    half3 reflection = GTGlossyEnvironmentReflection(reflectVector, GTPerceptualSmoothnessToPerceptualRoughness(_Smoothness), occlusion);
    output.rgb = (albedo.rgb * 0.5h) + (reflection * (_Smoothness + _Metallic) * 0.5h);
#endif

    // Fresnel lighting.
#if defined(_RIM_LIGHT)
    half fresnel = 1.0h - saturate(abs(dot(worldViewDir, worldNormal)));
    output.rgb += _RimColor * pow(fresnel, _RimPower);
#endif

    // Emmissive light.
#if defined(_EMISSION)
#if defined(UNITY_COLORSPACE_GAMMA)
    half3 emission = _EmissiveColor.rgb;
    emission *= emissionMap;
#else // Since emission is an HDR color convert from sRGB to linear.
    half3 emission = GTsRGBToLinear(_EmissiveColor.rgb);
    emission *= emissionMap;
#endif
#if defined(_CHANNEL_MAP)
    output.rgb += emission * channel.b;
#else
    output.rgb += emission;
#endif
#endif

    // Inner glow.
#if defined(_INNER_GLOW)
    half2 uvGlow = pow(abs(distanceToEdge * _InnerGlowColor.a), _InnerGlowPower);
    output.rgb += lerp(half3(0.0h, 0.0h, 0.0h), _InnerGlowColor.rgb, uvGlow.x + uvGlow.y);
#endif

    // Environment coloring.
#if defined(_ENVIRONMENT_COLORING)
    half3 environmentColor = incident.x * incident.x * _EnvironmentColorX +
                              incident.y * incident.y * _EnvironmentColorY +
                              incident.z * incident.z * _EnvironmentColorZ;
    output.rgb += environmentColor * max(0.0, dot(incident, worldNormal) + _EnvironmentColorThreshold) * _EnvironmentColorIntensity;

#endif

#if defined(_NEAR_PLANE_FADE)
    output *= input.worldPosition.w;
#endif

#if defined(_ADDITIVE_ON)
    output.rgb *= albedo.a;
#endif

    // Hover and proximity lighting should occur after near plane fading.
#if defined(_HOVER_LIGHT) || defined(_PROXIMITY_LIGHT)
    output.rgb += fluentLightColor * _FluentLightIntensity * pointToLight;
#endif

    // Perform non-alpha clipped primitive clipping on the final output.
#if defined(_CLIPPING_PRIMITIVE) && !defined(_ALPHA_CLIP)
    output *= saturate(primitiveDistance * (1.0 / _BlendedClippingWidth));
#endif

    // Fade the alpha channel (or RGB channels) on the final output.
#if defined(_ALPHABLEND_ON)
    output.a *= _Fade;
#elif defined(_ALPHABLEND_TRANS_ON)
    output *= _Fade;
#elif defined(_ADDITIVE_ON)
    output *= _Fade;
#endif

    return output;
}

#endif // GT_STANDARD_PROGRAM

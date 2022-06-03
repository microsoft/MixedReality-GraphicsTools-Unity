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
    public class CanvasMaterialAnimatorCanvasGlow : CanvasMaterialAnimatorBase
    {
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _Relative_To_Height_ = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Relative_To_Height_ID = Shader.PropertyToID("_Relative_To_Height_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _Bevel_Radius_ = 0.05f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Bevel_Radius_ID = Shader.PropertyToID("_Bevel_Radius_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _Line_Width_ = 0.03f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Line_Width_ID = Shader.PropertyToID("_Line_Width_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _Outer_Fuzz_Start_ = 0.002f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Outer_Fuzz_Start_ID = Shader.PropertyToID("_Outer_Fuzz_Start_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _Outer_Fuzz_End_ = 0.001f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Outer_Fuzz_End_ID = Shader.PropertyToID("_Outer_Fuzz_End_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _Fixed_Unit_Multiplier_ = 1000f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Fixed_Unit_Multiplier_ID = Shader.PropertyToID("_Fixed_Unit_Multiplier_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _Motion_ = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Motion_ID = Shader.PropertyToID("_Motion_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _Max_Intensity_ = 0.5f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Max_Intensity_ID = Shader.PropertyToID("_Max_Intensity_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 5f)] public float _Intensity_Fade_In_Exponent_ = 2f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Intensity_Fade_In_Exponent_ID = Shader.PropertyToID("_Intensity_Fade_In_Exponent_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _Color_ = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Color_ID = Shader.PropertyToID("_Color_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _Inner_Color_ = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Inner_Color_ID = Shader.PropertyToID("_Inner_Color_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 9f)] public float _Blend_Exponent_ = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Blend_Exponent_ID = Shader.PropertyToID("_Blend_Exponent_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 5f)] public float _Falloff_ = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Falloff_ID = Shader.PropertyToID("_Falloff_");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 1f)] public float _Bias_ = 0.5f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _Bias_ID = Shader.PropertyToID("_Bias_");
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
           public float _DstBlend = 1f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _DstBlendID = Shader.PropertyToID("_DstBlend");
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
           public float _ZTest = 4f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ZTestID = Shader.PropertyToID("_ZTest");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _ZWrite = 0f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ZWriteID = Shader.PropertyToID("_ZWrite");
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
           public Vector4 _ClipRect = Vector4.zero;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ClipRectID = Shader.PropertyToID("_ClipRect");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Vector4 _ClipRectRadii = Vector4.zero;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ClipRectRadiiID = Shader.PropertyToID("_ClipRectRadii");

        /// <inheritdoc/>
        public override void InitializeFromMaterial(Material material)
        {
            _Relative_To_Height_ = material.GetFloat(_Relative_To_Height_ID);
            _Bevel_Radius_ = material.GetFloat(_Bevel_Radius_ID);
            _Line_Width_ = material.GetFloat(_Line_Width_ID);
            _Outer_Fuzz_Start_ = material.GetFloat(_Outer_Fuzz_Start_ID);
            _Outer_Fuzz_End_ = material.GetFloat(_Outer_Fuzz_End_ID);
            _Fixed_Unit_Multiplier_ = material.GetFloat(_Fixed_Unit_Multiplier_ID);
            _Motion_ = material.GetFloat(_Motion_ID);
            _Max_Intensity_ = material.GetFloat(_Max_Intensity_ID);
            _Intensity_Fade_In_Exponent_ = material.GetFloat(_Intensity_Fade_In_Exponent_ID);
            _Color_ = material.GetColor(_Color_ID);
            _Inner_Color_ = material.GetColor(_Inner_Color_ID);
            _Blend_Exponent_ = material.GetFloat(_Blend_Exponent_ID);
            _Falloff_ = material.GetFloat(_Falloff_ID);
            _Bias_ = material.GetFloat(_Bias_ID);
            _SrcBlend = material.GetFloat(_SrcBlendID);
            _DstBlend = material.GetFloat(_DstBlendID);
            _StencilReference = material.GetFloat(_StencilReferenceID);
            _StencilComparison = material.GetFloat(_StencilComparisonID);
            _StencilOperation = material.GetFloat(_StencilOperationID);
            _ZTest = material.GetFloat(_ZTestID);
            _ZWrite = material.GetFloat(_ZWriteID);
            _MainTex = (Texture2D)material.GetTexture(_MainTexID);
            _ClipRect = material.GetVector(_ClipRectID);
            _ClipRectRadii = material.GetVector(_ClipRectRadiiID);
        }

        /// <inheritdoc/>
        public override void ApplyToMaterial(Material material)
        {
            material.SetFloat(_Relative_To_Height_ID, _Relative_To_Height_);
            material.SetFloat(_Bevel_Radius_ID, _Bevel_Radius_);
            material.SetFloat(_Line_Width_ID, _Line_Width_);
            material.SetFloat(_Outer_Fuzz_Start_ID, _Outer_Fuzz_Start_);
            material.SetFloat(_Outer_Fuzz_End_ID, _Outer_Fuzz_End_);
            material.SetFloat(_Fixed_Unit_Multiplier_ID, _Fixed_Unit_Multiplier_);
            material.SetFloat(_Motion_ID, _Motion_);
            material.SetFloat(_Max_Intensity_ID, _Max_Intensity_);
            material.SetFloat(_Intensity_Fade_In_Exponent_ID, _Intensity_Fade_In_Exponent_);
            material.SetColor(_Color_ID, _Color_);
            material.SetColor(_Inner_Color_ID, _Inner_Color_);
            material.SetFloat(_Blend_Exponent_ID, _Blend_Exponent_);
            material.SetFloat(_Falloff_ID, _Falloff_);
            material.SetFloat(_Bias_ID, _Bias_);
            material.SetFloat(_SrcBlendID, _SrcBlend);
            material.SetFloat(_DstBlendID, _DstBlend);
            material.SetFloat(_StencilReferenceID, _StencilReference);
            material.SetFloat(_StencilComparisonID, _StencilComparison);
            material.SetFloat(_StencilOperationID, _StencilOperation);
            material.SetFloat(_ZTestID, _ZTest);
            material.SetFloat(_ZWriteID, _ZWrite);
            material.SetTexture(_MainTexID, (Texture2D)_MainTex);
            material.SetVector(_ClipRectID, _ClipRect);
            material.SetVector(_ClipRectRadiiID, _ClipRectRadii);
        }

        /// <inheritdoc/>
        public override string GetTargetShaderName()
        {
            return "Graphics Tools/Canvas/Glow";
        }
    }
}

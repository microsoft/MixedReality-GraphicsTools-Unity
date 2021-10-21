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
    public class CanvasGlowCanvasMaterialAnimator : BaseCanvasMaterialAnimator
    {
        [Header("Material Properties")]
        [HideInInspector] public Texture2D _MainTex = null;
        public static int _MainTexID = Shader.PropertyToID("_MainTex");
        [HideInInspector, Range(0f, 1f)] public float _Bevel_Radius_ = 0.05f;
        public static int _Bevel_Radius_ID = Shader.PropertyToID("_Bevel_Radius_");
        [HideInInspector, Range(0f, 1f)] public float _Line_Width_ = 0.03f;
        public static int _Line_Width_ID = Shader.PropertyToID("_Line_Width_");
        [HideInInspector, Range(0f, 1f)] public float _Motion_ = 0f;
        public static int _Motion_ID = Shader.PropertyToID("_Motion_");
        [HideInInspector, Range(0f, 1f)] public float _Max_Intensity_ = 0.5f;
        public static int _Max_Intensity_ID = Shader.PropertyToID("_Max_Intensity_");
        [HideInInspector, Range(0f, 5f)] public float _Intensity_Fade_In_Exponent_ = 2f;
        public static int _Intensity_Fade_In_Exponent_ID = Shader.PropertyToID("_Intensity_Fade_In_Exponent_");
        [HideInInspector, Range(0f, 1f)] public float _Outer_Fuzz_Start_ = 0.002f;
        public static int _Outer_Fuzz_Start_ID = Shader.PropertyToID("_Outer_Fuzz_Start_");
        [HideInInspector, Range(0f, 1f)] public float _Outer_Fuzz_End_ = 0.001f;
        public static int _Outer_Fuzz_End_ID = Shader.PropertyToID("_Outer_Fuzz_End_");
        [HideInInspector] public Color _Color_ = Color.white;
        public static int _Color_ID = Shader.PropertyToID("_Color_");
        [HideInInspector] public Color _Inner_Color_ = Color.white;
        public static int _Inner_Color_ID = Shader.PropertyToID("_Inner_Color_");
        [HideInInspector, Range(0f, 9f)] public float _Blend_Exponent_ = 1f;
        public static int _Blend_Exponent_ID = Shader.PropertyToID("_Blend_Exponent_");
        [HideInInspector, Range(0f, 5f)] public float _Falloff_ = 1f;
        public static int _Falloff_ID = Shader.PropertyToID("_Falloff_");
        [HideInInspector, Range(0f, 1f)] public float _Bias_ = 0.5f;
        public static int _Bias_ID = Shader.PropertyToID("_Bias_");

        /// <inheritdoc/>
        public override void InitializeFromMaterial(Material material)
        {
            _MainTex = (Texture2D)material.GetTexture(_MainTexID);
            _Bevel_Radius_ = material.GetFloat(_Bevel_Radius_ID);
            _Line_Width_ = material.GetFloat(_Line_Width_ID);
            _Motion_ = material.GetFloat(_Motion_ID);
            _Max_Intensity_ = material.GetFloat(_Max_Intensity_ID);
            _Intensity_Fade_In_Exponent_ = material.GetFloat(_Intensity_Fade_In_Exponent_ID);
            _Outer_Fuzz_Start_ = material.GetFloat(_Outer_Fuzz_Start_ID);
            _Outer_Fuzz_End_ = material.GetFloat(_Outer_Fuzz_End_ID);
            _Color_ = material.GetColor(_Color_ID);
            _Inner_Color_ = material.GetColor(_Inner_Color_ID);
            _Blend_Exponent_ = material.GetFloat(_Blend_Exponent_ID);
            _Falloff_ = material.GetFloat(_Falloff_ID);
            _Bias_ = material.GetFloat(_Bias_ID);
        }

        /// <inheritdoc/>
        public override void ApplyToMaterial(Material material)
        {
            material.SetTexture(_MainTexID, (Texture2D)_MainTex);
            material.SetFloat(_Bevel_Radius_ID, _Bevel_Radius_);
            material.SetFloat(_Line_Width_ID, _Line_Width_);
            material.SetFloat(_Motion_ID, _Motion_);
            material.SetFloat(_Max_Intensity_ID, _Max_Intensity_);
            material.SetFloat(_Intensity_Fade_In_Exponent_ID, _Intensity_Fade_In_Exponent_);
            material.SetFloat(_Outer_Fuzz_Start_ID, _Outer_Fuzz_Start_);
            material.SetFloat(_Outer_Fuzz_End_ID, _Outer_Fuzz_End_);
            material.SetColor(_Color_ID, _Color_);
            material.SetColor(_Inner_Color_ID, _Inner_Color_);
            material.SetFloat(_Blend_Exponent_ID, _Blend_Exponent_);
            material.SetFloat(_Falloff_ID, _Falloff_);
            material.SetFloat(_Bias_ID, _Bias_);
        }

        /// <inheritdoc/>
        public override string GetTargetShaderName()
        {
            return "Graphics Tools/Canvas Glow";
        }
    }
}

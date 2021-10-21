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
    public class CanvasBackplateCanvasMaterialAnimator : BaseCanvasMaterialAnimator
    {
        [Header("Material Properties")]
        [HideInInspector, Range(0f, 10f)] public float _Line_Width_ = 1f;
        public static int _Line_Width_ID = Shader.PropertyToID("_Line_Width_");
        [HideInInspector, Range(0f, 4f)] public float _Filter_Width_ = 1f;
        public static int _Filter_Width_ID = Shader.PropertyToID("_Filter_Width_");
        [HideInInspector] public Color _Base_Color_ = Color.white;
        public static int _Base_Color_ID = Shader.PropertyToID("_Base_Color_");
        [HideInInspector] public Color _Line_Color_ = Color.white;
        public static int _Line_Color_ID = Shader.PropertyToID("_Line_Color_");
        [HideInInspector, Range(0f, 1f)] public float _Rate_ = 0f;
        public static int _Rate_ID = Shader.PropertyToID("_Rate_");
        [HideInInspector] public Color _Highlight_Color_ = Color.white;
        public static int _Highlight_Color_ID = Shader.PropertyToID("_Highlight_Color_");
        [HideInInspector, Range(0f, 2f)] public float _Highlight_Width_ = 0.25f;
        public static int _Highlight_Width_ID = Shader.PropertyToID("_Highlight_Width_");
        [HideInInspector] public Vector3 _Highlight_Transform_ = Vector3.zero;
        public static int _Highlight_Transform_ID = Shader.PropertyToID("_Highlight_Transform_");
        [HideInInspector, Range(0f, 1f)] public float _Highlight_ = 1f;
        public static int _Highlight_ID = Shader.PropertyToID("_Highlight_");
        [HideInInspector] public float _Iridescence_Enable_ = 1f;
        public static int _Iridescence_Enable_ID = Shader.PropertyToID("_Iridescence_Enable_");
        [HideInInspector, Range(0f, 1f)] public float _Iridescence_Intensity_ = 0f;
        public static int _Iridescence_Intensity_ID = Shader.PropertyToID("_Iridescence_Intensity_");
        [HideInInspector, Range(0f, 1f)] public float _Iridescence_Edge_Intensity_ = 0.56f;
        public static int _Iridescence_Edge_Intensity_ID = Shader.PropertyToID("_Iridescence_Edge_Intensity_");
        [HideInInspector] public Color _Iridescence_Tint_ = Color.white;
        public static int _Iridescence_Tint_ID = Shader.PropertyToID("_Iridescence_Tint_");
        [HideInInspector] public Texture2D _Iridescent_Map_ = null;
        public static int _Iridescent_Map_ID = Shader.PropertyToID("_Iridescent_Map_");
        [HideInInspector, Range(-90f, 90f)] public float _Angle_ = -45f;
        public static int _Angle_ID = Shader.PropertyToID("_Angle_");
        [HideInInspector] public float _Reflected_ = 1f;
        public static int _Reflected_ID = Shader.PropertyToID("_Reflected_");
        [HideInInspector, Range(0f, 10f)] public float _Frequency_ = 1f;
        public static int _Frequency_ID = Shader.PropertyToID("_Frequency_");
        [HideInInspector, Range(0f, 2f)] public float _Vertical_Offset_ = 0f;
        public static int _Vertical_Offset_ID = Shader.PropertyToID("_Vertical_Offset_");
        [HideInInspector] public Color _Gradient_Color_ = Color.white;
        public static int _Gradient_Color_ID = Shader.PropertyToID("_Gradient_Color_");
        [HideInInspector] public Color _Top_Left_ = Color.white;
        public static int _Top_Left_ID = Shader.PropertyToID("_Top_Left_");
        [HideInInspector] public Color _Top_Right_ = Color.white;
        public static int _Top_Right_ID = Shader.PropertyToID("_Top_Right_");
        [HideInInspector] public Color _Bottom_Left_ = Color.white;
        public static int _Bottom_Left_ID = Shader.PropertyToID("_Bottom_Left_");
        [HideInInspector] public Color _Bottom_Right_ = Color.white;
        public static int _Bottom_Right_ID = Shader.PropertyToID("_Bottom_Right_");
        [HideInInspector] public float _Edge_Only_ = 0f;
        public static int _Edge_Only_ID = Shader.PropertyToID("_Edge_Only_");
        [HideInInspector, Range(0f, 1f)] public float _Edge_Width_ = 0.5f;
        public static int _Edge_Width_ID = Shader.PropertyToID("_Edge_Width_");
        [HideInInspector, Range(0f, 10f)] public float _Edge_Power_ = 2f;
        public static int _Edge_Power_ID = Shader.PropertyToID("_Edge_Power_");
        [HideInInspector, Range(0f, 1f)] public float _Line_Gradient_Blend_ = 0.36f;
        public static int _Line_Gradient_Blend_ID = Shader.PropertyToID("_Line_Gradient_Blend_");
        [HideInInspector, Range(0f, 1f)] public float _Fade_Out_ = 1f;
        public static int _Fade_Out_ID = Shader.PropertyToID("_Fade_Out_");

        /// <inheritdoc/>
        public override void InitializeFromMaterial(Material material)
        {
            _Line_Width_ = material.GetFloat(_Line_Width_ID);
            _Filter_Width_ = material.GetFloat(_Filter_Width_ID);
            _Base_Color_ = material.GetColor(_Base_Color_ID);
            _Line_Color_ = material.GetColor(_Line_Color_ID);
            _Rate_ = material.GetFloat(_Rate_ID);
            _Highlight_Color_ = material.GetColor(_Highlight_Color_ID);
            _Highlight_Width_ = material.GetFloat(_Highlight_Width_ID);
            _Highlight_Transform_ = material.GetVector(_Highlight_Transform_ID);
            _Highlight_ = material.GetFloat(_Highlight_ID);
            _Iridescence_Enable_ = material.GetFloat(_Iridescence_Enable_ID);
            _Iridescence_Intensity_ = material.GetFloat(_Iridescence_Intensity_ID);
            _Iridescence_Edge_Intensity_ = material.GetFloat(_Iridescence_Edge_Intensity_ID);
            _Iridescence_Tint_ = material.GetColor(_Iridescence_Tint_ID);
            _Iridescent_Map_ = (Texture2D)material.GetTexture(_Iridescent_Map_ID);
            _Angle_ = material.GetFloat(_Angle_ID);
            _Reflected_ = material.GetFloat(_Reflected_ID);
            _Frequency_ = material.GetFloat(_Frequency_ID);
            _Vertical_Offset_ = material.GetFloat(_Vertical_Offset_ID);
            _Gradient_Color_ = material.GetColor(_Gradient_Color_ID);
            _Top_Left_ = material.GetColor(_Top_Left_ID);
            _Top_Right_ = material.GetColor(_Top_Right_ID);
            _Bottom_Left_ = material.GetColor(_Bottom_Left_ID);
            _Bottom_Right_ = material.GetColor(_Bottom_Right_ID);
            _Edge_Only_ = material.GetFloat(_Edge_Only_ID);
            _Edge_Width_ = material.GetFloat(_Edge_Width_ID);
            _Edge_Power_ = material.GetFloat(_Edge_Power_ID);
            _Line_Gradient_Blend_ = material.GetFloat(_Line_Gradient_Blend_ID);
            _Fade_Out_ = material.GetFloat(_Fade_Out_ID);
        }

        /// <inheritdoc/>
        public override void ApplyToMaterial(Material material)
        {
            material.SetFloat(_Line_Width_ID, _Line_Width_);
            material.SetFloat(_Filter_Width_ID, _Filter_Width_);
            material.SetColor(_Base_Color_ID, _Base_Color_);
            material.SetColor(_Line_Color_ID, _Line_Color_);
            material.SetFloat(_Rate_ID, _Rate_);
            material.SetColor(_Highlight_Color_ID, _Highlight_Color_);
            material.SetFloat(_Highlight_Width_ID, _Highlight_Width_);
            material.SetVector(_Highlight_Transform_ID, _Highlight_Transform_);
            material.SetFloat(_Highlight_ID, _Highlight_);
            material.SetFloat(_Iridescence_Enable_ID, _Iridescence_Enable_);
            material.SetFloat(_Iridescence_Intensity_ID, _Iridescence_Intensity_);
            material.SetFloat(_Iridescence_Edge_Intensity_ID, _Iridescence_Edge_Intensity_);
            material.SetColor(_Iridescence_Tint_ID, _Iridescence_Tint_);
            material.SetTexture(_Iridescent_Map_ID, (Texture2D)_Iridescent_Map_);
            material.SetFloat(_Angle_ID, _Angle_);
            material.SetFloat(_Reflected_ID, _Reflected_);
            material.SetFloat(_Frequency_ID, _Frequency_);
            material.SetFloat(_Vertical_Offset_ID, _Vertical_Offset_);
            material.SetColor(_Gradient_Color_ID, _Gradient_Color_);
            material.SetColor(_Top_Left_ID, _Top_Left_);
            material.SetColor(_Top_Right_ID, _Top_Right_);
            material.SetColor(_Bottom_Left_ID, _Bottom_Left_);
            material.SetColor(_Bottom_Right_ID, _Bottom_Right_);
            material.SetFloat(_Edge_Only_ID, _Edge_Only_);
            material.SetFloat(_Edge_Width_ID, _Edge_Width_);
            material.SetFloat(_Edge_Power_ID, _Edge_Power_);
            material.SetFloat(_Line_Gradient_Blend_ID, _Line_Gradient_Blend_);
            material.SetFloat(_Fade_Out_ID, _Fade_Out_);
        }

        /// <inheritdoc/>
        public override string GetTargetShaderName()
        {
            return "Graphics Tools/Canvas Backplate";
        }
    }
}

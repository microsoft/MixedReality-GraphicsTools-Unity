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
    public class CanvasMaterialAnimatorCanvasBackplate : CanvasMaterialAnimatorBase
    {

        [Header("Round Rect")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Base_Color_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Base_Color_ID = Shader.PropertyToID("_Base_Color_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Line_Disabled_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Line_Disabled_ID = Shader.PropertyToID("_Line_Disabled_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 10f)] public float _Line_Width_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Line_Width_ID = Shader.PropertyToID("_Line_Width_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Line_Color_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Line_Color_ID = Shader.PropertyToID("_Line_Color_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 4f)] public float _Filter_Width_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Filter_Width_ID = Shader.PropertyToID("_Filter_Width_");

        [Header("Line Highlight")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Rate_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Rate_ID = Shader.PropertyToID("_Rate_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Highlight_Color_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Highlight_Color_ID = Shader.PropertyToID("_Highlight_Color_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 2f)] public float _Highlight_Width_ = 0.25f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Highlight_Width_ID = Shader.PropertyToID("_Highlight_Width_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Vector4 _Highlight_Transform_ = Vector4.zero;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Highlight_Transform_ID = Shader.PropertyToID("_Highlight_Transform_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Highlight_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Highlight_ID = Shader.PropertyToID("_Highlight_");

        [Header("Iridescence")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Iridescence_Enable_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Iridescence_Enable_ID = Shader.PropertyToID("_Iridescence_Enable_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Iridescence_Intensity_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Iridescence_Intensity_ID = Shader.PropertyToID("_Iridescence_Intensity_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Iridescence_Edge_Intensity_ = 0.56f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Iridescence_Edge_Intensity_ID = Shader.PropertyToID("_Iridescence_Edge_Intensity_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Iridescence_Tint_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Iridescence_Tint_ID = Shader.PropertyToID("_Iridescence_Tint_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Texture2D _Iridescent_Map_ = null;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Iridescent_Map_ID = Shader.PropertyToID("_Iridescent_Map_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 10f)] public float _Frequency_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Frequency_ID = Shader.PropertyToID("_Frequency_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 2f)] public float _Vertical_Offset_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Vertical_Offset_ID = Shader.PropertyToID("_Vertical_Offset_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Orthographic_Distance_ = 400f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Orthographic_Distance_ID = Shader.PropertyToID("_Orthographic_Distance_");

        [Header("Gradient")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Gradient_Disabled_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Gradient_Disabled_ID = Shader.PropertyToID("_Gradient_Disabled_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Gradient_Color_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Gradient_Color_ID = Shader.PropertyToID("_Gradient_Color_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Top_Left_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Top_Left_ID = Shader.PropertyToID("_Top_Left_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Top_Right_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Top_Right_ID = Shader.PropertyToID("_Top_Right_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Bottom_Left_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Bottom_Left_ID = Shader.PropertyToID("_Bottom_Left_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Bottom_Right_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Bottom_Right_ID = Shader.PropertyToID("_Bottom_Right_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Edge_Only_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Edge_Only_ID = Shader.PropertyToID("_Edge_Only_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Line_Gradient_Blend_ = 0.36f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Line_Gradient_Blend_ID = Shader.PropertyToID("_Line_Gradient_Blend_");

        [Header("Fade")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Fade_Out_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Fade_Out_ID = Shader.PropertyToID("_Fade_Out_");

        [Header("Antialiasing")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Smooth_Edges_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Smooth_Edges_ID = Shader.PropertyToID("_Smooth_Edges_");

        [Header("Occlusion")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Occluded_Intensity_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Occluded_Intensity_ID = Shader.PropertyToID("_Occluded_Intensity_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Texture2D _OccludedTex = null;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _OccludedTexID = Shader.PropertyToID("_OccludedTex");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _OccludedColor = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _OccludedColorID = Shader.PropertyToID("_OccludedColor");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _GridScale = 0.02f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _GridScaleID = Shader.PropertyToID("_GridScale");

        [Header("Blending")]
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

        [Header("Stencil")]
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

        [Header("Depth")]
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
            _Base_Color_ = material.GetColor(_Base_Color_ID);
            _Line_Disabled_ = material.GetFloat(_Line_Disabled_ID);
            _Line_Width_ = material.GetFloat(_Line_Width_ID);
            _Line_Color_ = material.GetColor(_Line_Color_ID);
            _Filter_Width_ = material.GetFloat(_Filter_Width_ID);
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
            _Frequency_ = material.GetFloat(_Frequency_ID);
            _Vertical_Offset_ = material.GetFloat(_Vertical_Offset_ID);
            _Orthographic_Distance_ = material.GetFloat(_Orthographic_Distance_ID);
            _Gradient_Disabled_ = material.GetFloat(_Gradient_Disabled_ID);
            _Gradient_Color_ = material.GetColor(_Gradient_Color_ID);
            _Top_Left_ = material.GetColor(_Top_Left_ID);
            _Top_Right_ = material.GetColor(_Top_Right_ID);
            _Bottom_Left_ = material.GetColor(_Bottom_Left_ID);
            _Bottom_Right_ = material.GetColor(_Bottom_Right_ID);
            _Edge_Only_ = material.GetFloat(_Edge_Only_ID);
            _Line_Gradient_Blend_ = material.GetFloat(_Line_Gradient_Blend_ID);
            _Fade_Out_ = material.GetFloat(_Fade_Out_ID);
            _Smooth_Edges_ = material.GetFloat(_Smooth_Edges_ID);
            _Occluded_Intensity_ = material.GetFloat(_Occluded_Intensity_ID);
            _OccludedTex = (Texture2D)material.GetTexture(_OccludedTexID);
            _OccludedColor = material.GetColor(_OccludedColorID);
            _GridScale = material.GetFloat(_GridScaleID);
            _SrcBlend = material.GetFloat(_SrcBlendID);
            _DstBlend = material.GetFloat(_DstBlendID);
            _SrcBlendAlpha = material.GetFloat(_SrcBlendAlphaID);
            _DstBlendAlpha = material.GetFloat(_DstBlendAlphaID);
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
            material.SetColor(_Base_Color_ID, _Base_Color_);
            material.SetFloat(_Line_Disabled_ID, _Line_Disabled_);
            material.SetFloat(_Line_Width_ID, _Line_Width_);
            material.SetColor(_Line_Color_ID, _Line_Color_);
            material.SetFloat(_Filter_Width_ID, _Filter_Width_);
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
            material.SetFloat(_Frequency_ID, _Frequency_);
            material.SetFloat(_Vertical_Offset_ID, _Vertical_Offset_);
            material.SetFloat(_Orthographic_Distance_ID, _Orthographic_Distance_);
            material.SetFloat(_Gradient_Disabled_ID, _Gradient_Disabled_);
            material.SetColor(_Gradient_Color_ID, _Gradient_Color_);
            material.SetColor(_Top_Left_ID, _Top_Left_);
            material.SetColor(_Top_Right_ID, _Top_Right_);
            material.SetColor(_Bottom_Left_ID, _Bottom_Left_);
            material.SetColor(_Bottom_Right_ID, _Bottom_Right_);
            material.SetFloat(_Edge_Only_ID, _Edge_Only_);
            material.SetFloat(_Line_Gradient_Blend_ID, _Line_Gradient_Blend_);
            material.SetFloat(_Fade_Out_ID, _Fade_Out_);
            material.SetFloat(_Smooth_Edges_ID, _Smooth_Edges_);
            material.SetFloat(_Occluded_Intensity_ID, _Occluded_Intensity_);
            material.SetTexture(_OccludedTexID, (Texture2D)_OccludedTex);
            material.SetColor(_OccludedColorID, _OccludedColor);
            material.SetFloat(_GridScaleID, _GridScale);
            material.SetFloat(_SrcBlendID, _SrcBlend);
            material.SetFloat(_DstBlendID, _DstBlend);
            material.SetFloat(_SrcBlendAlphaID, _SrcBlendAlpha);
            material.SetFloat(_DstBlendAlphaID, _DstBlendAlpha);
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
            return "Graphics Tools/Canvas/Backplate";
        }
    }
}

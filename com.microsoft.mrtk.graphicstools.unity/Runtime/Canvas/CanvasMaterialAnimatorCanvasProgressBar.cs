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
    public class CanvasMaterialAnimatorCanvasProgressBar : CanvasMaterialAnimatorBase
    {

        [Header("Bar")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Back_Color_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Back_Color_ID = Shader.PropertyToID("_Back_Color_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Fill_1_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Fill_1_ID = Shader.PropertyToID("_Fill_1_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Fill_2_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Fill_2_ID = Shader.PropertyToID("_Fill_2_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Filled_Fraction_ = 0.2f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Filled_Fraction_ID = Shader.PropertyToID("_Filled_Fraction_");

        [Header("Animation")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Cycle_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Cycle_ID = Shader.PropertyToID("_Cycle_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 2f)] public float _Cycle_Rate_ = 0.7f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Cycle_Rate_ID = Shader.PropertyToID("_Cycle_Rate_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Rate_Vary_ = 0.5f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Rate_Vary_ID = Shader.PropertyToID("_Rate_Vary_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Fill_Vary_ = 0.7f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Fill_Vary_ID = Shader.PropertyToID("_Fill_Vary_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Period_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Period_ID = Shader.PropertyToID("_Period_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Cycle_Offset_ = -0.3f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Cycle_Offset_ID = Shader.PropertyToID("_Cycle_Offset_");

        [Header("Blurred Background")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Blur_Circle_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blur_Circle_ID = Shader.PropertyToID("_Blur_Circle_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Texture2D _blurTexture = null;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _blurTextureID = Shader.PropertyToID("_blurTexture");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Vector4 _BlurBackgroundRect = Vector4.zero;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _BlurBackgroundRectID = Shader.PropertyToID("_BlurBackgroundRect");

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
        public float _DstBlend = 10f;
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
            _Back_Color_ = material.GetColor(_Back_Color_ID);
            _Fill_1_ = material.GetColor(_Fill_1_ID);
            _Fill_2_ = material.GetColor(_Fill_2_ID);
            _Filled_Fraction_ = material.GetFloat(_Filled_Fraction_ID);
            _Cycle_ = material.GetFloat(_Cycle_ID);
            _Cycle_Rate_ = material.GetFloat(_Cycle_Rate_ID);
            _Rate_Vary_ = material.GetFloat(_Rate_Vary_ID);
            _Fill_Vary_ = material.GetFloat(_Fill_Vary_ID);
            _Period_ = material.GetFloat(_Period_ID);
            _Cycle_Offset_ = material.GetFloat(_Cycle_Offset_ID);
            _Blur_Circle_ = material.GetFloat(_Blur_Circle_ID);
            _blurTexture = (Texture2D)material.GetTexture(_blurTextureID);
            _BlurBackgroundRect = material.GetVector(_BlurBackgroundRectID);
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
            material.SetColor(_Back_Color_ID, _Back_Color_);
            material.SetColor(_Fill_1_ID, _Fill_1_);
            material.SetColor(_Fill_2_ID, _Fill_2_);
            material.SetFloat(_Filled_Fraction_ID, _Filled_Fraction_);
            material.SetFloat(_Cycle_ID, _Cycle_);
            material.SetFloat(_Cycle_Rate_ID, _Cycle_Rate_);
            material.SetFloat(_Rate_Vary_ID, _Rate_Vary_);
            material.SetFloat(_Fill_Vary_ID, _Fill_Vary_);
            material.SetFloat(_Period_ID, _Period_);
            material.SetFloat(_Cycle_Offset_ID, _Cycle_Offset_);
            material.SetFloat(_Blur_Circle_ID, _Blur_Circle_);
            material.SetTexture(_blurTextureID, (Texture2D)_blurTexture);
            material.SetVector(_BlurBackgroundRectID, _BlurBackgroundRect);
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
            return "Graphics Tools/Canvas/Progress Bar";
        }
    }
}

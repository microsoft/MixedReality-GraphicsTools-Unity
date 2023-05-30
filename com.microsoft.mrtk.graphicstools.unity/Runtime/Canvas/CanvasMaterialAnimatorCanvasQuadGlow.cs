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
    public class CanvasMaterialAnimatorCanvasQuadGlow : CanvasMaterialAnimatorBase
    {

        [Header("Color")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Color_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Color_ID = Shader.PropertyToID("_Color_");

        [Header("Shape")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Radius_ = 0.5f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Radius_ID = Shader.PropertyToID("_Radius_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Fixed_Radius_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Fixed_Radius_ID = Shader.PropertyToID("_Fixed_Radius_");
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
        [Range(0f, 4f)] public float _Filter_Width_ = 2f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Filter_Width_ID = Shader.PropertyToID("_Filter_Width_");

        [Header("Glow")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0.01f, 0.99f)] public float _Glow_Fraction_ = 0.5f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Glow_Fraction_ID = Shader.PropertyToID("_Glow_Fraction_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Glow_Max_ = 0.5f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Glow_Max_ID = Shader.PropertyToID("_Glow_Max_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 5f)] public float _Glow_Falloff_ = 2f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Glow_Falloff_ID = Shader.PropertyToID("_Glow_Falloff_");

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
            _Color_ = material.GetColor(_Color_ID);
            _Radius_ = material.GetFloat(_Radius_ID);
            _Fixed_Radius_ = material.GetFloat(_Fixed_Radius_ID);
            _Fixed_Unit_Multiplier_ = material.GetFloat(_Fixed_Unit_Multiplier_ID);
            _Filter_Width_ = material.GetFloat(_Filter_Width_ID);
            _Glow_Fraction_ = material.GetFloat(_Glow_Fraction_ID);
            _Glow_Max_ = material.GetFloat(_Glow_Max_ID);
            _Glow_Falloff_ = material.GetFloat(_Glow_Falloff_ID);
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
            material.SetColor(_Color_ID, _Color_);
            material.SetFloat(_Radius_ID, _Radius_);
            material.SetFloat(_Fixed_Radius_ID, _Fixed_Radius_);
            material.SetFloat(_Fixed_Unit_Multiplier_ID, _Fixed_Unit_Multiplier_);
            material.SetFloat(_Filter_Width_ID, _Filter_Width_);
            material.SetFloat(_Glow_Fraction_ID, _Glow_Fraction_);
            material.SetFloat(_Glow_Max_ID, _Glow_Max_);
            material.SetFloat(_Glow_Falloff_ID, _Glow_Falloff_);
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
            return "Graphics Tools/Canvas/Quad Glow";
        }
    }
}

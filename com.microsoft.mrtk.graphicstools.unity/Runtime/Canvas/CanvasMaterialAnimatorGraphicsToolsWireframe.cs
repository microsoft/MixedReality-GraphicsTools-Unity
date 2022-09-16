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
    public class CanvasMaterialAnimatorGraphicsToolsWireframe : CanvasMaterialAnimatorBase
    {
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _BaseColor = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _BaseColorID = Shader.PropertyToID("_BaseColor");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public Color _WireColor = Color.white;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _WireColorID = Shader.PropertyToID("_WireColor");
           /// <summary>
           /// Shader property.
           /// </summary>;
           [Range(0f, 800f)] public float _WireThickness = 100f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _WireThicknessID = Shader.PropertyToID("_WireThickness");
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
           public float _ZOffsetFactor = 50f;
           /// <summary>
           /// Shader property ID.
           /// </summary>
           public static int _ZOffsetFactorID = Shader.PropertyToID("_ZOffsetFactor");
           /// <summary>
           /// Shader property.
           /// </summary>;
           public float _ZOffsetUnits = 100f;
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

        /// <inheritdoc/>
        public override void InitializeFromMaterial(Material material)
        {
            _BaseColor = material.GetColor(_BaseColorID);
            _WireColor = material.GetColor(_WireColorID);
            _WireThickness = material.GetFloat(_WireThicknessID);
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
        }

        /// <inheritdoc/>
        public override void ApplyToMaterial(Material material)
        {
            material.SetColor(_BaseColorID, _BaseColor);
            material.SetColor(_WireColorID, _WireColor);
            material.SetFloat(_WireThicknessID, _WireThickness);
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
        }

        /// <inheritdoc/>
        public override string GetTargetShaderName()
        {
            return "Graphics Tools/Wireframe";
        }
    }
}

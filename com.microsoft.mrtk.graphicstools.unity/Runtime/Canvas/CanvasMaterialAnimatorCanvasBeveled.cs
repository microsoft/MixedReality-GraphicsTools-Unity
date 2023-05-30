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
    public class CanvasMaterialAnimatorCanvasBeveled : CanvasMaterialAnimatorBase
    {

        [Header("Sun")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 2f)] public float _Sun_Intensity_ = 0.75f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Sun_Intensity_ID = Shader.PropertyToID("_Sun_Intensity_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Sun_Theta_ = 0.73f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Sun_Theta_ID = Shader.PropertyToID("_Sun_Theta_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Sun_Phi_ = 0.48f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Sun_Phi_ID = Shader.PropertyToID("_Sun_Phi_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Indirect_Diffuse_ = 0.51f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Indirect_Diffuse_ID = Shader.PropertyToID("_Indirect_Diffuse_");

        [Header("Diffuse And Specular")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Albedo_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Albedo_ID = Shader.PropertyToID("_Albedo_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 5f)] public float _Specular_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Specular_ID = Shader.PropertyToID("_Specular_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 10f)] public float _Shininess_ = 10f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Shininess_ID = Shader.PropertyToID("_Shininess_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Sharpness_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Sharpness_ID = Shader.PropertyToID("_Sharpness_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Subsurface_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Subsurface_ID = Shader.PropertyToID("_Subsurface_");

        [Header("Reflection")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 2f)] public float _Reflection_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Reflection_ID = Shader.PropertyToID("_Reflection_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Fresnel_Disabled_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Fresnel_Disabled_ID = Shader.PropertyToID("_Fresnel_Disabled_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Front_Reflect_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Front_Reflect_ID = Shader.PropertyToID("_Front_Reflect_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Edge_Reflect_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Edge_Reflect_ID = Shader.PropertyToID("_Edge_Reflect_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 10f)] public float _Power_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Power_ID = Shader.PropertyToID("_Power_");

        [Header("Sky Environment")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Sky_Enabled_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Sky_Enabled_ID = Shader.PropertyToID("_Sky_Enabled_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Sky_Color_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Sky_Color_ID = Shader.PropertyToID("_Sky_Color_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Horizon_Color_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Horizon_Color_ID = Shader.PropertyToID("_Horizon_Color_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Ground_Color_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Ground_Color_ID = Shader.PropertyToID("_Ground_Color_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 10f)] public float _Horizon_Power_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Horizon_Power_ID = Shader.PropertyToID("_Horizon_Power_");

        [Header("Mapped Environment")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Mapped_Environment_Enabled_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Mapped_Environment_Enabled_ID = Shader.PropertyToID("_Mapped_Environment_Enabled_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Cubemap _Reflection_Map_ = null;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Reflection_Map_ID = Shader.PropertyToID("_Reflection_Map_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Cubemap _Indirect_Environment_ = null;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Indirect_Environment_ID = Shader.PropertyToID("_Indirect_Environment_");

        [Header("Decal Texture")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Decal_Enable_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Decal_Enable_ID = Shader.PropertyToID("_Decal_Enable_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Texture2D _Decal_ = null;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Decal_ID = Shader.PropertyToID("_Decal_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Vector4 _Decal_Scale_XY_ = Vector4.zero;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Decal_Scale_XY_ID = Shader.PropertyToID("_Decal_Scale_XY_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Decal_Front_Only_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Decal_Front_Only_ID = Shader.PropertyToID("_Decal_Front_Only_");

        [Header("Iridescence")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Iridescence_Enabled_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Iridescence_Enabled_ID = Shader.PropertyToID("_Iridescence_Enabled_");
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
        public Texture2D _Iridescence_Texture_ = null;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Iridescence_Texture_ID = Shader.PropertyToID("_Iridescence_Texture_");

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
            _Sun_Intensity_ = material.GetFloat(_Sun_Intensity_ID);
            _Sun_Theta_ = material.GetFloat(_Sun_Theta_ID);
            _Sun_Phi_ = material.GetFloat(_Sun_Phi_ID);
            _Indirect_Diffuse_ = material.GetFloat(_Indirect_Diffuse_ID);
            _Albedo_ = material.GetColor(_Albedo_ID);
            _Specular_ = material.GetFloat(_Specular_ID);
            _Shininess_ = material.GetFloat(_Shininess_ID);
            _Sharpness_ = material.GetFloat(_Sharpness_ID);
            _Subsurface_ = material.GetFloat(_Subsurface_ID);
            _Reflection_ = material.GetFloat(_Reflection_ID);
            _Fresnel_Disabled_ = material.GetFloat(_Fresnel_Disabled_ID);
            _Front_Reflect_ = material.GetFloat(_Front_Reflect_ID);
            _Edge_Reflect_ = material.GetFloat(_Edge_Reflect_ID);
            _Power_ = material.GetFloat(_Power_ID);
            _Sky_Enabled_ = material.GetFloat(_Sky_Enabled_ID);
            _Sky_Color_ = material.GetColor(_Sky_Color_ID);
            _Horizon_Color_ = material.GetColor(_Horizon_Color_ID);
            _Ground_Color_ = material.GetColor(_Ground_Color_ID);
            _Horizon_Power_ = material.GetFloat(_Horizon_Power_ID);
            _Mapped_Environment_Enabled_ = material.GetFloat(_Mapped_Environment_Enabled_ID);
            _Reflection_Map_ = (Cubemap)material.GetTexture(_Reflection_Map_ID);
            _Indirect_Environment_ = (Cubemap)material.GetTexture(_Indirect_Environment_ID);
            _Decal_Enable_ = material.GetFloat(_Decal_Enable_ID);
            _Decal_ = (Texture2D)material.GetTexture(_Decal_ID);
            _Decal_Scale_XY_ = material.GetVector(_Decal_Scale_XY_ID);
            _Decal_Front_Only_ = material.GetFloat(_Decal_Front_Only_ID);
            _Iridescence_Enabled_ = material.GetFloat(_Iridescence_Enabled_ID);
            _Iridescence_Intensity_ = material.GetFloat(_Iridescence_Intensity_ID);
            _Iridescence_Texture_ = (Texture2D)material.GetTexture(_Iridescence_Texture_ID);
            _ZTest = material.GetFloat(_ZTestID);
            _ZWrite = material.GetFloat(_ZWriteID);
            _StencilReference = material.GetFloat(_StencilReferenceID);
            _StencilComparison = material.GetFloat(_StencilComparisonID);
            _StencilOperation = material.GetFloat(_StencilOperationID);
            _MainTex = (Texture2D)material.GetTexture(_MainTexID);
            _ClipRect = material.GetVector(_ClipRectID);
            _ClipRectRadii = material.GetVector(_ClipRectRadiiID);
        }

        /// <inheritdoc/>
        public override void ApplyToMaterial(Material material)
        {
            material.SetFloat(_Sun_Intensity_ID, _Sun_Intensity_);
            material.SetFloat(_Sun_Theta_ID, _Sun_Theta_);
            material.SetFloat(_Sun_Phi_ID, _Sun_Phi_);
            material.SetFloat(_Indirect_Diffuse_ID, _Indirect_Diffuse_);
            material.SetColor(_Albedo_ID, _Albedo_);
            material.SetFloat(_Specular_ID, _Specular_);
            material.SetFloat(_Shininess_ID, _Shininess_);
            material.SetFloat(_Sharpness_ID, _Sharpness_);
            material.SetFloat(_Subsurface_ID, _Subsurface_);
            material.SetFloat(_Reflection_ID, _Reflection_);
            material.SetFloat(_Fresnel_Disabled_ID, _Fresnel_Disabled_);
            material.SetFloat(_Front_Reflect_ID, _Front_Reflect_);
            material.SetFloat(_Edge_Reflect_ID, _Edge_Reflect_);
            material.SetFloat(_Power_ID, _Power_);
            material.SetFloat(_Sky_Enabled_ID, _Sky_Enabled_);
            material.SetColor(_Sky_Color_ID, _Sky_Color_);
            material.SetColor(_Horizon_Color_ID, _Horizon_Color_);
            material.SetColor(_Ground_Color_ID, _Ground_Color_);
            material.SetFloat(_Horizon_Power_ID, _Horizon_Power_);
            material.SetFloat(_Mapped_Environment_Enabled_ID, _Mapped_Environment_Enabled_);
            material.SetTexture(_Reflection_Map_ID, (Cubemap)_Reflection_Map_);
            material.SetTexture(_Indirect_Environment_ID, (Cubemap)_Indirect_Environment_);
            material.SetFloat(_Decal_Enable_ID, _Decal_Enable_);
            material.SetTexture(_Decal_ID, (Texture2D)_Decal_);
            material.SetVector(_Decal_Scale_XY_ID, _Decal_Scale_XY_);
            material.SetFloat(_Decal_Front_Only_ID, _Decal_Front_Only_);
            material.SetFloat(_Iridescence_Enabled_ID, _Iridescence_Enabled_);
            material.SetFloat(_Iridescence_Intensity_ID, _Iridescence_Intensity_);
            material.SetTexture(_Iridescence_Texture_ID, (Texture2D)_Iridescence_Texture_);
            material.SetFloat(_ZTestID, _ZTest);
            material.SetFloat(_ZWriteID, _ZWrite);
            material.SetFloat(_StencilReferenceID, _StencilReference);
            material.SetFloat(_StencilComparisonID, _StencilComparison);
            material.SetFloat(_StencilOperationID, _StencilOperation);
            material.SetTexture(_MainTexID, (Texture2D)_MainTex);
            material.SetVector(_ClipRectID, _ClipRect);
            material.SetVector(_ClipRectRadiiID, _ClipRectRadii);
        }

        /// <inheritdoc/>
        public override string GetTargetShaderName()
        {
            return "Graphics Tools/Canvas/Beveled";
        }
    }
}

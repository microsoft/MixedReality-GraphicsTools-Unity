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
    public class CanvasMaterialAnimatorCanvasFrontplate : CanvasMaterialAnimatorBase
    {

        [Header("Round Rect")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Radius_ = 0.3125f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Radius_ID = Shader.PropertyToID("_Radius_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Line_Width_ = 0.031f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Line_Width_ID = Shader.PropertyToID("_Line_Width_");
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
        public float _Fixed_Unit_Multiplier_ = 1000f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Fixed_Unit_Multiplier_ID = Shader.PropertyToID("_Fixed_Unit_Multiplier_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 4f)] public float _Filter_Width_ = 1.5f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Filter_Width_ID = Shader.PropertyToID("_Filter_Width_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Color _Edge_Color_ = Color.white;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Edge_Color_ID = Shader.PropertyToID("_Edge_Color_");

        [Header("Fade")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Fade_Out_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Fade_Out_ID = Shader.PropertyToID("_Fade_Out_");

        [Header("Blob")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Blob_Enable_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Enable_ID = Shader.PropertyToID("_Blob_Enable_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Vector4 _Blob_Position_ = Vector4.zero;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Position_ID = Shader.PropertyToID("_Blob_Position_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 3f)] public float _Blob_Intensity_ = 0.34f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Intensity_ID = Shader.PropertyToID("_Blob_Intensity_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Blob_Near_Size_ = 0.025f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Near_Size_ID = Shader.PropertyToID("_Blob_Near_Size_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Blob_Far_Size_ = 0.05f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Far_Size_ID = Shader.PropertyToID("_Blob_Far_Size_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Blob_Near_Distance_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Near_Distance_ID = Shader.PropertyToID("_Blob_Near_Distance_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Blob_Far_Distance_ = 0.08f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Far_Distance_ID = Shader.PropertyToID("_Blob_Far_Distance_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Blob_Fade_Length_ = 0.08f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Fade_Length_ID = Shader.PropertyToID("_Blob_Fade_Length_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0.001f, 1f)] public float _Blob_Inner_Fade_ = 0.01f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Inner_Fade_ID = Shader.PropertyToID("_Blob_Inner_Fade_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Blob_Pulse_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Pulse_ID = Shader.PropertyToID("_Blob_Pulse_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Blob_Fade_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Fade_ID = Shader.PropertyToID("_Blob_Fade_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Blob_Pulse_Max_Size_ = 0.05f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Pulse_Max_Size_ID = Shader.PropertyToID("_Blob_Pulse_Max_Size_");

        [Header("Blob 2")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Blob_Enable_2_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Enable_2_ID = Shader.PropertyToID("_Blob_Enable_2_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Vector4 _Blob_Position_2_ = Vector4.zero;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Position_2_ID = Shader.PropertyToID("_Blob_Position_2_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Blob_Near_Size_2_ = 0.025f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Near_Size_2_ID = Shader.PropertyToID("_Blob_Near_Size_2_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Blob_Inner_Fade_2_ = 0.1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Inner_Fade_2_ID = Shader.PropertyToID("_Blob_Inner_Fade_2_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Blob_Pulse_2_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Pulse_2_ID = Shader.PropertyToID("_Blob_Pulse_2_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Blob_Fade_2_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Fade_2_ID = Shader.PropertyToID("_Blob_Fade_2_");

        [Header("Gaze")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Gaze_Intensity_ = 0.7f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Gaze_Intensity_ID = Shader.PropertyToID("_Gaze_Intensity_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Gaze_Focus_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Gaze_Focus_ID = Shader.PropertyToID("_Gaze_Focus_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Pinched_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Pinched_ID = Shader.PropertyToID("_Pinched_");

        [Header("Blob Texture")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public Texture2D _Blob_Texture_ = null;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Blob_Texture_ID = Shader.PropertyToID("_Blob_Texture_");

        [Header("Selection")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Selection_Fuzz_ = 0.5f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Selection_Fuzz_ID = Shader.PropertyToID("_Selection_Fuzz_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Selected_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Selected_ID = Shader.PropertyToID("_Selected_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Selection_Fade_ = 0f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Selection_Fade_ID = Shader.PropertyToID("_Selection_Fade_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Selection_Fade_Size_ = 0.3f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Selection_Fade_Size_ID = Shader.PropertyToID("_Selection_Fade_Size_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Selected_Distance_ = 0.08f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Selected_Distance_ID = Shader.PropertyToID("_Selected_Distance_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Selected_Fade_Length_ = 0.08f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Selected_Fade_Length_ID = Shader.PropertyToID("_Selected_Fade_Length_");

        [Header("Proximity")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Proximity_Max_Intensity_ = 0.45f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Proximity_Max_Intensity_ID = Shader.PropertyToID("_Proximity_Max_Intensity_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 2f)] public float _Proximity_Far_Distance_ = 0.16f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Proximity_Far_Distance_ID = Shader.PropertyToID("_Proximity_Far_Distance_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 2f)] public float _Proximity_Near_Radius_ = 0.03f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Proximity_Near_Radius_ID = Shader.PropertyToID("_Proximity_Near_Radius_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        [Range(0f, 1f)] public float _Proximity_Anisotropy_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Proximity_Anisotropy_ID = Shader.PropertyToID("_Proximity_Anisotropy_");

        [Header("Global")]
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Use_Global_Left_Index_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Use_Global_Left_Index_ID = Shader.PropertyToID("_Use_Global_Left_Index_");
        /// <summary>
        /// Shader property.
        /// </summary>;
        public float _Use_Global_Right_Index_ = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _Use_Global_Right_Index_ID = Shader.PropertyToID("_Use_Global_Right_Index_");

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
        public float _DstBlend = 1f;
        /// <summary>
        /// Shader property ID.
        /// </summary>
        public static int _DstBlendID = Shader.PropertyToID("_DstBlend");

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
            _Radius_ = material.GetFloat(_Radius_ID);
            _Line_Width_ = material.GetFloat(_Line_Width_ID);
            _Relative_To_Height_ = material.GetFloat(_Relative_To_Height_ID);
            _Fixed_Unit_Multiplier_ = material.GetFloat(_Fixed_Unit_Multiplier_ID);
            _Filter_Width_ = material.GetFloat(_Filter_Width_ID);
            _Edge_Color_ = material.GetColor(_Edge_Color_ID);
            _Fade_Out_ = material.GetFloat(_Fade_Out_ID);
            _Blob_Enable_ = material.GetFloat(_Blob_Enable_ID);
            _Blob_Position_ = material.GetVector(_Blob_Position_ID);
            _Blob_Intensity_ = material.GetFloat(_Blob_Intensity_ID);
            _Blob_Near_Size_ = material.GetFloat(_Blob_Near_Size_ID);
            _Blob_Far_Size_ = material.GetFloat(_Blob_Far_Size_ID);
            _Blob_Near_Distance_ = material.GetFloat(_Blob_Near_Distance_ID);
            _Blob_Far_Distance_ = material.GetFloat(_Blob_Far_Distance_ID);
            _Blob_Fade_Length_ = material.GetFloat(_Blob_Fade_Length_ID);
            _Blob_Inner_Fade_ = material.GetFloat(_Blob_Inner_Fade_ID);
            _Blob_Pulse_ = material.GetFloat(_Blob_Pulse_ID);
            _Blob_Fade_ = material.GetFloat(_Blob_Fade_ID);
            _Blob_Pulse_Max_Size_ = material.GetFloat(_Blob_Pulse_Max_Size_ID);
            _Blob_Enable_2_ = material.GetFloat(_Blob_Enable_2_ID);
            _Blob_Position_2_ = material.GetVector(_Blob_Position_2_ID);
            _Blob_Near_Size_2_ = material.GetFloat(_Blob_Near_Size_2_ID);
            _Blob_Inner_Fade_2_ = material.GetFloat(_Blob_Inner_Fade_2_ID);
            _Blob_Pulse_2_ = material.GetFloat(_Blob_Pulse_2_ID);
            _Blob_Fade_2_ = material.GetFloat(_Blob_Fade_2_ID);
            _Gaze_Intensity_ = material.GetFloat(_Gaze_Intensity_ID);
            _Gaze_Focus_ = material.GetFloat(_Gaze_Focus_ID);
            _Pinched_ = material.GetFloat(_Pinched_ID);
            _Blob_Texture_ = (Texture2D)material.GetTexture(_Blob_Texture_ID);
            _Selection_Fuzz_ = material.GetFloat(_Selection_Fuzz_ID);
            _Selected_ = material.GetFloat(_Selected_ID);
            _Selection_Fade_ = material.GetFloat(_Selection_Fade_ID);
            _Selection_Fade_Size_ = material.GetFloat(_Selection_Fade_Size_ID);
            _Selected_Distance_ = material.GetFloat(_Selected_Distance_ID);
            _Selected_Fade_Length_ = material.GetFloat(_Selected_Fade_Length_ID);
            _Proximity_Max_Intensity_ = material.GetFloat(_Proximity_Max_Intensity_ID);
            _Proximity_Far_Distance_ = material.GetFloat(_Proximity_Far_Distance_ID);
            _Proximity_Near_Radius_ = material.GetFloat(_Proximity_Near_Radius_ID);
            _Proximity_Anisotropy_ = material.GetFloat(_Proximity_Anisotropy_ID);
            _Use_Global_Left_Index_ = material.GetFloat(_Use_Global_Left_Index_ID);
            _Use_Global_Right_Index_ = material.GetFloat(_Use_Global_Right_Index_ID);
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
            material.SetFloat(_Radius_ID, _Radius_);
            material.SetFloat(_Line_Width_ID, _Line_Width_);
            material.SetFloat(_Relative_To_Height_ID, _Relative_To_Height_);
            material.SetFloat(_Fixed_Unit_Multiplier_ID, _Fixed_Unit_Multiplier_);
            material.SetFloat(_Filter_Width_ID, _Filter_Width_);
            material.SetColor(_Edge_Color_ID, _Edge_Color_);
            material.SetFloat(_Fade_Out_ID, _Fade_Out_);
            material.SetFloat(_Blob_Enable_ID, _Blob_Enable_);
            material.SetVector(_Blob_Position_ID, _Blob_Position_);
            material.SetFloat(_Blob_Intensity_ID, _Blob_Intensity_);
            material.SetFloat(_Blob_Near_Size_ID, _Blob_Near_Size_);
            material.SetFloat(_Blob_Far_Size_ID, _Blob_Far_Size_);
            material.SetFloat(_Blob_Near_Distance_ID, _Blob_Near_Distance_);
            material.SetFloat(_Blob_Far_Distance_ID, _Blob_Far_Distance_);
            material.SetFloat(_Blob_Fade_Length_ID, _Blob_Fade_Length_);
            material.SetFloat(_Blob_Inner_Fade_ID, _Blob_Inner_Fade_);
            material.SetFloat(_Blob_Pulse_ID, _Blob_Pulse_);
            material.SetFloat(_Blob_Fade_ID, _Blob_Fade_);
            material.SetFloat(_Blob_Pulse_Max_Size_ID, _Blob_Pulse_Max_Size_);
            material.SetFloat(_Blob_Enable_2_ID, _Blob_Enable_2_);
            material.SetVector(_Blob_Position_2_ID, _Blob_Position_2_);
            material.SetFloat(_Blob_Near_Size_2_ID, _Blob_Near_Size_2_);
            material.SetFloat(_Blob_Inner_Fade_2_ID, _Blob_Inner_Fade_2_);
            material.SetFloat(_Blob_Pulse_2_ID, _Blob_Pulse_2_);
            material.SetFloat(_Blob_Fade_2_ID, _Blob_Fade_2_);
            material.SetFloat(_Gaze_Intensity_ID, _Gaze_Intensity_);
            material.SetFloat(_Gaze_Focus_ID, _Gaze_Focus_);
            material.SetFloat(_Pinched_ID, _Pinched_);
            material.SetTexture(_Blob_Texture_ID, (Texture2D)_Blob_Texture_);
            material.SetFloat(_Selection_Fuzz_ID, _Selection_Fuzz_);
            material.SetFloat(_Selected_ID, _Selected_);
            material.SetFloat(_Selection_Fade_ID, _Selection_Fade_);
            material.SetFloat(_Selection_Fade_Size_ID, _Selection_Fade_Size_);
            material.SetFloat(_Selected_Distance_ID, _Selected_Distance_);
            material.SetFloat(_Selected_Fade_Length_ID, _Selected_Fade_Length_);
            material.SetFloat(_Proximity_Max_Intensity_ID, _Proximity_Max_Intensity_);
            material.SetFloat(_Proximity_Far_Distance_ID, _Proximity_Far_Distance_);
            material.SetFloat(_Proximity_Near_Radius_ID, _Proximity_Near_Radius_);
            material.SetFloat(_Proximity_Anisotropy_ID, _Proximity_Anisotropy_);
            material.SetFloat(_Use_Global_Left_Index_ID, _Use_Global_Left_Index_);
            material.SetFloat(_Use_Global_Right_Index_ID, _Use_Global_Right_Index_);
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
            return "Graphics Tools/Canvas/Frontplate";
        }
    }
}

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
    public class GraphicsToolsCanvasFrontplateCanvasMaterialAnimator : BaseCanvasMaterialAnimator
    {
        [Header("Material Properties")]
        [HideInInspector, Range(0f, 0.5f)] public float _Radius_ = 0.3125f;
        public static int _Radius_ID = Shader.PropertyToID("_Radius_");
        [HideInInspector, Range(0f, 1f)] public float _Line_Width_ = 0.031f;
        public static int _Line_Width_ID = Shader.PropertyToID("_Line_Width_");
        [HideInInspector, Range(0f, 4f)] public float _Filter_Width_ = 1.5f;
        public static int _Filter_Width_ID = Shader.PropertyToID("_Filter_Width_");
        [HideInInspector] public Color _Edge_Color_ = Color.white;
        public static int _Edge_Color_ID = Shader.PropertyToID("_Edge_Color_");
        [HideInInspector, Range(0f, 1f)] public float _Fade_Out_ = 1f;
        public static int _Fade_Out_ID = Shader.PropertyToID("_Fade_Out_");
        [HideInInspector] public float _Blob_Enable_ = 1f;
        public static int _Blob_Enable_ID = Shader.PropertyToID("_Blob_Enable_");
        [HideInInspector] public Vector3 _Blob_Position_ = Vector3.zero;
        public static int _Blob_Position_ID = Shader.PropertyToID("_Blob_Position_");
        [HideInInspector, Range(0f, 3f)] public float _Blob_Intensity_ = 0.34f;
        public static int _Blob_Intensity_ID = Shader.PropertyToID("_Blob_Intensity_");
        [HideInInspector, Range(0f, 1f)] public float _Blob_Near_Size_ = 0.025f;
        public static int _Blob_Near_Size_ID = Shader.PropertyToID("_Blob_Near_Size_");
        [HideInInspector, Range(0f, 1f)] public float _Blob_Far_Size_ = 0.05f;
        public static int _Blob_Far_Size_ID = Shader.PropertyToID("_Blob_Far_Size_");
        [HideInInspector, Range(0f, 1f)] public float _Blob_Near_Distance_ = 0f;
        public static int _Blob_Near_Distance_ID = Shader.PropertyToID("_Blob_Near_Distance_");
        [HideInInspector, Range(0f, 1f)] public float _Blob_Far_Distance_ = 0.08f;
        public static int _Blob_Far_Distance_ID = Shader.PropertyToID("_Blob_Far_Distance_");
        [HideInInspector, Range(0f, 1f)] public float _Blob_Fade_Length_ = 0.08f;
        public static int _Blob_Fade_Length_ID = Shader.PropertyToID("_Blob_Fade_Length_");
        [HideInInspector, Range(0.001f, 1f)] public float _Blob_Inner_Fade_ = 0.01f;
        public static int _Blob_Inner_Fade_ID = Shader.PropertyToID("_Blob_Inner_Fade_");
        [HideInInspector, Range(0f, 1f)] public float _Blob_Pulse_ = 0f;
        public static int _Blob_Pulse_ID = Shader.PropertyToID("_Blob_Pulse_");
        [HideInInspector, Range(0f, 1f)] public float _Blob_Fade_ = 1f;
        public static int _Blob_Fade_ID = Shader.PropertyToID("_Blob_Fade_");
        [HideInInspector, Range(0f, 1f)] public float _Blob_Pulse_Max_Size_ = 0.05f;
        public static int _Blob_Pulse_Max_Size_ID = Shader.PropertyToID("_Blob_Pulse_Max_Size_");
        [HideInInspector] public float _Blob_Enable_2_ = 0f;
        public static int _Blob_Enable_2_ID = Shader.PropertyToID("_Blob_Enable_2_");
        [HideInInspector] public Vector3 _Blob_Position_2_ = Vector3.zero;
        public static int _Blob_Position_2_ID = Shader.PropertyToID("_Blob_Position_2_");
        [HideInInspector, Range(0f, 1f)] public float _Blob_Near_Size_2_ = 0.025f;
        public static int _Blob_Near_Size_2_ID = Shader.PropertyToID("_Blob_Near_Size_2_");
        [HideInInspector, Range(0f, 1f)] public float _Blob_Inner_Fade_2_ = 0.1f;
        public static int _Blob_Inner_Fade_2_ID = Shader.PropertyToID("_Blob_Inner_Fade_2_");
        [HideInInspector, Range(0f, 1f)] public float _Blob_Pulse_2_ = 0f;
        public static int _Blob_Pulse_2_ID = Shader.PropertyToID("_Blob_Pulse_2_");
        [HideInInspector, Range(0f, 1f)] public float _Blob_Fade_2_ = 1f;
        public static int _Blob_Fade_2_ID = Shader.PropertyToID("_Blob_Fade_2_");
        [HideInInspector, Range(0f, 1f)] public float _Gaze_Intensity_ = 0f;
        public static int _Gaze_Intensity_ID = Shader.PropertyToID("_Gaze_Intensity_");
        [HideInInspector, Range(0f, 1f)] public float _Gaze_Focus_ = 0f;
        public static int _Gaze_Focus_ID = Shader.PropertyToID("_Gaze_Focus_");
        [HideInInspector] public float _Pinched_ = 0f;
        public static int _Pinched_ID = Shader.PropertyToID("_Pinched_");
        [HideInInspector] public Texture2D _Blob_Texture_ = null;
        public static int _Blob_Texture_ID = Shader.PropertyToID("_Blob_Texture_");
        [HideInInspector, Range(0f, 1f)] public float _Selection_Fuzz_ = 0.5f;
        public static int _Selection_Fuzz_ID = Shader.PropertyToID("_Selection_Fuzz_");
        [HideInInspector, Range(0f, 1f)] public float _Selected_ = 0f;
        public static int _Selected_ID = Shader.PropertyToID("_Selected_");
        [HideInInspector, Range(0f, 1f)] public float _Selection_Fade_ = 0f;
        public static int _Selection_Fade_ID = Shader.PropertyToID("_Selection_Fade_");
        [HideInInspector, Range(0f, 1f)] public float _Selection_Fade_Size_ = 0.3f;
        public static int _Selection_Fade_Size_ID = Shader.PropertyToID("_Selection_Fade_Size_");
        [HideInInspector, Range(0f, 1f)] public float _Selected_Distance_ = 0.08f;
        public static int _Selected_Distance_ID = Shader.PropertyToID("_Selected_Distance_");
        [HideInInspector, Range(0f, 1f)] public float _Selected_Fade_Length_ = 0.08f;
        public static int _Selected_Fade_Length_ID = Shader.PropertyToID("_Selected_Fade_Length_");
        [HideInInspector, Range(0f, 1f)] public float _Proximity_Max_Intensity_ = 0.45f;
        public static int _Proximity_Max_Intensity_ID = Shader.PropertyToID("_Proximity_Max_Intensity_");
        [HideInInspector, Range(0f, 2f)] public float _Proximity_Far_Distance_ = 0.16f;
        public static int _Proximity_Far_Distance_ID = Shader.PropertyToID("_Proximity_Far_Distance_");
        [HideInInspector, Range(0f, 2f)] public float _Proximity_Near_Radius_ = 0.03f;
        public static int _Proximity_Near_Radius_ID = Shader.PropertyToID("_Proximity_Near_Radius_");
        [HideInInspector, Range(0f, 1f)] public float _Proximity_Anisotropy_ = 1f;
        public static int _Proximity_Anisotropy_ID = Shader.PropertyToID("_Proximity_Anisotropy_");
        [HideInInspector] public float _Use_Global_Left_Index_ = 1f;
        public static int _Use_Global_Left_Index_ID = Shader.PropertyToID("_Use_Global_Left_Index_");
        [HideInInspector] public float _Use_Global_Right_Index_ = 1f;
        public static int _Use_Global_Right_Index_ID = Shader.PropertyToID("_Use_Global_Right_Index_");

        /// <inheritdoc/>
        public override void InitializeFromMaterial(Material material)
        {
            _Radius_ = material.GetFloat(_Radius_ID);
            _Line_Width_ = material.GetFloat(_Line_Width_ID);
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
        }

        /// <inheritdoc/>
        public override void ApplyToMaterial(Material material)
        {
            material.SetFloat(_Radius_ID, _Radius_);
            material.SetFloat(_Line_Width_ID, _Line_Width_);
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
        }

        /// <inheritdoc/>
        public override string GetTargetShaderName()
        {
            return "Graphics Tools/Canvas Frontplate";
        }
    }
}

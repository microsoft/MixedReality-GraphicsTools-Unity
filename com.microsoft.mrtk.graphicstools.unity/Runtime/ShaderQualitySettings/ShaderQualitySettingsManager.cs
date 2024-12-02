// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static Microsoft.MixedReality.GraphicsTools.ShaderQualitySettingsManager;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Shader Quality Settings Manager. Determines settings quality based on hardware and has 3 different levels: High, Medium and Low qualities.
    /// Supports setting the quality level on the shaders by use of Shader Keywords: MATERIAL_QUALITY_HIGH = High, MATERIAL_QUALITY_MEDIUM = Medium, MATERIAL_QUALITY_LOW = Low
    /// </summary>
    /// <remarks>
    /// and https://docs.unity3d.com/ScriptReference/SystemInfo-graphicsMemorySize.html
    /// </remarks>
    public class ShaderQualitySettingsManager : MonoBehaviour
    {
        [Serializable]
        public enum ShaderQualityLevel
        {
            Low = 100,
            Medium = 200,
            High = 300
        }

        [Tooltip("Toggle ON to use the hardware based shader quality setting.")]
        [SerializeField]
        private bool useHardwareSettings = true;

        [Tooltip("Toggle ON to override the hardware based shader quality setting with the current ManualQualityLevel dropdown one.")]
        [SerializeField]
        private bool overrideQualityLevel = false;

        [Tooltip("Shader quality options dropdown. Select one to use when the OverrideQualityLevel toggle is ON.")]
        [SerializeField]
        private ShaderQualityLevel manualQualityLevel = ShaderQualityLevel.High;

        public static event Action<ShaderQualityLevel> OnShaderQualityChanged;

        public static ShaderQualityLevel ShaderQualityLevelSetting { get; private set; } = ShaderQualityLevel.High;

        public static ShaderQualitySettingsManager Instance;

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            if (useHardwareSettings || overrideQualityLevel)
            {
                OnQualityChanged();
            }
        }

        private void DetermineShaderQualityLevel()
        {
            if (overrideQualityLevel)
            {
                ShaderQualityLevelSetting = manualQualityLevel;
                return;
            }

            if (useHardwareSettings)
            {
                // Select Quality settings level based on shader level support and on the size of the device's graphics memory
                if (SystemInfo.graphicsShaderLevel >= 45 && SystemInfo.graphicsMemorySize > 4096)
                {
                    ShaderQualityLevelSetting = ShaderQualityLevel.High;
                }
                else if (SystemInfo.graphicsShaderLevel >= 35 && SystemInfo.graphicsMemorySize > 2048)
                {
                    ShaderQualityLevelSetting = ShaderQualityLevel.Medium;
                }
                else
                {
                    ShaderQualityLevelSetting = ShaderQualityLevel.Low;
                }
            }
        }

        private void UpdateQualityLevel()
        {
            switch (ShaderQualityLevelSetting)
            {
                case ShaderQualityLevel.High:
                    MaterialQualityUtilities.SetGlobalShaderKeywords(MaterialQuality.High);
                    break;
                case ShaderQualityLevel.Medium:
                    MaterialQualityUtilities.SetGlobalShaderKeywords(MaterialQuality.Medium);
                    break;
                case ShaderQualityLevel.Low:
                    MaterialQualityUtilities.SetGlobalShaderKeywords(MaterialQuality.Low);
                    break;
                default:
                    MaterialQualityUtilities.SetGlobalShaderKeywords(MaterialQuality.High);
                    break;
            }
            OnShaderQualityChanged?.Invoke(ShaderQualityLevelSetting);
        }

        public void OnQualityChanged()
        {
            DetermineShaderQualityLevel();
            UpdateQualityLevel();
        }

        public void SetShaderQualityLevel(ShaderQualityLevel newQualityLevel)
        {
            if (ShaderQualityLevelSetting == newQualityLevel || overrideQualityLevel)
            {
                return;
            }
            ShaderQualityLevelSetting = newQualityLevel;
            UpdateQualityLevel();
            useHardwareSettings = false;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ShaderQualitySettingsManager))]
    public class ShaderQualitySettingsManagerEditor : Editor
    {
        SerializedProperty m_useHardwareSettings;
        SerializedProperty m_overrideQualityLevel;
        SerializedProperty m_manualQualityLevel;

        protected void OnEnable()
        {
            m_useHardwareSettings = serializedObject.FindProperty("useHardwareSettings");
            m_overrideQualityLevel = serializedObject.FindProperty("overrideQualityLevel");
            m_manualQualityLevel = serializedObject.FindProperty("manualQualityLevel");
            OnShaderQualityChanged += RefreshInspector;
        }

        protected void OnDisable()
        {
            OnShaderQualityChanged -= RefreshInspector;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (Application.IsPlaying(this))
            {
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
                EditorGUILayout.HelpBox($"Current Shader Quality: {ShaderQualityLevelSetting.ToString()}", MessageType.Info);
            }

            ShaderQualitySettingsManager shaderQualitySettingsManager = (ShaderQualitySettingsManager)target;

            if (!m_overrideQualityLevel.boolValue && !m_useHardwareSettings.boolValue)
            {
                shaderQualitySettingsManager.OnQualityChanged();
            } 
            else if (m_overrideQualityLevel.boolValue && m_manualQualityLevel.enumValueFlag != (int)ShaderQualityLevelSetting)
            {
                shaderQualitySettingsManager.OnQualityChanged();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void RefreshInspector(ShaderQualityLevel shaderQualityLevel)
        {
            Repaint();
        }
    }
#endif
}
#endif // GT_USE_URP

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    public class AmbientOcclusionToolWindow : EditorWindow
    {
        private AmbientOcclusionTool _ambientOcclusionTool;
        private bool _needsRepaint = false;

        [MenuItem("Window/Graphics Tools/Ambient occclusion")]
        private static void ShowWindow()
        {
            //if (Shader.Find(_standardShaderPath) == null)
            //{
            //    UnityEngine.Debug.LogError($"Unable to locate {_standardShaderPath}!");
            //}
            AmbientOcclusionToolWindow window = GetWindow<AmbientOcclusionToolWindow>();
            window.titleContent = new GUIContent("Ambient occlusion");
            window.Show();
        }

        public void Awake()
        {
            _ambientOcclusionTool = new AmbientOcclusionTool(AmbientOcclusionSettings.GetOrCreateSettings());
            
            OnSelectionChange();
        }

        public void CreateGUI()
        {
            rootVisualElement.Add(
                new HelpBox("Select game objects to modify", HelpBoxMessageType.Info));
            rootVisualElement.Add(AmbientOcclusionSettings.SettingsUI());
            rootVisualElement.Bind(AmbientOcclusionSettings.GetSerializedSettings());
            var button = new Button();
            button.name = nameof(_ambientOcclusionTool.GatherSelectionSamples);
            button.text = "Gather selection samples";
            rootVisualElement.Add(button);
        }

        private void OnValidate()
        {
            _needsRepaint = true;
        }

        private void OnFocus()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.duringSceneGui -= OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnSelectionChange()
        {
            _ambientOcclusionTool.OnSelectionChanged();
        }

        private void OnDestroy()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.duringSceneGui -= OnSceneGUI;
            ToolManager.RestorePreviousPersistentTool();
        }

        /// <summary>
        /// Custom inspector that exposes additional user controls for the AmbientOcclusion component
        /// </summary>
        //[CustomEditor(typeof(AmbientOcclusion)), CanEditMultipleObjects]
        private void OnSceneGUI(SceneView sceneView)
        {
            //EditorGUILayout.LabelField("Ray tracing");

            //var _samplesPerVertexLabels = new string[] { "1", "10", "100", "1000", "10000" };
            //_ambientOcclusionTool.settings.SamplesPerVertex = EditorGUILayout.Popup("Samples per vertex", _samplesIndex, _samplesPerVertexLabels);
            //_samplesPerVertex = (int)Mathf.Pow(10, _samplesIndex);

            //MaxSampleDistance = EditorGUILayout.FloatField("Max sample distance", MaxSampleDistance);
            //_upgradeMaterials = EditorGUILayout.Toggle("Update materials", PlayerPrefs.GetInt(upgradePrefsKey) > 0);
            //PlayerPrefs.SetInt(upgradePrefsKey, _upgradeMaterials == false ? 0 : 1);

            //if (GUILayout.Button("Gather selection samples"))
            //{
            //    _ambientOcclusionTool.GatherSelectionSamples();
            //}

            //_showSamples = EditorGUILayout.Toggle("Show samples", _showSamples);

            if (_needsRepaint)
            {
                HandleUtility.Repaint();
            }
        }
    }
}

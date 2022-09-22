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
        private static AmbientOcclusionToolWindow window;
        private HelpBox _helpBox;

        public void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            _ambientOcclusionTool = new AmbientOcclusionTool(AmbientOcclusionSettings.GetOrCreateSettings());
            OnSelectionChange();
        }

        public void CreateGUI()
        {
            _helpBox = new HelpBox("Select some objects to modify, the press 'Apply'.", HelpBoxMessageType.Info);
            rootVisualElement.Add(_helpBox);

            var toolUI = AmbientOcclusionSettings.SettingsUI();
            if (toolUI.Query<Button>("apply").First() is Button button)
            {
                button.clicked += OnApplyButtonClicked;
            }
            rootVisualElement.Add(toolUI);
            rootVisualElement.Bind(AmbientOcclusionSettings.GetSerializedSettings());
        }

        private void OnValidate()
        {
            _needsRepaint = true;
        }

        private void OnSelectionChange()
        {
            if (_helpBox != null)
            {
                if (Selection.gameObjects.Length < 1)
                {
                    _helpBox.text = "Select game objects to modify, then press 'Apply'.";
                }
                else
                {
                    _helpBox.text = "Press the 'Apply' button to calculate occlusion.";
                }
            }

            if (_ambientOcclusionTool != null)
            {
                _ambientOcclusionTool.OnSelectionChanged();
            }
        }

        private void OnDestroy()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.duringSceneGui -= OnSceneGUI;
            ToolManager.RestorePreviousPersistentTool();
        }

        [MenuItem("Window/Graphics Tools/Ambient occclusion")]
        private static void ShowWindow()
        {
            if (window == null)
            {
                window = GetWindow<AmbientOcclusionToolWindow>();
                window.titleContent = new GUIContent("Ambient occlusion");
                window.Show();
            }
            else
            {
                window.Repaint();
            }
        }

        //private void OnFocus()
        //{
        //    // Remove delegate listener if it has previously
        //    // been assigned.
        //    SceneView.duringSceneGui -= OnSceneGUI;
        //    // Add (or re-add) the delegate.
        //    SceneView.duringSceneGui += OnSceneGUI;
        //}

        private void OnApplyButtonClicked()
        {
            if (_ambientOcclusionTool != null)
            {
                _ambientOcclusionTool.GatherSelectionSamples();
            }
        }

        /// <summary>
        /// Custom inspector that exposes additional user controls for the AmbientOcclusion component
        /// </summary>
        private void OnSceneGUI(SceneView sceneView)
        {
            Handles.RadiusHandle(new Quaternion(), Vector3.zero, _ambientOcclusionTool.settings.MaxSampleDistance);
            if (_needsRepaint)
            {
                if (_ambientOcclusionTool != null)
                {
                    _ambientOcclusionTool.DrawVisualization();
                }
                HandleUtility.Repaint();
                _needsRepaint = false;
            }
        }
    }
}

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
        private static AmbientOcclusionToolWindow window;
        private HelpBox _helpBox;
        private Button _fixMeshCollider;

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

        public void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            _ambientOcclusionTool = new AmbientOcclusionTool(AmbientOcclusionSettings.GetOrCreateSettings());
        }

        public void CreateGUI()
        {
            /// To get a valid type attribute value for ObjectFields...
            /// For example, a Shader would evaluate: `typeof(Shader).AssemblyQualifiedName`
            
            var toolUI = AmbientOcclusionSettings.SettingsUI();
            if (toolUI.Query<Button>("apply").First() is Button button)
            {
                button.clicked += OnApplyButtonClicked;
            }
            if (toolUI.Query<HelpBox>("help").First() is HelpBox help)
            {
                _helpBox = help;
            }
            if (toolUI.Query<Button>("fixMeshCollider").First() is Button fixMeshCollider)
            {
                _fixMeshCollider = fixMeshCollider;
                _fixMeshCollider.clicked += FixMeshColliderClicked;
            }
            rootVisualElement.Add(toolUI);
            rootVisualElement.Bind(AmbientOcclusionSettings.GetSerializedSettings());
            UpdateHelp();
        }

        private void FixMeshColliderClicked()
        {
            foreach (var item in Selection.gameObjects)
            {
                if (item.GetComponent<MeshCollider>() == null)
                {
                    item.AddComponent<MeshCollider>();
                }
            }
            UpdateHelp();
        }

        private void OnSelectionChange()
        {
            UpdateHelp();

            if (_ambientOcclusionTool != null)
            {
                _ambientOcclusionTool.OnSelectionChanged();
            }
        }

        private void UpdateHelp()
        {
            if (_fixMeshCollider != null)
            {
                _fixMeshCollider.style.display = DisplayStyle.None;
            }

            if (_helpBox != null)
            {
                if (Selection.gameObjects.Length < 1)
                {
                    _helpBox.text = "Select game objects to modify, then press 'Apply'.";
                }
                else
                {
                    _helpBox.text = "Press 'Apply' to calculate occlusion and show visualization.";
                }

                foreach (var selected in Selection.gameObjects)
                {
                    if (selected.GetComponent<MeshCollider>() == null)
                    {
                        _helpBox.text = $"{selected.name} has no mesh collider! (You should probably add one)\n";
                        _helpBox.text += "Press 'Apply' to calculate occlusion and show visualization.";
                        if (_fixMeshCollider != null)
                        {
                            _fixMeshCollider.style.display = DisplayStyle.Flex;
                            break;
                        }
                    }
                }
                rootVisualElement.MarkDirtyRepaint();
            }
        }

        private void OnDestroy()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.duringSceneGui -= OnSceneGUI;
            ToolManager.RestorePreviousPersistentTool();
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
            if (_ambientOcclusionTool != null)
            {
                _ambientOcclusionTool.DrawVisualization();
            }
            HandleUtility.Repaint();
        }
    }
}

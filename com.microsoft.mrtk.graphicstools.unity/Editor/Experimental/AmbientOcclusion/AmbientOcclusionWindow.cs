// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Works with UXML to bind the controls for ambient occlusion tool settings
    /// </summary>
    public class AmbientOcclusionToolWindow : EditorWindow
    {
        private static AmbientOcclusionToolWindow window;

        private AmbientOcclusionTool _ambientOcclusionTool;
        private HelpBox _helpBox;
        private Button _fixMeshCollider; // used by help
        private int _maxVertexIndex; // used for user input validation
        private GameObject _lastVisualizedGO; // state used by visualization
        private bool _shouldShowVis; // controls if we display some visuals in the scene view
        private IntegerField _referenceVertexIndexField; // when selection changes we need to update this for validate

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

        private void OnEnable()
        {
            _ambientOcclusionTool = new AmbientOcclusionTool(AmbientOcclusionSettings.GetOrCreateSettings());
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void CreateGUI()
        {
            var toolUI = AmbientOcclusionSettings.SettingsUI();
            rootVisualElement.Add(toolUI);
            rootVisualElement.Bind(AmbientOcclusionSettings.GetSerializedSettings());
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
            // Note would be nice to not have to validate in Window and in Settings....
            if (toolUI.Query<IntegerField>("samplesPerVertex").First() is IntegerField integerField)
            {
                integerField.RegisterCallback<ChangeEvent<int>>(e => integerField.value = Mathf.Clamp(e.newValue, 1, 100000));
            }
            if (toolUI.Query<IntegerField>("referenceVertexIndex").First() is IntegerField refVtxIdxField)
            {
                refVtxIdxField.RegisterCallback<ChangeEvent<int>>(e => refVtxIdxField.value = Mathf.Clamp(e.newValue, 0, _maxVertexIndex));
                _referenceVertexIndexField = refVtxIdxField;
            }
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

            if (Selection.gameObjects.Length == 0)
            {
                return;
            }

            var firstSelectedGO = Selection.gameObjects[0];
            if (firstSelectedGO == null)
            {
                return;
            }

            // We've seen this before, draw it
            _shouldShowVis = false;
            if (firstSelectedGO == _lastVisualizedGO)
            {
                _shouldShowVis = true;
            }

            // Update validation limits for input fields
            var mf = firstSelectedGO.GetComponent<MeshFilter>();
            if (mf != null)
            {
                _maxVertexIndex = mf.sharedMesh.vertexCount - 1;
                _referenceVertexIndexField.value = Mathf.Clamp(_referenceVertexIndexField.value, 0, _maxVertexIndex);
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
                _helpBox.text = "";

                if (Selection.gameObjects.Length < 1)
                {
                    _helpBox.messageType = HelpBoxMessageType.Info;
                    _helpBox.text = "Select game objects to modify, then press 'Apply'.";
                }
                else
                {
                    _helpBox.messageType = HelpBoxMessageType.Info;
                    _helpBox.text = "Press 'Apply' to calculate occlusion and show visualization.";
                }

                foreach (var selected in Selection.gameObjects)
                {
                    var mf = selected.GetComponent<MeshFilter>();
                    if (mf == null || mf.sharedMesh == null)
                    {
                        _helpBox.messageType = HelpBoxMessageType.Error;
                        _helpBox.text = $"{selected.name} has no MeshFilter!";
                        continue;
                    }
                    if (selected.GetComponent<MeshCollider>() == null)
                    {
                        _helpBox.messageType = HelpBoxMessageType.Warning;
                        _helpBox.text = $"{selected.name} has no mesh collider! Please add one. You can delete it later.";
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
            SceneView.duringSceneGui -= OnSceneGUI;
            ToolManager.RestorePreviousPersistentTool();
        }

        private void OnApplyButtonClicked()
        {
            _lastVisualizedGO = null;
            foreach (var item in Selection.gameObjects)
            {
                // Used by visualization to draw the first selected thing
                if (_lastVisualizedGO == null)
                {
                    _lastVisualizedGO = item;
                }
                _ambientOcclusionTool.GatherSamples(item);
            }
            _shouldShowVis = true;  
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (_ambientOcclusionTool != null)
            {
                if (_shouldShowVis)
                {
                    _ambientOcclusionTool.DrawVisualization();
                }
            }
            HandleUtility.Repaint();
        }
    }
}

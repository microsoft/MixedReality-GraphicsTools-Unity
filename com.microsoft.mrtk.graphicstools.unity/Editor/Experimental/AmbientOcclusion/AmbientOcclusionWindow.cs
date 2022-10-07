// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
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
        private int _maxVertexIndex; // used for user input validation
        private GameObject _lastVisualizedGO; // state used by visualization
        private bool _shouldShowVis; // controls if we display some visuals in the scene view
        private IntegerField _referenceVertexIndexField; // when selection changes we need to update this for validate
        private Button _applyButton;
        private List<MeshCollider> _tempColliders = new List<MeshCollider>();

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
            // Be helpful
            if (toolUI.Query<HelpBox>("help").First() is HelpBox help)
            {
                _helpBox = help;
            }
            // Find and hook up buttons
            if (toolUI.Query<Button>("apply").First() is Button button)
            {
                button.clicked += OnApplyButtonClicked;
                _applyButton = button;
            }
            // Validation for input
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

        private void OnSelectionChange()
        {
            UpdateHelp();
            rootVisualElement.MarkDirtyRepaint();

            if (Selection.gameObjects.Length == 0)
            {
                _shouldShowVis = false;
                return;
            }

            GameObject firstSelected = FirstSelectedGameObjectWithMeshFilter();
            if (firstSelected == null)
            {
                _shouldShowVis = false;
                return;
            }

            // We've seen this before, draw it
            _shouldShowVis = false;
            if (firstSelected == _lastVisualizedGO)
            {
                _shouldShowVis = true;
            }

            // Update validation limits for input fields
            var firstMeshFilter = firstSelected.GetComponent<MeshFilter>();
            if (firstMeshFilter != null)
            {
                _maxVertexIndex = firstMeshFilter.sharedMesh.vertexCount - 1;
                _referenceVertexIndexField.value = Mathf.Clamp(_referenceVertexIndexField.value, 0, _maxVertexIndex);
            }
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            ToolManager.RestorePreviousPersistentTool();
        }
        
        private GameObject FirstSelectedGameObjectWithMeshFilter()
        {
            GameObject result = null;
            foreach (var item in Selection.GetFiltered<MeshFilter>(SelectionMode.Deep))
            {
                var meshFilter = item.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    result = item.gameObject;
                    break;
                }
            }
            return result;
        }

        private void UpdateHelp()
        {
            _applyButton.visible = true;

            if (_helpBox != null)
            {
                _helpBox.text = "";

                var first = FirstSelectedGameObjectWithMeshFilter();
                if (first == null)
                {
                    _helpBox.messageType = HelpBoxMessageType.Info;
                    _helpBox.text = "Select meshes to modify...";
                    _applyButton.visible = false;
                }
                else
                {
                    _helpBox.messageType = HelpBoxMessageType.Info;
                    _helpBox.text = "Press 'Apply' to calculate occlusion and show visualization.";
                }
            }
        }

        private void OnApplyButtonClicked()
        {
            var meshFilters = Selection.GetFiltered<MeshFilter>(SelectionMode.Deep);
            // Ensure we have something to collide with
            foreach (var item in meshFilters)
            {
                var meshCollider = item.GetComponent<MeshCollider>();
                if (meshCollider == null)
                {
                    _tempColliders.Add(item.gameObject.AddComponent<MeshCollider>());
                }
            }
            _lastVisualizedGO = null;
            foreach (var item in meshFilters)
            {
                // Used by visualization to draw the first selected thing
                if (_lastVisualizedGO == null)
                {
                    _lastVisualizedGO = item.gameObject;
                }
                var meshFilter = item.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    _ambientOcclusionTool.GatherSamples(meshFilter);
                }
                // Ensure material is setup to display results
                var meshRenderer = item.GetComponent<MeshRenderer>();
                if (_ambientOcclusionTool.settings._upgradeMaterials)
                {
                    if (meshRenderer != null)
                    {
                        if (_ambientOcclusionTool.settings._upgradeMaterials)
                        {
                            _ambientOcclusionTool.ModifyMaterials(meshRenderer);
                        }
                    }
                }
            }
            // Delete our temp colliders
            foreach (var collider in _tempColliders)
            {
                DestroyImmediate(collider);
            }
            _tempColliders.Clear();
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

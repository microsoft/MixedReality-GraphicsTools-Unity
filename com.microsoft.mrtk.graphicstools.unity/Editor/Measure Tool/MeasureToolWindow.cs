// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// A custom EditorWindow showing the measure tool
    /// </summary>
    public class MeasureToolWindow : EditorWindow
    {
        private MeasureTool measureTool;
        private bool needsRepaint = false;

        [MenuItem("Window/Graphics Tools/Measure Tool")]
        public static void ShowWindow()
        {
            MeasureToolWindow wnd = GetWindow<MeasureToolWindow>();
            wnd.titleContent = new GUIContent("Measure Tool");
            
            if (ToolManager.activeToolType == typeof(MeasureToolEditorTool))
            {
                ToolManager.RestorePreviousPersistentTool();
            }
        }

        [Shortcut("Measure Tool/Open Window", KeyCode.M, ShortcutModifiers.Shift)]
        private static void ToggleWindow()
        {
            if (HasOpenInstances<MeasureToolWindow>())
            {
                GetWindow<MeasureToolWindow>().Close();
            }
            else
            {
                ShowWindow();
            }
        }

        public void OnEnable()
        {
            measureTool = new MeasureTool(MeasureToolSettings.GetOrCreateSettings());
            var root = this.rootVisualElement;
            HelpBox helpBox = new HelpBox("Measurements of selected items will display while this window is open",HelpBoxMessageType.Info);
            root.Add(helpBox);
            root.Add(MeasureToolSettings.SettingsUI());
            root.Bind(MeasureToolSettings.GetSerializedSettings());
            OnSelectionChange();
            
        }

        private void OnValidate()
        {
            needsRepaint = true;
        }

        private void OnFocus()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.duringSceneGui -= OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDestroy()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.duringSceneGui -= OnSceneGUI;
            ToolManager.RestorePreviousPersistentTool();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            measureTool.DrawMeasurement(Selection.gameObjects);

            if (needsRepaint)
            {
                HandleUtility.Repaint();
            }
        }

        private void OnSelectionChange()
        {
            measureTool.OnSelectionChanged();
        }
    }
}

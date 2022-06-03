// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// A custom EditorTool for adding the measure tool to the Unity Toolbar
    /// </summary>
    [EditorTool("Measure Tool")]
    public class MeasureToolEditorTool : EditorTool
    {
        [SerializeField]
        private Texture2D toolIcon;

        private GUIContent iconContent;
        private VisualElement _toolRootElement;
        private MeasureTool measureTool;
        private bool needsRepaint = false;

        private void OnEnable()
        {
            iconContent = new GUIContent()
            {
                image = toolIcon,
                text = "Measure Tool",
                tooltip = "Measure Tool: Get measurements of rects, renderers and colliders with this tool"
            };

            measureTool = new MeasureTool(MeasureToolSettings.GetOrCreateSettings());
        }

        public override GUIContent toolbarIcon
        {
            get { return iconContent; }
        }

        /// <summary>
        /// Adds a shortcut for toggling the Tool
        /// Default key is M, it can be changed via the Edit/Shortcut menu
        /// </summary>
        [Shortcut("Measure Tool/Toggle", KeyCode.M)]
        private static void MeasureToolShortcut()
        {
            if (ToolManager.activeToolType == typeof(MeasureToolEditorTool))
            {
                ToolManager.RestorePreviousTool();
            }
            else
            {
                ToolManager.SetActiveTool<MeasureToolEditorTool>();
            }
        }        

        public override void OnToolGUI(EditorWindow window)
        {
            if (!(window is SceneView))
            {
                return;
            }

            if (!ToolManager.IsActiveTool(this))
            {
                return;
            }

            if (Selection.count < 1)
            {
                return;
            }
            if (EditorWindow.HasOpenInstances<MeasureToolWindow>())
            {
                return;
            }
            measureTool.DrawMeasurement(Selection.gameObjects);

            if (needsRepaint)
            {
                HandleUtility.Repaint();
            }

        }

        public override void OnActivated()
        {
            _toolRootElement = new VisualElement();
            _toolRootElement.style.width = 300;
            var backgroundColor = EditorGUIUtility.isProSkin
                ? new Color(0.21f, 0.21f, 0.21f, 0.8f)
                : new Color(0.8f, 0.8f, 0.8f, 0.8f);
            _toolRootElement.style.backgroundColor = backgroundColor;
            _toolRootElement.style.marginLeft = 10f;
            _toolRootElement.style.marginBottom = 10f;
            _toolRootElement.style.marginTop = 10f;
            _toolRootElement.style.paddingTop = 5f;
            _toolRootElement.style.paddingRight = 5f;
            _toolRootElement.style.paddingLeft = 5f;
            _toolRootElement.style.paddingBottom = 5f;
            var titleLabel = new Label("Measure Tool");
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

            Button button = new Button(() =>
            {
                MeasureToolWindow.ShowWindow();
                _toolRootElement.visible = !EditorWindow.HasOpenInstances<MeasureToolWindow>();
                ToolManager.RestorePreviousPersistentTool();
            })
            { text = "Keep tool active" };

            _toolRootElement.Add(titleLabel);
            _toolRootElement.Add(MeasureToolSettings.SettingsUI());
            _toolRootElement.Add(button);
            _toolRootElement.visible = !EditorWindow.HasOpenInstances<MeasureToolWindow>();

            var sv = SceneView.lastActiveSceneView;
            sv.rootVisualElement.Add(_toolRootElement);
            sv.rootVisualElement.Bind(MeasureToolSettings.GetSerializedSettings());
            sv.rootVisualElement.style.flexDirection = FlexDirection.ColumnReverse;

            measureTool.SelectedObjects = new List<GameObject>();
            Selection.selectionChanged += measureTool.OnSelectionChanged;
            measureTool.OnSelectionChanged();

        }

        public override void OnWillBeDeactivated()
        {
            _toolRootElement?.RemoveFromHierarchy();
            Selection.selectionChanged -= measureTool.OnSelectionChanged;
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UEditor = UnityEditor.Editor;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Helper class to get ScaleMeshEffect onto Canvas objects.
    /// </summary>

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Canvas))]
    public class CanvasInspector : UEditor
    {
        private readonly List<Graphic> graphicsWhichRequireScaleMeshEffect = new List<Graphic>();
        private Type canvasEditorType = null;
        private UEditor internalEditor = null;
        private Canvas canvas = null;
        private bool isRootCanvas = false;

        private void OnEnable()
        {
            canvasEditorType = Type.GetType("UnityEditor.CanvasEditor, UnityEditor");
            if (canvasEditorType != null)
            {
                internalEditor = CreateEditor(targets, canvasEditorType);
                canvas = target as Canvas;
                isRootCanvas = canvas.transform.parent == null || canvas.transform.parent.GetComponentInParent<Canvas>() == null;
            }
        }

        private void OnDisable()
        {
            if (canvasEditorType != null)
            {
                MethodInfo onDisable = canvasEditorType.GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic);
                if (onDisable != null)
                {
                    onDisable.Invoke(internalEditor, null);
                }
                DestroyImmediate(internalEditor);
            }
        }

        public override void OnInspectorGUI()
        {
            if (isRootCanvas && canvas != null)
            {
                List<Graphic> graphics = GetGraphicsWhichRequireScaleMeshEffect(targets);

                if (graphics.Count != 0)
                {
                    EditorGUILayout.HelpBox($"Canvas contains {graphics.Count} {typeof(Graphic).Name}(s) which require a {typeof(ScaleMeshEffect).Name} to work with the {StandardShaderUtility.GraphicsToolsStandardShaderName} shader.", MessageType.Warning);
                    if (GUILayout.Button($"Add {typeof(ScaleMeshEffect).Name}(s)"))
                    {
                        foreach (var graphic in graphics)
                        {
                            Undo.AddComponent<ScaleMeshEffect>(graphic.gameObject);
                        }
                    }
                }

                EditorGUILayout.Space();
            }

            if (internalEditor != null)
            {
                internalEditor.OnInspectorGUI();
            }
        }

        private List<Graphic> GetGraphicsWhichRequireScaleMeshEffect(UnityEngine.Object[] targets)
        {
            graphicsWhichRequireScaleMeshEffect.Clear();

            foreach (UnityEngine.Object target in targets)
            {
                Graphic[] graphics = (target as Canvas).GetComponentsInChildren<Graphic>();

                foreach (Graphic graphic in graphics)
                {
                    if (StandardShaderUtility.IsUsingGraphicsToolsStandardShader(graphic.material) &&
                        graphic.GetComponent<ScaleMeshEffect>() == null)
                    {
                        graphicsWhichRequireScaleMeshEffect.Add(graphic);
                    }
                }
            }

            return graphicsWhichRequireScaleMeshEffect;
        }
    }
}
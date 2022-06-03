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
    /// Helper class to show warnings on canvas renderer components.
    /// </summary>

    [CanEditMultipleObjects]
    [CustomEditor(typeof(CanvasRenderer))]
    public class CanvasRendererInspector : UEditor
    {
        private readonly List<Material> materialsToFix = new List<Material>();
        private readonly List<Graphic> graphicsToFix = new List<Graphic>();
        private Type canvasEditorType = null;
        private UEditor internalEditor = null;

        private void OnEnable()
        {
            canvasEditorType = Type.GetType("UnityEditor.CanvasRendererEditor, UnityEditor");
            if (canvasEditorType != null)
            {
                internalEditor = CreateEditor(targets, canvasEditorType);
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

        /// <summary>
        /// Renders a custom inspector GUI that displays warnings and mitigation options to the user.
        /// </summary>
        public override void OnInspectorGUI()
        {
            GetMaterialsWhichDontSupportCanvas(targets, materialsToFix);

            if (materialsToFix.Count != 0)
            {
                EditorGUILayout.HelpBox($"The {typeof(CanvasRenderer).Name} is using a {typeof(Material).Name} which is using the {StandardShaderUtility.GraphicsToolsStandardShaderName} instead of the {StandardShaderUtility.GraphicsToolsStandardCanvasShaderName} shader. The {StandardShaderUtility.GraphicsToolsStandardCanvasShaderName} is required for some shader features to function.", MessageType.Warning);
                if (GUILayout.Button($"Change {typeof(Shader).Name} to {StandardShaderUtility.GraphicsToolsStandardCanvasShaderName}"))
                {
                    Undo.RecordObjects(materialsToFix.ToArray(), "Change Shader");
                    foreach (var material in materialsToFix)
                    {
                        material.shader = StandardShaderUtility.GraphicsToolsStandardCanvasShader;
                    }
                }
            }

            GetGraphicsWhichRequireScaleMeshEffect(targets, graphicsToFix);

            EditorGUILayout.Space();

            if (graphicsToFix.Count != 0)
            {
                EditorGUILayout.HelpBox($"This gameobject requires a {typeof(ScaleMeshEffect).Name} component to work with the {StandardShaderUtility.GraphicsToolsStandardCanvasShaderName} or {StandardShaderUtility.GraphicsToolsStandardShaderName} shader.", MessageType.Warning);
                if (GUILayout.Button($"Add {typeof(ScaleMeshEffect).Name}(s)"))
                {
                    foreach (var graphic in graphicsToFix)
                    {
                        Undo.AddComponent<ScaleMeshEffect>(graphic.gameObject);
                    }
                }
            }

            EditorGUILayout.Space();
            if (internalEditor != null)
            {
                internalEditor.OnInspectorGUI();
            }
        }

        private static void GetMaterialsWhichDontSupportCanvas(UnityEngine.Object[] targets, List<Material> output)
        {
            output.Clear();

            foreach (UnityEngine.Object target in targets)
            {
                CanvasRenderer[] renderers = (target as CanvasRenderer).GetComponents<CanvasRenderer>();

                foreach (var renderer in renderers)
                {
                    var material = renderer.GetMaterial();

                    if (material != null && material.shader == StandardShaderUtility.GraphicsToolsStandardShader)
                    {
                        output.Add(material);
                    }
                }
            }
        }

        private static void GetGraphicsWhichRequireScaleMeshEffect(UnityEngine.Object[] targets, List<Graphic> output)
        {
            output.Clear();

            foreach (UnityEngine.Object target in targets)
            {
                Graphic[] graphics = (target as CanvasRenderer).GetComponents<Graphic>();

                foreach (Graphic graphic in graphics)
                {
                    if (StandardShaderUtility.IsUsingGraphicsToolsStandardShader(graphic.material) && graphic.GetComponent<ScaleMeshEffect>() == null)
                    {
                        output.Add(graphic);
                    }
                }
            }
        }
    }

}
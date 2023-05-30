// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_UGUI
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
    /// Helper class to show warnings on canvas objects.
    /// </summary>

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Canvas))]
    public class CanvasInspector : UEditor
    {
        private readonly List<Material> materialsToFix = new List<Material>();
        private readonly List<Graphic> graphicsToFix = new List<Graphic>();
        private readonly List<RectMask2D> masksToFix = new List<RectMask2D>();
        private Type canvasEditorType = null;
        private UEditor internalEditor = null;
        private Canvas canvas = null;
        private bool isRootCanvas = false;
        private bool showWarnings = true;

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

        /// <summary>
        /// Renders a custom inspector GUI that displays warnings and mitigation options to the user.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (isRootCanvas && canvas != null)
            {
                GetMaterialsWhichDontSupportCanvas(targets, materialsToFix);
                GetChildGraphicsWhichRequireScaleMeshEffect(targets, graphicsToFix);
                GetChildMasksThatRequireRectMask2DFast(targets, masksToFix);

                if (materialsToFix.Count > 0 || graphicsToFix.Count > 0 || masksToFix.Count > 0)
                {
                    showWarnings = EditorGUILayout.Foldout(showWarnings, "Warnings");
                    if (showWarnings)
                    {
                        if (materialsToFix.Count != 0)
                        {
                            EditorGUILayout.HelpBox($"Canvas contains {materialsToFix.Count} {typeof(Material).Name}(s) which are using the {StandardShaderUtility.GraphicsToolsStandardShaderName} instead of the {StandardShaderUtility.GraphicsToolsStandardCanvasShaderName} shader. The {StandardShaderUtility.GraphicsToolsStandardCanvasShaderName} is required for some shader features to function.", MessageType.Warning);
                            if (GUILayout.Button($"Change {typeof(Shader).Name}(s) to {StandardShaderUtility.GraphicsToolsStandardCanvasShaderName}"))
                            {
                                Undo.RecordObjects(materialsToFix.ToArray(), "Change Shader");
                                foreach (var material in materialsToFix)
                                {

                                    material.shader = StandardShaderUtility.GraphicsToolsStandardCanvasShader;
                                }
                            }
                        }

                        EditorGUILayout.Space();

                        if (graphicsToFix.Count != 0)
                        {
                            EditorGUILayout.HelpBox($"Canvas contains {graphicsToFix.Count} {typeof(Graphic).Name} components(s) which require a {typeof(ScaleMeshEffect).Name} to work with the {StandardShaderUtility.GraphicsToolsStandardCanvasShaderName} shader.", MessageType.Warning);
                            if (GUILayout.Button($"Add {typeof(ScaleMeshEffect).Name}(s)"))
                            {
                                foreach (var graphic in graphicsToFix)
                                {
                                    Undo.AddComponent<ScaleMeshEffect>(graphic.gameObject);
                                }
                            }
                        }

                        EditorGUILayout.Space();

                        if (masksToFix.Count != 0)
                        {
                            EditorGUILayout.HelpBox($"Canvas contains {masksToFix.Count} {typeof(RectMask2D).Name} components(s) which may be slow when masking many objects, consider switching to {typeof(RectMask2DFast).Name}.", MessageType.Warning);
                            if (GUILayout.Button($"Replace with {typeof(RectMask2DFast).Name}"))
                            {
                                foreach (RectMask2D mask in masksToFix)
                                {
                                    RectMask2DInspector.ReplaceRectMaskWithRectMask2DFast(mask);
                                }
                            }
                        }

                        EditorGUILayout.Space();
                    }
                }
            }

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
                CanvasRenderer[] renderers = (target as Canvas).GetComponentsInChildren<CanvasRenderer>();

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

        private static void GetChildGraphicsWhichRequireScaleMeshEffect(UnityEngine.Object[] targets, List<Graphic> output)
        {
            output.Clear();

            foreach (UnityEngine.Object target in targets)
            {
                Graphic[] graphics = (target as Canvas).GetComponentsInChildren<Graphic>();

                foreach (var graphic in graphics)
                {
                    if (StandardShaderUtility.IsUsingGraphicsToolsStandardShader(graphic.material) &&
                        graphic.GetComponent<ScaleMeshEffect>() == null)
                    {
                        output.Add(graphic);
                    }
                }
            }
        }

        private static void GetChildMasksThatRequireRectMask2DFast(UnityEngine.Object[] targets, List<RectMask2D> output)
        {
            output.Clear();

            foreach (UnityEngine.Object target in targets)
            {
                var masks = (target as Canvas).GetComponentsInChildren<RectMask2D>();

                foreach (var mask in masks)
                {
                    if (mask as RectMask2DFast == null)
                    {
                        output.Add(mask);
                    }
                }
            }
        }
    }
}
#endif // GT_USE_UGUI
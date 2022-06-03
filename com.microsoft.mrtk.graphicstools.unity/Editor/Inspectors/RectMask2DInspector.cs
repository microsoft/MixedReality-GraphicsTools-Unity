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
    /// Helper class to replace RectMask2D with RectMask2DFast objects.
    /// </summary>

    [CanEditMultipleObjects]
    [CustomEditor(typeof(RectMask2D))]
    public class RectMask2DInspector : UEditor
    {
        private readonly List<RectMask2D> masksToFix = new List<RectMask2D>();
        private Type rectMask2DEditorType = null;
        private UEditor internalEditor = null;

        private void OnEnable()
        {
            rectMask2DEditorType = Type.GetType("UnityEditor.UI.RectMask2DEditor, UnityEditor.UI");
            if (rectMask2DEditorType != null)
            {
                internalEditor = CreateEditor(targets, rectMask2DEditorType);
            }
        }

        private void OnDisable()
        {
            if (rectMask2DEditorType != null)
            {
                MethodInfo onDisable = rectMask2DEditorType.GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic);
                if (onDisable != null)
                {
                    onDisable.Invoke(internalEditor, null);
                }
                DestroyImmediate(internalEditor);
            }
        }

        public override void OnInspectorGUI()
        {
            GetMasksWhichRequireRectMask2DFast(Selection.gameObjects, masksToFix);

            if (masksToFix.Count != 0)
            {
                EditorGUILayout.HelpBox($"This component may be slow when masking many objects, consider switching to {typeof(RectMask2DFast).Name}.", MessageType.Warning);
                if (GUILayout.Button($"Replace with {typeof(RectMask2DFast).Name}"))
                {
                    foreach (var mask in masksToFix)
                    {
                        ReplaceRectMaskWithRectMask2DFast(mask);
                    }
                }
            }

            EditorGUILayout.Space();
            if (internalEditor != null && target != null)
            {
                internalEditor.OnInspectorGUI();
            }
        }

        private static void GetMasksWhichRequireRectMask2DFast(GameObject[] targets, List<RectMask2D> output)
        {
            output.Clear();

            foreach (GameObject target in targets)
            {
                RectMask2D mask = target.GetComponent<RectMask2D>();

                if (mask != null && mask as RectMask2DFast == null)
                {
                    output.Add(mask);
                }
            }
        }

        public static void ReplaceRectMaskWithRectMask2DFast(RectMask2D mask)
        {
            if (mask == null)
            {
                return;
            }

            GameObject gameObject = mask.gameObject;
            Vector4 oldPadding = mask.padding;
            Vector2Int oldSoftness = mask.softness;
            Undo.DestroyObjectImmediate(mask);
            mask = Undo.AddComponent<RectMask2DFast>(gameObject);
            mask.softness = oldSoftness;
            mask.padding = oldPadding;
        }
    }

}
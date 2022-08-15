// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Custom editor for the RoundedRectMask2D component.
    /// </summary>
    [CustomEditor(typeof(RoundedRectMask2D), true)]
    [CanEditMultipleObjects]
    public class RoundedRectMask2DInspector : RectMask2DFastInspector
    {
        private static GUIContent radiusContent = new GUIContent("Corner Radius", "Determines the radius of the rect mask's corners.");
        private static GUIContent radiiContent = new GUIContent("Corner Radii", "Determines the radius of each corner of the rect mask.");
        private static GUIContent topLeftContent = new GUIContent("Top Left", "Local -X, +Y corner.");
        private static GUIContent topRightContent = new GUIContent("Top Right", "Local +X, +Y corner.");
        private static GUIContent bottomLeftContent = new GUIContent("Bottom Left", "Local -X, -Y corner.");
        private static GUIContent bottomRightContent = new GUIContent("Bottom Right", "Local +X, -Y corner.");
        private static bool showRadii = false;

        private static GUIContent paddingContent = new GUIContent("Padding");
        private static GUIContent leftContent = new GUIContent("Left");
        private static GUIContent rightContent = new GUIContent("Right");
        private static GUIContent topContent = new GUIContent("Top");
        private static GUIContent bottomContent = new GUIContent("Bottom");
        private static bool showOffsets = false;

        private SerializedProperty independentRadii;
        private SerializedProperty radii;
        private SerializedProperty padding;

        [MenuItem("GameObject/UI/Graphics Tools/Rounded Rect Mask")]
        private static void CreateCanvasElement(MenuCommand menuCommand)
        {
            InspectorUtilities.CreateGameObjectFromMenu<RoundedRectMask2D>(menuCommand);
        }

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            independentRadii = serializedObject.FindProperty("independentRadii");
            radii = serializedObject.FindProperty("radii");
            padding = serializedObject.FindProperty("m_Padding");

            base.OnEnable();
        }

        /// <summary>
        /// Renders a custom inspector GUI that displays radius options and hides softness properties.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(independentRadii);

            if (independentRadii.boolValue)
            {
                showRadii = EditorGUILayout.Foldout(showRadii, radiiContent, true);

                if (showRadii)
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUI.indentLevel++;
                        Vector4 newRadii = radii.vector4Value;

                        newRadii.x = Mathf.Max(0.0f, EditorGUILayout.FloatField(topLeftContent, newRadii.x));
                        newRadii.y = Mathf.Max(0.0f, EditorGUILayout.FloatField(topRightContent, newRadii.y));
                        newRadii.z = Mathf.Max(0.0f, EditorGUILayout.FloatField(bottomLeftContent, newRadii.z));
                        newRadii.w = Mathf.Max(0.0f, EditorGUILayout.FloatField(bottomRightContent, newRadii.w));

                        if (check.changed)
                        {
                            radii.vector4Value = newRadii;
                        }

                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    float newRadius = radii.vector4Value.x;

                    newRadius = Mathf.Max(0.0f, EditorGUILayout.FloatField(radiusContent, newRadius));

                    if (check.changed)
                    {
                        Vector4 newRadii = radii.vector4Value;
                        newRadii.x = newRadius;
                        radii.vector4Value = newRadii;
                    }
                }
            }

            // Rather than call base.OnInspectorGUI() manually showing padding so that softness can be hidden since it is not supported by rounded rect masks.
            showOffsets = EditorGUILayout.Foldout(showOffsets, paddingContent, true);

            if (showOffsets)
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.indentLevel++;
                    Vector4 newPadding = padding.vector4Value;

                    newPadding.x = EditorGUILayout.FloatField(leftContent, newPadding.x);
                    newPadding.z = EditorGUILayout.FloatField(rightContent, newPadding.z);
                    newPadding.w = EditorGUILayout.FloatField(topContent, newPadding.w);
                    newPadding.y = EditorGUILayout.FloatField(bottomContent, newPadding.y);

                    if (check.changed)
                    {
                        padding.vector4Value = newPadding;
                    }
                    EditorGUI.indentLevel--;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// A custom inspector for MeshOutlineHierarchy.
    /// </summary>
    [CustomEditor(typeof(MeshOutlineHierarchy), true), CanEditMultipleObjects]
    public class MeshOutlineHierarchyInspector : BaseMeshOutlineInspector
    {
        private SerializedProperty exclusionMode;
        private SerializedProperty exclusionString;
        private SerializedProperty exclusionTag;

        protected override void OnEnable()
        {
            base.OnEnable();
            exclusionMode = serializedObject.FindProperty(nameof(exclusionMode));
            exclusionString = serializedObject.FindProperty(nameof(exclusionString));
            exclusionTag = serializedObject.FindProperty(nameof(exclusionTag));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(exclusionMode);

            var exclusionModeValue = (MeshOutlineHierarchy.ExclusionMode)exclusionMode.enumValueIndex;
            switch (exclusionModeValue)
            {
                case MeshOutlineHierarchy.ExclusionMode.None:
                default:
                    break;
                case MeshOutlineHierarchy.ExclusionMode.NameStartsWith:
                    exclusionString.stringValue = EditorGUILayout.TextField("Start String", exclusionString.stringValue);
                    break;
                case MeshOutlineHierarchy.ExclusionMode.NameContains:
                    exclusionString.stringValue = EditorGUILayout.TextField("Search String", exclusionString.stringValue);
                    break;
                case MeshOutlineHierarchy.ExclusionMode.Tag:
                    exclusionTag.stringValue = EditorGUILayout.TagField("Tag", exclusionTag.stringValue);
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

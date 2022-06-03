// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Custom inspector that displays properties when being previewed.
    /// </summary>
    [CustomEditor(typeof(CanvasMaterialAnimatorBase), true)]
    public class BaseCanvasMaterialAnimatorInspector : UnityEditor.Editor
    {
        private SerializedProperty materialPropertiesFoldedOut;
        private SerializedProperty instanceMaterials;

        private void OnEnable()
        {
            materialPropertiesFoldedOut = serializedObject.FindProperty("materialPropertiesFoldedOut");
            instanceMaterials = serializedObject.FindProperty("instanceMaterials");
        }

        /// <summary>
        /// Renders a custom inspector GUI. 
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(instanceMaterials);

            materialPropertiesFoldedOut.boolValue = EditorGUILayout.Foldout(materialPropertiesFoldedOut.boolValue, "Animated Material Properties");

            serializedObject.ApplyModifiedProperties();

            if (materialPropertiesFoldedOut.boolValue)
            {
                CanvasMaterialAnimatorBase animator = target as CanvasMaterialAnimatorBase;

                if (animator != null)
                {
                    // Allow the properties to be adjusted when in preview mode.
                    GUI.enabled = animator.PreviewMaterial != null;
                    DrawDefaultInspector();
                    GUI.enabled = true;
                }
            }
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Custom inspector that helps with magnifier configuration.
    /// </summary>
    [CustomEditor(typeof(MagnifierManager))]
    public class MagnifierManagerInspector : UnityEditor.Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty renderObjectsSettings;

       private bool showRenderObjects = false;

        private void OnEnable()
        {
            m_Script = serializedObject.FindProperty(nameof(m_Script));
            renderObjectsSettings = serializedObject.FindProperty(nameof(renderObjectsSettings));
        }

        /// <summary>
        /// Renders a custom inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                InspectorUtilities.DrawReadonlyPropertyField(m_Script);

                DrawPropertiesExcluding(serializedObject, nameof(m_Script), nameof(renderObjectsSettings));

                showRenderObjects = EditorGUILayout.Foldout(showRenderObjects, "Render Objects Settings");

                if (showRenderObjects)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(renderObjectsSettings);
                    EditorGUI.indentLevel--;
                }

                if (check.changed)
                {
                    MagnifierManager magnifier = target as MagnifierManager;

                    if (magnifier != null)
                    {
                        magnifier.ApplyMagnification();
                    }

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
#endif // GT_USE_URP
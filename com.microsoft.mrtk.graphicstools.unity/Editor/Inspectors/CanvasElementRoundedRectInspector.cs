// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_UGUI
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{

    /// <summary>
    /// Adds a shortcut to create a configured  game object and component from the game object context menu.
    /// </summary>
    [CustomEditor(typeof(CanvasElementRoundedRect))]
    public class CanvasElementRoundedRectInspector : UnityEditor.Editor
    {
        private int mainTexHash = Shader.PropertyToID("_MainTex");
        private int mainTexStHash = Shader.PropertyToID("_MainTex_ST");

        [MenuItem("GameObject/UI/Graphics Tools/Rounded Rect")]
        private static void CreateCanvasElement(MenuCommand menuCommand)
        {
            GameObject gameObject = InspectorUtilities.CreateGameObjectFromMenu<CanvasElementRoundedRect>(menuCommand, true);

            if (gameObject != null)
            {
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

                if (rectTransform != null)
                {
                    rectTransform.sizeDelta = new Vector2(32, 32);
                }
            }
        }

        /// <summary>
        /// Renders a custom inspector GUI.
        /// Don't display the texture and UVRect properties if the material doesn't have matching property.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Update the serialized object to ensure the property values are current
            serializedObject.Update();

            // Loop through all properties and draw them, except for ones we want to skip
            SerializedProperty property = serializedObject.GetIterator();
            property.NextVisible(true); // Move to the first property

            // get the material on this object
            CanvasElementRoundedRect roundCanvas = (CanvasElementRoundedRect)target;
            Material material = roundCanvas.materialForRendering;

            if (material == null)
            {
                EditorGUILayout.HelpBox("Material is not assigned.", MessageType.Warning);
                return;
            }

            // Iterate over all properties, skipping some if they aren't in the material
            while (property.NextVisible(false))
            {
                if (property.name == "m_Texture" && !material.HasProperty(mainTexHash))
                {
                    continue;
                }

                if (property.name == "m_UVRect" && !material.HasProperty(mainTexStHash))
                {
                    continue;
                }

                // Draw all other properties
                EditorGUILayout.PropertyField(property, true);
            }

            // Apply changes to the serialized property
            serializedObject.ApplyModifiedProperties();
        }
    }
}


#endif // GT_USE_UGUI
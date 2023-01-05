// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// General utility methods to help with inspector development.
    /// </summary>
    public static class InspectorUtilities
    {
        /// <summary>
        /// Creates a new game object of type T as a child of the menu command.
        /// </summary>
        public static GameObject CreateGameObjectFromMenu<T>(MenuCommand menuCommand) where T : MonoBehaviour
        {
            GameObject gameObject = new GameObject(typeof(T).Name, typeof(T));

            // Ensure the game object gets re-parented to the active context.
            GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);

            // Register the creation in the undo system.
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);

            Selection.activeObject = gameObject;

            return gameObject;
        }

        /// <summary>
        /// Draws a property that is greyed out and non-interactible.
        /// </summary>
        public static void DrawReadonlyPropertyField(SerializedProperty property, params GUILayoutOption[] options)
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(property, options);
            GUI.enabled = true;
        }
    }
}

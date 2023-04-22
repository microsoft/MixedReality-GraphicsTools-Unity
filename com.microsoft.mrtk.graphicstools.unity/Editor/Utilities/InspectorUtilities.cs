// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// General utility methods to help with inspector development.
    /// </summary>
    public static class InspectorUtilities
    {
        /// <summary>
        /// Line 19 - 109 uses an implementation from MRTK. 
        /// https://github.com/Zee2/MixedRealityToolkit-Unity/blob/mrtk3/com.microsoft.mrtk.uxcomponents/Editor/CreateElementMenus.cs#L39
        /// It automatically adds a configured canvas when a Graphics Tools
        /// UI GameObject is added to the scene.
        /// </summary>
         
        // Reflection into internal UGUI editor utilities.
        private static System.Reflection.MethodInfo PlaceUIElementRoot = null;

        /// <summary>
        /// Add canvas for a UI GameObject if none exists.
        /// </summary>
        private static GameObject SetupCanvas(GameObject gameObject, MenuCommand menuCommand)
        {
            // This is evil :)
            // UGUI contains plenty of helper utilities for spawning and managing new Canvas objects
            // at edit-time. Unfortunately, they're all internal, so we have to use reflection to
            // access them.
            if (PlaceUIElementRoot == null)
            {
                // We're using SelectableEditor type here to grab the assembly instead of going
                // and hunting down the assembly ourselves. It's a bit more convenient and durable.
                PlaceUIElementRoot = typeof(SelectableEditor).Assembly.GetType("UnityEditor.UI.MenuOptions")?.GetMethod(
                                                "PlaceUIElementRoot",
                                                System.Reflection.BindingFlags.NonPublic |
                                                System.Reflection.BindingFlags.Static );
                if (PlaceUIElementRoot == null)
                {
                    Debug.LogError("Whoops! Looks like Unity changed the internals of their UGUI editor utilities. Please file a bug!");
                    // Return early; we can't do anything else.
                    return gameObject;
                }
            }

            PlaceUIElementRoot.Invoke(null, new object[] { gameObject, menuCommand});

            // The above call will create a new Canvas for us (if we don't have one),
            // but it won't have optimal settings for MRTK UX. Let's fix that!
            Canvas canvas = gameObject.GetComponentInParent<Canvas>();
            RectTransform rt = canvas.GetComponent<RectTransform>();

            // If the canvas's only child is us; let's make sure the Canvas has reasonable starting defaults.
            // Otherwise, it was probably an existing canvas we were added to, so we shouldn't mess with it.
            if (rt.childCount == 1 && rt.GetChild(0) == gameObject.transform)
            {
                SetReasonableCanvasDefaults(canvas);
                
                // Reset our own object to zero-position relative to the parent canvas.
                gameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }

            return gameObject;
        }

        /// <summary>
        /// Configure some default properties for the canvas GameObject.
        /// Size, position from camera
        /// </summary>
        private static void SetReasonableCanvasDefaults(Canvas canvas)
        {
            RectTransform rt = canvas.GetComponent<RectTransform>();

            // 1mm : 1 unit measurement ratio.
            if (rt.lossyScale != Vector3.one * 0.001f)
            {
                rt.localScale = Vector3.one * 0.001f;
            }
            // 150mm x 150mm.
            rt.sizeDelta = Vector2.one * 150.0f;

            // All our canvases will be worldspace (by default.)
            canvas.renderMode = RenderMode.WorldSpace;
            Undo.RecordObject(canvas, "Set Canvas RenderMode to WorldSpace");

            // 30cm in front of the camera.
            rt.position = Camera.main.transform.position + Camera.main.transform.forward * 0.3f;
            Undo.RecordObject(rt, "Set Canvas Position");

            // No GraphicRaycaster by default. Users can add one, if they like.
            if (canvas.TryGetComponent(out GraphicRaycaster raycaster))
            {
                Undo.DestroyObjectImmediate(raycaster);
            }

            // CanvasScaler should be there by default.
            if (!canvas.TryGetComponent(out CanvasScaler _))
            {
                Undo.AddComponent<CanvasScaler>(canvas.gameObject);
            }
        }

        /// <summary>
        /// Creates a new game object of type T as a child of the menu command.
        /// </summary>
        public static GameObject CreateGameObjectFromMenu<T>(MenuCommand menuCommand, bool hasCanvasParent = false) where T : MonoBehaviour
        {
            GameObject gameObject = new GameObject(typeof(T).Name, typeof(T));

            // Ensure the game object gets re-parented to the active context.
            GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);

            // Register the creation in the undo system.
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);

            Selection.activeObject = gameObject;

            if (hasCanvasParent)
            {
                return SetupCanvas(gameObject, menuCommand);
            }

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

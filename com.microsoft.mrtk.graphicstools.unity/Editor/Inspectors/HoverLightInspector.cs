// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Improves object selection and adds a shortcut to create a configured game object and component from the game object context menu.
    /// </summary>
    [CustomEditor(typeof(HoverLight))]
    public class HoverLightInspector : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            HoverLight light = target as HoverLight;

            if (light == null)
            {
                return;
            }

            if (light.enabled)
            {
                Handles.color = light.Color;
            }
            else
            {
                Handles.color = Color.gray;
            }

            float radius = Handles.RadiusHandle(Quaternion.identity, light.transform.position, light.Radius);
            if (GUI.changed)
            {
                Undo.RecordObject(light, "Adjust Hover Light");
                light.Radius = radius;
            }
        }

        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var light = target as HoverLight;
            Debug.Assert(light != null);
            return new Bounds(light.transform.position, Vector3.one * light.Radius);
        }

       [MenuItem("GameObject/Light/Graphics Tools/Hover Light")]
        private static void CreateHoverLight(MenuCommand menuCommand)
        {
            InspectorUtilities.CreateGameObjectFromMenu<HoverLight>(menuCommand);
        }
    }
}

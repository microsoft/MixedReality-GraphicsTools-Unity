// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Improves object selection and adds a shortcut to create a configured game object and component from the game object context menu.
    /// </summary>
    [CustomEditor(typeof(ProximityLight))]
    public class ProximityLightInspector : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            ProximityLight light = target as ProximityLight;

            if (light == null)
            {
                return;
            }

            if (light.enabled)
            {
                Handles.color = new Color(light.Settings.CenterColor.r, light.Settings.CenterColor.g, light.Settings.CenterColor.b);
            }
            else
            {
                Handles.color = Color.gray;
            }

            float nearRadius = Handles.RadiusHandle(Quaternion.identity, light.transform.position, light.Settings.NearRadius);
            if (GUI.changed)
            {
                Undo.RecordObject(light, "Adjust Proximity Light Near Radius");
                light.Settings.NearRadius = nearRadius;
            }

            if (light.enabled)
            {
                Handles.color = new Color(light.Settings.OuterColor.r, light.Settings.OuterColor.g, light.Settings.OuterColor.b);
            }
            else
            {
                Handles.color = Color.gray;
            }

            float farRadius = Handles.RadiusHandle(Quaternion.identity, light.transform.position, light.Settings.FarRadius);
            if (GUI.changed)
            {
                Undo.RecordObject(light, "Adjust Proximity Light Far Radius");
                light.Settings.NearRadius = farRadius;
            }

            switch (light.Settings.HandednessType)
            {
                case ProximityLight.LightSettings.Handedness.Left:
                    {
                        Handles.Label(light.transform.position,"Left");
                    }
                    break;
                case ProximityLight.LightSettings.Handedness.Right:
                    {
                        Handles.Label(light.transform.position, "Right");
                    }
                    break;
            }
        }

        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var light = target as ProximityLight;
            Debug.Assert(light != null);
            return new Bounds(light.transform.position, Vector3.one * Mathf.Max(light.Settings.NearRadius, light.Settings.FarRadius));
        }

       [MenuItem("GameObject/Light/Graphics Tools/Proximity Light")]
        private static void CreateProximityLight(MenuCommand menuCommand)
        {
            InspectorUtilities.CreateGameObjectFromMenu<ProximityLight>(menuCommand);
        }
    }
}

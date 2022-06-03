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
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var light = target as ProximityLight;
            Debug.Assert(light != null);
            return new Bounds(light.transform.position, Vector3.one * light.Settings.FarRadius);
        }

        [MenuItem("GameObject/Light/Proximity Light")]
        private static void CreateProximityLight(MenuCommand menuCommand)
        {
            InspectorUtilities.CreateGameObjectFromMenu<ProximityLight>(menuCommand);
        }
    }
}

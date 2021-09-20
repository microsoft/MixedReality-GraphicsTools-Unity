// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    [CustomEditor(typeof(HoverLight))]
    public class HoverLightInspector : UnityEditor.Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var light = target as HoverLight;
            Debug.Assert(light != null);
            return new Bounds(light.transform.position, Vector3.one * light.Radius);
        }

        [MenuItem("GameObject/Light/Hover Light")]
        private static void CreateHoverLight(MenuCommand menuCommand)
        {
            InspectorUtilities.CreateGameObjectFromMenu<HoverLight>(menuCommand);
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Improves object selection and adds a shortcut to create a configured game object and component from the game object context menu.
    /// </summary>
    [CustomEditor(typeof(DistantLight))]
    public class DistantLightInspector : UnityEditor.Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var light = target as DistantLight;
            Debug.Assert(light != null);
            return new Bounds(light.transform.position, Vector3.one);
        }

        [MenuItem("GameObject/Light/Distant Light")]
        private static void CreateDistantLight(MenuCommand menuCommand)
        {
            GameObject gameObject = InspectorUtilities.CreateGameObjectFromMenu<DistantLight>(menuCommand);

            if (gameObject != null)
            {
                gameObject.transform.position = new Vector3(0.0f, 3.0f, 0.0f);
                Quaternion rotation = new Quaternion();
                rotation.eulerAngles = new Vector3(50.0f, -30.0f, 0.0f);
                gameObject.transform.rotation = rotation;
            }
        }
    }
}

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
        static readonly Vector3[] directionalLightHandlesRayPositions = new Vector3[]
        {
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, -1, 0),
            new Vector3(1, 1, 0).normalized,
            new Vector3(1, -1, 0).normalized,
            new Vector3(-1, 1, 0).normalized,
            new Vector3(-1, -1, 0).normalized
        };

        private void OnSceneGUI()
        {
            DistantLight light = target as DistantLight;

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

            Vector3 position = light.transform.position;
            float size;

            using (new Handles.DrawingScope(Matrix4x4.identity))
            {
                size = HandleUtility.GetHandleSize(position);
            }

            float radius = size * 0.2f;

            using (new Handles.DrawingScope(Matrix4x4.TRS(position, light.transform.rotation, Vector3.one)))
            {
                Handles.DrawWireDisc(Vector3.zero, Vector3.forward, radius);
                foreach (Vector3 normalizedPos in directionalLightHandlesRayPositions)
                {
                    Vector3 pos = normalizedPos * radius;
                    Handles.DrawLine(pos, pos + new Vector3(0, 0, size));
                }
            }
        }

        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var light = target as DistantLight;
            Debug.Assert(light != null);
            return new Bounds(light.transform.position, Vector3.one);
        }

       [MenuItem("GameObject/Light/Graphics Tools/Distant Light")]
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

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    [CustomEditor(typeof(CanvasMesh))]
    public class CanvasMeshtInspector : UnityEditor.Editor
    {
        [MenuItem("GameObject/UI/Canvas Mesh - Graphics Tools")]
        private static void CreateCanvasMesh(MenuCommand menuCommand)
        {
            GameObject canvasMesh = new GameObject("Canvas Mesh", typeof(CanvasMesh));

            // Ensure the object gets re-parented to the active context.
            GameObjectUtility.SetParentAndAlign(canvasMesh, menuCommand.context as GameObject);

            // Register the creation in the undo system.
            Undo.RegisterCreatedObjectUndo(canvasMesh, "Create " + canvasMesh.name);

            Selection.activeObject = canvasMesh;
        }
    }
}

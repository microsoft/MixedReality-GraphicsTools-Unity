// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    [CustomEditor(typeof(CanvasElementMesh))]
    public class CanvasElementMeshInspector : UnityEditor.Editor
    {
        [MenuItem("GameObject/UI/Mesh - Graphics Tools")]
        private static void CreateCanvasElementMesh(MenuCommand menuCommand)
        {
            InspectorUtilities.CreateGameObjectFromMenu<CanvasElementMesh>(menuCommand);
        }
    }
}

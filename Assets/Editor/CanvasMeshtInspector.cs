// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    [CustomEditor(typeof(CanvasMesh))]
    public class CanvasMeshInspector : UnityEditor.Editor
    {
        [MenuItem("GameObject/UI/Canvas Mesh - Graphics Tools")]
        private static void CreateCanvasMesh(MenuCommand menuCommand)
        {
            InspectorUtilities.CreateGameObjectFromMenu<CanvasMesh>(menuCommand);
        }
    }
}

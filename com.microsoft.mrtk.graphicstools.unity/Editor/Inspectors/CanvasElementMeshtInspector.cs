// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

#if GT_USE_UGUI
using UnityEditor;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Adds a shortcut to create a configured  game object and component from the game object context menu.
    /// </summary>
    [CustomEditor(typeof(CanvasElementMesh))]
    public class CanvasElementMeshInspector : UnityEditor.Editor
    {
        [MenuItem("GameObject/UI/Graphics Tools/Mesh")]
        private static void CreateCanvasElement(MenuCommand menuCommand)
        {
            InspectorUtilities.CreateGameObjectFromMenu<CanvasElementMesh>(menuCommand, true);
        }
    }
}
#endif

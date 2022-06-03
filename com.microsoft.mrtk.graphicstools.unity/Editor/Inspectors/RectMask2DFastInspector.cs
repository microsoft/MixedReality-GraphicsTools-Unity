// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEditor.UI;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Adds a shortcut to create a configured game object and component from the game object context menu.
    /// </summary>
    [CustomEditor(typeof(RectMask2DFast), true)]
    [CanEditMultipleObjects]
    public class RectMask2DFastInspector : RectMask2DEditor
    {
        [MenuItem("GameObject/UI/Rect Mask Fast - Graphics Tools")]
        private static void CreateCanvasElement(MenuCommand menuCommand)
        {
            InspectorUtilities.CreateGameObjectFromMenu<RectMask2DFast>(menuCommand);
        }
    }
}

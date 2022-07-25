// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Adds a shortcut to create a configured  game object and component from the game object context menu.
    /// </summary>
    [CustomEditor(typeof(CanvasElementBeveledRect))]
    public class CanvasElementBeveledRectInspector : UnityEditor.Editor
    {
        [MenuItem("GameObject/UI/Graphics Tools/Beveled Rect")]
        private static void CreateCanvasElement(MenuCommand menuCommand)
        {
            GameObject gameObject = InspectorUtilities.CreateGameObjectFromMenu<CanvasElementBeveledRect>(menuCommand);

            if (gameObject != null)
            {
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

                if (rectTransform != null)
                {
                    rectTransform.sizeDelta = new Vector2(32, 32);
                }
            }
        }
    }
}

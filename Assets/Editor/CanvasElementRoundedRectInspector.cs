// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    [CustomEditor(typeof(CanvasElementRoundedRect))]
    public class CanvasElementRoundedRectInspector : UnityEditor.Editor
    {
        [MenuItem("GameObject/UI/Rounded Rect - Graphics Tools")]
        private static void CreateCanvasElementRoundedRect(MenuCommand menuCommand)
        {
            GameObject gameObject = InspectorUtilities.CreateGameObjectFromMenu<CanvasElementRoundedRect>(menuCommand);

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

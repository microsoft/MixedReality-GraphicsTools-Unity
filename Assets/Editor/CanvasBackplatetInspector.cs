// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    [CustomEditor(typeof(CanvasBackplate))]
    public class CanvasBackplateInspector : UnityEditor.Editor
    {
        [MenuItem("GameObject/UI/Canvas Backplate - Graphics Tools")]
        private static void CreateCanvasBackplate(MenuCommand menuCommand)
        {
            GameObject gameObject = InspectorUtilities.CreateGameObjectFromMenu<CanvasBackplate>(menuCommand);

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

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// General utility methods to help with Unity UI development and usage.
    /// </summary>
    public static class CanvasUtilities
    {
        /// <summary>
        ///  Menu item which lets you move back and forth between expressing layout in the pinning system and 
        ///  the anchor (constraint) system inherent in RectTransform. This can be useful when moving between 
        ///  responsive and fixed scale layouts.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Canvas Anchors/Anchors to Corners %[")]
        private static void AnchorsToCorners()
        {
            RectTransform parent = Selection.activeTransform.parent as RectTransform;

            if (parent == null)
            {
                return;
            }

            foreach (Transform transform in Selection.transforms)
            {
                RectTransform rect = transform as RectTransform;

                if (rect == null)
                {
                    continue;
                }

                Undo.RecordObject(rect, "Transform Change");

                rect.anchorMin = new Vector2(rect.anchorMin.x + rect.offsetMin.x / parent.rect.width,
                                             rect.anchorMin.y + rect.offsetMin.y / parent.rect.height);
                rect.anchorMax = new Vector2(rect.anchorMax.x + rect.offsetMax.x / parent.rect.width,
                                             rect.anchorMax.y + rect.offsetMax.y / parent.rect.height);

                rect.offsetMin = rect.offsetMax = Vector2.zero;
            }
        }

        /// <summary>
        ///  Menu item which lets you move back and forth between expressing layout in the pinning system and 
        ///  the anchor (constraint) system inherent in RectTransform. This can be useful when moving between 
        ///  responsive and fixed scale layouts.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Canvas Anchors/Corners to Anchors %]")]
        private static void CornersToAnchors()
        {
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform rect = transform as RectTransform;

                if (rect == null)
                {
                    continue;
                }

                Undo.RecordObject(rect, "Transform Change");

                rect.offsetMin = rect.offsetMax = Vector2.zero;
            }
        }
    }
}

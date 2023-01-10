// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// A custom editor for the ClippingBox to allow for specification of the framing bounds.
    /// </summary>
    [CustomEditor(typeof(ClippingBox))]
    [CanEditMultipleObjects]
    public class ClippingBoxEditor : ClippingPrimitiveEditor
    {
        /// <inheritdoc/>
        protected override bool HasFrameBounds()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override Bounds OnGetFrameBounds()
        {
            var primitive = target as ClippingBox;
            Debug.Assert(primitive != null);
            return new Bounds(primitive.transform.position, primitive.transform.lossyScale);
        }

        [MenuItem("GameObject/Effects/Graphics Tools/Clipping Box")]
        private static void CreateClippingBox(MenuCommand menuCommand)
        {
            InspectorUtilities.CreateGameObjectFromMenu<ClippingBox>(menuCommand);
        }
    }
}

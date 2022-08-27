// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Custom inspector that exposes additional user controls for the AmbientOcclsion component
    /// </summary>
    [CustomEditor(typeof(AmbientOcclusion)), CanEditMultipleObjects]
    public class AmbientOcclusionInspector : UnityEditor.Editor
    {
        /// <summary>
        /// Renders a custom inspector
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Gather samples"))
            {
                var component = target as AmbientOcclusion;
                component.GatherSamples();
            }
            DrawDefaultInspector();
        }
    }
}

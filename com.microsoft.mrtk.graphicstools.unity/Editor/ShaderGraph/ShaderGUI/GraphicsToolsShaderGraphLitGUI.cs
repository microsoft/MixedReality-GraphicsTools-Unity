// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// TODO
    /// </summary>
    class GraphicsToolsShaderGraphLitGUI : ShaderGraphLitGUI
    {
        protected MaterialProperty SrcBlendAlpha;
        protected MaterialProperty DstBlendAlpha;

        public static GUIContent SrcBlendAlphaLabel = new GUIContent("Source Blend Alpha", "Blend Mode of Newly Calculated Alpha Channel (A)");
        public static GUIContent DstBlendAlphaLabel = new GUIContent("Destination Blend Alpha", "Blend Mode of Existing Alpha Channel (A)");

        /// <summary>
        /// TODO
        /// </summary>
        public override void FindProperties(MaterialProperty[] properties)
        {
            var material = materialEditor?.target as Material;

            if (material == null)
            {
                return;
            }

            base.FindProperties(properties);

            SrcBlendAlpha = FindProperty(GraphicsToolsCoreRenderStates.Property.SrcBlendAlpha, properties, false);
            DstBlendAlpha = FindProperty(GraphicsToolsCoreRenderStates.Property.DstBlendAlpha, properties, false);
        }

        /// <summary>
        /// TODO
        /// </summary>
        public override void DrawSurfaceOptions(Material material)
        {
            base.DrawSurfaceOptions(material);

            var names = Enum.GetNames(typeof(UnityEngine.Rendering.BlendMode));
            DoPopup(SrcBlendAlphaLabel, SrcBlendAlpha, names);
            DoPopup(DstBlendAlphaLabel, DstBlendAlpha, names);
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Material GUI extension to the GraphicsToolsUniversalUnlitSubTarget.
    /// </summary>
    class GraphicsToolsShaderGraphUnlitGUI : ShaderGraphUnlitGUI
    {
        protected MaterialProperty SrcBlendAlpha;
        protected MaterialProperty DstBlendAlpha;

        public static GUIContent SrcBlendAlphaLabel = new GUIContent("Source Blend Alpha", "Blend Mode of Newly Calculated Alpha Channel (A)");
        public static GUIContent DstBlendAlphaLabel = new GUIContent("Destination Blend Alpha", "Blend Mode of Existing Alpha Channel (A)");

        /// <summary>
        /// Extracts the alpha blending properties.
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
        /// Displays the alpha blending properties.
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
#endif // GT_USE_URP

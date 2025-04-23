// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Material GUI extension to the GraphicsToolsUniversalLitSubTarget.
    /// </summary>
    class GraphicsToolsShaderGraphLitGUI : ShaderGraphLitGUI
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

        /// <summary>
        /// Ensure that the alpha blending properties are maintained.
        /// </summary>
        public override void ValidateMaterial(Material material)
        {
            // We must cache the values of SrcBlendAlpha and DstBlendAlpha because base.ValidateMaterial() will overwrite them with it's own default values.
            float prevSrcBlendAlpha;

            if (SrcBlendAlpha != null)
            {
                prevSrcBlendAlpha = SrcBlendAlpha.floatValue;
            }
            else if (material.HasProperty(GraphicsToolsCoreRenderStates.Property.SrcBlendAlpha))
            {
                prevSrcBlendAlpha = material.GetFloat(GraphicsToolsCoreRenderStates.Property.SrcBlendAlpha);
            }
            else
            {
                prevSrcBlendAlpha = 1.0f;
            }

            float prevDstBlendAlpha;

            if (DstBlendAlpha != null)
            {
                prevDstBlendAlpha = DstBlendAlpha.floatValue;
            }
            else if (material.HasProperty(GraphicsToolsCoreRenderStates.Property.DstBlendAlpha))
            {
                prevDstBlendAlpha = material.GetFloat(GraphicsToolsCoreRenderStates.Property.DstBlendAlpha);
            }
            else
            {
                prevDstBlendAlpha = 1.0f;
            }

            base.ValidateMaterial(material);

            material.SetFloat(GraphicsToolsCoreRenderStates.Property.SrcBlendAlpha, prevSrcBlendAlpha);
            material.SetFloat(GraphicsToolsCoreRenderStates.Property.DstBlendAlpha, prevDstBlendAlpha);
        }
    }
}

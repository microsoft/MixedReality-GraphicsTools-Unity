// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// A custom shader inspector for the "Graphics Tools/Wireframe" shader.
    /// </summary>
    public class WireframeShaderGUI : BaseShaderGUI
    {

        /// <summary>
        /// Common names, keywords, and tooltips.
        /// </summary>
        protected static class Styles
        {
            public static string mainPropertiesTitle = "Main Properties";
            public static string advancedOptionsTitle = "Advanced Options";

            public static GUIContent baseColor = new GUIContent("Base Color", "Color of faces");
            public static GUIContent wireColor = new GUIContent("Wire Color", "Color of wires");
            public static GUIContent wireThickness = new GUIContent("Wire Thickness", "Thickness of wires");
        }

        protected MaterialProperty baseColor;
        protected MaterialProperty wireColor;
        protected MaterialProperty wireThickness;

        /// <summary>
        /// Displays inspector options for wireframe rendering.
        /// </summary>
        /// <param name="materialEditor">Current material editor in use.</param>
        /// <param name="props">Material properties to search.</param>
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            Material material = (Material)materialEditor.target;

            base.OnGUI(materialEditor, props);

            GUILayout.Label(Styles.mainPropertiesTitle, EditorStyles.boldLabel);
            materialEditor.ShaderProperty(baseColor, Styles.baseColor);
            materialEditor.ShaderProperty(wireColor, Styles.wireColor);
            materialEditor.ShaderProperty(wireThickness, Styles.wireThickness);

            AdvancedOptions(materialEditor, material);
        }

        /// <inheritdoc/>
        protected override void FindProperties(MaterialProperty[] props)
        {
            base.FindProperties(props);

            baseColor = FindProperty("_BaseColor", props);
            wireColor = FindProperty("_WireColor", props);
            wireThickness = FindProperty("_WireThickness", props);
        }

        /// <summary>
        /// Attempts to copy material properties from other shaders to the Graphics Tools/Wireframe shader.
        /// </summary>
        /// <param name="material">Current material.</param>
        /// <param name="oldShader">Previous shader.</param>
        /// <param name="newShader">Current shader.</param>
        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            float? cullMode = GetFloatProperty(material, "_Cull");

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            SetShaderFeatureActive(material, null, BaseStyles.cullModeName, cullMode);

            // Setup the rendering mode based on the old shader.
            if (oldShader == null || !oldShader.name.Contains(LegacyShadersPath))
            {
                SetupMaterialWithRenderingMode(material, (RenderingMode)material.GetFloat(BaseStyles.renderingModeName), RenderingMode.Opaque, -1);
            }
            else
            {
                RenderingMode mode = RenderingMode.Opaque;

                if (oldShader.name.Contains(TransparentCutoutShadersPath))
                {
                    mode = RenderingMode.Cutout;
                }
                else if (oldShader.name.Contains(TransparentShadersPath))
                {
                    mode = RenderingMode.Fade;
                }

                material.SetFloat(BaseStyles.renderingModeName, (float)mode);

                MaterialChanged(material);
            }
        }

        /// <summary>
        /// Displays inspectors settings for features users don't need to alter often.
        /// </summary>
        /// <param name="materialEditor">Current material editor in use.</param>
        /// <param name="material">Current material in use.</param>
        protected void AdvancedOptions(MaterialEditor materialEditor, Material material)
        {
            GUILayout.Label(Styles.advancedOptionsTitle, EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            materialEditor.ShaderProperty(renderQueueOverride, BaseStyles.renderQueueOverride);

            if (EditorGUI.EndChangeCheck())
            {
                MaterialChanged(material);
            }

            // Show the RenderQueueField but do not allow users to directly manipulate it. That is done via the renderQueueOverride.
            GUI.enabled = false;
            materialEditor.RenderQueueField();
            GUI.enabled = true;

            materialEditor.EnableInstancingField();
        }
    }
}

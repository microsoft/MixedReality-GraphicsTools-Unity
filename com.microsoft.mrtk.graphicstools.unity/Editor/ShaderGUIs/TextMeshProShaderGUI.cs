// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// A custom TMP_SDFShaderGUI inspector for the "Graphics Tools/TextMeshPro" shader.
    /// Adds the ability warning about depth write when depth buffer sharing is enabled.
    /// </summary>
    public class TextMeshProShaderGUI : TMP_SDFShaderGUI
    {
        private static bool doMode = true;

        /// <inheritdoc/>
        protected override void DoGUI()
        {
            doMode = BeginPanel("Mode", doMode);
            if (doMode)
            {
                DoModePanel();
            }
            EndPanel();

            base.DoGUI();
        }

        /// <inheritdoc/>
        protected void DoModePanel()
        {
            EditorGUI.indentLevel += 1;

            // Disabled to show state to the user, but not allow them to modify it.
            GUI.enabled = false;
            {
                var sourceBlend = FindProperty(BaseShaderGUI.BaseStyles.sourceBlendName, m_Properties, false);

                if (sourceBlend != null)
                {
                    sourceBlend.floatValue = EditorGUILayout.Popup(sourceBlend.displayName, (int)sourceBlend.floatValue, BaseShaderGUI.BaseStyles.blendModeNames);
                }

                var destinationBlend = FindProperty(BaseShaderGUI.BaseStyles.destinationBlendName, m_Properties, false);

                if (destinationBlend != null)
                {
                    destinationBlend.floatValue = EditorGUILayout.Popup(destinationBlend.displayName, (int)destinationBlend.floatValue, BaseShaderGUI.BaseStyles.blendModeNames);
                }

                var sourceBlendAlpha = FindProperty(BaseShaderGUI.BaseStyles.sourceBlendAlphaName, m_Properties, false);

                if (sourceBlendAlpha != null)
                {
                    sourceBlendAlpha.floatValue = EditorGUILayout.Popup(sourceBlendAlpha.displayName, (int)sourceBlendAlpha.floatValue, BaseShaderGUI.BaseStyles.blendModeNames);
                }

                var destinationBlendAlpha = FindProperty(BaseShaderGUI.BaseStyles.destinationBlendAlphaName, m_Properties, false);

                if (destinationBlendAlpha != null)
                {
                    destinationBlendAlpha.floatValue = EditorGUILayout.Popup(destinationBlendAlpha.displayName, (int)destinationBlendAlpha.floatValue, BaseShaderGUI.BaseStyles.blendModeNames);
                }
            }
            GUI.enabled = true;

            var depthTest = FindProperty(BaseShaderGUI.BaseStyles.depthTestName, m_Properties, false);

            if (depthTest != null)
            {
                depthTest.floatValue = EditorGUILayout.Popup(depthTest.displayName, (int)depthTest.floatValue, BaseShaderGUI.BaseStyles.depthTestNames);
            }

            var depthWrite = FindProperty(BaseShaderGUI.BaseStyles.depthWriteName, m_Properties, false);

            if (depthWrite != null)
            {
                depthWrite.floatValue = EditorGUILayout.Popup(depthWrite.displayName, (int)depthWrite.floatValue, BaseShaderGUI.BaseStyles.depthWriteNames);

                if (depthWrite.floatValue.Equals(0.0f))
                {
                    if (ShaderGUIUtilities.DisplayDepthWriteWarning(m_Editor))
                    {
                        depthWrite.floatValue = 1.0f;
                    }
                }
            }

            EditorGUI.indentLevel -= 1;
            EditorGUILayout.Space();
        }
    }
}

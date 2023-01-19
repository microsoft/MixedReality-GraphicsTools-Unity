// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Groups render state into a handful of common surface configurations.
    /// </summary>
    public enum RenderingMode
    {
        Opaque = 0,
        Cutout = 1,
        Fade = 2,
        Transparent = 3,
        Additive = 4,
        Custom = 5
    }

    /// <summary>
    /// Toggle for depth writing.
    /// </summary>
    public enum DepthWrite
    {
        Off = 0,
        On = 1
    }

    /// <summary>
    /// Fork of https://docs.unity3d.com/ScriptReference/Rendering.ColorWriteMask.html which included a none option.
    /// </summary>
    [Flags]
    public enum ColorWriteMask
    {
        None = 0,
        Alpha = 1,
        Blue = 2,
        Green = 4,
        Red = 8,
        All = 15
    }

    /// <summary>
    /// A custom base shader inspector for Graphics Tools shaders.
    /// </summary>
    public abstract class BaseShaderGUI : ShaderGUI
    {
        /// <summary>
        /// Common names, keywords, and tooltips.
        /// </summary>
        public static class BaseStyles
        {
            public static string renderingOptionsTitle = "Rendering Options";
            public static string advancedOptionsTitle = "Advanced Options";
            public static string renderTypeName = "RenderType";
            public static string renderingModeName = "_Mode";
            public static string customRenderingModeName = "_CustomMode";
            public static string sourceBlendName = "_SrcBlend";
            public static string destinationBlendName = "_DstBlend";
            public static string sourceBlendAlphaName = "_SrcBlendAlpha";
            public static string destinationBlendAlphaName = "_DstBlendAlpha";
            public static string blendOperationName = "_BlendOp";
            public static string depthTestName = "_ZTest";
            public static string depthWriteName = "_ZWrite";
            public static string depthOffsetFactorName = "_ZOffsetFactor";
            public static string depthOffsetUnitsName = "_ZOffsetUnits";
            public static string colorWriteMaskName = "_ColorWriteMask";
            public static string colorWriteMaskUIName = "_ColorMask";

            public static string cullModeName = "_CullMode";
            public static string renderQueueOverrideName = "_RenderQueueOverride";

            public static string alphaTestOnName = "_ALPHATEST_ON";
            public static string alphaBlendFadeOnName = "_ALPHABLEND_ON";
            public static string alphaBlendTransOnName = "_ALPHABLEND_TRANS_ON";
            public static string additiveOnName = "_ADDITIVE_ON";

            public static readonly string[] renderingModeNames = Enum.GetNames(typeof(RenderingMode));
            public static readonly string[] blendModeNames = Enum.GetNames(typeof(BlendMode));
            public static readonly string[] depthTestNames = Enum.GetNames(typeof(CompareFunction));
            public static readonly string[] depthWriteNames = Enum.GetNames(typeof(DepthWrite));
            public static GUIContent sourceBlend = new GUIContent("Source Blend", "Blend Mode of Newly Calculated Color (RGB)");
            public static GUIContent destinationBlend = new GUIContent("Destination Blend", "Blend Mode of Existing Color (RGB)");
            public static GUIContent sourceBlendAlpha = new GUIContent("Source Blend Alpha", "Blend Mode of Newly Calculated Alpha Channel (A)");
            public static GUIContent destinationBlendAlpha = new GUIContent("Destination Blend Alpha", "Blend Mode of Existing Alpha Channel (A)");
            public static GUIContent blendOperation = new GUIContent("Blend Operation", "Operation for Blending New Color With Existing Color");
            public static GUIContent depthTest = new GUIContent("Depth Test", "How Should Depth Testing Be Performed.");
            public static GUIContent depthWrite = new GUIContent("Depth Write", "Controls Whether Pixels From This Material Are Written to the Depth Buffer");
            public static GUIContent depthOffsetFactor = new GUIContent("Depth Offset Factor", "Scales the Maximum Z Slope, with Respect to X or Y of the Polygon");
            public static GUIContent depthOffsetUnits = new GUIContent("Depth Offset Units", "Scales the Minimum Resolvable Depth Buffer Value");
            public static GUIContent colorWriteMask = new GUIContent("Color Write Mask", "Color Channel Writing Mask");
            public static GUIContent cullMode = new GUIContent("Cull Mode", "Triangle Culling Mode");
            public static GUIContent renderQueueOverride = new GUIContent("Render Queue Override", "Manually Override the Render Queue");
        }

        protected bool initialized;

        protected MaterialProperty renderingMode;
        protected MaterialProperty customRenderingMode;
        protected MaterialProperty sourceBlend;
        protected MaterialProperty destinationBlend;
        protected MaterialProperty sourceBlendAlpha;
        protected MaterialProperty destinationBlendAlpha;
        protected MaterialProperty blendOperation;
        protected MaterialProperty depthTest;
        protected MaterialProperty depthWrite;
        protected MaterialProperty depthOffsetFactor;
        protected MaterialProperty depthOffsetUnits;
        protected MaterialProperty colorWriteMask;
        protected MaterialProperty cullMode;
        protected MaterialProperty renderQueueOverride;

        protected const string LegacyShadersPath = "Legacy Shaders/";
        protected const string TransparentShadersPath = "/Transparent/";
        protected const string TransparentCutoutShadersPath = "/Transparent/Cutout/";

        /// <summary>
        /// Finds properties and renders the default rendering mode options.
        /// </summary>
        /// <param name="materialEditor">Current material editor in use.</param>
        /// <param name="props">Material properties to search.</param>
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            Material material = (Material)materialEditor.target;

            FindProperties(props);
            Initialize(material);

            RenderingModeOptions(materialEditor);
        }

        /// <summary>
        /// Looks for common properties associated with render mode.
        /// </summary>
        /// <param name="props">Material properties to search.</param>
        protected virtual void FindProperties(MaterialProperty[] props)
        {
            renderingMode = FindProperty(BaseStyles.renderingModeName, props);
            customRenderingMode = FindProperty(BaseStyles.customRenderingModeName, props);
            sourceBlend = FindProperty(BaseStyles.sourceBlendName, props);
            destinationBlend = FindProperty(BaseStyles.destinationBlendName, props);
            sourceBlendAlpha = FindProperty(BaseStyles.sourceBlendAlphaName, props);
            destinationBlendAlpha = FindProperty(BaseStyles.destinationBlendAlphaName, props);
            blendOperation = FindProperty(BaseStyles.blendOperationName, props);
            depthTest = FindProperty(BaseStyles.depthTestName, props, false);
            depthWrite = FindProperty(BaseStyles.depthWriteName, props);
            depthOffsetFactor = FindProperty(BaseStyles.depthOffsetFactorName, props);
            depthOffsetUnits = FindProperty(BaseStyles.depthOffsetUnitsName, props);
            colorWriteMask = FindProperty(BaseStyles.colorWriteMaskName, props, false);
            colorWriteMask = colorWriteMask == null ? FindProperty(BaseStyles.colorWriteMaskUIName, props) : colorWriteMask;

            cullMode = FindProperty(BaseStyles.cullModeName, props);
            renderQueueOverride = FindProperty(BaseStyles.renderQueueOverrideName, props);
        }

        /// <summary>
        /// Sets up the rendering mode once.
        /// </summary>
        /// <param name="material">The material to apply render mode settings to.</param>
        protected void Initialize(Material material)
        {
            if (!initialized)
            {
                MaterialChanged(material);
                initialized = true;
            }
        }

        /// <summary>
        /// Ensures the new material has the right render mode settings.
        /// </summary>
        /// <param name="material">The material to apply render mode settings to.</param>
        protected virtual void MaterialChanged(Material material)
        {
            SetupMaterialWithRenderingMode(material,
                (RenderingMode)renderingMode.floatValue,
                (RenderingMode)customRenderingMode.floatValue,
                (int)renderQueueOverride.floatValue);
        }

        /// <summary>
        /// Displays various render mode options in the inspector.
        /// </summary>
        /// <param name="materialEditor">Current material editor in use.</param>
        protected void RenderingModeOptions(MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.showMixedValue = renderingMode.hasMixedValue;
            RenderingMode mode = (RenderingMode)renderingMode.floatValue;
            EditorGUI.BeginChangeCheck();
            mode = (RenderingMode)EditorGUILayout.Popup(renderingMode.displayName, (int)mode, BaseStyles.renderingModeNames);

            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(renderingMode.displayName);
                renderingMode.floatValue = (float)mode;

                Object[] targets = renderingMode.targets;

                foreach (Object target in targets)
                {
                    MaterialChanged((Material)target);
                }
            }

            EditorGUI.showMixedValue = false;

            if ((RenderingMode)renderingMode.floatValue == RenderingMode.Custom)
            {
                EditorGUI.indentLevel += 2;
                customRenderingMode.floatValue = EditorGUILayout.Popup(customRenderingMode.displayName, (int)customRenderingMode.floatValue, BaseStyles.renderingModeNames);
                materialEditor.ShaderProperty(sourceBlend, BaseStyles.sourceBlend);
                materialEditor.ShaderProperty(destinationBlend, BaseStyles.destinationBlend);
                materialEditor.ShaderProperty(sourceBlendAlpha, BaseStyles.sourceBlendAlpha);
                materialEditor.ShaderProperty(destinationBlendAlpha, BaseStyles.destinationBlendAlpha);
                materialEditor.ShaderProperty(blendOperation, BaseStyles.blendOperation);
                materialEditor.ShaderProperty(depthTest, BaseStyles.depthTest);
                depthWrite.floatValue = EditorGUILayout.Popup(depthWrite.displayName, (int)depthWrite.floatValue, BaseStyles.depthWriteNames);
                materialEditor.ShaderProperty(depthOffsetFactor, BaseStyles.depthOffsetFactor);
                materialEditor.ShaderProperty(depthOffsetUnits, BaseStyles.depthOffsetUnits);
                materialEditor.ShaderProperty(colorWriteMask, BaseStyles.colorWriteMask);
                EditorGUI.indentLevel -= 2;
            }

            if (!PropertyEnabled(depthWrite))
            {
                if (ShaderGUIUtilities.DisplayDepthWriteWarning(materialEditor))
                {
                    renderingMode.floatValue = (float)RenderingMode.Custom;
                    depthWrite.floatValue = (float)DepthWrite.On;
                }
            }

            materialEditor.ShaderProperty(cullMode, BaseStyles.cullMode);
        }

        /// <summary>
        /// Given a rendering mode applies all the material properties associated with that mode.
        /// </summary>
        /// <param name="material">The material to apply the settings to.</param>
        /// <param name="mode">The rendering mode.</param>
        /// <param name="customMode">The sub-categrory of the custome mode.</param>
        /// <param name="renderQueueOverride">The rendering order override.</param>
        protected static void SetupMaterialWithRenderingMode(Material material, RenderingMode mode, RenderingMode customMode, int renderQueueOverride)
        {
            // If we aren't switching to Custom, then set default values for all RenderingMode types. Otherwise keep whatever user had before
            if (mode != RenderingMode.Custom)
            {
                material.SetInt(BaseStyles.blendOperationName, (int)BlendOp.Add);
                material.SetInt(BaseStyles.depthTestName, (int)CompareFunction.LessEqual);
                material.SetFloat(BaseStyles.depthOffsetFactorName, 0.0f);
                material.SetFloat(BaseStyles.depthOffsetUnitsName, 0.0f);
                material.SetInt(BaseStyles.colorWriteMaskName, (int)ColorWriteMask.All);
                material.SetInt(BaseStyles.colorWriteMaskUIName, (int)ColorWriteMask.All);
            }

            switch (mode)
            {
                case RenderingMode.Opaque:
                    {
                        material.SetOverrideTag(BaseStyles.renderTypeName, BaseStyles.renderingModeNames[(int)RenderingMode.Opaque]);
                        material.SetInt(BaseStyles.customRenderingModeName, (int)RenderingMode.Opaque);
                        material.SetInt(BaseStyles.sourceBlendName, (int)BlendMode.One);
                        material.SetInt(BaseStyles.destinationBlendName, (int)BlendMode.Zero);
                        material.SetInt(BaseStyles.depthWriteName, (int)DepthWrite.On);
                        material.DisableKeyword(BaseStyles.alphaTestOnName);
                        material.DisableKeyword(BaseStyles.alphaBlendFadeOnName);
                        material.DisableKeyword(BaseStyles.alphaBlendTransOnName);
                        material.DisableKeyword(BaseStyles.additiveOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)RenderQueue.Geometry;
                    }
                    break;

                case RenderingMode.Cutout:
                    {
                        material.SetOverrideTag(BaseStyles.renderTypeName, BaseStyles.renderingModeNames[(int)RenderingMode.Cutout]);
                        material.SetInt(BaseStyles.customRenderingModeName, (int)RenderingMode.Cutout);
                        material.SetInt(BaseStyles.sourceBlendName, (int)BlendMode.One);
                        material.SetInt(BaseStyles.destinationBlendName, (int)BlendMode.Zero);
                        material.SetInt(BaseStyles.depthWriteName, (int)DepthWrite.On);
                        material.EnableKeyword(BaseStyles.alphaTestOnName);
                        material.DisableKeyword(BaseStyles.alphaBlendFadeOnName);
                        material.DisableKeyword(BaseStyles.alphaBlendTransOnName);
                        material.DisableKeyword(BaseStyles.additiveOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)RenderQueue.AlphaTest;
                    }
                    break;

                case RenderingMode.Fade:
                    {
                        material.SetOverrideTag(BaseStyles.renderTypeName, BaseStyles.renderingModeNames[(int)RenderingMode.Fade]);
                        material.SetInt(BaseStyles.customRenderingModeName, (int)RenderingMode.Fade);
                        material.SetInt(BaseStyles.sourceBlendName, (int)BlendMode.SrcAlpha);
                        material.SetInt(BaseStyles.destinationBlendName, (int)BlendMode.OneMinusSrcAlpha);
                        material.SetInt(BaseStyles.depthWriteName, (int)DepthWrite.Off);
                        material.DisableKeyword(BaseStyles.alphaTestOnName);
                        material.EnableKeyword(BaseStyles.alphaBlendFadeOnName);
                        material.DisableKeyword(BaseStyles.alphaBlendTransOnName);
                        material.DisableKeyword(BaseStyles.additiveOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)RenderQueue.Transparent;
                    }
                    break;

                case RenderingMode.Transparent:
                    {
                        material.SetOverrideTag(BaseStyles.renderTypeName, BaseStyles.renderingModeNames[(int)RenderingMode.Fade]);
                        material.SetInt(BaseStyles.customRenderingModeName, (int)RenderingMode.Fade);
                        material.SetInt(BaseStyles.sourceBlendName, (int)BlendMode.One);
                        material.SetInt(BaseStyles.destinationBlendName, (int)BlendMode.OneMinusSrcAlpha);
                        material.SetInt(BaseStyles.depthWriteName, (int)DepthWrite.Off);
                        material.DisableKeyword(BaseStyles.alphaTestOnName);
                        material.DisableKeyword(BaseStyles.alphaBlendFadeOnName);
                        material.EnableKeyword(BaseStyles.alphaBlendTransOnName);
                        material.DisableKeyword(BaseStyles.additiveOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)RenderQueue.Transparent;
                    }
                    break;

                case RenderingMode.Additive:
                    {
                        material.SetOverrideTag(BaseStyles.renderTypeName, BaseStyles.renderingModeNames[(int)RenderingMode.Fade]);
                        material.SetInt(BaseStyles.customRenderingModeName, (int)RenderingMode.Fade);
                        material.SetInt(BaseStyles.sourceBlendName, (int)BlendMode.One);
                        material.SetInt(BaseStyles.destinationBlendName, (int)BlendMode.One);
                        material.SetInt(BaseStyles.depthWriteName, (int)DepthWrite.Off);
                        material.DisableKeyword(BaseStyles.alphaTestOnName);
                        material.DisableKeyword(BaseStyles.alphaBlendFadeOnName);
                        material.DisableKeyword(BaseStyles.alphaBlendTransOnName);
                        material.EnableKeyword(BaseStyles.additiveOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)RenderQueue.Transparent;
                    }
                    break;

                case RenderingMode.Custom:
                    {
                        material.SetOverrideTag(BaseStyles.renderTypeName, BaseStyles.renderingModeNames[(int)customMode]);
                        // _SrcBlend, _DstBlend, _BlendOp, _ZTest, _ZWrite, _ColorWriteMask are controlled by UI.

                        switch (customMode)
                        {
                            case RenderingMode.Opaque:
                                {
                                    material.DisableKeyword(BaseStyles.alphaTestOnName);
                                    material.DisableKeyword(BaseStyles.alphaBlendFadeOnName);
                                    material.DisableKeyword(BaseStyles.alphaBlendTransOnName);
                                    material.DisableKeyword(BaseStyles.additiveOnName);
                                }
                                break;

                            case RenderingMode.Cutout:
                                {
                                    material.EnableKeyword(BaseStyles.alphaTestOnName);
                                    material.DisableKeyword(BaseStyles.alphaBlendFadeOnName);
                                    material.DisableKeyword(BaseStyles.alphaBlendTransOnName);
                                    material.DisableKeyword(BaseStyles.additiveOnName);
                                }
                                break;

                            case RenderingMode.Fade:
                                {
                                    material.DisableKeyword(BaseStyles.alphaTestOnName);
                                    material.EnableKeyword(BaseStyles.alphaBlendFadeOnName);
                                    material.DisableKeyword(BaseStyles.alphaBlendTransOnName);
                                    material.DisableKeyword(BaseStyles.additiveOnName);
                                }
                                break;

                            case RenderingMode.Transparent:
                                {
                                    material.DisableKeyword(BaseStyles.alphaTestOnName);
                                    material.DisableKeyword(BaseStyles.alphaBlendFadeOnName);
                                    material.EnableKeyword(BaseStyles.alphaBlendTransOnName);
                                    material.DisableKeyword(BaseStyles.additiveOnName);
                                }
                                break;

                            case RenderingMode.Additive:
                                {
                                    material.DisableKeyword(BaseStyles.alphaTestOnName);
                                    material.DisableKeyword(BaseStyles.alphaBlendFadeOnName);
                                    material.DisableKeyword(BaseStyles.alphaBlendTransOnName);
                                    material.EnableKeyword(BaseStyles.additiveOnName);
                                }
                                break;

                            case RenderingMode.Custom:
                                {
                                    material.DisableKeyword(BaseStyles.alphaTestOnName);
                                    material.DisableKeyword(BaseStyles.alphaBlendFadeOnName);
                                    material.DisableKeyword(BaseStyles.alphaBlendTransOnName);
                                    material.DisableKeyword(BaseStyles.additiveOnName);
                                }
                                break;
                        }

                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : material.renderQueue;
                    }
                    break;
            }
        }

        /// <summary>
        /// Check whether shader feature is enabled
        /// </summary>
        /// <param name="property">float property to check against</param>
        /// <returns>false if 0.0f, true otherwise</returns>
        protected static bool PropertyEnabled(MaterialProperty property)
        {
            return !property.floatValue.Equals(0.0f);
        }

        /// <summary>
        /// Get the value of a given float property for a material
        /// </summary>
        /// <param name="material">material to check</param>
        /// <param name="propertyName">name of property against material</param>
        /// <returns>if has property, then value of that property for current material, null otherwise</returns>
        protected static float? GetFloatProperty(Material material, string propertyName)
        {
            if (material.HasProperty(propertyName))
            {
                return material.GetFloat(propertyName);
            }

            return null;
        }

        /// <summary>
        /// Get the value of a given vector property for a material
        /// </summary>
        /// <param name="material">material to check</param>
        /// <param name="propertyName">name of property against material</param>
        /// <returns>if has property, then value of that property for current material, null otherwise</returns>
        protected static Vector4? GetVectorProperty(Material material, string propertyName)
        {
            if (material.HasProperty(propertyName))
            {
                return material.GetVector(propertyName);
            }

            return null;
        }

        /// <summary>
        /// Get the value of a given color property for a material
        /// </summary>
        /// <param name="material">material to check</param>
        /// <param name="propertyName">name of property against material</param>
        /// <returns>if has property, then value of that property for current material, null otherwise</returns>
        protected static Color? GetColorProperty(Material material, string propertyName)
        {
            if (material.HasProperty(propertyName))
            {
                return material.GetColor(propertyName);
            }

            return null;
        }

        /// <summary>
        /// Sets the shader feature controlled by keyword and property name parameters active or inactive
        /// </summary>
        /// <param name="material">Material to modify</param>
        /// <param name="keywordName">Keyword of shader feature</param>
        /// <param name="propertyName">Associated property name for shader feature</param>
        /// <param name="propertyValue">float to be treated as a boolean flag for setting shader feature active or inactive</param>
        protected static void SetShaderFeatureActive(Material material, string keywordName, string propertyName, float? propertyValue)
        {
            if (propertyValue.HasValue)
            {
                if (keywordName != null)
                {
                    if (!propertyValue.Value.Equals(0.0f))
                    {
                        material.EnableKeyword(keywordName);
                    }
                    else
                    {
                        material.DisableKeyword(keywordName);
                    }
                }

                material.SetFloat(propertyName, propertyValue.Value);
            }
        }

        /// <summary>
        /// Sets vector property against associated material
        /// </summary>
        /// <param name="material">material to control</param>
        /// <param name="propertyName">name of property to set</param>
        /// <param name="propertyValue">value of property to set</param>
        protected static void SetVectorProperty(Material material, string propertyName, Vector4? propertyValue)
        {
            if (propertyValue.HasValue)
            {
                material.SetVector(propertyName, propertyValue.Value);
            }
        }

        /// <summary>
        /// Set color property against associated material
        /// </summary>
        /// <param name="material">material to control</param>
        /// <param name="propertyName">name of property to set</param>
        /// <param name="propertyValue">value of property to set</param>
        protected static void SetColorProperty(Material material, string propertyName, Color? propertyValue)
        {
            if (propertyValue.HasValue)
            {
                material.SetColor(propertyName, propertyValue.Value);
            }
        }
    }
}

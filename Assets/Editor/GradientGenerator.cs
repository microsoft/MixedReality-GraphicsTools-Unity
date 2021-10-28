// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// QuadLinear will encode the values linearly. Values appear brigher than QuadSRGB
    /// QuadSRGB will encode the values in a way that appears more uniform and darker than QuadLinear
    /// </summary>
    public enum GradientMethod
    {
        QuadLinear,
        QuadPerceptualLinear
    }

    /// <summary>
    /// Generate procedural gradients for use as texture maps.
    /// </summary>
    public class GradientGenerator : EditorWindow
    {
        [Tooltip("The overall color of the gradient. The gradient result is multiplied by this color.")]
        public Color Tint = Color.white;

        [Tooltip("The color in the top-right corner of the resulting texture.")]
        public Color TopLeft = Color.red;

        [Tooltip("The color in the top-right corner of the resulting texture.")]
        public Color TopRight = Color.green;

        [Tooltip("The color in the bottom-left corner of the resulting texture.")]
        public Color BottomLeft = Color.blue;

        [Tooltip("The color in the bottom-right corner of the resulting texture.")]
        public Color BottomRight = Color.black;

        [Tooltip("The pixel resolution of the output texture.")]
        public int OutputResolution = 32;

        [Tooltip("When FALSE a time stamp is appended to the name of the texture output.")]
        public bool WillOverwriteOutput = true;

        [Tooltip("Disabling this can save some CPU cyles. The resolution of the preview is independent of the output resolution.")]
        public bool EnablePreview = true;

        [Tooltip("Changes the method used to generate the blends. This will affect the overall look.")]
        public GradientMethod Method = GradientMethod.QuadLinear;

        [Tooltip("The prefix applied to the saved textures and materials.")]
        public string AssetPrefix = "Gradient";

        private int _gradientColor = Shader.PropertyToID("_Gradient_Color_");
        private int _topLeftPropName = Shader.PropertyToID("_Top_Left_");
        private int _topRightPropName = Shader.PropertyToID("_Top_Right_");
        private int _bottomLeftPropName = Shader.PropertyToID("_Bottom_Left_");
        private int _bottomRightPropName = Shader.PropertyToID("_Bottom_Right_");

        /// <summary>
        /// This matches the shader math from CanvasBackplate
        /// Returns a blended color for the UV coordinate
        /// UV expected to be normalized in the 0-1 range.
        /// </summary>
        public static Color Gradient(
            Color Tint,
            Color Top_Left,
            Color Top_Right,
            Color Bottom_Left,
            Color Bottom_Right,
            Vector2 UV)
        {
            Vector4 result;

            var top = Top_Left + (Top_Right - Top_Left) * UV.x;
            var bottom = Bottom_Right + (Bottom_Left - Bottom_Right) * UV.x;

            result = Tint * (bottom + (top - bottom) * UV.y);

            return result;
        }

        /// <summary>
        /// Returns a texture map in float format for smooth results.
        /// </summary>
        public static Texture2D GradientToTexture(
            Color tint,
            Color topLeft,
            Color topRight,
            Color bottomLeft,
            Color bottomRight,
            GradientMethod method,
            int resolution)
        {
            var result = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false, true);

            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    // Sample the gradient
                    var st = new Vector2(x / ((float)resolution - 1), y / ((float)resolution - 1));
                    var gradient = Gradient(tint.linear, topLeft.linear, topRight.linear, bottomRight.linear, bottomLeft.linear, st);

                    switch (method)
                    {
                        case GradientMethod.QuadPerceptualLinear:
                            // Squash the linear colors so when they
                            // are boosted by sRGB they look more uniform.
                            gradient = SrgbToLinear(gradient);
                            result.SetPixel(x, y, gradient);

                            break;

                        default:
                            result.SetPixel(x, y, gradient);
                            break;
                    }
                }
            }

            result.Apply();

            return result;
        }

        [MenuItem("Tools/MRTK Graphics Tools/Gradient generator")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(GradientGenerator), true, "Gradient generator");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                Method = (GradientMethod)EditorGUILayout.EnumPopup("Method", Method);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Tint");
                Tint = EditorGUILayout.ColorField(Tint);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Top left");
                TopLeft = EditorGUILayout.ColorField(TopLeft);
                TopRight = EditorGUILayout.ColorField(TopRight);
                EditorGUILayout.LabelField("Top right");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Bottom left");
                BottomLeft = EditorGUILayout.ColorField(BottomLeft);
                BottomRight = EditorGUILayout.ColorField(BottomRight);
                EditorGUILayout.LabelField("Bottom right");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Resolution");
                OutputResolution = EditorGUILayout.IntSlider(OutputResolution, 1, 2048);
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Sample colors from canvas backplate"))
            {
                if (Selection.activeObject is GameObject go)
                {
                    if (go.GetComponent<CanvasElementRoundedRect>().material is Material mat)
                    {
                        if (mat.HasProperty(_gradientColor))
                        {
                            Tint = mat.GetColor(_gradientColor);
                        }

                        if (mat.HasProperty(_topLeftPropName))
                        {
                            TopLeft = mat.GetColor(_topLeftPropName);
                        }

                        if (mat.HasProperty(_topRightPropName))
                        {
                            TopRight = mat.GetColor(_topRightPropName);
                        }

                        if (mat.HasProperty(_bottomLeftPropName))
                        {
                            BottomLeft = mat.GetColor(_bottomLeftPropName);
                        }

                        if (mat.HasProperty(_bottomRightPropName))
                        {
                            BottomRight = mat.GetColor(_bottomRightPropName);
                        }
                    }
                }
            }

            // WARRNING The gradient preview gets called a lot, when the window is open.

            EnablePreview = EditorGUILayout.ToggleLeft("Enable preview (may slow down Editor)", EnablePreview);

            if (EnablePreview)
            {
                var previewResolution = 128;
                var texturePreview = GradientToTexture(Tint, TopLeft, TopRight, BottomLeft, BottomRight, Method, previewResolution);
                GUILayout.Box(texturePreview);
            }

            WillOverwriteOutput = EditorGUILayout.ToggleLeft("Overwrite output", WillOverwriteOutput);

            EnablePreview = EditorGUILayout.ToggleLeft("Generate preview material", EnablePreview);

            AssetPrefix = EditorGUILayout.TextField("Asset prefix", AssetPrefix);

            if (GUILayout.Button("Save to project assets"))
            {
                if (AssetPrefix == "")
                {
                    Debug.LogError("Asset prefix must have a value! Inserting default...");
                    AssetPrefix = "Gradient";
                }

                // Render the texture and save it to disk

                var texture = GradientToTexture(Tint, TopLeft, TopRight, BottomLeft, BottomRight, Method, OutputResolution);
                byte[] bytes = texture.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);

                var projectDirPath = Application.dataPath;

                if (!WillOverwriteOutput)
                {
                    var date = DateTime.Now.ToString("yyyyddMHHmmss");
                    AssetPrefix = $"{AssetPrefix}_{date}";
                }

                var fileNameExt = $"{AssetPrefix}.exr";
                var filePathName = $"{projectDirPath}/{fileNameExt}";

                System.IO.File.WriteAllBytes(filePathName, bytes);

                Debug.Log($"{bytes.Length / 1024} Kb saved to: {filePathName}");

                AssetDatabase.Refresh();

                // Setup the texture importer correctly for smooth results

                var assetPath = $"Assets/{fileNameExt}";
                var importer = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                if (importer != null)
                {
                    importer.wrapMode = TextureWrapMode.Clamp;
                    importer.sRGBTexture = false;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                }

                if (EnablePreview)
                {
                    // Update the preview material with the texture
                    var tex = (Texture)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture));
                    if (tex != null)
                    {
                        UpdatePreviewMaterial(tex);
                    }
                }
            }
        }

        /// <summary>
        /// As a convenience, create a material that uses the texture map
        /// </summary>
        private void UpdatePreviewMaterial(Texture texture)
        {
            var matAssetFilePathname = $"Assets/{AssetPrefix}_Preview.mat";
            var material = (Material)AssetDatabase.LoadAssetAtPath(matAssetFilePathname, typeof(Material));

            if (material == null)
            {
                // No preview material created before, make a new one...
                var unlit = new Material(Shader.Find("Particles/Standard Unlit"));
                AssetDatabase.CreateAsset(unlit, matAssetFilePathname);
                material = (Material)AssetDatabase.LoadAssetAtPath(matAssetFilePathname, typeof(Material));
                AssetDatabase.Refresh();
            }

            if (material != null)
            {
                material.SetTexture("_MainTex", texture);
            }
        }

        /// <summary>
        /// Removes sRGB encoding from a color
        /// </summary>
        /// <param name="srgb"></param>
        /// <returns></returns>
        public static Color SrgbToLinear(Color srgb)
        {
            return new Color(
                SrgbToLinear(srgb.r),
                SrgbToLinear(srgb.g),
                SrgbToLinear(srgb.b),
                SrgbToLinear(srgb.a));
        }

        /// <summary>
        /// Encodes a linear value as sRGB
        /// </summary>
        public static Color LinearToSrgb(Color srgb)
        {
            return new Color(
                LinearToSrgb(srgb.r),
                LinearToSrgb(srgb.g),
                LinearToSrgb(srgb.b),
                LinearToSrgb(srgb.a));
        }

        /// <summary>
        /// Removes sRGB encoding from a float
        /// https://en.wikipedia.org/wiki/SRGB#Transformation
        /// </summary>
        /// <param name="x"></param>
        public static float SrgbToLinear(float x)
        {
            return x <= 0.04045f ? x / 12.92f : Mathf.Pow((x + 0.055f) / 1.055f, 2.4f);
        }

        /// <summary>
        /// Encodes a linear value as sRGB
        /// https://en.wikipedia.org/wiki/SRGB#Transformation
        /// </summary>
        public static float LinearToSrgb(float x)
        {
            return x <= 0.0031308f ? x * 12.92f : Mathf.Pow(1.055f * x, 1.0f / 2.4f);
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Graphics Tools standard shader utility class with commonly used constants, types and convenience methods.
    /// </summary>
    public static class StandardShaderUtility
    {
        /// <summary>
        /// The string name of the Standard shader which can be used to identify a shader or for shader lookups.
        /// </summary>
        public static readonly string GraphicsToolsStandardShaderName = "Graphics Tools/Standard";

        /// <summary>
        /// Returns an instance of the Graphics Tools/Standard shader.
        /// </summary>
        public static Shader GraphicsToolsStandardShader
        {
            get
            {
                if (graphicsToolsStandardShader == null)
                {
                    graphicsToolsStandardShader = Shader.Find(GraphicsToolsStandardShaderName);
                }

                return graphicsToolsStandardShader;
            }

            private set
            {
                graphicsToolsStandardShader = value;
            }
        }

        private static Shader graphicsToolsStandardShader = null;

        /// <summary>
        /// The string name of the Standard Canvas shader which can be used to identify a shader or for shader lookups.
        /// </summary>
        public static readonly string GraphicsToolsStandardCanvasShaderName = "Graphics Tools/Standard Canvas";

        /// <summary>
        /// Returns an instance of the Graphics Tools/Standard shader.
        /// </summary>
        public static Shader GraphicsToolsStandardCanvasShader
        {
            get
            {
                if (graphicsToolsStandardCanvasShader == null)
                {
                    graphicsToolsStandardCanvasShader = Shader.Find(GraphicsToolsStandardCanvasShaderName);
                }

                return graphicsToolsStandardCanvasShader;
            }

            private set
            {
                graphicsToolsStandardCanvasShader = value;
            }
        }

        private static Shader graphicsToolsStandardCanvasShader = null;

        /// <summary>
        /// Checks if a shader is the Graphics Tools/Standard or Standard Canvas shader.
        /// </summary>
        /// <param name="shader">The shader to check.</param>
        /// <returns>True if the shader is the shader.</returns>
        public static bool IsGraphicsToolsStandardShader(Shader shader)
        {
            return shader == GraphicsToolsStandardShader ||
                   shader == GraphicsToolsStandardCanvasShader;
        }

        /// <summary>
        /// Checks if a material is using the Graphics Tools/Standard or Standard Canvas shader.
        /// </summary>
        /// <param name="material">The material to check.</param>
        /// <returns>True if the material is using the shader</returns>
        public static bool IsUsingGraphicsToolsStandardShader(Material material)
        {
            return IsGraphicsToolsStandardShader((material != null) ? material.shader : null);
        }

        /// <summary>
        /// The string name of the Graphics Tools Text Mesh Pro shader which can be used to identify a shader or for shader lookups.
        /// </summary>
        public static readonly string GraphicsToolsTextMeshProShaderName = "Graphics Tools/Text Mesh Pro";

        /// <summary>
        /// Returns an instance of the Graphics Tools/Text Mesh Pro shader.
        /// </summary>
        public static Shader GraphicsToolsTextMeshProShader
        {
            get
            {
                if (graphicsToolsTextMeshProShader == null)
                {
                    graphicsToolsTextMeshProShader = Shader.Find(GraphicsToolsTextMeshProShaderName);
                }

                return graphicsToolsTextMeshProShader;
            }

            private set
            {
                graphicsToolsTextMeshProShader = value;
            }
        }

        private static Shader graphicsToolsTextMeshProShader = null;

        /// <summary>
        /// Checks if a shader is the Graphics Tools/Text Mesh Pro shader.
        /// </summary>
        /// <param name="shader">The shader to check.</param>
        /// <returns>True if the shader is the shader.</returns>
        public static bool IsGraphicsToolsTextMeshProShader(Shader shader)
        {
            return shader == GraphicsToolsTextMeshProShader;
        }

        /// <summary>
        /// Checks if a material is using the Graphics Tools/Text Mesh Pro shader.
        /// </summary>
        /// <param name="material">The material to check.</param>
        /// <returns>True if the material is using the shader</returns>
        public static bool IsUsingGraphicsToolsTextMeshProShader(Material material)
        {
            return IsGraphicsToolsTextMeshProShader((material != null) ? material.shader : null);
        }

        /// <summary>
        /// Shifts a source color in HSV color space.
        /// </summary>
        public static Color ColorShiftHSV(Color source, float hueOffset, float saturationtOffset, float valueOffset)
        {
            float hue, saturation, value;
            Color.RGBToHSV(source, out hue, out saturation, out value);
            hue = hue + hueOffset;
            saturation = Mathf.Clamp01(saturation + saturationtOffset);
            value = Mathf.Clamp01(value + valueOffset);
            Color output = Color.HSVToRGB(hue, saturation, value);
            output.a = source.a;
            return output;
        }

        /// <summary>
        /// Given a source color produces a visually pleasing" gradient by shifting the source color in HSV space.
        /// </summary>
        public static void AutofillFourPointGradient(Color source, out Color topLeft, out Color topRight, out Color bottomLeft, out Color bottomRight, out Color stroke)
        {
            topLeft = source;
            topRight = ColorShiftHSV(source, 0.02f, -0.2f, -0.1f);
            bottomLeft = ColorShiftHSV(source, -0.03f, 0.1f, 0.3f);
            bottomRight = ColorShiftHSV(source, 0.01f, -0.1f, 0.2f);
            stroke = ColorShiftHSV(source, 0.01f, -0.2f, 0.0f);
        }

        /// <summary>
        /// Attempts to parse a CSS gradient (https://developer.mozilla.org/en-US/docs/Web/CSS/gradient) into an array of colors and times. 
        /// Note, only linear gradients are supported at the moment. And, not all CSS gradient features are supported.
        /// An example input sting is: background: background: linear-gradient(90deg, #0380FD 0%, #406FC8 19.05%, #2B398F 49.48%, #FF77C1 100%);
        /// </summary>
        public static bool TryParseCSSGradient(string cssGradient, out Color[] gradientColors, out float[] gradientTimes, out float gradientAngle)
        {
            const float defaultCSSAngle = 180.0f;

            bool success;

            try
            {
                // Extract the gradient structure.
                const string prefix = "linear-gradient(";
                const string postfix = ");";
                int start = cssGradient.IndexOf(prefix) + prefix.Length;
                int end = cssGradient.IndexOf(postfix, start);
                string gradient = cssGradient.Substring(start, end - start);

                string[] parameters = gradient.Split(',');

                float angle = defaultCSSAngle;
                List<Color> colorKeys = new List<Color>();
                List<float> timeKeys = new List<float>();

                // Parse each parameter.
                for (int i = 0; i < parameters.Length; ++i)
                {
                    // Handle degrees.
                    if (parameters[i].Contains("deg"))
                    {
                        float.TryParse(parameters[i].Replace("deg", string.Empty), out angle);
                    }
                    else // Handle colors and times.
                    {
                        // Parse rgba format.
                        if (parameters[i].Contains("rgba("))
                        {
                            float NormalizeColorChannel(float channel)
                            {
                                if (channel > 1.0f)
                                {
                                    channel = channel / 255;
                                }

                                return channel;
                            }

                            float red, green, blue, alpha = 1.0f;

                            if (float.TryParse(parameters[i].Replace("rgba(", string.Empty), out red))
                            {
                                red = NormalizeColorChannel(red);
                            }
                            if (float.TryParse(parameters[i + 1], out green))
                            {
                                green = NormalizeColorChannel(green);
                            }
                            if (float.TryParse(parameters[i + 2], out blue))
                            {
                                blue = NormalizeColorChannel(blue);
                            }

                            string[] colorKey = parameters[i + 3].Split(new string[] { ") " }, StringSplitOptions.RemoveEmptyEntries);

                            if (float.TryParse(colorKey[0], out alpha))
                            {
                                alpha = NormalizeColorChannel(alpha);
                            }

                            colorKeys.Add(new Color(red, green, blue, alpha));

                            float time;
                            if (colorKey.Length > 1 &&
                                float.TryParse(colorKey[1].Replace("%", string.Empty), out time))
                            {
                                timeKeys.Add(time / 100);
                            }
                            else
                            {
                                timeKeys.Add(-1.0f); // Signal that we need to generate a time value.
                            }

                            i += 3;
                        }
                        else // Parse hex/name format.
                        {
                            string[] colors = parameters[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            Color color;

                            if (ColorUtility.TryParseHtmlString(colors[0], out color))
                            {
                                colorKeys.Add(color);

                                float time;
                                if (colors.Length > 1 &&
                                    float.TryParse(colors[1].Replace("%", string.Empty), out time))
                                {
                                    timeKeys.Add(time / 100);
                                }
                                else
                                {
                                    timeKeys.Add(-1.0f); // Signal that we need to generate a time value.
                                }
                            }
                        }
                    }
                }

                if (timeKeys.Count >= 2)
                {
                    // If no times were provided, assume regular interval.
                    for (int i = 0; i < timeKeys.Count; ++i)
                    {
                        float time = timeKeys[i];

                        if (time < 0)
                        {
                            time = (timeKeys.Count != 1) ? (float)i / (timeKeys.Count - 1) : 0.0f;
                            timeKeys[i] = time;
                        }
                    }

                    // Ensure the last time goes to one.
                    timeKeys[colorKeys.Count - 1] = 1.0f;

                    success = true;
                }
                else
                {
                    success = false;
                }

                gradientColors = colorKeys.ToArray();
                gradientTimes = timeKeys.ToArray();
                gradientAngle = angle;
            }
            catch
            {
                // Failed to parse gradient.
                success = false;

                gradientColors = null;
                gradientTimes = null;
                gradientAngle = defaultCSSAngle;
            }

            return success;
        }
    }
}

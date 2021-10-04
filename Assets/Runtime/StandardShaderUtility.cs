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
        /// The string name of the Graphics Tools/Standard shader which can be used to identify a shader or for shader lookups.
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
        /// Checks if a material is using the Graphics Tools/Standard shader.
        /// </summary>
        /// <param name="material">The material to check.</param>
        /// <returns>True if the material is using the Graphics Tools/Standard shader</returns>
        public static bool IsUsingGraphicsToolsStandardShader(Material material)
        {
            return IsGraphicsToolsStandardShader((material != null) ? material.shader : null);
        }

        /// <summary>
        /// Checks if a shader is the Graphics Tools/Standard shader.
        /// </summary>
        /// <param name="shader">The shader to check.</param>
        /// <returns>True if the shader is the Graphics Tools/Standard shader.</returns>
        public static bool IsGraphicsToolsStandardShader(Shader shader)
        {
            return shader == GraphicsToolsStandardShader;
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
        /// Attempts to parse a CSS gradient (https://developer.mozilla.org/en-US/docs/Web/CSS/gradient) into an array of colors and stops. 
        /// Note, only linear gradients are supported at the moment. And, not all CSS gradient features are supported.
        /// An example input sting is: background: background: linear-gradient(90deg, #0380FD 0%, #406FC8 19.05%, #2B398F 49.48%, #FF77C1 100%);
        /// </summary>
        public static bool TryParseCSSGradient(string cssGradient, int keyCount, out Color[] gradientColors, out float[] gradientStops, out float gradientAngle)
        {
            const float defaultCSSAngle = 180.0f;

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
                List<float> stopKeys = new List<float>();

                // Parse each parameter, or up to `keyCount`.
                for (int i = 0; i < parameters.Length && colorKeys.Count <= keyCount; ++i)
                {
                    // Handle degrees.
                    if (parameters[i].Contains("deg"))
                    {
                        float.TryParse(parameters[i].Replace("deg", string.Empty), out angle);
                    }
                    else // Handle colors and stops.
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

                            string[] colorStop = parameters[i + 3].Split(new string[] { ") " }, StringSplitOptions.RemoveEmptyEntries);

                            if (float.TryParse(colorStop[0], out alpha))
                            {
                                alpha = NormalizeColorChannel(alpha);
                            }

                            colorKeys.Add(new Color(red, green, blue, alpha));

                            float stop;
                            if (colorStop.Length > 1 &&
                                float.TryParse(colorStop[1].Replace("%", string.Empty), out stop))
                            {
                                stopKeys.Add(stop / 100);
                            }
                            else
                            {
                                stopKeys.Add(-1.0f); // Signal that we need to generate a stop value.
                            }

                            i += 3;
                        }
                        else // Parse hex/name format.
                        {
                            string[] colorStop = parameters[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            Color color;

                            if (ColorUtility.TryParseHtmlString(colorStop[0], out color))
                            {
                                colorKeys.Add(color);

                                float stop;
                                if (colorStop.Length > 1 &&
                                    float.TryParse(colorStop[1].Replace("%", string.Empty), out stop))
                                {
                                    stopKeys.Add(stop / 100);
                                }
                                else
                                {
                                    stopKeys.Add(-1.0f); // Signal that we need to generate a stop value.
                                }
                            }
                        }
                    }
                }

                // If no stop was provided, assume regular interval.
                for (int i = 0; i < stopKeys.Count; ++i)
                {
                    float stop = stopKeys[i];

                    if (stop < 0)
                    {
                        stop = (stopKeys.Count != 1) ? (float)i / (stopKeys.Count - 1) : 0.0f;
                        stopKeys[i] = stop;
                    }
                }

                // Ensure we always have keyCount colors/stops.
                while (colorKeys.Count < keyCount)
                {
                    if (colorKeys.Count != 0)
                    {
                        colorKeys.Add(colorKeys[colorKeys.Count - 1]);
                        stopKeys.Add(stopKeys[stopKeys.Count - 1]);
                    }
                    else
                    {
                        colorKeys.Add(Color.white);
                        stopKeys.Add(1.0f);
                    }
                }

                // Ensure the last stop goes to one.
                stopKeys[colorKeys.Count - 1] = 1.0f;

                gradientColors = colorKeys.ToArray();
                gradientStops = stopKeys.ToArray();
                gradientAngle = angle;
            }
            catch
            {
                // Failed to parse gradient.
                gradientColors = new Color[keyCount];
                gradientStops = new float[keyCount];
                gradientAngle = defaultCSSAngle;

                return false;
            }

            return true;
        }
    }
}

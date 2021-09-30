// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    }
}

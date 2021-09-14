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
        public static bool IsUsingMrtkStandardShader(Material material)
        {
            return IsMrtkStandardShader((material != null) ? material.shader : null);
        }

        /// <summary>
        /// Checks if a shader is the Graphics Tools/Standard shader.
        /// </summary>
        /// <param name="shader">The shader to check.</param>
        /// <returns>True if the shader is the Graphics Tools/Standard shader.</returns>
        public static bool IsMrtkStandardShader(Shader shader)
        {
            return shader == GraphicsToolsStandardShader;
        }
    }
}

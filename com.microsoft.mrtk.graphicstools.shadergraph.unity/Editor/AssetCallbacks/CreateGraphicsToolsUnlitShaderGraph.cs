// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEditor;
using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// GraphicsToolsUniversalUnlitSubTarget creation.
    /// </summary>
    static class CreateGraphicsToolsUnlitShaderGraph
    {
        /// <summary>
        /// Menu item to automatically create a shader graph with the correct sub target.
        /// </summary>
        [MenuItem("Assets/Create/Shader Graph/GraphicsTools/URP/Unlit Shader Graph", priority = CoreUtils.Priorities.assetsCreateShaderMenuPriority + 1)]
        public static void CreateGraphicsToolsUnlitGraph()
        {
            var target = (UniversalTarget)Activator.CreateInstance(typeof(UniversalTarget));
            target.TrySetActiveSubTarget(typeof(GraphicsToolsUniversalUnlitSubTarget));

            var blockDescriptors = new[]
            {
                BlockFields.VertexDescription.Position,
                BlockFields.VertexDescription.Normal,
                BlockFields.VertexDescription.Tangent,
                BlockFields.SurfaceDescription.BaseColor,
            };

            GraphUtil.CreateNewGraphWithOutputs(new[] { target }, blockDescriptors);
        }
    }
}

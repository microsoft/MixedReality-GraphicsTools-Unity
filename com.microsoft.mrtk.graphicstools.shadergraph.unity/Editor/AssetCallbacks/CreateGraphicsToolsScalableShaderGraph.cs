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
    /// GraphicsToolsUniversalScalableSubTarget creation.
    /// </summary>
    static class CreateGraphicsToolsScalableShaderGraph
    {
        /// <summary>
        /// Menu item to automatically create a shader graph with the correct sub target.
        /// </summary>
        [MenuItem("Assets/Create/Shader Graph/GraphicsTools/URP/Scalable Shader Graph", priority = CoreUtils.Priorities.assetsCreateShaderMenuPriority)]
        public static void CreateGraphicsToolsScalableGraph()
        {
            var target = (UniversalTarget)Activator.CreateInstance(typeof(UniversalTarget));
            target.TrySetActiveSubTarget(typeof(GraphicsToolsUniversalScalableSubTarget));

            var blockDescriptors = new[]
            {
                BlockFields.VertexDescription.Position,
                BlockFields.VertexDescription.Normal,
                BlockFields.VertexDescription.Tangent,
                BlockFields.SurfaceDescription.BaseColor,
                BlockFields.SurfaceDescription.NormalTS,
                BlockFields.SurfaceDescription.Metallic,
                BlockFields.SurfaceDescription.Smoothness,
                BlockFields.SurfaceDescription.Emission,
                BlockFields.SurfaceDescription.Occlusion,
            };

            GraphUtil.CreateNewGraphWithOutputs(new[] { target }, blockDescriptors);
        }
    }
}

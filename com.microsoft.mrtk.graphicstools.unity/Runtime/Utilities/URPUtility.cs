// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// URP utility class for commonly used constants, types and convenience methods.
    /// </summary>
    public static class URPUtility
    {
        /// <summary>
        /// The universal render pipeline can have multiple renders, this method returns the 
        /// ScriptableRendererData (features and settings) for a renderer at a given index.
        /// 
        /// Note, this data is not public so our only resort is brittle reflection to access 
        /// the data programmatically.
        /// </summary>
#if UNITY_2021_2_OR_NEWER
        public static UniversalRendererData GetRendererData(int rendererIndex)
#else
        public static ForwardRendererData GetRendererData(int rendererIndex)
#endif
        {
            var pipeline = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            if (pipeline != null)
            {
                var propertyInfo = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);

                if (propertyInfo != null)
                {
                    var renderers = propertyInfo.GetValue(pipeline) as ScriptableRendererData[];

                    if (renderers != null)
                    {
                        if (rendererIndex < renderers.Length)
                        {
#if UNITY_2021_2_OR_NEWER
                            return renderers[rendererIndex] as UniversalRendererData;
#else
                            return renderers[rendererIndex] as ForwardRendererData;
#endif
                        }
                        else
                        {
                            Debug.LogError($"GetRendererData failed because rendererIndex is out of range. {renderers.Length} renderer(s) exist but index {rendererIndex} requested.");
                        }
                    }
                    else
                    {
                        Debug.LogError("GetRendererData failed because Unity changed the internals of m_RendererDataList. Please file a bug!");
                    }
                }
                else
                {
                    Debug.LogError("GetRendererData failed because Unity changed the internals of UniversalRenderPipelineAsset. Please file a bug!");
                }
            }
            else
            {
                Debug.LogWarning("GetRendererData failed because the current pipeline is not set or not a UniversalRenderPipelineAsset");
            }

            return null;
        }
    }
}
#endif // GT_USE_URP

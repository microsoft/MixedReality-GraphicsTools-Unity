// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Microsoft.MixedReality.GraphicsTools
{
   /// <summary>
   /// Manages creating and updating render features necessary for the magnification effect on the scriptable render pipeline that in use .
   /// </summary>
    public class MagnifierManager : MonoBehaviour
    {
        [SerializeField]
        private DrawFullscreenFeature.Settings drawFullscreenSettings = new DrawFullscreenFeature.Settings()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents,
            SourceType = BufferType.CameraColor,
            DestinationType = BufferType.Custom,
            SourceTextureId = string.Empty,
            DestinationTextureId = "MagnifierTexture",
        };

        [SerializeField, Header("Render Objects Settings")]
        private RenderObjects.RenderObjectsSettings renderObjectsSettings = new RenderObjects.RenderObjectsSettings()
        {
            Event = RenderPassEvent.AfterRenderingTransparents,
            filterSettings = new RenderObjects.FilterSettings()
            {
                RenderQueueType = RenderQueueType.Transparent
            }
        };

        [Tooltip("Which renderer to use in the UniversalRenderPipelineAsset.")]
        [SerializeField]
        private int rendererIndex = 0;

        private DrawFullscreenFeature magnifierFeature;
        private ScriptableRendererFeature renderTransparent;
        private bool initialized = false;

#if UNITY_2021_2_OR_NEWER
        private UniversalRendererData rendererData = null;
#else
        private ForwardRendererData rendererData = null;
#endif
        private LayerMask previousOpaqueLayerMask;
        private LayerMask previousTransparentLayerMask;

        private void OnEnable()
        {
            if (!initialized)
            {
                InitializeRendererData();

                if (rendererData != null)
                {
                    CreateRendererFeatures();
                    initialized = true;
                }
            }
        }

        private void OnDisable()
        {
            if (initialized)
            {
                if (magnifierFeature != null)
                {
                    rendererData.rendererFeatures.Remove(magnifierFeature);
                }

                if (renderTransparent != null)
                {
                    rendererData.rendererFeatures.Remove(renderTransparent);
                }

                // Reset the layer masks.
                rendererData.opaqueLayerMask = previousOpaqueLayerMask;
                rendererData.transparentLayerMask = previousTransparentLayerMask;

                rendererData.SetDirty();

                initialized = false;
            }
        }

        /// <summary>
        /// Method <c>InitializeRendererData</c> gets the selected scriptable render pipeline thats currently in use.
        /// </summary>
        private void InitializeRendererData()
        {
            var pipeline = ((UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline);

            if (pipeline == null)
            {
                Debug.LogWarning("Universal Render Pipeline not found");
            }
            else
            {
                FieldInfo propertyInfo = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
#if UNITY_2021_2_OR_NEWER
                rendererData = ((ScriptableRendererData[])propertyInfo?.GetValue(pipeline))?[rendererIndex] as UniversalRendererData;
#else
                rendererData = ((ScriptableRendererData[])propertyInfo?.GetValue(pipeline))?[rendererIndex] as ForwardRendererData;
#endif
            }
        }

        /// <summary>
        /// Method <c>CreateRendererFeatures</c> creates renderer features and adds them to a list of features to be deployed on the scriptable render pipeline.
        /// </summary>
        private void CreateRendererFeatures()
        {
            magnifierFeature = CreateMagnifierFullsreenFeature("Magnifier Draw Fullscreen Feature", drawFullscreenSettings);
            rendererData.rendererFeatures.Add(magnifierFeature);
            renderTransparent = CreateMagnifierRenderObjectsFeature("Magnifier Render Objects", renderObjectsSettings);
            rendererData.rendererFeatures.Add(renderTransparent);

            // Don't render the layers rendered by the RenderObjectsFeature
            previousOpaqueLayerMask = rendererData.opaqueLayerMask;
            previousTransparentLayerMask = rendererData.transparentLayerMask;
            rendererData.opaqueLayerMask &= ~renderObjectsSettings.filterSettings.LayerMask;
            rendererData.transparentLayerMask &= ~renderObjectsSettings.filterSettings.LayerMask;

            rendererData.SetDirty();
        }

        /// <summary>
        /// Method <c>CreateMagnifierFullsreenFeature</c> creates an instance of the draw fullscreen renderer feature.
        /// </summary>
        private DrawFullscreenFeature CreateMagnifierFullsreenFeature(string name, DrawFullscreenFeature.Settings settings)
        {
            DrawFullscreenFeature feature = ScriptableObject.CreateInstance<DrawFullscreenFeature>();
            feature.name = name;
            feature.settings = settings;

            return feature;
        }

        /// <summary>
        /// Method <c>CreateRenderObjectsFeature</c> creates an instance of the render objects renderer feature.
        /// </summary>
        private ScriptableRendererFeature CreateMagnifierRenderObjectsFeature(string name, RenderObjects.RenderObjectsSettings settings)
        {
            RenderObjects feature = ScriptableObject.CreateInstance<RenderObjects>();
            feature.name = name;
            feature.settings = settings;

            return feature;
        }
    }
}
#endif // GT_USE_URP

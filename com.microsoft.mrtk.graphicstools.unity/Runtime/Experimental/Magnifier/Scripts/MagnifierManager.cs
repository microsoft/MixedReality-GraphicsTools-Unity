// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Manages creating and updating render features necessary for the magnification effect on the scriptable render pipeline.
    /// </summary>
    public class MagnifierManager : MonoBehaviour
    {
        [Tooltip("Scaler of the magnification.")]
        [SerializeField, Range(0.0f, 1.0f)]
        private float magnification = 0.7f;

        // Scaler of the magnification.
        public float Magnification
        {
            get => magnification;
            set
            {
                magnification = Mathf.Clamp01(value);
                ApplyMagnification();
            }
        }

        [Tooltip("The name of the global shader property that drives the magnification ammount.")]
        [SerializeField]
        private string magnificationPropertyName = "MagnifierMagnification";

        public string MagnificationPropertyName
        {
            get => magnificationPropertyName;
            set
            {
                magnificationPropertyName = value;
                ApplyMagnification();
            }
        }

        [Tooltip("Which renderer to use in the UniversalRenderPipelineAsset.")]
        [SerializeField]
        private int rendererIndex = 0;

        [Tooltip("Should a DrawFullscreenFeature feature be automatically added?")]
        [SerializeField]
        private bool AutoAddDrawFullscreenFeature = true;

        [Tooltip("Is the magnifier in lens mode? i.e is moving based on the position of the mouse cursor")]
        [SerializeField]
        private bool inLensMode = false;

        public bool InLensMode
        {
            get => inLensMode;
            set
            {
                inLensMode = value;
                ApplyMagnifierCenter();
            }
        }

        [Tooltip("The name of the render feature to add the Draw Fullscreen Feature after (or before) to enforce custom sorting. Adds to the end of the render feature list when empty.")]
        public string targetDrawFullscreenFeatureName = string.Empty;

        public enum AddMode { After, Before }
        [Tooltip("When a Target Draw Fullscreen Feature Name is specified, should it be added before or after the feature in the list?")]
        public AddMode targetDrawFullscreenFeatureAddMode = AddMode.After;

        [SerializeField]
        private DrawFullscreenFeature.Settings drawFullscreenSettings = new DrawFullscreenFeature.Settings()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents,
            SourceType = BufferType.CameraColor,
            DestinationType = BufferType.Custom,
            SourceTextureId = string.Empty,
            DestinationTextureId = "MagnifierTexture",
        };

        [Tooltip("Should a RenderObjects feature be automatically added?")]
        [SerializeField]
        private bool AutoAddRenderObjectsFeature = true;

        [Tooltip("The name of the render feature to add the Draw Objects Feature after (or before) to enforce custom sorting. Adds to the end of the render feature list when empty.")]
        public string targetDrawObjectsFeatureName = string.Empty;

        [Tooltip("When a Target Draw Objects Feature Name is specified, should it be added before or after the feature in the list?")]
        public AddMode targetDrawObjectsFeatureAddMode = AddMode.After;

        private static readonly int magnifierCenterID = Shader.PropertyToID("MagnifierCenter");

        [SerializeField]
        private RenderObjects.RenderObjectsSettings renderObjectsSettings = new RenderObjects.RenderObjectsSettings()
        {
            Event = RenderPassEvent.AfterRenderingTransparents,
            filterSettings = new RenderObjects.FilterSettings()
            {
                RenderQueueType = RenderQueueType.Transparent
            }
        };

        [SerializeField, HideInInspector]
        private Material defaultBlitMaterial;

        private DrawFullscreenFeature drawFullscreenFeature;
        private ScriptableRendererFeature renderObjectsFeature;
        private bool initialized = false;

#if UNITY_2021_2_OR_NEWER
        private UniversalRendererData rendererData = null;
#else
        private ForwardRendererData rendererData = null;
#endif
        private LayerMask previousOpaqueLayerMask;
        private LayerMask previousTransparentLayerMask;
        private IntermediateTextureMode previousIntermediateTextureMode;

        private void Reset()
        {
            if (drawFullscreenSettings.BlitMaterial == null)
            {
                drawFullscreenSettings.BlitMaterial = defaultBlitMaterial;
            }
        }

        private void OnEnable()
        {
            if (!initialized)
            {
                rendererData = URPUtility.GetRendererData(rendererIndex);

                if (rendererData != null)
                {
                    // Previously, URP would force rendering to go through an intermediate renderer if the Renderer had any Renderer Features active. On some platforms, this has
                    // significant performance implications so we only want to enable this when we need it (which we do for magnification).
                    previousIntermediateTextureMode = rendererData.intermediateTextureMode;
                    rendererData.intermediateTextureMode = IntermediateTextureMode.Always;

                    CreateRendererFeatures();

                    initialized = true;
                }

                ApplyMagnification();
                ApplyMagnifierCenter();
            }
        }

        private void OnDisable()
        {
            if (initialized)
            {
                if (drawFullscreenFeature != null)
                {
                    rendererData.rendererFeatures.Remove(drawFullscreenFeature);
                    drawFullscreenFeature = null;
                }

                if (renderObjectsFeature != null)
                {
                    rendererData.rendererFeatures.Remove(renderObjectsFeature);
                    renderObjectsFeature = null;
                }

                // Reset the layer masks.
                if (AutoAddRenderObjectsFeature)
                {
                    rendererData.opaqueLayerMask = previousOpaqueLayerMask;
                    rendererData.transparentLayerMask = previousTransparentLayerMask;
                }

                // Reset the intermediate texture mode.
                rendererData.intermediateTextureMode = previousIntermediateTextureMode;

                rendererData.SetDirty();

                initialized = false;
            }
        }

        public void ApplyMagnification()
        {
            Shader.SetGlobalFloat(MagnificationPropertyName, 1.0f - Magnification);
        }

        private void ApplyMagnifierCenter()
        {
            if (inLensMode && Camera.main != null)
            {
                Vector3 cursorPosition;
#if USE_INPUT_SYSTEM
                Vector2 position2D = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
                cursorPosition = new Vector3(position2D.x, position2D.y, 0.0f);
#else
                cursorPosition = Input.mousePosition;
#endif // USE_INPUT_SYSTEM

                Vector3 viewportPostion = Camera.main.ScreenToViewportPoint(cursorPosition);
                Vector4 viewportPostion4 = new Vector4(viewportPostion.x, viewportPostion.y, 0, 0);
                Shader.SetGlobalVector(magnifierCenterID, viewportPostion4);
            }
            else
            {
                Shader.SetGlobalVector(magnifierCenterID, new Vector4(0.5f, 0.5f, 0, 0));
            }
        }

        private void Update()
        {
            if (InLensMode)
            {
                ApplyMagnifierCenter();
            }
        }

        private void CreateRendererFeatures()
        {
            if (AutoAddDrawFullscreenFeature)
            {
                drawFullscreenFeature = CreateMagnifierFullsreenFeature("Magnifier Draw Fullscreen", drawFullscreenSettings);
                InsertFeature(rendererData.rendererFeatures, drawFullscreenFeature, targetDrawFullscreenFeatureName, targetDrawFullscreenFeatureAddMode);
            }

            if (AutoAddRenderObjectsFeature)
            {
                renderObjectsFeature = CreateMagnifierRenderObjectsFeature("Magnifier Render Objects", renderObjectsSettings);
                InsertFeature(rendererData.rendererFeatures, renderObjectsFeature, targetDrawObjectsFeatureName, targetDrawObjectsFeatureAddMode);

                //Don't render the layers rendered by the RenderObjectsFeature
                previousOpaqueLayerMask = rendererData.opaqueLayerMask;
                previousTransparentLayerMask = rendererData.transparentLayerMask;
                rendererData.opaqueLayerMask &= ~renderObjectsSettings.filterSettings.LayerMask;
                rendererData.transparentLayerMask &= ~renderObjectsSettings.filterSettings.LayerMask;
            }

            rendererData.SetDirty();
        }

        private static DrawFullscreenFeature CreateMagnifierFullsreenFeature(string name, DrawFullscreenFeature.Settings settings)
        {
            DrawFullscreenFeature feature = ScriptableObject.CreateInstance<DrawFullscreenFeature>();
            feature.name = name;
            feature.settings = settings;

            return feature;
        }

        private static ScriptableRendererFeature CreateMagnifierRenderObjectsFeature(string name, RenderObjects.RenderObjectsSettings settings)
        {
            RenderObjects feature = ScriptableObject.CreateInstance<RenderObjects>();
            feature.name = name;
            feature.settings = settings;

            return feature;
        }

        private static void InsertFeature(List<ScriptableRendererFeature> features, ScriptableRendererFeature feature, string targetName, AddMode mode)
        {
            int insertIndex = -1;

            if (!string.IsNullOrEmpty(targetName))
            {
                insertIndex = features.FindIndex(x => x.name == targetName);
            }

            if (insertIndex == -1)
            {
                insertIndex = features.Count - 1;
            }

            if (mode == AddMode.After)
            {
                ++insertIndex;
            }

            features.Insert(insertIndex, feature);
        }
    }
}
#endif // GT_USE_URP

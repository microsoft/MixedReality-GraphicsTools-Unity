// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Does the buffer come from the camera or custom type.
    /// </summary>
    public enum BufferType
    {
        CameraColor,
        Custom 
    }

    /// <summary>
    /// Forked from: https://github.com/Unity-Technologies/UniversalRenderingExamples/tree/master/Assets/Scripts/Runtime/RenderPasses
    /// Performs a fullscreen blit via a custom render feature.
    /// </summary>
    public class DrawFullscreenFeature : ScriptableRendererFeature
    {
        /// <summary>
        /// Render feature configuration settings.
        /// </summary>
        [System.Serializable]
        public class Settings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            public Material BlitMaterial = null;
            public string BlitSourceTextureName = "_SourceTex";
            public int BlitMaterialPassIndex = -1;
            public BufferType SourceType = BufferType.CameraColor;
            public BufferType DestinationType = BufferType.CameraColor;
            public string SourceTextureId = "_SourceTexture";
            public string DestinationTextureId = "_DestinationTexture";
            public FilterMode FilterMode = FilterMode.Point;
            public bool RestoreCameraColorTarget = true;
        }

        /// <summary>
        /// Render feature configuration settings.
        /// </summary>
        public Settings settings = new Settings();

        private DrawFullscreenPass blitPass;

        /// <inheritdoc/>
        public override void Create()
        {
            blitPass = new DrawFullscreenPass(name);
        }

        /// <inheritdoc/>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.BlitMaterial == null)
            {
                Debug.LogWarningFormat($"{nameof(DrawFullscreenFeature)} is missing a blit material and will not be queued.");
                return;
            }

            blitPass.renderPassEvent = settings.renderPassEvent;
            blitPass.Settings = settings;

            renderer.EnqueuePass(blitPass);
        }
    }
}
#endif // GT_USE_URP

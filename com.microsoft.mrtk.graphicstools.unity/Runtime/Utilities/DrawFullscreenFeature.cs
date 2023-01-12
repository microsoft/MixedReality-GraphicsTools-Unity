// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Microsoft.MixedReality.GraphicsTools
{
    public enum BufferType
    {
        CameraColor,
        Custom 
    }

    /// <summary>
    /// Forked from: https://github.com/Unity-Technologies/UniversalRenderingExamples/tree/master/Assets/Scripts/Runtime/RenderPasses
    /// Performs fullscreen blit via a custom Render Pass
    /// </summary>
    public class DrawFullscreenFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        /// <summary>
        /// Class <c>Settings</c> outlines controls for the Render Feature
        /// </summary>
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
            public bool RestoreCameraColorTarget = true;
        }

        /// <summary>
        /// Defines a new Settings class
        /// </summary>
        public Settings settings = new Settings();
        private DrawFullscreenPass blitPass;

        /// <summary>
        /// Method <c>Create</c> creates the render pass.
        /// </summary>
        public override void Create()
        {
            blitPass = new DrawFullscreenPass(name);
            blitPass.FilterMode = FilterMode.Bilinear;
        }

        /// <summary>
        /// Method <c>AddRenderPasses</c> calls the custom render pass.
        /// </summary>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.BlitMaterial == null)
            {
                Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }

            blitPass.renderPassEvent = settings.renderPassEvent;
            blitPass.Settings = settings;

            // Previously, URP would force rendering to go through an intermediate renderer if the Renderer had any Renderer Features active. On some platforms, this has
            // significant performance implications. Due to that, Renderer Features are now expected to declare their inputs using ScriptableRenderPass.ConfigureInput.
            // This information is used to decide automatically whether rendering via an intermediate texture is necessary.
            if (settings.SourceType == BufferType.CameraColor)
            {
                blitPass.ConfigureInput(ScriptableRenderPassInput.Color);
            }

            renderer.EnqueuePass(blitPass);
        }
    }
}
#endif // GT_USE_URP

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

            public Material blitMaterial = null;
            public string blitSourceTextureName = "_SourceTex";
            public int blitMaterialPassIndex = -1;
            public BufferType sourceType = BufferType.CameraColor;
            public BufferType destinationType = BufferType.CameraColor;
            public string sourceTextureId = "_SourceTexture";
            public string destinationTextureId = "_DestinationTexture";
            public bool restoreCameraColorTarget = true;
        }

        public Settings settings = new Settings();
        private DrawFullscreenPass blitPass;
        /// <summary>
        /// Method <c>Create</c> creates the render pass.
        /// </summary>
        public override void Create()
        {
            blitPass = new DrawFullscreenPass(name);
            blitPass.filterMode = FilterMode.Bilinear;
        }
        /// <summary>
        /// Method <c>AddRenderPasses</c> calls the custom render pass.
        /// </summary>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.blitMaterial == null)
            {
                Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }

            blitPass.renderPassEvent = settings.renderPassEvent;
            blitPass.settings = settings;
            renderer.EnqueuePass(blitPass);
        }
    }
}
#endif // GT_USE_URP

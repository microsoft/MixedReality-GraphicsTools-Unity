// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Scriptable renderer feature that captures the current framebuffer with optional downsampling and blurring
    /// </summary>

    public class AcrylicBlurFeature : ScriptableRendererFeature
    {
        [Experimental]
        [SerializeField]
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        [SerializeField]
        public Material blurMaterial = null;

        [SerializeField]
        [Range(2, 7)]
        public int blur = 5;

        [SerializeField]
        [Range(0, 2)]
        public int downSample = 2;

        [SerializeField]
        public string textureName = "_blurTexture";

        [SerializeField]
        public Camera targetCamera = null;

        [HideInInspector]
        public bool rendered = false;

        AcrylicBlurRenderPass pass;

        private bool setMaterialTexture = false;
        private bool applyBlur = false;
        private RenderTexture providedTexture = null;
        private AcrylicFilterDual blurFilter = null;

        public void SetStorageTexture(RenderTexture texture)
        {
            providedTexture = texture;
        }

        public void SetMaterialTexture(bool on)
        {
            setMaterialTexture = on;
            if (pass != null)
            {
                pass.setMaterialTexture = on;
            }
        }

        public void ApplyBlur(bool on)
        {
            applyBlur = on;
        }

        public void SetBlurMethod(AcrylicFilterDual _blurFilter)
        {
            blurFilter = _blurFilter;
        }

        public override void Create()
        {
            pass = new AcrylicBlurRenderPass(name, downSample, blur, blurMaterial, textureName, applyBlur, providedTexture, blurFilter);
            pass.renderPassEvent = renderPassEvent;
            pass.setMaterialTexture = setMaterialTexture;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (targetCamera == null || renderingData.cameraData.camera == targetCamera)
            {
                pass.Initialize(renderer.cameraColorTarget);
                renderer.EnqueuePass(pass);
                rendered = true;
            }
        }
    }
}
#endif // GT_USE_URP

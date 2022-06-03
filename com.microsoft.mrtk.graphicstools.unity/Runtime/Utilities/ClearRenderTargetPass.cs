// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Microsoft.MixedReality.GraphicsTools
{
    public class ClearRenderTargetPass : ScriptableRenderPass
    {
        private ClearRenderTarget.PassSettings settings;

        /// <summary>
        /// Caches pass settings.
        /// </summary>
        public ClearRenderTargetPass(ClearRenderTarget.PassSettings passSettings)
        {
            settings = passSettings;
            renderPassEvent = settings.RenderPassEvent;
        }

        /// <summary>
        /// Queues a ClearRenderTarget command based on the pass settings.
        /// </summary>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, new ProfilingSampler("Clear Render Target Pass")))
            {
                cmd.ClearRenderTarget(clearDepth: settings.ClearDepth, clearColor: settings.ClearColor, backgroundColor: settings.BackgroundColor);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
#endif // GT_USE_URP

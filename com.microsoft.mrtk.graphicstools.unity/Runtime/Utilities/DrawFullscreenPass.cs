// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Draws full screen mesh using given material and pass and reading from source target.
    /// Forked from: https://github.com/Unity-Technologies/UniversalRenderingExamples/tree/master/Assets/Scripts/Runtime/RenderPasses
    /// </summary>
    class DrawFullscreenPass : ScriptableRenderPass
    { 
        ///<summary>
        /// Pass configuration settings.
        ///</summary>
        public DrawFullscreenFeature.Settings Settings;

#if UNITY_2022_1_OR_NEWER
        private RTHandle source;
        private RTHandle destination;

        private bool isSourceAndDestinationSameTarget;
        private string profilerTag;

        ///<summary>
        /// Constructor.
        ///</summary>
        public DrawFullscreenPass(string tag)
        {
            profilerTag = tag;
        }

        /// <inheritdoc/>
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            blitTargetDescriptor.depthBufferBits = 0;

            isSourceAndDestinationSameTarget = Settings.SourceType == Settings.DestinationType &&
                (Settings.SourceType == BufferType.CameraColor || Settings.SourceTextureId == Settings.DestinationTextureId);

            if (Settings.SourceType == BufferType.CameraColor)
            {
                source = renderingData.cameraData.renderer.cameraColorTargetHandle;
            }
            else
            {
                RenderingUtils.ReAllocateIfNeeded(ref source, blitTargetDescriptor, Settings.FilterMode,
                                                  TextureWrapMode.Clamp, name: Settings.SourceTextureId);
            }

            if (isSourceAndDestinationSameTarget)
            {
                RenderingUtils.ReAllocateIfNeeded(ref destination, blitTargetDescriptor, Settings.FilterMode,
                                                  TextureWrapMode.Clamp, name: "_TempRT");
            }
            else if (Settings.DestinationType == BufferType.CameraColor)
            {
                destination = renderingData.cameraData.renderer.cameraColorTargetHandle;
            }
            else
            {
                RenderingUtils.ReAllocateIfNeeded(ref destination, blitTargetDescriptor, Settings.FilterMode,
                                                  TextureWrapMode.Clamp, name: Settings.DestinationTextureId);
            }
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            // Can't read and write to same color target, create a temp render target to blit.
            if (isSourceAndDestinationSameTarget)
            {
                Blitter.BlitCameraTexture(cmd, source, destination, Settings.BlitMaterial, Settings.BlitMaterialPassIndex);
                Blitter.BlitCameraTexture(cmd, destination, source, Settings.BlitMaterial, 0);
            }
            else
            {
                Blitter.BlitCameraTexture(cmd, source, destination, Settings.BlitMaterial, Settings.BlitMaterialPassIndex);
            }

            cmd.SetGlobalTexture(Settings.DestinationTextureId, destination);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// <summary>
        /// Unsure when this is called? This is the only resource I could find to dispose of RTHandles.
        /// https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/upgrade-guide-2022-1.html
        /// </summary>
        private void Dispose()
        {
            source?.Release();
            destination?.Release();
        }
#else
        private RenderTargetIdentifier source;
        private RenderTargetIdentifier destination;
        private RenderTargetIdentifier cameraColorTarget;
        private int temporaryRTId = Shader.PropertyToID("_TempRT");

        private int sourceId;
        private int destinationId;
        private bool isSourceAndDestinationSameTarget;
        private string profilerTag;

        ///<summary>
        /// Constructor.
        ///</summary>
        public DrawFullscreenPass(string tag)
        {
            profilerTag = tag;
        }

        /// <inheritdoc/>
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            blitTargetDescriptor.depthBufferBits = 0;

            isSourceAndDestinationSameTarget = Settings.SourceType == Settings.DestinationType &&
                (Settings.SourceType == BufferType.CameraColor || Settings.SourceTextureId == Settings.DestinationTextureId);

            var renderer = renderingData.cameraData.renderer;

            if (Settings.SourceType == BufferType.CameraColor)
            {
                sourceId = -1;
                source = renderer.cameraColorTarget;
            }
            else
            {
                sourceId = Shader.PropertyToID(Settings.SourceTextureId);
                cmd.GetTemporaryRT(sourceId, blitTargetDescriptor, Settings.FilterMode);
                source = new RenderTargetIdentifier(sourceId);
            }

            if (isSourceAndDestinationSameTarget)
            {
                destinationId = temporaryRTId;
                cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, Settings.FilterMode);
                destination = new RenderTargetIdentifier(destinationId);
            }
            else if (Settings.DestinationType == BufferType.CameraColor)
            {
                destinationId = -1;
                destination = renderer.cameraColorTarget;
            }
            else
            {
                destinationId = Shader.PropertyToID(Settings.DestinationTextureId);
                cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, Settings.FilterMode);
                destination = new RenderTargetIdentifier(destinationId);
            }

            cameraColorTarget = renderer.cameraColorTarget;
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            bool useDrawProceduleBlit = renderingData.cameraData.xrRendering;

            // Can't read and write to same color target, create a temp render target to blit.
            if (isSourceAndDestinationSameTarget)
            {
                Blit(cmd, source, destination, Settings.BlitMaterial, Settings.BlitMaterialPassIndex, useDrawProceduleBlit);
                Blit(cmd, destination, source, Settings.BlitMaterial, 0, useDrawProceduleBlit);
            }
            else
            {
                Blit(cmd, source, destination, Settings.BlitMaterial, Settings.BlitMaterialPassIndex, useDrawProceduleBlit);
            }

            if (Settings.RestoreCameraColorTarget)
            {
                cmd.SetRenderTarget(cameraColorTarget);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// <inheritdoc/>
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (destinationId != -1)
                cmd.ReleaseTemporaryRT(destinationId);

            if (source == destination && sourceId != -1)
                cmd.ReleaseTemporaryRT(sourceId);
        }

        private struct ShaderPropertyId
        {
            public static readonly int sourceTex = Shader.PropertyToID("_SourceTex");
            public static readonly int scaleBias = Shader.PropertyToID("_ScaleBias");
            public static readonly int scaleBiasRt = Shader.PropertyToID("_ScaleBiasRt");
        }

        /// <summary>
        /// Fork of the internal UnityEngine.Rendering.Universal.RenderingUtils.Blit method.
        /// </summary>
        private static void Blit(CommandBuffer cmd,
                         RenderTargetIdentifier source,
                         RenderTargetIdentifier destination,
                         Material material,
                         int passIndex = 0,
                         bool useDrawProcedural = false,
                         RenderBufferLoadAction colorLoadAction = RenderBufferLoadAction.Load,
                         RenderBufferStoreAction colorStoreAction = RenderBufferStoreAction.Store,
                         RenderBufferLoadAction depthLoadAction = RenderBufferLoadAction.Load,
                         RenderBufferStoreAction depthStoreAction = RenderBufferStoreAction.Store)
        {
            cmd.SetGlobalTexture(ShaderPropertyId.sourceTex, source);
            if (useDrawProcedural)
            {
                Vector4 scaleBias = new Vector4(1, 1, 0, 0);
                Vector4 scaleBiasRt = new Vector4(1, 1, 0, 0);
                cmd.SetGlobalVector(ShaderPropertyId.scaleBias, scaleBias);
                cmd.SetGlobalVector(ShaderPropertyId.scaleBiasRt, scaleBiasRt);
                cmd.SetRenderTarget(new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1),
                    colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction);
                cmd.DrawProcedural(Matrix4x4.identity, material, passIndex, MeshTopology.Quads, 4, 1, null);
            }
            else
            {
                cmd.SetRenderTarget(destination, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction);
                cmd.Blit(source, BuiltinRenderTextureType.CurrentActive, material, passIndex);
            }
        }
#endif // UNITY_2022_1_OR_NEWER
    }
}
#endif // GT_USE_URP

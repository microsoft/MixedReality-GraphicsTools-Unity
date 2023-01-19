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
        ///Declares a filtering mode enum for  the source and destination render textures during blit
        ///</summary>
        public FilterMode FilterMode { get; set; }

        ///<summary>
        ///A set of outlined controls for performing a fullscreen blit via this render pass
        ///</summary>
        public DrawFullscreenFeature.Settings Settings;

        private RenderTargetIdentifier source;
        private RenderTargetIdentifier destination;
        private RenderTargetIdentifier cameraColorTarget;
        private int temporaryRTId = Shader.PropertyToID("_TempRT");

        private int sourceId;
        private int destinationId;
        private bool isSourceAndDestinationSameTarget;
        private string profilerTag;

        ///<summary>
        /// Assigns tag to the CMD buffer for this render pass
        ///</summary>
        public DrawFullscreenPass(string tag)
        {
            profilerTag = tag;
        }

        ///<summary>
        /// Extracts the camera's view as a render texture in order for it to be assigned to the material of the fullscreen mesh
        ///</summary>
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
                cmd.GetTemporaryRT(sourceId, blitTargetDescriptor, FilterMode);
                source = new RenderTargetIdentifier(sourceId);
            }

            if (isSourceAndDestinationSameTarget)
            {
                destinationId = temporaryRTId;
                cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, FilterMode);
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
                cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, FilterMode);
                destination = new RenderTargetIdentifier(destinationId);
            }

            cameraColorTarget = renderer.cameraColorTarget;
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            bool isXR = renderingData.cameraData.xrRendering;

            // Can't read and write to same color target, create a temp render target to blit.
            if (isSourceAndDestinationSameTarget)
            {
                Blit(cmd, source, destination, Settings.BlitMaterial, Settings.BlitMaterialPassIndex, isXR);
                Blit(cmd, destination, source, Settings.BlitMaterial, 0, isXR);
            }
            else
            {
                Blit(cmd, source, destination, Settings.BlitMaterial, Settings.BlitMaterialPassIndex, isXR);
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

        // URP Blit() doesn't currently work with multiview.
        private void Blit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, Material material, int pass, bool isXR)
        {
            if (isXR)
            {
                Vector4 scaleBias = new Vector4(1, 1, 0, 0);
                Vector4 scaleBiasRt = new Vector4(1, 1, 0, 0);
                cmd.SetGlobalVector("_ScaleBias", scaleBias);
                cmd.SetGlobalVector("_ScaleBiasRt", scaleBiasRt);
                cmd.SetRenderTarget(target);
                cmd.DrawProcedural(Matrix4x4.identity, material, pass, MeshTopology.Quads, 4, 1, null);
            }
            else
            {
                cmd.SetRenderTarget(target);
                cmd.Blit(source, BuiltinRenderTextureType.CurrentActive, material, pass);
            }
        }
    }
}
#endif // GT_USE_URP

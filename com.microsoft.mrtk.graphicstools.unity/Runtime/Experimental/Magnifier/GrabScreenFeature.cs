using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace Microsoft.MixedReality.GraphicsTools
{
    [ExecuteInEditMode]
    public class GrabScreenFeature : ScriptableRendererFeature
    {
       
        [System.Serializable]
        public class Settings
        { 
            public Material shaderM;
            public string TextureName = "_GrabPassTransparent";
            public LayerMask LayerMask;
        }
        class GrabPass : ScriptableRenderPass
        {
            RenderTargetHandle tempColorTarget;
            Settings settings;
            RenderTargetIdentifier cameraTarget;

            public GrabPass(Settings s)
            {
                settings = s;
             
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
                tempColorTarget.Init(settings.TextureName);
                
            }

            public void Setup(RenderTargetIdentifier cameraTarget)
            {
                this.cameraTarget = cameraTarget;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                cmd.GetTemporaryRT(tempColorTarget.id, cameraTextureDescriptor);
                cmd.SetGlobalTexture(settings.TextureName, tempColorTarget.Identifier());
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
              
                CommandBuffer cmd = CommandBufferPool.Get();
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                
             //Chuks: old blit method 
             Blit(cmd, cameraTarget, tempColorTarget.Identifier());
               // LocalBlit(cmd, cameraTarget, tempColorTarget.Identifier(), settings.shaderM);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            private void LocalBlit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, Material material =null)
            {
               cmd.SetRenderTarget(target);
               
                cmd.SetGlobalTexture(settings.TextureName, source);
                cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, material);
            }
            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(tempColorTarget.id);
            }
        }

        class RenderPass : ScriptableRenderPass
        {
            Settings settings;
            List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

            FilteringSettings m_FilteringSettings;
            RenderStateBlock m_RenderStateBlock;

            public RenderPass(Settings settings)
            {
                this.settings = settings;
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents + 1;

                m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
                m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));

                m_FilteringSettings = new FilteringSettings(RenderQueueRange.all, settings.LayerMask);
                m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get();

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                DrawingSettings drawSettings;
                drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, SortingCriteria.CommonTransparent);
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings, ref m_RenderStateBlock);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }

        GrabPass grabPass;
        RenderPass renderPass;
        [SerializeField] Settings settings;

        public override void Create()
        {
           
            grabPass = new GrabPass(settings);
            renderPass = new RenderPass(settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            grabPass.Setup(renderer.cameraColorTarget);

            renderer.EnqueuePass(grabPass);
            renderer.EnqueuePass(renderPass);
        }
    }

}

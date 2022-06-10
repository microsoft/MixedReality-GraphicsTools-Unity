
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MagnifyBlitRendererFeature : ScriptableRendererFeature
{
    public Shader m_Shader;
    Material m_Material;

   MagnifyBlitPass m_RenderPass = null;
 
	public float m_Amount = 0.33f;
	public float m_RadiusX = 0.66f ;
	public float m_RadiusY = 0.5f;
	public float m_RadiusInner = 1f;
	public float m_RadiusOuter =0.6f;
	public float m_PosX = 0.5f;
	public float m_PosY = 0.5f;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            //Calling ConfigureInput with the ScriptableRenderPassInput.Color argument ensures that the opaque texture is available to the Render Pass
            m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            m_RenderPass.SetTarget(renderer.cameraColorTarget, m_Amount);
            renderer.EnqueuePass(m_RenderPass);
        }
    }

    public override void Create()
    {
        if (m_Shader != null)
            m_Material = new Material(m_Shader);

        m_RenderPass = new MagnifyBlitPass(m_Material,m_Amount,m_RadiusInner,m_RadiusOuter,m_RadiusY,m_RadiusX,m_PosY,m_PosX);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Material);
    }
}


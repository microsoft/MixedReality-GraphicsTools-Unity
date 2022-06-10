using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MagnifyBlitPass : ScriptableRenderPass
{    static readonly int TempTargetId = Shader.PropertyToID("Magnifying Glass/Circle");
   
    ProfilingSampler m_ProfilingSampler = new ProfilingSampler("Magnifying Glass/Circle");
    Material m_Material;
    RenderTargetIdentifier m_CameraColorTarget;
    float m_Intensity;
    private float m_Amount;
	private float m_RadiusX;
	private float m_RadiusY;
	private float m_RadiusInner;
	private float m_RadiusOuter;
	private float m_PosX;
	private float m_PosY;
  
    public MagnifyBlitPass(Material material,float Amount,float RadiusInner,float RadiusOuter,float RadiusY,float RadiusX, float PosY, float PosX)
    {
        m_Material = material;
        m_Amount = Amount;
        m_RadiusInner =RadiusInner;
        m_RadiusOuter =RadiusOuter;
        m_RadiusX = RadiusX;
        m_RadiusY = RadiusY;
        m_PosX = PosX;
        m_PosY =PosY;
        string Amountstr = "Amount";
		m_Material.SetFloat (Amountstr, m_Amount);
		string CenterRadial = "CenterRadial";
		m_Material.SetVector (CenterRadial, new Vector4 (m_PosX, m_PosY, m_RadiusX, m_RadiusY));
		string RadiusInnerstr = "RadiusInner";
		m_Material.SetFloat (RadiusInnerstr, m_RadiusInner);
		string RadiusOuterstr = "RadiusOuter";
		m_Material.SetFloat (RadiusOuterstr, m_RadiusOuter);
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public void SetTarget(RenderTargetIdentifier colorHandle, float intensity)
    {
        m_CameraColorTarget = colorHandle;
        m_Intensity = intensity;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ConfigureTarget(new RenderTargetIdentifier(m_CameraColorTarget, 0, CubemapFace.Unknown, -1));
    }

   public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
	{
		
		
		/*string Amount = "Amount";
		m_Material.SetFloat (Amount, m_Amount);
		string CenterRadial = "CenterRadial";
		m_Material.SetVector (CenterRadial, new Vector4 (m_PosX, m_PosY, m_RadiusX, m_RadiusY));
		string RadiusInner = "RadiusInner";
		m_Material.SetFloat (RadiusInner, m_RadiusInner);
		string RadiusOuter = "RadiusOuter";
		m_Material.SetFloat (RadiusOuter, m_RadiusOuter);*/

	//	Graphics.Blit (sourceTexture, destTexture, m_Material, pass);
	}
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var camera = renderingData.cameraData.camera;
        if (camera.cameraType != CameraType.Game)
            return;

        if (m_Material == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get();
        Render(cmd,ref renderingData );
        using (new ProfilingScope(cmd, m_ProfilingSampler))
        {
            m_Material.SetFloat("_Intensity", m_Intensity);
            cmd.SetRenderTarget(new RenderTargetIdentifier(m_CameraColorTarget, 0, CubemapFace.Unknown, -1));
            //The RenderingUtils.fullscreenMesh argument specifies that the mesh to draw is a quad.
           // cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m_Material);
        }
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }
     void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        CameraData cameraData = renderingData.cameraData;
        RenderTargetIdentifier source = m_CameraColorTarget;
        int destination = TempTargetId;
        int shaderPass = 0;
   
        int w = cameraData.camera.scaledPixelWidth >> 3;
        int h = cameraData.camera.scaledPixelHeight >> 3;
   
        cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
        //cmd.Blit(source, destination, blitRenderMaterial, shaderPass);
        cmd.Blit(source, source, m_Material, shaderPass);
   
        cmd.ReleaseTemporaryRT(destination);
    }
}

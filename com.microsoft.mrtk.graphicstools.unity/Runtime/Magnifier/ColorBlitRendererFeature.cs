
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class ColorBlitRendererFeature : ScriptableRendererFeature
{
    public Shader m_Shader;
    public float m_Intensity;

    Material m_Material;

    ColorBlitPass m_RenderPass = null;


    public float m_InitialAmount = 0.5f;
	public float m_InitialRadiusX = 0.1f;
	public float m_InitialRadiusY = 0.1f;
	public float m_InitialRadiusInner = 0.3f;
	public float m_InitialRadiusOuter = 0.6f;
	private float m_Amount;
	private float m_RadiusX ;
	private float m_RadiusY;
	private float m_RadiusInner;
	private float m_RadiusOuter;
	private float m_MouseX = 0f;
	private float m_MouseY = 0f;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            //Calling ConfigureInput with the ScriptableRenderPassInput.Color argument ensures that the opaque texture is available to the Render Pass
            m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            m_RenderPass.SetTarget(renderer.cameraColorTarget, m_Intensity);
            renderer.EnqueuePass(m_RenderPass);
        }
    }
void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		int pass = 0;
		
		string Amount = "Amount";
		m_Material.SetFloat (Amount, m_Amount);
		string CenterRadial = "CenterRadial";
		m_Material.SetVector (CenterRadial, new Vector4 (m_MouseX, m_MouseY, m_RadiusX, m_RadiusY));
		string RadiusInner = "RadiusInner";
		m_Material.SetFloat (RadiusInner, m_RadiusInner);
		string RadiusOuter = "RadiusOuter";
		m_Material.SetFloat (RadiusOuter, m_RadiusOuter);

		Graphics.Blit (sourceTexture, destTexture, m_Material, pass);
	}
    public override void Create()
    {
        if (m_Shader != null)
            m_Material = new Material(m_Shader);

        m_RenderPass = new ColorBlitPass(m_Material);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Material);
    }
}


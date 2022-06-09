using UnityEngine;

public class MagnifyingGlass : MonoBehaviour
{
	public Material m_Mat = null;
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
	//private bool m_TraceMouse = false;
	private Rect[] m_GUIRects = new Rect[9];
	private bool m_InvertScale = false;
	private int m_GlassIndex = 0;
	
	void ResetData ()
	{
		// reset values
		m_Amount = m_InitialAmount;
			m_RadiusX = m_InitialRadiusX;
			m_RadiusY = m_InitialRadiusY;
			m_RadiusInner = m_InitialRadiusInner;
			m_RadiusOuter = m_InitialRadiusOuter;
		// reset material parameters
	
		m_Mat.SetVector ("CenterRadial", new Vector4 (0.3f, 0.2f, m_RadiusX, m_RadiusY));
		
		m_Mat.SetFloat ("Amount", m_Amount);
	
		m_Mat.SetFloat ("RadiusInner", m_RadiusInner);
		
		m_Mat.SetFloat ("RadiusOuter", m_RadiusOuter);
		
	}
	void Start ()
	{
		
		QualitySettings.antiAliasing = 8;
		
		ResetData ();
		m_GUIRects[0] = new Rect (10, 45, 150, 25);
		m_GUIRects[1] = new Rect (10, 70, 150, 25);
		m_GUIRects[2] = new Rect (10, 100, 200, 25);
		m_GUIRects[3] = new Rect (95, 145, 115, 25);
		m_GUIRects[4] = new Rect (95, 175, 115, 25);
		m_GUIRects[5] = new Rect (95, 205, 115, 25);
		m_GUIRects[6] = new Rect (95, 235, 115, 25);
		m_GUIRects[7] = new Rect (95, 265, 115, 25);
		m_GUIRects[8] = new Rect (10, 295, 150, 25);
		m_MouseX = m_MouseY = 0.5f;
	}
	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		int pass = 0;
	
		int ind = m_GlassIndex;
		
		string Amount = "Amount";
		m_Mat.SetFloat (Amount, m_Amount);
		string CenterRadial = "CenterRadial";
		m_Mat.SetVector (CenterRadial, new Vector4 (m_MouseX, m_MouseY, m_RadiusX, m_RadiusY));
		string RadiusInner = "RadiusInner";
		m_Mat.SetFloat (RadiusInner, m_RadiusInner);
		string RadiusOuter = "RadiusOuter";
		m_Mat.SetFloat (RadiusOuter, m_RadiusOuter);

		Graphics.Blit (sourceTexture, destTexture, m_Mat, pass);
	}
	void Update ()
	{
/*		if (Input.GetMouseButtonDown (0))
		{
			m_TraceMouse = true;
			
			for (int i = 0; i < m_GUIRects.Length; i++)
			{
				if (m_GUIRects[i].Contains (new Vector2 (Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
				{
					m_TraceMouse = false;
					break;
				}
			}
		}
		else if (Input.GetMouseButtonUp (0))
		{
			m_TraceMouse = false;
		}
		else if (Input.GetMouseButton (0))
		{
			if (m_TraceMouse)
			{
				m_MouseX = Input.mousePosition.x / Screen.width;
				// unity anti-alias will flip Y coordinate of uv
//				if (QualitySettings.antiAliasing != 0)
//					m_MouseY = 1f - Input.mousePosition.y / Screen.height;
//				else
					m_MouseY = Input.mousePosition.y / Screen.height;
			}
		}*/
	}
	void OnGUI ()
	{
		int previousFrameGlassIndex = m_GlassIndex;
	
		GUI.Box (new Rect (10, 130, 80, 25), "Amount");
		m_Amount = GUI.HorizontalSlider (m_GUIRects[3], m_Amount, 0f, 1f);
		GUI.Box (new Rect (10, 160, 80, 25), "Radial X");
		m_RadiusX = GUI.HorizontalSlider (m_GUIRects[4], m_RadiusX, 0f, 0.7f);
		GUI.Box (new Rect (10, 190, 80, 25), "Radial Y");
		m_RadiusY = GUI.HorizontalSlider (m_GUIRects[5], m_RadiusY, 0f, 0.7f);
	
			GUI.Box (new Rect (10, 220, 80, 25), "Inner");
			m_RadiusInner = GUI.HorizontalSlider (m_GUIRects[6], m_RadiusInner, 0f, 1f);
			GUI.Box (new Rect (10, 250, 80, 25), "Outer");
			m_RadiusOuter = GUI.HorizontalSlider (m_GUIRects[7], m_RadiusOuter, 0f, 1f);
		
		m_InvertScale = GUI.Toggle (m_GUIRects[8], m_InvertScale, " Invert Scale");
		if (m_InvertScale)
			m_Amount = -m_Amount;
	}
}
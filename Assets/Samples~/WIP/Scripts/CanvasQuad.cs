//
// Copyright (C) Microsoft. All rights reserved.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// NOTE: Uncomment the following line to enable obfuscation
//[System.Reflection.Obfuscation(Exclude = false)]
public class CanvasQuad : Graphic
{
	private Material localMaterial = null;

	private bool animateMaterial = false;

	public bool AnimateMaterial
	{
		get
		{
			return animateMaterial;
		}
		set
		{
			if (value!=animateMaterial)
			{
				animateMaterial = value;
				SetMaterialDirty();
			}
		}
	}

	#region override methods
	protected override void Awake()
	{
		base.Awake();

		var canvas = GetComponentInParent<Canvas>();
		if (canvas != null)
		{
			canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord3;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
		}
	}
	protected override void OnDestroy()
	{
		if (localMaterial!=null)
		{
			Destroy(localMaterial);
			localMaterial = null;
		}
		base.OnDestroy();
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		var uv2 = new Vector2(rectTransform.rect.width * rectTransform.localScale.x,
								rectTransform.rect.height * rectTransform.localScale.y);
        var canvas = GetComponentInParent<Canvas>();
        var uv3 = new Vector2(Mathf.Min(uv2.x, uv2.y), canvas ? -Mathf.Min(canvas.transform.lossyScale.x, canvas.transform.lossyScale.y, canvas.transform.lossyScale.z) : -1.0f);

        vh.Clear();
        AddQuadMesh(vh, rectTransform.rect.min, rectTransform.rect.max, rectTransform.localScale, uv2, uv3);
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		SetVerticesDirty();
		SetMaterialDirty();
	}

	protected override void OnTransformParentChanged()
	{
		base.OnTransformParentChanged();
		SetVerticesDirty();
		SetMaterialDirty();
	}

	public override Material materialForRendering => animateMaterial ? LocalMaterial : material;

	#endregion

	#region private methods

	private Material LocalMaterial
	{
		get
		{
			if (localMaterial == null) localMaterial = Instantiate(material);
			return localMaterial;
		}
	}

	private UIVertex vert = new UIVertex();

	private void AddQuadMesh(VertexHelper vh, Vector2 min, Vector2 max, Vector2 scale, Vector2 uv2, Vector2 uv3)
	{
		vert.uv2 = uv2;
        vert.uv3 = uv3;

		float x1 = min.x;
		float x2 = max.x;
		float y1 = min.y;
		float y2 = max.y;

		float u1 = 0.0f;
		float u2 = 1.0f;

		float v1 = 0.0f;
		float v2 = 1.0f;

		int i11 = AddVertex(vh, x1, y1, u1, v1);
		int i21 = AddVertex(vh, x2, y1, u2, v1);
		int i12 = AddVertex(vh, x1, y2, u1, v2);
		int i22 = AddVertex(vh, x2, y2, u2, v2);

		AddQuad(vh, i11, i21, i12, i22);
	}

	private int AddVertex(VertexHelper vh, float x, float y, float u, float v, Vector3 normal, Vector3 tangent)
	{
		int ix = vh.currentVertCount;

		vert.color = this.color;

		vert.position = new Vector3(x, y, 0.0f);
		vert.uv0 = new Vector2(u, v);
		vert.normal = normal;
		//vert.tangent = tangent;
		vh.AddVert(vert);
		return ix;
	}

	private int AddVertex(VertexHelper vh, float x, float y, float u, float v)
	{
		return AddVertex(vh, x, y, u, v, Vector3.forward, Vector3.forward);
	}

	private void AddQuad(VertexHelper vh, int v00, int v10, int v01, int v11)
	{
		vh.AddTriangle(v00, v11, v10);
		vh.AddTriangle(v00, v01, v11);
	}
	#endregion
}

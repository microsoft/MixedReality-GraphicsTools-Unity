//
// Copyright (C) Microsoft. All rights reserved.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ButtonTest : MonoBehaviour, IPointerEnterHandler,
	IPointerExitHandler

{
	[SerializeField]
	private Graphic backGlow = null;

	[SerializeField]
	private Graphic hoverGraphic = null;

	[SerializeField]
	private float animationDuration = 2.0f;

	[SerializeField]
	private float maxPressDistance = 0.01f;

	[SerializeField]
	private float focusDuration = 0.25f;

	[SerializeField]
	private float hoverDuration = 0.35f;

	[SerializeField]
	private float hoverDistance = 3.0f;

	private MaterialPropertyBlock propertyBlock = null;
	private Graphic frontPlate = null;
	private bool animatePress = false;
	private float motion = 0.0f;
	private float animationProgress = 0.0f;
	private Vector3 originalPosition = Vector3.zero;
	private bool inFocus = false;
	private float focus = 0.0f;
	private float hover = 0.0f;
	private Vector3 hoverOriginalPosition = Vector3.zero;
	private CanvasQuad backQuad = null;
	private CanvasQuad frontQuad = null;

	#region public methods

	public void TriggerPressAnimation()
	{
		if (!animatePress)
		{
			animatePress = true;
			animationProgress = 0.0f;
			if (backQuad != null) backQuad.AnimateMaterial = true;
		}
	}

	#endregion

	#region monobehavior methods

	void Awake()
	{
		propertyBlock = new MaterialPropertyBlock();
		frontPlate = GetComponent<Graphic>();
		originalPosition = transform.localPosition;
		if (hoverGraphic!=null)
		{
			hoverOriginalPosition = hoverGraphic.transform.localPosition;
		}
		if (backGlow != null) backQuad = backGlow.gameObject.GetComponent<CanvasQuad>();
		if (frontPlate != null) frontQuad = frontPlate.gameObject.GetComponent<CanvasQuad>();
	}

	void Update()
	{
		UpdatePressVisuals();
		UpdateFocusVisuals();
		UpdateHoverVisuals();
	}

	#endregion

	#region pointer methods

	public void OnPointerEnter(PointerEventData args)
	{
		inFocus = true;
	}

	public void OnPointerExit(PointerEventData args)
	{
		inFocus = false;
		//Material m = roundedRect.canvasRenderer.GetMaterial();
		//m.SetFloat("_Gaze_Focus_", 0.0f);
	}

	#endregion

	#region private methods

	private void UpdatePressVisuals()
	{
		if (animatePress)
		{
			animationProgress += Time.deltaTime;
			float t = animationProgress / animationDuration;
			if (t > 1.0f)
			{
				t = 1.0f;
				animatePress = false;
				if (backQuad != null) backQuad.AnimateMaterial = false;
			}
			float k = 1.0f - Mathf.Abs(t - 0.5f) * 2.0f;
			if (backQuad != null)
			{
				Material m = backQuad.materialForRendering;
				m.SetFloat("_Motion_", k);
			}
			transform.localPosition = originalPosition + k * maxPressDistance * Vector3.forward;
		}
	}

	private void UpdateFocusVisuals()
	{
		if (inFocus || focus != 0.0f)
		{
			float newFocus = focus + (inFocus ? 1.0f : -1.0f) * Time.deltaTime / focusDuration;
			newFocus = Mathf.Clamp01(newFocus);
			if (newFocus != focus)
			{
				focus = newFocus;
				if (frontQuad != null)
				{
					frontQuad.AnimateMaterial = focus != 0.0f;
					Material m = frontQuad.materialForRendering;
					m.SetFloat("_Gaze_Focus_", focus);
				}
			}
		}
	}

	private void UpdateHoverVisuals()
	{
		if (hoverGraphic != null)
		{
			if (inFocus || hover != 0.0f)
			{
				float newHover = hover + (inFocus ? 1.0f : -1.0f) * Time.deltaTime / hoverDuration;
				newHover = Mathf.Clamp01(newHover);
				if (newHover != hover)
				{
					hoverGraphic.transform.localPosition = hoverOriginalPosition + hover * hoverDistance * Vector3.back;
					hover = newHover;
				}
			}
		}
	}
	#endregion
}

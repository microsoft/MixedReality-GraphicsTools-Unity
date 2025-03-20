// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
	/// <summary>
	/// This component filters an area light cookie texture to make it more suitable for use with area lights.
	/// A user can control how often the filter is applied, and the filter can be applied manually via script.
	/// </summary>
	[AddComponentMenu("Scripts/GraphicsTools/AreaLightCookieFilter")]
	public class AreaLightCookieFilter : MonoBehaviour
	{
		[Tooltip("The texture to filter for the area light.")]
		[SerializeField]
		private Texture cookie;

		/// <summary>
		/// The texture to filter for the area light.
		/// </summary>
		public Texture Cookie
		{
			get => cookie;
			set => cookie = value;
		}

		[Tooltip("The result of filtering.")]
		[SerializeField]
		private RenderTexture cookieFiltered;

		/// <summary>
		/// The result of filtering.
		/// </summary>
		public RenderTexture CookieFiltered
		{
			get => cookieFiltered;
			private set => cookieFiltered = value;
		}

		[Tooltip("A material that uses Dual blurring to perform the filtering")]
		[SerializeField]
		private Material cookieFilterMaterial;

		/// <summary>
		/// A material that uses Dual blurring to perform the filtering.
		/// </summary>
		public Material CookieFilterMaterial
		{
			get => cookieFilterMaterial;
			set => cookieFilterMaterial = value;
		}

		[Tooltip("How many blur passes to perform during Dual blurring.")]
		[SerializeField]
		[Range(2, 7)]
		private int blurPasses = 4;

		/// <summary>
		/// How many blur passes to perform during Dual blurring.
		/// </summary>
		public int BlurPasses
		{
			get => blurPasses;
			set => blurPasses = Mathf.Clamp(value, 2, 7);
		}

		/// <summary>
		/// Various behaviors for refreshing the filter.
		/// </summary>
		public enum RefreshModeTechnique
		{
			OnEnable,
			EveryFrame,
			ViaScript
		}

		[Tooltip("The technique to use to refresh the filter.")]
		[SerializeField]
		private RefreshModeTechnique refreshMode;

		/// <summary>
		/// The technique to use to refresh the filter.
		/// </summary>
		public RefreshModeTechnique RefreshMode
		{
			get => refreshMode;
			set => refreshMode = value;
		}

		[Tooltip("If the \"Every Frame\" refresh mode is selected, this period can be used to adjust the filter rate. Value in seconds. A value of zero means every frame.")]
		[SerializeField]
		private float everyFramePeriod = 0.0f;

		/// <summary>
		/// If the "Every Frame" refresh mode is selected, this period can be used to adjust the filter rate. Value in seconds. A value of zero means every frame.
		/// </summary>
		public float EveryFramePeriod
		{
			get => everyFramePeriod;
			set => everyFramePeriod = value;
		}

		private bool cookieCreatedLocally = false;
		private Coroutine filterCoroutine;

		public void Filter()
		{
			if (cookieFilterMaterial == null)
			{
				return;
			}

			int width = cookie.width;
			int height = cookie.height;

			if (cookieFiltered == null)
			{
				cookieFiltered = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
				cookieFiltered.name = $"{cookie.name}_Filtered";
				cookieCreatedLocally = true;
			}
			else if (cookieFiltered.width != width || cookieFiltered.height != height)
			{
				cookieFiltered.Release();
				cookieFiltered.width = width;
				cookieFiltered.height = height;
				cookieFiltered.Create();
			}

			// Note, using blit rather than CopyTexture because the source texture is sometimes compressed.
			Graphics.Blit(cookie, cookieFiltered);

			AcrylicLayer layer = new(null, null, 0, 0, true, null, cookieFilterMaterial);
			layer.ApplyDualBlur(ref cookieFiltered, blurPasses);
			layer.Dispose();
		}

		private void OnEnable()
		{
			switch (refreshMode)
			{
				case RefreshModeTechnique.OnEnable:
					Filter();
					break;
				case RefreshModeTechnique.EveryFrame:
					filterCoroutine = StartCoroutine(FilterCoroutine());
					break;

				default:
				case RefreshModeTechnique.ViaScript:
					// Do nothing.
					break;
			}
		}

		private void OnDisable()
		{
			if (filterCoroutine != null)
			{
				StopCoroutine(filterCoroutine);
			}
		}

		private IEnumerator FilterCoroutine()
		{
			while (true)
			{
				Filter();

				if (everyFramePeriod > 0.0f)
				{
					yield return new WaitForSeconds(everyFramePeriod);
				}
				else
				{
					yield return null;
				}
			}
		}

		private void OnDestroy()
		{
			if (cookieCreatedLocally && cookieFiltered != null)
			{
				cookieFiltered.Release();
				cookieFiltered = null;
			}
		}
	}
}

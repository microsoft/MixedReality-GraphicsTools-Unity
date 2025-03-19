// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
	/// <summary>
	/// TODO
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

		[Tooltip("The material to perform the filtering.")]
		[SerializeField]
		private Material cookieFilterMaterial;

		/// <summary>
		/// The material to perform the filtering.
		/// </summary>
		public Material CookieFilterMaterial
		{
			get => cookieFilterMaterial;
			set => cookieFilterMaterial = value;
		}

		private bool cookieCreatedLocally = false;

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

			// Note, using Blit rather than CopyTexture because the source texture is often compressed.
			Graphics.Blit(cookie, cookieFiltered);

			AcrylicLayer.Settings settings = new();
			AcrylicLayer layer = new(null, settings, 0, 0, true, null, cookieFilterMaterial);
			layer.ApplyDualBlur(ref cookieFiltered);
			layer.Dispose();
		}

		public float delayTemp = 1;
		private float timer = 0;

		private void Update()
		{
			// TODO, do this smarter.
			timer += Time.deltaTime;
			if (timer >= delayTemp)
			{
				Filter();
				timer = 0;
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

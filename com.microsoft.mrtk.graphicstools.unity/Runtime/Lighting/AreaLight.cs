// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
	/// <summary>
	/// TODO
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Scripts/GraphicsTools/AreaLight")]
	public partial class AreaLight : BaseLight
	{
		private const int areaLightCount = 1;
		private const int areaLightDataSize = 2;
		private static readonly Vector4 invalidLightDirection = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);

		private static Texture2D transformInvTexture_Specular;
		private static Texture2D transformInvTexture_Diffuse;
		private static Texture2D ampDiffAmpSpecFresnel;

		private static List<AreaLight> activeAreaLights = new(areaLightCount);
		private static Vector4[] areaLightData = new Vector4[areaLightDataSize * areaLightCount];
		private static int _AreaLightDataID;
		private static int lastAreaLightUpdate = -1;

		[Tooltip("Specifies the light color.")]
		[SerializeField]
		private Color color = new Color(255.0f / 255.0f, 244.0f / 255.0f, 214.0f / 255.0f, 1.0f);

		/// <summary>
		/// Specifies the light color.
		/// </summary>
		public Color Color
		{
			get => color;
			set => color = value;
		}

		[Tooltip("Scales the brightness of the light.")]
		[SerializeField, Min(0.0f)]
		private float intensity = 1.0f;

		/// <summary>
		/// Scales the brightness of the light.
		/// </summary>
		public float Intensity
		{
			get => intensity;
			set => intensity = Mathf.Max(0.0f, value);
		}

		[Tooltip("TODO")]
		[SerializeField, Range(0.0f, 179.0f)]
		private float angle = 100.0f;

		/// <summary>
		/// TODO
		/// </summary>
		public float Angle
		{
			get => angle;
			set => angle = Mathf.Clamp(value, 0.0f, 179.0f);
		}

		[Tooltip("TODO")]
		[SerializeField]
		private Vector3 size = new Vector3(1.5f, 1.0f, 2.0f);

		/// <summary>
		/// TODO
		/// </summary>
		public Vector3 Size
		{
			get => size;
			set => size = value;
		}

		/// <summary>
		/// TODO
		/// </summary>
		public float GetNearToCenter()
		{
			if (angle == 0.0f)
			{
				return 0.0f;
			}

			return size.y * 0.5f / Mathf.Tan(angle * 0.5f * Mathf.Deg2Rad);
		}

		/// <summary>
		/// TODO
		/// </summary>
		public Bounds GetFrustumBounds()
		{
			if (angle == 0.0f)
			{
				return new Bounds(Vector3.zero, size);
			}

			float tanHalfFOV = Mathf.Tan(angle * 0.5f * Mathf.Deg2Rad);
			float near = size.y * 0.5f / tanHalfFOV;
			float z = size.z;
			float y = (near + size.z) * tanHalfFOV * 2.0f;
			float x = size.x * y / size.y;
			return new Bounds(Vector3.forward * size.z * 0.5f, new Vector3(x, y, z));
		}

		/// <summary>
		/// TODO
		/// </summary>
		public Matrix4x4 GetOffsetMatrix(float zOffset)
		{
			Matrix4x4 m = Matrix4x4.identity;
			m.SetColumn(3, new Vector4(0, 0, zOffset, 1));
			return m;
		}

		#region BaseLight Implementation

		/// <inheritdoc/>
		protected override void Initialize()
		{
			_AreaLightDataID = Shader.PropertyToID("_AreaLightData");

			if (transform.localScale != Vector3.one)
			{
#if UNITY_EDITOR
				Debug.LogError("AreaLights don't like to be scaled. Setting local scale to 1.", this);
#endif
				transform.localScale = Vector3.one;
			}

			CreateLUTs();
		}

		/// <inheritdoc/>
		protected override void AddLight()
		{
			if (activeAreaLights.Count == areaLightCount)
			{
				Debug.LogWarningFormat("Max area light count {0} exceeded. {1} will not be considered by the Graphics Tools/Standard shader until other lights are removed.", areaLightCount, gameObject.name);
			}

			activeAreaLights.Add(this);
		}

		/// <inheritdoc/>
		protected override void RemoveLight()
		{
			activeAreaLights.Remove(this);
		}

		/// <inheritdoc/>
		protected override void UpdateLights(bool forceUpdate = false)
		{
			if (lastAreaLightUpdate == -1)
			{
				Initialize();
			}

			if (!forceUpdate && (Time.frameCount == lastAreaLightUpdate))
			{
				return;
			}

			for (int i = 0; i < areaLightCount; ++i)
			{
				AreaLight light = (i >= activeAreaLights.Count) ? null : activeAreaLights[i];
				int dataIndex = i * areaLightDataSize;

				if (light)
				{
					Vector4 direction = -light.transform.forward;
					areaLightData[dataIndex] = new Vector4(direction.x,
														   direction.y,
														   direction.z,
															  1.0f);
					areaLightData[dataIndex + 1] = new Vector4(light.Color.r * intensity,
															   light.Color.g * intensity,
															   light.Color.b * intensity,
															   1.0f);
				}
				else
				{
					areaLightData[dataIndex] = invalidLightDirection;
					areaLightData[dataIndex + 1] = Vector4.zero;
				}
			}

			Shader.SetGlobalVectorArray(_AreaLightDataID, areaLightData);

			lastAreaLightUpdate = Time.frameCount;
		}

		#endregion BaseLight Implementation

		private void CreateLUTs()
		{
			if (transformInvTexture_Diffuse == null)
				transformInvTexture_Diffuse = LoadLUT(LUTType.TransformInv_DisneyDiffuse);
			if (transformInvTexture_Specular == null)
				transformInvTexture_Specular = LoadLUT(LUTType.TransformInv_GGX);
			if (ampDiffAmpSpecFresnel == null)
				ampDiffAmpSpecFresnel = LoadLUT(LUTType.AmpDiffAmpSpecFresnel);

			Shader.SetGlobalTexture("_TransformInv_Diffuse", transformInvTexture_Diffuse);
			Shader.SetGlobalTexture("_TransformInv_Specular", transformInvTexture_Specular);
			Shader.SetGlobalTexture("_AmpDiffAmpSpecFresnel", ampDiffAmpSpecFresnel);
		}

		private enum LUTType
		{
			TransformInv_DisneyDiffuse,
			TransformInv_GGX,
			AmpDiffAmpSpecFresnel
		}

		private const int kLUTResolution = 64;
		private const int kLUTMatrixDim = 3;

		private static Texture2D LoadLUT(LUTType type)
		{
			switch (type)
			{
				case LUTType.TransformInv_DisneyDiffuse: return LoadLUT(s_LUTTransformInv_DisneyDiffuse);
				case LUTType.TransformInv_GGX: return LoadLUT(s_LUTTransformInv_GGX);
				case LUTType.AmpDiffAmpSpecFresnel: return LoadLUT(s_LUTAmplitude_DisneyDiffuse, s_LUTAmplitude_GGX, s_LUTFresnel_GGX);
			}

			return null;
		}

		static Texture2D CreateLUT(TextureFormat format, Color[] pixels)
		{
			Texture2D tex = new Texture2D(kLUTResolution, kLUTResolution, format, false /*mipmap*/, true /*linear*/);
			tex.hideFlags = HideFlags.HideAndDontSave;
			tex.wrapMode = TextureWrapMode.Clamp;
			tex.SetPixels(pixels);
			tex.Apply();

			return tex;
		}

		static Texture2D LoadLUT(double[,] LUTTransformInv)
		{
			const int count = kLUTResolution * kLUTResolution;
			Color[] pixels = new Color[count];

			for (int i = 0; i < count; i++)
			{
				// Only columns 0, 2, 4 and 6 contain interesting values (at least in the case of GGX).
				pixels[i] = new Color((float)LUTTransformInv[i, 0],
										(float)LUTTransformInv[i, 2],
										(float)LUTTransformInv[i, 4],
										(float)LUTTransformInv[i, 6]);
			}

			return CreateLUT(TextureFormat.RGBAHalf, pixels);
		}

		static Texture2D LoadLUT(float[] LUTScalar0, float[] LUTScalar1, float[] LUTScalar2)
		{
			const int count = kLUTResolution * kLUTResolution;
			Color[] pixels = new Color[count];

			// Amplitude.
			for (int i = 0; i < count; i++)
			{
				pixels[i] = new Color(LUTScalar0[i], LUTScalar1[i], LUTScalar2[i], 0);
			}

			return CreateLUT(TextureFormat.RGBAHalf, pixels);
		}
	}
}

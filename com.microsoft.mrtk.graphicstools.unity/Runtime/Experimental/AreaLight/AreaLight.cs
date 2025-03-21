// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
	/// <summary>
	/// An area light is a light source with a defined rectangular shape that produces soft, diffused lighting.
	/// Based off work from: https://github.com/Unity-Technologies/VolumetricLighting
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Scripts/GraphicsTools/AreaLight")]
	public partial class AreaLight : BaseLight, IComparable<AreaLight>
	{
		private const int areaLightCount = 2;
		private const int areaLightDataSize = 1;
		private const int maxAreaLights = 32;
		private static readonly float[,] offsets = new float[4, 2] { { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
		private const int lutResolution = 64;
		private const int lutMatrixDim = 3;

		private static Texture2D transformInvTextureSpecular;
		private static Texture2D transformInvTextureDiffuse;
		private static Texture2D ampDiffAmpSpecFresnel;

		private static List<AreaLight> activeAreaLights = new(maxAreaLights);
		private static List<AreaLight> activeAreaLightsSorted = new(maxAreaLights);
		private static Vector4[] areaLightData = new Vector4[areaLightDataSize * areaLightCount];
		private static Matrix4x4[] areaLightVerts = new Matrix4x4[areaLightCount];
		private static Texture[] areaLightCookies = new Texture[areaLightCount];
		private static int _AreaLightDataID;
		private static int _AreaLightVertsID;
		private static int[] _AreaLightCookiesIDs;
		private static int lastAreaLightUpdate = -1;
		private static CullingGroup cullingGroup;
		private static BoundingSphere[] boundingSpheres = new BoundingSphere[maxAreaLights];

		[Experimental]
		[Tooltip("Specifies the light color.")]
		[SerializeField]
		private Color color = new Color(150.0f / 255.0f, 180.0f / 255.0f, 255.0f / 255.0f, 1.0f);

		/// <summary>
		/// Specifies the light color.
		/// </summary>
		public Color Color
		{
			get
			{
				if (QualitySettings.activeColorSpace == ColorSpace.Gamma)
				{
					return color * intensity;
				}

				return new Color(Mathf.GammaToLinearSpace(color.r * intensity),
								 Mathf.GammaToLinearSpace(color.g * intensity),
								 Mathf.GammaToLinearSpace(color.b * intensity),
								 1.0f);
			}
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

		[Tooltip("Width and height of the light.")]
		[SerializeField]
		private Vector2 size = new Vector2(2.0f, 1.0f);

		/// <summary>
		/// Width and height of the light.
		/// </summary>
		public Vector2 Size
		{
			get => size;
			set => size = value;
		}

		[Tooltip("Optional texture to use instead of a solid color.")]
		[SerializeField]
		private Texture cookie;

		/// <summary>
		/// Optional texture to use instead of a solid color.
		/// </summary>
		public Texture Cookie
		{
			get => cookie;
			set => cookie = value;
		}

		[Tooltip("Should the area light have a visualization?")]
		[SerializeField]
		private bool drawLightSource = true;

		/// <summary>
		/// Should the area light have a visualization?
		/// </summary>
		public bool DrawLightSource
		{
			get => drawLightSource;
			set
			{
				drawLightSource = value;
				UpdateLightSourceVisual();
			}
		}

		[SerializeField, HideInInspector]
		private MeshRenderer lightSourceVisual;

		private bool isVisible;

		/// <summary>
		/// True of the AreaLight's BoundsWorldSpace is visible by the camera. Only valid when CullingActive is true.
		/// </summary>
		public bool IsVisible
		{
			get => isVisible;
		}

		private float distance;

		/// <summary>
		/// Square distance from the camera. Only valid when CullingActive is true.
		/// </summary>
		public float Distance
		{
			get => distance;
		}

		/// <summary>
		/// True when there are more lights than we can render.
		/// </summary>
		public bool CullingActive
		{
			get => (activeAreaLights.Count > areaLightCount);
		}

		/// <summary>
		/// Calculates the AreaLight's Bounds in worldspace.
		/// </summary>
		public BoundingSphere SphereBoundsWorldSpace
		{
			get
			{
				var bounds = BoundsWorldSpace;
				float radius = bounds.extents.magnitude;
				return new BoundingSphere(bounds.center, radius);
			}
		}

		/// <summary>
		/// Calculates the AreaLight's Bounds in worldspace.
		/// </summary>
		public Bounds BoundsWorldSpace
		{
			get
			{
				var right = transform.right;
				var up = transform.up;
				var halfSize = size * 0.5f;
				var bounds = new Bounds(transform.position, Vector3.zero);
				bounds.Encapsulate(transform.TransformPoint(new Vector3(-halfSize.x, -halfSize.y, 0.0f)));
				bounds.Encapsulate(transform.TransformPoint(new Vector3(halfSize.x, -halfSize.y, 0.0f)));
				bounds.Encapsulate(transform.TransformPoint(new Vector3(halfSize.x, halfSize.y, 0.0f)));
				bounds.Encapsulate(transform.TransformPoint(new Vector3(-halfSize.x, halfSize.y, 0.0f)));
				return bounds;
			}
		}

		/// <summary>
		/// Accessors for the culling group camera.
		/// </summary>
		public Camera CullingGroupCamera
		{
			get => cullingGroup != null ? cullingGroup.targetCamera : Camera.main;
			set
			{
				if (cullingGroup != null)
				{
					cullingGroup.targetCamera = value;
				}
			}
		}

		#region BaseLight Implementation

		/// <inheritdoc/>
		protected override void Initialize()
		{
			_AreaLightDataID = Shader.PropertyToID("_AreaLightData");
			_AreaLightVertsID = Shader.PropertyToID("_AreaLightVerts");

			_AreaLightCookiesIDs = new int[areaLightCount];
			for (int i = 0; i < _AreaLightCookiesIDs.Length; ++i)
			{
				_AreaLightCookiesIDs[i] = Shader.PropertyToID($"_AreaLightCookie{i}");
			}

			CreateLUTs();
			UpdateLightSourceVisual();

			// Only create a culling group when playing since we can't consistently dispose of them in edit mode.
			if (Application.isPlaying)
			{
				cullingGroup = new CullingGroup();
				cullingGroup.targetCamera = Camera.main;
				cullingGroup.SetBoundingSpheres(boundingSpheres);
				cullingGroup.SetBoundingDistances(new float[] { 1, 5, 10, 30, 50, Mathf.Infinity});
				cullingGroup.SetBoundingSphereCount(activeAreaLights.Count);
			}
		}

		/// <inheritdoc/>
		protected override void AddLight()
		{
			if (activeAreaLights.Count == areaLightCount)
			{
				Debug.LogWarning($"Max active area light count {areaLightCount} exceeded. Some lights will be culled.");
			}

			if (activeAreaLights.Count == maxAreaLights)
			{
				Debug.LogWarning($"Max area light count {maxAreaLights} exceeded. AreaLight named \"{gameObject.name}\" will not be considered.");

				return;
			}

			activeAreaLights.Add(this);
			activeAreaLightsSorted.Add(this);

			if (cullingGroup != null)
			{
				int count = activeAreaLights.Count;
				cullingGroup.SetBoundingSphereCount(count);
				boundingSpheres[count - 1] = SphereBoundsWorldSpace;
			}
		}

		/// <inheritdoc/>
		protected override void RemoveLight()
		{
			activeAreaLights.Remove(this);
			activeAreaLightsSorted.Remove(this);

			if (cullingGroup != null)
			{
				cullingGroup.SetBoundingSphereCount(activeAreaLights.Count);
			}
		}

		/// <inheritdoc/>
		protected override void UpdateLights(bool forceUpdate = false)
		{
			// Strange case where disable is called on load? 
			if (forceUpdate && lastAreaLightUpdate == -1)
			{
				return;
			}

			if (lastAreaLightUpdate == -1)
			{
				Initialize();
			}

			if (!forceUpdate && (Time.frameCount == lastAreaLightUpdate))
			{
				return;
			}

			// Update culling if we have more lights than we can render.
			if (CullingActive)
			{
				var camera = CullingGroupCamera;

				if (camera != null)
				{
					if (cullingGroup != null)
					{
						for (int i = 0; i < activeAreaLights.Count; ++i)
						{
							boundingSpheres[i] = activeAreaLights[i].SphereBoundsWorldSpace;

							activeAreaLights[i].isVisible = cullingGroup.IsVisible(i); // This is a frame behind, but that is okay.
							activeAreaLights[i].distance = Vector3.SqrMagnitude(camera.transform.position - activeAreaLights[i].transform.position);
						}
					}
					else // Perform slow visibility checks in editor.
					{
						var planes = GeometryUtility.CalculateFrustumPlanes(camera);

						foreach (var light in activeAreaLights)
						{
							light.isVisible = GeometryUtility.TestPlanesAABB(planes, light.BoundsWorldSpace);
							light.distance = Vector3.SqrMagnitude(camera.transform.position- light.transform.position);
						}
					}

					// Sort the lights by importance (visibility and distance).
					activeAreaLightsSorted.Sort();
				}
			}

			for (int i = 0; i < areaLightCount; ++i)
			{
				AreaLight light = (i >= activeAreaLightsSorted.Count) ? null : activeAreaLightsSorted[i];
				int dataIndex = i * areaLightDataSize;

				if (light)
				{
					areaLightData[dataIndex] = light.Color;

					// A little bit of bias to prevent the light from lighting itself.
					const float z = 0.01f;

					Matrix4x4 lightVerts = new Matrix4x4();
					for (int v = 0; v < 4; ++v)
					{
						Vector3 vertex = new Vector3(light.size.x * offsets[v, 0],
													 light.size.y * offsets[v, 1],
													 z) * 0.5f;
						lightVerts.SetRow(v, light.transform.TransformPoint(vertex));
					}

					areaLightVerts[i] = lightVerts;

					if (light.cookie != null)
					{
						areaLightCookies[i] = light.cookie;
					}
					else
					{
						areaLightCookies[i] = Texture2D.whiteTexture;
					}

					light.UpdateLightSourceVisual();
				}
				else
				{
					areaLightData[dataIndex] = Vector4.zero;
					areaLightVerts[i] = Matrix4x4.zero;
					areaLightCookies[i] = Texture2D.whiteTexture;
				}
			}

			Shader.SetGlobalVectorArray(_AreaLightDataID, areaLightData);
			Shader.SetGlobalMatrixArray(_AreaLightVertsID, areaLightVerts);

			// There is no SetGlobalTextureArray so pass in 1 by 1.
			for (int i = 0; i < areaLightCookies.Length; ++i)
			{
				Shader.SetGlobalTexture(_AreaLightCookiesIDs[i], areaLightCookies[i]);
			}

			lastAreaLightUpdate = Time.frameCount;
		}

		#endregion BaseLight Implementation

		#region IComparable Implementation

		public int CompareTo(AreaLight other)
		{
			if (other == null) return 1;

			// Sort by visibility first.
			if (this.isVisible && !other.isVisible) return -1;
			if (!this.isVisible && other.isVisible) return 1;

			// If both are either visible or not, sort by distance (prefer smaller distances).
			return this.distance.CompareTo(other.distance);
		}

		#endregion IComparable Implementation

#if UNITY_EDITOR
		private void Reset()
		{
			DestroyLightVisual();
		}


		private void Awake()
		{
			// Destroy component which get copied by the editor.
			if (!Application.isPlaying)
			{
				DestroyLightVisual(false);
			}
		}
#endif

		private void OnDestroy()
		{
			if (cullingGroup != null && activeAreaLights.Count == 0)
			{
				cullingGroup.Dispose();
				cullingGroup = null;
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Gizmos.DrawIcon(transform.position, "AreaLight Gizmo", true, Color);
		}
#endif

		private static void CreateLUTs()
		{
			if (transformInvTextureDiffuse == null)
			{
				transformInvTextureDiffuse = LoadLUT(LUTType.TransformInv_DisneyDiffuse);
			}

			if (transformInvTextureSpecular == null)
			{
				transformInvTextureSpecular = LoadLUT(LUTType.TransformInv_GGX);
			}

			if (ampDiffAmpSpecFresnel == null)
			{
				ampDiffAmpSpecFresnel = LoadLUT(LUTType.AmpDiffAmpSpecFresnel);
			}

			Shader.SetGlobalTexture("_TransformInvDiffuse", transformInvTextureDiffuse);
			Shader.SetGlobalTexture("_TransformInvSpecular", transformInvTextureSpecular);
			Shader.SetGlobalTexture("_AmpDiffAmpSpecFresnel", ampDiffAmpSpecFresnel);
		}

		private void UpdateLightSourceVisual()
		{
			if (drawLightSource && enabled)
			{
				if (lightSourceVisual == null)
				{
					lightSourceVisual = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshRenderer>();
					lightSourceVisual.gameObject.name = "AreaLightVisual";
					lightSourceVisual.gameObject.hideFlags = HideFlags.NotEditable;
					lightSourceVisual.transform.parent = transform;
					lightSourceVisual.transform.localPosition = Vector3.zero;
					lightSourceVisual.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
					lightSourceVisual.sharedMaterial = new Material(Shader.Find("Hidden/Graphics Tools/Experimental/Area Light Visualize"));
				}

				lightSourceVisual.sharedMaterial.color = Color;
				lightSourceVisual.sharedMaterial.mainTexture = cookie; // Use the unblurred version for visualization.
				lightSourceVisual.transform.localScale = new Vector3(size.x, size.y, 1.0f);
			}
			else
			{
				DestroyLightVisual();
			}
		}

		private void DestroyLightVisual(bool destroyMaterial = true)
		{
			if (lightSourceVisual)
			{
				if (Application.isPlaying)
				{
					if (destroyMaterial)
					{
						Destroy(lightSourceVisual.sharedMaterial);
					}

					Destroy(lightSourceVisual.gameObject);
				}
				else
				{
					if (destroyMaterial)
					{
						DestroyImmediate(lightSourceVisual.sharedMaterial);
					}

					DestroyImmediate(lightSourceVisual.gameObject);
				}

				lightSourceVisual = null;
			}
		}

		private enum LUTType
		{
			TransformInv_DisneyDiffuse,
			TransformInv_GGX,
			AmpDiffAmpSpecFresnel
		}

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

		private static Texture2D CreateLUT(TextureFormat format, Color[] pixels)
		{
			var tex = new Texture2D(lutResolution, lutResolution, format, false /*mipmap*/, true /*linear*/);
			tex.hideFlags = HideFlags.HideAndDontSave;
			tex.wrapMode = TextureWrapMode.Clamp;
			tex.SetPixels(pixels);
			tex.Apply();

			return tex;
		}

		private static Texture2D LoadLUT(double[,] LUTTransformInv)
		{
			const int count = lutResolution * lutResolution;
			Color[] pixels = new Color[count];

			for (int i = 0; i < count; ++i)
			{
				// Only columns 0, 2, 4 and 6 contain interesting values (at least in the case of GGX).
				pixels[i] = new Color((float)LUTTransformInv[i, 0],
									  (float)LUTTransformInv[i, 2],
									  (float)LUTTransformInv[i, 4],
									  (float)LUTTransformInv[i, 6]);
			}

			return CreateLUT(TextureFormat.RGBAHalf, pixels);
		}

		private static Texture2D LoadLUT(float[] LUTScalar0, float[] LUTScalar1, float[] LUTScalar2)
		{
			const int count = lutResolution * lutResolution;
			Color[] pixels = new Color[count];

			// Amplitude.
			for (int i = 0; i < count; ++i)
			{
				pixels[i] = new Color(LUTScalar0[i], LUTScalar1[i], LUTScalar2[i], 0);
			}

			return CreateLUT(TextureFormat.RGBAHalf, pixels);
		}
	}
}

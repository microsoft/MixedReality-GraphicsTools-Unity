// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

#if GT_USE_GLTFAST
using MaterialProperty = GLTFast.Materials.MaterialProperty;
#endif

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
	public class LightCombinerWindow : EditorWindow
	{
		private enum TechniqueMode
		{
			MergeWithAlbedo,
			HijackAnotherTexture
		}

		// Export settings.
		private TechniqueMode techniqueMode = TechniqueMode.MergeWithAlbedo;
		private float textureScalar = 1.0f;
		private bool exportHDR = false;
		private TextureImporterCompression textureCompression = TextureImporterCompression.CompressedHQ;
		private int textureDilationSteps = 16;

		private enum CombineMode
		{
			AlbedoAndLightmap,
			Albedo,
			Lightmap
		}

		// Debug settings.
		private CombineMode combineMode = CombineMode.AlbedoAndLightmap;
		private bool saveIntermediateTextures = false;

		private string infoText = "I'm a friendly help message.";
		private string errorText = null;

		private const string kWorkingDirectoryPostfix = "LightCombined";

		[MenuItem("Window/Graphics Tools/Light Combiner")]
		private static void ShowWindow()
		{
			var window = GetWindow<LightCombinerWindow>();
			window.titleContent = new GUIContent("Light Combiner", EditorGUIUtility.IconContent("SceneviewLighting").image);
			window.minSize = new Vector2(480.0f, 540.0f);
			window.Show();
		}

		private void OnGUI()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Export Options", EditorStyles.boldLabel);

				techniqueMode = (TechniqueMode)EditorGUILayout.EnumPopup("Technique", techniqueMode);
				exportHDR = EditorGUILayout.Toggle("HDR", exportHDR);
				textureCompression = (TextureImporterCompression)EditorGUILayout.EnumPopup("Compression Mode", textureCompression);

				if (techniqueMode == TechniqueMode.MergeWithAlbedo)
				{
					textureScalar = EditorGUILayout.FloatField("Texture Scalar", textureScalar);
					textureDilationSteps = EditorGUILayout.IntSlider("Dilation Steps", textureDilationSteps, 0, 256);
				}
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Debug Options", EditorStyles.boldLabel);

				if (techniqueMode == TechniqueMode.MergeWithAlbedo)
				{
					combineMode = (CombineMode)EditorGUILayout.EnumPopup("Combine Mode", combineMode);
					saveIntermediateTextures = EditorGUILayout.Toggle("Save Intermediate Textures", saveIntermediateTextures);
				}
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.Space();

				if (GUILayout.Button("Combine Lighting"))
				{
					Save();
				}

				EditorGUILayout.Space();

				if (!string.IsNullOrEmpty(errorText))
				{
					EditorGUILayout.HelpBox(errorText, MessageType.Error);
				}
				else
				{
					EditorGUILayout.HelpBox(infoText, MessageType.Info);
				}
			}
			EditorGUILayout.EndVertical();
		}

		private void Save()
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();

			// Get the active scene.
			var currentScene = SceneManager.GetActiveScene();
			var currentScenePath = currentScene.path;

			if (string.IsNullOrEmpty(currentScenePath))
			{
				errorText = "Current scene is not saved. Please save the scene first.";
				return;
			}

			// Generate a new path for the duplicated scene.
			var newScenePath = Path.Combine(Path.GetDirectoryName(currentScenePath),
											Path.GetFileNameWithoutExtension(currentScenePath) + $"_{kWorkingDirectoryPostfix}.unity");

			if (AssetDatabase.CopyAsset(currentScenePath, newScenePath))
			{
				EditorSceneManager.OpenScene(newScenePath);

				currentScene = SceneManager.GetActiveScene();

				var workingDirectory = Path.Combine(Path.GetDirectoryName(currentScene.path), currentScene.name);
				Directory.CreateDirectory(workingDirectory);

				switch (techniqueMode)
				{
					case TechniqueMode.MergeWithAlbedo:
						MergeWithAlbedo(currentScene, workingDirectory);
						break;
					case TechniqueMode.HijackAnotherTexture:
						HijackAnotherTexture(currentScene, workingDirectory);
						break;
					default:
						errorText = "Invalid technique specified.";
						break;
				}

				AssetDatabase.SaveAssets();

				Lightmapping.ClearLightingDataAsset();
				Lightmapping.Clear();

				EditorSceneManager.SaveScene(currentScene);
			}
			else
			{
				errorText = "Failed to duplicate the scene.";
			}

			var seconds = watch.ElapsedMilliseconds / 1000.0f;
			infoText = $"LightCombinerWindow.Save took {seconds} s.";
			Debug.Log(infoText);
		}

		private struct TextureSize
		{
			public int Width;
			public int Height;
		}

		private void MergeWithAlbedo(Scene scene, string workingDirectory)
		{
			foreach (var renderer in GetAllLightmappedRenderers(scene))
			{
				// If the mesh has a second UV set, then duplicate the mesh and swap uv0 with uv1.
				var meshFilter = renderer.GetComponent<MeshFilter>();

				if (meshFilter == null || meshFilter.sharedMesh == null)
				{
					continue;
				}

				var originalMesh = meshFilter.sharedMesh;
				var uv1 = originalMesh.uv2;

				if (uv1 != null && uv1.Length != 0)
				{
					var uv0 = originalMesh.uv;

					var newMesh = Instantiate(originalMesh);
					newMesh.uv = uv1;
					newMesh.uv2 = uv0;
					meshFilter.sharedMesh = SaveMesh(newMesh, workingDirectory, renderer.gameObject.name);
				}

				// Check if the lightmap UVs are outside the normal 0-1 space. If so, we need to combine textures in lightmap space.
				bool normalizedUVs = true;
				var lightmapUVs = meshFilter.sharedMesh.uv;

				foreach (var uv in lightmapUVs)
				{
					var epsilon = 0.001f;
					if (uv.x < (0.0f - epsilon) || uv.x > (1.0f + epsilon) ||
						uv.y < (0.0f - epsilon) || uv.y > (1.0f + epsilon))
					{
						normalizedUVs = false;
					}
				}

				// For now duplicate all materials (later only do this if required).
				var materials = renderer.sharedMaterials;

				for (int i = 0; i < materials.Length; ++i)
				{
					if (materials[i] == null)
					{
						continue;
					}

					// Combine the the albedo texture and lightmap texture.
					// First determine the size of the combined texture.
					var albedoTexture = materials[i].mainTexture as Texture2D;
					var albedoTextureScale = materials[i].mainTextureScale;
					var albedoTextureOffset = materials[i].mainTextureOffset;
					var albedoTextureSize = GetScaledTextureSize(albedoTexture, albedoTextureScale);

					var lightmapTexture = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;
					var lightmapScale = new Vector2(renderer.lightmapScaleOffset.x, renderer.lightmapScaleOffset.y);
					var lightmapOffset = new Vector2(renderer.lightmapScaleOffset.z, renderer.lightmapScaleOffset.w);
					var lightmapSize = GetScaledTextureSize(lightmapTexture, lightmapScale);

					TextureSize combinedSize;

					if (normalizedUVs)
					{
						combinedSize.Width = (int)(Mathf.Max(albedoTextureSize.Width, lightmapSize.Width) * textureScalar);
						combinedSize.Height = (int)(Mathf.Max(albedoTextureSize.Height, lightmapSize.Height) * textureScalar);
					}
					else
					{
						combinedSize.Width = (int)(lightmapTexture.width * textureScalar);
						combinedSize.Height = (int)(lightmapTexture.height * textureScalar);
					}

					// Use the GPU to perform the texture combination.
					var format = RenderTextureFormat.DefaultHDR;
					var colorSpace = RenderTextureReadWrite.Linear;
					var outputRT = RenderTexture.GetTemporary(combinedSize.Width, combinedSize.Height, 0, format, colorSpace);
					var remappedAlbedoRT = RenderTexture.GetTemporary(combinedSize.Width, combinedSize.Height, 0, format, colorSpace);
					var remappedAlbedoMaskRT = RenderTexture.GetTemporary(combinedSize.Width, combinedSize.Height, 0, format, colorSpace);

					CommandBuffer cb = new CommandBuffer();
					cb.SetProjectionMatrix(Matrix4x4.Ortho(0, 1, 0, 1, -100, 100));

					var lightCombiner = new Material(Shader.Find("Hidden/Graphics Tools/Light Combiner"));
					lightCombiner.SetColor("_AlbedoColor", materials[i].color);

					if (!normalizedUVs)
					{
						lightCombiner.EnableKeyword("USE_LIGHTMAP_SCALE_OFFSET");
					}

					if (albedoTexture != null)
					{
						lightCombiner.SetTexture("_AlbedoMap", albedoTexture);
						lightCombiner.SetVector("_AlbedoMapScaleOffset", new Vector4(albedoTextureScale.x, albedoTextureScale.y,
																					 albedoTextureOffset.x, albedoTextureOffset.y));
					}

					lightCombiner.SetTexture("_LightMap", lightmapTexture);
					lightCombiner.SetVector("_LightMapScaleOffset", new Vector4(lightmapScale.x, lightmapScale.y,
																				lightmapOffset.x, lightmapOffset.y));

					// First pass, render the albedo in uv0 space.
					if (combineMode == CombineMode.AlbedoAndLightmap || combineMode == CombineMode.Albedo)
					{
						cb.SetRenderTarget(remappedAlbedoRT);
						cb.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
						cb.DrawMesh(meshFilter.sharedMesh, Matrix4x4.identity, lightCombiner, i, lightCombiner.FindPass("Albedo"));
						lightCombiner.SetTexture("_RemappedAlbedoMap", remappedAlbedoRT);
					}

					if (!exportHDR)
					{
						lightCombiner.EnableKeyword("CONVERT_TO_SRGB");
					}

					if (textureDilationSteps > 0)
					{
						lightCombiner.EnableKeyword("DILATE");
						lightCombiner.SetFloat("_DilationSteps", textureDilationSteps);
						lightCombiner.SetVector("_AlbedoMapSize", new Vector2(combinedSize.Width, combinedSize.Height));
					}

					if (combineMode == CombineMode.AlbedoAndLightmap || combineMode == CombineMode.Lightmap)
					{
						// Second pass, render a albedo mask.
						cb.SetRenderTarget(remappedAlbedoMaskRT);
						cb.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
						cb.DrawMesh(meshFilter.sharedMesh, Matrix4x4.identity, lightCombiner, i, lightCombiner.FindPass("AlbedoMask"));
						lightCombiner.SetTexture("_RemappedAlbedoMaskMap", remappedAlbedoMaskRT);

						// Third pass, blit the lightmap onto the albedo.
						cb.Blit(null, outputRT, lightCombiner, lightCombiner.FindPass("Lightmap"));
					}

					cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);

					Graphics.ExecuteCommandBuffer(cb);

					// Save the active render texture to disk.
					var name = renderer.gameObject.name + (normalizedUVs ? string.Empty : "_Lightmap");
					var combinedTextureAsset = SaveRenderTexture(outputRT, combinedSize, workingDirectory, name, exportHDR, textureCompression);

					if (saveIntermediateTextures)
					{
						SaveRenderTexture(remappedAlbedoRT, combinedSize, workingDirectory, name + "AlbedoRemap", exportHDR, textureCompression); // DEBUG
						SaveRenderTexture(remappedAlbedoMaskRT, combinedSize, workingDirectory, name + "AlbedoMask", exportHDR, textureCompression); // DEBUG
					}

					// Cleanup resources.
					DestroyImmediate(lightCombiner);
					RenderTexture.ReleaseTemporary(remappedAlbedoRT);
					RenderTexture.ReleaseTemporary(outputRT);

					// Apply to a duplicated material.
					var duplicateMaterial = DuplicateAndSaveMaterial(materials[i], workingDirectory, name);
					duplicateMaterial.color = Color.white;
					duplicateMaterial.mainTexture = combinedTextureAsset;

					if (normalizedUVs)
					{
						duplicateMaterial.mainTextureScale = Vector2.one;
						duplicateMaterial.mainTextureOffset = Vector2.zero;
					}
					else
					{
						duplicateMaterial.mainTextureScale = lightmapScale;
						duplicateMaterial.mainTextureOffset = lightmapOffset;
					}

					materials[i] = duplicateMaterial;
				}

				renderer.sharedMaterials = materials;

				// glTF has no concept of static, so remove it.
				renderer.gameObject.isStatic = false;

				// Now that lightmaps have been removed and baked into the albedo Unity will try to use light probes for the ambient
				// light info. The ambient probe is too dark because it does not consider the baked lights. So tell the renderer to
				// use a solid white probe.
				renderer.EnsureComponent<CustomProbe>();
			}
		}

		private void HijackAnotherTexture(Scene scene, string workingDirectory)
		{
			foreach (var renderer in GetAllLightmappedRenderers(scene))
			{
				// For now duplicate all materials (later only do this if required).
				var materials = renderer.sharedMaterials;

				for (int i = 0; i < materials.Length; ++i)
				{
					if (materials[i] == null)
					{
						continue;
					}

					var lightmapTexture = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;
					var lightmapScale = new Vector2(renderer.lightmapScaleOffset.x, renderer.lightmapScaleOffset.y);
					var lightmapOffset = new Vector2(renderer.lightmapScaleOffset.z, renderer.lightmapScaleOffset.w);

					var name = renderer.gameObject.name;
					var duplicateMaterial = DuplicateAndSaveMaterial(materials[i], workingDirectory, name);

					// Required for glTFast to export emission.
					duplicateMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
					duplicateMaterial.EnableKeyword("_EMISSION");

					duplicateMaterial.SetTexture("_EmissionMap", lightmapTexture);
					duplicateMaterial.SetTextureScale("_EmissionMap", lightmapScale);
					duplicateMaterial.SetTextureOffset("_EmissionMap", lightmapOffset);

					materials[i] = duplicateMaterial;
				}

				renderer.sharedMaterials = materials;

				// glTF has no concept of static, so remove it.
				renderer.gameObject.isStatic = false;
			}
		}

		private List<Renderer> GetAllLightmappedRenderers(Scene scene)
		{
			var output = new List<Renderer>();

			foreach (var rootObject in scene.GetRootGameObjects())
			{
				foreach (var renderer in rootObject.GetComponentsInChildren<Renderer>())
				{
					// Is the renderer active?
					if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
					{
						continue;
					}

					// Does the renderer use a lightmap?
					if (renderer.lightmapIndex < 0 || renderer.sharedMaterials.Length == 0)
					{
						continue;
					}

					output.Add(renderer);
				}
			}

			return output;
		}

		private static Mesh SaveMesh(Mesh mesh, string workingDirectory, string fileName)
		{
			var path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(workingDirectory, $"{fileName}.asset"));
			AssetDatabase.CreateAsset(mesh, path);

			return mesh;
		}

		private static Material DuplicateAndSaveMaterial(Material originalMaterial, string workingDirectory, string fileName)
		{
			var material = new Material(originalMaterial);
			var path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(workingDirectory, $"{fileName}.mat"));
			AssetDatabase.CreateAsset(material, path);

			return material;
		}

		private static Texture2D SaveRenderTexture(RenderTexture renderTexture, TextureSize size, string workingDirectory, string fileName, bool exportHDR, TextureImporterCompression textureCompression)
		{
			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = renderTexture;
			var texture = new Texture2D(size.Width, size.Height, TextureFormat.RGBAFloat, false, true);
			texture.ReadPixels(new Rect(0.0f, 0.0f, size.Width, size.Height), 0, 0);
			texture.Apply();
			RenderTexture.active = previous;

			byte[] textureData = null;
			string extension = null;

			if (exportHDR)
			{
				textureData = ImageConversion.EncodeToEXR(texture, Texture2D.EXRFlags.CompressZIP);
				extension = ".exr";
			}
			else
			{
				textureData = ImageConversion.EncodeArrayToPNG(texture.GetRawTextureData(), texture.graphicsFormat, (uint)texture.width, (uint)texture.height);
				extension = ".png";
			}

			if (textureData == null)
			{
				Debug.LogError($"Failed to encode texture \"{texture.name}\" to \"{extension}\".");
				return null;
			}

			DestroyImmediate(texture);

			var path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(workingDirectory, $"{fileName}{extension}"));
			File.WriteAllBytes(path, textureData);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			if (textureImporter == null)
			{
				return null;
			}

			textureImporter.textureCompression = textureCompression;

			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

			return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		}

		private static TextureSize GetScaledTextureSize(Texture2D texture, Vector2 scale)
		{
			const int minTextureSize = 2;
			const int maxTextureSize = 2048;

			TextureSize output;

			if (texture == null)
			{
				output.Width = 2;
				output.Height = 2;
			}
			else
			{
				output.Width = Mathf.Min(Mathf.Max(Mathf.ClosestPowerOfTwo((int)(texture.width * scale.x)), minTextureSize), maxTextureSize);
				output.Height = Mathf.Min(Mathf.Max(Mathf.ClosestPowerOfTwo((int)(texture.height * scale.y)), minTextureSize), maxTextureSize);
			}

			return output;
		}
	}
}
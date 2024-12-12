// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
	public class LightCombinerWindow : EditorWindow
	{
		private enum CombineMode
		{
			AlbedoAndLightmap,
			Albedo,
			Lightmap
		}

		// Settings.
		private CombineMode combineMode = CombineMode.AlbedoAndLightmap;
		private float textureScalar = 2.0f;
		private bool exportHDR = false;

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

				combineMode = (CombineMode)EditorGUILayout.EnumPopup("Combine Mode", combineMode);
				textureScalar = EditorGUILayout.FloatField("Texture Scalar", textureScalar);
				exportHDR = EditorGUILayout.Toggle("HDR", exportHDR);
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
					EditorGUILayout.HelpBox("I'm a friendly help message.", MessageType.Info);
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
			var newScenePath = Path.Combine(Path.GetDirectoryName(currentScenePath), Path.GetFileNameWithoutExtension(currentScenePath) + $"_{kWorkingDirectoryPostfix}.unity");

			if (AssetDatabase.CopyAsset(currentScenePath, newScenePath))
			{
				EditorSceneManager.OpenScene(newScenePath);

				currentScene = SceneManager.GetActiveScene();
				CombineLighting(currentScene);

				Lightmapping.ClearLightingDataAsset();
				Lightmapping.Clear();

				EditorSceneManager.SaveScene(currentScene);
			}
			else
			{
				errorText = "Failed to duplicate the scene.";
			}

			Debug.LogFormat("LightCombinerWindow.Save took {0} ms.", watch.ElapsedMilliseconds);
		}

		private struct TextureSize
		{
			public int Width;
			public int Height;
		}

		private void CombineLighting(Scene scene)
		{
			var workingDirectory = Path.Combine(Path.GetDirectoryName(scene.path), scene.name);
			Directory.CreateDirectory(workingDirectory);

			foreach (var rootObject in scene.GetRootGameObjects())
			{
				foreach (var renderer in rootObject.GetComponentsInChildren<Renderer>())
				{
					// Does the renderer use a lightmap?
					if (renderer.lightmapIndex < 0 || renderer.sharedMaterials.Length == 0)
					{
						continue;
					}

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
						combinedSize.Width = (int)(Mathf.Max(albedoTextureSize.Width, lightmapSize.Width) * textureScalar);
						combinedSize.Height = (int)(Mathf.Max(albedoTextureSize.Height, lightmapSize.Height) * textureScalar);

						// Use the GPU to perform the texture combination.
						var outputRT = RenderTexture.GetTemporary(combinedSize.Width, combinedSize.Height, 0, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
						var remappedAlbedoRT = RenderTexture.GetTemporary(combinedSize.Width, combinedSize.Height, 0, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);

						CommandBuffer cb = new CommandBuffer();

						cb.SetRenderTarget(outputRT);
						cb.SetProjectionMatrix(Matrix4x4.Ortho(0, 1, 0, 1, -100, 100));
						cb.ClearRenderTarget(true, true, Color.white);

						var lightCombiner = new Material(Shader.Find("Hidden/Graphics Tools/Light Combiner"));
						lightCombiner.SetColor("_AlbedoColor", materials[i].color);

						// First pass, render the albedo in uv0 space.
						if (albedoTexture != null)
						{
							lightCombiner.SetTexture("_AlbedoMap", albedoTexture);
							lightCombiner.SetVector("_AlbedoMapScaleOffset", new Vector4(albedoTextureScale.x, albedoTextureScale.y,
																						 albedoTextureOffset.x, albedoTextureOffset.y));
						}

						if (combineMode == CombineMode.AlbedoAndLightmap || combineMode == CombineMode.Albedo)
						{
							cb.DrawMesh(meshFilter.sharedMesh, Matrix4x4.identity, lightCombiner, i, lightCombiner.FindPass("Albedo"));

							cb.Blit(BuiltinRenderTextureType.CurrentActive, remappedAlbedoRT);
							lightCombiner.SetTexture("_RemappedAlbedoMap", remappedAlbedoRT);
						}

						// Second pass, blit the lightmap onto the albedo.
						lightCombiner.SetTexture("_LightMap", lightmapTexture);
						lightCombiner.SetVector("_LightMapScaleOffset", new Vector4(lightmapScale.x, lightmapScale.y,
																					lightmapOffset.x, lightmapOffset.y));

						if (combineMode == CombineMode.AlbedoAndLightmap || combineMode == CombineMode.Lightmap)
						{
							cb.Blit(null, outputRT, lightCombiner, lightCombiner.FindPass("Lightmap"));
						}

						Graphics.ExecuteCommandBuffer(cb);

						// Save the active render texture to disk.
						RenderTexture previous = RenderTexture.active;
						RenderTexture.active = outputRT;
						var combinedTexture = new Texture2D(combinedSize.Width, combinedSize.Height, TextureFormat.RGBAFloat, false, true);
						combinedTexture.ReadPixels(new Rect(0.0f, 0.0f, combinedSize.Width, combinedSize.Height), 0, 0);
						combinedTexture.Apply();
						RenderTexture.active = previous;
						var combinedTextureAsset = SaveTexture(combinedTexture, workingDirectory, renderer.gameObject.name, exportHDR);

						// Cleanup resources.
						DestroyImmediate(combinedTexture);
						DestroyImmediate(lightCombiner);
						RenderTexture.ReleaseTemporary(remappedAlbedoRT);
						RenderTexture.ReleaseTemporary(outputRT);

						// Apply to a duplicated material.
						var duplicateMaterial = DuplicateAndSaveMaterial(materials[i], workingDirectory, renderer.gameObject.name);
						duplicateMaterial.color = Color.white;
						duplicateMaterial.mainTexture = combinedTextureAsset;
						duplicateMaterial.mainTextureScale = new Vector2(1.0f, 1.0f);
						duplicateMaterial.mainTextureOffset = new Vector2(0.0f, 0.0f);
						materials[i] = duplicateMaterial;
					}

					renderer.sharedMaterials = materials;
				}
			}

			AssetDatabase.SaveAssets();
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

		private static Texture2D SaveTexture(Texture2D texture, string workingDirectory, string fileName, bool exportHDR)
		{
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

			var path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(workingDirectory, $"{fileName}{extension}"));
			File.WriteAllBytes(path, textureData);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			if (textureImporter == null)
			{
				return null;
			}

			// Saving texture data in linear color space.
			textureImporter.sRGBTexture = false;
			textureImporter.textureCompression = TextureImporterCompression.CompressedHQ; // TODO, why does this make glTFast export darker textures?

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
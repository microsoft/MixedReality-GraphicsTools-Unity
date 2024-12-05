// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
	public class LightCombinerWindow : EditorWindow
	{
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
				// TODO
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
					EditorGUILayout.HelpBox("TODO", MessageType.Info);
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

					// For now duplicate all materials (later only do this if required).
					for (int i = 0; i < renderer.sharedMaterials.Length; ++i)
					{
						if (renderer.sharedMaterials[i] == null)
						{
							continue;
						}

						// Combine the the main texture and lightmap textures.
						// First determine the size of the combined texture.
						var mainTexture = renderer.sharedMaterials[i].mainTexture as Texture2D;
						var mainTextureScale = renderer.sharedMaterials[i].mainTextureScale;
						var mainTextureOffset = renderer.sharedMaterials[i].mainTextureScale;
						var mainTextureSize = GetScaledTextureSize(mainTexture, mainTextureScale);

						var lightmapTexture = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;
						var lightmapScale = new Vector2(renderer.lightmapScaleOffset.x, renderer.lightmapScaleOffset.y);
						var lightmapOffset = new Vector2(renderer.lightmapScaleOffset.z, renderer.lightmapScaleOffset.w);
						var lightmapSize = GetScaledTextureSize(lightmapTexture, lightmapScale);

						TextureSize combinedSize;
						combinedSize.Width = Mathf.Max(mainTextureSize.Width, lightmapSize.Width);
						combinedSize.Height = Mathf.Max(mainTextureSize.Height, lightmapSize.Height);

						// Use the GPU to perform the combination.
						var lightCombiner = new Material(Shader.Find("Hidden/Graphics Tools/Light Combiner"));

						if (mainTexture != null)
						{
							lightCombiner.SetTexture("_AlbedoMap", mainTexture);
							lightCombiner.SetVector("_MetallicMapChannel", new Vector4(mainTextureScale.x, mainTextureScale.y, mainTextureOffset.x, mainTextureOffset.y));
						}

						lightCombiner.SetTexture("_LightMap", mainTexture);
						lightCombiner.SetVector("_LightMapScaleOffset", new Vector4(lightmapScale.x, lightmapScale.y, lightmapOffset.x, lightmapOffset.y));

						var renderTexture = RenderTexture.GetTemporary(combinedSize.Width, combinedSize.Height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
						Graphics.Blit(null, renderTexture, lightCombiner);
						DestroyImmediate(lightCombiner);

						// Save the last render texture to disk.
						RenderTexture previous = RenderTexture.active;
						RenderTexture.active = renderTexture;
						var combinedTexture = new Texture2D(combinedSize.Width, combinedSize.Height);
						combinedTexture.ReadPixels(new Rect(0.0f, 0.0f, combinedSize.Width, combinedSize.Height), 0, 0);
						combinedTexture.Apply();
						RenderTexture.active = previous;
						RenderTexture.ReleaseTemporary(renderTexture);
						var combinedTextureAsset = SaveTexture(combinedTexture, workingDirectory, renderer.gameObject.name);
						DestroyImmediate(combinedTexture);

						// Apply to a duplicated material.
						var duplicateMaterial = DuplicateAndSaveMaterial(renderer.sharedMaterials[i], workingDirectory);
						duplicateMaterial.mainTexture = combinedTextureAsset;
						duplicateMaterial.mainTextureScale = new Vector2(1.0f, 1.0f);
						duplicateMaterial.mainTextureOffset = new Vector2(0.0f, 0.0f);
						renderer.sharedMaterials[i] = duplicateMaterial;
						EditorUtility.SetDirty(duplicateMaterial);
					}
				}
			}
		}

		private static Material DuplicateAndSaveMaterial(Material originalMaterial, string workingDirectory)
		{
			var path = AssetDatabase.GetAssetPath(originalMaterial);
			var newPath = Path.Combine(workingDirectory, Path.GetFileNameWithoutExtension(path) + $"_{kWorkingDirectoryPostfix}.mat");
			var newMaterial = new Material(originalMaterial);
			AssetDatabase.CreateAsset(newMaterial, newPath);
			AssetDatabase.SaveAssets();

			return AssetDatabase.LoadAssetAtPath<Material>(newPath);
		}

		private static Texture2D SaveTexture(Texture2D texture, string workingDirectory, string fileName)
		{
			var textureData = TextureFile.Encode(texture, TextureFile.Format.PNG);

			if (textureData == null)
			{
				return null;
			}

			var path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(workingDirectory, $"{fileName}.png"));
			File.WriteAllBytes(path, textureData);
			AssetDatabase.Refresh();

			return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		}

		private static TextureSize GetScaledTextureSize(Texture2D texture, Vector2 scale)
		{
			const int minTextureSize = 2;
			const int maxTextureSize = 4096;

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
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

						var duplicateMaterial = DuplicateMaterial(renderer.sharedMaterials[i], workingDirectory);
						renderer.sharedMaterials[i] = duplicateMaterial;

						// Combine the the main texture and lightmap textures.
						// First determine the size of the combined texture.
						var mainTexture = duplicateMaterial.mainTexture as Texture2D;
						var mainTextureScale = duplicateMaterial.mainTextureScale;
						var mainTextureOffset = duplicateMaterial.mainTextureScale;
						var mainTextureSize = GetScaledTextureSize(mainTexture, mainTextureScale);

						var lightmapTexture = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;
						var lightmapScale = new Vector2(renderer.lightmapScaleOffset.x, renderer.lightmapScaleOffset.y);
						var lightmapOffset = new Vector2(renderer.lightmapScaleOffset.z, renderer.lightmapScaleOffset.w);
						var lightmapSize = GetScaledTextureSize(lightmapTexture, lightmapScale);

						TextureSize combinedSize;
						combinedSize.Width = Mathf.Max(mainTextureSize.Width, lightmapSize.Width);
						combinedSize.Height = Mathf.Max(mainTextureSize.Height, lightmapSize.Height);

						// Create a new texture to store the combined texture.
						var combinedTexture = new Texture2D(combinedSize.Width, combinedSize.Height, TextureFormat.RGBA32, false);
	
					}
				}
			}
		}

		private Material DuplicateMaterial(Material originalMaterial, string workingDirectory)
		{
			var path = AssetDatabase.GetAssetPath(originalMaterial);
			var newPath = Path.Combine(workingDirectory, Path.GetFileNameWithoutExtension(path) + $"_{kWorkingDirectoryPostfix}.mat");
			var newMaterial = new Material(originalMaterial);
			AssetDatabase.CreateAsset(newMaterial, newPath);
			AssetDatabase.SaveAssets();

			return newMaterial;
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
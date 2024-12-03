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

			// Get the active scene
			var currentScene = SceneManager.GetActiveScene();
			var currentScenePath = currentScene.path;

			if (string.IsNullOrEmpty(currentScenePath))
			{
				errorText = "Current scene is not saved. Please save the scene first.";
				return;
			}

			// Generate a new path for the duplicated scene
			var newScenePath = Path.Combine(Path.GetDirectoryName(currentScenePath), Path.GetFileNameWithoutExtension(currentScenePath) + "_Copy.unity");

			// Copy the scene file
			if (AssetDatabase.CopyAsset(currentScenePath, newScenePath))
			{
				// Open the duplicated scene
				EditorSceneManager.OpenScene(newScenePath);
			}
			else
			{
				errorText = "Failed to duplicate the scene.";
			}

			Debug.LogFormat("LightCombinerWindow.Save took {0} ms.", watch.ElapsedMilliseconds);
		}
	}
}

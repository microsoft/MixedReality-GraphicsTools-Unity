// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
	public class LightCombinerWindow : EditorWindow
	{
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
		}

		private void Save(MeshUtility.MeshCombineResult result)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();

			// TODO

			Debug.LogFormat("LightCombinerWindow.Save took {0} ms.", watch.ElapsedMilliseconds);
		}
	}
}

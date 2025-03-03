// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
	/// <summary>
	/// Improves object selection and adds a shortcut to create a configured game object and component from the game object context menu.
	/// </summary>
	[CustomEditor(typeof(AreaLight))]
	public class AreaLightInspector : UnityEditor.Editor
	{
		private void OnSceneGUI()
		{
			AreaLight light = target as AreaLight;

			if (light == null)
			{
				return;
			}

			if (light.enabled)
			{
				Handles.color = light.Color;
			}
			else
			{
				Handles.color = Color.gray;
			}

			using (new Handles.DrawingScope(light.transform.localToWorldMatrix))
			{
				Handles.DrawWireCube(new Vector3(0, 0, 0.5f * light.Size.z), light.Size);
			}
		}

		private bool HasFrameBounds() { return true; }

		private Bounds OnGetFrameBounds()
		{
			var light = target as AreaLight;
			Debug.Assert(light != null);
			return new Bounds(Vector3.zero, light.Size);
		}

		[MenuItem("GameObject/Light/Graphics Tools/Area Light")]
		private static void CreateAreaLight(MenuCommand menuCommand)
		{
			GameObject gameObject = InspectorUtilities.CreateGameObjectFromMenu<AreaLight>(menuCommand);

			if (gameObject != null)
			{
				gameObject.transform.position = new Vector3(0.0f, 3.0f, 0.0f);
				Quaternion rotation = new Quaternion();
				rotation.eulerAngles = new Vector3(50.0f, -30.0f, 0.0f);
				gameObject.transform.rotation = rotation;
			}
		}
	}
}

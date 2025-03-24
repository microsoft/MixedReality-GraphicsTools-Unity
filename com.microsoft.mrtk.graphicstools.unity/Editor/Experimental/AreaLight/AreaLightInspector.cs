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
				Handles.DrawWireCube(Vector3.zero, light.Size);
			}

			Handles.color = new Color(255.0f / 255.0f, 165.0f / 255.0f, 0.0f / 255.0f); // Orange.

			// Different visual representation for runtime and edit mode because runtime using CullingGroup to cull lights.
			if (Application.isPlaying)
			{
				var bounds = light.SphereBoundsWorldSpace;
				Handles.RadiusHandle(Quaternion.identity, bounds.position, bounds.radius);
			}
			else
			{
				var bounds = light.BoundsWorldSpace;
				Handles.DrawWireCube(bounds.center, bounds.size);
			}

			if (light.CullingActive)
			{
				Handles.Label(light.transform.position, $"Visible: {light.IsVisible}\nDist: {light.Distance.ToString("n1")}");
			}
		}

		private bool HasFrameBounds() { return true; }

		private Bounds OnGetFrameBounds()
		{
			var light = target as AreaLight;
			Debug.Assert(light != null);
			return light.BoundsWorldSpace;
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

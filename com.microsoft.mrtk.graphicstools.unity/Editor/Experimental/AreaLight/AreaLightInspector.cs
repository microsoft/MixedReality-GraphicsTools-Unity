// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

			EditorGUI.BeginChangeCheck();
			Vector2 size = DrawRectHandles(light.transform.rotation, light.transform.position, light.Size);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(light, "Adjust Area Light Size");
				light.Size = size;
			}

			// Draw the area light's normal only if it will not overlap with the current tool.
			if (!((Tools.current == Tool.Move || Tools.current == Tool.Scale) && Tools.pivotRotation == PivotRotation.Local))
			{
				var normal = light.transform.forward * ((light.Facing == AreaLight.ForwardFacing.PositiveZ) ? 1.0f : -1.0f);
				Handles.DrawLine(light.transform.position, light.transform.position + normal);
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
				gameObject.transform.position = new Vector3(0.0f, 1.0f, 0.0f);
				gameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
			}
		}

		private static float SizeSlider(Vector3 p, Vector3 d, float r)
		{
			Vector3 position = p + d * r;
			float size = HandleUtility.GetHandleSize(position);
			bool temp = GUI.changed;
			GUI.changed = false;
			position = Handles.Slider(position, d, size * 0.03f, Handles.DotHandleCap, 0.0f);

			if (GUI.changed)
			{
				r = Vector3.Dot(position - p, d);
			}

			GUI.changed |= temp;
			return r;
		}
		private static Color ToActiveColorSpace(Color color)
		{
			return (QualitySettings.activeColorSpace == ColorSpace.Linear) ? color.linear : color;
		}

		private static Vector2 DrawRectHandles(Quaternion rotation, Vector3 position, Vector2 size)
		{
			Vector3 up = rotation * Vector3.up;
			Vector3 right = rotation * Vector3.right;

			float halfWidth = 0.5f * size.x;
			float halfHeight = 0.5f * size.y;

			Vector3 topRight = position + up * halfHeight + right * halfWidth;
			Vector3 bottomRight = position - up * halfHeight + right * halfWidth;
			Vector3 bottomLeft = position - up * halfHeight - right * halfWidth;
			Vector3 topLeft = position + up * halfHeight - right * halfWidth;

			// Draw the rectangle.
			Handles.DrawLine(topRight, bottomRight);
			Handles.DrawLine(bottomRight, bottomLeft);
			Handles.DrawLine(bottomLeft, topLeft);
			Handles.DrawLine(topLeft, topRight);

			// Give handles twice the alpha of the lines.
			Color originalColor = Handles.color;
			Color color = Handles.color;
			color.a = Mathf.Clamp01(Handles.color.a * 2);
			Handles.color = ToActiveColorSpace(color);

			// Draw the handles.
			halfHeight = SizeSlider(position, up, halfHeight);
			halfHeight = SizeSlider(position, -up, halfHeight);
			halfWidth = SizeSlider(position, right, halfWidth);
			halfWidth = SizeSlider(position, -right, halfWidth);

			size.x = Mathf.Max(0.0f, 2.0f * halfWidth);
			size.y = Mathf.Max(0.0f, 2.0f * halfHeight);

			Handles.color = originalColor;

			return size;
		}
	}
}

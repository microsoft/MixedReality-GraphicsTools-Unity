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

			if (light.Angle == 0.0f)
			{
				using (new Handles.DrawingScope(light.transform.localToWorldMatrix))
				{
					Handles.DrawWireCube(new Vector3(0, 0, 0.5f * light.Size.z), light.Size);
				}
			}
			else
			{
				float near = light.GetNearToCenter();

				using (new Handles.DrawingScope(light.transform.localToWorldMatrix * light.GetOffsetMatrix(-near)))
				{
					float far = near + light.Size.z;
					float halfFOV = light.Angle * 0.5f;
					float aspect = light.Size.x / light.Size.y;

					Vector3[] nearCorners = new Vector3[4];
					Vector3[] farCorners = new Vector3[4];

					float nearHeight = 2.0f * Mathf.Tan(Mathf.Deg2Rad * halfFOV) * near;
					float nearWidth = nearHeight * aspect;
					float farHeight = 2.0f * Mathf.Tan(Mathf.Deg2Rad * halfFOV) * far;
					float farWidth = farHeight * aspect;

					nearCorners[0] = new Vector3(-nearWidth * 0.5f, -nearHeight * 0.5f, near);
					nearCorners[1] = new Vector3(nearWidth * 0.5f, -nearHeight * 0.5f, near);
					nearCorners[2] = new Vector3(nearWidth * 0.5f, nearHeight * 0.5f, near);
					nearCorners[3] = new Vector3(-nearWidth * 0.5f, nearHeight * 0.5f, near);

					farCorners[0] = new Vector3(-farWidth * 0.5f, -farHeight * 0.5f, far);
					farCorners[1] = new Vector3(farWidth * 0.5f, -farHeight * 0.5f, far);
					farCorners[2] = new Vector3(farWidth * 0.5f, farHeight * 0.5f, far);
					farCorners[3] = new Vector3(-farWidth * 0.5f, farHeight * 0.5f, far);

					for (int i = 0; i < 4; i++)
					{
						Handles.DrawLine(nearCorners[i], farCorners[i]);
						Handles.DrawLine(nearCorners[i], nearCorners[(i + 1) % 4]);
						Handles.DrawLine(farCorners[i], farCorners[(i + 1) % 4]);
					}
				}

				Handles.color = Color.white;

				using (new Handles.DrawingScope(light.transform.localToWorldMatrix))
				{
					Bounds bounds = light.GetFrustumBounds();
					Handles.DrawWireCube(bounds.center, bounds.size);
				}
			}
		}

		private bool HasFrameBounds() { return true; }

		private Bounds OnGetFrameBounds()
		{
			var light = target as AreaLight;
			Debug.Assert(light != null);
			return light.GetFrustumBounds();
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

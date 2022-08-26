// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// General purpose tool for viewing metric sizes of gameObjects.
    /// </summary>
    public class MeasureTool
    {
        private MeasureToolSettings settings;
        private List<GameObject> selectedObjects;

        public List<GameObject> SelectedObjects { get => selectedObjects; set => selectedObjects = value; }

        public MeasureTool(MeasureToolSettings toolSettings)
        {
            settings = toolSettings;
            selectedObjects = new List<GameObject>();
        }

        /// <summary>
        /// Method calls the proper Draw method for GameObjects based on the current ToolMode setting.
        /// </summary>
        /// <param name="gameObjects">An array of selected GameObjects to be measured</param>
        public void DrawMeasurement(GameObject[] gameObjects)
        {
            if (gameObjects.Length == 0) { return; }

            switch (settings.Mode)
            {
                case MeasureToolSettings.ToolMode.Auto:
                    foreach (GameObject gameObject in gameObjects)
                    {
                        DrawAuto(gameObject);
                    }
                    break;
                case MeasureToolSettings.ToolMode.Rect:
                    foreach (GameObject gameObject in gameObjects)
                    {
                        DrawRectMeasurement(gameObject);
                    }
                    break;
                case MeasureToolSettings.ToolMode.Collider:
                    foreach (GameObject gameObject in gameObjects)
                    {
                        DrawColliderMeasurement(gameObject);
                    }
                    break;
                case MeasureToolSettings.ToolMode.Renderer:
                    foreach (GameObject gameObject in gameObjects)
                    {
                        DrawRendererMeasurement(gameObject);
                    }
                    break;
                case MeasureToolSettings.ToolMode.BetweenObjects:
                    DrawDistanceBetweenObjects(selectedObjects);
                    break;
            }
        }

        /// <summary>
        /// The default mode of the tool, automatically detects the proper mode based on selected GameObject.
        /// Using RectTransforms for 2D GameObjects and prefering Renderers to Colliders for 3D GameObjects
        /// </summary>
        /// <param name="gameObject">GameObject to be measured</param>
        private void DrawAuto(GameObject gameObject)
        {
            if (gameObject.GetComponent<RectTransform>() != null)
            {
                DrawRectMeasurement(gameObject);
            }
            else if (gameObject.GetComponent<Renderer>() != null)
            {
                DrawRendererMeasurement(gameObject);
            }
            else
            {
                DrawColliderMeasurement(gameObject);
            }
        }

        /// <summary>
        /// Gets 4 Vector3s used as corners of the box.
        /// Calculates distance between 2 horizontal vectors and 2 vertical vectors.
        /// Draws a box around the GameObject from an array of Vector3.
        /// Draws handles and text around the box with measurement displayed.
        /// </summary>
        /// <param name="gameObject">GameObject to be measured, requires a RectTransform attached</param>
        private void DrawRectMeasurement(GameObject gameObject)
        {
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            if (rt == null) return;

            Vector3[] v = new Vector3[4];

            switch (settings.Scale)
            {
                case MeasureToolSettings.ToolScale.Local:
                    rt.GetLocalCorners(v);
                    TransformPoints(gameObject, v);
                    break;
                case MeasureToolSettings.ToolScale.World:
                default:
                    rt.GetWorldCorners(v);
                    break;
            }

            Vector3 verticalOffset = gameObject.transform.right * settings.Offset;
            Vector3 horizontalOffset = gameObject.transform.up * settings.Offset;

            Handles.color = settings.LineColor;

            DrawRectFromVectorArray(v);
            DrawHandle(v[0], v[3], -horizontalOffset, DistanceInUnits(Vector3.Distance(v[0], v[3])), Color.red);
            DrawHandle(v[3], v[2], verticalOffset, DistanceInUnits(Vector3.Distance(v[3], v[2])), Color.green);
        }

        /// <summary>
        /// Gets bounds from attached 3D collider and draws a box based on those bounds.
        /// Draws handles and text around the box with measurement displayed.
        /// </summary>
        /// <param name="gameObject">GameObject to be measured, requires a 3D collider attached</param>
        private void DrawColliderMeasurement(GameObject gameObject)
        {
            Collider col = gameObject.GetComponent<Collider>();
            if (col == null) return;

            Vector3[] array = VerticesFromBounds(col.bounds);

            if (settings.Scale == MeasureToolSettings.ToolScale.Local)
            {
                if (col.GetType() == typeof(SphereCollider))
                {
                    SphereCollider sphereCollider = (SphereCollider)col;

                    array = VerticesFromSize(sphereCollider.center, Vector3.one * sphereCollider.radius * 2);
                    TransformPoints(gameObject, array);
                }
                else if (col.GetType() == typeof(BoxCollider))
                {
                    BoxCollider boxCol = (BoxCollider)col;

                    array = VerticesFromSize(boxCol.center, boxCol.size);
                    TransformPoints(gameObject, array);
                }
                else if (col.GetType() == typeof(CapsuleCollider))
                {
                    CapsuleCollider capsuleCol = (CapsuleCollider)col;

                    array = VerticesFromSize(capsuleCol.center, new Vector3(capsuleCol.radius * 2f, capsuleCol.height, capsuleCol.radius * 2f));
                    TransformPoints(gameObject, array);
                }

                else if (col.GetType() == typeof(MeshCollider))
                {
                    MeshCollider meshCol = (MeshCollider)col;

                    array = VerticesFromBounds(meshCol.sharedMesh.bounds);
                    TransformPoints(gameObject, array);
                }
                DrawCubeFromVectorArray(array, gameObject.transform);
                return;
            }
            DrawCubeFromVectorArray(array, null);
        }

        /// <summary>
        ///Gets bounds from attached Renderer and draws a box based on those bounds.
        ///Draws handles and text around the box with measurement displayed. 
        /// </summary>
        ///<param name = "gameObject" > GameObject to be measured, requires a Renderer attached</param>
        private void DrawRendererMeasurement(GameObject gameObject)
        {
            Renderer rend = gameObject.GetComponent<Renderer>();
            if (rend == null) return;

            Handles.color = settings.LineColor;

#if UNITY_2021_2_OR_NEWER
            Vector3[] array = VerticesFromBounds(rend.localBounds);
#else
            Mesh mesh = rend.GetComponent<MeshFilter>().sharedMesh;
            Vector3[] array = VerticesFromBounds(mesh.bounds);
#endif
            if (settings.Scale == MeasureToolSettings.ToolScale.Local)
            {
                TransformPoints(gameObject, array);
                DrawCubeFromVectorArray(array, gameObject.transform);
            }
            else
            {
                array = VerticesFromBounds(rend.bounds);
                DrawCubeFromVectorArray(array, null);
            }

        }

        /// <summary>
        /// Utility method to transform a Vector3 array from local space to world space
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="array"></param>
        private void TransformPoints(GameObject gameObject, Vector3[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = gameObject.transform.TransformPoint(array[i]);
            }
        }

        /// <summary>
        /// Utility method for drawing a cube and handles from a Vector3[8]
        /// Used instead of Gizmos.DrawWireCube for Oriented Bounding Box support
        /// </summary>
        /// <param name="array">Vector3[8] containing all corner positions of the cube</param>
        /// <param name="attachedTransform">Transform required to ensure handles rotate with cube</param>
        private void DrawCubeFromVectorArray(Vector3[] array, Transform attachedTransform)
        {
            Vector3 zOffset = Vector3.forward * settings.Offset * 10f;
            Vector3 xOffset = Vector3.right * settings.Offset * 10f;
            Vector3 diagonalOffset = (Vector3.right + Vector3.forward).normalized * settings.Offset * 10f;

            DrawLine(array[5], array[1]);
            DrawLine(array[1], array[7]);
            DrawLine(array[7], array[3]);
            DrawLine(array[3], array[5]);

            DrawLine(array[2], array[6]);
            DrawLine(array[6], array[4]);
            DrawLine(array[4], array[0]);
            DrawLine(array[0], array[2]);

            DrawLine(array[5], array[2]);
            DrawLine(array[1], array[6]);
            DrawLine(array[7], array[4]);
            DrawLine(array[3], array[0]);

            if (attachedTransform != null)
            {
                zOffset = attachedTransform.forward * settings.Offset * 10f;
                xOffset = attachedTransform.right * settings.Offset * 10f;
                diagonalOffset = (attachedTransform.right + attachedTransform.forward).normalized * settings.Offset * 10f;
            }

            DrawHandle(array[4], array[0], -zOffset, DistanceInUnits(Vector3.Distance(array[4], array[0])), Color.red);
            DrawHandle(array[0], array[2], -xOffset, DistanceInUnits(Vector3.Distance(array[0], array[2])), Color.blue);
            DrawHandle(array[3], array[0], -diagonalOffset, DistanceInUnits(Vector3.Distance(array[3], array[0])), Color.green);
        }

        /// <summary>
        /// Utility method for drawing a rectangle from a Vector3[4]
        /// </summary>
        /// <param name="array">Vector3[4] containing all corner positions of the rectangle</param>
        private void DrawRectFromVectorArray(Vector3[] array)
        {
            DrawLine(array[0], array[1]);
            DrawLine(array[1], array[2]);
            DrawLine(array[2], array[3]);
            DrawLine(array[3], array[0]);
        }

        /// <summary>
        /// Draws a line and measurement between GameObjects
        /// </summary>
        /// <param name="gameObjects">List of GameObjects to be measured</param>
        private void DrawDistanceBetweenObjects(List<GameObject> gameObjects)
        {
            for (int i = 1; i < gameObjects.Count; i++)
            {
                var pos1 = gameObjects[i].transform.position;
                var pos2 = gameObjects[i - 1].transform.position;

                Handles.color = settings.LineColor;
                Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, settings.LineThickness, pos1, pos2);

                GUIStyle style = GetTextStyle();
                Handles.Label(Vector3.Lerp(pos1, pos2, 0.5f), DistanceInUnits(Vector3.Distance(pos1, pos2)), style);
            }
        }

        /// <summary>
        /// Utility method that returns a Vector3[8] from a Bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private Vector3[] VerticesFromBounds(Bounds bounds)
        {
            var vertices = new Vector3[8];
            vertices[0] = bounds.min;
            vertices[1] = bounds.max;
            vertices[2] = new Vector3(vertices[0].x, vertices[0].y, vertices[1].z);
            vertices[3] = new Vector3(vertices[0].x, vertices[1].y, vertices[0].z);
            vertices[4] = new Vector3(vertices[1].x, vertices[0].y, vertices[0].z);
            vertices[5] = new Vector3(vertices[0].x, vertices[1].y, vertices[1].z);
            vertices[6] = new Vector3(vertices[1].x, vertices[0].y, vertices[1].z);
            vertices[7] = new Vector3(vertices[1].x, vertices[1].y, vertices[0].z);

            return vertices;
        }

        /// <summary>
        /// Utility method that given a position and a size, returns a Vector3[8] of points around the position
        /// </summary>
        /// <param name="pos">Position, used as the center of the cube</param>
        /// <param name="size"></param>
        /// <returns></returns>
        private Vector3[] VerticesFromSize(Vector3 pos, Vector3 size)
        {
            size = size * 0.5f;
            size = pos + size;
            var vertices = new Vector3[8];
            vertices[0] = new Vector3(-size.x, -size.y, -size.z);
            vertices[1] = new Vector3(size.x, size.y, size.z);
            vertices[2] = new Vector3(-size.x, -size.y, size.z);
            vertices[3] = new Vector3(-size.x, size.y, -size.z);
            vertices[4] = new Vector3(size.x, -size.y, -size.z);
            vertices[5] = new Vector3(-size.x, size.y, size.z);
            vertices[6] = new Vector3(size.x, -size.y, size.z);
            vertices[7] = new Vector3(size.x, size.y, -size.z);

            return vertices;
        }

        /// <summary>
        /// Utility method to define a consistent Text style
        /// </summary>
        /// <returns>Method returns a GUIStyle </returns>
        private GUIStyle GetTextStyle()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = settings.TextColor;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = (int)settings.TextSize;
            style.fontStyle = FontStyle.Bold;
            return style;
        }

        /// <summary>
        /// Utility method to format a float value as a measurement including the units.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private string DistanceInUnits(float val)
        {
            switch (settings.Unit)
            {
                case MeasureToolSettings.ToolUnits.Millimeter:
                    return $"{(val * 1000).ToString("0.##")} mm";
                case MeasureToolSettings.ToolUnits.Centimeter:
                    return $"{(val * 100).ToString("0.##")} cm";
                case MeasureToolSettings.ToolUnits.Meter:
                default:
                    return $"{val.ToString("0.##")} m";
            }
        }

        /// <summary>
        /// Utility method to Draw a colored line between two Vector3 positions
        /// </summary>
        /// <param name="start">Line start position</param>
        /// <param name="end">Line end position</param>
        /// <param name="color">Line color</param>
        private void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            Handles.color = color;
            Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, settings.LineThickness, start, end);
        }

        /// <summary>
        /// Utility method to Draw a line between two Vector3 positions
        /// </summary>
        /// <param name="start">Line start position</param>
        /// <param name="end">Line end position</param>
        private void DrawLine(Vector3 start, Vector3 end)
        {
            DrawLine(start, end, settings.LineColor);
        }

        /// <summary>
        /// Utility method to Draw a colored line and some text between two Vector3 positions
        /// </summary>
        /// <param name="start">Line start position</param>
        /// <param name="end">Line end position</param>
        /// <param name="offset">Handle offset</param>
        /// <param name="text">Text rendered on the line</param>
        /// <param name="color">Line color</param>
        private void DrawHandle(Vector3 start, Vector3 end, Vector3 offset, string text, Color color)
        {
            DrawLine(start + offset, end + offset, color);
            Handles.DrawDottedLines(new Vector3[] { start, start + offset }, 4f);
            Handles.DrawDottedLines(new Vector3[] { end, end + offset }, 4f);
            Handles.Label(Vector3.Lerp(start, end, 0.5f) + offset, text, GetTextStyle());
        }

        public void OnSelectionChanged()
        {
            if (Selection.count == 0)
            {
                selectedObjects.Clear();
                return;
            }
            if (Selection.count == 1)
            {
                selectedObjects.Clear();
                selectedObjects.Add(Selection.activeGameObject);
                return;
            }

            foreach (var item in Selection.gameObjects)
            {
                if (!selectedObjects.Contains(item))
                {
                    selectedObjects.Add(item);
                }
            }

            //check for removed items
            List<GameObject> objectsToRemove = new List<GameObject>();
            foreach (var item in selectedObjects)
            {
                if (!Selection.Contains(item))
                {
                    objectsToRemove.Add(item);
                }
            }

            foreach (var item in objectsToRemove)
            {
                selectedObjects.Remove(item);
            }
        }
    }
}

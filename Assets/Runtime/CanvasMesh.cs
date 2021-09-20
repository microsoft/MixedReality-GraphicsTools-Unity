// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.GraphicsTools
{
    public enum ZScaleMode
    {
        Width,
        Height,
        Area,
        Manual,
        Flatten
    }

    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasRenderer))]
    public class CanvasMesh : Graphic
    {
        // Inspector properties
        public Mesh Mesh = null;

        public ZScaleMode ZScaleMode = ZScaleMode.Area;

        public float ManualZScale = 1.0f;

        public Vector3 OffsetPosition;
        public Vector3 OffsetRotation;

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector4> tangents = new List<Vector4>();

        /// <summary>
        /// Callback function when a UI element needs to generate vertices.
        /// </summary>
        /// <param name="vh">VertexHelper utility.</param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            verts.Clear();
            uvs.Clear();
            tris.Clear();
            normals.Clear();
            tangents.Clear();

            if (Mesh == null) return;

            // Get data from mesh
            Mesh.GetVertices(verts);
            Mesh.GetUVs(0, uvs);
            Mesh.GetTriangles(tris, 0);
            Mesh.GetNormals(normals);
            Mesh.GetTangents(tangents);

            // Get mesh bounds parameters
            Vector3 meshMin = Mesh.bounds.min;
            Vector3 meshSize = Mesh.bounds.size;

            Vector3 rectPivot = rectTransform.pivot;
            Vector3 rectSize = rectTransform.rect.size;

            switch (ZScaleMode)
            {
                case ZScaleMode.Width:
                    rectSize.z = rectSize.x;
                    break;
                case ZScaleMode.Height:
                    rectSize.z = rectSize.y;
                    break;
                case ZScaleMode.Area:
                    rectSize.z = Mathf.Sqrt(rectSize.x * rectSize.y);
                    break;
                case ZScaleMode.Manual:
                    rectSize.z = ManualZScale;
                    break;
                case ZScaleMode.Flatten:
                    rectSize.z = 0.0f;
                    break;
            }

            Quaternion offsetRotation = Quaternion.Euler(OffsetRotation);

            // Add scaled vertices
            for (int ii = 0; ii < verts.Count; ii++)
            {
                Vector3 v = verts[ii];
                v.x = (v.x - meshMin.x) / meshSize.x;
                v.y = (v.y - meshMin.y) / meshSize.y;
                v = Vector3.Scale(v - rectPivot, rectSize);

                v += OffsetPosition;

                v = offsetRotation * v;

                vh.AddVert(v, color, uvs[ii], uvs[ii], normals[ii], tangents[ii]);
            }
            // Add triangles
            for (int ii = 0; ii < tris.Count; ii += 3)
                vh.AddTriangle(tris[ii], tris[ii + 1], tris[ii + 2]);
        }

        protected override void UpdateMaterial()
        {
            if (!IsActive())
                return;

            canvasRenderer.materialCount = 1;
            canvasRenderer.SetMaterial(materialForRendering, 0);
        }

        /// <summary>
        /// Converts a vertex in mesh coordinates to a point in world coordinates.
        /// </summary>
        /// <param name="vertex">The input vertex.</param>
        /// <returns>A point in world coordinates.</returns>
        public Vector3 TransformVertex(Vector3 vertex)
        {
            // Convert vertex into local coordinates
            Vector2 v;
            v.x = (vertex.x - Mesh.bounds.min.x) / Mesh.bounds.size.x;
            v.y = (vertex.y - Mesh.bounds.min.y) / Mesh.bounds.size.y;
            v = Vector2.Scale(v - rectTransform.pivot, rectTransform.rect.size);
            // Convert from local into world
            return transform.TransformPoint(v);
        }

        /// <summary>
        /// Converts a vertex in world coordinates into a vertex in mesh coordinates.
        /// </summary>
        /// <param name="vertex">The input vertex.</param>
        /// <returns>A point in mesh coordinates.</returns>
        public Vector3 InverseTransformVertex(Vector3 vertex)
        {
            // Convert from world into local coordinates
            Vector2 v = transform.InverseTransformPoint(vertex);
            // Convert into mesh coordinates
            v.x /= rectTransform.rect.size.x;
            v.y /= rectTransform.rect.size.y;
            v += rectTransform.pivot;
            v = Vector2.Scale(v, Mesh.bounds.size);
            v.x += Mesh.bounds.min.x;
            v.y += Mesh.bounds.min.y;
            return v;
        }
    }
}

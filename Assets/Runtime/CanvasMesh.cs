// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Allows a 3D mesh to be rendered within UnityUI canvas. 
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasRenderer))]
    public class CanvasMesh : Graphic
    {
        [Tooltip("The source mesh to use for populating the Graphic with vertex information.")]
        [SerializeField]
        [FormerlySerializedAs("Mesh")]
        private Mesh mesh = null;

        /// <summary>
        /// The source mesh to use for populating the Graphic with vertex information.
        /// </summary>
        public Mesh Mesh
        {
            get => mesh;
            set
            {
                mesh = value;
                UpdateGeometry();
            }
        }

        [Tooltip("Whether this CanvasMesh should preserve its source mesh aspect ratio (scale).")]
        [SerializeField]
        private bool preserveAspect = true;

        /// <summary>
        /// Whether this CanvasMesh should preserve its source mesh aspect ratio (scale).
        /// </summary>
        public bool PreserveAspect
        {
            get => preserveAspect;
            set
            {
                preserveAspect = value;
                UpdateGeometry();
            }
        }

        private Mesh PreviousMesh = null;
        private List<UIVertex> uiVerticies = new List<UIVertex>();
        private List<int> uiIndices = new List<int>();

        #region Graphic Implementation

        /// <summary>
        /// Callback function when a UI element needs to generate vertices.
        /// </summary>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            RefresheMesh();

            if (Mesh == null || uiVerticies.Count == 0)
            {
                return;
            }

            Vector3 meshSize = Mesh.bounds.size;
            Vector3 rectSize = rectTransform.rect.size;
            rectSize.z = Mathf.Min(rectSize.x, rectSize.y);

            if (preserveAspect)
            {
                float meshRatio = meshSize.x / meshSize.y;
                float rectRatio = rectSize.x / rectSize.y;
                float scaler;

                // Wide
                if (meshSize.x > meshSize.y)
                {
                    scaler = rectRatio > meshRatio ? rectSize.y * meshRatio : rectSize.x;
                }
                else // Tall
                {
                    scaler = rectRatio > meshRatio ? rectSize.y : rectSize.x * (1.0f / meshRatio);
                }

                rectSize = new Vector3(scaler, scaler, scaler);
            }

            Vector3 rectPivot = rectTransform.pivot;
            List<UIVertex> uiVerticiesTRS = new List<UIVertex>(uiVerticies);

            // Scale, translate and rotate vertices.
            for (int i = 0; i < uiVerticiesTRS.Count; i++)
            {
                UIVertex vertex = uiVerticiesTRS[i];

                // Scale the vector from the normalized position to the pivot by the rect size.
                vertex.position = Vector3.Scale(vertex.position - rectPivot, rectSize);

                uiVerticiesTRS[i] = vertex;
            }

            vh.AddUIVertexStream(uiVerticiesTRS, uiIndices);
        }

        /// <summary>
        /// Called to update the Material of the graphic onto the CanvasRenderer.
        /// </summary>
        protected override void UpdateMaterial()
        {
            if (!IsActive())
            {
                return;
            }

            canvasRenderer.materialCount = 1;
            canvasRenderer.SetMaterial(materialForRendering, 0);
        }

        #endregion Graphic Implementation

        /// <summary>
        /// Determines if vertex attributes within the Mesh need to be re-cached.
        /// </summary>
        [ContextMenu("Refresh Mesh")]
        private void RefresheMesh()
        {
            if (PreviousMesh != Mesh || uiVerticies.Count == 0)
            {
                uiVerticies.Clear();
                uiIndices.Clear();

                if (Mesh != null)
                {
                    List<Vector3> vertices = new List<Vector3>();
                    Mesh.GetVertices(vertices);
                    List<Vector2> uv0s = new List<Vector2>();
                    Mesh.GetUVs(0, uv0s);
                    List<Vector2> uv1s = new List<Vector2>();
                    Mesh.GetUVs(1, uv1s);
                    List<Vector2> uv2s = new List<Vector2>();
                    Mesh.GetUVs(2, uv2s);
                    List<Vector2> uv3s = new List<Vector2>();
                    Mesh.GetUVs(3, uv3s);
                    List<Vector3> normals = new List<Vector3>();
                    Mesh.GetNormals(normals);
                    List<Vector4> tangents = new List<Vector4>();
                    Mesh.GetTangents(tangents);

                    Vector3 rectPivot = new Vector3(0.5f, 0.5f, 0);

                    Vector3 meshCenter = Mesh.bounds.center;
                    Vector3 meshSize = Mesh.bounds.extents;

                    float scaler = 0.5f / Mathf.Max(meshSize.x, meshSize.y);

                    for (int i = 0; i < vertices.Count; ++i)
                    {
                        UIVertex vertex = new UIVertex();
                        vertex.position = vertices[i];

                        // Center the mesh at the origin.
                        vertex.position -= meshCenter;

                        // Normalize the mesh in a 1x1x1 cube.
                        vertex.position *= scaler;

                        // Center the mesh at the pivot.
                        vertex.position += rectPivot;

                        // Set the other attributes.
                        vertex.normal = normals[i];
                        vertex.tangent = tangents[i];
                        vertex.color = color;

                        if (i < uv0s.Count)
                        {
                            vertex.uv0 = uv0s[i];
                        }

                        if (i < uv1s.Count)
                        {
                            vertex.uv1 = uv1s[i];
                        }

                        if (i < uv2s.Count)
                        {
                            vertex.uv2 = uv2s[i];
                        }

                        if (i < uv3s.Count)
                        {
                            vertex.uv3 = uv3s[i];
                        }

                        uiVerticies.Add(vertex);
                    }

                    Mesh.GetTriangles(uiIndices, 0);
                }

                PreviousMesh = Mesh;
            }
        }
    }
}

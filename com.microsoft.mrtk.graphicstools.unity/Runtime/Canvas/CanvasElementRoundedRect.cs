// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_UGUI
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Procedurally generates a 3D rounded rect mesh to be rendered within a UnityUI canvas.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class CanvasElementRoundedRect : MaskableGraphic
    {
        [Tooltip("Specifies the corner radius of the rounded rect in world units. Should be less than half the width or height.")]
        [SerializeField]
        private float radius = 10f;

        /// <summary>
        /// Specifies the corner radius in world units. Should be less than half the width or height.
        /// </summary>
        public float Radius
        {
            get => radius;
            set
            {
                radius = value;
                SetVerticesDirty();
            }
        }

        [Tooltip("Controls the depth of the rounded rect along the local z axis.")]
        [SerializeField]
        private float thickness = 1f;

        /// <summary>
        /// Controls the depth of the rounded rect along the local z axis.
        /// </summary>
        public float Thickness
        {
            get => thickness;
            set
            {
                thickness = value;
                SetVerticesDirty();
            }
        }

        [Tooltip("Controls how many subdivisions at the corners of the rounded rect. More wedges produces smoother corners at the expense of more triangles.")]
        [SerializeField]
        private int wedges = 8;

        /// <summary>
        /// Controls how many subdivisions at the corners of the rounded rect. More wedges produces smoother corners at the expense of more triangles.
        /// </summary>
        public int Wedges
        {
            get => wedges;
            set
            {
                wedges = value;
                SetVerticesDirty();
            }
        }

        [Tooltip("Apply anti-aliasing to silhouette edges of the rounded rectangle (also requires enabling this feature in the material).")]
        [SerializeField]
        private bool calculateSmoothEdges = true;

        public bool SmoothEdges
        {
            get => calculateSmoothEdges;
            set
            {
                calculateSmoothEdges = value;
                SetVerticesDirty();
            }
        }


#region override methods
        /// <summary>
        /// Ensures the canvas generates required vertex attributes.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord3;
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
                if (SmoothEdges)
                {
                    // store normal of adjacent face as tangent
                    canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;
                }
            }
        }

        /// <summary>
        /// When the mesh changes, rebuilds the vertices.
        /// </summary>
        /// <param name="vh">Vertex stream to populate.</param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var uv2 = new Vector2(rectTransform.rect.width * rectTransform.localScale.x,
                                    rectTransform.rect.height * rectTransform.localScale.y);
            var uv3 = new Vector2(radius / rectTransform.rect.height, 0.0f);
            AddRoundedRect(vh, rectTransform.rect.min, rectTransform.rect.max, rectTransform.localScale, uv2, uv3, radius);
        }

        /// <summary>
        /// Marks the vertices dirty when the rect changes.
        /// </summary>
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
            SetMaterialDirty();
        }

        /// <summary>
        /// Marks the vertices dirty when the parent changes.
        /// </summary>
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            SetVerticesDirty();
            SetMaterialDirty();
        }

#endregion

#region private methods

        private UIVertex vert = new UIVertex();

        private void AddRoundedRect(VertexHelper vh, Vector2 min, Vector2 max, Vector2 scale, Vector2 uv2, Vector2 uv3, float radius)
        {
            vert.uv2 = uv2;
            vert.uv3 = uv3;

            float aspect = scale.y / scale.x;
            float rx = radius * aspect;

            float x1 = min.x + rx;
            float x2 = max.x - rx;
            float y1 = min.y + radius;
            float y2 = max.y - radius;

            float u1 = rx / (max.x - min.x);
            float u2 = 1.0f - u1;

            float v1 = radius / (max.y - min.y);
            float v2 = 1.0f - v1;

            int i10 = AddVertex(vh, x1, min.y, u1, 0.0f, Vector3.forward, Vector3.down);
            int i20 = AddVertex(vh, x2, min.y, u2, 0.0f, Vector3.forward, Vector3.down);

            int i01 = AddVertex(vh, min.x, y1, 0.0f, v1, Vector3.forward, Vector3.left);
            int i11 = AddVertex(vh, x1, y1, u1, v1);
            int i21 = AddVertex(vh, x2, y1, u2, v1);
            int i31 = AddVertex(vh, max.x, y1, 1.0f, v1, Vector3.forward, Vector3.right);

            int i02 = AddVertex(vh, min.x, y2, 0.0f, v2, Vector3.forward, Vector3.left);
            int i12 = AddVertex(vh, x1, y2, u1, v2);
            int i22 = AddVertex(vh, x2, y2, u2, v2);
            int i32 = AddVertex(vh, max.x, y2, 1.0f, v2, Vector3.forward, Vector3.right);

            int i13 = AddVertex(vh, x1, max.y, u1, 1.0f, Vector3.forward, Vector3.up);
            int i23 = AddVertex(vh, x2, max.y, u2, 1.0f, Vector3.forward, Vector3.up);

            AddQuad(vh, i10, i20, i11, i21);
            AddQuad(vh, i01, i11, i02, i12);
            AddQuad(vh, i11, i21, i12, i22);
            AddQuad(vh, i21, i31, i22, i32);
            AddQuad(vh, i12, i22, i13, i23);

            AddEdge(vh, i10, i20, Vector3.down, Vector3.down);
            AddEdge(vh, i31, i32, Vector3.right, Vector3.right);
            AddEdge(vh, i23, i13, Vector3.up, Vector3.up);
            AddEdge(vh, i02, i01, Vector3.left, Vector3.left);

            AddCorner(vh, new Vector2(x1, y1), i11, i01, i10, rx, radius, Vector2.left, Vector2.down, new Vector2(u1, v1), u1, v1);
            AddCorner(vh, new Vector2(x2, y1), i21, i20, i31, radius, rx, Vector2.down, Vector2.right, new Vector2(u2, v1), v1, u1);
            AddCorner(vh, new Vector2(x1, y2), i12, i13, i02, radius, rx, Vector2.up, Vector2.left, new Vector2(u1, v2), v1, u1);
            AddCorner(vh, new Vector2(x2, y2), i22, i32, i23, rx, radius, Vector2.right, Vector2.up, new Vector2(u2, v2), u1, v1);
        }

        private int AddVertex(VertexHelper vh, float x, float y, float u, float v, Vector3 normal, Vector3 tangent)
        {
            int ix = vh.currentVertCount;

            vert.color = this.color;

            vert.position = new Vector3(x, y, Mathf.Max(0.0f, thickness));
            vert.uv0 = new Vector2(u, v);
            vert.normal = normal;
            if (SmoothEdges)
            {
                vert.tangent = tangent;
            }
            vh.AddVert(vert);

            if (thickness != 0.0f)
            {
                vert.position = new Vector3(x, y, Mathf.Min(0.0f, thickness));
                vert.normal = new Vector3(normal.x, normal.y, -normal.z);
                if (SmoothEdges)
                {
                    vert.tangent = new Vector3(tangent.x, tangent.y, -tangent.z);
                }
                vh.AddVert(vert);
            }

            return ix;
        }

        private int AddVertex(VertexHelper vh, float x, float y, float u, float v)
        {
            return AddVertex(vh, x, y, u, v, Vector3.forward, Vector3.forward);
        }

        private void AddQuad(VertexHelper vh, int v00, int v10, int v01, int v11)
        {
            vh.AddTriangle(v00, v10, v11);
            vh.AddTriangle(v00, v11, v01);

            if (thickness != 0.0f)
            {
                vh.AddTriangle(v00 + 1, v11 + 1, v10 + 1);
                vh.AddTriangle(v00 + 1, v01 + 1, v11 + 1);
            }
        }

        private void AddTriangle(VertexHelper vh, int i0, int i1, int i2)
        {
            vh.AddTriangle(i0, i1, i2);
            if (thickness != 0.0f)
            {
                vh.AddTriangle(i0 + 1, i2 + 1, i1 + 1);
            }
        }

        private void AddEdge(VertexHelper vh, int i0, int i1, Vector3 normal0, Vector3 normal1)
        {
            if (thickness != 0.0f)
            {
                vh.PopulateUIVertex(ref vert, i0);
                int j0 = AddVertex(vh, vert.position.x, vert.position.y, vert.uv0.x, vert.uv0.y, normal0, Vector3.forward);

                vh.PopulateUIVertex(ref vert, i1);
                int j1 = AddVertex(vh, vert.position.x, vert.position.y, vert.uv0.x, vert.uv0.y, normal1, Vector3.forward);

                vh.AddTriangle(j0, j1 + 1, j1);
                vh.AddTriangle(j0, j0 + 1, j1 + 1);
            }
        }

        private void AddCorner(VertexHelper vh, Vector2 center, int centerix, int startix, int endix, float rx, float ry, Vector2 xaxis, Vector2 yaxis, Vector2 uvCenter, float ur, float vr)
        {
            if (SmoothEdges)
            {
                AddCornerTangents(vh, center, centerix, startix, endix, rx, ry, xaxis, yaxis, uvCenter, ur, vr);
            } else
            {
                AddCornerNoTangents(vh, center, centerix, startix, endix, rx, ry, xaxis, yaxis, uvCenter, ur, vr);
            }
        }

        private void AddCornerTangents(VertexHelper vh, Vector2 center, int centerix, int startix, int endix, float rx, float ry, Vector2 xaxis, Vector2 yaxis, Vector2 uvCenter, float ur, float vr)
        {
            vh.PopulateUIVertex(ref vert, startix);
            Vector2 lastp = center + rx * xaxis;
            Vector2 lastuv = uvCenter + ur * xaxis;
            for (int i = 1; i <= wedges; i++)
            {
                float angle = (float)i / (float)wedges * 3.14159f * 0.5f;
                float cosa = Mathf.Cos(angle);
                float sina = Mathf.Sin(angle);

                Vector2 newp = center + rx * cosa * xaxis + ry * sina * yaxis;
                Vector2 newuv = uvCenter + ur * cosa * xaxis + vr * sina * yaxis;

                Vector2 tangent = (newp+lastp)*0.5f - center;

                int ix0 = AddVertex(vh, lastp.x, lastp.y, lastuv.x, lastuv.y, Vector3.forward, tangent);
                int ix1 = AddVertex(vh, newp.x, newp.y, newuv.x, newuv.y, Vector3.forward, tangent);

                AddTriangle(vh, centerix, ix0, ix1);
                AddEdge(vh, ix0, ix1, lastp-center, newp-center);
                lastp = newp;
                lastuv = newuv;
            }
        }

        private void AddCornerNoTangents(VertexHelper vh, Vector2 center, int centerix, int startix, int endix, float rx, float ry, Vector2 xaxis, Vector2 yaxis, Vector2 uvCenter, float ur, float vr)
        {
            int lastix = startix;
            vh.PopulateUIVertex(ref vert, startix);
            Vector2 lastTangent = new Vector2(vert.position.x, vert.position.y) - center;
            Vector2 tangent;
            for (int i = 1; i < wedges; i++)
            {
                float angle = (float)i / (float)wedges * 3.14159f * 0.5f;
                float cosa = Mathf.Cos(angle);
                float sina = Mathf.Sin(angle);

                Vector2 newp = center + rx * cosa * xaxis + ry * sina * yaxis;
                Vector2 newuv = uvCenter + ur * cosa * xaxis + vr * sina * yaxis;

                tangent = newp - center;

                int ix = AddVertex(vh, newp.x, newp.y, newuv.x, newuv.y, Vector3.forward, tangent);
                AddTriangle(vh, centerix, lastix, ix);
                AddEdge(vh, lastix, ix, lastTangent, tangent);
                lastix = ix;
                lastTangent = tangent;
            }
            AddTriangle(vh, centerix, lastix, endix);
            vh.PopulateUIVertex(ref vert, endix);
            tangent = new Vector2(vert.position.x, vert.position.y) - center;
            AddEdge(vh, lastix, endix, lastTangent, tangent);
        }

#endregion
    }
}
#endif // GT_USE_UGUI
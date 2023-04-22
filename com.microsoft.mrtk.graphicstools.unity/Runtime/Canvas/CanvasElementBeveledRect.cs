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
    public class CanvasElementBeveledRect : MaskableGraphic
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
        private float thickness = 3f;

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

        [Header("Bevel")]

        [Tooltip("Specifies the front bevel radius in world units. Should be less than the corner radius.")]
        [SerializeField]
        private float frontBevelRadius = 1f;

        /// <summary>
        /// Specifies the front bevel radius in world units. Should be less than the radius
        /// </summary>
        public float FrontBevelRadius
        {
            get => frontBevelRadius;
            set
            {
                frontBevelRadius = value;
                SetVerticesDirty();
            }
        }

        [Tooltip("Specifies the front bevel radius in world units. Should be less than the corner radius.")]
        [SerializeField]
        private int frontBevelSections = 4;

        /// <summary>
        /// Specifies the front bevel radius in world units. Should be less than half the radius
        /// </summary>
        public int FrontBevelSections
        {
            get => frontBevelSections;
            set
            {
                frontBevelSections = value;
                SetVerticesDirty();
            }
        }

        [Tooltip("Specifies the back bevel radius in world units. Should be less than the corner radius.")]
        [SerializeField]
        private float backBevelRadius = 1f;

        /// <summary>
        /// Specifies the back bevel radius in world units. Should be less than the radius
        /// </summary>
        public float BackBevelRadius
        {
            get => backBevelRadius;
            set
            {
                backBevelRadius = value;
                SetVerticesDirty();
            }
        }

        [Tooltip("Specifies the back bevel radius in world units. Should be less than the corner radius.")]
        [SerializeField]
        private int backBevelSections = 4;

        /// <summary>
        /// Specifies the back bevel radius in world units. Should be less than half the radius
        /// </summary>
        public int BackBevelSections
        {
            get => backBevelSections;
            set
            {
                backBevelSections = value;
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
        private Vector2 uvOrigin = Vector2.zero;
        private Vector2 uvSize = Vector2.one;

        private void AddRoundedRect(VertexHelper vh, Vector2 min, Vector2 max, Vector3 scale, Vector2 uv2, Vector2 uv3, float radius)
        {
            vert.uv2 = uv2;
            vert.uv3 = uv3;

            float aspect = scale.y / scale.x;
            float zaspect = scale.y / scale.z;
            float rx = radius * aspect;

            float x1 = min.x + rx;
            float x2 = max.x - rx;
            float y1 = min.y + radius;
            float y2 = max.y - radius;

            uvOrigin = min;
            uvSize = max - min;

            float frontRadius, backRadius;
            int frontSections, backSections;

            if (frontBevelRadius > 0.0f && frontBevelSections > 0)
            {
                frontRadius = frontBevelRadius;
                frontSections = frontBevelSections;
            }
            else
            {
                frontRadius = 0.0f;
                frontSections = 0;
            }

            if (backBevelRadius > 0.0f && backBevelSections > 0)
            {
                backRadius = backBevelRadius;
                backSections = backBevelSections;
            }
            else
            {
                backRadius = 0.0f;
                backSections = 0;
            }

            int corner11 = vh.currentVertCount;
            AddBeveledCorner(vh, x1, y1, frontRadius, frontSections, backRadius, backSections, aspect, 1.0f, zaspect, Vector2.left, Vector2.down);
            int corner21 = vh.currentVertCount;
            AddBeveledCorner(vh, x2, y1, frontRadius, frontSections, backRadius, backSections, 1.0f, aspect, zaspect, Vector2.down, Vector2.right);
            int corner12 = vh.currentVertCount;
            AddBeveledCorner(vh, x1, y2, frontRadius, frontSections, backRadius, backSections, 1.0f, aspect, zaspect, Vector2.up, Vector2.left);
            int corner22 = vh.currentVertCount;
            AddBeveledCorner(vh, x2, y2, frontRadius, frontSections, backRadius, backSections, aspect, 1.0f, zaspect, Vector2.right, Vector2.up);

            int frontSize = (frontSections + 1) * (wedges + 1) + 1;
            int backSize = (backSections + 1) * (wedges + 1) + 1;

            ConnectEdges(vh, corner11, corner11 + frontSize, corner11 + frontSize - (frontSections + 1), corner11 + frontSize + backSize - (backSections + 1),
                             corner21, corner21 + frontSize, corner21 + 1, corner21 + frontSize + 1, frontSections, backSections);

            ConnectEdges(vh, corner21, corner21 + frontSize, corner21 + frontSize - (frontSections + 1), corner21 + frontSize + backSize - (backSections + 1),
                             corner22, corner22 + frontSize, corner22 + 1, corner22 + frontSize + 1, frontSections, backSections);

            ConnectEdges(vh, corner22, corner22 + frontSize, corner22 + frontSize - (frontSections + 1), corner22 + frontSize + backSize - (backSections + 1),
                             corner12, corner12 + frontSize, corner12 + 1, corner12 + frontSize + 1, frontSections, backSections);

            ConnectEdges(vh, corner12, corner12 + frontSize, corner12 + frontSize - (frontSections + 1), corner12 + frontSize + backSize - (backSections + 1),
                             corner11, corner11 + frontSize, corner11 + 1, corner11 + frontSize + 1, frontSections, backSections);

            // center panels
            AddQuad(vh, corner11, corner21, corner22, corner12, false);
            AddQuad(vh, corner11 + frontSize, corner21 + frontSize, corner22 + frontSize, corner12 + frontSize, true);
        }

        private void ConnectEdges(VertexHelper vh, int leftFrontCenter, int leftBackCenter, int leftFront, int leftBack, int rightFrontCenter, int rightBackCenter, int rightFront, int rightBack, int frontSections, int backSections)
        {
            AddQuad(vh, leftFrontCenter, leftFront + frontSections, rightFront + frontSections, rightFrontCenter, false);
            for (int i = 0; i < frontSections; i++)
            {
                AddQuad(vh, leftFront + i + 1, leftFront + i, rightFront + i, rightFront + i + 1, false);
            }
            AddQuad(vh, leftFront, leftBack, rightBack, rightFront, false);
            for (int i = 0; i < backSections; i++)
            {
                AddQuad(vh, leftBack + i, leftBack + i + 1, rightBack + i + 1, rightBack + i, false);
            }
            AddQuad(vh, leftBackCenter, leftBack + backSections, rightBack + backSections, rightBackCenter, true);
        }

        private void AddBeveledCorner(VertexHelper vh, float centerx, float centery, float frontRadius, int frontSections, float backRadius, int backSections, float rx, float ry, float rz, Vector2 xaxis, Vector2 yaxis)
        {
            Vector3 frontCenter = new Vector3(centerx, centery, Mathf.Max(0.0f, thickness));

            int frontedge = vh.currentVertCount;
            AddBeveledWedge(vh, frontSections, frontRadius, frontCenter, Vector3.forward, rx, ry, rz, xaxis, yaxis);

            Vector3 backCenter = new Vector3(centerx, centery, Mathf.Min(0.0f, thickness));
            int backedge = vh.currentVertCount;
            AddBeveledWedge(vh, backSections, backRadius, backCenter, Vector3.back, rx, ry, rz, xaxis, yaxis);

            // connect front and back
            for (int i = 1; i <= wedges; i++)
            {
                AddQuad(vh, frontedge + (i) * (frontSections + 1) + 1, frontedge + (i - 1) * (frontSections + 1) + 1, backedge + (i - 1) * (backSections + 1) + 1, backedge + i * (backSections + 1) + 1, false);
            }
        }

        private void AddBeveledWedge(VertexHelper vh, int nbevel, float bevelRadius, Vector3 center, Vector3 zdir, float rx, float ry, float rz, Vector2 xaxis, Vector2 yaxis)
        {
            int centerix = AddVertex(vh, center, zdir);
            bool reverse = zdir.z < 0.0f;
            float bevelAngleDivisor = Mathf.Max(1, nbevel);
            for (int i = 0; i <= wedges; i++)
            {
                float wedgeAngle = (float)i / wedges * Mathf.PI * 0.5f;
                Vector3 dir = (Mathf.Cos(wedgeAngle) * rx) * xaxis + (Mathf.Sin(wedgeAngle) * ry) * yaxis;
                Vector3 localCenter = center + dir * (radius - bevelRadius) - zdir * bevelRadius * rz;
                for (int j = 0; j <= nbevel; j++)
                {
                    float bevelAngle = (float)j / bevelAngleDivisor * Mathf.PI * 0.5f;
                    Vector3 p = localCenter + bevelRadius * (Mathf.Cos(bevelAngle) * dir + Mathf.Sin(bevelAngle) * zdir * rz);
                    Vector3 n = (p - localCenter).normalized;
                    int ix = AddVertex(vh, p, n);
                    if (i > 0)
                    {
                        if (j > 0)
                        {
                            AddQuad(vh, ix - 1, ix, ix - (nbevel + 1), ix - (nbevel + 2), reverse);
                        }
                        if (j == nbevel)
                        {
                            AddTriangle(vh, ix - (nbevel + 1), ix, centerix, reverse);
                        }
                    }
                }
            }
        }

        private int AddVertex(VertexHelper vh, Vector3 position, Vector3 normal)
        {
            int ix = vh.currentVertCount;
            vert.color = this.color;
            vert.position = position;
            vert.uv0 = new Vector2((position.x - uvOrigin.x) / uvSize.x, (position.y - uvOrigin.y) / uvSize.y);
            vert.normal = normal;
            vh.AddVert(vert);
            return ix;
        }

        private void AddQuad(VertexHelper vh, int v00, int v10, int v11, int v01, bool reverse)
        {
            AddTriangle(vh, v00, v10, v11, reverse);
            AddTriangle(vh, v00, v11, v01, reverse);
        }

        private void AddTriangle(VertexHelper vh, int i0, int i1, int i2, bool reverse)
        {
            if (reverse)
            {
                vh.AddTriangle(i0, i2, i1);
            }
            else
            {
                vh.AddTriangle(i0, i1, i2);
            }
        }
#endregion
    }
}
#endif // GT_USE_UGUI
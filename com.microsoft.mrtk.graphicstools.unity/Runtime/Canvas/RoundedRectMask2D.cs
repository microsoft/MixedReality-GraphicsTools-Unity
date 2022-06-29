// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// A 2D rounded rectangular mask that allows for clipping / masking of areas outside the mask.
    /// Constraints:
    /// - Material instancing must be controlled by the user. i.e shared materials may be effected by RoundedRectMask2D.
    /// - RoundedRectMask2D only work with Graphics Tools/Canvas/... shaders and the Graphics Tools/Standard Canvas shader.
    /// - Nested RoundedRectMask's work but are not advised since they will not consider grandparent, great grandparent, etc. radii.
    /// - Plus, same constraints as RectMask2D.
    /// </summary>
    public class RoundedRectMask2D : RectMask2DFast
    {
        public static readonly string RoundedKeyword = "_UI_CLIP_RECT_ROUNDED";
        public static readonly string RoundedIndependentKeyword = "_UI_CLIP_RECT_ROUNDED_INDEPENDENT";
        public static readonly string RadiiPropertyName = "_ClipRectRadii";

        [Tooltip("False if the rounded rect has the same radii for all four corners, true if they are all different.")]
        [SerializeField]
        protected bool independentRadii = false;

        /// <summary>
        /// False if the rounded rect has the same radii for all four corners, true if they are all different.
        /// </summary>
        public bool IndependentRadii
        {
            get => independentRadii;
            set
            {
                independentRadii = value;
                MaskUtilities.Notify2DMaskStateChanged(this);
                ForceClip = true;
            }
        }

        [Tooltip("The four corner radii of the rounded rect. (x: top left, y: top right, z: bottom left, w: bottom right)")]
        [SerializeField]
        protected Vector4 radii = Vector4.one * 10.0f;

        /// <summary>
        /// If IndependentRadii is true, the four corner radii of the rounded rect. (x: top left, y: top right, z: bottom left, w: bottom right)
        /// If IndependentRadii is false, the x value is used for all four corners of the rounded rec. (x: all, y: unused, z: unused, w: unused)
        /// </summary>
        public Vector4 Radii
        {
            get => radii;
            set
            {
                radii = value;
                MaskUtilities.Notify2DMaskStateChanged(this);
                ForceClip = true;
            }
        }

        private static int clipRectRadiiID = 0;

        #region RectMask2DFast Implementation

        /// <summary>
        /// Lazy initialize shader IDs.
        /// </summary>
        public override void PerformClipping()
        {
            if (clipRectRadiiID == 0)
            {
                clipRectRadiiID = Shader.PropertyToID(RadiiPropertyName);
            }

            base.PerformClipping();
        }

        /// <summary>
        /// enables shader keywords/properties based on the RoundedRectMask2D state.
        /// </summary>
        protected override void OnSetClipRect(MaskableGraphic maskableTarget)
        {
            Material targetMaterial = maskableTarget.materialForRendering;

            // TODO - [Cameron-Micka] for cleanliness we should reset these keywords/properties when done. But, since UnityUI controls the
            // material's lifetime it is difficult to achieve. Fortunately the "UNITY_UI_CLIP_RECT" keyword does get reset by UnityUI which
            // means the below keywords/properties no longer factor into shader computations but may dirty the material state.
            if (targetMaterial != null)
            {
                if (IndependentRadii)
                {
                    targetMaterial.EnableKeyword(RoundedIndependentKeyword);
                    targetMaterial.DisableKeyword(RoundedKeyword);
                }
                else
                {
                    targetMaterial.DisableKeyword(RoundedIndependentKeyword);
                    targetMaterial.EnableKeyword(RoundedKeyword);
                }

                targetMaterial.SetVector(clipRectRadiiID, Radii);
            }
        }

        #endregion RectMask2DFast Implementation
    }
}


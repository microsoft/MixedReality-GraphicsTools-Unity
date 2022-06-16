// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Component which can be used to render an outline around a hierarchy of mesh renderers using
    /// the <see cref="Microsoft.MixedReality.GraphicsTools.MeshOutline"/> component.
    /// </summary>
    [AddComponentMenu("Scripts/GraphicsTools/MeshOutlineHierarchy")]
    public class MeshOutlineHierarchy : BaseMeshOutline
    {
        private List<MeshOutline> meshOutlines = null;

        #region MonoBehaviour Implementation

        /// <summary>
        /// Creates a <see cref="Microsoft.MixedReality.GraphicsTools.MeshOutline"/> component on each child MeshRenderer.
        /// </summary>
        private void Awake()
        {
            Renderer[] meshRenderers = GetComponentsInChildren<Renderer>();
            meshOutlines = new List<MeshOutline>();

            for (int i = 0; i < meshRenderers.Length; ++i)
            {
                if (meshRenderers[i] as SpriteRenderer == null)
                {
                    var meshOutline = meshRenderers[i].gameObject.AddComponent<MeshOutline>();
                    meshOutline.OutlineMaterial = outlineMaterial;
                    meshOutline.OutlineWidth = outlineWidth;
                    meshOutlines.Add(meshOutline);
                }
            }
        }

        /// <summary>
        /// Removes any components this component has created.
        /// </summary>
        private void OnDestroy()
        {
            foreach (var meshOutline in meshOutlines)
            {
                Destroy(meshOutline);
            }
        }

        #endregion MonoBehaviour Implementation

        #region BaseMeshOutline Implementation

        /// <summary>
        /// Forwards the outlineMaterial to all children <see cref="Microsoft.MixedReality.GraphicsTools.MeshOutline"/>s.
        /// </summary>
        protected override void ApplyOutlineMaterial()
        {
            if (meshOutlines != null)
            {
                foreach (var meshOutline in meshOutlines)
                {
                    if (meshOutline != null)
                    {
                        meshOutline.OutlineMaterial = outlineMaterial;
                    }
                }
            }
        }

        /// <summary>
        /// Forwards the outlineWidth to all children <see cref="Microsoft.MixedReality.GraphicsTools.MeshOutline"/>s.
        /// </summary>
        protected override void ApplyOutlineWidth()
        {
            if (meshOutlines != null)
            {
                foreach (var meshOutline in meshOutlines)
                {
                    if (meshOutline != null)
                    {
                        meshOutline.OutlineWidth = outlineWidth;
                    }
                }
            }
        }

        #endregion BaseMeshOutline Implementation
    }
}

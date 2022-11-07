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
        /// Creates a <see cref="Microsoft.MixedReality.GraphicsTools.MeshOutline"/> component on each child MeshRenderer/SkinnedMeshRenderer.
        /// </summary>
        private void Awake()
        {
            meshOutlines = new List<MeshOutline>();

            var meshRenderers = GetComponentsInChildren<MeshRenderer>();
   
            for (int i = 0; i < meshRenderers.Length; ++i)
            {
                AddMeshOutline(meshRenderers[i]);
            }

            var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skinnedMeshRenderers.Length; ++i)
            {
                AddMeshOutline(skinnedMeshRenderers[i]);
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
        /// Forwards settings to all children <see cref="Microsoft.MixedReality.GraphicsTools.MeshOutline"/>s.
        /// </summary>
        public override void ApplyOutlineMaterial()
        {
            ApplyToChildren();
        }

        /// <summary>
        /// Forwards settings to all children <see cref="Microsoft.MixedReality.GraphicsTools.MeshOutline"/>s.
        /// </summary>
        public override void ApplyOutlineWidth()
        {
            ApplyToChildren();
        }

        #endregion BaseMeshOutline Implementation

        private void AddMeshOutline(Renderer target)
        {
            var meshOutline = target.gameObject.AddComponent<MeshOutline>();
            meshOutline.CopyFrom(this);
            meshOutlines.Add(meshOutline);
        }

        private void ApplyToChildren()
        {
            if (meshOutlines != null)
            {
                foreach (var meshOutline in meshOutlines)
                {
                    if (meshOutline != null)
                    {
                        meshOutline.CopyFrom(this);
                    }
                }
            }
        }
    }
}

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
    [DisallowMultipleComponent, AddComponentMenu("Scripts/GraphicsTools/MeshOutlineHierarchy")]
    public class MeshOutlineHierarchy : BaseMeshOutline
    {
        /// <summary>
        /// Defines how to exclude objects from the outline hierarchy.
        /// </summary>
        public enum ExclusionMode
        {
            None,
            Tag,
            NameStartsWith,
            NameContains,
        }

        [Tooltip("Whether and how to exclude objects from the outline hierarchy.")]
        [SerializeField, HideInInspector] private ExclusionMode exclusionMode = ExclusionMode.None;
        [SerializeField, HideInInspector] private string exclusionString = string.Empty;
        [SerializeField, HideInInspector] private string exclusionTag = "Untagged";

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
        /// Enables all child mesh outlines.
        /// </summary>
        private void OnEnable()
        {
            foreach (var meshOutline in meshOutlines)
            {
                if (meshOutline != null)
                {
                    meshOutline.enabled = true;
                }
            }
        }

        /// <summary>
        /// Disables all child mesh outlines.
        /// </summary>
        private void OnDisable()
        {
            foreach (var meshOutline in meshOutlines)
            {
                if (meshOutline != null)
                {
                    meshOutline.enabled = false;
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

        /// <summary>
        /// Forwards settings to all children <see cref="Microsoft.MixedReality.GraphicsTools.MeshOutline"/>s.
        /// </summary>
        public override void ApplyStencilReference()
        {
            ApplyToChildren();
        }

        #endregion BaseMeshOutline Implementation

        private void AddMeshOutline(Renderer target)
        {
            switch (exclusionMode)
            {
                case ExclusionMode.None:
                default:
                    break;
                case ExclusionMode.NameStartsWith:
                    if (target.name.StartsWith(exclusionString))
                    {
                        return;
                    }
                    break;
                case ExclusionMode.NameContains:
                    if (target.name.Contains(exclusionString))
                    {
                        return;
                    }
                    break;
                case ExclusionMode.Tag:
                    if (target.CompareTag(exclusionTag))
                    {
                        return;
                    }
                    break;
            }

            var meshOutline = target.gameObject.EnsureComponent<MeshOutline>();
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

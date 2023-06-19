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

        /// <summary>
        /// Whether and how to exclude objects from the outline hierarchy.
        /// </summary>
        public ExclusionMode Exclusion
        {
            get { return exclusionMode; }
            set
            {
                if (exclusionMode != value)
                {
                    exclusionMode = value;
                    Refresh();
                }
            }
        }

        [Tooltip("Whether and how to exclude objects from the outline hierarchy.")]
        [SerializeField, HideInInspector] 
        private ExclusionMode exclusionMode = ExclusionMode.None;

        /// <summary>
        /// When exclusionMode is set to "NameStartsWith" or "NameContains" what string to match against.
        /// </summary>
        public string ExclusionString
        {
            get { return exclusionString; }
            set
            {
                if (exclusionString != value)
                {
                    exclusionString = value;
                    Refresh();
                }
            }
        }

        [Tooltip("When exclusionMode is set to \"NameStartsWith\" or \"NameContains\" what string to match against.")]
        [SerializeField, HideInInspector] 
        private string exclusionString = string.Empty;

        /// <summary>
        /// When exclusionMode is set to "NameStartsWith" or "NameContains" what string to match against.
        /// </summary>
        public string ExclusionTag
        {
            get { return exclusionTag; }
            set
            {
                if (exclusionTag != value)
                {
                    exclusionTag = value;
                    Refresh();
                }
            }
        }

        [Tooltip("When exclusionMode is set to \"Tag\" what tag to compare against.")]
        [SerializeField, HideInInspector] 
        private string exclusionTag = "Untagged";

        private List<MeshOutline> meshOutlines = new List<MeshOutline>();

        #region MonoBehaviour Implementation

        /// <summary>
        /// Creates a <see cref="Microsoft.MixedReality.GraphicsTools.MeshOutline"/> component on each child MeshRenderer/SkinnedMeshRenderer.
        /// </summary>
        private void Awake()
        {
            Create();
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
            // Don't use DestroyImmediate on the same object in OnDisable or OnDestroy.
            Clear(false);
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

        /// <summary>
        /// Removes and re-adds all <see cref="Microsoft.MixedReality.GraphicsTools.MeshOutline"/> this component has created.
        /// </summary>
        public void Refresh()
        {
            Clear(true);
            Create();
        }

        /// <summary>
        /// Creates a <see cref="Microsoft.MixedReality.GraphicsTools.MeshOutline"/> component on each child MeshRenderer/SkinnedMeshRenderer.
        /// </summary>
        private void Create()
        {
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
        private void Clear(bool destroyImmediately)
        {
            for (int i = 0; i < meshOutlines.Count; ++i)
            {
                if (destroyImmediately)
                {
                    DestroyImmediate(meshOutlines[i]);
                }
                else
                {
                    Destroy(meshOutlines[i]);
                }
            }

            meshOutlines.Clear();
        }

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

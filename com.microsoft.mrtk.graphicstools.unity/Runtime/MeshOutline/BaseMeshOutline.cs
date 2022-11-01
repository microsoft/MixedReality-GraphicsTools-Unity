// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Abstract component to encapsulate common functionality around outline components. 
    /// </summary>
    public abstract class BaseMeshOutline : MonoBehaviour
    {
        /// <summary>
        /// The material used to render the outline. Outline materials should normal have "Depth Write" set to Off and "Vertex Extrusion" enabled.
        /// Most Graphics Tools/Standard features should work as an outline material, but it is recommended to keep the outline material as simple as possible.
        /// </summary>
        public Material OutlineMaterial
        {
            get { return outlineMaterial; }
            set
            {
                if (outlineMaterial != value)
                {
                    outlineMaterial = value;
                    ApplyOutlineMaterial();
                }
            }
        }

        [Tooltip("The material used to render the outline. Outline materials should normal have \"Depth Write\" set to Off and \"Vertex Extrusion\" enabled.")]
        [SerializeField]
        protected Material outlineMaterial = null;

        /// <summary>
        /// Should the stencil buffer be used to mask this outline rather than relying on depth? Required when a skybox is in use.
        /// </summary>
        public bool UseStencilOutline
        {
            get { return useStencilOutline; }
            set
            {
                if (useStencilOutline != value)
                {
                    useStencilOutline = value;
                }
            }
        }

        [Tooltip("Should the stencil buffer be used to mask this outline rather than relying on depth? Required when a skybox is in use.")]
        [SerializeField]
        protected bool useStencilOutline = false;

        /// <summary>
        /// The material used write a value to the stencil buffer. This material should have \"Depth Write\" set to Off and a \"ColorMask\" set to Zero.
        /// </summary>
        public Material StencilMaterial
        {
            get { return stencilMaterial; }
            set
            {
                if (stencilMaterial != value)
                {
                    stencilMaterial = value;
                    ApplyOutlineMaterial();
                }
            }
        }

        [Tooltip("The material used write a value to the stencil buffer. This material should have \"Depth Write\" set to Off and a \"ColorMask\" set to Zero.")]
        [SerializeField]
        protected Material stencilMaterial = null;

        /// <summary>
        /// How thick (in meters) should the outline be. Overrides the "Extrusion Value" in the Graphics Tools/Standard material.
        /// </summary>
        public float OutlineWidth
        {
            get { return outlineWidth; }
            set
            {
                if (outlineWidth != value)
                {
                    outlineWidth = value;
                    ApplyOutlineWidth();
                }
            }
        }

        [Tooltip("How thick (in meters) should the outline be. Overrides the \"Extrusion Value\" in the Graphics Tools/Standard material.")]
        [SerializeField]
        [Range(0.001f, 1.0f)]
        protected float outlineWidth = 0.01f;

        #region MonoBehaviour Implementation

        /// <summary>
        /// Event for when the animation system updates any serialized properties.
        /// </summary>
        protected virtual void OnDidApplyAnimationProperties()
        {
            ApplyOutlineWidth();
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Interface to apply the outline material to the renderer(s).
        /// </summary>
        public abstract void ApplyOutlineMaterial();

        /// <summary>
        /// Interface to to update the outline width with the renderer(s).
        /// </summary>
        public abstract void ApplyOutlineWidth();
    }
}

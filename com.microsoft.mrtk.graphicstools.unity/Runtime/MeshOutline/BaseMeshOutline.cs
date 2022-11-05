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
        /// The main color of the outline. Applied to the "_Color" shader property.
        /// </summary>
        public Color OutlineColor
        {
            get { return outlineColor; }
            set
            {
                if (outlineColor != value)
                {
                    outlineColor = value;
                    ApplyOutlineColor();
                }
            }
        }

        [Tooltip("The main color of the outline. Applied to the \"_Color\" shader property.")]
        [SerializeField]
        protected Color outlineColor = Color.green;

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
        public Material StencilWriteMaterial
        {
            get { return stencilWriteMaterial; }
            set
            {
                if (stencilWriteMaterial != value)
                {
                    stencilWriteMaterial = value;
                    ApplyOutlineMaterial();
                }
            }
        }

        [Tooltip("The material used write a value to the stencil buffer. This material should have \"Depth Write\" set to Off and a \"ColorMask\" set to Zero.")]
        [SerializeField]
        protected Material stencilWriteMaterial = null;

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
        /// Interface to to update the outline color with the renderer(s).
        /// </summary>
        public abstract void ApplyOutlineColor();

        /// <summary>
        /// Interface to to update the outline width with the renderer(s).
        /// </summary>
        public abstract void ApplyOutlineWidth();
    }
}

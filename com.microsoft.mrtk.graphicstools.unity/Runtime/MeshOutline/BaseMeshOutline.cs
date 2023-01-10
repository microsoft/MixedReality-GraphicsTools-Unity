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
        /// Note, this material is not automatically instanced.
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

        [Tooltip("The material used to render the outline. Outline materials should normal have \"Depth Write\" set to Off and \"Vertex Extrusion\" enabled. Note, this material is not automatically instanced.")]
        [SerializeField]
        protected Material outlineMaterial = null;

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
        [Range(0.0f, 1.0f)]
        protected float outlineWidth = 0.01f;

        /// <summary>
        /// Should the render queue be automatically calculated or based on the material properties?
        /// </summary>
        public bool AutoAssignRenderQueue
        {
            get { return autoAssignRenderQueue; }
            set
            {
                if (autoAssignRenderQueue != value)
                {
                    autoAssignRenderQueue = value;
                    ApplyOutlineMaterial();
                }
            }
        }

        [Tooltip("Should the render queue be automatically calculated or based on the material properties?")]
        [SerializeField]
        protected bool autoAssignRenderQueue = true;

        /// <summary>
        /// Should the stencil buffer be used to mask this outline rather than relying on depth? Required when a skybox is in use.
        /// Note, it is up to the user to manage the stencil reference value.
        /// </summary>
        public bool UseStencilOutline
        {
            get { return useStencilOutline; }
            set 
            { 
                if (useStencilOutline != value)
                {
                    useStencilOutline = value;
                    ApplyOutlineMaterial();
                }
            }
        }

        [Tooltip("Should the stencil buffer be used to mask this outline rather than relying on depth? Required when a skybox is in use. Note, it is up to the user to manage the stencil reference value.")]
        [SerializeField]
        protected bool useStencilOutline = false;

        /// <summary>
        /// The material used write a value to the stencil buffer. This material should have \"Depth Write\" set to Off and a \"ColorMask\" set to Zero.
        /// Note, this material is not automatically instanced.
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

        [Tooltip("The material used write a value to the stencil buffer. This material should have \"Depth Write\" set to Off and a \"ColorMask\" set to Zero. Note, this material is not automatically instanced.")]
        [SerializeField]
        protected Material stencilWriteMaterial = null;

        /// <summary>
        /// How far (in meters) should the outline be begin from the surface. Overrides the "Extrusion Value" in the Graphics Tools/Standard material.
        /// </summary>
        public float OutlineOffset
        {
            get { return outlineOffset; }
            set
            {
                if (outlineOffset != value)
                {
                    outlineOffset = value;
                    ApplyOutlineWidth();
                }
            }
        }

        [Tooltip("How far (in meters) should the outline be begin from the surface. Overrides the \"Extrusion Value\" in the Graphics Tools/Standard material.")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        protected float outlineOffset = 0.0f;

        /// <summary>
        /// "The value written to and read from the stencil buffer. This should be unique per outlined object."
        /// </summary>
        public int StencilReference
        {
            get { return stencilReference; }
            set
            {
                if (stencilReference != value)
                {
                    stencilReference = value;
                    ApplyStencilReference();
                }
            }
        }

        [Tooltip("The value written to and read from the stencil buffer. This should be unique per outlined object.")]
        [SerializeField]
        [Range(1, 255)]
        protected int stencilReference = 1;

        #region MonoBehaviour Implementation

        /// <summary>
        /// Event for when the animation system updates any serialized properties.
        /// </summary>
        protected virtual void OnDidApplyAnimationProperties()
        {
            ApplyOutlineWidth();
            ApplyStencilReference();
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

        /// <summary>
        /// Interface to to update the stencil ID with the renderer(s).
        /// </summary>
        public abstract void ApplyStencilReference();
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Component which can be used to render an outline around a mesh renderer. Enabling this component introduces an additional render pass(es) 
    /// of the object being outlined, but is designed to run performantly on mobile Mixed Reality devices and does not utilize any post processes.
    /// This behavior is designed to be used in conjunction with the Graphics Tools/Standard shader. Limitations of this effect include it not working well 
    /// on objects which are not watertight (or required to be two sided) and depth sorting issues may occur on overlapping objects.
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(Renderer)), AddComponentMenu("Scripts/GraphicsTools/MeshOutline")]
    public class MeshOutline : BaseMeshOutline
    {
        private const string vertexExtrusionKeyword = "_VERTEX_EXTRUSION";
        private const string vertexExtrusionSmoothNormalsKeyword = "_VERTEX_EXTRUSION_SMOOTH_NORMALS";

        private Renderer meshRenderer = null;
        private int stencilReferenceID = Shader.PropertyToID("_StencilReference");
        private int vertexExtrusionValueID = Shader.PropertyToID("_VertexExtrusionValue");
        private Material[] defaultMaterials = null;
        private MeshSmoother createdMeshSmoother = null;

        #region MonoBehaviour Implementation

        /// <summary>
        /// Gathers initial render state.
        /// </summary>
        private void Awake()
        {
            if (GetComponent<MeshRenderer>() == null && 
                GetComponent<SkinnedMeshRenderer>() == null)
            {
                Debug.LogWarning($"{this.GetType()} is not supported on this type of renderer.");
            }

            meshRenderer = GetComponent<Renderer>();
            defaultMaterials = meshRenderer.sharedMaterials;
        }

        /// <summary>
        /// Enables the outline.
        /// </summary>
        private void OnEnable()
        {
            ApplyOutlineMaterial();
        }

        /// <summary>
        /// Resets the renderer materials to the default settings.
        /// </summary>
        private void OnDisable()
        {
            meshRenderer.materials = defaultMaterials;
        }

        /// <summary>
        /// Removes any components this component has created.
        /// </summary>
        private void OnDestroy()
        {
            Destroy(createdMeshSmoother);
        }

        #endregion MonoBehaviour Implementation

        #region BaseMeshOutline Implementation

        /// <summary>
        /// Prepares and applies the current outline material to the MeshRenderer/SkinnedMeshRenderer.
        /// </summary>
        public override void ApplyOutlineMaterial()
        {
            if (enabled == false ||
                gameObject.activeInHierarchy == false ||
                meshRenderer == null ||
                outlineMaterial == null)
            {
                return;
            }

            Debug.AssertFormat(outlineMaterial.IsKeywordEnabled(vertexExtrusionKeyword),
           "The material \"{0}\" does not have vertex extrusion enabled, an outline might not be rendered.", outlineMaterial.name);

            if (UseStencilOutline)
            {
                if (stencilWriteMaterial == null)
                {
                    Debug.LogError("ApplyOutlineMaterial failed due to missing stencil write material.");
                    return;
                }

                // Ensure that the stencil material renders after the default materials and the outline material after the stencil material.
                if (AutoAssignRenderQueue)
                {
                    int maxRenderQueue = Mathf.Max(GetMaxRenderQueue(defaultMaterials), (int)RenderQueue.Overlay);
                    stencilWriteMaterial.renderQueue = maxRenderQueue + 1;
                    outlineMaterial.renderQueue = maxRenderQueue + 2;
                }
            }
            else
            {
                // Ensure that the outline material always renders before the default materials.
                if (AutoAssignRenderQueue)
                {
                    outlineMaterial.renderQueue = GetMinRenderQueue(defaultMaterials) - 1;
                }
            }

            // If smooth normals are requested, make sure the mesh has smooth normals.
            if (outlineMaterial.IsKeywordEnabled(vertexExtrusionSmoothNormalsKeyword))
            {
                var meshSmoother = (createdMeshSmoother == null) ? gameObject.GetComponent<MeshSmoother>() : createdMeshSmoother;

                if (meshSmoother == null)
                {
                    createdMeshSmoother = gameObject.AddComponent<MeshSmoother>();
                    meshSmoother = createdMeshSmoother;
                }

                meshSmoother.SmoothNormals();
            }

            ApplyOutlineWidth();
            ApplyStencilReference();

            // Add the outline material as another material pass.
            var materials = new Material[UseStencilOutline ? defaultMaterials.Length + 2 : defaultMaterials.Length + 1];
            defaultMaterials.CopyTo(materials, 0);
            int nextFreeIdx = defaultMaterials.Length;

            if (UseStencilOutline)
            {
                materials[nextFreeIdx++] = stencilWriteMaterial;
            }

            materials[nextFreeIdx] = outlineMaterial;

            meshRenderer.materials = materials;
        }

        /// <summary>
        /// Updates the current vertex extrusion value used by the materials.
        /// </summary>
        public override void ApplyOutlineWidth()
        {
            if (outlineMaterial != null)
            {
                outlineMaterial.SetFloat(vertexExtrusionValueID, outlineOffset + outlineWidth);
            }

            if (stencilWriteMaterial != null)
            {
                // Clamp to ensure we don't get z-fighting.
                stencilWriteMaterial.SetFloat(vertexExtrusionValueID, Mathf.Max(0.00001f, outlineOffset));
            }
        }

        /// <summary>
        /// Updates the current stencil ID used by the materials.
        /// </summary>
        public override void ApplyStencilReference()
        {
            if (outlineMaterial != null)
            {
                outlineMaterial.SetFloat(stencilReferenceID, stencilReference);
            }

            if (stencilWriteMaterial != null)
            {
                stencilWriteMaterial.SetFloat(stencilReferenceID, stencilReference);
            }
        }

        #endregion BaseMeshOutline Implementation

        /// <summary>
        /// Makes this BaseMeshOutline's settings match the other BaseMeshOutline.
        /// </summary>
        public void CopyFrom(BaseMeshOutline other)
        {
            outlineWidth = other.OutlineWidth;
            outlineMaterial = other.OutlineMaterial;
            autoAssignRenderQueue = other.AutoAssignRenderQueue;
            useStencilOutline = other.UseStencilOutline;
            stencilWriteMaterial = other.StencilWriteMaterial;
            outlineOffset = other.OutlineOffset;
            stencilReference = other.StencilReference;
            ApplyOutlineMaterial();
        }

        /// <summary>
        /// Searches for the minimum render queue value in a list of materials.
        /// </summary>
        private static int GetMinRenderQueue(Material[] materials)
        {
            var min = int.MaxValue;

            foreach (var material in materials)
            {
                if (material != null)
                {
                    min = Mathf.Min(min, material.renderQueue);
                }
            }

            if (min == int.MaxValue)
            {
                min = (int)RenderQueue.Background;
            }

            return min;
        }

        /// <summary>
        /// Searches for the maximum render queue value in a list of materials.
        /// </summary>
        private static int GetMaxRenderQueue(Material[] materials)
        {
            var max = int.MinValue;

            foreach (var material in materials)
            {
                if (material != null)
                {
                    max = Mathf.Max(max, material.renderQueue);
                }
            }

            if (max == int.MinValue)
            {
                max = (int)RenderQueue.Overlay;
            }

            return max;
        }
    }
}

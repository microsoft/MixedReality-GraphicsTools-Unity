// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// An abstract primitive component to animate and visualize a clipping primitive that can be 
    /// used to drive per pixel based clipping.
    /// </summary>
    [ExecuteAlways]
    public abstract class ClippingPrimitive : MonoBehaviour, IMaterialInstanceOwner
    {
        [Header("Renders to Clip")]
        [SerializeField, Tooltip("Toggles whether clipping will apply to shared materials or material instances (default) on renderers within the renderers list. This cannot be altered when renderers are already specified.")]
        private bool applyToSharedMaterial = false;

        /// <summary>
        /// Toggles whether clipping will apply to shared materials or material instances (default) on renderers within the renderers list. This cannot be altered when renderers are already specified.
        /// </summary>
        /// <remarks>
        /// Applying to shared materials will allow for GPU instancing to batch calls between Renderers that interact with the same clipping primitives.
        /// </remarks>
        public bool ApplyToSharedMaterial
        {
            get => applyToSharedMaterial;
            set
            {
                if (value != applyToSharedMaterial)
                {
                    if (renderers.Count > 0)
                    {
                        throw new InvalidOperationException("Cannot change material applied to after renderers have been added.");
                    }
                    applyToSharedMaterial = value;
                }
            }
        }

        [Tooltip("The renderer(s) that should be affected by the primitive. Renderers with materials in the materials list do not need to be added to this list.")]
        [SerializeField]
        protected List<Renderer> renderers = new List<Renderer>();

        [Header("Materials to Clip")]
        [Tooltip("The materials(s) that should be affected by the primitive. Materials on renderers within the renderers list do not need to be added to this list.")]
        [SerializeField]
        protected List<Material> materials = new List<Material>();

        /// <summary>
        /// Should clipping occur inside or outside of the clipping shape?
        /// </summary>
        public enum Side
        {
            Inside = 1,
            Outside = -1
        }

        [Header("Clip Settings")]
        [Tooltip("Which side of the primitive to clip pixels against.")]
        [SerializeField]
        protected Side clippingSide = Side.Inside;

        /// <summary>
        /// Which side of the primitive to clip pixels against.
        /// </summary>
        public Side ClippingSide
        {
            get => clippingSide;
            set => clippingSide = value;
        }

        [SerializeField]
        [Tooltip("Toggles whether the primitive will use the Camera OnPreRender event")]
        private bool useOnPreRender;

        /// <summary>
        /// Toggles whether the primitive will use the Camera OnPreRender event.
        /// </summary>
        /// <remarks>
        /// This is especially helpful if you're trying to clip dynamically created objects that may be added to the scene after LateUpdate such as OnWillRender 
        /// </remarks>
        public bool UseOnPreRender
        {
            get => useOnPreRender;
            set
            {
                if (cameraMethods == null)
                {
                    cameraMethods = Camera.main.gameObject.EnsureComponent<CameraEventRouter>();
                }

                if (useOnPreRender != value)
                {
                    if (value)
                    {
                        cameraMethods.OnCameraPreRender += OnCameraPreRender;
                    }
                    else if (!value)
                    {
                        cameraMethods.OnCameraPreRender -= OnCameraPreRender;
                    }

                    useOnPreRender = value;
                }
            }
        }

        private int clippingSideID;
        private CameraEventRouter cameraMethods;
        private bool isDirty;

        /// <summary>
        /// Keeping track of any field, property or transformation changes to optimize material property block setting.
        /// </summary>
        public bool IsDirty
        {
            get => isDirty;
            set => isDirty = value;
        }

        /// <summary>
        /// The property block in use for material adjustments. 
        /// </summary>
        protected MaterialPropertyBlock materialPropertyBlock;

        /// <summary>
        /// Returns the shader keyword name used to toggle this clipping feature on/off.
        /// </summary>
        protected abstract string Keyword { get; }

        /// <summary>
        /// Returns the shader property name used to control the clipping side.
        /// </summary>
        protected abstract string ClippingSideProperty { get; }

        /// <summary>
        /// Adds a renderer to the list of objects this clipping primitive clips.
        /// </summary>
        public void AddRenderer(Renderer _renderer)
        {
            if (_renderer != null)
            {
                if (!renderers.Contains(_renderer))
                {
                    renderers.Add(_renderer);
                }

                ToggleClippingFeature(AcquireMaterials(_renderer), gameObject.activeInHierarchy);
                IsDirty = true;
            }
        }

        /// <summary>
        /// Removes a renderer from the list of objects this clipping primitive clips.
        /// </summary>
        public void RemoveRenderer(Renderer _renderer)
        {
            int index = renderers.IndexOf(_renderer);
            if (index >= 0)
            {
                RemoveRenderer(index);
            }
        }

        /// <summary>
        /// Resets the state of the renderer to before being added to the primitive.
        /// </summary>
        public void ResetRenderer(Renderer _renderer, bool autoDestroyMaterial = true)
        {
            if (_renderer != null)
            {
                // There is no need to acquire new instances if ones do not already exist since we are 
                // in the process of removing.
                ToggleClippingFeature(AcquireMaterials(_renderer, instance: false), false);

                var materialInstance = _renderer.GetComponent<MaterialInstance>();

                if (materialInstance != null)
                {
                    materialInstance.ReleaseMaterial(this, autoDestroyMaterial);
                }

                // Reset the material property block.
                materialPropertyBlock.Clear();
                _renderer.SetPropertyBlock(materialPropertyBlock);
            }
        }

        private void RemoveRenderer(int index, bool autoDestroyMaterial = true)
        {
            Renderer _renderer = renderers[index];

            int lastIndex = renderers.Count - 1;
            if (index != lastIndex)
            {
                renderers[index] = renderers[lastIndex];
            }

            renderers.RemoveAt(lastIndex);

            ResetRenderer(_renderer, autoDestroyMaterial);
        }

        /// <summary>
        /// Removes all renderers in the list of objects this clipping primitive clips.
        /// </summary>
        public void ClearRenderers(bool autoDestroyMaterial = true)
        {
            if (renderers != null)
            {
                while (renderers.Count != 0)
                {
                    RemoveRenderer(renderers.Count - 1, autoDestroyMaterial);
                }
            }
        }

        /// <summary>
        /// Adds a material to the list of objects this clipping primitive clips.
        /// </summary>
        public void AddMaterial(Material material)
        {
            if (material != null)
            {
                if (!materials.Contains(material))
                {
                    materials.Add(material);
                }

                ToggleClippingFeature(material, gameObject.activeInHierarchy);
                IsDirty = true;
            }
        }

        /// <summary>
        /// Removes a material from the list of objects this clipping primitive clips.
        /// </summary>
        public void RemoveMaterial(Material material)
        {
            int index = materials.IndexOf(material);
            if (index >= 0)
            {
                RemoveMaterial(index);
            }
        }

        /// <summary>
        /// Resets the state of the material to before being added to the primitive.
        /// </summary>
        public void ResetMaterial(Material material)
        {
            if (material != null)
            {
                ToggleClippingFeature(material, false);
            }
        }

        private void RemoveMaterial(int index)
        {
            Material material = materials[index];

            int lastIndex = materials.Count - 1;
            if (index != lastIndex)
            {
                materials[index] = materials[lastIndex];
            }

            materials.RemoveAt(lastIndex);

            ResetMaterial(material);
        }

        /// <summary>
        /// Removes all materials in the list of objects this clipping primitive clips.
        /// </summary>
        public void ClearMaterials()
        {
            if (materials != null)
            {
                while (materials.Count != 0)
                {
                    RemoveMaterial(materials.Count - 1);
                }
            }
        }

        /// <summary>
        /// Returns a copy of the current list of renderers.
        /// </summary>
        public IEnumerable<Renderer> GetRenderersCopy()
        {
            return new List<Renderer>(renderers);
        }

        /// <summary>
        /// Returns a copy of the current list of materials.
        /// </summary>
        public IEnumerable<Material> GetMaterialsCopy()
        {
            return new List<Material>(materials);
        }

        #region MonoBehaviour Implementation

        /// <summary>
        /// Turns clipping on.
        /// </summary>
        protected void OnEnable()
        {
            Initialize();
            UpdateRenderState();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.update += EditorUpdate;
            }
#endif

            ToggleClippingFeature(true);

            if (useOnPreRender)
            {
                cameraMethods = Camera.main.gameObject.EnsureComponent<CameraEventRouter>();
                cameraMethods.OnCameraPreRender += OnCameraPreRender;
            }
        }

        /// <summary>
        /// Turns clipping off.
        /// </summary>
        protected void OnDisable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= EditorUpdate;
#endif

            UpdateRenderState();
            ToggleClippingFeature(false);

            if (cameraMethods != null)
            {
                UseOnPreRender = false;
            }
        }

#if UNITY_EDITOR
        // We need this class to be updated once per frame even when in edit mode. Ideally this would 
        // occur after all other objects are updated in LateUpdate(), but because the ExecuteInEditMode 
        // attribute only invokes Update() we handle edit mode updating here and runtime updating 
        // in LateUpdate().
        protected void EditorUpdate()
        {
            Initialize();
            UpdateRenderState();
        }
#endif

        /// <summary>
        /// Updates all renderers with latest clipping state.
        /// </summary>
        protected void LateUpdate()
        {
            // Deferring the LateUpdate() call to OnCameraPreRender()
            if (!useOnPreRender)
            {
                UpdateRenderState();
            }
        }

        /// <summary>
        /// Updates all renderers with latest clipping state.
        /// </summary>
        protected void OnCameraPreRender(CameraEventRouter router)
        {
            // Only subscribed to via UseOnPreRender property setter
            UpdateRenderState();
        }

        /// <summary>
        /// Removes all renderers from the clipping primitive. 
        /// </summary>
        protected void OnDestroy()
        {
            ClearRenderers();
            ClearMaterials();
        }

        #endregion MonoBehaviour Implementation

        #region IMaterialInstanceOwner Implementation

        /// <inheritdoc />
        public void OnMaterialChanged(MaterialInstance materialInstance)
        {
            if (materialInstance != null)
            {
                ToggleClippingFeature(materialInstance.AcquireMaterials(this), gameObject.activeInHierarchy);
            }

            UpdateRenderState();
        }

        #endregion IMaterialInstanceOwner Implementation

        /// <summary>
        /// Caches commonly used state.
        /// </summary>
        protected virtual void Initialize()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            clippingSideID = Shader.PropertyToID(ClippingSideProperty);
        }

        /// <summary>
        /// Updates the render state of all renderers and materials.
        /// </summary>
        protected virtual void UpdateRenderState()
        {
            CheckTransformChange();

            if (!IsDirty) 
            { 
                return; 
            }

            BeginUpdateShaderProperties();

            if (renderers != null)
            {
                for (int i = renderers.Count - 1; i >= 0; --i)
                {
                    var _renderer = renderers[i];
                    if (_renderer == null)
                    {
                        if (!Application.isEditor)
                        {
                            RemoveRenderer(i);
                        }
                        continue;
                    }

                    _renderer.GetPropertyBlock(materialPropertyBlock);
                    materialPropertyBlock.SetFloat(clippingSideID, (float)clippingSide);
                    UpdateShaderProperties(materialPropertyBlock);
                    _renderer.SetPropertyBlock(materialPropertyBlock);
                }
            }

            if (materials != null)
            {
                for (int i = materials.Count - 1; i >= 0; --i)
                {
                    var material = materials[i];
                    if (material == null)
                    {
                        if (!Application.isEditor)
                        {
                            RemoveMaterial(i);
                        }
                        continue;
                    }

                    material.SetFloat(clippingSideID, (float)clippingSide);
                    UpdateShaderProperties(material);
                }
            }

            EndUpdateShaderProperties();
            IsDirty = false;
        }

        /// <summary>
        /// Method called when before each renderer is updated.
        /// </summary>
        protected virtual void BeginUpdateShaderProperties() { }

        /// <summary>
        /// Method called for each renderer.
        /// </summary>
        protected abstract void UpdateShaderProperties(MaterialPropertyBlock materialPropertyBlock);

        /// <summary>
        /// Method called for each material.
        /// </summary>
        protected abstract void UpdateShaderProperties(Material material);

        /// <summary>
        /// Method called when after each renderer is updated.
        /// </summary>
        protected virtual void EndUpdateShaderProperties() { }

        /// <summary>
        /// Enables or disables clipping for all renderers.
        /// </summary>
        protected void ToggleClippingFeature(bool keywordOn)
        {
            if (renderers != null)
            {
                for (var i = 0; i < renderers.Count; ++i)
                {
                    var _renderer = renderers[i];

                    if (_renderer != null)
                    {
                        ToggleClippingFeature(AcquireMaterials(_renderer), keywordOn);
                    }
                }
            }

            if (materials != null)
            {
                for (var i = 0; i < materials.Count; ++i)
                {
                    var material = materials[i];

                    if (material != null)
                    {
                        ToggleClippingFeature(material, keywordOn);
                    }
                }
            }
        }

        /// <summary>
        /// Enables or disables clipping for a list of materials.
        /// </summary>
        protected void ToggleClippingFeature(Material[] materialsToToggle, bool keywordOn)
        {
            if (materialsToToggle != null)
            {
                foreach (var material in materialsToToggle)
                {
                    ToggleClippingFeature(material, keywordOn);
                }
            }
        }

        /// <summary>
        /// Enables or disables clipping on a material.
        /// </summary>
        protected void ToggleClippingFeature(Material materialToToggle, bool keywordOn)
        {
            if (materialToToggle != null)
            {
                if (keywordOn)
                {
                    materialToToggle.EnableKeyword(Keyword);
                }
                else
                {
                    materialToToggle.DisableKeyword(Keyword);
                }
            }
        }

        private Material[] AcquireMaterials(Renderer renderer, bool instance = true)
        {
            if (applyToSharedMaterial)
            {
                return renderer.sharedMaterials;
            }
            else
            {
                return renderer.EnsureComponent<MaterialInstance>().AcquireMaterials(this, instance);
            }
        }

        private void CheckTransformChange()
        {
            if (transform.hasChanged)
            {
                IsDirty = true;
                transform.hasChanged = false;
            }
        }
    }
}

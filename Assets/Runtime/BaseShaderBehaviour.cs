// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// TODO
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasRenderer))]
    public abstract class BaseShaderBehaviour : MonoBehaviour
    {
        /// <summary>
        /// "When animated should a new material be instantiated?"
        /// </summary>
        public bool UseSharedMaterial
        {
            get { return useSharedMaterial; }
            set { useSharedMaterial = value; }
        }

        [Tooltip("When animated should a new material be instantiated?")]
        [SerializeField]
        private bool useSharedMaterial = false;

        private CanvasRenderer canvasRenderer = null;
        private Material materialInstance = null;

        #region MonoBehaviour Implementation

        /// <summary>
        /// TODO
        /// </summary>
        private void Awake()
        {
            canvasRenderer = GetComponent<CanvasRenderer>();

            Material material = canvasRenderer.GetMaterial();

            if (material != null)
            {
                InitializeFromMaterial(material);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void OnDidApplyAnimationProperties()
        {
            Material material = canvasRenderer.GetMaterial();

            // Create a material instance when first animated.
            if (!useSharedMaterial && materialInstance == null)
            {
                materialInstance = Instantiate(material);
                canvasRenderer.SetMaterial(materialInstance, 0);
                material = materialInstance;
            }

            if (material != null)
            {
                ApplyToMaterial(material);
            }
        }

        #endregion MonoBehaviour Implementation

        #region BaseShaderBehaviour Implementation
        /// <summary>
        /// TODO
        /// </summary>
        public abstract void InitializeFromMaterial(Material material);

        /// <summary>
        /// TODO
        /// </summary>
        public abstract void ApplyToMaterial(Material material);

        #endregion BaseShaderBehaviour Implementation
    }
}

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
        private CanvasRenderer canvasRenderer = null;

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

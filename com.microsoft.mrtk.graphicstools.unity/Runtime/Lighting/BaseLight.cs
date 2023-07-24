// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

#if GT_USE_XRI
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// An abstract light class used for all light types within Graphics Tools.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class BaseLight : MonoBehaviour
    {
        /// <summary>
        /// Method called before updating any lights.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Called when a light is added to a scene.
        /// </summary>
        protected abstract void AddLight();

        /// <summary>
        /// Called when a light is removed from a scene.
        /// </summary>
        protected abstract void RemoveLight();

        /// <summary>
        /// Called when light data needs to be updated in the shader system.
        /// </summary>
        protected abstract void UpdateLights(bool forceUpdate = false);

        #region MonoBehaviour Implementation

        /// <summary>
        /// This function is called when the light becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            AddLight();
            Application.onBeforeRender += UpdateLightsCallback;
        }

        /// <summary>
        /// This function is called when the light becomes disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
            Application.onBeforeRender -= UpdateLightsCallback;
            RemoveLight();
            UpdateLights(true);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Only called in the editor since LateUpdate is not called with ExecuteInEditMode.
        /// </summary>
        protected virtual void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            Initialize();
            UpdateLights();
        }
#endif // UNITY_EDITOR

        // Retained for breaking change purposes
        protected virtual void LateUpdate() { }

        /// <summary>
        /// Lights are updated in Application.onBeforeRender to ensure other behaviors have had a chance to move the lights.
        /// </summary>
#if GT_USE_XRI
        [BeforeRenderOrder(XRInteractionUpdateOrder.k_BeforeRenderLineVisual)]
#endif
        protected virtual void UpdateLightsCallback()
        {
            UpdateLights();
        }

        #endregion MonoBehaviour Implementation
    }
}

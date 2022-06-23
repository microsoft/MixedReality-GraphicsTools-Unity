// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// An abstract light class used for all light types within Graphics Tools.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class BaseLight : MonoBehaviour
    {
        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void AddLight();

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void RemoveLight();

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void UpdateLights(bool forceUpdate = false);

        #region MonoBehaviour Implementation

        /// <summary>
        /// TODO
        /// </summary>
        protected virtual void OnEnable()
        {
            AddLight();
        }

        /// <summary>
        /// TODO
        /// </summary>
        protected virtual void OnDisable()
        {
            RemoveLight();
            UpdateLights(true);
        }

#if UNITY_EDITOR
        /// <summary>
        /// TODO
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

        /// <summary>
        /// TODO
        /// </summary>
        protected virtual void LateUpdate()
        {
            UpdateLights();
        }

        #endregion MonoBehaviour Implementation
    }
}

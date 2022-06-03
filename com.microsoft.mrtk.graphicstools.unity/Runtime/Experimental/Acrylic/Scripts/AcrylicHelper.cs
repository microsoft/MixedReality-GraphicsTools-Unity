// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Helper component that automatically enables/disables the specified acrylic layer when an object is enabled/disabled
    /// (notifying the acrylic layer manager and updating the object's material).  Attach to any object that uses an acrylic material.
    /// EnableLayer() & DisableLayer() methods can be used to explicitly enable or disable the designated layer.
    /// </summary>

    public class AcrylicHelper : MonoBehaviour
    {
        [Experimental]
        [SerializeField]
        [Range(0, 1)]
        private int blurLayer = 0;

        private bool useAcrylic = false;
        private Graphic cachedGraphic = null;
        private Coroutine initCoroutine = null;

#region Monobehavior methods

        private void OnEnable()
        {
            initCoroutine = StartCoroutine(WaitForAcrylicLayerManager());
        }

        private void OnDisable()
        {
            if (initCoroutine != null)
            {
                StopCoroutine(initCoroutine);
                initCoroutine = null;
            }
            else
            {
                DisableLayer();
            }
        }

#endregion

#region public methods

        /// <summary>
        /// Adds a reference to the current blur layer.
        /// </summary>
        public void EnableLayer()
        {
            if (AcrylicLayerManager.Instance != null)
            {
                AcrylicLayerManager.Instance.EnableLayer(blurLayer);
            }
        }

        /// <summary>
        /// Removes a reference from the current blur layer.
        /// </summary>
        public void DisableLayer()
        {
            if (AcrylicLayerManager.Instance != null)
            {
                AcrylicLayerManager.Instance.DisableLayer(blurLayer);
            }
        }

#endregion

#region private methods

        private void UpdateMaterialState()
        {
            if (cachedGraphic == null)
            {
                cachedGraphic = GetComponent<Graphic>();
            }

            if (cachedGraphic != null)
            {
                useAcrylic = AcrylicLayerManager.Instance != null && AcrylicLayerManager.Instance.AcrylicActive;
                SetMaterialState(cachedGraphic.material, "_BLUR_TEXTURE_ENABLE_", useAcrylic && blurLayer == 0);
                SetMaterialState(cachedGraphic.material, "_BLUR_TEXTURE_2_ENABLE_", useAcrylic && blurLayer == 1);
                cachedGraphic.SetMaterialDirty();
            }
        }

        private void SetMaterialState(Material m, string keyword, bool enable)
        {
            if (enable)
            {
                m.EnableKeyword(keyword);
            }
            else
            {
                m.DisableKeyword(keyword);
            }
        }

        private IEnumerator WaitForAcrylicLayerManager()
        {
            // wait for the AcylicLayerManager to exist
            while (AcrylicLayerManager.Instance == null)
            {
                yield return null;
            }

            // then update material/layer
            UpdateMaterialState();
            EnableLayer();

            initCoroutine = null;
        }
#endregion
    }
}
#endif // GT_USE_URP

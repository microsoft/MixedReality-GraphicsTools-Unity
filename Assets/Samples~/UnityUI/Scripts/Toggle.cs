// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    public class Toggle : MonoBehaviour
    {
        [Tooltip("The GameObject to activate and deactivate.")]
        public GameObject target = null;

        /// <summary>
        /// If a GameObject is active this method deactivates it and vice versa.
        /// </summary>
        public void ToggleOnOff()
        {
            if (target != null)
            {
                target.SetActive(!target.activeSelf);
            }
        }
    }
}

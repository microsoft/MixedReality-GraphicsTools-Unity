// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.GraphicsTools.Samples.MeshInstancing
{
    /// <summary>
    /// Logic to toggle on/off samples based on UI toggle.
    /// </summary>
    public class SamplesToggle : MonoBehaviour
    {
        /// <summary>
        /// UnityUI toggle/sample pair.
        /// </summary>
        [Serializable]
        public struct ToggleSample
        {
            public Toggle Toggle;
            public GameObject Sample;
        }

        /// <summary>
        /// The list of samples to toggle with their UI toggle.
        /// </summary>
        public ToggleSample[] toggleSamples = null;

        /// <summary>
        /// Subscribe to toggle events.
        /// </summary>
        private void OnEnable()
        {
            foreach (var pair in toggleSamples)
            {
                if (pair.Toggle != null)
                {
                    pair.Toggle.onValueChanged.AddListener((bool on) =>
                    {
                        if (pair.Sample != null)
                        {
                            pair.Sample.SetActive(on);
                        }
                    });
                }
            }
        }
    }
}

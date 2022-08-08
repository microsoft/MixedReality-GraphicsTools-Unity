// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Samples.MeshInstancing
{
    /// <summary>
    /// Simple instance placement and colorization example.
    /// </summary>
    public class InstancingRandom : MonoBehaviour
    {
        [SerializeField]
        private MeshInstancer instancer = null;
        [SerializeField, Min(1)]
        private int instanceCount = 20000;
        [SerializeField]
        private float instanceScale = 0.1f;

        /// <summary>
        /// Re-spawn instances when a property changes.
        /// </summary>
        private void OnValidate()
        {
            if (instancer != null && instancer.InstanceCount != 0)
            {
                CreateInstances();
            }
        }

        /// <summary>
        /// Create instances on enable.
        /// </summary>
        private void OnEnable()
        {
            CreateInstances();
        }

        /// <summary>
        /// Create a bunch of random instances within a unity sphere.
        /// </summary>
        private void CreateInstances()
        {
            // Clear any existing instances.
            instancer = (instancer == null) ? GetComponent<MeshInstancer>() : instancer;
            instancer.Clear();

            int colorID = Shader.PropertyToID("_Color");

            for (int i = 0; i < instanceCount; ++i)
            {
                var instance = instancer.Instantiate(Random.onUnitSphere, Random.rotation, Random.insideUnitSphere * instanceScale);
                instance.SetVector(colorID, Random.ColorHSV());
            }
        }
    }
}

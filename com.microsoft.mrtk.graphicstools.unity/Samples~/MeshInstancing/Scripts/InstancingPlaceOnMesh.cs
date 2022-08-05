// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Samples.MeshInstancing
{
    /// <summary>
    /// TODO
    /// </summary>
    public class InstancingPlaceOnMesh : MonoBehaviour
    {
        [SerializeField]
        private MeshInstancer instancer = null;
        //[SerializeField]
        //private Mesh PlacementMesh = null;
        //[SerializeField, Min(0)]
        //private int PlacementMeshSubMeshIndex = 0;

        [Header("Instance Properties")]
        [SerializeField, Min(1)]
        private int instanceCount = 20000;

        private bool didStart = false;

        /// <summary>
        /// Re-spawn instances when a property changes.
        /// </summary>
        private void OnValidate()
        {
            if (didStart)
            {
                CreateInstances();
            }
        }

        /// <summary>
        /// Create instances on start.
        /// </summary>
        private void Start()
        {
            CreateInstances();
            didStart = true;
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void CreateInstances()
        {
            // Clear any existing instances.
            instancer = (instancer == null) ? GetComponent<MeshInstancer>() : instancer;
            instancer.Clear();

            int colorID = Shader.PropertyToID("_Color");

            for (int i = 0; i < instanceCount; ++i)
            {
                var instance = instancer.Instantiate(Random.onUnitSphere, Random.rotation, Random.insideUnitSphere);
                instance.SetVector(colorID, Random.ColorHSV());
            }
        }
    }
}

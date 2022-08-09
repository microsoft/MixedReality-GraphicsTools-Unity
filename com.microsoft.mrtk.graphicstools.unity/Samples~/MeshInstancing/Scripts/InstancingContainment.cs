// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Samples.MeshInstancing
{
    /// <summary>
    /// Simulates a bunch of swimming fish within a containment radius.
    /// </summary>
    public class InstancingContainment : MonoBehaviour
    {
        [SerializeField]
        private MeshInstancer instancer = null;

        [Header("Simulation Properties")]
        [SerializeField, Min(0)]
        private float containmentRadius = 3.0f;

        [Header("Instance Properties")]
        [SerializeField, Min(1)]
        private int instanceCount = 20000;
        [SerializeField, Min(0.01f)]
        private float instanceSizeMin = 0.02f;
        [SerializeField, Min(0.01f)]
        private float instanceSizeMax = 0.08f;

        private class UserData
        {
            public Vector3 velocity;
            public Quaternion targetRotation;
            public bool reflecting;
        }

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
        /// Render the bounds of the instances.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(Vector3.zero, containmentRadius);
        }

        /// <summary>
        /// Create a bunch of swimming fish.
        /// </summary>
        private void CreateInstances()
        {
            // Clear any existing instances.
            instancer = (instancer == null) ? GetComponent<MeshInstancer>() : instancer;
            instancer.Clear();

            float minMass = instanceSizeMin * instanceSizeMin * instanceSizeMin;
            float maxMass = instanceSizeMax * instanceSizeMax * instanceSizeMax;
            int colorID = Shader.PropertyToID("_Color");
            int swimSpeed = Shader.PropertyToID("_SwimSpeed");

            for (int i = 0; i < instanceCount; ++i)
            {
                // Create some random initial properties.
                Vector3 scale = Vector3.one * Random.Range(instanceSizeMin, instanceSizeMax);
                float normalizedMass = ((scale.x * scale.y * scale.z) - minMass) / maxMass;
                Vector3 velocity = Random.onUnitSphere * ((1.0f - normalizedMass) * 0.2f);
                Quaternion rotation = Quaternion.LookRotation(velocity);

                // Create an instance object at a random position within the containment radius.
                var instance = instancer.Instantiate(Random.insideUnitSphere * containmentRadius,
                                                     rotation,
                                                     scale);
                instance.SetVector(colorID, Random.ColorHSV(normalizedMass, normalizedMass, 1.0f, 1.0f, 1.0f, 1.0f));
                instance.SetFloat(swimSpeed, Mathf.Lerp(400.0f, 20.0f, normalizedMass));

                // Set user data to use during update.
                instance.UserData = new UserData()
                {
                    velocity = velocity,
                    targetRotation = rotation,
                    reflecting = false
                };

                instance.SetParallelUpdate(ParallelUpdate);
            }
        }

        /// <summary>
        /// Method called potentially concurrently across many threads. Make sure all function calls are thread safe.
        /// </summary>
        private void ParallelUpdate(float deltaTime, MeshInstancer.Instance instance)
        {
            // Cast the user data to our data type.
            UserData data = (UserData)instance.UserData;

            // Euler integration.
            instance.LocalPosition += data.velocity * deltaTime;
            instance.LocalRotation = Quaternion.Slerp(instance.LocalRotation, data.targetRotation, deltaTime * data.velocity.sqrMagnitude * 40.0f);

            // Check if outside the containment radius.
            if (instance.LocalPosition.sqrMagnitude > (containmentRadius * containmentRadius))
            {
                if (!data.reflecting)
                {
                    // If outside the radius and not already reflecting then reflect off the containment sphere.
                    Vector3 sphereNormal = (Vector3.zero - instance.LocalPosition).normalized;
                    data.velocity = Vector3.Reflect(data.velocity, sphereNormal);
                    data.targetRotation = Quaternion.LookRotation(data.velocity);

                    data.reflecting = true;
                }
            }
            else
            {
                data.reflecting = false;
            }

            // Update the user data state.
            instance.UserData = data;
        }

    }
}

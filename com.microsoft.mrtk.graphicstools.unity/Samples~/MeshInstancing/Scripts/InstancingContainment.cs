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
            public Vector3 targetVelocity;
            public Quaternion targetRotation;
            public float changeTargetTime;
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
                Vector3 velocity = Random.onUnitSphere * ((1.2f - normalizedMass) * 0.2f);
                Quaternion rotation = Quaternion.LookRotation(velocity);

                // Create an instance object at a random position within the containment radius.
                var instance = instancer.Instantiate(Random.onUnitSphere * containmentRadius,
                                                     rotation,
                                                     scale);
                instance.SetVector(colorID, Random.ColorHSV(normalizedMass, normalizedMass, 1.0f, 1.0f, 1.0f, 1.0f));
                instance.SetFloat(swimSpeed, Mathf.Lerp(400.0f, 20.0f, normalizedMass));

                // Set user data to use during update.
                instance.UserData = new UserData()
                {
                    velocity = Vector3.zero,
                    targetVelocity = velocity,
                    targetRotation = rotation,
                    changeTargetTime = Random.Range(4.0f, 20.0f)
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
            data.velocity = Vector3.Lerp(data.velocity, data.targetVelocity, deltaTime);
            instance.LocalPosition += data.velocity * deltaTime;
            instance.LocalRotation = Quaternion.Slerp(instance.LocalRotation, data.targetRotation, deltaTime * data.targetVelocity.sqrMagnitude * 40.0f);

            data.changeTargetTime -= deltaTime;

            // Time to pick a new target.
            if (data.changeTargetTime <= 0.0f)
            {
                Vector3 randomTarget = ThreadSafeRandom.onUnitSphere * containmentRadius;
                Vector3 direction = (randomTarget - instance.LocalPosition).normalized;
                data.targetVelocity = direction * data.targetVelocity.magnitude;
                data.targetRotation = Quaternion.LookRotation(data.targetVelocity);

                data.changeTargetTime = ThreadSafeRandom.Range(4.0f, 20.0f);
            }

            // Update the user data state.
            instance.UserData = data;
        }

    }
}

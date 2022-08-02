// Copyri// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Microsoft.MixedReality.GraphicsTools.Samples.MeshInstancing
{
    /// <summary>
    /// A simple driver that demonstrates how to use the MeshInstancer component to generate instances with per instance 
    /// material properties and how to update instances in a concurrent way.
    /// </summary>
    public class InstancingStressTest : MonoBehaviour
    {
        [SerializeField]
        private MeshInstancer instancer = null;
        [SerializeField, Range(0, 100000)]
        private int instanceCount = 500;
        [SerializeField]
        private bool uniformScale = true;
        [SerializeField, Range(0.01f, 10.0f)]
        private float instanceSizeMin = 0.02f;
        [SerializeField, Range(0.01f, 10.0f)]
        private float instanceSizeMax = 0.08f;
        [SerializeField, Range(0.1f, 100.0f)]
        private float instanceRadius = 2.0f;
        [SerializeField, Range(0.0f, 1.0f)]
        private float minHue = 0.5f;
        [SerializeField, Range(0.0f, 1.0f)]
        private float maxHue = 0.8f;
        [SerializeField, Range(0.0f, 1.0f)]
        private float minSaturation = 0.2f;
        [SerializeField, Range(0.0f, 1.0f)]
        private float maxSaturation = 0.7f;
        [SerializeField, Range(0.0f, 1.0f)]
        private float minValue = 1.0f;
        [SerializeField, Range(0.0f, 1.0f)]
        private float maxValue = 1.0f;
        [SerializeField]
        private bool animate = true;
        [SerializeField, Range(0.0f, 100.0f)]
        private float minMovepeed = 1.0f;
        [SerializeField, Range(0.0f, 100.0f)]
        private float maxMovepeed = 5.0f;
        [SerializeField, Range(0, 100)]
        private int minTurnUpdateTime = 1;
        [SerializeField, Range(0, 100)]
        private int maxTurnUpdateTime = 5;

        private int colorID;
        private MeshInstancer.RaycastHit lastRaycastHit;

        private class SimulationData
        {
            public float speed;
            public float turnTime;
            public Quaternion targetRotation;
        }

        private void Start()
        {
            colorID = Shader.PropertyToID("_Color");

            // Create a bunch of random instances with asynchronous update methods.
            for (int i = 0; i < instanceCount; ++i)
            {
                Vector3 scale = uniformScale ? Vector3.one * Random.Range(instanceSizeMin, instanceSizeMax) : new Vector3(Random.Range(instanceSizeMin, instanceSizeMax), 
                                                                                                                          Random.Range(instanceSizeMin, instanceSizeMax), 
                                                                                                                          Random.Range(instanceSizeMin, instanceSizeMax));

                MeshInstancer.Instance instance = instancer.Instantiate(Random.insideUnitSphere * instanceRadius, Quaternion.LookRotation(Random.onUnitSphere, Vector3.up), scale);
                instance.SetVector(colorID, Random.ColorHSV(minHue, maxHue, minSaturation, maxSaturation, minValue, maxValue));
                instance.UserData = new SimulationData() { speed = Random.Range(minMovepeed, maxMovepeed), turnTime = Random.Range(minTurnUpdateTime, maxTurnUpdateTime), targetRotation = instance.LocalRotation };

                if (animate)
                {
                    instance.SetParallelUpdate(AsyncUpdate);
                }
            }
        }

        private void Update()
        {
            if (instancer.RaycastInstances)
            {
                // Default to the camera look position.
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

#if ENABLE_INPUT_SYSTEM
                if (Mouse.current != null)
                {
                    Vector2 mousePosition2D = Mouse.current.position.ReadValue();
                    Vector3 mousePosition = new Vector3(mousePosition2D.x, mousePosition2D.y, Camera.main.nearClipPlane);
                    ray.origin = Camera.main.ScreenToWorldPoint(mousePosition);
                    ray.direction = (Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.farClipPlane)) - ray.origin).normalized;
                }
#else
                if (Input.mousePresent)
                {
                    Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
                    ray.origin = Camera.main.ScreenToWorldPoint(mousePosition);
                    ray.direction = (Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.farClipPlane)) - ray.origin).normalized;
                }
#endif

                // Update the ray each frame.
                instancer.RayCollider = ray;

                // Visualize the ray and hits.
                Debug.DrawLine(instancer.RayCollider.origin, instancer.RayCollider.origin + instancer.RayCollider.direction * 100.0f, Color.red);

                // Clear the color on the last raycast hit,
                if (lastRaycastHit.Instance != null)
                {
                    lastRaycastHit.Instance.SetVector(colorID, Color.blue);
                }

                // Find the closest raycast hit.
                lastRaycastHit = new MeshInstancer.RaycastHit();
                lastRaycastHit.Distance = float.MaxValue;

                foreach (MeshInstancer.RaycastHit hit in instancer.RaycastHits)
                {
                    if (hit.Distance < lastRaycastHit.Distance)
                    {
                        lastRaycastHit = hit;
                    }
                }

                // Color the hit as red.
                if (lastRaycastHit.Instance != null)
                {
                    Debug.DrawLine(lastRaycastHit.Point, lastRaycastHit.Point + lastRaycastHit.Direction, Color.blue);
                    lastRaycastHit.Instance.SetVector(colorID, Color.red);
                }
            }
        }

        private void AsyncUpdate(float deltaTime, MeshInstancer.Instance instance)
        {
            SimulationData simulationData = (SimulationData)instance.UserData;

            // Update position.
            float speed = simulationData.speed;
            Vector3 forward = instance.LocalTransformation.GetColumn(0);
            Vector3 position = instance.LocalPosition + (forward * simulationData.speed * deltaTime);

            // Update orientation.
            float turnTime = simulationData.turnTime;
            Quaternion rotation = instance.LocalRotation;
            turnTime -= deltaTime;

            if (turnTime < 0.0f)
            {
                turnTime = ThreadSafeRandom.Range(minTurnUpdateTime, maxTurnUpdateTime);
                simulationData.targetRotation = Quaternion.LookRotation(ThreadSafeRandom.onUnitSphere, Vector3.up);
            }

            simulationData.turnTime = turnTime;

            // Update the user data.
            instance.UserData = simulationData;

            // Apply the final transformation.
            instance.LocalTransformation = Matrix4x4.TRS(position, Quaternion.Slerp(rotation, simulationData.targetRotation, deltaTime * speed), instance.LocalScale);
        }
    }
}

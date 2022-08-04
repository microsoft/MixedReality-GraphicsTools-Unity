// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Microsoft.MixedReality.GraphicsTools.Samples.MeshInstancing
{
    /// <summary>
    /// Renders a (dimension x dimension x dimension) cube of instances that can be clicked on and destroyed.
    /// </summary>
    public class InstancingMining : MonoBehaviour
    {
        [SerializeField]
        private MeshInstancer instancer = null;

        [Header("Simulation Properties")]
        [SerializeField, Min(1)]
        private int dimension = 10;

        private bool didStart = false;
        private MeshInstancer.RaycastHit lastRaycastHit;
        private Color lastColor;

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

        private void Update()
        {
            if (instancer.RaycastInstances)
            {
                // Update the ray each frame.
                instancer.RayCollider = GetRay();

                // Visualize the ray and hits.
                Debug.DrawLine(instancer.RayCollider.origin, instancer.RayCollider.origin + instancer.RayCollider.direction * 100.0f, Color.red);
                int colorID = Shader.PropertyToID("_Color");

                // Clear the color on the last raycast hit,
                if (lastRaycastHit.Instance != null)
                {
                    lastRaycastHit.Instance.SetVector(colorID, lastColor);
                }

                // Get the closest hit and color it red.
                if (instancer.GetClosestRaycastHit(ref lastRaycastHit))
                {
                    lastColor = lastRaycastHit.Instance.GetVector(colorID);
                    lastRaycastHit.Instance.SetVector(colorID, Color.red);

                    // Destroy the instance if the mouse is pressed.
#if ENABLE_INPUT_SYSTEM
                    if (Mouse.current.leftButton.isPressed)
#else
                    if (Input.GetMouseButtonDown(0))
#endif
                    {
                        lastRaycastHit.Instance.Destroy();
                        lastRaycastHit.Instance = null;
                    }
                }
            }
        }

        /// <summary>
        /// Render the bounds of the instances.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * dimension);
        }

        /// <summary>
        /// Create a bunch of instances in a cube formation.
        /// </summary>
        private void CreateInstances()
        {
            // Clear any existing instances.
            instancer = (instancer == null) ? GetComponent<MeshInstancer>() : instancer;
            instancer.Clear();

            int colorID = Shader.PropertyToID("_Color");
            Vector3 offset = Vector3.one * (dimension - 1) * 0.5f;

            for (int i = 0; i < dimension; ++i)
            {
                for (int j = 0; j < dimension; ++j)
                {
                    for (int k = 0; k < dimension; ++k)
                    {
                        var instance = instancer.Instantiate(new Vector3(i, j, k) - offset, Quaternion.identity, Vector3.one);
                        instance.SetVector(colorID, new Color((float)i / dimension, (float)j / dimension, (float)k / dimension, 1.0f));
                    }
                }
            }
        }

        /// <summary>
        /// Returns the ray going from the mouse (or camera if no mouse) into the world.
        /// </summary>
        private Ray GetRay()
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
            return ray;
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// A simple "fly" camera for moving the camera while playing with a mouse and keyboard.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        private class CameraState
        {
            public Vector3 Position;
            public Vector3 Rotation;

            public void SetFromTransform(Transform t)
            {
                Position = t.position;
                Rotation = t.eulerAngles;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 rotatedTranslation = Quaternion.Euler(new Vector3(Rotation.y, Rotation.x, Rotation.z)) * translation;
                Position += rotatedTranslation;
            }

            public void LerpTowards(CameraState target, float positionLerp, float rotationLerp)
            {
                Rotation = Vector3.Lerp(Rotation, target.Rotation, rotationLerp);
                Position = Vector3.Lerp(Position, target.Position, positionLerp);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(Rotation.y, Rotation.x, Rotation.z);
                t.position = Position;
            }
        }

        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        public float Boost = 1.0f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1.0f)]
        public float PositionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve MouseSensitivityCurve = new AnimationCurve(new Keyframe(0.0f, 0.5f, 0.0f, 5f), new Keyframe(1.0f, 2.5f, 0.0f, 0.0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1.0f)]
        public float RotationLerpTime = 0.1f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        public bool InvertY = false;

        [Header("Other Settings")]
        public bool showControlsText = true;

        private CameraState targetCameraState = new CameraState();
        private CameraState interpolatingCameraState = new CameraState();
        private List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();

        private void OnEnable()
        {
            targetCameraState.SetFromTransform(transform);
            interpolatingCameraState.SetFromTransform(transform);
        }

        private void Start()
        {
            SubsystemManager.GetInstances(xrDisplaySubsystems);

#if !UNITY_EDITOR
            Cursor.visible = false;
#endif
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // Editor input.
            if (!XRDeviceIsPresent())
            {
                // Lock cursor when right mouse button pressed.
                if (Input.GetMouseButtonDown(1))
                {
                    Cursor.lockState = CursorLockMode.Locked;
#if UNITY_EDITOR
                    Cursor.visible = false;
#endif
                }

                // Unlock when right mouse button released.
                if (Input.GetMouseButtonUp(1))
                {
                    Cursor.lockState = CursorLockMode.None;
#if UNITY_EDITOR
                    Cursor.visible = true;
#endif
                }

                // Rotation.
                if (Input.GetMouseButton(1))
                {
                    Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (InvertY ? 1.0f : -1.0f));

                    float mouseSensitivityFactor = MouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                    targetCameraState.Rotation.x += mouseMovement.x * mouseSensitivityFactor;
                    targetCameraState.Rotation.y += mouseMovement.y * mouseSensitivityFactor;
                }

                // Translation.
                Vector3 translation = GetInputTranslationDirection() * dt;

                // Speed up movement when shift key held.
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    translation *= 10.0f;
                }

                // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel).
                Boost += Input.mouseScrollDelta.y * dt;
                translation *= Mathf.Pow(2.0f, Boost);

                targetCameraState.Translate(translation);

                // Framerate-independent interpolation.
                // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time.
                float positionLerp = 1.0f - Mathf.Exp((Mathf.Log(1.0f - 0.99f) / PositionLerpTime) * dt);
                float rotationLerp = 1.0f - Mathf.Exp((Mathf.Log(1.0f - 0.99f) / RotationLerpTime) * dt);
                interpolatingCameraState.LerpTowards(targetCameraState, positionLerp, rotationLerp);

                interpolatingCameraState.UpdateTransform(transform);
            }
        }

        private void OnGUI()
        {
            if (!XRDeviceIsPresent() && showControlsText)
            {
                GUI.Label(new Rect(10.0f, 10.0f, 256.0f, 128.0f), "Camera Controls\nRight Click + Mouse Move to Rotate\n'W' 'A' 'S' 'D' to Translate");
            }
        }

        private bool XRDeviceIsPresent()
        {
            foreach (var xrDisplay in xrDisplaySubsystems)
            {
                if (xrDisplay != null && xrDisplay.running)
                {
                    return true;
                }
            }

            return false;
        }

        private Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = new Vector3();

            if (Input.GetKey(KeyCode.W)) { direction += Vector3.forward; }
            if (Input.GetKey(KeyCode.S)) { direction += Vector3.back; }
            if (Input.GetKey(KeyCode.A)) { direction += Vector3.left; }
            if (Input.GetKey(KeyCode.D)) { direction += Vector3.right; }
            if (Input.GetKey(KeyCode.Q)) { direction += Vector3.down; }
            if (Input.GetKey(KeyCode.E)) { direction += Vector3.up; }

            return direction;
        }
    }
}

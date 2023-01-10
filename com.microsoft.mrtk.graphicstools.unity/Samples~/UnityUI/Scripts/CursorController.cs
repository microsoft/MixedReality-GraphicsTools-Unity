// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
#define USE_INPUT_SYSTEM
#endif

#if GT_USE_UGUI
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if USE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif // USE_INPUT_SYSTEM

namespace Microsoft.MixedReality.GraphicsTools.Samples.UnityUI
{
    /// <summary>
    /// Translates and pulses a simple cursor based on scene raycasting.
    /// </summary>
    public class CursorController : MonoBehaviour
    {
        [Tooltip("The light visual to use for the cursor.")]
        public ProximityLight ProximityLight = null;

        private List<RaycastResult> raycastResults = new List<RaycastResult>();

        /// <summary>
        /// Called every frame to update the cursor transform.
        /// </summary>
        private void Update()
        {
            if (ProximityLight != null)
            {
                Vector3 hitPoint;
                GameObject hitObject;
                if (Cursor.visible && RaycastScene(out hitPoint, out hitObject))
                {
                    ProximityLight.enabled = true;
                    ProximityLight.transform.position = hitPoint;

#if USE_INPUT_SYSTEM
                    if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
#else
                    if (Input.GetMouseButtonDown(0))
#endif // USE_INPUT_SYSTEM
                    {
                        // Pulse the Graphics Tools/Standard and Graphics Tools/Standard Canvas shaders.
                        ProximityLight.Pulse();

                        // Pulse the Graphics Tools/Non - Canvas/Frontplate and Graphics Tools/Canvas/Frontplate shaders.
                        FrontPlatePulse pulse = hitObject.GetComponent<FrontPlatePulse>();

                        if (pulse != null)
                        {
                            pulse.PulseAt(hitPoint, 0);
                        }
                    }
                }
                else
                {
                    ProximityLight.enabled = false;
                }
            }
        }

        /// <summary>
        /// Raycasts the scene then event system. Returns true if something was hit.
        /// </summary>
        private bool RaycastScene(out Vector3 hitPoint, out GameObject hitObject)
        {
            Vector3 cursorPosition;
#if USE_INPUT_SYSTEM
            Vector2 position2D = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
            cursorPosition = new Vector3(position2D.x, position2D.y, 0.0f);
#else
            cursorPosition = Input.mousePosition;
#endif // USE_INPUT_SYSTEM

            Ray ray = Camera.main.ScreenPointToRay(cursorPosition);
            RaycastHit raycastResult;

            if (Physics.Raycast(ray, out raycastResult, 100))
            {
                hitPoint = raycastResult.point;
                hitObject = raycastResult.collider.gameObject;
                return true;
            }

            if (EventSystem.current)
            {
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = cursorPosition;
                EventSystem.current.RaycastAll(pointerEventData, raycastResults);

                if (raycastResults.Count != 0)
                {
                    hitPoint = raycastResults[0].worldPosition;
                    hitObject = raycastResults[0].gameObject;
                    return true;
                }
            }

            hitPoint = Vector3.zero;
            hitObject = null;
            return false;
        }
    }
}
#endif // GT_USE_UGUI
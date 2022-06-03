// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Ensures that a input system module exists for legacy input system projects. 
    /// </summary>
    public class AutoAddInputModules : MonoBehaviour
    {
        private void OnValidate()
        {
            // Check if a valid input module exists.
            EventSystem eventSystem = GetComponent<EventSystem>();

            if (eventSystem != null)
            {
                if (eventSystem.currentInputModule == null)
                {
                    // If the app is using the legacy input system and not the "new" one (they can be used at the same time). 
                    // Then add the default input module.
#if ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM

                    if (gameObject.GetComponent<StandaloneInputModule>() == null)
                    {
                        gameObject.AddComponent<StandaloneInputModule>();
                    }
#endif
                }
            }
        }
    }
}

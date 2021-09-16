﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// A collection of shared functionality for Graphics Tools shader GUIs.
    /// </summary>
    public static class ShaderGUIUtilities
    {
        /// <summary>
        /// GUI content styles which are common among shader GUIs.
        /// </summary>
        public static class Styles
        {
            public static readonly GUIContent DepthWriteWarning = new GUIContent("<color=yellow>Warning:</color> Depth buffer sharing is enabled for this project, but this material does not write depth. Enabling depth will improve reprojection, but may cause rendering artifacts in translucent materials.");
            public static readonly GUIContent DepthWriteFixNowButton = new GUIContent("Fix Now", "Enables Depth Write For This Material");
        }

        /// <summary>
        /// Checks if the project has depth buffer sharing enabled.
        /// </summary>
        /// <returns>True if the project has depth buffer sharing enabled, false otherwise.</returns>
        public static bool IsDepthBufferSharingEnabled()
        {
#if !UNITY_2020_1_OR_NEWER
            if (IsBuildTargetOpenVR())
            {
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                if (PlayerSettings.VROculus.sharedDepthBuffer)
#pragma warning restore 0618
                {
                    return true;
                }
            }
            else if (IsBuildTargetUWP())
            {
#if UNITY_2019_1_OR_NEWER
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                if (PlayerSettings.VRWindowsMixedReality.depthBufferSharingEnabled)
#pragma warning restore 0618
                {
                    return true;
                }
#else
                var playerSettings = GetSettingsObject("PlayerSettings");
                var property = playerSettings?.FindProperty("vrSettings.hololens.depthBufferSharingEnabled");
                if (property != null && property.boolValue)
                {
                    return true;
                }
#endif // UNITY_2019_1_OR_NEWER
            }
#endif // !UNITY_2020_1_OR_NEWER

            return true;
        }

        /// <summary>
        /// Displays a depth write warning and fix button if depth buffer sharing is enabled.
        /// </summary>
        /// <param name="materialEditor">The material editor to display the warning in.</param>
        /// <param name="dialogTitle">The title of the dialog window to display when the user selects the fix button.</param>
        /// <param name="dialogMessage">The message in the dialog window when the user selects the fix button.</param>
        /// <returns>True if the user opted to fix the warning, false otherwise.</returns>
        public static bool DisplayDepthWriteWarning(MaterialEditor materialEditor, string dialogTitle = "Depth Write", string dialogMessage = "Change this material to write to the depth buffer?")
        {
            bool dialogConfirmed = false;

            if (IsDepthBufferSharingEnabled())
            {
                var defaultValue = EditorStyles.helpBox.richText;
                EditorStyles.helpBox.richText = true;

                if (materialEditor.HelpBoxWithButton(Styles.DepthWriteWarning, Styles.DepthWriteFixNowButton))
                {
                    if (EditorUtility.DisplayDialog(dialogTitle, dialogMessage, "Yes", "No"))
                    {
                        dialogConfirmed = true;
                    }
                }

                EditorStyles.helpBox.richText = defaultValue;
            }

            return dialogConfirmed;
        }
    }
}

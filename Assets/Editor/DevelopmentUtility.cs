// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// General utility methods to help with Graphics Tools development.
    /// </summary>
    public class DevelopmentUtility
    {
        private static bool IsInitialized = false;
        private static bool InstalledViaPackage = true;

        private static readonly string VisibleSamplesPath = "Samples";
        private static readonly string HiddenSamplesPath = "Samples~";

        /// <summary>
        /// Performs one time initialization.
        /// </summary>
        private static void Initialize()
        {
            if (!IsInitialized)
            {
                // Check if Graphics Tools is currently installed as a package.
                InstalledViaPackage = IsPackageInstalled("com.microsoft.mixedreality.graphicstools.unity");
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Automatically shows the samples folder if currently hidden.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Show Samples")]
        public static void ShowSamples()
        {
            try
            {
                string hiddenPath = GetFullpath(HiddenSamplesPath);
                if (Directory.Exists(hiddenPath))
                {
                    Directory.Move(hiddenPath, GetFullpath(VisibleSamplesPath));
                    AssetDatabase.Refresh();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Menu item validation.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Show Samples", true)]
        public static bool ValidateShowSamples()
        {
            Initialize();
            return !InstalledViaPackage && Directory.Exists(GetFullpath(HiddenSamplesPath));
        }

        /// <summary>
        /// Automatically hides the samples folder if currently visible.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Hide Samples")]
        public static void HideSamples()
        {
            try
            {
                string visiblePath = GetFullpath(VisibleSamplesPath);
                if (Directory.Exists(visiblePath))
                {
                    Directory.Move(visiblePath, GetFullpath(HiddenSamplesPath));
                    File.Delete(visiblePath + ".meta"); // Remove the lingering meta files as well.
                    AssetDatabase.Refresh();
                }

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Menu item validation.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Hide Samples", true)]
        public static bool ValidateHideSamples()
        {
            Initialize();
            return !InstalledViaPackage && Directory.Exists(GetFullpath(VisibleSamplesPath));
        }

        /// <summary>
        /// Returns true if the package id is found in the manifest.json file.
        /// </summary>
        private static bool IsPackageInstalled(string packageId)
        {
            try
            {
                string manifest = File.ReadAllText("Packages/manifest.json");
                return manifest.Contains(packageId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return false;
        }

        /// <summary>
        /// Returns the full path of a directory relative to the Assets folder.
        /// </summary>
        private static string GetFullpath(string localPath)
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, localPath));
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
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
                string hiddenPath = GetFullPath(HiddenSamplesPath);
                if (Directory.Exists(hiddenPath))
                {
                    string visiblePath = GetFullPath(VisibleSamplesPath);
                    if (Directory.Exists(visiblePath) && IsDirectoryEmpty(visiblePath))
                    {
                        Directory.Delete(visiblePath);
                    }

                    Directory.Move(hiddenPath, visiblePath);

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

            if (InstalledViaPackage)
            {
                return false;
            }

            string path = GetFullPath(HiddenSamplesPath);
            return Directory.Exists(path) && !IsDirectoryEmpty(path);
        }

        /// <summary>
        /// Automatically hides the samples folder if currently visible.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Hide Samples")]
        public static void HideSamples()
        {
            try
            {
                string visiblePath = GetFullPath(VisibleSamplesPath);
                if (Directory.Exists(visiblePath))
                {
                    string hiddenPath = GetFullPath(HiddenSamplesPath);
                    if (Directory.Exists(hiddenPath) && IsDirectoryEmpty(hiddenPath))
                    {
                        Directory.Delete(hiddenPath);
                    }

                    Directory.Move(visiblePath, hiddenPath);
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

            if (InstalledViaPackage)
            {
                return false;
            }

            string path = GetFullPath(VisibleSamplesPath);
            return Directory.Exists(path) && !IsDirectoryEmpty(path);
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
        private static string GetFullPath(string localPath)
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, localPath));
        }

        /// <summary>
        /// Returns true if a directory has nothing in it.
        /// </summary>
        private static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
    }
}

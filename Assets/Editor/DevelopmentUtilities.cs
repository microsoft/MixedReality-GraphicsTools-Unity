// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// General utility methods to help with Graphics Tools development.
    /// </summary>
    public class DevelopmentUtilities
    {
        private static bool isInitialized = false;
        private static bool installedViaPackage = true;

        private static readonly string packageName = "com.microsoft.mixedreality.graphicstools.unity";
        private static readonly string visibleSamplesPath = "Samples";
        private static readonly string hiddenSamplesPath = "Samples~";
        private static readonly Regex quotesRegex = new Regex("(?<=\")(.*?)(?=\")");

        /// <summary>
        /// Performs one time initialization.
        /// </summary>
        private static void Initialize()
        {
            if (!isInitialized)
            {
                // Check if Graphics Tools is currently installed as a package.
                installedViaPackage = IsPackageInstalled(packageName);
                isInitialized = true;
            }
        }

        /// <summary>
        /// Opens the packages-lock.json files and removes the hash commit for the Graphics Tools package. This forces Unity to re-sync the latest version.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Install Latest Package")]
        public static void InstallLatestPackage()
        {
            try
            {
                string packagesLockPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Packages", "packages-lock.json");
                string[] lines = File.ReadAllLines(packagesLockPath);
                bool foundPackage = false;
                bool success = false;

                for (int i = 0; i < lines.Length; ++i)
                {
                    string line = lines[i];

                    if (line.Contains(packageName))
                    {
                        foundPackage = true;

                        continue;
                    }

                    if (foundPackage && line.Contains("hash"))
                    {
                        line = line.Replace("\"hash\": ", string.Empty);
                        Match match = quotesRegex.Match(line);

                        if (match.Success)
                        {
                            string hash = match.Groups[0].Value;
                            lines[i] = lines[i].Replace(hash, string.Empty);
                            success = true;

                            break;
                        }

                        foundPackage = false;
                    }
                }

                if (success)
                {
                    string output = string.Empty;

                    foreach (string line in lines)
                    {
                        output += line;
                        output += Environment.NewLine;
                    }

                    File.WriteAllText(packagesLockPath, output);
                    Debug.LogFormat("Installed the latest version of: {0}", packageName);
                    AssetDatabase.Refresh();

                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            Debug.LogErrorFormat("Failed to install the latest version of: {0}", packageName);
        }

        /// <summary>
        /// Menu item validation.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Install Latest Package", true)]
        public static bool ValidateInstallLatestPackage()
        {
            Initialize();

            return installedViaPackage;
        }

        /// <summary>
        /// Automatically shows the samples folder if currently hidden.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Show Samples")]
        public static void ShowSamples()
        {
            try
            {
                string hiddenPath = GetFullPath(hiddenSamplesPath);
                if (Directory.Exists(hiddenPath))
                {
                    string visiblePath = GetFullPath(visibleSamplesPath);
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

            if (installedViaPackage)
            {
                return false;
            }

            string path = GetFullPath(hiddenSamplesPath);
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
                string visiblePath = GetFullPath(visibleSamplesPath);
                if (Directory.Exists(visiblePath))
                {
                    string hiddenPath = GetFullPath(hiddenSamplesPath);
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

            if (installedViaPackage)
            {
                return false;
            }

            string path = GetFullPath(visibleSamplesPath);
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

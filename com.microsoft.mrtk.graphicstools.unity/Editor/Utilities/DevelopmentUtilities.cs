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
    public static class DevelopmentUtilities
    {
        public static readonly string PackageName = "com.microsoft.mrtk.graphicstools.unity";

        private static bool isInitialized = false;
        private static bool isPackageMutable = false;

        private static readonly string visibleSamplesPath = "../../com.microsoft.mrtk.graphicstools.unity/Runtime/Samples";
        private static readonly string hiddenSamplesPath = "../../com.microsoft.mrtk.graphicstools.unity/Samples~";

        /// <summary>
        /// Performs one time initialization.
        /// </summary>
        private static void Initialize()
        {
            if (!isInitialized)
            {
                isPackageMutable = !InstalledInProject(PackageName) ||
                                    InstalledInProjectAsLocalFolder(PackageName);

                isInitialized = true;
            }
        }

        /// <summary>
        /// Returns true if the Graphics Tools package can be altered.
        /// </summary>
        public static bool IsPackageMutable()
        {
            Initialize();

            return isPackageMutable;
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
                    if (Directory.Exists(visiblePath))
                    {
                        if (!IsDirectoryEmpty(visiblePath))
                        {
                            if (EditorUtility.DisplayDialog("Overwrite?",
                                                            string.Format("\"{0}\" contains files. Would you like to overwrite them?", visiblePath),
                                                            "Delete",
                                                            "Cancel"))
                            {
                                Directory.Delete(visiblePath, true);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            Directory.Delete(visiblePath);
                        }
                    }

                    Directory.Move(hiddenPath, visiblePath);

                    AssetDatabase.Refresh();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorUtility.DisplayDialog("Failed to show samples",
                            $"Showing samples failed.\n\nTry closing apps or windows that are currently using files within the \"Samples~\" folder.\n\nException: {e.Message}",
                            "Ok");
            }
        }

        /// <summary>
        /// Menu item validation.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Show Samples", true)]
        public static bool ValidateShowSamples()
        {
            if (!IsPackageMutable())
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
                    if (Directory.Exists(hiddenPath))
                    {
                        if (!IsDirectoryEmpty(hiddenPath))
                        {
                            if (EditorUtility.DisplayDialog("Overwrite?",
                                                            string.Format("\"{0}\" contains files. Would you like to overwrite them?", hiddenPath),
                                                            "Delete",
                                                            "Cancel"))
                            {
                                Directory.Delete(hiddenPath, true);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            Directory.Delete(hiddenPath);
                        }
                    }

                    Directory.Move(visiblePath, hiddenPath);
                    File.Delete(visiblePath + ".meta"); // Remove the lingering meta files as well.

                    AssetDatabase.Refresh();
                }

            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorUtility.DisplayDialog("Failed to hide samples", 
                                            $"Hiding samples failed.\n\nTry closing apps or windows that are currently using files within the \"Samples\" folder.\n\nException: {e.Message}",
                                            "Ok");
            }
        }

        /// <summary>
        /// Menu item validation.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Hide Samples", true)]
        public static bool ValidateHideSamples()
        {
            if (!IsPackageMutable())
            {
                return false;
            }

            string path = GetFullPath(visibleSamplesPath);
            return Directory.Exists(path) && !IsDirectoryEmpty(path);
        }

        /// <summary>
        /// Generates a material for Graphics Tools Standard shader..
        /// </summary>
        [MenuItem("Assets/Create/Graphics Tools/Material", false, 1)]
        public static void CreateGraphicsToolsMaterial()
        {
            var directory = AssetDatabase.GetAssetPath(Selection.activeObject);
            Material material = new Material(StandardShaderUtility.GraphicsToolsStandardShader);

            if (string.IsNullOrEmpty(directory))
            {
                directory = "Assets";
            }

            var extension = Path.GetExtension(directory);

            if (!string.IsNullOrEmpty(extension))
            {
                var filename = Path.GetFileName(directory);
                var startIndex = directory.LastIndexOf(filename) - 1;
                var count = filename.Length + 1;
                directory = directory.Remove(startIndex, count);
            }

            var path = directory + "/" + "NewGraphicsToolsMaterial.mat";
            var uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(material, uniquePath);
            AssetDatabase.SaveAssets();
            Selection.activeObject = material;
        }


        // Only show these menu items in the URP since built-in and HDRP have their own mipmap debug visualizers. 
#if GT_USE_URP
        /// <summary>
        /// Enables a debug draw mode in all scene views which tints pixels based on their texel to pixel ratio.
        ///  - Original color means it's a perfect match (1:1 texels to pixels ratio at the current distance and resolution).
        ///  - Red indicates that the texture is larger than necessary.
        ///  - Blue indicates that the texture could be larger.
        /// Note, the ideal texture sizes depend on the resolution at which your application will run and how close the camera can get to a surface.
        /// </summary>
        [MenuItem("Window/Graphics Tools/Draw Modes/Mipmaps - Enable")]
        public static void EnableDrawModeMipmaps()
        {
            Shader debugShader = Shader.Find("Hidden/Graphics Tools/DebugMipColor");

            if (debugShader != null)
            {
                foreach (SceneView sceneView in SceneView.sceneViews)
                {
                    sceneView.SetSceneViewShaderReplace(debugShader, null);
                }

                SceneView.RepaintAll();
            }
            else
            {
                Debug.LogWarning("Failed to find \"Hidden/Graphics Tools/DebugMipColor\" shader.");
            }
        }

        /// <summary>
        /// Disables the mipmap debug draw mode in all scene views. (See EnableDrawModeMipMaps for more info.)
        /// </summary>
        [MenuItem("Window/Graphics Tools/Draw Modes/Mipmaps - Disable")]
        public static void DisableDrawModeMipmaps()
        {
            foreach (SceneView sceneView in SceneView.sceneViews)
            {
                sceneView.SetSceneViewShaderReplace(null, null);
            }

            SceneView.RepaintAll();
        }
#endif // GT_USE_URP

        /// <summary>
        /// Returns true if the package id is found in the manifest.json file.
        /// </summary>
        private static bool InstalledInProject(string packageId)
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
        /// Returns true if the package id is found in the manifest.json file and is installed locally as a folder.
        /// </summary>
        private static bool InstalledInProjectAsLocalFolder(string packageId)
        {
            try
            {
                foreach (string line in File.ReadLines("Packages/manifest.json"))
                {
                    if (line.Contains(packageId) && line.Contains("file:") && !line.Contains(".tgz"))
                    {
                        return true;
                    }
                }
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


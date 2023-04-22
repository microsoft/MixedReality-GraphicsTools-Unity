// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Utility class to help restore materials which are assets (normally shared materials) to their original state when modified.
    /// </summary>
    public static class MaterialRestorer
    {
#if UNITY_EDITOR
        private class MaterialSnapshot
        {
            public Material Snapshot = null;
            public int RefCount = 0;

            public MaterialSnapshot(Material material)
            {
                Snapshot = material;
                RefCount = 1;
            }
        }

        private static Dictionary<Material, MaterialSnapshot> materialsToRestore = new Dictionary<Material, MaterialSnapshot>();
#endif

        /// <summary>
        /// Call this method to save a snapshot of a materials current state in time. 
        /// This only works with material assets.
        /// </summary>
        public static void Capture(Material material)
        {
#if UNITY_EDITOR
            if (material != null)
            {
                // Ensure this material represents an asset. 
                if (AssetDatabase.Contains(material))
                {
                    if (!materialsToRestore.ContainsKey(material))
                    {
                        materialsToRestore.Add(material, new MaterialSnapshot(new Material(material)));
                    }
                    else
                    {
                        ++materialsToRestore[material].RefCount;
                    }
                }
            }
#endif
        }

        /// <summary>
 /// Call this method to restore a material to the state in time it was called with AddMaterialSnapshot.
 /// This only works with material assets.
 /// </summary>
 public static void Restore(Material material)
        {
#if UNITY_EDITOR
 if (material != null)
            {
                MaterialSnapshot materialRef;
                if (materialsToRestore.TryGetValue(material, out materialRef))
                {
                    --materialRef.RefCount;

                    if (materialRef.RefCount == 0)
                    {
                        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(material.GetInstanceID(), out string guid, out long _) && !new GUID(guid).Empty())
                        {
 // Restore to the original material snapshot.
 material.CopyPropertiesFromMaterial(materialRef.Snapshot);

                            // SaveAssetIfDirty does not exist in 2021.1.10f1.
#if UNITY_2021_2_OR_NEWER
                            AssetDatabase.SaveAssetIfDirty(material);
#else
 AssetDatabase.SaveAssets();
#endif
 }
                        else
                        {
                            Debug.LogError($"Failed to restore material \"{material.name}\" because the material is no longer in the asset database.");
                        }

                        materialsToRestore.Remove(material);
                    }
                }
            }
#endif
 }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// The MaterialInstance behavior aides in tracking instance material lifetime and automatically destroys instanced materials for the user. 
    /// This utility component can be used as a replacement to <see href="https://docs.unity3d.com/ScriptReference/Renderer-material.html">Renderer.material</see> or 
    /// <see href="https://docs.unity3d.com/ScriptReference/Renderer-materials.html">Renderer.materials</see>. When invoking Unity's Renderer.material(s), Unity 
    /// automatically instantiates new materials. It is the caller's responsibility to destroy the materials when a material is no longer needed or the game object is 
    /// destroyed. The MaterialInstance behavior helps avoid material leaks and keeps material allocation paths consistent during edit and run time.
    /// </summary>
    [ExecuteAlways, RequireComponent(typeof(Renderer))]
    [AddComponentMenu("Scripts/GraphicsTools/MaterialInstance")]
    public class MaterialInstance : MonoBehaviour
    {
        /// <summary>
        /// Default postfix applied to all instance material names.
        /// </summary>
        public static readonly string InstancePostfix = " (Instance)";

        /// <summary>
        /// Returns true if a material is instanced (this is currently tracked by a material having the InstancePostfix in the name).
        /// </summary>
        public static bool IsInstance(Material material)
        {
            return ((material != null) && material.name.Contains(InstancePostfix));
        }

        /// <summary>
        /// Creates a new material instance based on a source material. The name is updated to reflect being an instance.
        /// </summary>
        public static Material Instance(Material source)
        {
            if (source == null)
            {
                return null;
            }

            if (IsInstance(source))
            {
                Debug.LogWarning($"The material ({source.name}) is already instanced and is being instanced multiple times.");
            }

            Material output = new Material(source);
            output.name += InstancePostfix;

            return output;
        }

        /// <summary>
        /// Returns the first instantiated Material assigned to the renderer, similar to <see href="https://docs.unity3d.com/ScriptReference/Renderer-material.html">Renderer.material</see>. 
        /// If any owner is specified the instanced material(s) will not be released until all owners are released. When a material
        /// is no longer needed ReleaseMaterial should be called with the matching owner.
        /// </summary>
        /// <param name="owner">An optional owner to track instance ownership.</param>
        /// <returns>The first instantiated Material.</returns>
        public Material AcquireMaterial(Object owner = null, bool instance = true)
        {
            if (owner != null)
            {
                materialOwners.Add(owner);
            }

            if (instance)
            {
                AcquireInstances();
            }

            if (instanceMaterials?.Length > 0)
            {
                return instanceMaterials[0];
            }

            return null;
        }

        /// <summary>
        /// Returns all the instantiated materials of this object, similar to <see href="https://docs.unity3d.com/ScriptReference/Renderer-materials.html">Renderer.materials</see>. 
        /// If any owner is specified the instanced material(s) will not be released until all owners are released. When a material
        /// is no longer needed ReleaseMaterial should be called with the matching owner.
        /// </summary>
        /// <param name="owner">An optional owner to track instance ownership.</param>
        /// <param name="instance">Should this acquisition attempt to instance materials?</param>
        /// <returns>All the instantiated materials.</returns>
        public Material[] AcquireMaterials(Object owner = null, bool instance = true)
        {
            if (owner != null)
            {
                materialOwners.Add(owner);
            }

            if (instance)
            {
                AcquireInstances();
            }

            return instanceMaterials;
        }

        /// <summary>
        /// Relinquishes ownership of a material instance. This should be called when a material is no longer needed
        /// after acquire ownership with AcquireMaterial(s).
        /// </summary>
        /// <param name="owner">The same owner which originally acquire ownership via AcquireMaterial(s).</param>
        /// <param name="instance">Should this acquisition attempt to instance materials?</param>
        /// <param name="autoDestroy">When ownership count hits zero should the MaterialInstance component be destroyed?</param>
        public void ReleaseMaterial(Object owner, bool autoDestroy = true)
        {
            materialOwners.Remove(owner);

            if (autoDestroy && materialOwners.Count == 0)
            {
                DestroySafe(this);

                // OnDestroy not called on inactive objects
                if (!gameObject.activeInHierarchy)
                {
                    RestoreRenderer();
                }
            }
        }

        /// <summary>
        /// Returns the first instantiated Material assigned to the renderer, similar to <see href="https://docs.unity3d.com/ScriptReference/Renderer-material.html">Renderer.material</see>.
        /// </summary>
        public Material Material
        {
            get { return AcquireMaterial(); }
        }

        /// <summary>
        /// Returns all the instantiated materials of this object, similar to <see href="https://docs.unity3d.com/ScriptReference/Renderer-materials.html">Renderer.materials</see>.
        /// </summary>
        public Material[] Materials
        {
            get { return AcquireMaterials(); }
        }

        private Renderer CachedRenderer
        {
            get
            {
                if (cachedRenderer == null)
                {
                    cachedRenderer = GetComponent<Renderer>();
                }

                return cachedRenderer;
            }
        }

        private Renderer cachedRenderer = null;

        [SerializeField, HideInInspector]
        private Material[] defaultMaterials = null;
        private Material[] instanceMaterials = null;
        private bool initialized = false;
        private bool materialsInstanced = false;
        private readonly HashSet<Object> materialOwners = new HashSet<Object>();
        private readonly List<Material> sharedMaterials = new List<Material>();

        #region MonoBehaviour Implementation

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            // If the materials get changed via outside of MaterialInstance.
            CachedRenderer.GetSharedMaterials(sharedMaterials);

            if (!MaterialsMatch(sharedMaterials, instanceMaterials))
            {
                // Re-create the material instances.
                var newDefaultMaterials = new Material[sharedMaterials.Count];
                var min = Math.Min(newDefaultMaterials.Length, defaultMaterials.Length);

                // Copy the old defaults.
                for (var i = 0; i < min; ++i)
                {
                    newDefaultMaterials[i] = defaultMaterials[i];
                }

                // Patch in the new defaults.
                for (var i = 0; i < newDefaultMaterials.Length; ++i)
                {
                    var material = sharedMaterials[i];

                    if (!IsInstance(material))
                    {
                        newDefaultMaterials[i] = material;
                    }
                }

                defaultMaterials = newDefaultMaterials;
                CreateInstances();

                // Notify owners of the change.
                foreach (var owner in materialOwners)
                {
                    (owner as IMaterialInstanceOwner)?.OnMaterialChanged(this);
                }
            }
        }

        private void OnDestroy()
        {
            RestoreRenderer();
        }

        private void RestoreRenderer()
        {
            if (CachedRenderer != null && defaultMaterials != null)
            {
                CachedRenderer.sharedMaterials = defaultMaterials;
            }

            DestroyMaterials(instanceMaterials);
            instanceMaterials = null;
        }

        #endregion MonoBehaviour Implementation

        private void Initialize()
        {
            if (!initialized && CachedRenderer != null)
            {
                // Cache the default materials if ones do not already exist.
                if (!HasValidMaterial(defaultMaterials))
                {
                    defaultMaterials = CachedRenderer.sharedMaterials;
                }
                else if (!materialsInstanced) // Restore the clone to its initial state.
                {
                    CachedRenderer.sharedMaterials = defaultMaterials;
                }

                initialized = true;
            }
        }

        private void AcquireInstances()
        {
            if (CachedRenderer != null)
            {
                CachedRenderer.GetSharedMaterials(sharedMaterials);
                if (!MaterialsMatch(sharedMaterials, instanceMaterials))
                {
                    CreateInstances();
                }
            }
        }

        private void CreateInstances()
        {
            // Initialize must get called to set the defaultMaterials in case CreateInstances get's invoked before Awake.
            Initialize();

            DestroyMaterials(instanceMaterials);
            instanceMaterials = InstanceMaterials(defaultMaterials);

            if (CachedRenderer != null && instanceMaterials != null)
            {
                CachedRenderer.sharedMaterials = instanceMaterials;
            }

            materialsInstanced = true;
        }

        private static bool MaterialsMatch(List<Material> a, Material[] b)
        {
            if (a?.Count != b?.Length)
            {
                return false;
            }

            for (int i = 0; i < a?.Count; ++i)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static Material[] InstanceMaterials(Material[] source)
        {
            if (source == null)
            {
                return null;
            }

            var output = new Material[source.Length];

            for (var i = 0; i < source.Length; ++i)
            {
                output[i] = Instance(source[i]);
            }

            return output;
        }

        private static void DestroyMaterials(Material[] materials)
        {
            if (materials != null)
            {
                for (var i = 0; i < materials.Length; ++i)
                {
                    DestroySafe(materials[i]);
                }
            }
        }

        private static bool HasValidMaterial(Material[] materials)
        {
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    if (material != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void DestroySafe(Object toDestroy)
        {
            if (toDestroy != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(toDestroy);
                }
                else
                {
#if UNITY_EDITOR
                    // Let Unity handle unload of unused assets if lifecycle is transitioning from editor to play mode
                    // Deferring the call during this transition would destroy reference only after play mode Awake, leading to possible broken material references on TMPro objects
                    if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    {
                        EditorApplication.delayCall += () =>
                        {
                            if (toDestroy != null)
                            {
                                DestroyImmediate(toDestroy);
                            }
                        };
                    }
#endif
                }
            }
        }
    }
}

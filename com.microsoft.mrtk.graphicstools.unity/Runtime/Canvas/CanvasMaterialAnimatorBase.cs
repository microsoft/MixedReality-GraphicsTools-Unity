// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

#if GT_USE_UGUI
using UnityEngine.UI;
#endif // GT_USE_UGUI

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// The base class for all CanvasMaterialAnimators generated via Assets > Graphics Tools > Generate Canvas Material Animator.
    /// This behavior will expose all material properties of a Graphic's material so they can animated by Unity's animation system.
    /// </summary>
    [ExecuteInEditMode]
#if GT_USE_UGUI
    [RequireComponent(typeof(Graphic))]
#endif // GT_USE_UGUI
    public abstract class CanvasMaterialAnimatorBase : MonoBehaviour, IAnimationWindowPreview
    {
        /// <summary>
        /// Flag to keep track of if the material properties are folded out (visible) or not.
        /// </summary>
        public bool MaterialPropertiesFoldedOut
        {
            get => materialPropertiesFoldedOut;
            set => materialPropertiesFoldedOut = value;
        }

        [Tooltip("Flag to keep track of if the material properties are folded out (visible) or not.")]
        [SerializeField, NotKeyable, HideInInspector]
        private bool materialPropertiesFoldedOut = false;

        /// <summary>
        /// When animated should a new material be instantiated?
        /// </summary>
        public bool UseInstanceMaterials
        {
            get { return instanceMaterials; }
            set 
            { 
                if (isInitialized)
                {
                    Debug.LogError("Cannot toggle UseInstanceMaterials after initialization.");
                }
                else
                {
                    instanceMaterials = value;
                }
            }
        }

        [Tooltip("When animated should a new material be created?")]
        [SerializeField, NotKeyable, HideInInspector]
        private bool instanceMaterials = false;

        /// <summary>
        /// Accessor to the material used during preview in editor.
        /// </summary>
        public Material PreviewMaterial
        {
            get => previewMaterial;
            private set => previewMaterial = value;
        }

        private Material previewMaterial = null;

#if GT_USE_UGUI
        private Graphic graphic
        {
            get
            {
                if (cachedGraphic == null)
                {
                    cachedGraphic = GetComponent<Graphic>();
                }

                return cachedGraphic;
            }
        }

        private Graphic cachedGraphic;
#endif // GT_USE_UGUI

        private bool isInitialized = false;
#if GT_USE_UGUI
        private Material currentMaterial = null;
        private Material sharedMaterial = null;
#endif // GT_USE_UGUI

        #region MonoBehaviour Implementation

        /// <summary>
        /// State clean up.
        /// </summary>
        private void OnDestroy()
        {
            Terminate();
        }

        /// <summary>
        /// Event for when the animation system updates any serialized properties.
        /// </summary>
        private void OnDidApplyAnimationProperties()
        {
            ApplyToMaterial();
        }

        /// <summary>
        /// Editor-only function that Unity calls when the script is loaded or a value changes in the Inspector.
        /// </summary>
        private void OnValidate()
        {
            // If we are in the act of previewing apply any material changes from the inspector. 
            if (previewMaterial != null)
            {
                ApplyToMaterial();
            }
        }

#endregion MonoBehaviour Implementation

#region IAnimationWindowPreview Implementation

        /// <summary>
        /// Notification callback when the Animation window starts previewing an AnimationClip.
        /// </summary>
        public void StartPreview()
        {
#if GT_USE_UGUI
            previewMaterial = graphic.material;
#endif // GT_USE_UGUI
        }

        /// <summary>
        /// Notification callback when the Animation window stops previewing an AnimationClip.
        /// </summary>
        public void StopPreview()
        {
#if GT_USE_UGUI
            Terminate();

            graphic.material = previewMaterial;
            previewMaterial = null;
#endif // GT_USE_UGUI
        }

        /// <summary>
        /// Notification callback when the Animation Window updates its PlayableGraph before sampling an AnimationClip.
        /// </summary>
        public void UpdatePreviewGraph(PlayableGraph graph)
        {
            // Intentionally blank to implement the interface.
        }

        /// <summary>
        /// The Animation window calls this function when it samples an AnimationClip for the first time.
        /// </summary>
        public Playable BuildPreviewGraph(PlayableGraph graph, Playable inputPlayable)
        {
            // Intentionally blank to implement the interface.
            return inputPlayable;
        }

#endregion IAnimationWindowPreview Implementation

#region BaseCanvasMaterialAnimator Implementation

        /// <summary>
        /// Initializes all material properties based on the default material.
        /// </summary>
        private void Initialize()
        {
#if GT_USE_UGUI
            Material material = graphic.material;
            sharedMaterial = material;

            if (material != null)
            {
                if (material.shader.name == GetTargetShaderName())
                {
                    InitializeFromMaterial(material);

                    if (UseInstanceMaterials)
                    {
                        currentMaterial = MaterialInstance.Instance(material);
                        graphic.material = currentMaterial;
                    }
                    else
                    {
                        currentMaterial = material;
                        MaterialRestorer.Capture(material);
                    }

                    isInitialized = true;
                }
                else
                {
                    Debug.LogErrorFormat("Failed to initialize CanvasMaterialAnimator. Expected shader {0} but using {1}.", GetTargetShaderName(), material.shader.name);
                }
            }
#endif // GT_USE_UGUI
        }

        /// <summary>
        /// Destroys any assets this created in initialize.
        /// </summary>
        private void Terminate()
        {
#if GT_USE_UGUI
            if (UseInstanceMaterials)
            {
                if (currentMaterial != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(currentMaterial);
                    }
                    else
                    {
                        DestroyImmediate(currentMaterial);
                    }

                    currentMaterial = null;
                }
            }
            else
            {
                MaterialRestorer.Restore(currentMaterial);
            }

            isInitialized = false;
#endif
        }

        /// <summary>
        /// Call this method after any properties have been modified via code and they need to be applied to the current material. 
        /// </summary>
        public void ApplyToMaterial()
        {
#if GT_USE_UGUI
            if (!isInitialized)
            {
                Initialize();
            }

            Material material = graphic.material;

            if (material != null)
            {
                // Release the previous material and obtain the current.
                if (material != currentMaterial)
                {
                    Terminate();
                    Initialize();
                }

                ApplyToMaterial(material);
            }
#endif
        }

        /// <summary>
        /// Call this method when the animator is idle and you wish to return the graphic's material to its default state. 
        /// This will restore any batching behavior before animating.
        /// </summary>
        public void RestoreToSharedMaterial()
        {
#if GT_USE_UGUI
            Terminate();

            graphic.material = sharedMaterial;
            sharedMaterial = null;
#endif
        }

        /// <summary>
        /// This method will extract all material properties from the material passed in to apply default values to 
        /// the serialized properties of the current CanvasMaterialAnimator.
        /// </summary>
        public abstract void InitializeFromMaterial(Material material);

        /// <summary>
        /// This method will apply all serialized material properties on the current CanvasMaterialAnimator to the material passed in.
        /// </summary>
        public abstract void ApplyToMaterial(Material material);

        /// <summary>
        /// Returns the name of the shader this class was generated from.
        /// </summary>
        public abstract string GetTargetShaderName();

#endregion BaseCanvasMaterialAnimator Implementation
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// The base class for all CanvasMaterialAnimators generated via Assets > Graphics Tools > Generate Canvas Material Animator.
    /// This behavior will expose all material properties of a CanvasRenderer so they can animated by Unity's animation system.
    /// </summary>
    [ExecuteInEditMode, RequireComponent(typeof(CanvasRenderer))]
    public abstract class BaseCanvasMaterialAnimator : MonoBehaviour, IAnimationWindowPreview
    {
        /// <summary>
        /// "When animated should a new material be instantiated?"
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
        [SerializeField]
        private bool instanceMaterials = false;

        private bool isInitialized = false;
        private CanvasRenderer canvasRenderer = null;
        private Material currentMaterial = null;
        private Material previewMaterial = null;

        #region MonoBehaviour Implementation

        /// <summary>
        /// Called when the script instance is loaded.
        /// </summary>
        private void Awake()
        {
            canvasRenderer = GetComponent<CanvasRenderer>();
        }

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
            if (!isInitialized)
            {
                Initialize();
            }

            Material material = canvasRenderer.GetMaterial();

            if (material != null)
            {
                if (material != currentMaterial)
                {
                    // Do we need to handle this case?
                    Debug.LogWarning("Material swapped after initialization. Animation will not be applied.");
                }
                else
                {
                    ApplyToMaterial(material);
                }
            }
        }

        #endregion MonoBehaviour Implementation

        #region IAnimationWindowPreview Implementation

        /// <summary>
        /// Notification callback when the Animation window starts previewing an AnimationClip.
        /// </summary>
        public void StartPreview()
        {
            previewMaterial = canvasRenderer.GetMaterial();
        }

        /// <summary>
        /// Notification callback when the Animation window stops previewing an AnimationClip.
        /// </summary>
        public void StopPreview()
        {
            Terminate();

            isInitialized = false;
            canvasRenderer.SetMaterial(previewMaterial, 0);
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
            Material material = canvasRenderer.GetMaterial();

            if (material != null)
            {
                if (material.shader.name == GetTargetShaderName())
                {
                    InitializeFromMaterial(material);

                    if (UseInstanceMaterials)
                    {
                        currentMaterial = new Material(material);
                        canvasRenderer.SetMaterial(currentMaterial, 0);
                    }
                    else
                    {
                        currentMaterial = material;
                        MaterialRestorer.AddMaterialSnapshot(material);
                    }

                    isInitialized = true;
                }
                else
                {
                    Debug.LogErrorFormat("Failed to initialize CanvasMaterialAnimator. Expected shader {0} but using {1}.", GetTargetShaderName(), material.shader.name);
                }
            }
        }

        /// <summary>
        /// Destroys any assets this created in initialize.
        /// </summary>
        private void Terminate()
        {
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
                MaterialRestorer.RestoreMaterialSnapshot(currentMaterial);
            }
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

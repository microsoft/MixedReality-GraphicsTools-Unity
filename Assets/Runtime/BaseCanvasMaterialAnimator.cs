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
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasRenderer))]
    public abstract class BaseCanvasMaterialAnimator : MonoBehaviour, IAnimationWindowPreview
    {
        /// <summary>
        /// "When animated should a new material be instantiated?"
        /// </summary>
        public bool UseInstanceMaterials
        {
            get { return instanceMaterials; }
            set { instanceMaterials = value; }
        }

        [Tooltip("When animated should a new material be created?")]
        [SerializeField]
        private bool instanceMaterials = false;

        private bool isInitialized = false;
        private CanvasRenderer canvasRenderer = null;
        private Material instanceMaterial = null;
        private Material previewSourceMaterial = null;

        #region MonoBehaviour Implementation

        /// <summary>
        /// Called when the script instance is loaded.
        /// </summary>
        private void Awake()
        {
            canvasRenderer = GetComponent<CanvasRenderer>();
        }

        /// <summary>
        /// Cleans up any materials this component created.
        /// </summary>
        private void OnDestroy()
        {
            if (instanceMaterial != null)
            {
                Destroy(instanceMaterial);
                instanceMaterial = null;
            }
        }

        /// <summary>
        /// Event for when the animation system updates any serilzed properties. 
        /// This method will (lazy) create a material instance and pass it to the derived class to modify based on animation updates.
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
                // Create a material instance when first animated. We must instance materials when not playing
                // to avoid changing materials on disk.
                if (UseInstanceMaterials || !Application.isPlaying)
                {
                    if (instanceMaterial == null)
                    {
                        instanceMaterial = Instantiate(material);
                        canvasRenderer.SetMaterial(instanceMaterial, 0);
                        material = instanceMaterial;
                    }
                }

                ApplyToMaterial(material);
            }
        }

        #endregion MonoBehaviour Implementation

        #region IAnimationWindowPreview Implementation

        /// <summary>
        /// Notification callback when the Animation window starts previewing an AnimationClip.
        /// </summary>
        public void StartPreview()
        {
            // Cache the material so that we can restore it on StopPreview.
            previewSourceMaterial = canvasRenderer.GetMaterial();
        }

        /// <summary>
        /// Notification callback when the Animation window stops previewing an AnimationClip.
        /// </summary>
        public void StopPreview()
        {
            canvasRenderer.SetMaterial(previewSourceMaterial, 0);

            if (instanceMaterial != null)
            {
                DestroyImmediate(instanceMaterial);
                instanceMaterial = null;
            }
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
                    isInitialized = true;
                }
                else
                {
                    Debug.LogErrorFormat("Failed to initialize CanvasMaterialAnimator. Expected shader {0} but using {1}.", GetTargetShaderName(), material.shader.name);
                }
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

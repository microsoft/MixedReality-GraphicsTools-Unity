// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Animates material properties for the Graphics Tools/Non-Canvas/Frontplate and 
    /// Graphics Tools/Canvas/Frontplate shaders to play a pulse ring effect at a point in 3D space.
    /// </summary>
    [AddComponentMenu("Scripts/GraphicsTools/FrontPlatePulse")]
    public class FrontPlatePulse : MonoBehaviour
    {
        /// <summary>
        /// The total duration of the pulse effect in seconds.
        /// </summary>
        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        [SerializeField]
        [Tooltip("The total duration of the pulse effect in seconds.")]
        private float duration = 1.2f;

        private bool isInitialized = false;
        private Renderer _renderer = null;
        private MaterialPropertyBlock materialProperty = null;
        private CanvasMaterialAnimatorCanvasFrontplate animator = null;
        private Graphic graphic = null;

        private struct PulseState
        {
            public string useGlobalBlob, blobPosition, blobPulse, blobFade;
            public bool pulseActive;
            public float startTime;
            public Vector3 initialPosition;

            public PulseState(string useGlobalBlob, string blobPosition, string blobPulse, string blobFade)
            {
                pulseActive = false;
                startTime = 0.0f;
                initialPosition = Vector3.zero;
                this.useGlobalBlob = useGlobalBlob;
                this.blobPosition = blobPosition;
                this.blobPulse = blobPulse;
                this.blobFade = blobFade;
            }
        }

        private PulseState[] states = new PulseState[2]
        {
            new PulseState("_Use_Global_Left_Index_", "_Blob_Position_", "_Blob_Pulse_", "_Blob_Fade_"),
            new PulseState("_Use_Global_Right_Index_", "_Blob_Position_2_", "_Blob_Pulse_2_", "_Blob_Fade_2_")
        };

        #region MonoBehaviour Implementation

        private void Start()
        {
            _renderer = gameObject.GetComponent<Renderer>();

            if (_renderer != null)
            {
                materialProperty = new MaterialPropertyBlock();
            }
            else
            {
                animator = GetComponent<CanvasMaterialAnimatorCanvasFrontplate>();

                if (animator == null)
                {
                    graphic = GetComponent<Graphic>();
                    MaterialRestorer.Capture(graphic.material);
                }
            }

            Debug.Assert(_renderer != null || animator != null || graphic != null,
                         "The FrontPlatePulse component must have a Renderer, CanvasMaterialAnimatorCanvasFrontplate, or Graphic component.");

            isInitialized = true;
        }

        private void OnDestroy()
        {
            if (graphic != null)
            {
                // When in editor restore any material changes made during play.
                MaterialRestorer.Restore(graphic.material);
            }
        }

        private void OnEnable()
        {
            if (isInitialized)
            {
                for (int i = 0; i < states.Length; i++)
                {
                    if (states[i].pulseActive)
                    {
                        ResetPulse(ref states[i], i);
                    }
                }
            }
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Starts the pulse animation at a position and index.
        /// </summary>
        public void PulseAt(Vector3 position, int index)
        {
            if (isInitialized)
            {
                if (states[index].pulseActive)
                {
                    StopAllCoroutines();
                    ResetPulse(ref states[index], index);
                }

                states[index].startTime = Time.time;
                states[index].pulseActive = true;
                states[index].initialPosition = position;

                if (_renderer != null)
                {
                    _renderer.GetPropertyBlock(materialProperty);

                    materialProperty.SetFloat(states[index].useGlobalBlob, 0.0f);

                    _renderer.SetPropertyBlock(materialProperty);
                }
                else if (animator != null)
                {
                    if (index == 0)
                    {
                        animator._Use_Global_Left_Index_ = 0.0f;
                    }
                    else
                    {
                        animator._Use_Global_Right_Index_ = 0.0f;
                    }

                    animator.ApplyToMaterial();
                }
                else if (graphic != null)
                {
                    graphic.material.SetFloat(states[index].useGlobalBlob, 0.0f);
                }

                StartCoroutine(UpdatePulse());
            }
        }

        /// <summary>
        /// Overloaded method to specify if this is a left or right pulse.
        /// </summary>
        public void Pulse(Vector3 position, bool left)
        {
            PulseAt(position, left ? 0 : 1);
        }

        /// <summary>
        /// Pulse without a spatial input source to define position or index.
        /// </summary>
        public void PulseNonSpatial()
        {
            PulseAt(transform.position, 0);
        }

        /// <summary>
        /// Returns true of if any PulseState is active.
        /// </summary>
        public bool IsPulsing()
        {
            if (!isInitialized)
            {
                return false;
            }

            return states[0].pulseActive || states[1].pulseActive;
        }

        private IEnumerator UpdatePulse()
        {
            if (isInitialized)
            {
                while (IsPulsing())
                {
                    for (int i = 0; i < states.Length; ++i)
                    {
                        UpdateState(ref states[i], i);
                    }

                    yield return null;
                }
            }
        }

        private void UpdateState(ref PulseState state, int index)
        {
            if (state.pulseActive)
            {
                if (_renderer != null)
                {
                    _renderer.GetPropertyBlock(materialProperty);
                }

                // The front plate plane may change position frame to frame, so we need to update it.
                Vector3 position = new Plane(transform.forward, transform.position).ClosestPointOnPlane(state.initialPosition);

                if (_renderer != null)
                {
                    materialProperty.SetVector(states[index].blobPosition, position);
                }
                else if (animator != null)
                {
                    if (index == 0)
                    {
                        animator._Blob_Position_ = position;
                    }
                    else
                    {
                        animator._Blob_Position_2_ = position;
                    }
                }
                else if (graphic != null)
                {
                    graphic.material.SetVector(states[index].blobPosition, position);
                }

                float t = (Time.time - state.startTime) / Duration * 2.0f;

                if (t < 0.5f)
                {
                    if (_renderer != null)
                    {
                        materialProperty.SetFloat(state.blobPulse, t * 2.0f);
                    }
                    else if (animator != null)
                    {
                        if (index == 0)
                        {
                            animator._Blob_Pulse_ = t * 2.0f;
                        }
                        else
                        {
                            animator._Blob_Pulse_2_ = t * 2.0f;
                        }
                    }
                    else if (graphic != null)
                    {
                        graphic.material.SetFloat(state.blobPulse, t * 2.0f);
                    }
                }
                else if (t < 1.0f)
                {
                    if (_renderer != null)
                    {
                        materialProperty.SetFloat(state.useGlobalBlob, 1.0f);
                        materialProperty.SetFloat(state.blobPulse, 0.0f);
                        materialProperty.SetFloat(state.blobFade, 2.0f * t - 1.0f);
                    }
                    else if (animator != null)
                    {
                        if (index == 0)
                        {
                            animator._Use_Global_Left_Index_ = 1.0f;
                            animator._Blob_Pulse_ = 0.0f;
                            animator._Blob_Fade_ = 2.0f * t - 1.0f;
                        }
                        else
                        {
                            animator._Use_Global_Right_Index_ = 1.0f;
                            animator._Blob_Pulse_2_ = 0.0f;
                            animator._Blob_Fade_2_ = 2.0f * t - 1.0f;
                        }
                    }
                    else if (graphic != null)
                    {
                        graphic.material.SetFloat(state.useGlobalBlob, 1.0f);
                        graphic.material.SetFloat(state.blobPulse, 0.0f);
                        graphic.material.SetFloat(state.blobFade, 2.0f * t - 1.0f);
                    }
                }
                else
                {
                    state.pulseActive = false;

                    if (_renderer != null)
                    {
                        materialProperty.SetFloat(state.blobFade, 1.0f);
                    }
                    else if (animator != null)
                    {
                        if (index == 0)
                        {
                            animator._Blob_Fade_ = 1.0f;
                        }
                        else
                        {
                            animator._Blob_Fade_2_ = 1.0f;
                        }
                    }
                    else if (graphic != null)
                    {
                        graphic.material.SetFloat(state.blobFade, 1.0f);
                    }
                }

                if (_renderer != null)
                {
                    _renderer.SetPropertyBlock(materialProperty);
                }

                if (animator != null)
                {
                    animator.ApplyToMaterial();
                }
            }
        }

        private void ResetPulse(ref PulseState state, int index)
        {
            if (isInitialized)
            {
                state.pulseActive = false;

                if (_renderer != null)
                {
                    _renderer.GetPropertyBlock(materialProperty);

                    materialProperty.SetFloat(state.blobFade, 1.0f);
                    materialProperty.SetFloat(state.useGlobalBlob, 1.0f);
                    materialProperty.SetFloat(state.blobPulse, 0.0f);

                    _renderer.SetPropertyBlock(materialProperty);
                }
                else if (animator != null)
                {
                    if (index == 0)
                    {
                        animator._Blob_Fade_ = 1.0f;
                        animator._Use_Global_Left_Index_ = 1.0f;
                        animator._Blob_Pulse_ = 0.0f;
                    }
                    else
                    {
                        animator._Blob_Fade_2_ = 1.0f;
                        animator._Use_Global_Right_Index_ = 1.0f;
                        animator._Blob_Pulse_2_ = 0.0f;
                    }

                    animator.ApplyToMaterial();
                }
                else if (graphic != null)
                {
                    graphic.material.SetFloat(state.blobFade, 1.0f);
                    graphic.material.SetFloat(state.useGlobalBlob, 1.0f);
                    graphic.material.SetFloat(state.blobPulse, 0.0f);
                }
            }
        }
    }
}

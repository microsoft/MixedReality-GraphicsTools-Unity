// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Utility component to animate and visualize a light that can be used with the GraphicsTools/Standard and 
    /// GraphicsTools/Standard Canvas shaders that have the "_PROXIMITY_LIGHT" keyword enabled.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Scripts/GraphicsTools/ProximityLight")]
    public class ProximityLight : BaseLight
    {
        // Two proximity lights are supported at this time.
        private const int proximityLightCount = 2;
        private const int proximityLightDataSize = 6;
        private static List<ProximityLight> activeProximityLights = new List<ProximityLight>(proximityLightCount);
        private static Vector4[] proximityLightData = new Vector4[proximityLightCount * proximityLightDataSize];
        private static int proximityLightDataID;
        private static int globalPositionLeftID;
        private static int globalPositionRightID;
        private static int lastProximityLightUpdate = -1;

        /// <summary>
        /// All settings that represent a proximity light.
        /// </summary>
        [Serializable]
        public class LightSettings
        {
            public enum Handedness
            {
                None = 0,
                Left = 1,
                Right = 2
            }

            /// <summary>
            /// The handedness of the finger this proximityLight is attached to.
            /// Necessary to populate the Global_Left/Right_Index_Tip_Position properties.
            /// </summary>
            public Handedness HandednessType
            {
                get { return handedness; }
                set { handedness = value; }
            }

            [Tooltip("What hand is this light assigned to?")]
            [SerializeField]
            private Handedness handedness = Handedness.None;

            /// <summary>
            /// Specifies the radius of the ProximityLight effect when near to a surface.
            /// </summary>
            public float NearRadius
            {
                get { return nearRadius; }
                set { nearRadius = value; }
            }

            [Header("Proximity Settings")]
            [Tooltip("Specifies the radius of the ProximityLight effect when near to a surface.")]
            [SerializeField, Min(0.0f)]
            private float nearRadius = 0.05f;

            /// <summary>
            /// Specifies the radius of the ProximityLight effect when far from a surface.
            /// </summary>
            public float FarRadius
            {
                get { return farRadius; }
                set { farRadius = value; }
            }

            [Tooltip("Specifies the radius of the ProximityLight effect when far from a surface.")]
            [SerializeField, Min(0.0f)]
            private float farRadius = 0.2f;

            /// <summary>
            /// Specifies the distance a ProximityLight must be from a surface to be considered near.
            /// </summary>
            public float NearDistance
            {
                get { return nearDistance; }
                set { nearDistance = value; }
            }

            [Tooltip("Specifies the distance a ProximityLight must be from a surface to be considered near.")]
            [SerializeField, Min(0.0f)]
            private float nearDistance = 0.02f;

            /// <summary>
            /// When a ProximityLight is near, the smallest size percentage from the far size it can shrink to.
            /// </summary>
            public float MinNearSizePercentage
            {
                get { return minNearSizePercentage; }
                set { minNearSizePercentage = value; }
            }

            [Tooltip("When a ProximityLight is near, the smallest size percentage from the far size it can shrink to.")]
            [SerializeField, Min(0.0f)]
            private float minNearSizePercentage = 0.35f;

            /// <summary>
            /// The color of the ProximityLight gradient at the center (RGB) and (A) is gradient extent.
            /// </summary>
            public Color CenterColor
            {
                get { return centerColor; }
                set { centerColor = value; }
            }

            [Header("Color Settings")]
            [Tooltip("The color of the ProximityLight gradient at the center (RGB) and (A) is gradient extent.")]
            [ColorUsageAttribute(true, true)]
            [SerializeField]
            private Color centerColor = new Color(54.0f / 255.0f, 142.0f / 255.0f, 250.0f / 255.0f, 0.0f / 255.0f);

            /// <summary>
            /// The color of the ProximityLight gradient at the center (RGB) and (A) is gradient extent.
            /// </summary>
            public Color MiddleColor
            {
                get { return middleColor; }
                set { middleColor = value; }
            }

            [Tooltip("The color of the ProximityLight gradient at the middle (RGB) and (A) is gradient extent.")]
            [SerializeField]
            [ColorUsageAttribute(true, true)]
            private Color middleColor = new Color(47.0f / 255.0f, 132.0f / 255.0f, 255.0f / 255.0f, 51.0f / 255.0f);

            /// <summary>
            /// The color of the ProximityLight gradient at the center (RGB) and (A) is gradient extent.
            /// </summary>
            public Color OuterColor
            {
                get { return outerColor; }
                set { outerColor = value; }
            }

            [Tooltip("The color of the ProximityLight gradient at the outer (RGB) and (A) is gradient extent.")]
            [SerializeField]
            [ColorUsageAttribute(true, true)]
            private Color outerColor = new Color((82.0f * 3.0f) / 255.0f, (31.0f * 3.0f) / 255.0f, (191.0f * 3.0f) / 255.0f, 255.0f / 255.0f);
        }

        /// <summary>
        /// Accessor to the light settings.
        /// </summary>
        public LightSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        [SerializeField]
        private LightSettings settings = new LightSettings();

        private float pulseTime;
        private float pulseFade;

        /// <summary>
        /// Initiates a pulse, if one is not already occurring, which simulates a user touching a surface.
        /// </summary>
        /// <param name="pulseDuration">How long in seconds should the pulse animate over.</param>
        /// <param name="fadeBegin">At what point during the pulseDuration should the pulse begin to fade out as a percentage. Range should be [0, 1].</param>
        /// <param name="fadeSpeed">The speed to fade in and out.</param>
        public void Pulse(float pulseDuration = 0.2f, float fadeBegin = 0.8f, float fadeSpeed = 10.0f)
        {
            if (pulseTime <= 0.0f)
            {
                StartCoroutine(PulseRoutine(pulseDuration, fadeBegin, fadeSpeed));
            }
        }

        #region MonoBehaviour Implementation

        /// <inheritdoc/>
        protected override void OnDisable()
        {
            base.OnDisable();
            pulseTime = 0.0f;
            pulseFade = 0.0f;
        }

        #endregion MonoBehaviour Implementation

        #region BaseLight Implementation

        /// <inheritdoc/>
        protected override void Initialize()
        {
            proximityLightDataID = Shader.PropertyToID("_ProximityLightData");
            globalPositionLeftID = Shader.PropertyToID("Global_Left_Index_Tip_Position");
            globalPositionRightID = Shader.PropertyToID("Global_Right_Index_Tip_Position");
        }

        /// <inheritdoc/>
        protected override void AddLight()
        {
            if (activeProximityLights.Count == proximityLightCount)
            {
                Debug.LogWarningFormat("Max proximity light count {0} exceeded. {1} will not be considered by the Graphics Tools/Standard shader until other lights are removed.", proximityLightCount, gameObject.name);
            }

            activeProximityLights.Add(this);
        }

        /// <inheritdoc/>
        protected override void RemoveLight()
        {
            activeProximityLights.Remove(this);
        }

        /// <inheritdoc/>
        protected override void UpdateLights(bool forceUpdate = false)
        {
            if (lastProximityLightUpdate == -1)
            {
                Initialize();
            }

            if (!forceUpdate && (Time.frameCount == lastProximityLightUpdate))
            {
                return;
            }

            // Initially disable the light for the Graphics Tools front plate shaders by moving them "far away." If enabled, they will be
            // moved to the correct location in the below loop.
            // TODO - [Cameron-Micka] add a better way to disable these lights?
            Vector3 farAwayPosition = Vector3.one * float.MaxValue;
            Shader.SetGlobalVector(globalPositionLeftID, farAwayPosition);
            Shader.SetGlobalVector(globalPositionRightID, farAwayPosition);

            for (int i = 0; i < proximityLightCount; ++i)
            {
                ProximityLight light = (i >= activeProximityLights.Count) ? null : activeProximityLights[i];
                int dataIndex = i * proximityLightDataSize;

                if (light)
                {
                    // Pass data into the Graphics Tools/Standard shaders. 
                    proximityLightData[dataIndex] = new Vector4(light.transform.position.x,
                                                                light.transform.position.y,
                                                                light.transform.position.z,
                                                                1.0f);
                    float pulseScaler = 1.0f + light.pulseTime;
                    proximityLightData[dataIndex + 1] = new Vector4(light.Settings.NearRadius * pulseScaler,
                                                                    1.0f / Mathf.Clamp(light.Settings.FarRadius * pulseScaler, 0.001f, 1.0f),
                                                                    1.0f / Mathf.Clamp(light.Settings.NearDistance * pulseScaler, 0.001f, 1.0f),
                                                                    Mathf.Clamp01(light.Settings.MinNearSizePercentage));
                    proximityLightData[dataIndex + 2] = new Vector4(light.Settings.NearDistance * light.pulseTime,
                                                                    Mathf.Clamp01(1.0f - light.pulseFade),
                                                                    0.0f,
                                                                    0.0f);
                    proximityLightData[dataIndex + 3] = light.Settings.CenterColor;
                    proximityLightData[dataIndex + 4] = light.Settings.MiddleColor;
                    proximityLightData[dataIndex + 5] = light.Settings.OuterColor;

                    // Pass data into the Graphics Tools front plate shaders.
                    switch (light.Settings.HandednessType)
                    {
                        case LightSettings.Handedness.Left:
                            {
                                Shader.SetGlobalVector(globalPositionLeftID, light.transform.position);
                            }
                            break;
                        case LightSettings.Handedness.Right:
                            {
                                Shader.SetGlobalVector(globalPositionRightID, light.transform.position);
                            }
                            break;
                    }
                }
                else
                {
                    // Disable the light for the Graphics Tools/Standard shaders.
                    proximityLightData[dataIndex] = Vector4.zero;
                }
            }

            Shader.SetGlobalVectorArray(proximityLightDataID, proximityLightData);

            lastProximityLightUpdate = Time.frameCount;
        }

        #endregion BaseLight Implementation

        private IEnumerator PulseRoutine(float pulseDuration, float fadeBegin, float fadeSpeed)
        {
            float pulseTimer = 0.0f;

            while (pulseTimer < pulseDuration)
            {
                pulseTimer += Time.deltaTime;
                pulseTime = pulseTimer / pulseDuration;

                if (pulseTime > fadeBegin)
                {
                    pulseFade += Time.deltaTime;
                }

                yield return null;
            }

            while (pulseFade < 1.0f)
            {
                pulseFade += Time.deltaTime * fadeSpeed;

                yield return null;
            }

            pulseTime = 0.0f;

            while (pulseFade > 0.0f)
            {
                pulseFade -= Time.deltaTime * fadeSpeed;

                yield return null;
            }

            pulseFade = 0.0f;
        }
    }
}

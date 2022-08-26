// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Utility component to animate and visualize a light that can be used with the GraphicsTools/Standard and 
    /// GraphicsTools/Standard Canvas shaders that have the "_DISTANT_LIGHT" keyword enabled.
    /// 
    /// A DistantLight can be used as a replacement for a Unity DirectionalLight. The main purpose of DistantLights is to recreate 
    /// important light sources for environments, i.e. the sun or moon.
    /// 
    /// By default The Graphics Tools/Standard and Graphics Tools/Standard Canvas shaders use the first Unity DirectionalLight 
    /// added to a scene. But, there are some cases where you need a light that is decoupled from the environment (such as 
    /// in user interfaces). Unity normally performs this decoupling with light masks. But, light masks can be expensive on some platforms.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Scripts/GraphicsTools/DistantLight")]
    public class DistantLight : BaseLight
    {
        // The Graphics Tools/Standard and Graphics Tools/Standard Canvas shaders supports up to one (1) distant lights.
        private const int distantLightCount = 1;
        private const int distantLightDataSize = 2;
        private static readonly Vector4 invalidLightDirection = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);

        private static List<DistantLight> activeDistantLights = new List<DistantLight>(distantLightCount);
        private static Vector4[] distantLightData = new Vector4[distantLightDataSize * distantLightCount];
        private static int _DistantLightDataID;
        private static int lastDistantLightUpdate = -1;

        [Tooltip("Specifies the light color.")]
        [SerializeField]
        private Color color = new Color(255.0f / 255.0f, 244.0f / 255.0f, 214.0f / 255.0f, 1.0f);

        /// <summary>
        /// Specifies the light color.
        /// </summary>
        public Color Color
        {
            get => color;
            set => color = value;
        }

        [Tooltip("Scales the brightness of the light.")]
        [SerializeField, Min(0.0f)]
        private float intensity = 1.0f;

        /// <summary>
        /// Scales the brightness of the light.
        /// </summary>
        public float Intensity
        {
            get => intensity;
            set => intensity = Mathf.Max(0.0f, value);
        }

        #region BaseLight Implementation

        /// <inheritdoc/>
        protected override void Initialize()
        {
            _DistantLightDataID = Shader.PropertyToID("_DistantLightData");
        }

        /// <inheritdoc/>
        protected override void AddLight()
        {
            if (activeDistantLights.Count == distantLightCount)
            {
                Debug.LogWarningFormat("Max distant light count {0} exceeded. {1} will not be considered by the Graphics Tools/Standard shader until other lights are removed.", distantLightCount, gameObject.name);
            }

            activeDistantLights.Add(this);
        }

        /// <inheritdoc/>
        protected override void RemoveLight()
        {
            activeDistantLights.Remove(this);
        }

        /// <inheritdoc/>
        protected override void UpdateLights(bool forceUpdate = false)
        {
            if (lastDistantLightUpdate == -1)
            {
                Initialize();
            }

            if (!forceUpdate && (Time.frameCount == lastDistantLightUpdate))
            {
                return;
            }

            for (int i = 0; i < distantLightCount; ++i)
            {
                DistantLight light = (i >= activeDistantLights.Count) ? null : activeDistantLights[i];
                int dataIndex = i * distantLightDataSize;

                if (light)
                {
                    Vector4 direction = -light.transform.forward;
                    distantLightData[dataIndex] = new Vector4(direction.x,
                                                              direction.y,
                                                              direction.z,
                                                              1.0f);
                    distantLightData[dataIndex + 1] = new Vector4(light.Color.r * intensity,
                                                                  light.Color.g * intensity,
                                                                  light.Color.b * intensity, 
                                                                  1.0f);
                }
                else
                {
                    distantLightData[dataIndex] = invalidLightDirection;
                    distantLightData[dataIndex + 1] = Vector4.zero;
                }
            }

            Shader.SetGlobalVectorArray(_DistantLightDataID, distantLightData);

            lastDistantLightUpdate = Time.frameCount;
        }

        #endregion BaseLight Implementation
    }
}


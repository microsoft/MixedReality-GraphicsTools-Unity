// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Manages creating and updating blurred background maps for use with the acrylic material.
    /// </summary>
    [ExecuteInEditMode]
    public class AcrylicLayerManager : MonoBehaviour
    {
     
        private static AcrylicLayerManager instance;

        public static AcrylicLayerManager Instance
        {
            get { return instance; }
        }
        [Experimental]
        [Tooltip("Whether this platforms supports creating a blurred acrylic map")]
        [SerializeField]
        private bool acrylicSupported = true;

        public bool AcrylicSupported
        {
            get { return acrylicSupported; }
            set { acrylicSupported = value; }
        }

        public enum AcrylicMethod { CopyFramebuffer, RenderToTexture }

        [Tooltip("Capture method for background image")]
        [SerializeField]
        private AcrylicMethod captureMethod = AcrylicMethod.CopyFramebuffer;

        [Tooltip("Use 16-bit or 24-bit depth buffer")]
        [SerializeField]
        private bool _24BitDepthBuffer = false;

        public bool UseOnlyMainCamera
        {
            get { return useOnlyMainCamera; }
            private set { useOnlyMainCamera = value; }
        }

        [Tooltip("When true the targetCamera is always updated to be the camera tagged as MainCamera.")]
        [SerializeField]
        private bool useOnlyMainCamera = false;

        [Tooltip("Which camera to add the blur pass(es) to. None applies to all cameras. Setting UseOnlyMainCamera to true will overwrite this.")]
        [SerializeField]
        private Camera targetCamera = null;

        public int RendererIndex
        {
            get { return rendererIndex; }
            set 
            { 
                if (initialized)
                {
                    Debug.LogWarning("Failed to set the render index because the layer manager is already initialized.");
                    return;
                }

                rendererIndex = value; 
            }
        }

        [Tooltip("Which renderer to use in the UniversalRenderPipelineAsset.")]
        [SerializeField]
        private int rendererIndex = 0;

        public enum BlurMethod { Kawase, Dual }

        [Header("Blur")]

        [SerializeField]
        private BlurMethod filterMethod = BlurMethod.Kawase;

        public BlurMethod FilterMethod
        {
            get { return filterMethod; }
            set
            {
                if (initialized)
                {
                    Debug.LogWarning("Failed to set the filter method because the layer manager is already initialized.");
                    return;
                }

                filterMethod = value;
            }
        }

        [Tooltip("Material for kawase blur")]
        [SerializeField]
        private Material kawaseFilterMaterial = null;

        public Material KawaseFilterMaterial
        {
            get { return kawaseFilterMaterial; }
            private set { kawaseFilterMaterial = value; }
        }

        [Tooltip("Material for dual blur filter")]
        [SerializeField]
        private Material dualFilterMaterial = null;

        public Material DualFilterMaterial
        {
            get { return dualFilterMaterial; }
            private set { dualFilterMaterial = value; }
        }

        [Header("Blur Map Update Options")]

        [Tooltip("Whether to automatically update blur map")]
        [SerializeField]
        private bool autoUpdateBlurMap = true;

        public bool AutoUpdateBlurMap
        {
            get { return autoUpdateBlurMap; }
            set { autoUpdateBlurMap = value; }
        }

        [Tooltip("How often to record a new background image for the blur map")]
        [SerializeField]
        [Range(1, 60)]
        private int updatePeriod = 1;

        [Tooltip("Frames over which to blend from old to new blur map.")]
        [SerializeField]
        [Range(0, 60)]
        private int blendFrames = 0;

        [Tooltip("Material used when blending between old and new blur maps.")]
        [SerializeField]
        private Material blendMaterial = null;

        [SerializeField]
        private List<AcrylicLayer.Settings> layers;

        public List<AcrylicLayer.Settings> Layers
        {
            get { return layers; }
            private set { layers = value; }
        }

        [Header("Editor Only")]

#pragma warning disable 414
        [Tooltip("Enable all acrylic layers on start.")]
        [SerializeField]
        private bool enableLayersOnStart = false;

        [Tooltip("When the capture framebuffer method is used, keep layers on after playing in editor.")]
        [SerializeField]
        private bool retainInEditor = false;
#pragma warning restore 414

#region private properties

        private List<AcrylicLayer> layerData = new List<AcrylicLayer>();
#if UNITY_2021_2_OR_NEWER
        private UniversalRendererData rendererData = null;
#else
        private ForwardRendererData rendererData = null;
#endif
        private bool initialized = false;
        private bool acrylicActive = true;
        private const string namePrefix = "AcrylicBlur";
        private bool ExecuteBeforeRenderAdded = false;
        private Coroutine updateRoutine = null;

        // In older versions of the URP all intermediate texture behavior works like "Always."
#if UNITY_2021_2_OR_NEWER
        private IntermediateTextureMode intermediateTextureMode = IntermediateTextureMode.Always;
#endif

#endregion

#region Monobehavior methods

        private void OnDestroy()
        {
            switch (captureMethod)
            {
                case AcrylicMethod.CopyFramebuffer:
                    RemoveAllLayers();
#if UNITY_EDITOR
                    if (retainInEditor)
                    {
                        AddLayersAsPersistent();
                        return;
                    }
#endif
                    break;

                case AcrylicMethod.RenderToTexture:
                    if (ExecuteBeforeRenderAdded)
                    {
                        RenderPipelineManager.beginCameraRendering -= ExecuteBeforeCameraRender;
                    }
                    break;

                default:
                    break;
            }

            for (int i = 0; i < layerData.Count; ++i)
            {
                layerData[i].Dispose();
            }

            layerData.Clear();
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogErrorFormat("An instance of the AcrylicLayerManager already exists on gameobject {0}", instance.name);
                return;
            }

            instance = this;

            InitializeBlurTexturesToBlack();
        }

        private void Start()
        {
            if (AcrylicSupported)
            {
                Initialize();
#if UNITY_EDITOR
                if (enableLayersOnStart)
                {
                    AddAllLayers();
                }
#endif
            }
        }

        private void OnValidate()
        {
            InitializeBlurTexturesToBlack();
        }

#endregion

#region Public methods
        public void EnableLayer(int i)
        {
            if (!AcrylicSupported) return;

            Initialize();
            if (i >= 0 && i < layerData.Count)
            {
                layerData[i].activeCount++;
                if (layerData[i].activeCount == 1)
                {
                    layerData[i].frameCount = 0;
                    layerData[i].firstFrameRendered = false;
                    if (captureMethod == AcrylicMethod.CopyFramebuffer)
                    {
                        UpdateActiveLayers();
                    }
                    StartUpdateRoutine();
                }
            }
        }

        public void DisableLayer(int i)
        {
            if (!AcrylicSupported) return;

            Initialize();
            if (i >= 0 && i < layerData.Count && layerData[i].activeCount > 0)
            {
                layerData[i].activeCount--;
                if (layerData[i].activeCount == 0 && rendererData != null)
                {
                    UpdateActiveLayers();
                }
            }
        }

        public bool LayerVisible(int i)
        {
            return (i >= 0 && i < layerData.Count && layerData[i].activeCount > 0 && rendererData != null);
        }


        public bool AcrylicActive
        {
            get
            {
                return acrylicActive && AcrylicSupported;
            }
            set
            {
                if (value != acrylicActive)
                {
                    acrylicActive = value;
                    if (AcrylicSupported)
                    {
                        UpdateActiveLayers();
                    }
                }
            }
        }

        public void SetTargetCamera(Camera newtargetCamera)
        {
            if (targetCamera != newtargetCamera)
            {
                targetCamera = newtargetCamera;

                foreach (AcrylicLayer layer in layerData)
                {
                    layer.SetTargetCamera(newtargetCamera);
                }
            }
        }

#endregion

#region private methods
        private void Initialize()
        {
            if (!initialized && AcrylicSupported)
            {
                initialized = true;
                InitializeRendererData();
                if (rendererData != null)
                {
                    RemoveExistingAcrylicPasses();
                }
                CreateLayers();
                if (captureMethod == AcrylicMethod.RenderToTexture)
                {
                    ExecuteBeforeRenderAdded = true;
                    RenderPipelineManager.beginCameraRendering += ExecuteBeforeCameraRender;
                }
            }
        }

        private void InitializeBlurTexturesToBlack()
        {
            if (layers != null)
            {
                foreach (AcrylicLayer.Settings layer in layers)
                {
                    if (!string.IsNullOrEmpty(layer.blurTextureName))
                    {
                        Shader.SetGlobalTexture(layer.blurTextureName, Texture2D.blackTexture);
                    }
                }
            }
        }

        private void CreateLayers()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layerData.Add(CreateLayer(layers[i], i));
            }
        }

        private void AddAllLayers()
        {
            if (rendererData != null)
            {
                for (int i = 0; i < layerData.Count; i++)
                {
                    EnableLayer(i);
                }
            }
        }

        private AcrylicLayer CreateLayer(AcrylicLayer.Settings settings, int index)
        {
            AcrylicLayer layer = new AcrylicLayer(targetCamera, settings, index, _24BitDepthBuffer ? 24 : 16, filterMethod==BlurMethod.Dual, kawaseFilterMaterial, dualFilterMaterial);
            if (captureMethod == AcrylicMethod.CopyFramebuffer)
            {
                layer.CreateRendererFeatures();
#if UNITY_EDITOR
                if (retainInEditor)
                {
                    layer.MakeFeaturesPersistent(rendererData);
                }
#endif
            }
            return layer;
        }

        private void UpdateActiveLayers()
        {
            RemoveAllLayers();
            if (AcrylicActive)
                AddActiveLayers();
        }

        private void InitializeRendererData()
        {
            var pipeline = ((UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline);
            if (pipeline == null)
            {
                Debug.LogWarning("Universal Render Pipeline not found.  Acrylic material maps will not be available.");
            } 
            else 
            {
                FieldInfo propertyInfo = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
#if UNITY_2021_2_OR_NEWER
                rendererData = ((ScriptableRendererData[])propertyInfo?.GetValue(pipeline))?[rendererIndex] as UniversalRendererData;
#else
                rendererData = ((ScriptableRendererData[])propertyInfo?.GetValue(pipeline))?[rendererIndex] as ForwardRendererData;
#endif
            }
        }

        private void RemoveExistingAcrylicPasses()
        {
            List<UnityEngine.Rendering.Universal.ScriptableRendererFeature> passes = rendererData.rendererFeatures;
            for (int i = passes.Count - 1; i >= 0; i--)
            {
                if (passes[i].name.Contains("Acrylic"))
                {
#if (UNITY_EDITOR)
                    UnityEditor.AssetDatabase.RemoveObjectFromAsset(passes[i]);
#endif
                    rendererData.rendererFeatures.Remove(passes[i]);
                }
            }
        }

        private bool AnyLayersActive()
        {
            for (int i = 0; i < layerData.Count; i++)
            {
                if (layerData[i].activeCount > 0) return true;
            }
            return false;
        }

        private bool AnyLayersNeedUpdating()
        {
            for (int i = 0; i < layerData.Count; i++)
            {
                if (layerData[i].activeCount > 0)
                {
                    if (autoUpdateBlurMap || !layerData[i].FirstFrameGenerated) return true;
                }
            }
            return false;
        }

#endregion

#region Render to texture methods

        private void ExecuteBeforeCameraRender(ScriptableRenderContext context, Camera camera)
        {
            if (captureMethod == AcrylicMethod.RenderToTexture)
            {
                for (int i = 0; i < layerData.Count; i++)
                {
                    if (layerData[i].activeCount > 0 && layerData[i].frameCount == 0)
                    {
                        layerData[i].RenderToTexture(context, camera, updatePeriod, blendFrames, CumulativeLayerMask(i));
                    }
                }
            }
        }

        // returns the union of the current layer mask and all layer masks 'above' it
        private LayerMask CumulativeLayerMask(int layer)
        {
            LayerMask mask = layers[layer].renderLayers;
            for (int i = layer + 1; i < layers.Count; i++)
            {
                mask |= layers[i].renderLayers;
            }
            return mask;
        }

#endregion

#region Periodic update methods

        private void StartUpdateRoutine()
        {
            if (updateRoutine == null)
            {
                updateRoutine = StartCoroutine(UpdateRoutine());
            }
        }

        private IEnumerator UpdateRoutine()
        {
            //while (AnyLayersActive())
            while (AnyLayersNeedUpdating())
            {
                bool updateActiveFeatures = false;
                for (int i = 0; i < layerData.Count; i++)
                {
                    if (layerData[i].activeCount > 0)
                    {
                        if (UseOnlyMainCamera)
                        {
                            layerData[i].SetTargetCamera(Camera.main);
                        }

                        layerData[i].UpdateFrame(rendererData, captureMethod == AcrylicMethod.CopyFramebuffer, updatePeriod, blendFrames, blendMaterial, autoUpdateBlurMap);
                        if (captureMethod==AcrylicMethod.CopyFramebuffer)
                        {
                            bool inList = layerData[i].InFeaturesList(rendererData);
                            if (layerData[i].CaptureNextFrame)
                            {
                                if (!inList) updateActiveFeatures = true;
                                if (updatePeriod == 1 && autoUpdateBlurMap) layerData[i].ForceCaptureNextFrame();  //needed if updatePeriod changed in editor
                            }
                            else
                            {
                                if (inList) updateActiveFeatures = true;
                            }
                        }
                    }
                }
                if (updateActiveFeatures) UpdateActiveLayers();
                yield return null;
            }
            updateRoutine = null;
        }
#endregion

#region Copy framebuffer related methods

        private void AddActiveLayers()
        {
            if (captureMethod != AcrylicMethod.CopyFramebuffer) return;

#if UNITY_2021_2_OR_NEWER
            if (rendererData.intermediateTextureMode != IntermediateTextureMode.Always)
            {
                intermediateTextureMode = rendererData.intermediateTextureMode;
                rendererData.intermediateTextureMode = IntermediateTextureMode.Always;
            }
#endif

            for (int i = 0; i < layerData.Count; i++)
            {
                if (layerData[i].activeCount > 0 && layerData[i].CaptureNextFrame)
                {
                    layerData[i].AddLayerRendererFeatures(rendererData, updatePeriod < 2 && autoUpdateBlurMap);
                }
            }
        }

        private void RemoveAllLayers()
        {
            if (captureMethod != AcrylicMethod.CopyFramebuffer) return;

#if UNITY_2021_2_OR_NEWER
            if (intermediateTextureMode != IntermediateTextureMode.Always)
            {
                rendererData.intermediateTextureMode = intermediateTextureMode;
                intermediateTextureMode = IntermediateTextureMode.Always;
            }
#endif

            for (int i = 0; i < layerData.Count; i++)
            {
                layerData[i].RemoveLayerRendererFeatures(rendererData);
            }
        }

        private void AddLayersAsPersistent()
        {
            if (captureMethod != AcrylicMethod.CopyFramebuffer) return;

#if UNITY_2021_2_OR_NEWER
            if (rendererData.intermediateTextureMode != IntermediateTextureMode.Always)
            {
                intermediateTextureMode = rendererData.intermediateTextureMode;
                rendererData.intermediateTextureMode = IntermediateTextureMode.Always;
            }
#endif

            for (int i = 0; i < layerData.Count; i++)
            {
                layerData[i].AddLayerRendererFeatures(rendererData, true);
            }
        }
#endregion
    }
}
#endif // GT_USE_URP

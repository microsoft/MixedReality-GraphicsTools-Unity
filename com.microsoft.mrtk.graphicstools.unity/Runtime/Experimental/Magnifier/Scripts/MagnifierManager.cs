using ICSharpCode.NRefactory.Ast;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Microsoft.MixedReality.GraphicsTools.AcrylicLayerManager;

namespace Microsoft.MixedReality.GraphicsTools
{
  
    public class MagnifierManager : MonoBehaviour
    {
        public class Settings
        {


        }
        [Tooltip("When to copy the framebuffer in the rendering pipeline. No effect when render-to-texture is used.")]
        public RenderPassEvent captureEvent = RenderPassEvent.AfterRenderingTransparents;
        [Tooltip("If not nothing, creates render object features for the specified layers.")]
        public LayerMask renderLayers;
        public Material blitMaterial;

        public string blitSourceTextureName;
        public int blitMaterialPassIndex;
       
        public BufferType sourceType = BufferType.CameraColor;
        public BufferType destyinationType = BufferType.Custom;
        public string sourceTextureId;
        public string destinationTextureId = "MagnifierTexture";
        public bool restoreCameraColorTarget;
        public Settings settings;
        private int index;
        private static MagnifierManager instance;
        private DrawFullscreenFeature magnifierFeature;
        private ScriptableRendererFeature renderTransparent;
        public static MagnifierManager Instance
        {
            get { return instance; }
        }
        [Tooltip("Which renderer to use in the UniversalRenderPipelineAsset.")]
        [SerializeField]
        private int rendererIndex = 0;
        private bool initialized = false;
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
#if UNITY_2021_2_OR_NEWER
        private UniversalRendererData rendererData = null;
#else
        private ForwardRendererData rendererData = null;
#endif
        // Start is called before the first frame update
        void OnEnable()
        {
            Initialize();

        }
        private void OnDisable()
        {
            rendererData.rendererFeatures.Remove(magnifierFeature);
            rendererData.rendererFeatures.Remove(renderTransparent);
            rendererData.SetDirty();
        }
        private void Initialize()
        {
            if (!initialized)
            {
                initialized = true;
                InitializeRendererData();
                if (rendererData != null)
                {
                    // RemoveExistingAcrylicPasses();
                }
                CreateRendererFeatures();
                

            }
        }
        private void InitializeRendererData()
        {
            var pipeline = ((UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline);
            if (pipeline == null)
            {
                Debug.LogWarning("Universal Render Pipeline not found");
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
        public void CreateRendererFeatures()
        {
            magnifierFeature = setMagnifier("Magnifier Fullscreen Feature" + index, captureEvent, blitMaterial, Camera.main);
            rendererData.rendererFeatures.Add(magnifierFeature);
            renderTransparent = CreateRenderObjectsFeature("Render After Transparent " + index, RenderQueueType.Transparent, captureEvent);
            rendererData.rendererFeatures.Add(renderTransparent);
            rendererData.SetDirty();
        }
        public DrawFullscreenFeature setMagnifier(string name, RenderPassEvent passEvent, Material blitMaterial, Camera targetCamera)
        {
            DrawFullscreenFeature magnifierFeature = null;

            magnifierFeature = ScriptableObject.CreateInstance<DrawFullscreenFeature>();
            magnifierFeature.name = name;
            magnifierFeature.settings.renderPassEvent = captureEvent;
            magnifierFeature.settings.SourceTextureId = "";
            magnifierFeature.settings.DestinationTextureId = "MagnifierTexture";
            magnifierFeature.settings.DestinationType = BufferType.Custom;
            magnifierFeature.settings.SourceType= BufferType.CameraColor;
            magnifierFeature.settings.BlitMaterialPassIndex = -1;
            magnifierFeature.settings.BlitMaterial = blitMaterial;
            //  magnifierFeature.targetCamera = targetCamera;


            return magnifierFeature;
        }

        private ScriptableRendererFeature CreateRenderObjectsFeature(string name, RenderQueueType queue, RenderPassEvent renderPassEvent)
        {
            RenderObjects r = ScriptableObject.CreateInstance<RenderObjects>();
            r.name = name;
            r.settings.Event = renderPassEvent;
            r.settings.filterSettings.RenderQueueType = queue;
            r.settings.filterSettings.LayerMask = renderLayers;
            return r;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Persists tool options for Ambient Occlusion
    /// </summary>
    public class AmbientOcclusionSettings : ScriptableObject
    {
        [Tooltip("The number of rays cast into the hemisphere above the surface, per vertex. Bigger numbers will be slower.")]
        [SerializeField] internal int _samplesPerVertex;
        [SerializeField] internal float _maxSampleDistance;
        [SerializeField] internal bool _smoothNormals;
        [SerializeField] internal int _referenceVertexIndex;
        [SerializeField] internal bool _showOrigin;
        [SerializeField] internal Color _originColor;
        [SerializeField] internal float _originRadius;
        [SerializeField] internal bool _showNormal;
        [SerializeField] internal Color _normalColor;
        [SerializeField] internal float _normalScale;
        [SerializeField] internal bool _meshNormals;
        [SerializeField] internal bool _meshSmoothNormals;
        [SerializeField] internal bool _showBentNormal;
        [SerializeField] internal bool _vertexID;
        [SerializeField] internal Color _bentNormalColor;
        [SerializeField] internal float _bentNormalScale;
        [SerializeField] internal bool _showSamples;
        [SerializeField] internal Color _sampleColor;
        [SerializeField] internal bool _showHits;
        [SerializeField] internal float _hitRadius;
        [SerializeField] internal Color _hitColor;
        [SerializeField] internal bool _showCoverage;
        [SerializeField] internal float _coverageRadius;
        [SerializeField] internal float _originNormalOffset;
        [SerializeField] internal int _uvChannel;
        [SerializeField] internal bool _upgradeMaterials;
        [SerializeField] internal int _samplesIndex;
        [SerializeField] internal string _materialPropertyName = "_VertexBentNormalAo";
        [SerializeField] internal string _shaderKeyword = "_VERTEX_AO";
        [Tooltip("The shader is used when 'Upgrade materials' is true. It's expected to support the ambient occlusion metadata.")]

        // UXML note:
        // The type attribute for ObjectField in UXML can be specified with the "<Namespace>.<Class>, <Namespace>.<Module>" syntax
        // Lets use the Shader class as an example type...
        // In Visual Studio: Use the Immediate Window with debugger attched and evalue this: typeof(Shader).AssemblyQualifiedName
        // Take the first two items....
        // Result in UXML: <ObjectField type="UnityEngine.Shader, UnityEngine.CoreModule">
        // -or-
        // Look up docs online for the class, let's use Material for example. https://docs.unity3d.com/ScriptReference/Material.html
        // You'll see at the top...
        // "Material class in UnityEngine / inherits from:Object / Implemented in:UnityEngine.CoreModule..."
        // Which becomes <ObjectField type="UnityEngine.Material, UnityEngine.CoreModule">
        [SerializeField] internal Shader _ambientOcclusionShader;

        private const string AmbientOcclusionSettingsPath = "Assets/Editor/AmbientOcclusionSettings.asset";

        private void Reset()
        {
            _samplesPerVertex = 100;
            _maxSampleDistance = 1;
            _smoothNormals = true;
            _referenceVertexIndex = 0;
            _showOrigin = true;
            _originColor = Color.cyan;
            _originRadius = .03f;
            _showNormal = true;
            _normalColor = Color.cyan;
            _normalScale = 1;
            _meshNormals = false;
            _meshSmoothNormals = false;
            _showBentNormal = false;
            _vertexID = false;
            _bentNormalColor = Color.magenta;
            _bentNormalScale = 1;
            _showSamples = false;
            _sampleColor = Color.yellow;
            _showHits = false;
            _hitRadius = .03f;
            _hitColor = Color.green;
            _showCoverage = false;
            _coverageRadius = .03f;
            _originNormalOffset = .0001f;
            _uvChannel = 4;
            _upgradeMaterials = true;
            _samplesIndex = 2;
            _materialPropertyName = "_VertexBentNormalAo";
            _shaderKeyword = "_VERTEX_AO";
            _ambientOcclusionShader = StandardShaderUtility.GraphicsToolsStandardShader;
        }

        private void OnValidate()
        {
            _samplesPerVertex = Mathf.Clamp(_samplesPerVertex, 1, 10000);
        }

        /// <summary>
        /// Handles getting the settings from a .asset file or creating a new one.
        /// </summary>
        /// <returns>Returns the current settings for the Ambient Occlusion Tool</returns>
        internal static AmbientOcclusionSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AmbientOcclusionSettings>(AmbientOcclusionSettingsPath);
            if (settings == null)
            {
                settings = CreateInstance<AmbientOcclusionSettings>();
                if (!AssetDatabase.IsValidFolder("Assets/Editor/"))
                {
                    AssetDatabase.CreateFolder("Assets", "Editor");
                    AssetDatabase.Refresh();
                }
                AssetDatabase.CreateAsset(settings, AmbientOcclusionSettingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        /// <summary>
        /// Returns a Visual Element used to draw the Settings UI.
        /// </summary>
        /// <returns></returns>
        internal static VisualElement SettingsUI()
        {
            VisualElement result = null;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.microsoft.mrtk.graphicstools.unity/Editor/Experimental/AmbientOcclusion/AmbientOcclusion.uxml");
            if (visualTree)
            {
                result = visualTree.Instantiate();
            }
            return result;
        }
    }
}

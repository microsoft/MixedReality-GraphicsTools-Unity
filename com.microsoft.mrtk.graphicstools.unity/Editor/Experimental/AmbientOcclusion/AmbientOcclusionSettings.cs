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
        [SerializeField] internal int _referenceVertexIndex;
        [SerializeField] internal bool _showOrigin;
        [SerializeField] internal Color _originColor;
        [SerializeField] internal float _originRadius;
        [SerializeField] internal bool _showNormal;
        [SerializeField] internal Color _normalColor;
        [SerializeField] internal float _normalScale;
        [SerializeField] internal bool _showBentNormal;
        [SerializeField] internal Color _bentNormalColor;
        [SerializeField] internal float _bentNormalScale;
        [SerializeField] internal bool _showSamples;
        [SerializeField] internal Color _sampleColor;
        [SerializeField] internal bool _showHits;
        [SerializeField] internal float _hitRadius;
        [SerializeField] internal bool _showCoverage;
        [SerializeField] internal float _coverageRadius;
        [SerializeField] internal float _originNormalOffset;
        [SerializeField] internal int _uvChannel;
        [SerializeField] internal bool _upgradeMaterials;
        [SerializeField] internal int _samplesIndex;
        [SerializeField] internal string _materialPropertyName = "_VertexBentNormalAo";
        [SerializeField] internal string _shaderPropertyKeyword = "_VERTEX_BENTNORMALAO";
        [Tooltip("The shader is used when 'Upgrade materials' is true. It's expected to support the ambient occlusion metadata.")]

        // UXML note:
        // To find the appropriate string for an <ObjectField>'s type attribute...
        // Lets use a Shader as an example type...
        // Evaluate C#: typeof(Shader).AssemblyQualifiedName
        // Take the first two items....
        // Result in UXML: <ObjectField type="UnityEngine.Shader, UnityEngine.CoreModule">
        [SerializeField] internal Shader _standardShader;

        private const string AmbientOcclusionSettingsPath = "Assets/Editor/AmbientOcclusionSettings.asset";

        private void Reset()
        {
            _samplesPerVertex = 100;
            _maxSampleDistance = 1;
            _referenceVertexIndex = 0;
            _showOrigin = true;
            _originColor = Color.cyan;
            _originRadius = .03f;
            _showNormal = true;
            _normalColor = Color.cyan;
            _normalScale = 1;
            _showBentNormal = true;
            _bentNormalColor = Color.magenta;
            _bentNormalScale = 1;
            _showSamples = false;
            _sampleColor = Color.yellow;
            _showHits = false;
            _hitRadius = .03f;
            _showCoverage = false;
            _coverageRadius = .03f;
            _originNormalOffset = .0001f;
            _uvChannel = 5;
            _upgradeMaterials = false;
            _samplesIndex = 2;
            _materialPropertyName = "_VertexBentNormalAo";
            _shaderPropertyKeyword = "_VERTEX_BENTNORMALAO";
            _standardShader = Shader.Find("Graphics Tools/Standard");
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
        /// Returns a Visual Element to draw the Settings UI.
        /// This ensures that the settings are drawn consistently everywhere.
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

        private void OnMouseDown(MouseDownEvent evt)
        {
            // XXX this needed?
            throw new System.NotImplementedException();
        }
    }
}

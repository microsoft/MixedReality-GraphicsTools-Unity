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
        [SerializeField] private int _samplesPerVertex;
        [SerializeField] private float _maxSampleDistance;
        [SerializeField] private int _referenceVertexIndex;
        [SerializeField] private bool _showOrigin;
        [SerializeField] private Color _originColor;
        [SerializeField] private float _originRadius;
        [SerializeField] private bool _showNormal;
        [SerializeField] private Color _normalColor;
        [SerializeField] private float _normalScale;
        [SerializeField] private bool _showBentNormal;
        [SerializeField] private Color _bentNormalColor;
        [SerializeField] private float _bentNormalScale;
        [SerializeField] private bool _showSamples;
        [SerializeField] private Color _sampleColor;
        [SerializeField] private bool _showHits;
        [SerializeField] private float _hitRadius;
        [SerializeField] private bool _showCoverage;
        [SerializeField] private float _coverageRadius;
        [SerializeField] private float _originNormalOffset;
        [SerializeField] private int _uvChannel;
        [SerializeField] private bool _upgradeMaterials;
        [SerializeField] private int _samplesIndex;
        [SerializeField] private string _shaderPropertyName = "_VertexBentNormalAo";
        [Tooltip("The shader is used when 'Upgrade materials' is true. It's expected to support the ambient occlusion metadata.")]
        [SerializeField] private Shader _standardShader;

        /// <summary>How far to search for nearby colliders in the scene.</summary>
        public int SamplesPerVertex { get => _samplesPerVertex; }
        public float MaxSampleDistance { get => _maxSampleDistance; }
        /// <summary>The index of vertex to visualize</summary>
        public int ReferenceVertexIndex { get => _referenceVertexIndex; }
        public bool ShowOrigin { get => _showOrigin; }
        public Color OriginColor { get => _originColor; }
        public float OriginRadius { get => _originRadius; }
        public bool ShowNormal { get => _showNormal; }
        public Color NormalColor { get => _normalColor; }
        public float NormalScale { get => _normalScale; }
        public bool ShowBentNormal { get => _showBentNormal; }
        public Color BentNormalColor { get => _bentNormalColor; }
        public float BentNormalScale { get => _bentNormalScale; }
        public bool ShowSamples { get => _showSamples; }
        public Color SampleColor { get => _sampleColor; }
        public bool ShowHits { get => _showHits; }
        public float HitRadius { get => _hitRadius; }
        public bool ShowCoverage { get => _showCoverage; }
        public float CoverageRadius { get => _coverageRadius; }
        public float OriginNormalOffset { get => _originNormalOffset; }
        public int UvChannel { get => _uvChannel; }
        public bool UpgradeMaterials { get => _upgradeMaterials; }
        public int SamplesIndex { get => _samplesIndex; }
        public string ShaderPropertyName { get => _shaderPropertyName; }
        public Shader StandardShader { get => _standardShader; }

        public const string AmbientOcclusionSettingsPath = "Assets/Editor/AmbientOcclusionSettings.asset";

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
            _shaderPropertyName = "_VertexBentNormalAo";
            _standardShader = Shader.Find("Graphics Tools/Standard");
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
        public static VisualElement SettingsUI()
        {
            VisualElement result = null;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.microsoft.mrtk.graphicstools.unity/Editor/Experimental/AmbientOcclusion/AmbientOcclusion.uxml");
            if (visualTree)
            {
                result = visualTree.CloneTree();
            }

            return result;
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            Debug.Log(nameof(OnMouseDown));
        }
    }
}

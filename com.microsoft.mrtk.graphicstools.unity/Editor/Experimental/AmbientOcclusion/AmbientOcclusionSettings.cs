// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{

    /// <summary>
    /// A ScriptableObject that holds and persists the settings for the Ambient Occlusion Tool
    /// </summary>
    public class AmbientOcclusionSettings : ScriptableObject
    {
        SerializedProperty _samplesPerVertex;
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
        [SerializeField] private Color _hitColor;
        [SerializeField] private float _hitRadius;
        [SerializeField] private bool _showCoverage;
        [SerializeField] private float _coverageRadius;
        [SerializeField] private float _originNormalOffset;
        [SerializeField] private int _uvChannel;
        [SerializeField] private bool _upgradeMaterials;
        [SerializeField] private int _samplesIndex;
        [SerializeField] private string _shaderPropertyName = "_VertexBentNormalAo";
        [SerializeField] private string _standardShaderPath = "Graphics Tools/Standard";

        /// <summary>How far to search for nearby colliders in the scene.</summary>
        public int SamplesPerVertex { get => _samplesPerVertex.intValue; }
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
        public Color HitColor { get => _hitColor; }
        public float HitRadius { get => _hitRadius; }
        public bool ShowCoverage { get => _showCoverage; }
        public float CoverageRadius { get => _coverageRadius; }
        public float OriginNormalOffset { get => _originNormalOffset; }
        public int UvChannel { get => _uvChannel; }
        public bool UpgradeMaterials { get => _upgradeMaterials; }
        public int SamplesIndex { get => _samplesIndex; }
        public string ShaderPropertyName { get => _shaderPropertyName; }
        public string StandardShaderPath { get => _standardShaderPath; }

        public const string AmbientOcclusionSettingsPath = "Assets/Editor/AmbientOcclusionSettings.asset";

        private void Reset()
        {
            //_samplesPerVertex = 100;
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
            _hitColor = Color.black;
            _hitRadius = .03f;
            _showCoverage = false;
            _coverageRadius = .03f;
            _originNormalOffset = .0001f;
            _uvChannel = 5;
            _upgradeMaterials = false;
            _samplesIndex = 2;
            _shaderPropertyName = "_VertexBentNormalAo";
            _standardShaderPath = "Graphics Tools/Standard";
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
        /// Integrates the settings into the Unity Editor
        /// Edit/Project Settings/Ambient occlusion tool settings
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        private static SettingsProvider CreateAmbientOcclusionToolSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Ambient occlusion tool settings", SettingsScope.Project)
            {
                activateHandler = (searchContext, rootElement) =>
                {
                    //rootElement.Add(SettingsUI());
                    rootElement.Bind(GetSerializedSettings());
                },
                keywords = new HashSet<string>(new[] { "Ambient", "Occlusion", "Tool" })
            };
            return provider;
        }

        /// <summary>
        /// Returns a Visual Element to draw the Settings UI.
        /// This ensures that the settings are drawn consistently everywhere.
        /// </summary>
        /// <returns></returns>
        public static VisualElement SettingsUI()
        {
            ScrollView scrollView = new ScrollView();
            scrollView.Add(new IntegerField("Samples per vertex") { bindingPath = nameof(_samplesPerVertex) });
            scrollView.Add(new FloatField("Max sample distance") { bindingPath = nameof(_maxSampleDistance) });
            scrollView.Add(new IntegerField("Reference vertex index") { bindingPath = nameof(_referenceVertexIndex) });
            scrollView.Add(new Toggle("Show origin") { bindingPath = nameof(_showOrigin) });
            scrollView.Add(new ColorField("Origin color") { bindingPath = nameof(_originColor) });
            scrollView.Add(new FloatField("Origin radius") { bindingPath = nameof(_originRadius) });
            scrollView.Add(new Toggle("Show normal") { bindingPath = nameof(_showNormal) });
            scrollView.Add(new ColorField("Normal color") { bindingPath = nameof(_normalColor) });
            scrollView.Add(new FloatField("Normal scale") { bindingPath = nameof(_normalScale) });
            scrollView.Add(new Toggle("Show bent normal") { bindingPath = nameof(_showBentNormal) });
            scrollView.Add(new ColorField("Bent normal color") { bindingPath = nameof(_bentNormalColor) });
            scrollView.Add(new FloatField("Bent normal scale") { bindingPath = nameof(_bentNormalScale) });
            scrollView.Add(new Toggle("Show samples") { bindingPath = nameof(_showSamples) });
            scrollView.Add(new ColorField("Sample color") { bindingPath = nameof(_sampleColor) });
            scrollView.Add(new Toggle("Show hits") { bindingPath = nameof(_showHits) });
            scrollView.Add(new ColorField("Hit color") { bindingPath = nameof(_hitColor) });
            scrollView.Add(new FloatField("Hit radius") { bindingPath = nameof(_hitRadius) });
            scrollView.Add(new Toggle("Show coverage") { bindingPath = nameof(_showCoverage) });
            scrollView.Add(new FloatField("Coverage radius") { bindingPath = nameof(_coverageRadius) });
            scrollView.Add(new FloatField("Origin normal offset") { bindingPath = nameof(_originNormalOffset) });
            scrollView.Add(new IntegerField("UV channel") { bindingPath = nameof(_uvChannel) });
            scrollView.Add(new Toggle("Upgrade materials") { bindingPath = nameof(_upgradeMaterials) });
            scrollView.Add(new IntegerField("Samples index") { bindingPath = nameof(_samplesIndex) });
            scrollView.Add(new TextField("Shader property name") { bindingPath = nameof(_shaderPropertyName) });
            scrollView.Add(new TextField("Standard shader path") { bindingPath = nameof(_standardShaderPath) });
            return scrollView;
        }
    }
}

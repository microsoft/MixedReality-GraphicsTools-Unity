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
        [SerializeField] private int _samplesPerVertex;
        [SerializeField] private float _maxSampleDistance;
        [SerializeField] private int _referenceVertexIndex;
        [SerializeField] private bool _showOrigin;
        [SerializeField] private Color _originColor;
        [SerializeField] private float _originRadius;
        [SerializeField] private bool _showNormal;
        [SerializeField] private Color _normalColor;
        [SerializeField] private float _normalScale ;
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
        public Color HitColor { get => _hitColor; }
        public float HitRadius { get => _hitRadius; }
        public bool ShowCoverage { get => _showCoverage; }
        public float CoverageRadius { get => _coverageRadius; }
        public float OriginNormalOffset { get => _originNormalOffset; }
        public int UvChannel { get => _uvChannel; }
        public bool UpgradeMaterials { get => _upgradeMaterials; }
        public int SamplesIndex { get => _samplesIndex; }
        public string ShaderPropertyName { get => _shaderPropertyName; }

        public const string AmbientOcclusionSettingsPath = "Assets/Editor/AmbientOcclusionSettings.asset";

        /// <summary>
        /// Handles getting the settings from a .asset file or creating a new one.
        /// </summary>
        /// <returns>Returns the current settings for the Measure Tool</returns>
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
            _hitColor = Color.black;
            _hitRadius = .03f;
            _showCoverage = false;
            _coverageRadius = .03f;
            _originNormalOffset = .0001f;
            _uvChannel = 5;
            _upgradeMaterials = false;
            _samplesIndex = 2;
            _shaderPropertyName = "_VertexBentNormalAo";
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
        private static SettingsProvider CreateMeasureToolSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Ambient occlusion tool settings", SettingsScope.Project)
            {
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = MeasureToolSettings.GetSerializedSettings();

                    rootElement.Add(SettingsUI());
                    rootElement.Bind(settings);
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
            EnumField modeField = new EnumField("Mode");
            modeField.bindingPath = "mode";
            scrollView.Add(modeField);

            Foldout settingsFoldout = new Foldout();
            settingsFoldout.text = "Settings";
            settingsFoldout.value = false;
            scrollView.Add(settingsFoldout);

            EnumField unitField = new EnumField("Units");
            unitField.name = "unitField";
            unitField.bindingPath = "unit";
            settingsFoldout.Add(unitField);

            EnumField scaleField = new EnumField("Scale");
            scaleField.name = "scaleField";
            scaleField.bindingPath = "scale";
            settingsFoldout.Add(scaleField);

            ColorField textColorField = new ColorField("Text Color");
            textColorField.name = "textColorField";
            textColorField.bindingPath = "textColor";
            settingsFoldout.Add(textColorField);

            Slider textSizeField = new Slider("Text Size", 10f, 28f);
            textSizeField.name = "textSizeField";
            textSizeField.bindingPath = "textSize";
            settingsFoldout.Add(textSizeField);

            ColorField lineColorField = new ColorField("Line Color");
            lineColorField.name = "lineColorField";
            lineColorField.bindingPath = "lineColor";
            settingsFoldout.Add(lineColorField);

            Slider offsetField = new Slider("Lines Offset", 0f, 0.1f);
            offsetField.name = "offsetField";
            offsetField.bindingPath = "offset";
            settingsFoldout.Add(offsetField);

            Slider lineThicknessField = new Slider("Line Thickness", 1f, 12f);
            lineThicknessField.bindingPath = "lineThickness";
            settingsFoldout.Add(lineThicknessField);

            return scrollView;
        }
    }
}

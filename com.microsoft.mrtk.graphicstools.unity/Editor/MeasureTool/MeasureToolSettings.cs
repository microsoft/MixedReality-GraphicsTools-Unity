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
    /// A ScriptableObject that holds and persists the settings for the Measure Tool
    /// </summary>
    public class MeasureToolSettings : ScriptableObject
    {
        public const string MeasureToolSettingsPath = "Assets/Editor/MeasureToolSettings.asset";

        public enum ToolMode
        {
            Auto = 0,
            Rect = 1,
            Collider = 2,
            Renderer = 3,
            BetweenObjects = 4,
        }

        public enum ToolUnits
        {
            Millimeter = 0,
            Centimeter = 1,
            Meter = 2,
        }

        public enum ToolScale
        {
            World = 0,
            Local = 1,
        }

        [SerializeField]
        private ToolMode mode;
        [SerializeField]
        private ToolUnits unit;
        [SerializeField]
        private ToolScale scale;

        [SerializeField]
        private Color lineColor = Color.black;
        [SerializeField]
        private float handlesSize = 0.01f;
        [SerializeField]
        private Color handlesColor = Color.white;
        [SerializeField]
        private float textSize = 16f;
        [SerializeField]
        private Color textColor = Color.yellow;
        [SerializeField]
        private float offset = 0.03f;
        [SerializeField]
        private float lineThickness = 4f;

        public ToolMode Mode { get => mode; }
        public ToolUnits Unit { get => unit; }
        public ToolScale Scale { get => scale; }
        public Color LineColor { get => lineColor; }
        public float HandlesSize { get => handlesSize; }
        public Color HandlesColor { get => handlesColor; }
        public float TextSize { get => textSize; }
        public Color TextColor { get => textColor; }
        public float Offset { get => offset; }
        public float LineThickness { get => lineThickness; set => lineThickness = value; }

        /// <summary>
        /// Handles getting the settings from a .asset file or creating a new one.
        /// </summary>
        /// <returns>Returns the current settings for the Measure Tool</returns>
        internal static MeasureToolSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<MeasureToolSettings>(MeasureToolSettingsPath);
            if (settings == null)
            {
                settings = CreateInstance<MeasureToolSettings>();
                if (!AssetDatabase.IsValidFolder("Assets/Editor"))
                {
                    AssetDatabase.CreateFolder("Assets", "Editor");
                    AssetDatabase.Refresh();
                }
                settings.mode = ToolMode.Auto;
                settings.unit = ToolUnits.Millimeter;
                settings.scale = ToolScale.World;

                settings.lineColor = Color.black;
                settings.handlesSize = 0.01f;
                settings.handlesColor = Color.white;
                settings.textSize = 16f;
                settings.textColor = Color.yellow;
                settings.offset = 0.03f;
                settings.lineThickness = 4f;

                AssetDatabase.CreateAsset(settings, MeasureToolSettingsPath);
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
        /// Edit/Project Settings/Measure Tool Settings
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        private static SettingsProvider CreateMeasureToolSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Measure Tool Settings", SettingsScope.Project)
            {
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = MeasureToolSettings.GetSerializedSettings();

                    rootElement.Add(SettingsUI());
                    rootElement.Bind(settings);
                },
                keywords = new HashSet<string>(new[] { "Measure", "Tool" })
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

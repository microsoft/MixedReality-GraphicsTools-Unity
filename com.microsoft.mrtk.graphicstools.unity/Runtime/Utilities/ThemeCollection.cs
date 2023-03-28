// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// A scriptable object which contains data and UI functionality to swap object references within a scene.
    /// </summary>
    [CreateAssetMenu(fileName = "Theme", menuName = "Graphics Tools/Theme Collection")]
    public class ThemeCollection : ScriptableObject
    {
        [System.Serializable]
        public class Theme
        {
            /// <summary>
            /// The name of the theme (for the user only).
            /// </summary>
            public string Name;

            /// <summary>
            /// The assets that represent the theme.
            /// </summary>
            public List<Object> Assets;
        }

        /// <summary>
        /// The list of themes within the collection. Must contain at least 2 items.
        /// </summary>
        public List<Theme> Themes = new List<Theme>();

        /// <summary>
        /// The number of assets in each theme. Must contain at least 1 item.
        /// </summary>
        public int AssetCount = 1;
    }

#if UNITY_EDITOR

    /// <summary>
    /// Custom inspector for the ThemeCollection scriptable object. In the same file for portability.
    /// </summary>
    [CustomEditor(typeof(ThemeCollection))]
    public class ThemeCollectionEditor : UnityEditor.Editor
    {
        // Table
        private SerializedProperty themeCount;
        private SerializedProperty assetCount;
        private Vector2 scrollPosition;

        // Controls.
        private int selectedThemeIndex;

        private enum SelectionMode
        {
            WholeScene,
            SelectionWithChildren,
            SelectionWithoutChildren
        }

        private SelectionMode selectionMode;

        void OnEnable()
        {
            themeCount = serializedObject.FindProperty($"{nameof(ThemeCollection.Themes)}.Array.size");
            assetCount = serializedObject.FindProperty("AssetCount");
        }

        public override void OnInspectorGUI()
        {
            DrawTable();

            ThemeCollection themeCollection = serializedObject.targetObject as ThemeCollection;

            if (themeCollection != null)
            {
                DrawControls(themeCollection);
            }
        }

        private void DrawTable()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(themeCount, new GUIContent("Themes (Minimum of 2)"));

                    int value = themeCount.intValue;

                    if (GUILayout.Button("+"))
                    {
                        ++value;
                    }

                    if (GUILayout.Button("-"))
                    {
                        --value;
                    }

                    themeCount.intValue = Mathf.Max(value, 2);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(assetCount, new GUIContent("Assets (Minimum of 1)"));

                    int value = assetCount.intValue;

                    if (GUILayout.Button("+"))
                    {
                        ++value;
                    }

                    if (GUILayout.Button("-"))
                    {
                        --value;
                    }

                    assetCount.intValue = Mathf.Max(value, 1);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            const float minThemeWidth = 128.0f;
            const float minThemeHeight = 18.0f;
            int themeCountInt = themeCount.intValue;
            int assetCountInt = assetCount.intValue;

            EditorGUILayout.BeginHorizontal("Box");
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height((3 + assetCountInt) * minThemeHeight));
                {
                    EditorGUILayout.BeginHorizontal("Box");
                    {
                        for (int i = 0; i < themeCountInt; ++i)
                        {
                            SerializedProperty themeData = serializedObject.FindProperty($"{nameof(ThemeCollection.Themes)}.Array.data[{i}]");

                            EditorGUILayout.BeginVertical("Box");
                            {
                                SerializedProperty themeDataName = themeData.FindPropertyRelative(nameof(ThemeCollection.Theme.Name));
                                EditorGUILayout.PropertyField(themeDataName, GUIContent.none, false, GUILayout.MinWidth(minThemeWidth));

                                SerializedProperty themeDataAssets = themeData.FindPropertyRelative($"{nameof(ThemeCollection.Theme.Assets)}.Array");

                                SerializedProperty assetsCount = themeDataAssets.FindPropertyRelative("size");
                                assetsCount.intValue = assetCountInt;

                                for (int j = 0; j < assetCountInt; ++j)
                                {
                                    SerializedProperty assetsData = themeDataAssets.FindPropertyRelative($"data[{j}]");

                                    EditorGUILayout.PropertyField(assetsData, GUIContent.none, false, GUILayout.MinWidth(minThemeWidth));
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawControls(ThemeCollection themeCollection)
        {
            EditorGUILayout.BeginVertical("Box");
            {
                List<string> displayedOptions = new List<string>(themeCollection.Themes.Count);

                for (int i = 0; i < themeCollection.Themes.Count; ++i)
                {
                    var theme = themeCollection.Themes[i];
                    displayedOptions.Add(GetThemeName(theme, i));
                }

                selectedThemeIndex = EditorGUILayout.Popup("Selected Theme", selectedThemeIndex, displayedOptions.ToArray());
                selectionMode = (SelectionMode)EditorGUILayout.EnumPopup("Selection Mode", selectionMode);

                string warning;
                GUI.enabled = ValidateThemeCollection(themeCollection, out warning);

                GameObject[] gameObjects = null;

                if (GUI.enabled == true)
                {
                    GUI.enabled = ValidateSelection(selectionMode, out gameObjects, out warning);
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Apply"))
                {
                    Apply(gameObjects, 
                          themeCollection, 
                          themeCollection.Themes[selectedThemeIndex], 
                          selectionMode != SelectionMode.SelectionWithoutChildren);
                }

                if (GUI.enabled == false)
                {
                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                    GUI.enabled = true;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private static bool ValidateThemeCollection(ThemeCollection themeCollection, out string warning)
        {
            // Make sure all themes have valid assets and each row contains similar types.
            for (int i  = 0; i < themeCollection.Themes[0].Assets.Count ; ++i)
            {
                System.Type type = null;

                for (int j = 0; j < themeCollection.Themes.Count; ++j)
                {
                    if (themeCollection.Themes[j].Assets[i] == null)
                    {
                        warning = $"Theme \"{GetThemeName(themeCollection.Themes[j], j)}\" asset index {i} is null.";
                        return false;
                    }

                    System.Type currentType = themeCollection.Themes[j].Assets[i].GetType();

                    if (j == 0)
                    {
                        type = currentType;
                    }
                    else if(currentType != type)
                    {
                        warning = $"Theme \"{GetThemeName(themeCollection.Themes[j], j)}\" asset index {i} is of mismatched type. Expected \"{type}\" and got \"{currentType}\".";
                        return false;
                    }
                }
            }

            warning = string.Empty;
            return true;
        }

        private static bool ValidateSelection(SelectionMode selectionMode, out GameObject[] gameObjects, out string warning)
        {
            switch (selectionMode)
            {
                case SelectionMode.WholeScene:
                    {
                        gameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

                        if (gameObjects.Length == 0)
                        {
                            warning = "The scene is empty, please create at least one game object.";
                            return false;
                        }
                    }
                    break;

                default:
                case SelectionMode.SelectionWithChildren:
                case SelectionMode.SelectionWithoutChildren:
                    {
                        gameObjects = Selection.gameObjects;

                        if (gameObjects.Length == 0)
                        {
                            warning = "Please select at least one game object in the scene. Locking this panel may help in selection.";
                            return false;
                        }
                    }
                    break;
            }

            warning = string.Empty;
            return true;
        }

        private static void Apply(GameObject[] gameObjects, ThemeCollection themeCollection, ThemeCollection.Theme selectedTheme, bool recurse)
        {
            int progress = 0;
            foreach (var gameObject in gameObjects)
            {
                EditorUtility.DisplayProgressBar("Applying Theme Change", "Please wait...", (float)progress / gameObjects.Length);

                if (recurse)
                {
                    SwapObjectReferencesRecurse(gameObject, themeCollection, selectedTheme);
                }
                else
                {
                    SwapObjectReferences(gameObject, themeCollection, selectedTheme);
                }

                ++progress; 
            }

            EditorUtility.ClearProgressBar();
        }

        private static void SwapObjectReferences(GameObject gameObject, ThemeCollection themeCollection, ThemeCollection.Theme selectedTheme)
        {
            var components = gameObject.GetComponents<Component>();

            foreach (var component in components)
            {
                if (component == null)
                {
                    continue;
                }

                SerializedObject serializedObject = new SerializedObject(component);
                SerializedProperty property = serializedObject.GetIterator();
                bool modified = false;

                while (property.NextVisible(true))
                {
                    if (property.propertyType == SerializedPropertyType.ObjectReference &&
                        property.objectReferenceValue != null)
                    {
                        Object currentAsset = property.objectReferenceValue;

                        // Does the current asset match any non-selected theme(s) asset(s)?
                        if (currentAsset != null)
                        {
                            foreach (var theme in themeCollection.Themes)
                            {
                                if (theme == selectedTheme)
                                {
                                    continue;
                                }

                                int assetIndex = 0;

                                foreach (var asset in theme.Assets)
                                {
                                    if (asset == currentAsset)
                                    {
                                        property.objectReferenceValue = selectedTheme.Assets[assetIndex];
                                        modified = true;
                                    }

                                    ++assetIndex;
                                }
                            }
                        }
                    }
                }

                if (modified == true)
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private static void SwapObjectReferencesRecurse(GameObject gameObject, ThemeCollection themeCollection, ThemeCollection.Theme selectedTheme)
        {
            SwapObjectReferences(gameObject, themeCollection, selectedTheme);

            foreach (Transform child in gameObject.transform)
            {
                SwapObjectReferencesRecurse(child.gameObject, themeCollection, selectedTheme);
            }
        }

        private static string GetThemeName(ThemeCollection.Theme theme, int index)
        {
            return string.IsNullOrEmpty(theme.Name) ? $"Unnamed {index}" : theme.Name;
        }
    }

#endif // UNITY_EDITOR
}

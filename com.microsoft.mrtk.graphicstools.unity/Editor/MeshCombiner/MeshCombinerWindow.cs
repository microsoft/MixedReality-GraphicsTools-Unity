// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    public class MeshCombinerWindow : EditorWindow
    {
        private GameObject targetHierarchy = null;
        private bool includeInactive = false;
        private TextureFile.Format textureExtension = TextureFile.Format.TGA;

        private MeshCombineSettingsObject settingsObject = null;
        private SerializedObject settingsSerializedObject = null;

        private Vector2 meshFilterScrollPosition = Vector2.zero;
        private Vector2 textureSettingsScrollPosition = Vector2.zero;

        private const int editorGUIIndentAmmount = 2;

        [MenuItem("Window/Graphics Tools/Mesh Combiner")]
        private static void ShowWindow()
        {
            var window = GetWindow<MeshCombinerWindow>();
            window.titleContent = new GUIContent("Mesh Combiner", EditorGUIUtility.IconContent("d_Particle Effect").image);
            window.minSize = new Vector2(480.0f, 540.0f);
            window.Show();
        }

        private void OnGUI()
        {
            if (settingsObject == null || settingsSerializedObject == null)
            {
                settingsObject = CreateInstance<MeshCombineSettingsObject>();
                settingsSerializedObject = new SerializedObject(settingsObject);
            }

            var settings = settingsObject.Context;

            EditorGUILayout.BeginVertical("Box");
            {
                GUILayout.Label("Import", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("Box");
                {
                    targetHierarchy = (GameObject)EditorGUILayout.ObjectField("Target Hierarchy:", targetHierarchy, typeof(GameObject), true);
                    settings.TargetLOD = EditorGUILayout.IntField("Target LOD", settings.TargetLOD);
                    includeInactive = EditorGUILayout.Toggle("Include Inactive", includeInactive);
                    AutopopulateMeshFilters();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("Box");
                {
                    meshFilterScrollPosition = EditorGUILayout.BeginScrollView(meshFilterScrollPosition);
                    {
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(settingsSerializedObject.FindProperty("Context.MeshFilters"), true);
                        GUI.enabled = true;
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            {
                var combinableMeshCount = settings.MeshFilters.Count;
                var canCombineMeshes = combinableMeshCount >= 2;

                GUI.enabled = canCombineMeshes;

                GUILayout.Label("Export", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("Box");
                {
                    var previousLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 42;
                    settings.BakeMaterialColorIntoVertexColor = EditorGUILayout.Toggle("Bake Material Color Into Vertex Color", settings.BakeMaterialColorIntoVertexColor);
                    settings.BakeMeshIDIntoUVChannel = EditorGUILayout.Toggle("Bake Mesh ID Into UV Channel", settings.BakeMeshIDIntoUVChannel);
                    EditorGUIUtility.labelWidth = previousLabelWidth;

                    if (settings.BakeMeshIDIntoUVChannel)
                    {
                        EditorGUI.indentLevel += editorGUIIndentAmmount;
                        settings.MeshIDUVChannel = (MeshUtility.MeshCombineSettings.UVChannel)EditorGUILayout.EnumPopup("UV Channel", settings.MeshIDUVChannel);
                        EditorGUI.indentLevel -= editorGUIIndentAmmount;
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("Box");
                {
                    textureExtension = (TextureFile.Format)EditorGUILayout.EnumPopup("Texture Extension", textureExtension);

                    textureSettingsScrollPosition = EditorGUILayout.BeginScrollView(textureSettingsScrollPosition);
                    {
                        EditorGUILayout.PropertyField(settingsSerializedObject.FindProperty("Context.TextureSettings"), true);

                        if (settings.TextureSettings.Count > 1)
                        {
                            EditorGUILayout.HelpBox("Merging textures requires adjusting UVs. Make sure UVs are remapped properly when merging multiple texture types by having a unique texture present for each property, or choosing different destination channels.", MessageType.Info);
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical("Box");
                {
                    EditorGUILayout.Space();

                    if (GUILayout.Button("Combine Mesh"))
                    {
                        Save(MeshUtility.CombineModels(settings));
                    }

                    EditorGUILayout.Space();

                    if (!canCombineMeshes)
                    {
                        EditorGUILayout.HelpBox("Please select at least 2 Mesh Filters to combine.", MessageType.Info);
                    }
                    else
                    {
                        GUILayout.Box(string.Format("Combinable Mesh Count: {0}", combinableMeshCount), EditorStyles.helpBox, new GUILayoutOption[0]);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            settingsSerializedObject.ApplyModifiedProperties();
        }

        private void AutopopulateMeshFilters()
        {
            if (targetHierarchy != null)
            {
                var meshFilters = new List<MeshFilter>();
                var newMeshFilters = targetHierarchy.GetComponentsInChildren<MeshFilter>(includeInactive);

                foreach (var meshFilter in newMeshFilters)
                {
                    if (MeshUtility.CanCombine(meshFilter, settingsObject.Context.TargetLOD))
                    {
                        meshFilters.Add(meshFilter);
                    }
                }

                bool isEqual = settingsObject.Context.MeshFilters.Count == meshFilters.Count;

                if (isEqual)
                {
                    foreach (var meshFilter in settingsObject.Context.MeshFilters)
                    {
                        if (!meshFilters.Contains(meshFilter))
                        {
                            isEqual = false;
                            break;
                        }
                    }
                }

                if (isEqual)
                {
                    foreach (var meshFilter in meshFilters)
                    {
                        if (!settingsObject.Context.MeshFilters.Contains(meshFilter))
                        {
                            isEqual = false;
                            break;
                        }
                    }
                }

                settingsObject.Context.pivot = targetHierarchy.transform.worldToLocalMatrix;
                settingsObject.Context.MeshFilters = meshFilters;

                if (!isEqual)
                {
                    settingsSerializedObject = new SerializedObject(settingsObject);
                }
            }
        }

        private static TextureFormat GetUncompressedEquivalent(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.DXT1:
                case TextureFormat.DXT1Crunched:
                    {
                        return TextureFormat.RGB24;
                    }
                case TextureFormat.DXT5:
                case TextureFormat.DXT5Crunched:
                default:
                    {
                        return TextureFormat.RGBA32;
                    }
            }
        }

        private static string AppendToFileName(string source, string appendValue)
        {
            return $"{Path.Combine(Path.GetDirectoryName(source), Path.GetFileNameWithoutExtension(source))}{appendValue}{Path.GetExtension(source)}";
        }

        private void Save(MeshUtility.MeshCombineResult result)
        {
            if (result.Mesh == null)
            {
                return;
            }

            var path = AssetDatabase.GetAssetPath(targetHierarchy);
            path = string.IsNullOrEmpty(path) ? PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(targetHierarchy) : path;
            var directory = string.IsNullOrEmpty(path) ? Application.dataPath : Path.GetDirectoryName(path);
            var filename = string.Format("{0}{1}", string.IsNullOrEmpty(path) ? targetHierarchy.name : Path.GetFileNameWithoutExtension(path), "Combined");

            path = EditorUtility.SaveFilePanelInProject("Save Combined Mesh", filename, "prefab", "Please enter a file name.", directory);

            var watch = System.Diagnostics.Stopwatch.StartNew();

            if (path.Length != 0)
            {
                // Save the mesh.
                AssetDatabase.CreateAsset(result.Mesh, Path.ChangeExtension(path, ".asset"));

                // Save the textures.
                for (var i = 0; i < result.TextureTable.Count; ++i)
                {
                    var pair = result.TextureTable[i];

                    if (pair.Texture != null && !AssetDatabase.Contains(pair.Texture))
                    {
                        var decompressedTexture = new Texture2D(pair.Texture.width, pair.Texture.height, GetUncompressedEquivalent(pair.Texture.format), true);
                        decompressedTexture.SetPixels(pair.Texture.GetPixels());
                        decompressedTexture.Apply();

                        // Don't destroy textures we don't own.
                        if (pair.Texture != Texture2D.whiteTexture)
                        {
                            DestroyImmediate(pair.Texture);
                        }

                        var textureData = TextureFile.Encode(decompressedTexture, textureExtension);

                        DestroyImmediate(decompressedTexture);

                        var texturePath = AppendToFileName(Path.ChangeExtension(path, TextureFile.GetExtension(textureExtension)), pair.Property);
                        File.WriteAllBytes(texturePath, textureData);

                        AssetDatabase.Refresh();

                        result.TextureTable[i] = new MeshUtility.MeshCombineResult.PropertyTexture2DID()
                        {
                            Property = pair.Property,
                            Texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath)
                        };
                    }
                }

                // Save the material.
                foreach (var pair in result.TextureTable)
                {
                    result.Material.SetTexture(pair.Property, pair.Texture);
                }

                AssetDatabase.CreateAsset(result.Material, Path.ChangeExtension(path, ".mat"));

                // Save the result.
                var meshCombineResultObject = CreateInstance<MeshCombineResultObject>();
                meshCombineResultObject.Context = result;
                AssetDatabase.CreateAsset(meshCombineResultObject, AppendToFileName(Path.ChangeExtension(path, ".asset"), "Result"));

                // Save the prefab.
                var prefab = new GameObject(filename);
                prefab.AddComponent<MeshFilter>().sharedMesh = result.Mesh;
                prefab.AddComponent<MeshRenderer>().sharedMaterial = result.Material;
                Selection.activeGameObject = PrefabUtility.SaveAsPrefabAsset(prefab, path);
                DestroyImmediate(prefab);
                AssetDatabase.SaveAssets();

                Debug.LogFormat("Saved combined mesh to: {0}", path);
            }

            Debug.LogFormat("MeshCombinerWindow.Save took {0} ms.", watch.ElapsedMilliseconds);
        }
    }
}

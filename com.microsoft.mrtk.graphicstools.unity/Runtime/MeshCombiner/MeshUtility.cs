// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.GraphicsTools
{
    public static class MeshUtility
    {
        [System.Serializable]
        public class MeshCombineResult
        {
            public UnityEngine.Mesh Mesh = null;
            public Material Material = null;

            [System.Serializable]
            public struct PropertyTexture2DID
            {
                public string Property;
                public Texture2D Texture;
            }

            public List<PropertyTexture2DID> TextureTable = new List<PropertyTexture2DID>();

            [System.Serializable]
            public struct MeshID
            {
                public UnityEngine.Mesh Mesh;
                public int MeshFilterID;
                public int VertexAttributeID;
            }

            public List<MeshID> MeshIDTable = new List<MeshID>();
        }

        [System.Serializable]
        public class MeshCombineSettings
        {
            public Matrix4x4 pivot = Matrix4x4.identity;
            public List<MeshFilter> MeshFilters = new List<MeshFilter>();

            [Min(0)]
            public int TargetLOD = 0;
            public bool BakeMaterialColorIntoVertexColor = false;
            public bool BakeMeshIDIntoUVChannel = false;

            public enum UVChannel
            {
                UV0 = 0,
                UV1 = 1,
                UV2 = 2,
                UV3 = 3,
            }

            public UVChannel MeshIDUVChannel = UVChannel.UV3;

            public enum TextureUsage
            {
                Color = 0,
                Normal = 1
            }

            public static readonly Color[] TextureUsageColorDefault = new Color[]
            {
                new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f),
                new Color(127.0f / 255.0f, 127.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f)
            };

            [System.Serializable]
            public class TextureSetting
            {
                public string TextureProperty = "Name";
                public UVChannel SourceUVChannel = UVChannel.UV0;
                public UVChannel DestUVChannel = UVChannel.UV0;
                public TextureUsage Usage = TextureUsage.Color;
                [Range(2, 4096)]
                public int MaxResolution = 2048;
                [Range(0, 256)]
                public int Padding = 4;
                public bool OverridePaddingColor = false;
                public Color PaddingColorOverride = TextureUsageColorDefault[0];
            }

            public List<TextureSetting> TextureSettings = new List<TextureSetting>()
            {
                new TextureSetting() { TextureProperty = "_MainTex", Usage = TextureUsage.Color, MaxResolution = 2048, Padding = 4 }
            };

            public bool RequiresMaterialData()
            {
                if (BakeMaterialColorIntoVertexColor)
                {
                    return true;
                }

                return TextureSettings.Count != 0;
            }

            public bool AllowsMeshInstancing()
            {
                return !RequiresMaterialData() &&
                       !BakeMeshIDIntoUVChannel;
            }

            private static Texture2D normalTexture = null;

            public static Texture2D GetTextureUsageDefault(TextureUsage usage)
            {
                switch (usage)
                {
                    default:
                    case TextureUsage.Color:
                        {
                            return Texture2D.whiteTexture;
                        }

                    case TextureUsage.Normal:
                        {
                            if (normalTexture == null)
                            {
                                var dimension = 4;
                                normalTexture = new Texture2D(dimension, dimension);
                                var normal = TextureUsageColorDefault[(int)usage];
                                normalTexture.SetPixels(Repeat(new Color(1.0f, normal.g, 1.0f, normal.r), dimension * dimension).ToArray());
                                normalTexture.Apply();
                            }

                            return normalTexture;
                        }
                }
            }
        }

        public static bool CanCombine(MeshFilter meshFilter, int targetLOD)
        {
            if (meshFilter == null)
            {
                return false;
            }

            if (meshFilter.sharedMesh == null)
            {
                return false;
            }

            if (meshFilter.sharedMesh.vertexCount == 0)
            {
                return false;
            }

            var renderer = meshFilter.GetComponent<Renderer>();

            if (renderer is SkinnedMeshRenderer)
            {
                // Don't merge skinned meshes.
                return false;
            }

            // Don't merge meshes from multiple LOD groups.
            if (renderer != null)
            {
                var lodGroups = meshFilter.GetComponentsInParent<LODGroup>();

                foreach (var lodGroup in lodGroups)
                {
                    var lods = lodGroup.GetLODs();

                    for (int i = 0; i < lods.Length; ++i)
                    {
                        if (i == targetLOD)
                        {
                            continue;
                        }

                        // If this renderer is contained in a parent LOD group which is not being merged, ignore it.
                        if (Array.Exists(lods[i].renderers, element => element == renderer))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static MeshCombineResult CombineModels(MeshCombineSettings settings)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var output = new MeshCombineResult();

            var combineInstances = new List<CombineInstance>();
            var meshIDTable = new List<MeshCombineResult.MeshID>();
            var textureToCombineInstanceMappings = new List<Dictionary<Texture2D, List<CombineInstance>>>(settings.TextureSettings.Count);
            Material defaultMaterial = null;

            foreach (var textureSetting in settings.TextureSettings)
            {
                textureToCombineInstanceMappings.Add(new Dictionary<Texture2D, List<CombineInstance>>());
            }

            var vertexCount = GatherCombineData(settings, combineInstances, meshIDTable, textureToCombineInstanceMappings, ref defaultMaterial);

            if (vertexCount != 0)
            {
                output.TextureTable = CombineTextures(settings, textureToCombineInstanceMappings);
                output.Mesh = CombineMeshes(combineInstances, vertexCount);
                output.Material = new Material(defaultMaterial);
                output.MeshIDTable = meshIDTable;
            }
            else
            {
                Debug.LogWarning("The MeshCombiner failed to find any meshes to combine.");
            }

            Debug.LogFormat("MeshCombine took {0} ms on {1} meshes.", watch.ElapsedMilliseconds, settings.MeshFilters.Count);

            return output;
        }

        private static uint GatherCombineData(MeshCombineSettings settings,
                                             List<CombineInstance> combineInstances,
                                             List<MeshCombineResult.MeshID> meshIDTable,
                                             List<Dictionary<Texture2D, List<CombineInstance>>> textureToCombineInstanceMappings, 
                                             ref Material defaultMaterial)
        {
            var meshID = 0;
            var vertexCount = 0U;

            // Create a CombineInstance for each mesh filter and sub mesh.
            foreach (var meshFilter in settings.MeshFilters)
            {
                /// TODO - [Cameron-Micka] assume if submesh 0 is valid other submeshes are valid. Safe assumption?
                if (!CanCombine(meshFilter, settings.TargetLOD))
                {
                    continue;
                }

                for (int i = 0; i < meshFilter.sharedMesh.subMeshCount; ++i)
                {
                    var combineInstance = new CombineInstance();
                    combineInstance.mesh = settings.AllowsMeshInstancing() ? meshFilter.sharedMesh : UnityEngine.Object.Instantiate(meshFilter.sharedMesh) as UnityEngine.Mesh;
                    combineInstance.subMeshIndex = i;

                    if (settings.BakeMeshIDIntoUVChannel)
                    {
                        // Write the MeshID to each a UV channel.
                        ++meshID;
                        combineInstance.mesh.SetUVs((int)settings.MeshIDUVChannel, Repeat(new Vector2(meshID, 0.0f), combineInstance.mesh.vertexCount));
                    }

                    if (settings.RequiresMaterialData())
                    {
                        Material material = null;
                        var renderer = meshFilter.GetComponent<Renderer>();

                        if (renderer != null)
                        {
                            material = renderer.sharedMaterial;
                        }

                        if (material != null)
                        {
                            // The first valid material will become the default material used.
                            if (defaultMaterial == null)
                            {
                                defaultMaterial = material;
                            }

                            if (settings.BakeMaterialColorIntoVertexColor)
                            {
                                // Write the material color to all vertex colors.
                                combineInstance.mesh.colors = Repeat(material.color, combineInstance.mesh.vertexCount).ToArray();
                            }

                            var textureSettingIndex = 0;

                            foreach (var textureSetting in settings.TextureSettings)
                            {
                                // Map textures to CombineInstances
                                var texture = material.GetTexture(textureSetting.TextureProperty) as Texture2D;
                                texture = texture ?? MeshCombineSettings.GetTextureUsageDefault(textureSetting.Usage);

                                if (textureToCombineInstanceMappings[textureSettingIndex].TryGetValue(texture, out List<CombineInstance> combineInstanceMappings))
                                {
                                    combineInstanceMappings.Add(combineInstance);
                                }
                                else
                                {
                                    textureToCombineInstanceMappings[textureSettingIndex][texture] = new List<CombineInstance>(new CombineInstance[] { combineInstance });
                                }

                                ++textureSettingIndex;
                            }
                        }
                    }

                    combineInstance.transform = settings.pivot * meshFilter.gameObject.transform.localToWorldMatrix;
                    vertexCount += (uint)combineInstance.mesh.vertexCount;

                    combineInstances.Add(combineInstance);
                    meshIDTable.Add(new MeshCombineResult.MeshID() { Mesh = meshFilter.sharedMesh, MeshFilterID = meshFilter.GetInstanceID(), VertexAttributeID = meshID });
                }
            }

            return vertexCount;
        }

        private static List<MeshCombineResult.PropertyTexture2DID> CombineTextures(MeshCombineSettings settings,
                                                                                   List<Dictionary<Texture2D, List<CombineInstance>>> textureToCombineInstanceMappings)
        {
            var output = new List<MeshCombineResult.PropertyTexture2DID>();
            var uvsAltered = new bool[4] { false, false, false, false };
            var textureSettingIndex = 0;

            foreach (var textureSetting in settings.TextureSettings)
            {
                var mapping = textureToCombineInstanceMappings[textureSettingIndex];
                var sourceChannel = (int)textureSetting.SourceUVChannel;
                var destChannel = (int)textureSetting.DestUVChannel;

                if (mapping.Count != 0)
                {
                    // Build a texture atlas of the accumulated textures.
                    var textures = new Texture2D[mapping.Keys.Count];
                    mapping.Keys.CopyTo(textures, 0);

                    if (textures.Length > 1)
                    {
                        var atlas = new Texture2D(textureSetting.MaxResolution, textureSetting.MaxResolution);
                        output.Add(new MeshCombineResult.PropertyTexture2DID() { Property = textureSetting.TextureProperty, Texture = atlas });

#if UNITY_EDITOR
                        // Cache the texture's readable state and mark all textures as readable (only works in editor).
                        var readableState = new bool[mapping.Keys.Count];

                        for (int i = 0; i < textures.Length; ++i)
                        {
                            readableState[i] = textures[i].isReadable;
                            SetTextureReadable(textures[i], true);
                        }
#endif
                        // PackTextures requires textures be readable. In editor we set this flag automatically.
                        var rects = atlas.PackTextures(textures, textureSetting.Padding, textureSetting.MaxResolution);

#if UNITY_EDITOR
                        // Reset the texture's readable state (only works in editor).
                        for (int i = 0; i < textures.Length; ++i)
                        {
                            SetTextureReadable(textures[i], readableState[i]);
                        }
#endif

                        PostprocessTexture(atlas, rects, textureSetting);

                        if (!uvsAltered[destChannel])
                        {
                            // Remap the current UVs to their respective rects in the texture atlas.
                            for (var i = 0; i < textures.Length; ++i)
                            {
                                var rect = rects[i];

                                foreach (var combineInstance in mapping[textures[i]])
                                {
                                    var uvs = new List<Vector2>();
                                    combineInstance.mesh.GetUVs(sourceChannel, uvs);
                                    var remappedUvs = new List<Vector2>(uvs.Count);

                                    for (var j = 0; j < uvs.Count; ++j)
                                    {
                                        remappedUvs.Add(new Vector2(Mathf.Lerp(rect.xMin, rect.xMax, uvs[j].x),
                                                                    Mathf.Lerp(rect.yMin, rect.yMax, uvs[j].y)));
                                    }

                                    combineInstance.mesh.SetUVs(destChannel, remappedUvs);
                                }
                            }

                            uvsAltered[destChannel] = true;
                        }
                    }
                    else
                    {
                        output.Add(new MeshCombineResult.PropertyTexture2DID() { Property = textureSetting.TextureProperty, Texture = textures[0] });
                    }
                }

                ++textureSettingIndex;
            }

            return output;
        }

        private static UnityEngine.Mesh CombineMeshes(List<CombineInstance> combineInstances, uint vertexCount)
        {
            var output = new UnityEngine.Mesh();
            output.indexFormat = (vertexCount >= ushort.MaxValue) ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
            output.CombineMeshes(combineInstances.ToArray(), true, true, false);

            return output;
        }

        private static void PostprocessTexture(Texture2D texture, Rect[] usedRects, MeshCombineSettings.TextureSetting settings)
        {
            var pixels = texture.GetPixels();
            var width = texture.width;
            var height = texture.height;

            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    var usedPixel = false;
                    var position = new Vector2((float)x / width, (float)y / height);

                    foreach (var rect in usedRects)
                    {
                        if (rect.Contains(position))
                        {
                            usedPixel = true;
                            break;
                        }
                    }

                    if (usedPixel)
                    {
                        if (settings.Usage == MeshCombineSettings.TextureUsage.Normal)
                        {
                            // Apply Unity's UnpackNormalDXT5nm method to go from DXTnm to RGB.
                            var c = pixels[(y * width) + x];
                            c.r = c.a;
                            Vector2 normal = new Vector2(c.r, c.g);
                            c.b = (Mathf.Sqrt(1.0f - Mathf.Clamp01(Vector2.Dot(normal, normal))) * 0.5f) + 0.5f;
                            pixels[(y * width) + x] = c;
                        }
                    }
                    else
                    {
                        // Unity's PackTextures method defaults to black for areas that do not contain texture data. Because Unity's material 
                        // system defaults to a white texture for color textures (and a 'suitable' normal for normal textures) that do not have texture 
                        // specified, we need to fill in areas of the atlas with appropriate defaults.
                        pixels[(y * width) + x] = settings.OverridePaddingColor ? settings.PaddingColorOverride : MeshCombineSettings.TextureUsageColorDefault[(int)settings.Usage];
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
        }

        private static List<T> Repeat<T>(T value, int count)
        {
            var output = new List<T>(count);

            for (int i = 0; i < count; ++i)
            {
                output.Add(value);
            }

            return output;
        }

        private static void SetTextureReadable(Texture2D texture, bool isReadable)
        {
#if UNITY_EDITOR
            if (texture != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(texture);
                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                if (importer != null)
                {
                    if (importer.isReadable != isReadable)
                    {
                        importer.isReadable = isReadable;

                        AssetDatabase.ImportAsset(assetPath);
                        AssetDatabase.Refresh();
                    }
                }
            }
#endif
        }
    }
}

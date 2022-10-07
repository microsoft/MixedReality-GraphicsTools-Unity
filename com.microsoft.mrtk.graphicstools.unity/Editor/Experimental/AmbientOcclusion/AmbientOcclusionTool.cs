// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Probes the scene to compute coverage above the surface
    /// Metadata is added to the mesh 
    /// Shader references metadata later in pipeline
    /// </summary>
    public class AmbientOcclusionTool
    {
        internal AmbientOcclusionSettings settings;

        public AmbientOcclusionTool(AmbientOcclusionSettings toolSettings)
        {
            settings = toolSettings;
        }

        private Vector3[] _vertexs;
        private Vector3[] _normals;
        private Vector4[] _bentNormalsAo;
        private List<Vector3> _referenceVertexSamples = new List<Vector3>();
        private List<RaycastHit> _referenceVertexHits = new List<RaycastHit>();
        private List<RaycastHit> _hitsInHemisphere = new List<RaycastHit>();
        private MeshFilter _meshFilter;
        private Material _defaultMaterial;
        private const string _magicPrefix = "A.O.";
        private const string _defaultMaterialName = "Default-Material";

        internal void DrawVisualization()
        {
            if (_vertexs == null
                || _vertexs.Length == 0
                || settings._referenceVertexIndex >= _vertexs.Length
                || settings._referenceVertexIndex >= _normals.Length)
            {
                return;
            }

            settings._maxSampleDistance = Handles.RadiusHandle(Quaternion.LookRotation(_normals[settings._referenceVertexIndex], Vector3.up),
                                                               _vertexs[settings._referenceVertexIndex],
                                                               settings._maxSampleDistance);

            if (settings._showOrigin)
            {
                Handles.color = settings._originColor;
                Handles.Label(_vertexs[settings._referenceVertexIndex], $"{settings._referenceVertexIndex}");
                Handles.SphereHandleCap(0, _vertexs[settings._referenceVertexIndex], Quaternion.identity, settings._originRadius, EventType.Repaint);
            }

            if (settings._showNormal)
            {
                Handles.color = settings._normalColor;
                Handles.DrawLine(
                    _vertexs[settings._referenceVertexIndex],
                    _vertexs[settings._referenceVertexIndex] + _normals[settings._referenceVertexIndex] * settings._normalScale);
            }

            if (settings._showBentNormal)
            {
                Handles.color = settings._bentNormalColor;
                var bn = _bentNormalsAo[settings._referenceVertexIndex];
                var bn3 = new Vector3(bn.x, bn.y, bn.z);
                Handles.DrawLine(_vertexs[settings._referenceVertexIndex],
                                 _vertexs[settings._referenceVertexIndex] + bn3 * settings._bentNormalScale);
            }

            if (settings._showSamples)
            {
                for (int i = 0; i < _referenceVertexSamples.Count; i++)
                {
                    Handles.color = settings._sampleColor;
                    Handles.DrawLine(
                        _vertexs[settings._referenceVertexIndex],
                        _vertexs[settings._referenceVertexIndex] + _referenceVertexSamples[i] * settings._maxSampleDistance);
                }
            }

            if (settings._showCoverage)
            {
                for (int i = 0; i < _vertexs.Length; i++)
                {
                    var occlusion = _bentNormalsAo[i].w;
                    Handles.color = new Color(occlusion, occlusion, occlusion, 1);
                    Handles.SphereHandleCap(0, _vertexs[i], Quaternion.identity, settings._coverageRadius * (1 - occlusion), EventType.Repaint);
                }
            }

            if (settings._showHits)
            {
                for (int i = 0; i < _referenceVertexHits.Count; i++)
                {
                    Handles.color = settings._hitColor;
                    Handles.SphereHandleCap(0, _referenceVertexHits[i].point, Quaternion.identity, settings._hitRadius, EventType.Repaint);
                }
            }

            return;
        }

        /// <summary>
        /// Get a random sample direction from a hemisphere who's base is 
        /// perpendicular to the reference normal.
        /// </summary>
        /// <param name="normalizedReferenceNormal">Must be noramlized</param>
        /// <returns></returns>
        private Vector3 RandomSampleAboveHemisphere(Vector3 normalizedReferenceNormal)
        {
            Vector3 randomAxis;
            while (true)
            {
                // This method is appears to be more uniform than Random.onUnitSphere in testing...
                Random.rotationUniform.ToAngleAxis(out _, out randomAxis);
                if (Vector3.Dot(randomAxis, normalizedReferenceNormal) > 0)
                {
                    return randomAxis;
                }
            }
        }

        /// <summary>
        /// Peform the ambient occlusion calculation
        /// </summary>
        internal void GatherSamples(MeshFilter meshFilter)
        {
            var mesh = DeepCopyMesh(meshFilter.sharedMesh);
            mesh.name = $"{_magicPrefix} {meshFilter.gameObject.name}";
            meshFilter.mesh = mesh;

            if (_bentNormalsAo == null || _bentNormalsAo.Length != mesh.vertexCount)
            {
                _bentNormalsAo = new Vector4[mesh.vertexCount];
            }

            var watch = Stopwatch.StartNew();
            RaycastHit hit;

            _vertexs = mesh.vertices;
            _normals = mesh.normals;
            _referenceVertexSamples.Clear();
            _referenceVertexHits.Clear();

            Physics.queriesHitBackfaces = true;

            for (int vi = 0; vi < mesh.vertexCount; vi++)
            {
                // Do the work in world space
                _vertexs[vi] = meshFilter.transform.TransformPoint(_vertexs[vi]);
                _normals[vi] = meshFilter.transform.TransformVector(_normals[vi]);

                _hitsInHemisphere.Clear();

                Vector3 averageDir = Vector3.zero;

                var origin = _vertexs[vi] + _normals[vi].normalized * settings._originNormalOffset;

                for (int ni = 0; ni < settings._samplesPerVertex; ni++)
                {
                    var sampleDirection = RandomSampleAboveHemisphere(_normals[vi]);

                    // Visualization: Save samples for gizmo
                    if (vi == settings._referenceVertexIndex && settings._showSamples)
                    {
                        _referenceVertexSamples.Add(sampleDirection);
                    }

                    if (Physics.Raycast(origin, sampleDirection, out hit, settings._maxSampleDistance))
                    {
                        _hitsInHemisphere.Add(hit);

                        // Visualization: Save hits for gizmo
                        if (vi == settings._referenceVertexIndex)
                        {
                            _referenceVertexHits.Add(hit);
                        }
                    }
                    else
                    {
                        averageDir += sampleDirection;
                    }
                }

                var ao = 1 - ((float)_hitsInHemisphere.Count / settings._samplesPerVertex);
                var avgN = averageDir.normalized;
                _bentNormalsAo[vi] = new Vector4(avgN.x, avgN.y, avgN.z, ao);
            }

            Physics.queriesHitBackfaces = false;

            mesh.SetUVs(settings._uvChannel, _bentNormalsAo);

            Debug.Log($"{nameof(GatherSamples)} vertex-count={mesh.vertexCount} rays-per-vertex={settings._samplesPerVertex} elapsed-ms={watch.ElapsedMilliseconds}");
        }

        private Mesh DeepCopyMesh(Mesh source)
        {
            var result = new Mesh();
            result.name = source.name;
            result.vertices = source.vertices;
            result.normals = source.normals;
            result.uv = source.uv;
            result.uv2 = source.uv2;
            result.uv3 = source.uv3;
            result.uv4 = source.uv4;
            result.uv5 = source.uv5;
            result.uv6 = source.uv6;
            result.uv7 = source.uv7;
            result.uv8 = source.uv8;
            result.triangles = source.triangles;
            result.tangents = source.tangents;
            return result;
        }

        private Material GetOrMakeMaterialAsset(string assetPath, string shaderName = "Standard")
        {
            if (_defaultMaterial != null)
            {
                return _defaultMaterial;
            }
            if (AssetDatabase.FindAssets(assetPath).Length == 0)
            {
                _defaultMaterial = new Material(Shader.Find(shaderName));
                AssetDatabase.CreateAsset(_defaultMaterial, "Assets/" + assetPath);
                AssetDatabase.SaveAssets();
            }
            else
            {
                _defaultMaterial = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            }
            return _defaultMaterial;
        }

        /// <summary>
        /// For the results to be visible to the user
        /// we need to be using the graphics tools standard shader
        /// and the appropriate parameter must be enabled
        /// </summary>
        internal void ModifyMaterials(MeshRenderer meshRenderer)
        {
            // No material? No problem.
            if (meshRenderer.sharedMaterials.Length == 0)
            {
                meshRenderer.sharedMaterial = GetOrMakeMaterialAsset($"{_defaultMaterialName}-AO.mat");
            }
            Material[] materialsCopy = new Material[meshRenderer.sharedMaterials.Length];
            System.Array.Copy(meshRenderer.sharedMaterials, materialsCopy, materialsCopy.Length);
            // Visit all the materials and check on them
            for (int i = 0; i < materialsCopy.Length; i++)
            {
                if (materialsCopy[i].name.Contains(_defaultMaterialName))
                {
                    materialsCopy[i] = GetOrMakeMaterialAsset($"{_defaultMaterialName}-AO.mat");
                }
                if (!StandardShaderUtility.IsUsingGraphicsToolsStandardShader(materialsCopy[i]))
                {
                    StandardShaderGUI.ConvertToGTStandard(materialsCopy[i], materialsCopy[i].shader, settings._ambientOcclusionShader);
                    // Enable keyword feature for shader specified in settings
                    if (!materialsCopy[i].IsKeywordEnabled(settings._shaderKeyword))
                    {
                        materialsCopy[i].EnableKeyword(settings._shaderKeyword);
                    }
                    // Set the material properties to display AO
                    if (materialsCopy[i].HasProperty(settings._materialPropertyName)
                        && materialsCopy[i].GetFloat(settings._materialPropertyName) == 0)
                    {
                        materialsCopy[i].SetFloat(settings._materialPropertyName, 1);
                    }
                }
            }
            meshRenderer.sharedMaterials = materialsCopy;
        }
    }
}

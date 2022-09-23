// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
        private List<GameObject> selectedObjects;

        public AmbientOcclusionTool(AmbientOcclusionSettings toolSettings)
        {
            settings = toolSettings;
            selectedObjects = new List<GameObject>();
        }

        private Vector3[] _vertexs;
        private Vector3[] _normals;
        private Vector4[] _bentNormalsAo;
        private List<Vector3> _referenceVertexSamples = new List<Vector3>();
        private List<RaycastHit> _referenceVertexHits = new List<RaycastHit>();
        private List<RaycastHit> _hitsInHemisphere = new List<RaycastHit>();
        private MeshFilter _meshFilter;

        internal bool DrawVisualization()
        {
            if (_vertexs == null
                || _vertexs.Length == 0
                || settings.ReferenceVertexIndex >= _vertexs.Length
                || settings.ReferenceVertexIndex >= _normals.Length)
            {
                return false;
            }

            Handles.RadiusHandle(new Quaternion(),
                                 _vertexs[settings.ReferenceVertexIndex],
                                 settings.MaxSampleDistance);

            if (settings.ShowOrigin)
            {
                Handles.color = settings.OriginColor;
                Handles.Label(_vertexs[settings.ReferenceVertexIndex], $"{settings.ReferenceVertexIndex}");
                Handles.RadiusHandle(new Quaternion(), _vertexs[settings.ReferenceVertexIndex], settings.OriginRadius);
            }

            if (settings.ShowNormal)
            {
                Handles.color = settings.NormalColor;
                Handles.DrawLine(
                    _vertexs[settings.ReferenceVertexIndex],
                    _vertexs[settings.ReferenceVertexIndex] + _normals[settings.ReferenceVertexIndex] * settings.NormalScale);
            }

            if (settings.ShowBentNormal)
            {
                Handles.color = settings.BentNormalColor;
                var bn = _bentNormalsAo[settings.ReferenceVertexIndex];
                var bn3 = new Vector3(bn.x, bn.y, bn.z);
                Handles.DrawLine(_vertexs[settings.ReferenceVertexIndex],
                                _vertexs[settings.ReferenceVertexIndex] + bn3 * settings.BentNormalScale);
            }

            if (settings.ShowSamples)
            {
                for (int i = 0; i < _referenceVertexSamples.Count; i++)
                {
                    Handles.color = settings.SampleColor;
                    Handles.DrawLine(
                        _vertexs[settings.ReferenceVertexIndex],
                        _vertexs[settings.ReferenceVertexIndex] + _referenceVertexSamples[i] * settings.MaxSampleDistance);
                }
            }

            if (settings.ShowCoverage)
            {
                for (int i = 0; i < _vertexs.Length; i++)
                {
                    Handles.color = new Color(_bentNormalsAo[i].x,
                                             _bentNormalsAo[i].y,
                                             _bentNormalsAo[i].z,
                                             1);
                    var coverage = 1 - _bentNormalsAo[i].w;
                    Handles.color = new Color(coverage, coverage, coverage, 1);
                    Handles.RadiusHandle(new Quaternion(), _vertexs[i], settings.CoverageRadius * coverage);
                }
            }

            if (settings.ShowHits)
            {
                for (int i = 0; i < _referenceVertexHits.Count; i++)
                {
                    Handles.color = new Color(_bentNormalsAo[i].x,
                                             _bentNormalsAo[i].y,
                                             _bentNormalsAo[i].z,
                                             1);
                    Handles.RadiusHandle(new Quaternion(), _referenceVertexHits[i].point, settings.HitRadius);
                }
            }

            return true;
        }

        /// <summary>
        /// Get a random sample direction from a hemisphere who's base is 
        /// perpendicular to the reference normal.
        /// </summary>
        /// <param name="normalizedReferenceNormal">Must be noramlized</param>
        /// <returns></returns>
        private Vector3 RandomSampleAboveHemisphere(Vector3 normalizedReferenceNormal)
        {
            float angle;
            Vector3 randomAxis;
            while (true)
            {
                // This method is appears to be more uniform than Random.onUnitSphere in testing...
                Random.rotationUniform.ToAngleAxis(out angle, out randomAxis);
                if (Vector3.Dot(randomAxis, normalizedReferenceNormal) > 0)
                {
                    return randomAxis;
                }
            }
        }

        /// <summary>
        /// Peform the ambient occlusion calculation
        /// </summary>
        internal void GatherSelectionSamples()
        {
            Selection.gameObjects.ToList().ForEach(i => GatherSamples(i));
        }

        /// <summary>
        /// Peform the ambient occlusion calculation
        /// </summary>
        [ContextMenu(nameof(GatherSamples))]
        public void GatherSamples(GameObject go)
        {
            var meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                UnityEngine.Debug.LogWarning($"No mesh filter found for {go.name}! - skipping.", go);
                return;
            }

            var mesh = DeepCopyMesh(meshFilter.sharedMesh);
            mesh.name = $"A.O. {go.name}";
            meshFilter.mesh = mesh;

            if (_bentNormalsAo == null || _bentNormalsAo.Length != mesh.vertexCount)
            {
                _bentNormalsAo = new Vector4[mesh.vertexCount];
            }

            var watch = Stopwatch.StartNew();

            _vertexs = mesh.vertices;
            _normals = mesh.normals;

            RaycastHit hit;

            _referenceVertexSamples.Clear();
            _referenceVertexHits.Clear();

            for (int vi = 0; vi < mesh.vertexCount; vi++)
            {
                // Do the work in world space
                _vertexs[vi] = go.transform.TransformPoint(_vertexs[vi]);
                _normals[vi] = go.transform.TransformVector(_normals[vi]);

                _hitsInHemisphere.Clear();

                Vector3 averageDir = Vector3.zero;

                var origin = _vertexs[vi] + _normals[vi].normalized * settings.OriginNormalOffset;

                Physics.queriesHitBackfaces = true;

                for (int ni = 0; ni < settings.SamplesPerVertex; ni++)
                {
                    var sampleDirection = RandomSampleAboveHemisphere(_normals[vi]);

                    // Visualization: Save samples for gizmo
                    if (vi == settings.ReferenceVertexIndex && settings.ShowSamples)
                    {
                        _referenceVertexSamples.Add(sampleDirection);
                    }

                    if (Physics.Raycast(origin, sampleDirection, out hit, settings.MaxSampleDistance))
                    {
                        _hitsInHemisphere.Add(hit);

                        // Visualization: Save hits for gizmo
                        if (vi == settings.ReferenceVertexIndex)
                        {
                            _referenceVertexHits.Add(hit);
                        }
                    }
                    else
                    {
                        averageDir += sampleDirection;
                    }
                }

                Physics.queriesHitBackfaces = false;

                var avgN = averageDir.normalized;
                _bentNormalsAo[vi] = new Vector4(avgN.x,
                                                 avgN.y,
                                                 avgN.z,
                                                 1 - ((float)_hitsInHemisphere.Count / settings.SamplesPerVertex));
            }

            mesh.SetUVs(settings.UvChannel, _bentNormalsAo);

            // For the results to be visible to the user
            // we need to be using the graphics tools standard shader
            // and the appropriate parameter must be turned on

            if (go.GetComponent<MeshRenderer>() is MeshRenderer meshRenderer)
            {
                foreach (var material in meshRenderer.sharedMaterials)
                {
                    if (!IsStandardShader(material))
                    {
                        UnityEngine.Debug.LogWarning($"No Graphics Tools Standard material found with 'Vertex ambient occlusion' enabled on {go.name}.");
                        if (settings.UpgradeMaterials)
                        {
                            if (material.name == "Default-Material")
                            {
                                Material newMaterial = new Material(settings.StandardShader);
                                AssetDatabase.CreateAsset(newMaterial, $"Assets/{material.name}-AO.mat");
                                UnityEngine.Debug.Log($"Created new GT Standard material {AssetDatabase.GetAssetPath(newMaterial)}");
                                // Todo: change SetFloat signature to use int version Shader.PropToId...
                                newMaterial.SetFloat(settings.ShaderPropertyName, 1);
                                meshRenderer.material = newMaterial;
                            }
                            else
                            {
                                UnityEngine.Debug.Log($"Upgrading material {material.shader.name} to Graphics Tools Standard.");
                            }
                        }
                    }
                }
            }

            UnityEngine.Debug.Log($"{nameof(GatherSamples)} vertex-count={mesh.vertexCount} rays-per-vertex={settings.SamplesPerVertex} elapsed-ms={watch.ElapsedMilliseconds}");
        }

        private bool IsStandardShader(Material material)
        {
            if (material.HasProperty(settings.ShaderPropertyName))
            {
                if (material.GetFloat(settings.ShaderPropertyName) == 1f)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnSelectionChanged()
        {
            DrawVisualization();
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
    }
}

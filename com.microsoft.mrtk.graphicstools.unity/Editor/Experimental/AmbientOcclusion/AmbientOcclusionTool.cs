// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Radius handles don't work
// Shader doesn't update
// Check we're using bent normals.

using System.Collections.Generic;
using System.Diagnostics;
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
        private const string _magicPrefix = "A.O.";

        internal void DrawVisualization()
        {
            if (_vertexs == null
                || _vertexs.Length == 0
                || settings._referenceVertexIndex >= _vertexs.Length
                || settings._referenceVertexIndex >= _normals.Length)
            {
                return;
            }

            Handles.RadiusHandle(new Quaternion(),
                                 _vertexs[settings._referenceVertexIndex],
                                 settings._maxSampleDistance);

            if (settings._showOrigin)
            {
                Handles.color = settings._originColor;
                Handles.Label(_vertexs[settings._referenceVertexIndex], $"{settings._referenceVertexIndex}");
                Handles.RadiusHandle(new Quaternion(), _vertexs[settings._referenceVertexIndex], settings._originRadius);
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
                    Handles.color = new Color(_bentNormalsAo[i].x,
                                             _bentNormalsAo[i].y,
                                             _bentNormalsAo[i].z,
                                             1);
                    var coverage = 1 - _bentNormalsAo[i].w;
                    Handles.color = new Color(_bentNormalsAo[i].w, _bentNormalsAo[i].w, _bentNormalsAo[i].w, 1);
                    Handles.SphereHandleCap(0, _vertexs[i], Quaternion.identity, settings._coverageRadius * coverage, EventType.Repaint);
                }
            }

            if (settings._showHits)
            {
                for (int i = 0; i < _referenceVertexHits.Count; i++)
                {
                    Handles.color = new Color(_bentNormalsAo[i].x,
                                             _bentNormalsAo[i].y,
                                             _bentNormalsAo[i].z,
                                             1);
                    //Handles.RadiusHandle(new Quaternion(), _referenceVertexHits[i].point, settings._hitRadius);
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
        internal void GatherSamples(GameObject go)
        {
            var meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                UnityEngine.Debug.LogWarning($"No mesh filter found for {go.name}! - skipping.", go);
                return;
            }

            var mesh = DeepCopyMesh(meshFilter.sharedMesh);
            mesh.name = $"{_magicPrefix} {go.name}";
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

            for (int vi = 0; vi < mesh.vertexCount; vi++)
            {
                // Do the work in world space
                _vertexs[vi] = go.transform.TransformPoint(_vertexs[vi]);
                _normals[vi] = go.transform.TransformVector(_normals[vi]);

                _hitsInHemisphere.Clear();

                Vector3 averageDir = Vector3.zero;

                var origin = _vertexs[vi] + _normals[vi].normalized * settings._originNormalOffset;

                Physics.queriesHitBackfaces = true;

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

                Physics.queriesHitBackfaces = false;

                var avgN = averageDir.normalized;
                _bentNormalsAo[vi] = new Vector4(avgN.x,
                                                 avgN.y,
                                                 avgN.z,
                                                 1 - ((float)_hitsInHemisphere.Count / settings._samplesPerVertex));
            }

            mesh.SetUVs(settings._uvChannel, _bentNormalsAo);

            // For the results to be visible to the user
            // we need to be using the graphics tools standard shader
            // and the appropriate parameter must be enabled

            if (go.GetComponent<MeshRenderer>() is MeshRenderer meshRenderer)
            {
                foreach (var material in meshRenderer.sharedMaterials)
                {
                    if (!IsStandardShader(material))
                    {
                        UnityEngine.Debug.LogWarning($"No Graphics Tools Standard material found with 'Vertex ambient occlusion' enabled on {go.name}.");
                        if (settings._upgradeMaterials)
                        {
                            if (material.name == "Default-Material")
                            {
                                Material newMaterial;
                                var name = $"{material.name}-AO";
                                var assetPath = $"Assets/{name}.mat";
                                // See if we made one of these previously
                                var found = AssetDatabase.FindAssets(name);
                                if (found.Length == 0)
                                {
                                    newMaterial = new Material(settings._standardShader);
                                    AssetDatabase.CreateAsset(newMaterial, assetPath);
                                }
                                else
                                {
                                    newMaterial = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                                }
                                // Ensure new material will display the effect
                                newMaterial.SetFloat(settings._materialPropertyName, 1);
                                newMaterial.EnableKeyword(settings._shaderPropertyKeyword);
                                meshRenderer.material = newMaterial;
                                UnityEngine.Debug.Log($"Created new GT Standard material {AssetDatabase.GetAssetPath(newMaterial)}");
                            }
                            else
                            {
                                UnityEngine.Debug.Log($"Upgrading material {material.shader.name} to Graphics Tools Standard.");
                            }
                        }
                    }
                }
            }

            UnityEngine.Debug.Log($"{nameof(GatherSamples)} vertex-count={mesh.vertexCount} rays-per-vertex={settings._samplesPerVertex} elapsed-ms={watch.ElapsedMilliseconds}");
        }

        private bool IsStandardShader(Material material)
        {
            if (material.HasProperty(settings._materialPropertyName))
            {
                if (material.GetFloat(settings._materialPropertyName) == 1f)
                {
                    return true;
                }
            }
            return false;
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
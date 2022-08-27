// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Computes coverage in the hemisphere above a surface mesh
    /// and the information in the mesh for use by shaders
    /// </summary>
    [RequireComponent(typeof(MeshCollider))]
    [ExecuteAlways]
    public class AmbientOcclusion : MonoBehaviour
    {
        [Tooltip("When enabled, this will gather samples as you change parameters in the inspector.")]
        [SerializeField] private bool _updateOnParameterChange = false;

        [Header("Ray tracing")]

        [Tooltip("The number of rays to send into the hemisphere above the surface, per vertex. Bigger numbers will be slower.")]
        [SerializeField, Min(1)] private int SamplesPerVertex = 100;

        [Tooltip("How far to search for nearby colliders in the scene.")]
        [SerializeField] private float MaxSampleDistance = 1;

        [Header("Visualization")]
        [Tooltip("The index of vertex to visualize")]
        [SerializeField, Min(0)] private int ReferenceVertexIndex = 0;

        [SerializeField] private bool _showOrigin = true;
        [SerializeField] private Color _originColor = Color.cyan;
        [SerializeField] private float _originRadius = .03f;

        [SerializeField] private bool _showNormal = true;
        [SerializeField] private Color _normalColor = Color.cyan;
        [SerializeField] private float _normalScale = 1;

        [SerializeField] private bool _showBentNormal = true;
        [SerializeField] private Color _bentNormalColor = Color.magenta;
        [SerializeField] private float _bentNormalScale = 1;

        [SerializeField] private bool _showSamples;
        [SerializeField] private Color _sampleColor = Color.yellow;

        [SerializeField] private bool _showHits;
        [SerializeField] private Color _hitColor = Color.black;
        [SerializeField] private float _hitRadius = .03f;

        [SerializeField] private bool _showCoverage;
        [SerializeField] private float _coverageRadius = .03f;

        private const float kOriginNormalOffset = .0001f;
        private const int kUvChannel = 5;

        private Vector3[] _vertexs;
        private Vector3[] _normals;
        private Vector4[] _bentNormalsAo;
        private List<Vector3> _referenceVertexSamples = new List<Vector3>();
        private List<RaycastHit> _referenceVertexHits = new List<RaycastHit>();
        public Mesh _originalMesh;
        public Mesh _modifiedMesh;
        private List<RaycastHit> _hitsInHemisphere = new List<RaycastHit>();
        private bool _hasSavedOriginalMesh = false;

        private MeshFilter _meshFilter;
        private MeshFilter MyMeshFilter
        {
            get
            {
                if (_meshFilter == null)
                {
                    if (GetComponent<MeshFilter>() is MeshFilter mf)
                    {
                        _meshFilter = mf;
                    }
                }

                if (_meshFilter == null)
                {
                    UnityEngine.Debug.LogWarning($"{nameof(AmbientOcclusion)} requires a MeshFilter, disabling.");
                    enabled = false;
                }
                return _meshFilter;
            }
            set { _meshFilter = value; }
        }

        private void OnEnable()
        {
            UnityEngine.Debug.Log("OnEnable");
            if (_hasSavedOriginalMesh)
            {
                _meshFilter.sharedMesh = _modifiedMesh;
            }
        }

        private void OnDisable()
        {
            if (_hasSavedOriginalMesh)
            {
                _modifiedMesh = _meshFilter.sharedMesh;
                _meshFilter.sharedMesh = _originalMesh;
            }
        }

        private void Start()
        {
            if (GetComponent<MeshFilter>() is MeshFilter mf)
            {
                _meshFilter = mf;
            }
            else
            {
                UnityEngine.Debug.LogWarning($"{nameof(AmbientOcclusion)} requires a MeshFilter, disabling.");
                enabled = false;
                return;
            }
            _originalMesh = DeepCopyMesh(_meshFilter.sharedMesh);
            _hasSavedOriginalMesh = true;
        }

        private void OnValidate()
        {
            if (_updateOnParameterChange)
            {
                GatherSamples();
            }
        }

        private void OnDestroy()
        {
            UnityEngine.Debug.Log($"OnDestroy");
        }

        void OnDrawGizmosSelected()
        {
            if (_vertexs == null
                || _vertexs.Length == 0
                || !isActiveAndEnabled
                || ReferenceVertexIndex >= _vertexs.Length
                || ReferenceVertexIndex >= _normals.Length)
            {
                return;
            }

            if (_showOrigin)
            {
                Gizmos.color = _originColor;
                Handles.Label(_vertexs[ReferenceVertexIndex], $"{ReferenceVertexIndex}");
                Gizmos.DrawSphere(_vertexs[ReferenceVertexIndex], _originRadius);
            }

            if (_showNormal)
            {
                Gizmos.color = _normalColor;
                Gizmos.DrawLine(
                    _vertexs[ReferenceVertexIndex],
                    _vertexs[ReferenceVertexIndex] + _normals[ReferenceVertexIndex] * _normalScale);
            }

            if (_showBentNormal)
            {
                Gizmos.color = _bentNormalColor;
                var bn = _bentNormalsAo[ReferenceVertexIndex];
                var bn3 = new Vector3(bn.x, bn.y, bn.z);
                Gizmos.DrawLine(_vertexs[ReferenceVertexIndex],
                                _vertexs[ReferenceVertexIndex] + bn3 * _bentNormalScale);
            }

            if (_showSamples)
            {
                for (int i = 0; i < _referenceVertexSamples.Count; i++)
                {
                    Gizmos.color = _sampleColor;
                    Gizmos.DrawLine(
                        _vertexs[ReferenceVertexIndex],
                        _vertexs[ReferenceVertexIndex] + _referenceVertexSamples[i] * MaxSampleDistance);
                }
            }

            if (_showCoverage)
            {
                for (int i = 0; i < _vertexs.Length; i++)
                {
                    Gizmos.color = new Color(_bentNormalsAo[i].x,
                                             _bentNormalsAo[i].y,
                                             _bentNormalsAo[i].z,
                                             1);
                    var coverage = 1 - _bentNormalsAo[i].w;
                    Gizmos.color = new Color(coverage, coverage, coverage, 1);
                    Gizmos.DrawSphere(_vertexs[i], _coverageRadius * coverage);
                }
            }

            if (_showHits)
            {
                for (int i = 0; i < _referenceVertexHits.Count; i++)
                {
                    Gizmos.color = new Color(_bentNormalsAo[i].x,
                                             _bentNormalsAo[i].y,
                                             _bentNormalsAo[i].z,
                                             1);
                    Gizmos.DrawSphere(_referenceVertexHits[i].point, _hitRadius);
                }
            }
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
                UnityEngine.Random.rotationUniform.ToAngleAxis(out angle, out randomAxis);
                if (Vector3.Dot(randomAxis, normalizedReferenceNormal) > 0)
                {
                    return randomAxis;
                }
            }
        }

        /// <summary>
        /// Peform the ambient occlusion calculation
        /// </summary>
        [ContextMenu(nameof(GatherSamples))]
        public void GatherSamples()
        {
            if (!enabled)
            {
                UnityEngine.Debug.LogWarning($"{nameof(AmbientOcclusion)} can't gather samples while component is disabled.");
                return;
            }

            var mesh = _meshFilter.sharedMesh;

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
                _vertexs[vi] = transform.TransformPoint(_vertexs[vi]);
                _normals[vi] = transform.TransformVector(_normals[vi]);

                _hitsInHemisphere.Clear();

                Vector3 averageDir = Vector3.zero;

                var origin = _vertexs[vi] + _normals[vi].normalized * kOriginNormalOffset;

                Physics.queriesHitBackfaces = true;

                for (int ni = 0; ni < SamplesPerVertex; ni++)
                {
                    var sampleDirection = RandomSampleAboveHemisphere(_normals[vi]);

                    // Visualization: Save samples for gizmo
                    if (vi == ReferenceVertexIndex && _showSamples)
                    {
                        _referenceVertexSamples.Add(sampleDirection);
                    }

                    if (Physics.Raycast(origin, sampleDirection, out hit, MaxSampleDistance))
                    {
                        _hitsInHemisphere.Add(hit);

                        // Visualization: Save hits for gizmo
                        if (vi == ReferenceVertexIndex)
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
                                                 1 - ((float)_hitsInHemisphere.Count / SamplesPerVertex));
            }

            mesh.SetUVs(kUvChannel, _bentNormalsAo);

            UnityEngine.Debug.Log($"{nameof(GatherSamples)} elapsed-ms={watch.ElapsedMilliseconds} vertex-count={mesh.vertexCount} rays-per-vertex={SamplesPerVertex}.");
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

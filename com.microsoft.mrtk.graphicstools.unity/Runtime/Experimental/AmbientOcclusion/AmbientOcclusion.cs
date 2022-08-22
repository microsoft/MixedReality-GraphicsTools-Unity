// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
    public class AmbientOcclusion : MonoBehaviour
    {
        [Tooltip("When enabled, this will gather samples as you change parameters.")]
        [SerializeField] private bool _updateWhenValidating = false;

        [Header("Ray tracing")]

        [Tooltip("The number of rays to send into the hemisphere above the surface, per vertex. Bigger numbers will be slower.")]
        [SerializeField] private int SamplesPerVertex = 100;

        [Tooltip("How far to search for nearby colliders in the scene.")]
        [SerializeField] private float MaxSampleDistance = 1;

        [Header("Visualization")]
        [Tooltip("The index of vertex to visualize")]
        public int ReferenceVertexIndex = 0;

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

        private int _seed = 42;
        private Vector4[] _tempVector4;
        private Vector3[] _vertexes;
        private Vector3[] _normals;
        private Vector4[] _bentNormalsAo;
        private List<Vector3> _referenceVertexSamples = new List<Vector3>();
        private List<RaycastHit> _referenceVertexHits = new List<RaycastHit>();
        private List<Vector4> _originalUv4 = new List<Vector4>();
        private Mesh _sourceMesh = null;
        private bool _originalHasColors = false;
        private Color[] _originalColors = null;
        private MeshFilter _meshFilter;
        private List<RaycastHit> _hitsInHemisphere = new List<RaycastHit>();

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
                return _meshFilter;
            }
            set { _meshFilter = value; }
        }

        private Mesh SourceMesh
        {
            get
            {
#if UNITY_EDITOR
                _sourceMesh = MyMeshFilter.sharedMesh;
#else
                _sourceMesh = MyMeshFilter.mesh;
#endif
                return _sourceMesh;
            }
            set { _sourceMesh = value; }
        }

        private void Setup()
        {
            UnityEngine.Debug.Log("Setup");
        }

        private void Reset()
        {
            UnityEngine.Debug.Log("Reset");
            SaveOriginalColors();
        }

        private void Awake()
        {
            UnityEngine.Debug.Log("Awake");
        }

        private void OnEnable()
        {
            UnityEngine.Debug.Log("OnEnable");
            SaveOriginalColors();
        }

        private void OnDisable()
        {
            UnityEngine.Debug.Log("OnDisable");
        }

        private void Start()
        {
            UnityEngine.Debug.Log("Start");
        }

        private void OnValidate()
        {
            UnityEngine.Debug.Log("OnValidate");

            SamplesPerVertex = Mathf.Max(1, SamplesPerVertex);

            if (_vertexes == null)
            {
                ReferenceVertexIndex = 0;
            }
            else
            {
                ReferenceVertexIndex = Mathf.Clamp(ReferenceVertexIndex, 0, _vertexes.Length);
            }

            if (enabled)
            {
                UnityEngine.Debug.Log("this.enabled");
                RestoreColors();
                if (_updateWhenValidating)
                {
                    GatherSamples();
                }
            }
            else
            {
                UnityEngine.Debug.Log("disabled (!this.enabled)");
                RestoreOriginalColors();
            }
        }

        void OnDrawGizmosSelected()
        {
            if (_vertexes == null
                || _vertexes.Length == 0
                || !isActiveAndEnabled
                || ReferenceVertexIndex >= _vertexes.Length
                || ReferenceVertexIndex >= _normals.Length)
            {
                return;
            }

            if (_showOrigin)
            {
                Gizmos.color = _originColor;
                Handles.Label(_vertexes[ReferenceVertexIndex], $"{ReferenceVertexIndex}");
                Gizmos.DrawSphere(_vertexes[ReferenceVertexIndex], _originRadius);
            }

            if (_showNormal)
            {
                Gizmos.color = _normalColor;
                Gizmos.DrawLine(
                    _vertexes[ReferenceVertexIndex],
                    _vertexes[ReferenceVertexIndex] + _normals[ReferenceVertexIndex] * _normalScale);
            }

            if (_showBentNormal)
            {
                Gizmos.color = _bentNormalColor;
                var bn = _bentNormalsAo[ReferenceVertexIndex];
                var bn3 = new Vector3(bn.x, bn.y, bn.z);
                Gizmos.DrawLine(_vertexes[ReferenceVertexIndex],
                                _vertexes[ReferenceVertexIndex] + bn3 * _bentNormalScale);
            }

            if (_showSamples)
            {
                for (int i = 0; i < _referenceVertexSamples.Count; i++)
                {
                    Gizmos.color = _sampleColor;
                    Gizmos.DrawLine(
                        _vertexes[ReferenceVertexIndex],
                        _vertexes[ReferenceVertexIndex] + _referenceVertexSamples[i] * MaxSampleDistance);
                }
            }

            if (_showCoverage)
            {
                for (int i = 0; i < _vertexes.Length; i++)
                {
                    Gizmos.color = new Color(_bentNormalsAo[i].x,
                                             _bentNormalsAo[i].y,
                                             _bentNormalsAo[i].z,
                                             1);
                    var coverage = 1 - _bentNormalsAo[i].w;
                    Gizmos.color = new Color(coverage, coverage, coverage, 1);
                    Gizmos.DrawSphere(_vertexes[i], _coverageRadius * coverage);
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
        /// Get a random sample direction from a hemisphere who's
        /// base is perpendicular to the reference normal
        /// </summary>
        /// <param name="normalizedReferenceNormal">Must be noramlized</param>
        /// <returns></returns>
        private Vector3 RandomSampleAboveHemisphere(Vector3 normalizedReferenceNormal)
        {
            float angle;
            Vector3 randomAxis;
            while (true)
            {
                UnityEngine.Random.rotationUniform.ToAngleAxis(out angle, out randomAxis);
                if (Vector3.Dot(randomAxis, normalizedReferenceNormal) > 0)
                {
                    return randomAxis;
                }
            }
        }

        /// <summary>
        /// Eats local space mesh.VERTEX and mesh.NORMAL
        /// Converts them to world space...
        /// Looks randomly in that general direction for ray hits
        /// Returns the random direction looked at, in the hemisphere above the origin
        /// and maybe a RayCast object if we collided with something
        /// </summary>
        /// <param name="vertex">Origin assumed mesh local space</param>
        /// <param name="normal">Direction to look assumed mesh local space</param>
        /// <param name="maxdist">How far to look for colliders</param>
        /// <returns></returns>
        private (Vector3, RaycastHit?) SampleHemisphere(Vector3 vertex,
                                                        Vector3 normal,
                                                        float maxdist)
        {
            var dir = RandomSampleAboveHemisphere(normal);
            var origin = vertex + normal * .00001f; // just a wee bit off the surface...
            RaycastHit hit;

            if (Physics.Raycast(origin, dir, out hit, maxdist))
            {
                return (Vector3.zero, hit);
            }
            else
            {
                return (dir, null);
            }
        }

        [ContextMenu(nameof(GatherSamples))]
        public void GatherSamples()
        {
            if (_bentNormalsAo == null || _bentNormalsAo.Length != SourceMesh.vertexCount)
            {
                _bentNormalsAo = new Vector4[SourceMesh.vertexCount];
            }

            var watch = Stopwatch.StartNew();

            UnityEngine.Random.InitState(_seed);

            _vertexes = new Vector3[SourceMesh.vertexCount];
            Array.Copy(SourceMesh.vertices, _vertexes, SourceMesh.vertexCount);

            _normals = new Vector3[SourceMesh.vertexCount];
            Array.Copy(SourceMesh.normals, _normals, SourceMesh.vertexCount);

            RaycastHit hit;

            _referenceVertexSamples.Clear();
            _referenceVertexHits.Clear();

            for (int vi = 0; vi < SourceMesh.vertexCount; vi++)
            {
                // Do the work in world space
                _vertexes[vi] = transform.TransformPoint(_vertexes[vi]);
                _normals[vi] = transform.TransformVector(_normals[vi]);

                _hitsInHemisphere.Clear();

                Vector3 averageDir = Vector3.zero;

                var origin = _vertexes[vi] + _normals[vi].normalized * kOriginNormalOffset;

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
                var avgN = averageDir.normalized;
                _bentNormalsAo[vi] = new Vector4(avgN.x,
                                                 avgN.y,
                                                 avgN.z,
                                                 1 - ((float)_hitsInHemisphere.Count / SamplesPerVertex));
            }

            SourceMesh.SetUVs(kUvChannel, _bentNormalsAo);

            UnityEngine.Debug.Log($"{nameof(GatherSamples)} elapsed-ms={watch.ElapsedMilliseconds} vertex-count={SourceMesh.vertexCount} rays-per-vertex={SamplesPerVertex}.");
        }

        private void SaveOriginalColors()
        {
            UnityEngine.Debug.Log(nameof(SaveOriginalColors));
            if (SourceMesh.colors.Length == SourceMesh.vertexCount)
            {
                _originalHasColors = true;
                _originalColors = new Color[SourceMesh.vertexCount];
                Array.Copy(SourceMesh.colors, _originalColors, SourceMesh.vertexCount);
                SourceMesh.GetUVs(kUvChannel, _originalUv4);
            }
        }

        private void RestoreColors()
        {
            // XXX see if we can reuse old data without re-sample
            UnityEngine.Debug.Log(nameof(RestoreColors));
        }

        public void RestoreOriginalColors()
        {
            if (_originalColors == null)
            {
                SaveOriginalColors();
            }

            var colors = new Color[_originalColors.Length];

            if (_originalHasColors)
            {
                for (int i = 0; i < _originalColors.Length; i++)
                {
                    colors[i] = _originalColors[i];
                }
                SourceMesh.colors = colors;
            }
            else
            {
                SourceMesh.colors = null;
            }
            SourceMesh.SetUVs(kUvChannel, _originalUv4);
        }
    }
}

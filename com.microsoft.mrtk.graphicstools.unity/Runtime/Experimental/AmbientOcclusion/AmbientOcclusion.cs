using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    public enum GatherBatchingMethod
    {
        None,
        RaysPerFrame
    }

    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
    public class AmbientOcclusion : MonoBehaviour
    {
        [SerializeField] private bool _aoReplacesColor = true;

        [Header("Batching")]
        public GatherBatchingMethod BatchingMethod;
        public int RayBatchSize = 1;

        [Header("Ray tracing")]
        public int SamplesPerVertex = 100;
        public float MaxSampleDistance = 1;
        public int Seed = 42;

        [Header("Visualization")]
        [Tooltip("The index of vertex to visualize")]
        public int ReferenceVertexIndex;

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

        private const float OriginNormalOffset = .0001f;

        private Color[] _tempColors;
        private Vector3[] _vertexes;
        private Vector3[] _normals;
        private Vector3[] _bentNormals;
        private float[] _coverages;

        private List<Vector3> _referenceVertexSamples = new List<Vector3>();
        private List<RaycastHit> _referenceVertexHits = new List<RaycastHit>();

        private RaycastHit[] _sampleHits;
        private RaycastHit[] SampleHits
        {
            get
            {
                if (_sampleHits == null || _sampleHits.Length != SourceMesh.vertexCount)
                {
                    _sampleHits = new RaycastHit[SourceMesh.vertexCount];
                }
                return _sampleHits;
            }
            set { _sampleHits = value; }
        }

        private Mesh _sourceMesh = null;
        private bool _originalHasColors = false;
        private Color[] _originalColors;

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
            //GatherSamples();
        }

        private void Reset()
        {
            UnityEngine.Debug.Log("Reset");
            SaveOriginalColors();
            //Setup();
        }

        //private void Awake()
        //{
        //    UnityEngine.Debug.Log("Awake");
        //    Setup();
        //}

        private void OnEnable()
        {
            UnityEngine.Debug.Log("OnEnable");
            SaveOriginalColors();
            //Setup();
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

            RayBatchSize = Mathf.Max(RayBatchSize, 1);

            if (enabled)
            {
                UnityEngine.Debug.Log("this.enabled");
                RestoreColors();
                GatherSamples();
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
                Gizmos.DrawLine(
                    _vertexes[ReferenceVertexIndex],
                    _vertexes[ReferenceVertexIndex] + _bentNormals[ReferenceVertexIndex] * _bentNormalScale);
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
                    Gizmos.color = new Color(_coverages[i], _coverages[i], _coverages[i], 1);
                    Gizmos.DrawSphere(_vertexes[i], _coverageRadius);
                }
            }

            if (_showHits)
            {
                for (int i = 0; i < _referenceVertexHits.Count; i++)
                {
                    Gizmos.color = _hitColor;
                    Gizmos.DrawSphere(_referenceVertexHits[i].point, _hitRadius);
                }
            }
        }

        /// <summary>
        /// Get a random sample direction from the hemisphere around reference normal
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
        private (Vector3, RaycastHit?) SampleScene(Vector3 vertex,
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

        private IEnumerator ExperimentalBatching()
        {
            _referenceVertexHits.Clear();

            var rayBatchCount = 0;
            var watch = Stopwatch.StartNew();

            for (int vi = 0; vi < SourceMesh.vertexCount; vi++)
            {
                Vector3 avgDir = Vector3.zero;
                for (int si = 0; si < SamplesPerVertex; si++)
                {
                    (Vector3 sampleDir, RaycastHit? hit) = SampleScene(transform.TransformPoint(SourceMesh.vertices[vi]),
                                                                       transform.TransformVector(SourceMesh.normals[vi]),
                                                                       MaxSampleDistance);
                    if (hit.HasValue)
                    {
                        SampleHits[si] = hit.Value;

                        // Stash result for visualization
                        if (ReferenceVertexIndex == vi && _showHits)
                        {
                            _referenceVertexHits.Add(hit.Value);
                        }
                    }
                    else
                    {
                        avgDir += sampleDir;
                    }

                    // Stash result for visualization
                    if (ReferenceVertexIndex == vi && _showSamples)
                    {
                        _referenceVertexSamples.Add(sampleDir);
                    }

                    if (rayBatchCount == RayBatchSize - 1)
                    {
                        rayBatchCount = 0;
                        yield return null;
                    }
                    else
                    {
                        rayBatchCount++;
                        continue;
                    }
                }

                _coverages[vi] = (float)SampleHits.Length / SamplesPerVertex;
                _bentNormals[vi] = avgDir.normalized;
            }

            if (BatchingMethod == GatherBatchingMethod.RaysPerFrame)
            {
            }
            UnityEngine.Debug.LogFormat($"Batch elapsed-ms={watch.ElapsedMilliseconds} batch-size={RayBatchSize}");
        }

        [ContextMenu(nameof(GatherSamples))]
        public void GatherSamples()
        {
            if (_bentNormals == null || _bentNormals.Length != SourceMesh.vertexCount)
            {
                _bentNormals = new Vector3[SourceMesh.vertexCount];
            }

            if (_coverages == null || _coverages.Length != SourceMesh.vertexCount)
            {
                _coverages = new float[SourceMesh.vertexCount];
            }

            if (BatchingMethod == GatherBatchingMethod.RaysPerFrame)
            {
#if !UNITY_EDITOR
                StartCoroutine(ExperimentalBatching());
                return;
#else
                UnityEngine.Debug.LogWarning($"{nameof(GatherBatchingMethod.RaysPerFrame)} only available at runtime!");
#endif
            }

            // Do it all in one-shot

            var watch = Stopwatch.StartNew();

            UnityEngine.Random.InitState(Seed);

            _vertexes = new Vector3[SourceMesh.vertexCount];
            Array.Copy(SourceMesh.vertices, _vertexes, SourceMesh.vertexCount);

            _normals = new Vector3[SourceMesh.vertexCount];
            Array.Copy(SourceMesh.normals, _normals, SourceMesh.vertexCount);

            RaycastHit hit;

            _referenceVertexSamples.Clear();
            _referenceVertexHits.Clear();

            var hitsInHemisphere = new List<RaycastHit>(SourceMesh.vertexCount);

            // For each vertex
            for (int vi = 0; vi < SourceMesh.vertexCount; vi++)
            {
                // Do the work in world space
                _vertexes[vi] = transform.TransformPoint(_vertexes[vi]);
                _normals[vi] = transform.TransformVector(_normals[vi]);

                hitsInHemisphere.Clear();

                Vector3 averageDir = Vector3.zero;

                var origin = _vertexes[vi] + _normals[vi].normalized * OriginNormalOffset;

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
                        // Visualization: Save hits for gizmo
                        if (vi == ReferenceVertexIndex)
                        {
                            _referenceVertexHits.Add(hit);
                        }
                        hitsInHemisphere.Add(hit);
                    }
                    else
                    {
                        averageDir += sampleDirection;
                    }
                }
                _bentNormals[vi] = averageDir.normalized;
                _coverages[vi] = (float)hitsInHemisphere.Count / SamplesPerVertex;
            }

            ApplyCoverage();
            UnityEngine.Debug.Log($"{nameof(GatherSamples)} elapsed-ms={watch.ElapsedMilliseconds} vertex-count={SourceMesh.vertexCount} rays-per-vertex={SamplesPerVertex}.");
        }
        public void ApplyCoverage()
        {
            if (_tempColors == null || _tempColors.Length != SourceMesh.vertexCount)
            {
                _tempColors = new Color[SourceMesh.vertexCount];
            }

            if (_aoReplacesColor)
            {
                for (int i = 0; i < SourceMesh.vertexCount; i++)
                {
                    var ao = 1 - _coverages[i];
                    _tempColors[i] = new Color(ao, ao, ao, 1);
                }
            }
            else
            {
                for (int i = 0; i < SourceMesh.vertexCount; i++)
                {
                    var ao = 1 - _coverages[i];
                    _tempColors[i] = new Color(_bentNormals[i].x, _bentNormals[i].y, _bentNormals[i].z, ao);
                }
            }
            SourceMesh.colors = _tempColors;
        }

        private void SaveOriginalColors()
        {
            UnityEngine.Debug.Log(nameof(SaveOriginalColors));
            if (SourceMesh.colors.Length == SourceMesh.vertexCount)
            {
                _originalHasColors = true;
                _originalColors = new Color[SourceMesh.vertexCount];
                Array.Copy(SourceMesh.colors, _originalColors, SourceMesh.vertexCount);
            }
        }

        private void RestoreColors()
        {
            UnityEngine.Debug.Log(nameof(RestoreColors));
        }

        public void RestoreOriginalColors()
        {
            if (_originalHasColors)
            {
                for (int i = 0; i < _originalColors.Length; i++)
                {
                    _tempColors[i] = _originalColors[i];
                }
                SourceMesh.colors = _tempColors;
            }
            else
            {
                SourceMesh.colors = null;
            }
        }
    }
}

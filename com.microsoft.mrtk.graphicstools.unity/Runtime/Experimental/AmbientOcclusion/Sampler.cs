using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Microsoft.MixedReality.GraphicsTools
{
    public class Sampler : MonoBehaviour
    {
        [Header("Ray tracing options")]
        public int RaysCastPerVertex = 100;
        public float MaxSampleDistance = 1;
        public int Seed = 32;

        [Header("Configuration")]
        public bool UseSharedMesh = true;

        [Header("Events")]
        public UnityEvent samplesUpdated;

        [Header("Visualization")]
        public int SelectedVertexId;

        public float[] Coverages;

        [SerializeField] private bool _showNormal;
        [SerializeField] private Color _normalColor = Color.cyan;
        [SerializeField] private float _normalScale = 1;
        [SerializeField] private float _normalOriginRadius = .03f;

        [SerializeField] private bool _showLeastCoverageNormal;
        [SerializeField] private Color _leastCoverageNormalColor = Color.magenta;
        [SerializeField] private float _leastCoverageNormalScale = 1;

        [SerializeField] private bool _showSamples;
        [SerializeField] private Color _sampleColor = Color.yellow;

        [SerializeField] private bool _showHits;
        [SerializeField] private Color _hitColor = Color.black;
        [SerializeField] private float _hitRadius = .03f;

        [SerializeField] private bool _showCoverage;
        [SerializeField] private float _coverageRadius = .03f;

        private const float OriginNormalOffset = .0001f;

        private List<Vector3> _selectedVertexSamples = new List<Vector3>();
        private List<RaycastHit> _selectedVertexHits = new List<RaycastHit>();
        private Vector3[] _vertexes;
        private Vector3[] _normals;
        private Vector3[] _bentNormals;

        private Mesh _sourceMesh;
        public Mesh SourceMesh
        {
            get
            {
                if (GetComponent<MeshFilter>() is MeshFilter mf)
                {
                    if (UseSharedMesh)
                    {
                        _sourceMesh = mf.sharedMesh;
                    }
                    else
                    {
                        _sourceMesh = mf.mesh;
                    }
                }

                return _sourceMesh;
            }
            set { _sourceMesh = value; }
        }

        private void OnEnable()
        {
            // This forces the Inspector to show the active checkbox for this component
            UpdateCoverage();
        }

        private void OnValidate()
        {
            if (_vertexes == null)
            {
                SelectedVertexId = 0;
            }
            else
            {
                SelectedVertexId = Mathf.Clamp(SelectedVertexId, 0, _vertexes.Length);
            }

            UpdateCoverage();
        }

        void OnDrawGizmosSelected()
        {
            // The various tests for being "all good"
            var shouldReturnEarly = false;
            shouldReturnEarly |= _vertexes == null;
            shouldReturnEarly |= _vertexes.Length == 0;
            shouldReturnEarly |= !isActiveAndEnabled;
            shouldReturnEarly |= SelectedVertexId >= _vertexes.Length;
            shouldReturnEarly |= SelectedVertexId >= _normals.Length;

            if (shouldReturnEarly)
            {
                return;
            }

            if (_showNormal)
            {
                Gizmos.color = _normalColor;
                Gizmos.DrawSphere(_vertexes[SelectedVertexId], _normalOriginRadius);
                Gizmos.DrawLine(
                    _vertexes[SelectedVertexId],
                    _vertexes[SelectedVertexId] + _normals[SelectedVertexId] * _normalScale);
            }

            if (_showLeastCoverageNormal)
            {
                Gizmos.color = _leastCoverageNormalColor;
                Gizmos.DrawSphere(_vertexes[SelectedVertexId], _normalOriginRadius);
                Gizmos.DrawLine(
                    _vertexes[SelectedVertexId],
                    _vertexes[SelectedVertexId] + _bentNormals[SelectedVertexId] * _leastCoverageNormalScale);
            }

            if (_showSamples)
            {
                for (int i = 0; i < _selectedVertexSamples.Count; i++)
                {
                    Gizmos.color = _sampleColor;
                    Gizmos.DrawLine(
                        _vertexes[SelectedVertexId],
                        _vertexes[SelectedVertexId] + _selectedVertexSamples[i] * MaxSampleDistance);
                }
            }

            if (_showCoverage)
            {
                for (int i = 0; i < _vertexes.Length; i++)
                {
                    Gizmos.color = new Color(Coverages[i], Coverages[i], Coverages[i], 1);
                    Gizmos.DrawSphere(_vertexes[i], _coverageRadius);
                }
            }

            if (_showHits)
            {
                for (int i = 0; i < _selectedVertexHits.Count; i++)
                {
                    Gizmos.color = _hitColor;
                    Gizmos.DrawSphere(_selectedVertexHits[i].point, _hitRadius);
                }
            }
        }

        private Vector3[] SampleHemisphere(Vector3 normalizedReferenceNormal, int sampleCount)
        {
            var watch = Stopwatch.StartNew();
            var validSamples = new Vector3[sampleCount];
            var validSampleCount = 0;
            float angle;
            Vector3 randomAxis;
            while (validSampleCount < sampleCount)
            {
                Random.rotationUniform.ToAngleAxis(out angle, out randomAxis);
                // Only allow samples that point the same way as our reference normal
                // Critical that the incoming normal is normalized (but not this functions job)
                if (Vector3.Dot(randomAxis, normalizedReferenceNormal) > 0)
                {
                    validSamples[validSampleCount] = randomAxis;
                    ++validSampleCount;
                }
            }
            Assert.AreEqual(sampleCount, validSampleCount);
            UnityEngine.Debug.LogFormat($"{nameof(SampleHemisphere)} took {watch.ElapsedMilliseconds} ms for {sampleCount} samples.");
            return validSamples;
        }

        [ContextMenu(nameof(UpdateCoverage))]
        public void UpdateCoverage()
        {
            Random.InitState(Seed);

            _vertexes = SourceMesh.vertices;
            _normals = SourceMesh.normals;
            _bentNormals = new Vector3[_vertexes.Length];
            Coverages = new float[_vertexes.Length];

            RaycastHit hit;

            if (_showSamples)
            {
                _selectedVertexSamples.Clear();
            }

            _selectedVertexHits.Clear();

            var sampleHits = new List<RaycastHit>(_vertexes.Length);

            // For each vertex
            for (int vi = 0; vi < _vertexes.Length; vi++)
            {
                sampleHits.Clear();

                // Do the work in world space
                _vertexes[vi] = transform.TransformPoint(_vertexes[vi]);
                _normals[vi] = transform.TransformVector(_normals[vi]);

                // Cast a bunch of rays
                var sampleDirections = SampleHemisphere(_normals[vi].normalized, RaysCastPerVertex);

                //var coverage = 0f;

                for (int ni = 0; ni < sampleDirections.Length; ni++)
                {
                    // Save samples for reference vertex visualization
                    if (vi == SelectedVertexId && _showSamples)
                    {
                        _selectedVertexSamples.Add(sampleDirections[ni]);
                    }

                    var origin = _vertexes[vi] + _normals[vi].normalized * OriginNormalOffset;

                    if (Physics.Raycast(origin, sampleDirections[ni], out hit, MaxSampleDistance))
                    {
                        // Stash this specific vertex result for the gizmo
                        if (vi == SelectedVertexId)
                        {
                            _selectedVertexHits.Add(hit);
                        }
                        sampleHits.Add(hit);
                        //coverage += Mathf.Clamp01(Vector3.Dot(_normals[vi], sampleDirections[ni]));
                    }
                }

                // now do it backwards :)
                // captures planes without backplates

                for (int ni = 0; ni < sampleDirections.Length; ni++)
                {                                                                                                                                                                                                                                                                                                                                                                                 

                }

                Coverages[vi] = (float)sampleHits.Count / RaysCastPerVertex;
            }

            samplesUpdated.Invoke();
        }
    }
}

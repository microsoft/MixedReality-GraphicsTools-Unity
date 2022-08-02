using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Microsoft.MixedReality.GraphicsTools
{
    public enum GatherBatchingMethod
    {
        None,
        PerFrame
    }

    public class Sampler : MonoBehaviour
    {
        [Header("Configuration")]
        public bool UseSharedMesh = true;

        [Header("Batching")]
        public GatherBatchingMethod BatchingMethod;
        public int PerFrameCount;

        [Header("Ray tracing")]
        public int RaysPerVertex = 100;
        public float MaxSampleDistance = 1;
        public int Seed = 32;

        [Header("Events")]
        public UnityEvent SamplesUpdated;

        [Header("Visualization")]
        public int ReferenceVertexIndex;

        public float[] Coverages;

        [SerializeField] private bool _showNormal;
        [SerializeField] private Color _normalColor = Color.cyan;
        [SerializeField] private float _normalScale = 1;
        [SerializeField] private float _normalOriginRadius = .03f;

        [SerializeField] private bool _showBentNormal;
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

        private Vector3[] _vertexes;
        private Vector3[] _normals;
        private Vector3[] _bentNormals;
        private List<Vector3> _referenceVertexSamples = new List<Vector3>();
        private List<RaycastHit> _referenceVertexHits = new List<RaycastHit>();
        private Queue<Vector3[]> _directionsToSample = new Queue<Vector3[]>();

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
                ReferenceVertexIndex = 0;
            }
            else
            {
                ReferenceVertexIndex = Mathf.Clamp(ReferenceVertexIndex, 0, _vertexes.Length);
            }

            UpdateCoverage();

            PerFrameCount = Mathf.Clamp(PerFrameCount, 0, RaysPerVertex);
        }

        void OnDrawGizmosSelected()
        {
            // The various tests for being "all good"
            var shouldReturnEarly = false;
            shouldReturnEarly |= _vertexes == null;
            shouldReturnEarly |= _vertexes.Length == 0;
            shouldReturnEarly |= !isActiveAndEnabled;
            shouldReturnEarly |= ReferenceVertexIndex >= _vertexes.Length;
            shouldReturnEarly |= ReferenceVertexIndex >= _normals.Length;

            if (shouldReturnEarly)
            {
                return;
            }

            if (_showNormal)
            {
                Gizmos.color = _normalColor;
                Gizmos.DrawSphere(_vertexes[ReferenceVertexIndex], _normalOriginRadius);
                Gizmos.DrawLine(
                    _vertexes[ReferenceVertexIndex],
                    _vertexes[ReferenceVertexIndex] + _normals[ReferenceVertexIndex] * _normalScale);
            }

            if (_showBentNormal)
            {
                Gizmos.color = _bentNormalColor;
                Gizmos.DrawSphere(_vertexes[ReferenceVertexIndex], _normalOriginRadius);
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
                    Gizmos.color = new Color(Coverages[i], Coverages[i], Coverages[i], 1);
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

        private Vector3 GetHemisphereSample(Vector3 normalizedReferenceNormal, int sampleCount)
        {
            float angle;
            Vector3 randomAxis;
            while (true)
            {
                Random.rotationUniform.ToAngleAxis(out angle, out randomAxis);
                // Only allow samples that point the same way as our reference normal
                // Critical that the incoming normal is normalized (but not this functions job)
                if (Vector3.Dot(randomAxis, normalizedReferenceNormal) > 0)
                {
                    return randomAxis;
                }
            }
        }

        private Vector3[] GetHemisphereSamples(Vector3 normalizedReferenceNormal, int sampleCount)
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
            UnityEngine.Debug.LogFormat($"{nameof(GetHemisphereSamples)} took {watch.ElapsedMilliseconds} ms for {sampleCount} samples.");
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
                _referenceVertexSamples.Clear();
            }

            _referenceVertexHits.Clear();

            var sampleHits = new List<RaycastHit>(_vertexes.Length);

            // For each vertex
            for (int vi = 0; vi < _vertexes.Length; vi++)
            {
                sampleHits.Clear();

                // Do the work in world space
                _vertexes[vi] = transform.TransformPoint(_vertexes[vi]);
                _normals[vi] = transform.TransformVector(_normals[vi]);

                // Cast a bunch of rays
                var sampleDirections = GetHemisphereSamples(_normals[vi].normalized, RaysPerVertex);

                Vector3 averageDir = Vector3.zero;
                //var coverage = 0f;

                for (int ni = 0; ni < RaysPerVertex; ni++)
                {
                    // Save samples for reference vertex visualization
                    if (vi == ReferenceVertexIndex && _showSamples)
                    {
                        _referenceVertexSamples.Add(sampleDirections[ni]);
                    }

                    var origin = _vertexes[vi] + _normals[vi].normalized * OriginNormalOffset;

                    if (Physics.Raycast(origin, sampleDirections[ni], out hit, MaxSampleDistance))
                    {
                        // Stash this specific vertex result for the gizmo
                        if (vi == ReferenceVertexIndex)
                        {
                            _referenceVertexHits.Add(hit);
                        }
                        sampleHits.Add(hit);
                        //coverage += Mathf.Clamp01(Vector3.Dot(_normals[vi], sampleDirections[ni]));
                    }
                    else
                    {
                        averageDir += sampleDirections[ni];
                    }
                }

                _bentNormals[vi] = averageDir.normalized;

                // now do it backwards :)
                // captures planes without backplates

                for (int ni = 0; ni < RaysPerVertex; ni++)
                {                                                                                                                                                                                                                                                                                                                                                                                 

                }

                Coverages[vi] = (float)sampleHits.Count / RaysPerVertex;
            }

            SamplesUpdated.Invoke();
        }
    }
}

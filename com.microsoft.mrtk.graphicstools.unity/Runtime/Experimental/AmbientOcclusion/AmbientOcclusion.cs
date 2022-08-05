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
        [Header("Configuration")]
        public bool UseSharedMesh = false;

        [Header("Batching")]
        public GatherBatchingMethod BatchingMethod;
        public int RayBatchSize = 1;

        [Header("Ray tracing")]
        public int RaysPerVertex = 100;
        public float MaxSampleDistance = 1;
        public int Seed = 32;

        [HideInInspector]
        public float[] Coverages;

        [Header("Visualization")]
        [Tooltip("The index of vertex to visualize")]
        public int ReferenceVertexIndex;

        [SerializeField] private bool _showOrigin;
        [SerializeField] private Color _originColor = Color.cyan;
        [SerializeField] private float _originRadius = .03f;

        [SerializeField] private bool _showNormal;
        [SerializeField] private Color _normalColor = Color.cyan;
        [SerializeField] private float _normalScale = 1;

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

        // TODO job system version
        //private NativeArray<Vector3> _vertexesNA;
        //private JobHandle jobHandle;
        //private JobHandle parallelJobHandle;
        //private JobHandle transformJobHandle;

        private Color[] _colors;
        private Vector3[] _vertexes;
        private Vector3[] _normals;
        private Vector3[] _bentNormals;
        private List<Vector3> _referenceVertexSamples = new List<Vector3>();
        private List<RaycastHit> _referenceVertexHits = new List<RaycastHit>();

        private Mesh _sourceMesh;
        private Mesh _originalMesh;

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

        private void Reset()
        {
            GatherSamples();
        }

        private void Start()
        {
            _originalMesh = SourceMesh;
        }

        private void OnEnable()
        {
            //if (ShouldGatherOnEnable)
            //{
            //    GatherSamples();
            //}
        }

        private void OnValidate()
        {
            StopAllCoroutines();
            if (_vertexes == null)
            {
                ReferenceVertexIndex = 0;
            }
            else
            {
                ReferenceVertexIndex = Mathf.Clamp(ReferenceVertexIndex, 0, _vertexes.Length);
            }

            RayBatchSize = Mathf.Max(RayBatchSize, 1);

            //PerformGather();
            if (!isActiveAndEnabled)
            {
                FloodVertexColor(Color.white);
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

        /// <summary>
        /// Get a random sample direction from the hemisphere around reference normal
        /// </summary>
        /// <param name="normalizedReferenceNormal">Must be noramlized</param>
        /// <returns></returns>
        private Vector3 SampleHemisphere(Vector3 normalizedReferenceNormal)
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
        /// Given a local space mesh VERTEX position
        /// look into the scene in the direction of NORMAL
        /// no futher than MAXDIST
        /// Returns a random direction in the hemisphere above the origin
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
            vertex = transform.TransformPoint(vertex);
            normal = transform.TransformVector(normal);
            var origin = vertex + normal.normalized * .00001f; // just a wee bit off the surface...
            var dir = SampleHemisphere(normal);
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

        private IEnumerator ProcessRays()
        {
            var sampleHits = new List<RaycastHit>(SourceMesh.vertexCount);

            _referenceVertexHits.Clear();

            var rayBatchCount = 0;
            var watch = Stopwatch.StartNew();

            for (int vi = 0; vi < SourceMesh.vertexCount; vi++)
            {
                //UnityEngine.Debug.Log($"vi={vi}");
                sampleHits.Clear();
                Vector3 avgDir = Vector3.zero;
                for (int si = 0; si < RaysPerVertex; si++)
                {
                    //UnityEngine.Debug.Log($"si={si}");

                    (Vector3 sampleDir, RaycastHit? hit) = SampleScene(SourceMesh.vertices[vi],
                                                                       SourceMesh.normals[vi],
                                                                       MaxSampleDistance);
                    if (hit.HasValue)
                    {
                        sampleHits.Add(hit.Value);

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

                Coverages[vi] = (float)sampleHits.Count / RaysPerVertex;
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
            _bentNormals = new Vector3[SourceMesh.vertexCount];
            Coverages = new float[SourceMesh.vertexCount];

            // Only available at runtime
            if (BatchingMethod == GatherBatchingMethod.RaysPerFrame)
            {
#if !UNITY_EDITOR
                StartCoroutine(ProcessRays());
                return;
#else
                UnityEngine.Debug.LogWarning($"{nameof(GatherBatchingMethod.RaysPerFrame)} only available at runtime!");
#endif
            }

            var watch = Stopwatch.StartNew();

            UnityEngine.Random.InitState(Seed);

            _vertexes = SourceMesh.vertices;
            _normals = SourceMesh.normals;

            RaycastHit hit;

            if (_showSamples)
            {
                _referenceVertexSamples.Clear();
            }

            _referenceVertexHits.Clear();

            var sampleHits = new List<RaycastHit>(SourceMesh.vertexCount);

            // For each vertex
            for (int vi = 0; vi < SourceMesh.vertexCount; vi++)
            {
                sampleHits.Clear();

                // Do the work in world space
                _vertexes[vi] = transform.TransformPoint(_vertexes[vi]);
                _normals[vi] = transform.TransformVector(_normals[vi]);

                Vector3 averageDir = Vector3.zero;
                //var coverage = 0f;

                var referenceDir = _normals[vi];

                var origin = _vertexes[vi] + _normals[vi].normalized * OriginNormalOffset;

                for (int ni = 0; ni < RaysPerVertex; ni++)
                {
                    var sampleDirection = SampleHemisphere(referenceDir);

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
                        sampleHits.Add(hit);
                    }
                    else
                    {
                        averageDir += sampleDirection;
                    }
                }

                _bentNormals[vi] = averageDir.normalized;

                Coverages[vi] = (float)sampleHits.Count / RaysPerVertex;
            }

            ApplyCoverage();
            UnityEngine.Debug.Log($"{nameof(GatherSamples)} elapsed-ms={watch.ElapsedMilliseconds} vertex-count={SourceMesh.vertexCount} rays-per-vertex={RaysPerVertex}.");
        }

        public void FloodVertexColor(Color color)
        {
            var colors = new Color[SourceMesh.vertexCount];
            for (int i = 0; i < SourceMesh.vertexCount; i++)
            {
                colors[i] = color;
            }
            SourceMesh.colors = _colors;
        }

        public void ApplyCoverage()
        {
            _colors = new Color[SourceMesh.vertexCount];

            for (int i = 0; i < SourceMesh.vertexCount; i++)
            {
                var ao = 1 - Coverages[i];
                _colors[i] = new Color(1, 1, 1, ao);
            }
            SourceMesh.colors = _colors;
        }
    }
}

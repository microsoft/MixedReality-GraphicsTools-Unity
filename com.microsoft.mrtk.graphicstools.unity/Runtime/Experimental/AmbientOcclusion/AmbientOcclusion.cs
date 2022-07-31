using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Microsoft.MixedReality.GraphicsTools
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
    public class AmbientOcclusion : MonoBehaviour
    {
        [SerializeField] private int _seed = 32;

        [SerializeField] private int _sampleCount = 32;

        [SerializeField] private float _maxDistance = 1;

        [Header("Visualization")]

        [SerializeField] private bool _shouldShowNormals;
        [SerializeField] private bool _shouldShowSamples;
        [SerializeField] private bool _shouldShowHits;
        [SerializeField] private float _normalScale = .1f;

        [SerializeField] private float _sampleSphereRadius = .005f;

        [SerializeField] private int _vertexIndex;

        private Mesh _mesh;
        private Vector3[] _vertexs;
        private Vector3[] _vertexNormals;
        private Vector3[] _sampleDirections;
        private Vector3[] _visSampleDirections;
        private float[] _vertexCoverage;

        private List<RaycastHit> sampleHits = new List<RaycastHit>();
        private List<RaycastHit> debugSampleHits = new List<RaycastHit>();

        private Mesh originalMesh;

        private MeshFilter _filter;

        private MeshFilter Filter
        {
            get
            {
                if (_filter == null)
                {
                    _filter = GetComponent<MeshFilter>();
                }
                return _filter;
            }
            set { _filter = value; }
        }

        private void OnEnable()
        {
            originalMesh = Filter.sharedMesh;
        }

        [ContextMenu(nameof(CalculateAmbientOcclusion))]
        public void CalculateAmbientOcclusion()
        {
            Random.InitState(_seed);

            var watch = Stopwatch.StartNew();
            _vertexs = Filter.mesh.vertices;
            _vertexNormals = Filter.mesh.normals;
            _vertexCoverage = new float[_vertexs.Length];

            if (_shouldShowSamples)
            {
                _visSampleDirections = new Vector3[_sampleCount];
            }

            ChangeLocalToWorldSpace(_vertexs, _vertexNormals); // modifies input!

            RaycastHit hit;

            debugSampleHits.Clear();

            for (int vertexIndex = 0; vertexIndex < _vertexs.Length; vertexIndex++)
            {
                _sampleDirections = HemisphereSamplesForNormal(_vertexNormals[vertexIndex], _sampleCount);

                sampleHits.Clear();

                for (int sampleIndex = 0; sampleIndex < _sampleCount; sampleIndex++)
                {
                    bool needsRecord = true;

                    if (Physics.Raycast(_vertexs[vertexIndex], _sampleDirections[sampleIndex], out hit, _maxDistance))
                    {
                        // Stash this specific vertex result for the gizmo
                        if (vertexIndex == _vertexIndex)
                        {
                            debugSampleHits.Add(hit);
                            needsRecord = false;
                        }
                        sampleHits.Add(hit);
                    }

                    if (_shouldShowSamples && needsRecord)
                    {
                        if (vertexIndex == _vertexIndex)
                        {
                            _visSampleDirections[sampleIndex] = _sampleDirections[sampleIndex];
                        }
                    }    

                    _vertexCoverage[vertexIndex] = (float)sampleHits.Count / _sampleCount;

                    //UnityEngine.Debug.Log($"coverage={coverage[i]}");
                }
            }
            UnityEngine.Debug.LogFormat($"{nameof(CalculateAmbientOcclusion)} took {watch.ElapsedMilliseconds} ms for {_vertexs.Length} vertices.");
        }

        private Vector3[] SphericalSamples(int sampleCount)
        {
            var result = new Vector3[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                result[i] = Random.rotationUniform.eulerAngles;
            }
            return result;
        }

        private void ChangeLocalToWorldSpace(Vector3[] vtxs, Vector3[] normals)
        {
            var localToWorld = transform.localToWorldMatrix;
            for (int i = 0; i < vtxs.Length; i++)
            {
                vtxs[i] = localToWorld.MultiplyPoint3x4(vtxs[i]);
                normals[i] = localToWorld.MultiplyPoint3x4(normals[i]);
            }
        }

        private Vector3[] HemisphereSamplesForNormal(Vector3 normalizedReferenceNormal, int sampleCount)
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

            UnityEngine.Debug.LogFormat($"{nameof(HemisphereSamplesForNormal)} took {watch.ElapsedMilliseconds} ms for {sampleCount} samples.");

            return validSamples;
        }

        void OnDrawGizmos()
        {
            if (_vertexs == null)
            {
                return;
            }

            if (_shouldShowNormals)
            {
                var normal = _vertexNormals[_vertexIndex];
                Gizmos.color = ColorVector(normal);
                Gizmos.DrawLine(_vertexs[_vertexIndex], _vertexs[_vertexIndex] + _vertexNormals[_vertexIndex] * _normalScale);
            }

            // AO spheres
            for (int i = 0; i < _vertexs.Length; i++)
            {
                var occ = 1 - _vertexCoverage[i];
                Gizmos.color = new Color(occ, occ, occ, 1);
                Gizmos.DrawSphere(_vertexs[i], _sampleSphereRadius);
            }

            if (_shouldShowSamples)
            {
                foreach (var sampleDir in _visSampleDirections)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(_vertexs[_vertexIndex], _vertexs[_vertexIndex] + sampleDir);
                }
            }

            if (_shouldShowHits)
            {
                // Show hits from samples
                foreach (var hit in debugSampleHits)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(_vertexs[_vertexIndex], hit.point);
                    Gizmos.DrawSphere(hit.point, _sampleSphereRadius);
                }
            }
        }

        private void OnValidate()
        {
            CalculateAmbientOcclusion();
        }

        private Color ColorVector(Vector3 vector3)
        {
            return new Color(
                vector3.x * .5f + .5f,
                vector3.y * .5f + .5f,
                vector3.z * .5f + .5f);
        }
    }
}

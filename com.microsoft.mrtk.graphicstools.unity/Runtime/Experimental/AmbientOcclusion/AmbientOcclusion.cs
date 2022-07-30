using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.UI;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
    public class AmbientOcclusion : MonoBehaviour
    {
        [SerializeField]
        private int seed = 32;

        [SerializeField]
        private int sampleCount = 32;

        [SerializeField]
        private float MaxDistance;

        [Header("Visualization")]

        [SerializeField]
        private bool shouldShowNormals;

        [SerializeField]
        private float normalScale;

        //[SerializeField]
        //private float sphereRadius = .01f;

        [SerializeField]
        private float sampleSphereRaidus = .005f;


        [SerializeField]
        private int vertexVisualizationIndex;



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private Mesh mesh;
        private Vector3[] vertexs;
        private Vector3[] normals;
        private Vector3[] sampleDirections;
        private float[] coverage;

        private List<RaycastHit> sampleHits = new List<RaycastHit>();
        private List<RaycastHit> debugSampleHits = new List<RaycastHit>();


        [ContextMenu(nameof(Process))]
        public void Process()
        {
            Random.InitState(seed);

            var watch = Stopwatch.StartNew();

            var filter = GetComponent<MeshFilter>();

            vertexs = filter.mesh.vertices;
            normals = filter.mesh.normals;
            coverage = new float[vertexs.Length];

            ChangeLocalToWorldSpace(vertexs, normals); // modifies input!

            RaycastHit hit;

            debugSampleHits.Clear();

            for (int i = 0; i < vertexs.Length; i++)
            {
                sampleDirections = HemisphereSamplesForNormal(normals[i], sampleCount);

                sampleHits.Clear();

                for (int j = 0; j < sampleCount; j++)
                {
                    if (Physics.Raycast(vertexs[i], sampleDirections[i], out hit, MaxDistance))
                    {
                        // Stash this specific vertex result for the gizmo
                        if (i == vertexVisualizationIndex)
                        {
                            debugSampleHits.Add(hit);
                        }
                        sampleHits.Add(hit);
                    }

                    coverage[i] = (float)sampleHits.Count / sampleCount;

                    UnityEngine.Debug.Log($"coverage={coverage}");
                }
            }

            UnityEngine.Debug.LogFormat("Ambient occlusion took {0} ms on {1} vertices.", watch.ElapsedMilliseconds, vertexs.Length);
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
            var samples = new Vector3[sampleCount];
            var i = 0;
            while (i < sampleCount)
            {
                var sampleDirection = Random.rotationUniform.eulerAngles;
                // Only allow samples that point the same way as our reference normal
                // Critical that the incoming normal is normalized (but not this functions job)
                if (Vector3.Dot(sampleDirection, normalizedReferenceNormal) > 0)
                {
                    samples[i] = sampleDirection;
                    ++i;
                }
            }
            return samples;
        }

        void OnDrawGizmos()
        {
            for (int i = 0; i < vertexs.Length; i++)
            {
                if (i == vertexVisualizationIndex)
                {
                    if (shouldShowNormals)
                    {
                        var normal = normals[i];
                        var r = normal.x * .5f + .5f;
                        var g = normal.y * .5f + .5f;
                        var b = normal.z * .5f + .5f;
                        var normalColor = new Color32(254, 235, 134, 255);
                        Gizmos.color = normalColor;
                        Gizmos.DrawLine(vertexs[i], vertexs[i] + normals[i] * normalScale);
                    }

                    //var occ = 1 - coverage[i];
                    //Gizmos.color = new Color(occ, occ, occ, 1);
                    //Gizmos.DrawSphere(vertexs[i], sphereRadius);

                    foreach (var hit in debugSampleHits)
                    {
                        Gizmos.color = new Color(1, 0, 1, 1);
                        Gizmos.DrawLine(vertexs[i], hit.point);
                        Gizmos.DrawSphere(hit.point, sampleSphereRaidus);
                    }
                }
            }
        }

        private void OnValidate()
        {
            Process();
        }
    }
}

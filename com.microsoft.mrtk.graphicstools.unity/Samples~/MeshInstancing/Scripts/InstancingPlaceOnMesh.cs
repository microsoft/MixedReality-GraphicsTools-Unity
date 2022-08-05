// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Samples.MeshInstancing
{
    /// <summary>
    /// TODO
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class InstancingPlaceOnMesh : MonoBehaviour
    {
        [SerializeField]
        private MeshInstancer instancer = null;
        //[SerializeField, Min(0)]
        //private int PlacementMeshSubMeshIndex = 0;
        [SerializeField, Min(1)]
        private int instanceCount = 20000;
        [SerializeField]
        private float instanceScale = 0.05f;

        private bool didStart = false;

        /// <summary>
        /// Re-spawn instances when a property changes.
        /// </summary>
        private void OnValidate()
        {
            if (didStart)
            {
                CreateInstances();
            }
        }

        /// <summary>
        /// Create instances on start.
        /// </summary>
        private void Start()
        {
            CreateInstances();
            didStart = true;
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void CreateInstances()
        {
            // Clear any existing instances.
            instancer = (instancer == null) ? GetComponent<MeshInstancer>() : instancer;
            instancer.Clear();

            // Get the verticies and indicies from the current mesh filter.
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            if (meshFilter == null)
            {
                return;
            }

            UnityEngine.Mesh mesh = meshFilter.sharedMesh;

            if (mesh == null)
            {
                return;
            }

            Vector3[] verticies = mesh.vertices;
            int[] indicies = mesh.triangles;

            // Turn the verticies and indicies into triangles and areas.
            int triangleCount = indicies.Length / 3;
            Vector3[,] triangles = new Vector3[triangleCount, 3];
            float[] areas = new float[triangleCount];
            //Vector3 normals = new float[triangleCount];

            for (int i = 0; i < indicies.Length; i += 3)
            {
                int index = i / 3;
                triangles[index, 0] = verticies[indicies[i + 0]];
                triangles[index, 1] = verticies[indicies[i + 1]];
                triangles[index, 2] = verticies[indicies[i + 2]];

                // Area of a triangle is half of the cross product's magnitude.
                Vector3 a = triangles[index, 1] - triangles[index, 0];
                Vector3 b = triangles[index, 2] - triangles[index, 0];
                areas[index] = Vector3.Cross(a, b).magnitude * 0.5f;
            }

            // Accumulate the area of the mesh.
            float meshArea = 0.0f;

            for (int i = 0; i < areas.Length; ++i)
            {
                meshArea += areas[i];
            }

            // The mesh has no area.
            if (meshArea <= 0.0f)
            {
                return;
            }

            int colorID = Shader.PropertyToID("_Color");

            for (int i = 0; i < instanceCount; ++i)
            {
                Vector3[] triangle = PickRandomTriangle(triangles, areas, meshArea);
                Vector3 position = PickRandomPointOnTriangle(triangle);

                var instance = instancer.Instantiate(position, Random.rotation, Vector3.one * instanceScale);
                instance.SetVector(colorID, Random.ColorHSV());
            }
        }

        private static Vector3[] PickRandomTriangle(Vector3[,] triangles, float[] areas, float meshArea)
        {
            var random = Random.Range(0.0f, meshArea);

            for (int i = 0; i < triangles.Length; ++i)
            {
                if (random < areas[i])
                {
                    return new Vector3[] { triangles[i, 0], triangles[i, 1], triangles[i, 2] };
                }
                random -= areas[i];
            }

            int last = triangles.Length - 1;
            return new Vector3[] { triangles[last, 0], triangles[last, 1], triangles[last, 2] };
        }

        private static Vector3 PickRandomPointOnTriangle(Vector3[] triangle)
        {
            float r1 = Mathf.Sqrt(Random.Range(0.0f, 1.0f));
            float r2 = Random.Range(0.0f, 1.0f);
            float m1 = 1 - r1;
            float m2 = r1 * (1.0f - r2);
            float m3 = r2 * r1;

            Vector3 a = triangle[0];
            Vector3 b = triangle[1];
            Vector3 c = triangle[2];

            return (m1 * a) + (m2 * b) + (m3 * c);
        }
    }
}

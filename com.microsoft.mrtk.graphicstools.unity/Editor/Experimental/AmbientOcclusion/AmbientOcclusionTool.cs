// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        //private Shader _standardShader;
        //private static string _standardShaderPath = "Graphics Tools/Standard";

        //public override void OnToolGUI(EditorWindow window)
        //{

        //}

        //[MenuItem("Window/Graphics Tools/Ambient occclusion")]
        //private static void ShowWindow()
        //{
        //    if (Shader.Find(_standardShaderPath) == null)
        //    {
        //        UnityEngine.Debug.LogError($"Unable to locate {_standardShaderPath}!");
        //    }
        //    var window = GetWindow<AmbientOcclusionToolWindow>();
        //    window.titleContent = new GUIContent("Ambient occlusion");
        //    window.Show();
        //}

        /// <summary>
        /// Custom inspector that exposes additional user controls for the AmbientOcclusion component
        /// </summary>
        //[CustomEditor(typeof(AmbientOcclusion)), CanEditMultipleObjects]
        //private void OnGUI()
        //{
        //    EditorGUILayout.LabelField("Ray tracing");
        //    var _samplesPerVertexLabels = new string[] { "1", "10", "100", "1000", "10000" };
        //    _samplesIndex = EditorGUILayout.Popup("Samples per vertex", _samplesIndex, _samplesPerVertexLabels);
        //    _samplesPerVertex = (int)Mathf.Pow(10, _samplesIndex);

        //    MaxSampleDistance = EditorGUILayout.FloatField("Max sample distance", MaxSampleDistance);

        //    if (GUILayout.Button("Gather selection samples"))
        //    {
        //        GatherSelectionSamples();
        //    }

        //    _showSamples = EditorGUILayout.Toggle("Show samples", _showSamples);
        //}

        private void DrawVisualization()
        {
            if (_vertexs == null
                || _vertexs.Length == 0
                || settings.ReferenceVertexIndex >= _vertexs.Length
                || settings.ReferenceVertexIndex >= _normals.Length)
            {
                return;
            }

            if (settings.ShowOrigin)
            {
                Gizmos.color = settings.OriginColor;
                Handles.Label(_vertexs[settings.ReferenceVertexIndex], $"{settings.ReferenceVertexIndex}");
                Gizmos.DrawSphere(_vertexs[settings.ReferenceVertexIndex], settings.OriginRadius);
            }

            if (settings.ShowNormal)
            {
                Gizmos.color = settings.NormalColor;
                Gizmos.DrawLine(
                    _vertexs[settings.ReferenceVertexIndex],
                    _vertexs[settings.ReferenceVertexIndex] + _normals[settings.ReferenceVertexIndex] * settings.NormalScale);
            }

            if (settings.ShowBentNormal)
            {
                Gizmos.color = settings.BentNormalColor;
                var bn = _bentNormalsAo[settings.ReferenceVertexIndex];
                var bn3 = new Vector3(bn.x, bn.y, bn.z);
                Gizmos.DrawLine(_vertexs[settings.ReferenceVertexIndex],
                                _vertexs[settings.ReferenceVertexIndex] + bn3 * settings.BentNormalScale);
            }

            if (settings.ShowSamples)
            {
                for (int i = 0; i < _referenceVertexSamples.Count; i++)
                {
                    Gizmos.color = settings.SampleColor;
                    Gizmos.DrawLine(
                        _vertexs[settings.ReferenceVertexIndex],
                        _vertexs[settings.ReferenceVertexIndex] + _referenceVertexSamples[i] * settings.MaxSampleDistance);
                }
            }

            if (settings.ShowCoverage)
            {
                for (int i = 0; i < _vertexs.Length; i++)
                {
                    Gizmos.color = new Color(_bentNormalsAo[i].x,
                                             _bentNormalsAo[i].y,
                                             _bentNormalsAo[i].z,
                                             1);
                    var coverage = 1 - _bentNormalsAo[i].w;
                    Gizmos.color = new Color(coverage, coverage, coverage, 1);
                    Gizmos.DrawSphere(_vertexs[i], settings.CoverageRadius * coverage);
                }
            }

            if (settings.ShowHits)
            {
                for (int i = 0; i < _referenceVertexHits.Count; i++)
                {
                    Gizmos.color = new Color(_bentNormalsAo[i].x,
                                             _bentNormalsAo[i].y,
                                             _bentNormalsAo[i].z,
                                             1);
                    Gizmos.DrawSphere(_referenceVertexHits[i].point, settings.HitRadius);
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
        internal void GatherSelectionSamples()
        {
            Selection.gameObjects.ToList().ForEach(i => GatherSamples(i));
        }

        /// <summary>
        /// Peform the ambient occlusion calculation
        /// </summary>
        [ContextMenu(nameof(GatherSamples))]
        public void GatherSamples(GameObject go)
        {
            var meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                UnityEngine.Debug.LogWarning($"No mesh filter found for {go.name}! - skipping.", go);
                return;
            }

            var mesh = DeepCopyMesh(meshFilter.sharedMesh);
            mesh.name = $"A.O. {go.name}";
            meshFilter.mesh = mesh;

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
                _vertexs[vi] = go.transform.TransformPoint(_vertexs[vi]);
                _normals[vi] = go.transform.TransformVector(_normals[vi]);

                _hitsInHemisphere.Clear();

                Vector3 averageDir = Vector3.zero;

                var origin = _vertexs[vi] + _normals[vi].normalized * settings.OriginNormalOffset;

                Physics.queriesHitBackfaces = true;

                for (int ni = 0; ni < settings.SamplesPerVertex; ni++)
                {
                    var sampleDirection = RandomSampleAboveHemisphere(_normals[vi]);

                    // Visualization: Save samples for gizmo
                    if (vi == settings.ReferenceVertexIndex && settings.ShowSamples)
                    {
                        _referenceVertexSamples.Add(sampleDirection);
                    }

                    if (Physics.Raycast(origin, sampleDirection, out hit, settings.MaxSampleDistance))
                    {
                        _hitsInHemisphere.Add(hit);

                        // Visualization: Save hits for gizmo
                        if (vi == settings.ReferenceVertexIndex)
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
                                                 1 - ((float)_hitsInHemisphere.Count / settings.SamplesPerVertex));
            }

            mesh.SetUVs(settings.UvChannel, _bentNormalsAo);

            // For the results to be visible to the user
            // we need to be using the graphics tools standard shader
            // and the appropriate parameter must be turned on

            if (go.GetComponent<MeshRenderer>() is MeshRenderer meshRenderer)
            {
                foreach (var material in meshRenderer.sharedMaterials)
                {
                    if (!IsStandardShader(material))
                    {
                        UnityEngine.Debug.LogWarning($"No Graphics Tools Standard material found with 'Vertex ambient occlusion' enabled on {go.name}.");
                        if (settings.UpgradeMaterials)
                        {
                            if (material.name == "Default-Material")
                            {
                                Material newMaterial = new Material(Shader.Find(settings.StandardShaderPath));
                                AssetDatabase.CreateAsset(newMaterial, $"Assets/{material.name}-AO.mat");
                                UnityEngine.Debug.Log($"Created new GT Standard material {AssetDatabase.GetAssetPath(newMaterial)}");
                                // Todo: change SetFloat signature to use int version Shader.PropToId...
                                newMaterial.SetFloat(settings.ShaderPropertyName, 1);
                                meshRenderer.material = newMaterial;
                            }
                            else
                            {
                                UnityEngine.Debug.Log($"Upgrading material {material.shader.name} to Graphics Tools Standard.");
                            }
                        }
                    }
                }
            }

            UnityEngine.Debug.Log($"{nameof(GatherSamples)} vertex-count={mesh.vertexCount} rays-per-vertex={settings.SamplesPerVertex} elapsed-ms={watch.ElapsedMilliseconds}");
        }

        private bool IsStandardShader(Material material)
        {
            if (material.HasProperty(settings.ShaderPropertyName))
            {
                if (material.GetFloat(settings.ShaderPropertyName) == 1f)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnSelectionChanged()
        {
            //throw new System.NotImplementedException();

            //if (Selection.count == 0)
            //{
            //    selectedObjects.Clear();
            //    return;
            //}

            //if (Selection.count == 1)
            //{
            //    selectedObjects.Clear();
            //    selectedObjects.Add(Selection.activeGameObject);
            //    return;
            //}

            //foreach (var item in Selection.gameObjects)
            //{
            //    if (!selectedObjects.Contains(item))
            //    {
            //        selectedObjects.Add(item);
            //    }
            //}

            ////check for removed items
            //List<GameObject> objectsToRemove = new List<GameObject>();
            //foreach (var item in selectedObjects)
            //{
            //    if (!Selection.Contains(item))
            //    {
            //        objectsToRemove.Add(item);
            //    }
            //}

            //foreach (var item in objectsToRemove)
            //{
            //    selectedObjects.Remove(item);
            //}
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

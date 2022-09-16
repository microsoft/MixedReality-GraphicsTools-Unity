// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    public class AmbientOcclusionToolWindow : EditorWindow
    {
        [Header("Ray tracing")]

        [Tooltip("The number of rays to send into the hemisphere above the surface, per vertex. Bigger numbers will be slower.")]
        private int _samplesPerVertex = 100;

        [Tooltip("How far to search for nearby colliders in the scene.")]
        private float MaxSampleDistance = 1;

        [Header("Visualization")]
        [Tooltip("The index of vertex to visualize")]
        private int ReferenceVertexIndex = 0;

        private bool _showOrigin = true;
        private Color _originColor = Color.cyan;
        private float _originRadius = .03f;

        private bool _showNormal = true;
        private Color _normalColor = Color.cyan;
        private float _normalScale = 1;

        private bool _showBentNormal = true;
        private Color _bentNormalColor = Color.magenta;
        private float _bentNormalScale = 1;

        private bool _showSamples;
        private Color _sampleColor = Color.yellow;
        private bool _showHits;
        private Color _hitColor = Color.black;
        private float _hitRadius = .03f;
        private bool _showCoverage;
        private float _coverageRadius = .03f;

        private const float kOriginNormalOffset = .0001f;
        private const int kUvChannel = 5;

        private Vector3[] _vertexs;
        private Vector3[] _normals;
        private Vector4[] _bentNormalsAo;
        private List<Vector3> _referenceVertexSamples = new List<Vector3>();
        private List<RaycastHit> _referenceVertexHits = new List<RaycastHit>();
        private List<RaycastHit> _hitsInHemisphere = new List<RaycastHit>();
        private bool _upgradeMaterials = false;
        private MeshFilter _meshFilter;
        private int _samplesIndex = 2;

        //private Shader _standardShader;
        private int _magicPropId = Shader.PropertyToID("_VertexBentNormalAo");
        private string upgradePrefsKey = $"{nameof(AmbientOcclusionToolWindow)}{nameof(_upgradeMaterials)}";
        private static string _standardShaderPath = "Graphics Tools/Standard";

        [MenuItem("Window/Graphics Tools/Ambient occclusion")]
        private static void ShowWindow()
        {
            if (Shader.Find(_standardShaderPath) == null)
            {
                UnityEngine.Debug.LogError($"Unable to locate {_standardShaderPath}!");
            }
            AmbientOcclusionToolWindow window = GetWindow<AmbientOcclusionToolWindow>();
            window.titleContent = new GUIContent("Ambient occlusion");
            window.Show();
        }

        /// <summary>
        /// Custom inspector that exposes additional user controls for the AmbientOcclusion component
        /// </summary>
        [CustomEditor(typeof(AmbientOcclusion)), CanEditMultipleObjects]
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Ray tracing");
            var _samplesPerVertexLabels = new string[] { "1", "10", "100", "1000", "10000" };
            _samplesIndex = EditorGUILayout.Popup("Samples per vertex", _samplesIndex, _samplesPerVertexLabels);
            _samplesPerVertex = (int)Mathf.Pow(10, _samplesIndex);

            MaxSampleDistance = EditorGUILayout.FloatField("Max sample distance", MaxSampleDistance);
            _upgradeMaterials = EditorGUILayout.Toggle("Update materials", PlayerPrefs.GetInt(upgradePrefsKey) > 0);
            PlayerPrefs.SetInt(upgradePrefsKey, _upgradeMaterials == false ? 0 : 1);

            if (GUILayout.Button("Gather selection samples"))
            {
                GatherSelectionSamples();
            }

            _showSamples = EditorGUILayout.Toggle("Show samples", _showSamples);
        }

        void OnDrawGizmos()
        {
            if (_vertexs == null
                || _vertexs.Length == 0
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
        private void GatherSelectionSamples()
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

                var origin = _vertexs[vi] + _normals[vi].normalized * kOriginNormalOffset;

                Physics.queriesHitBackfaces = true;

                for (int ni = 0; ni < _samplesPerVertex; ni++)
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
                                                 1 - ((float)_hitsInHemisphere.Count / _samplesPerVertex));
            }

            mesh.SetUVs(kUvChannel, _bentNormalsAo);

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
                        if (_upgradeMaterials)
                        {
                            if (material.name == "Default-Material")
                            {
                                Material newMaterial = new Material(Shader.Find(_standardShaderPath));
                                AssetDatabase.CreateAsset(newMaterial, $"Assets/{material.name}-AO.mat");
                                UnityEngine.Debug.Log($"Created new GT Standard material {AssetDatabase.GetAssetPath(newMaterial)}");
                                newMaterial.SetFloat(_magicPropId, 1);
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

            UnityEngine.Debug.Log($"{nameof(GatherSamples)} elapsed-ms={watch.ElapsedMilliseconds} vertex-count={mesh.vertexCount} rays-per-vertex={_samplesPerVertex}");
        }

        private bool IsStandardShader(Material material)
        {
            if (material.HasProperty(_magicPropId))
            {
                if (material.GetFloat(_magicPropId) == 1f)
                {
                    return true;
                }
            }
            return false;
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

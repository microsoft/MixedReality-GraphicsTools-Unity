using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(Sampler))]
    public class AmbientOcclusion : MonoBehaviour
    {
        // stores mesh before we go and mess with vertex color
        private Mesh originalMesh;
        private void OnEnable()
        {
            originalMesh = GetComponent<MeshFilter>().sharedMesh;
            ApplySamplerCoverageToVertexs();
        }

        private void OnDisable()
        {
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            var colors = new Color[mesh.vertexCount];
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                colors[i] = Color.white;
            }
            mesh.colors = colors;
        }

        [ContextMenu(nameof(ApplySamplerCoverageToVertexs))]
        public void ApplySamplerCoverageToVertexs()
        {
            var coverages = GetComponent<Sampler>().Coverages;
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            var colors = new Color[mesh.vertexCount];
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                var ao = 1 - coverages[i];
                colors[i] = new Color(ao, ao, ao);
            }
            mesh.colors = colors;
        }

        private void OnValidate()
        {
            ApplySamplerCoverageToVertexs();
        }
    }
}

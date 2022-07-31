using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    [RequireComponent(typeof(MeshFilter))]
    public class ShowMeshNormals : MonoBehaviour
    {
        [SerializeField] private float scale = 1;
        private Vector3[] _worldPositions;
        private Vector3[] _worldNormals;

        private void OnDrawGizmosSelected()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            UpdatePositionsAndNormals();

            for (int i = 0; i < _worldPositions.Length; i++)
            {
                Gizmos.color = ColorVector(_worldNormals[i]);
                Gizmos.DrawLine(_worldPositions[i], _worldPositions[i] + _worldNormals[i] * scale);
            }
        }

        private void UpdatePositionsAndNormals()
        {
            var vertexs = GetComponent<MeshFilter>().sharedMesh.vertices;
            var normals = GetComponent<MeshFilter>().sharedMesh.normals;

            _worldPositions = new Vector3[vertexs.Length];
            _worldNormals = new Vector3[normals.Length];

            for (int i = 0; i < vertexs.Length; i++)
            {
                _worldPositions[i] = transform.TransformPoint(vertexs[i]); // world space point
                _worldNormals[i] = transform.TransformVector(normals[i]); // world space normal
            }
        }

        private Color ColorVector(Vector3 vector3)
        {
            return new Color(
                vector3.x * .5f + .5f,
                vector3.y * .5f + .5f,
                vector3.z * .5f + .5f);
        }

        private void OnEnable()
        {
            UpdatePositionsAndNormals();
            // This forces the Inspector to show the active checkbox for this component
        }
    }
}

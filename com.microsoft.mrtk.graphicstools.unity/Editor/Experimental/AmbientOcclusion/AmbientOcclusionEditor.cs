using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    [CustomEditor(typeof(AmbientOcclusion)), CanEditMultipleObjects]
    public class AmbientOcclusionInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Gather samples"))
            {
                var component = target as AmbientOcclusion;
                component.GatherSamples();
            }
            DrawDefaultInspector();
        }
    }
}

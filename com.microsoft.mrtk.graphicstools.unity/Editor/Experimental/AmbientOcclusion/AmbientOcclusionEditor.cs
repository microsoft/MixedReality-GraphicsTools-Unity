using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    public class AmbientOcclusionEditor : UnityEditor.Editor
    {
        [MenuItem("Window/Graphics Tools/Add ambient occlusion")]
        public static void AddAmbientOcclusion()
        {
            foreach (var go in Selection.gameObjects)
            {
                go.AddComponent<AmbientOcclusion>();
            }
        }
    }

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

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Custom inspector that displays the blurred texture within the editor.
    /// </summary>
    [CustomEditor(typeof(AcrylicBackgroundRectProvider), true)]
    public class AcrylicBackgroundRectProviderInspector : UnityEditor.Editor
    {
        /// <summary>
        /// Renders a custom inspector GUI. 
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AcrylicBackgroundRectProvider provider = target as AcrylicBackgroundRectProvider;

            if (provider != null)
            {
                GUILayout.BeginVertical("Box");
                if (provider.BlurredTexture != null)
                {
                    GUILayout.Label("Blurred Texture");
                    GUILayout.Label(provider.BlurredTexture);
                }
                else
                {
                    GUILayout.Label("Source Texture");
                    GUILayout.Label(provider.SourceTexture);
                }

                GUILayout.EndVertical();
            }
        }
    }
}
#endif // GT_USE_URP
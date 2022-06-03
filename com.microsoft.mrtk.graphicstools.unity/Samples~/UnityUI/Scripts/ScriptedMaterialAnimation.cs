// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Samples.UnityUI
{
    /// <summary>
    /// Example of how to use a CanvasMaterialAnimatorBase to animate properties via script.
    /// </summary>
    public class ScriptedMaterialAnimation : MonoBehaviour
    {
        public CanvasMaterialAnimatorGraphicsToolsStandardCanvas Animator;

        private void Update()
        {
            Animator._VertexExtrusionValue = Mathf.Lerp(0, 0.002f, (Mathf.Sin(Mathf.Repeat(Time.time, Mathf.PI * 2.0f)) + 1.0f) * 0.5f);
            Animator.ApplyToMaterial();
        }
    }
}

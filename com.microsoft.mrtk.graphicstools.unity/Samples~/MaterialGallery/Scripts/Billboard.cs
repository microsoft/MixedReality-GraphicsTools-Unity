// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Samples.MaterialGallery
{
    public class Billboard : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}

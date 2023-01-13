// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;


namespace Microsoft.MixedReality.GraphicsTools.Samples.ClippingPrimitives
{

    public class Spin : MonoBehaviour
    {
        [Range(0,1)]
        public float spin = 0.2f;


        void Update()
        {
            this.gameObject.transform.Rotate(0, spin, 0);
        }
    }
}


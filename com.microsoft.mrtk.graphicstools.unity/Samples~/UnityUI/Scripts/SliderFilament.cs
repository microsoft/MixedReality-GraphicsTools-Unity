// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.GraphicsTools.Samples.UnityUI
{
    public class SliderFilament : MonoBehaviour
    {

        public Slider slider;
        Material mat;

        void Start()
        {
            mat = GetComponent<Renderer>().material;
        }

        void Update()
        {

            {
                mat.SetFloat("_RimPower", (slider.value * -1.1f + 1)*5);
            }

        }
    }
}

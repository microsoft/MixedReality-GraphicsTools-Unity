// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.GraphicsTools.Samples.UnityUI
{
    public class SliderLight : MonoBehaviour
    {

        public Slider slider;
        public Button red, green, yellow;
        Material mat;

        void Start()
        {
            red.onClick.AddListener(RedLight);
            green.onClick.AddListener(GreenLight);
            yellow.onClick.AddListener(YellowLight);
            mat = GetComponent<Renderer>().material;
        }
        void RedLight()
        {
            mat.SetColor("_EmissiveColor", Color.red);
        }
        void GreenLight()
        {
            mat.SetColor("_EmissiveColor", Color.green);
        }
        void YellowLight()
        {
            mat.SetColor("_EmissiveColor", Color.yellow);
        }
        void Update()
        {
            
            {
                mat.SetFloat("_Fade", slider.value * 0.66f);
            }

        }
    }
}

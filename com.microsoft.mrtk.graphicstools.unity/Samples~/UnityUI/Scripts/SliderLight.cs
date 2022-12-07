using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.GraphicsTools.Samples.UnityUI
{
    public class SliderLight : MonoBehaviour
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
                mat.SetFloat("_Fade", slider.value * 0.66f);
            }

        }
    }
}

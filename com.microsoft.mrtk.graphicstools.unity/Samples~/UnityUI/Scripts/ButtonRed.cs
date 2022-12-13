using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.GraphicsTools.Samples.UnityUI
{
    public class Buttonred : MonoBehaviour
    {

        Material mat;
        Renderer rend;

        void Start()
        {
            rend = GetComponent<Renderer>();
            mat = GetComponent<Renderer>().material;
            mat.SetColor("_EmissiveColor", Color.red);

        }

        
    }
}
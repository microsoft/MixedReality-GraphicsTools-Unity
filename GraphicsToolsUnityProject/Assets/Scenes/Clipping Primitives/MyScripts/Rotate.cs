using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float spin = .5f;

   
    void Update()
    {
        this.gameObject.transform.Rotate(0,spin,0);
    }
}

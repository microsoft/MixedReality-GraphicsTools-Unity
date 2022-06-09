using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifyShaderScreenPos : MonoBehaviour
{
    public Material material;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 screenPixels =Camera.main.WorldToScreenPoint(transform.position);
        screenPixels = new Vector2(screenPixels.x/Screen.width,screenPixels.y/Screen.height);
        material.SetVector("creenPos",screenPixels);
    }
}

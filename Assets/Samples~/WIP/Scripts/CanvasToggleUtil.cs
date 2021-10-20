using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasToggleUtil : MonoBehaviour
{
    public GameObject target = null;

    public void ToggleOnOff()
    {
        bool on = target.activeSelf;
        target.SetActive(!on);
    }
}

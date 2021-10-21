// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Event triggered by the IPointerEnterHandler interface.")]
    public UnityEvent OnPointerEnterEvent = new UnityEvent();

    [Tooltip("Event triggered by the IPointerExitHandler interface.")]
    public UnityEvent OnPointerExitEvent = new UnityEvent();

    /// <summary>
    /// Use to detect when the mouse (or pointer) begins to hover over a certain GameObject. 
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent.Invoke();
    }

    /// <summary>
    /// Use to detect when the mouse (or pointer) stops hovering over a certain GameObject. 
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent.Invoke();
    }
}

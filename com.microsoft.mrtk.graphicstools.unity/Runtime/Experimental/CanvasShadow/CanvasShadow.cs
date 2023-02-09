// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// Displays a simulated shadow onto a canvas using projected shadowing.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CanvasShadow : MonoBehaviour
    {
        [Experimental]
        [SerializeField]
        [Tooltip("Enables real time shadow positioning")]
        private bool Realtime = false;

        [SerializeField]
        [Tooltip("How much bigger the shadow should be than the caster (for soft edges)")]
        private float Spread = 10.0f;
        [SerializeField]
        [Tooltip("Raise shadow off the backing shadow receiver (avoid z fighting)")]
        private float BackingOffset = -0.01f;

        [SerializeField]
        [Tooltip("The Directional Light that is casting shadows")]
        private Light Light;

        private RectTransform _casterRectTransform;
        private RectTransform _shadowRectTransform;
        private Transform _lightTransform;
        private Plane _shadowPlane = new Plane();
        private Ray _lightDirectionRay = new Ray();
        private Vector2 _sizeDelta = new Vector2();

        private void Start()
        {
            Initialize();
            UpdateShadow();
        }

        private void Update()
        {
            if (Realtime == false)
            {
                return;
            }

            UpdateShadow();
        }

        private void Initialize()
        {
            _casterRectTransform = transform.parent.gameObject.GetComponent<RectTransform>();
            _shadowRectTransform = GetComponent<RectTransform>();

            if (Light != null)
            {
                _lightTransform = Light.transform;
            }
        }

        private void UpdateShadow()
        {
            if (_lightTransform == null)
                return;

            Vector3 shadowLocalPosition = _casterRectTransform.localPosition;
            shadowLocalPosition.z = -_casterRectTransform.localPosition.z + BackingOffset;
            _shadowRectTransform.localPosition = shadowLocalPosition;

            _sizeDelta.Set(Spread, Spread);
            _shadowRectTransform.sizeDelta = _sizeDelta;

            _shadowPlane.SetNormalAndPosition(_shadowRectTransform.forward, _shadowRectTransform.position);

            _lightDirectionRay.origin = _casterRectTransform.position;
            _lightDirectionRay.direction = _lightTransform.forward;

            float intersectDistance;

            _shadowPlane.Raycast(_lightDirectionRay, out intersectDistance);

            _shadowRectTransform.position = _lightDirectionRay.GetPoint(intersectDistance);
        }

#if UNITY_EDITOR
        [ContextMenu("Update Shadow")]
        public void UpdateShadowInEditor()
        {
            Initialize();
            UpdateShadow();
        }
#endif
    }
}

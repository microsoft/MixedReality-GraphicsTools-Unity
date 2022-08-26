// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.using System.Collections;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Samples.Acrylic
{
    public class Orbitals : MonoBehaviour
    {
        public int count = 16;

        public PrimitiveType primitiveType = PrimitiveType.Cube;

        public Material orbitalMaterial;

        public float size = 0.1f;

        public float sizeVary = 0.5f;

        public float speed = 1.0f;

        public float speedVary = 0.5f;

        public float radius = 1.0f;

        public float radiusVary = 0.5f;

        public float tiltVary = 0.1f;

        public class Orbiter
        {
            GameObject goParent;

            private float angle;
            private float speed;
            private float tiltY, tiltX;
            public Orbiter(PrimitiveType primitiveType, Material material, Transform parent, int index, float size, float radius, float startSpeed, float startAngle, float tiltx, float tilty)
            {
                GameObject go = GameObject.CreatePrimitive(primitiveType);
                goParent = new GameObject("Orb " + index);
                goParent.transform.SetParent(parent, false);
                go.transform.SetParent(goParent.transform, false);
                go.transform.localScale = size * Vector3.one;
                go.transform.localPosition = radius * Vector3.right;
                speed = startSpeed;
                tiltX = tiltx;
                tiltY = tilty;
                angle = startAngle;

                Renderer r = go.GetComponent<Renderer>();
                if (r != null)
                {
                    r.material = material;
                }
                UpdatePosition(0.0f);
            }

            public void UpdatePosition(float dt)
            {
                angle += speed * dt;
                goParent.transform.localEulerAngles = new Vector3(tiltX, tiltY, angle);
            }
        }

        private List<Orbiter> orbiters = new List<Orbiter>();

        void Start()
        {
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;
                float tsize = size * (1.0f - sizeVary * Random.value);
                float tspeed = speed * (1.0f - speedVary * Random.value) * 360.0f;
                float tradius = radius * (1.0f - radiusVary * t);
                float tangle = 360.0f * t;
                float tiltx = tiltVary * (0.5f - Random.value) * 180.0f;
                float tilty = tiltVary * (0.5f - Random.value) * 180.0f;
                orbiters.Add(new Orbiter(primitiveType, orbitalMaterial, transform, i, tsize, tradius, tspeed, tangle, tiltx, tilty));
            }
        }

        void Update()
        {
            for (int i = 0; i < count; i++)
            {
                orbiters[i].UpdatePosition(Time.deltaTime);
            }
        }
    }
}
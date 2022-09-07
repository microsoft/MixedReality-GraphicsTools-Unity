// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// A implementation of Unity's Random class which is safe to call from multiple threads.
    /// </summary>
    public static class ThreadSafeRandom
    {
        [System.ThreadStatic]
        private static System.Random local;
        private static System.Random global = new System.Random();

        /// <summary>
        /// Returns a random float within [0.0..1.0] (range is inclusive) (Read Only).
        /// </summary>
        public static float value
        {
            get
            {
                Initialize();
                return (float)(local.NextDouble());
            }
        }

        /// <summary>
        /// Returns a random int within [minInclusive..maxInclusive] (range is inclusive).
        /// </summary>
        public static int Range(int min, int max)
        {
            Initialize();
            return local.Next(min, max);
        }

        /// <summary>
        /// Returns a random float within [minInclusive..maxInclusive] (range is inclusive).
        /// </summary>
        public static float Range(float min, float max)
        {
            return value * (max - min) + min;
        }

        /// <summary>
        /// Returns a random point inside or on a circle with radius 1.0 (Read Only).
        /// </summary>
        public static Vector2 insideUnitCircle
        {
            get
            {
                return new Vector2(value * 2.0f - 1.0f,
                                   value * 2.0f - 1.0f).normalized;
            }
        }

        /// <summary>
        /// Returns a random point on the surface of a sphere with radius 1.0 (Read Only).
        /// </summary>
        public static Vector3 onUnitSphere
        {
            get
            {
                return new Vector3(value * 2.0f - 1.0f,
                                   value * 2.0f - 1.0f,
                                   value * 2.0f - 1.0f).normalized;
            }
        }

        /// <summary>
        /// Returns a random point inside or on a sphere with radius 1.0 (Read Only).
        /// </summary>
        public static Vector3 insideUnitSphere
        {
            get
            {
                return onUnitSphere * value;
            }
        }

        /// <summary>
        /// Returns a random rotation (Read Only).
        /// </summary>
        public static Quaternion rotation
        {
            get
            {
                return new Quaternion(value, value, value, value);
            }
        }

        private static void Initialize()
        {
            if (local == null)
            {
                int seed;

                lock (global)
                {
                    seed = global.Next();
                }

                local = new System.Random(seed);
            }
        }
    }
}

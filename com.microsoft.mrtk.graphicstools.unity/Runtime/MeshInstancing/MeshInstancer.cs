// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

// WebGL doesn't support threaded operations.
#if !UNITY_WEBGL
using System.Collections.Concurrent;
using System.Threading.Tasks;
#endif

namespace Microsoft.MixedReality.GraphicsTools
{
    /// <summary>
    /// The MeshInstancer is a component used for quickly using Unity's Graphics.DrawMeshInstanced API. The MeshInstancer should be
    /// used when you need to render and update massive amounts of objects (greater than ~1000). Note, each object rendered is not
    /// a typical Unity GameObject, for performance reasons, but is instead a MeshInstancer.Instance. MeshInstancer.Instances can be
    /// updated concurrently by specifying a ParallelUpdate delegate.
    /// </summary>
    public class MeshInstancer : MonoBehaviour
    {
        // Note: You can only draw a maximum of 1023 instances at once.
        // https://docs.unity3d.com/ScriptReference/Graphics.DrawMeshInstanced.html
        public const int UNITY_MAX_INSTANCE_COUNT = 1023;

        /// <summary>
        /// The mesh to draw via Graphics.DrawMeshInstanced.
        /// </summary>
        [Header("Visuals"), Tooltip("The mesh to draw via Graphics.DrawMeshInstanced.")]
        public UnityEngine.Mesh InstanceMesh = null;

        /// <summary>
        /// Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.
        /// </summary>
        [Min(0), Tooltip("Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.")]
        public int InstanceSubMeshIndex = 0;

        /// <summary>
        /// The material to use when drawing. The material must have "Enable GPU Instancing" checked on.
        /// </summary>
        [Tooltip("The material to use when drawing. The material must have \"Enable GPU Instancing\" checked on.")]
        public Material InstanceMaterial = null;

        /// <summary>
        /// Determines whether the Meshes should cast shadows.
        /// </summary>
        [Tooltip("Determines whether the Meshes should cast shadows.")]
        public UnityEngine.Rendering.ShadowCastingMode ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        /// <summary>
        /// Determines whether the Meshes should receive shadows.
        /// </summary>
        [Tooltip("Determines whether the Meshes should receive shadows.")]
        public bool RecieveShadows = false;

        [Serializable]
        private class FloatMaterialProperty
        {
            public string Name = null;
            public float DefaultValue = 0.0f;
        }

        [SerializeField, Tooltip("Name and default value of instanced float material properties. These must be set in order to update them at runtime.")]
        private FloatMaterialProperty[] FloatMaterialProperties = new FloatMaterialProperty[0];

        [Serializable]
        private class VectorMaterialProperty
        {
            public string Name = null;
            public Vector4 DefaultValue = Vector3.zero;
        }

        [SerializeField, Tooltip("Name and default value of instanced vector material properties. These must be set in order to update them at runtime.")]
        private VectorMaterialProperty[] VectorMaterialProperties = new VectorMaterialProperty[0];

        [Serializable]
        private class MatrixMaterialProperty
        {
            public string Name = null;
            public Matrix4x4 DefaultValue = Matrix4x4.identity;
        }

        [SerializeField, Tooltip("Name and default value of instanced matrix material properties. These must be set in order to update them at runtime.")]
        private MatrixMaterialProperty[] MatrixMaterialProperties = new MatrixMaterialProperty[0];

        /// <summary>
        /// If true, the RaycastHits list is filled out each frame with instances that intersect the DeferredRayQuery. Disable this if you don't need to query instances.
        /// </summary>
        [Header("Physics"), Tooltip("If true, the RaycastHits list is filled out each frame with instances that intersect the DeferredRayQuery. Disable this if you don't need to query instances.")]
        public bool RaycastInstances = false;

        /// <summary>
        /// AABB of an instance.
        /// </summary>
        [Serializable]
        public struct Box
        {
            public Vector3 Center;
            public Vector3 Size;
        }

        /// <summary>
        /// Raycast query results when RaycastInstances is true and the DeferredRayQuery intersects an instance.
        /// </summary>
        public struct RaycastHit
        {
            /// <summary>
            /// The instance hit with the raycast query.
            /// </summary>
            public Instance Instance;

            /// <summary>
            /// The raycast intersection point in world space.
            /// </summary>
            public Vector3 Point;

            /// <summary>
            /// The raycast intersection normal in world space. 
            /// </summary>
            public Vector3 Normal;

            /// <summary>
            /// The distance along the ray the intersection point lies at in world space.
            /// </summary>
            public float Distance;

            /// <summary>
            /// The ray used during the query.
            /// </summary>
            public Ray Ray;

            /// <summary>
            /// Restores the hit to the default state.
            /// </summary>
            public void Reset()
            {
                Instance = null;
                Point = Vector3.zero;
                Normal = Vector3.zero;
                Distance = float.MaxValue;
                Ray = new Ray();
            }
        }

        /// <summary>
        /// The AABB local space position and extents of every instance's collider.
        /// </summary>
        public Box BoxCollider = new Box() { Center = Vector3.zero, Size = Vector3.one };

        /// <summary>
        /// The ray to use in queries if RaycastInstances is true.
        /// </summary>
        public Ray DeferredRayQuery { get; set; }

        /// <summary>
        /// The list of all hits that DeferredRayQuery intersects with when RaycastInstances is true. 
        /// Results are from the previous frame's LateUpdate query.
        /// </summary>
        public List<RaycastHit> DeferredRaycastHits { get; private set; }

        /// <summary>
        /// Signature of the multi-threaded update method for each instance.
        /// </summary>
        public delegate void ParallelUpdate(float deltaTime, Instance instance);

        /// <summary>
        /// The total number of instances across all buckets.
        /// </summary>
        public int InstanceCount { get; private set; }

        /// <summary>
        /// If true displays diagnostic GUI text in the bottom left of the screen displaying how long it took to update all instances.
        /// </summary>
        [Header("Diagnostics"), Tooltip("If true displays diagnostic GUI text in the bottom left of the screen displaying how long it took to update all instances.")]
        public bool DisplayUpdateTime = false;

        /// <summary>
        /// Forces all instance updating to happen on the main thread. Helpful when debugging multi-threaded issues. 
        /// </summary>
        [Tooltip("Forces all instance updating to happen on the main thread. Helpful when debugging multi-threaded issues. ")]
        public bool DisableParallelUpdate = false;

        /// <summary>
        /// An object that represents a instance to be drawn. Akin to a Unity GameObject.
        /// </summary>
        public class Instance
        {
            /// <summary>
            /// The unique instance id within a bucket.
            /// NOTE: Thread safe.
            /// </summary>
            public int InstanceIndex { get; private set; }

            /// <summary>
            /// The id of the bucket the instances lives in. Instances live in buckets of UNITY_MAX_INSTANCE_COUNT.
            /// NOTE: Thread safe.
            /// </summary>
            public int InstanceBucketIndex { get; private set; }

            /// <summary>
            /// Pointer to any custom data the developer wishes to store on a per instance basis.
            /// NOTE: Thread safe.
            /// NOTE: This data should be a value type to avoid allocations when boxing and unboxing.
            /// </summary>
            public System.Object UserData { get; set; }

            /// <summary>
            /// The world space position of the instance.
            /// NOTE: Thread safe.
            /// </summary>
            public Vector3 Position
            {
                get { return Transformation.GetColumn(3); }
                set { Transformation = Matrix4x4.TRS(value, Rotation, LossyScale); }
            }

            /// <summary>
            /// Position of the instance relative to the parent MeshInstancer.
            /// NOTE: Thread safe.
            /// </summary>
            public Vector3 LocalPosition
            {
                get { return LocalTransformation.GetColumn(3); }
                set { LocalTransformation = Matrix4x4.TRS(value, LocalRotation, LocalScale); }
            }

            /// <summary>
            /// A Quaternion that stores the rotation of the instance in world space.
            /// NOTE: Thread safe.
            /// </summary>
            public Quaternion Rotation
            {
                get { Matrix4x4 matrix = Transformation; return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1)); }
                set { Transformation = Matrix4x4.TRS(Position, value, LossyScale); }
            }

            /// <summary>
            /// The rotation of the instance relative to the instance rotation of the parent MeshInstancer.
            /// NOTE: Thread safe.
            /// </summary>
            public Quaternion LocalRotation
            {
                get { Matrix4x4 matrix = LocalTransformation; return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1)); }
                set { LocalTransformation = Matrix4x4.TRS(LocalPosition, value, LocalScale); }
            }

            /// <summary>
            /// The global scale of the instance (Read Only).
            /// NOTE: Thread safe.
            /// </summary>
            public Vector3 LossyScale
            {
                get { Matrix4x4 matrix = Transformation; return new Vector3(matrix.GetColumn(0).magnitude, matrix.GetColumn(1).magnitude, matrix.GetColumn(2).magnitude); }
            }

            /// <summary>
            /// The scale of the instance relative to the parent MeshInstancer.
            /// NOTE: Thread safe.
            /// </summary>
            public Vector3 LocalScale
            {
                get { Matrix4x4 matrix = LocalTransformation; return new Vector3(matrix.GetColumn(0).magnitude, matrix.GetColumn(1).magnitude, matrix.GetColumn(2).magnitude); }
                set { LocalTransformation = Matrix4x4.TRS(LocalPosition, LocalRotation, value); }
            }

            /// <summary>
            /// Translation, rotation, and scale matrix in world space.
            /// NOTE: Thread safe.
            /// </summary>
            public Matrix4x4 Transformation
            {
                get { return meshInstancer.transform.localToWorldMatrix * LocalTransformation; }
                set { LocalTransformation = meshInstancer.transform.worldToLocalMatrix * value; }
            }

            /// <summary>
            ///  Translation, rotation, and scale matrix relative to the parent MeshInstancer.
            /// NOTE: Thread safe.
            /// </summary>
            public Matrix4x4 LocalTransformation
            {
                get { return meshInstancer.instanceBuckets[InstanceBucketIndex].Matricies[InstanceIndex]; }
                set { meshInstancer.instanceBuckets[InstanceBucketIndex].Matricies[InstanceIndex] = value; }
            }

            /// <summary>
            /// True if this instance has been destoryed (removed) from the parent MeshInstancer.
            /// NOTE: Thread safe.
            /// </summary>
            public bool Destroyed
            {
                get { return (meshInstancer == null); }
            }

            private MeshInstancer meshInstancer;

            private Instance() { }

            /// <summary>
            /// Default constructor.
            /// NOTE: Thread safe.
            /// </summary>
            public Instance(int instanceIndex, int instanceBucketIndex, System.Object data, MeshInstancer instancer)
            {
                InstanceIndex = instanceIndex;
                InstanceBucketIndex = instanceBucketIndex;
                UserData = data;
                meshInstancer = instancer;
            }

            /// <summary>
            /// Call this method to destory an instance so it no longer renders or updates.
            /// NOTE: Not thread safe.
            /// </summary>
            public void Destroy()
            {
                if (Destroyed) { Debug.LogWarning("Attempting to double destroy a MeshInstancer instance."); return; }

                meshInstancer.Destroy(this);

                InstanceIndex = -1;
                InstanceBucketIndex = -1;
                meshInstancer = null;
            }

            /// <summary>
            /// Sets an instanced material float property.
            /// NOTE: Not thread safe.
            /// </summary>
            public void SetFloat(string name, float value)
            {
                SetFloat(Shader.PropertyToID(name), value);
            }

            /// <summary>
            /// Sets an instanced material float property.
            /// NOTE: Not thread safe.
            /// </summary>
            public void SetFloat(int nameID, float value)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to SetFloat on a destroyed MeshInstancer instance."); return; }

                meshInstancer.SetFloat(this, nameID, value);
            }

            /// <summary>
            /// Gets an instanced material float property.
            /// NOTE: Not thread safe.
            /// </summary>
            public float GetFloat(string name)
            {
                return GetFloat(Shader.PropertyToID(name));
            }

            /// <summary>
            /// Gets an instanced material float property.
            /// NOTE: Not thread safe.
            /// </summary>
            public float GetFloat(int nameID)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to GetFloat on a destroyed MeshInstancer instance."); return 0.0f; }

                return meshInstancer.GetFloat(this, nameID);
            }

            /// <summary>
            /// Sets an instanced material vector property.
            /// NOTE: Not thread safe.
            /// </summary>
            public void SetVector(string name, Vector4 value)
            {
                SetVector(Shader.PropertyToID(name), value);
            }

            /// <summary>
            /// Sets an instanced material vector property.
            /// NOTE: Not thread safe.
            /// </summary>
            public void SetVector(int nameID, Vector4 value)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to SetVector on a destroyed MeshInstancer instance."); return; }

                meshInstancer.SetVector(this, nameID, value);
            }

            /// <summary>
            /// Gets an instanced material vector property.
            /// NOTE: Not thread safe.
            /// </summary>
            public Vector4 GetVector(string name)
            {
                return GetVector(Shader.PropertyToID(name));
            }

            /// <summary>
            /// Gets an instanced material vector property.
            /// NOTE: Not thread safe.
            /// </summary>
            public Vector4 GetVector(int nameID)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to GetVector on a destroyed MeshInstancer instance."); return Vector4.zero; }

                return meshInstancer.GetVector(this, nameID);
            }

            /// <summary>
            /// Sets an instanced material matrix property.
            /// NOTE: Not thread safe.
            /// </summary>
            public void SetMatrix(string name, Matrix4x4 value)
            {
                SetMatrix(Shader.PropertyToID(name), value);
            }

            /// <summary>
            /// Sets an instanced material matrix property.
            /// NOTE: Not thread safe.
            /// </summary>
            public void SetMatrix(int nameID, Matrix4x4 value)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to SetMatrix on a destroyed MeshInstancer instance."); return; }

                meshInstancer.SetMatrix(this, nameID, value);
            }

            /// <summary>
            /// Gets an instanced material matrix property.
            /// NOTE: Not thread safe.
            /// </summary>
            public Matrix4x4 GetMatrix(string name)
            {
                return GetMatrix(Shader.PropertyToID(name));
            }

            /// <summary>
            /// Gets an instanced material matrix property.
            /// NOTE: Not thread safe.
            /// </summary>
            public Matrix4x4 GetMatrix(int nameID)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to v on a destroyed MeshInstancer instance."); return Matrix4x4.identity; }

                return meshInstancer.GetMatrix(this, nameID);
            }

            /// <summary>
            /// Sets the delegate update method to call each frame. Make sure all function within this method are thread safe.
            /// NOTE: Not thread safe.
            /// </summary>
            public void SetParallelUpdate(ParallelUpdate parallelUpdate)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to set the ParallelUpdate method on a destroyed MeshInstancer instance."); return; }

                meshInstancer.instanceBuckets[InstanceBucketIndex].ParallelUpdates[InstanceIndex] = parallelUpdate;
            }
        }

        private class InstanceBucket
        {
            public int InstanceCount = 0;
            public Instance[] Instances = new Instance[UNITY_MAX_INSTANCE_COUNT];
            public Matrix4x4[] Matricies = new Matrix4x4[UNITY_MAX_INSTANCE_COUNT];
            public MaterialPropertyBlock Properties = new MaterialPropertyBlock();
            public ParallelUpdate[] ParallelUpdates = new ParallelUpdate[UNITY_MAX_INSTANCE_COUNT];
#if UNITY_WEBGL
            public List<RaycastHit> RaycastHits = new List<RaycastHit>();
#else
            public ConcurrentBag<RaycastHit> RaycastHits = new ConcurrentBag<RaycastHit>();
#endif
            private Matrix4x4[] matrixScratchBuffer = new Matrix4x4[UNITY_MAX_INSTANCE_COUNT];

            private InstanceBucket() { }

            public InstanceBucket(List<KeyValuePair<int, float>> floatMaterialProperties,
                                  List<KeyValuePair<int, Vector4>> vectorMaterialProperties,
                                  List<KeyValuePair<int, Matrix4x4>> matrixMaterialProperties)
            {
                RegisterMaterialPropertiesFloat(floatMaterialProperties);
                RegisterMaterialPropertiesVector(vectorMaterialProperties);
                RegisterMaterialPropertiesMatrix(matrixMaterialProperties);
            }

            public void RegisterMaterialPropertiesFloat(List<KeyValuePair<int, float>> materialProperties)
            {
                foreach (var property in materialProperties)
                {
                    Properties.SetFloatArray(property.Key, Repeat(property.Value, UNITY_MAX_INSTANCE_COUNT));
                }
            }

            public void RegisterMaterialPropertiesVector(List<KeyValuePair<int, Vector4>> materialProperties)
            {
                foreach (var property in materialProperties)
                {
                    Properties.SetVectorArray(property.Key, Repeat(property.Value, UNITY_MAX_INSTANCE_COUNT));
                }
            }

            public void RegisterMaterialPropertiesMatrix(List<KeyValuePair<int, Matrix4x4>> materialProperties)
            {
                foreach (var property in materialProperties)
                {
                    Properties.SetMatrixArray(property.Key, Repeat(property.Value, UNITY_MAX_INSTANCE_COUNT));
                }
            }

            public void UpdateJob(float deltaTime, Matrix4x4 localToWorld, int firstIndex, int lastIndex)
            {
                for (int i = firstIndex; i < lastIndex; ++i)
                {
                    // Apply any per instance logic.
                    ParallelUpdates[i]?.Invoke(deltaTime, Instances[i]);

                    // Calculate the final transformation matrix.
                    matrixScratchBuffer[i] = localToWorld * Matricies[i];
                }
            }

            public void UpdateJobRaycast(float deltaTime, Matrix4x4 localToWorld, Box box, Ray ray, int firstIndex, int lastIndex)
            {
                // Pre calculate collider info.
                Vector3 boxHalfSize = box.Size * 0.5f;
                Vector3 boxMin = box.Center - boxHalfSize;
                Vector3 boxMax = box.Center + boxHalfSize;

                for (int i = firstIndex; i < lastIndex; ++i)
                {
                    // Apply any per instance logic.
                    ParallelUpdates[i]?.Invoke(deltaTime, Instances[i]);

                    // Calculate the final transformation matrix.
                    matrixScratchBuffer[i] = localToWorld * Matricies[i];

                    // Perform a ray cast against the current instance. First do a coarse test against a sphere then a fine test against the OOBB.
                    // TODO - [Cameron-Micka] accelerate this with spatial partitioning?
                    float radius = Vector3.Scale(boxHalfSize, matrixScratchBuffer[i].lossyScale).magnitude;

                    if (RaycastSphere(ray, matrixScratchBuffer[i].GetColumn(3), radius))
                    {
                        RaycastHit hitInfo;
                        if (RaycastOOBB(ray, matrixScratchBuffer[i], boxMin, boxMax, out hitInfo))
                        {
                            hitInfo.Instance = Instances[i];
                            RaycastHits.Add(hitInfo);
                        }
                    }
                }
            }

            public void Draw(UnityEngine.Mesh mesh, int submeshIndex, Material material, UnityEngine.Rendering.ShadowCastingMode shadowCastingMode, bool recieveShadows)
            {
                Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrixScratchBuffer, InstanceCount, Properties, shadowCastingMode, recieveShadows);
            }

            private static T[] Repeat<T>(T element, int count)
            {
                var output = new T[count];

                for (int i = 0; i < count; ++i)
                {
                    output[i] = element;
                }

                return output;
            }

            private static bool RaycastSphere(Ray ray, Vector3 center, float radius)
            {
                // 5.3.2 Intersecting Ray or Segment Against Sphere
                // http://realtimecollisiondetection.net/

                Vector3 m = ray.origin - center;
                float b = Vector3.Dot(m, ray.direction);
                float c = Vector3.Dot(m, m) - (radius * radius);

                // Exit if ray’s origin outside sphere (c > 0) and ray pointing away from sphere (b > 0).
                if (c > 0.0f && b > 0.0f)
                {
                    return false;
                }

                // A negative discriminant corresponds to ray missing sphere.
                float discriminant = b * b - c;

                return (discriminant >= 0.0f);
            }

            private static bool Approximately(float a, float b, float epsilon = 0.0001f)
            {
                return (Mathf.Abs(a - b) < epsilon);
            }

            private static bool RaycastOOBB(Ray ray, Matrix4x4 localToWorld, Vector3 boxMin, Vector3 boxMax, out RaycastHit hitInfo)
            {
                // Transform the ray into the instance's local space.
                Matrix4x4 localToWorldInverse = localToWorld.inverse;
                Ray localRay = new Ray(localToWorldInverse.MultiplyPoint3x4(ray.origin), localToWorldInverse.MultiplyVector(ray.direction));

                if (RaycastAABB(localRay, boxMin, boxMax, out hitInfo))
                {
                    // Transform back to world space.
                    Vector3 localPoint = localRay.origin + (localRay.direction * hitInfo.Distance);
                    hitInfo.Point = localToWorld.MultiplyPoint3x4(localPoint);
                    hitInfo.Normal = new Vector3(Approximately(Mathf.Abs(localPoint.x), 0.5f) ? localPoint.x * 2.0f : 0.0f,
                                                 Approximately(Mathf.Abs(localPoint.y), 0.5f) ? localPoint.y * 2.0f : 0.0f,
                                                 Approximately(Mathf.Abs(localPoint.z), 0.5f) ? localPoint.z * 2.0f : 0.0f);
                    hitInfo.Normal = localToWorld.MultiplyVector(hitInfo.Normal).normalized;
                    hitInfo.Distance = (hitInfo.Point - ray.origin).magnitude;
                    hitInfo.Ray = ray;

                    return true;
                }

                return false;
            }

            private static bool RaycastAABB(Ray ray, Vector3 boxMin, Vector3 boxMax, out RaycastHit hitInfo)
            {
                // Fast and Robust Ray/OBB Intersection Using the Lorentz Transformation
                // https://www.realtimerendering.com/raytracinggems/rtg2/index.html

                Vector3 rayDirectionInverse = new Vector3(1.0f / ray.direction.x, 1.0f / ray.direction.y, 1.0f / ray.direction.z);

                Vector3 tMin = Vector3.Scale(boxMin - ray.origin, rayDirectionInverse);
                Vector3 tMax = Vector3.Scale(boxMax - ray.origin, rayDirectionInverse);
                Vector3 tMins = Vector3.Min(tMin, tMax);
                Vector3 tMaxes = Vector3.Max(tMin, tMax);
                float tBoxMin = Mathf.Max(Mathf.Max(tMins.x, tMins.y), tMins.z);
                float tBoxMax = Mathf.Min(Mathf.Min(tMaxes.x, tMaxes.y), tMaxes.z);

                hitInfo = new RaycastHit();

                if (tBoxMin <= tBoxMax)
                {
                    hitInfo.Distance = tBoxMin < 0.0f ? tBoxMax : tBoxMin;
                    return true;
                }

                return false;
            }
        }

        private bool isInitialized = false;
        private List<InstanceBucket> instanceBuckets = new List<InstanceBucket>();
        private List<KeyValuePair<int, float>> floatMaterialProperties = new List<KeyValuePair<int, float>>();
        private List<KeyValuePair<int, Vector4>> vectorMaterialProperties = new List<KeyValuePair<int, Vector4>>();
        private List<KeyValuePair<int, Matrix4x4>> matrixMaterialProperties = new List<KeyValuePair<int, Matrix4x4>>();

        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private long[] stopwatchSamples = new long[120];
        private int stopwatchSampleIndex = 0;
        private float averageElapsedMilliseconds = 0.0f;

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void LateUpdate()
        {
            DeferredRaycastHits.Clear();

            UpdateBuckets();

            foreach (var bucket in instanceBuckets)
            {
                // Draw each instance bucket.
                bucket.Draw(InstanceMesh, InstanceSubMeshIndex, InstanceMaterial, ShadowCastingMode, RecieveShadows);

                // Collect the aggregate raycast hits.
                if (RaycastInstances)
                {
                    DeferredRaycastHits.AddRange(bucket.RaycastHits);
                }
            }
        }

        private void OnGUI()
        {
            if (DisplayUpdateTime)
            {
                if (stopwatchSampleIndex == stopwatchSamples.Length)
                {
                    long sum = 0;
                    for (int i = 0; i < stopwatchSampleIndex; ++i)
                    {
                        sum += stopwatchSamples[i];
                    }

                    averageElapsedMilliseconds = (float)sum / stopwatchSampleIndex;
                    stopwatchSampleIndex = 0;
                }
                else
                {
                    stopwatchSamples[stopwatchSampleIndex] = stopwatch.ElapsedMilliseconds;
                    ++stopwatchSampleIndex;
                }

                string label = string.Format("MeshInstancer Update: {0} instances @ {1:f2} ms", InstanceCount, averageElapsedMilliseconds);
                GUI.Label(new Rect(10.0f, Screen.height - 24.0f, Screen.height, 128.0f), label);
            }
        }

        private void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            // Register all properties specified in the component properties.
            foreach (FloatMaterialProperty property in FloatMaterialProperties)
            {
                RegisterMaterialPropertyCommonFloat(Shader.PropertyToID(property.Name), property.DefaultValue, false);
            }

            foreach (VectorMaterialProperty property in VectorMaterialProperties)
            {
                RegisterMaterialPropertyCommonVector(Shader.PropertyToID(property.Name), property.DefaultValue, false);
            }

            foreach (MatrixMaterialProperty property in MatrixMaterialProperties)
            {
                RegisterMaterialPropertyCommonMatrix(Shader.PropertyToID(property.Name), property.DefaultValue, false);
            }

            DeferredRaycastHits = new List<RaycastHit>();

            isInitialized = true;
        }

        private void UpdateBuckets()
        {
            // Nothing to process.
            if (instanceBuckets.Count == 0)
            {
                return;
            }

            if (DisplayUpdateTime)
            {
                stopwatch.Restart();
            }

            float deltaTime = Time.deltaTime;
            Matrix4x4 localToWorld = transform.localToWorldMatrix;
            int processorCount = Environment.ProcessorCount;

            // WebGL doesn't support threaded operations.
#if UNITY_WEBGL
          foreach (InstanceBucket bucket in instanceBuckets)
          {
              if (RaycastInstances)
              {
                  bucket.RaycastHits = new List<RaycastHit>();
                  bucket.UpdateJobRaycast(deltaTime, localToWorld, BoxCollider, DeferredRayQuery, 0, bucket.InstanceCount);
              }
              else
              {
                  bucket.UpdateJob(deltaTime, localToWorld, 0, bucket.InstanceCount);
              }
          };
#else
            // We are on a single processor machine, so best to not multi-thread.
            if (processorCount == 1 || DisableParallelUpdate)
            {
                foreach (InstanceBucket bucket in instanceBuckets)
                {
                    if (RaycastInstances)
                    {
                        bucket.RaycastHits = new ConcurrentBag<RaycastHit>();
                        bucket.UpdateJobRaycast(deltaTime, localToWorld, BoxCollider, DeferredRayQuery, 0, bucket.InstanceCount);
                    }
                    else
                    {
                        bucket.UpdateJob(deltaTime, localToWorld, 0, bucket.InstanceCount);
                    }
                };
            }
            else if (processorCount > instanceBuckets.Count)  // More processors than buckets so split up the work within buckets.
            {
                int processorsPerBucket = Mathf.CeilToInt((float)processorCount / instanceBuckets.Count);
                int count = UNITY_MAX_INSTANCE_COUNT / processorsPerBucket;

                // Clear all bucket raycast hits before splicing the bucket.
                if (RaycastInstances)
                {
                    foreach (InstanceBucket bucket in instanceBuckets)
                    {
                        bucket.RaycastHits = new ConcurrentBag<RaycastHit>();
                    }
                }

                Parallel.For(0, instanceBuckets.Count * processorsPerBucket, (i, state) =>
                {
                    int bucketIndex = i / processorsPerBucket;
                    InstanceBucket bucket = instanceBuckets[bucketIndex];

                    int iteration = i % processorsPerBucket;
                    int firstIndex = iteration * count;

                    if (firstIndex < bucket.InstanceCount)
                    {
                        int lastIndex = firstIndex + count;

                        // Ensure the whole bucket is processed for the last iteration.
                        if (iteration == (processorsPerBucket - 1) || lastIndex > bucket.InstanceCount)
                        {
                            lastIndex = bucket.InstanceCount;
                        }

                        if (RaycastInstances)
                        {
                            bucket.UpdateJobRaycast(deltaTime, localToWorld, BoxCollider, DeferredRayQuery, firstIndex, lastIndex);
                        }
                        else
                        {
                            bucket.UpdateJob(deltaTime, localToWorld, firstIndex, lastIndex);
                        }
                    }
                });
            }
            else // More buckets than processors, so each processor gets (at least) a bucket to chew though.
            {
                // Spin up an update job for each instance bucket.
                Parallel.ForEach(instanceBuckets, (bucket) =>
                {
                    if (RaycastInstances)
                    {
                        bucket.RaycastHits = new ConcurrentBag<RaycastHit>();
                        bucket.UpdateJobRaycast(deltaTime, localToWorld, BoxCollider, DeferredRayQuery, 0, bucket.InstanceCount);
                    }
                    else
                    {
                        bucket.UpdateJob(deltaTime, localToWorld, 0, bucket.InstanceCount);
                    }
                });
            }
#endif // UNITY_WEBGL

            if (DisplayUpdateTime)
            {
                stopwatch.Stop();
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (InstanceMesh)
            {
                Gizmos.matrix = transform.localToWorldMatrix;

                if (InstanceMesh.normals.Length != 0)
                {
                    Gizmos.DrawMesh(InstanceMesh);
                }
                else if (InstanceMaterial)
                {
                    // Using Graphics.DrawMeshInstanced instead of Gizmos.DrawMesh because Gizmos.DrawMesh requires that a mesh has normals.
                    Graphics.DrawMeshInstanced(InstanceMesh, InstanceSubMeshIndex, InstanceMaterial, new Matrix4x4[1] { Gizmos.matrix }, 1, null, ShadowCastingMode, RecieveShadows);
                }

                if (RaycastInstances)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(BoxCollider.Center, BoxCollider.Size);
                }
            }
        }

        /// <summary>
        /// Creates an instance at a position.
        /// </summary>
        public Instance Instantiate(Vector3 position, bool instantiateInWorldSpace = false)
        {
            return Instantiate(Matrix4x4.TRS(position, Quaternion.identity, Vector3.one), instantiateInWorldSpace);
        }

        /// <summary>
        /// Creates an instance at a position and rotation.
        /// </summary>
        public Instance Instantiate(Vector3 position, Quaternion rotation, bool instantiateInWorldSpace = false)
        {
            return Instantiate(Matrix4x4.TRS(position, rotation, Vector3.one), instantiateInWorldSpace);
        }

        /// <summary>
        /// Creates an instance at a position, rotation, and scale.
        /// </summary>
        public Instance Instantiate(Vector3 position, Quaternion rotation, Vector3 scale, bool instantiateInWorldSpace = false)
        {
            return Instantiate(Matrix4x4.TRS(position, rotation, scale), instantiateInWorldSpace);
        }

        /// <summary>
        /// Creates an instance at TRS matrix.
        /// </summary>
        public Instance Instantiate(Matrix4x4 transformation, bool instantiateInWorldSpace = false)
        {
            Initialize();

            int instanceBucketIndex = AllocateInstanceBucketIndex();
            int instanceIndex = AllocateInstanceIndex(instanceBucketIndex);

            InstanceBucket bucket = instanceBuckets[instanceBucketIndex];
            bucket.Instances[instanceIndex] = new Instance(instanceIndex, instanceBucketIndex, null, this);
            bucket.Matricies[instanceIndex] = (instantiateInWorldSpace) ? transform.worldToLocalMatrix * transformation : transformation;

            ++InstanceCount;

            return bucket.Instances[instanceIndex];
        }

        /// <summary>
        /// Removes all instances from the MeshInstancer.
        /// </summary>
        public void Clear()
        {
            // Destroy in reverse order to avoid array compacting.
            for (int i = (instanceBuckets.Count - 1); i >= 0; --i)
            {
                for (int j = (instanceBuckets[i].InstanceCount - 1); j >= 0; --j)
                {
                    instanceBuckets[i].Instances[j].Destroy();
                }
            }

            instanceBuckets.Clear();
        }

        /// <summary>
        /// Registers a float material property that can be updated at runtime per instance.
        /// </summary>
        public bool RegisterMaterialProperty(string name, float defaultValue)
        {
            return RegisterMaterialPropertyCommonFloat(Shader.PropertyToID(name), defaultValue);
        }

        /// <summary>
        /// Registers a float material property that can be updated at runtime per instance.
        /// </summary>
        public bool RegisterMaterialProperty(int nameID, float defaultValue)
        {
            return RegisterMaterialPropertyCommonFloat(nameID, defaultValue);
        }

        /// <summary>
        /// Registers a vector material property that can be updated at runtime per instance.
        /// </summary>
        public bool RegisterMaterialProperty(string name, Vector4 defaultValue)
        {
            return RegisterMaterialPropertyCommonVector(Shader.PropertyToID(name), defaultValue);
        }

        /// <summary>
        /// Registers a vector material property that can be updated at runtime per instance.
        /// </summary>
        public bool RegisterMaterialProperty(int nameID, Vector4 defaultValue)
        {
            return RegisterMaterialPropertyCommonVector(nameID, defaultValue);
        }

        /// <summary>
        /// Registers a float matrix property that can be updated at runtime per instance.
        /// </summary>
        public bool RegisterMaterialProperty(string name, Matrix4x4 defaultValue)
        {
            return RegisterMaterialPropertyCommonMatrix(Shader.PropertyToID(name), defaultValue);
        }

        /// <summary>
        /// Registers a matrix material property that can be updated at runtime per instance.
        /// </summary>
        public bool RegisterMaterialProperty(int nameID, Matrix4x4 defaultValue)
        {
            return RegisterMaterialPropertyCommonMatrix(nameID, defaultValue);
        }

        /// <summary>
        /// Returns the hit thats the smallest distance away from the DeferredRayQuery origin.
        /// </summary>
        public bool GetClosestRaycastHit(ref RaycastHit hit)
        {
            Initialize();

            hit.Reset();

            foreach (RaycastHit h in DeferredRaycastHits)
            {
                if (h.Distance < hit.Distance)
                {
                    hit = h;
                }
            }

            return (hit.Instance != null);
        }

        private bool RegisterMaterialPropertyCommonFloat(int nameID, float defaultValue, bool initialize = true)
        {
            if (initialize)
            {
                Initialize();
            }

            if (floatMaterialProperties.Exists(element => (element.Key == nameID)))
            {
                Debug.LogWarningFormat("RegisterMaterialProperty failed because {0} has already been registered.", nameID);

                return false;
            }

            floatMaterialProperties.Add(new KeyValuePair<int, float>(nameID, defaultValue));

            foreach (var bucket in instanceBuckets)
            {
                bucket.RegisterMaterialPropertiesFloat(floatMaterialProperties);
            }

            return true;
        }

        private bool RegisterMaterialPropertyCommonVector(int nameID, Vector4 defaultValue, bool initialize = true)
        {
            if (initialize)
            {
                Initialize();
            }

            if (vectorMaterialProperties.Exists(element => (element.Key == nameID)))
            {
                Debug.LogWarningFormat("RegisterMaterialProperty failed because {0} has already been registered.", nameID);

                return false;
            }

            vectorMaterialProperties.Add(new KeyValuePair<int, Vector4>(nameID, defaultValue));

            foreach (var bucket in instanceBuckets)
            {
                bucket.RegisterMaterialPropertiesVector(vectorMaterialProperties);
            }

            return true;
        }

        private bool RegisterMaterialPropertyCommonMatrix(int nameID, Matrix4x4 defaultValue, bool initialize = true)
        {
            if (initialize)
            {
                Initialize();
            }

            if (matrixMaterialProperties.Exists(element => (element.Key == nameID)))
            {
                Debug.LogWarningFormat("RegisterMaterialProperty failed because {0} has already been registered.", nameID);

                return false;
            }

            matrixMaterialProperties.Add(new KeyValuePair<int, Matrix4x4>(nameID, defaultValue));

            foreach (var bucket in instanceBuckets)
            {
                bucket.RegisterMaterialPropertiesMatrix(matrixMaterialProperties);
            }

            return true;
        }

        private void SetFloat(Instance instance, int nameID, float value)
        {
            Initialize();

            if (!floatMaterialProperties.Exists(element => (element.Key == nameID)))
            {
                Debug.LogWarningFormat("SetFloat failed because {0} is not a registered material property on the MeshInstancer.", nameID);

                return;
            }

            InstanceBucket bucket = instanceBuckets[instance.InstanceBucketIndex];
            float[] values = bucket.Properties.GetFloatArray(nameID);
            values[instance.InstanceIndex] = value;
            bucket.Properties.SetFloatArray(nameID, values);
        }

        private float GetFloat(Instance instance, int nameID)
        {
            Initialize();

            if (!floatMaterialProperties.Exists(element => (element.Key == nameID)))
            {
                Debug.LogWarningFormat("GetFloat failed because {0} is not a registered material property on the MeshInstancer.", nameID);

                return 0.0f;
            }

            InstanceBucket bucket = instanceBuckets[instance.InstanceBucketIndex];
            float[] values = bucket.Properties.GetFloatArray(nameID);
            return values[instance.InstanceIndex];
        }

        private void SetVector(Instance instance, int nameID, Vector4 value)
        {
            Initialize();

            if (!vectorMaterialProperties.Exists(element => (element.Key == nameID)))
            {
                Debug.LogWarningFormat("SetVector failed because {0} is not a registered material property on the MeshInstancer.", nameID);

                return;
            }

            InstanceBucket bucket = instanceBuckets[instance.InstanceBucketIndex];
            Vector4[] values = bucket.Properties.GetVectorArray(nameID);
            values[instance.InstanceIndex] = value;
            bucket.Properties.SetVectorArray(nameID, values);
        }

        private Vector4 GetVector(Instance instance, int nameID)
        {
            Initialize();

            if (!vectorMaterialProperties.Exists(element => (element.Key == nameID)))
            {
                Debug.LogWarningFormat("GetVector failed because {0} is not a registered material property on the MeshInstancer.", nameID);

                return Vector4.zero;
            }

            InstanceBucket bucket = instanceBuckets[instance.InstanceBucketIndex];
            Vector4[] values = bucket.Properties.GetVectorArray(nameID);
            return values[instance.InstanceIndex];
        }

        private void SetMatrix(Instance instance, int nameID, Matrix4x4 value)
        {
            Initialize();

            if (!matrixMaterialProperties.Exists(element => (element.Key == nameID)))
            {
                Debug.LogWarningFormat("SetMatrix failed because {0} is not a registered material property on the MeshInstancer.", nameID);

                return;
            }

            InstanceBucket bucket = instanceBuckets[instance.InstanceBucketIndex];
            Matrix4x4[] values = bucket.Properties.GetMatrixArray(nameID);
            values[instance.InstanceIndex] = value;
            bucket.Properties.SetMatrixArray(nameID, values);
        }

        private Matrix4x4 GetMatrix(Instance instance, int nameID)
        {
            Initialize();

            if (!matrixMaterialProperties.Exists(element => (element.Key == nameID)))
            {
                Debug.LogWarningFormat("GetMatrix failed because {0} is not a registered material property on the MeshInstancer.", nameID);

                return Matrix4x4.identity;
            }

            InstanceBucket bucket = instanceBuckets[instance.InstanceBucketIndex];
            Matrix4x4[] values = bucket.Properties.GetMatrixArray(nameID);
            return values[instance.InstanceIndex];
        }

        private void Destroy(Instance instance)
        {
            // TODO - [Cameron-Micka] Defragment InstanceBuckets when instance bucket count becomes low across the list?

            InstanceBucket bucket = instanceBuckets[instance.InstanceBucketIndex];
            int newInstanceCount = bucket.InstanceCount - 1;

            // If this is the last instance of the date, remove the whole instance bucket.
            if (newInstanceCount == 0)
            {
                instanceBuckets.RemoveAt(instance.InstanceBucketIndex);

                --InstanceCount;

                return;
            }

            // If this is not the last instance, move the last instance to the place of the recently destroyed instance.
            if (newInstanceCount != instance.InstanceIndex)
            {
                // Update the transformation and instance reference.
                bucket.Matricies[instance.InstanceIndex] = bucket.Matricies[newInstanceCount];
                bucket.Instances[instance.InstanceIndex] = new Instance(instance.InstanceIndex, 
                                                                        bucket.Instances[newInstanceCount].InstanceBucketIndex, 
                                                                        bucket.Instances[newInstanceCount].UserData, 
                                                                        this);
                bucket.Instances[newInstanceCount] = null;

                // Update material properties.
                foreach (var property in floatMaterialProperties)
                {
                    float[] values = bucket.Properties.GetFloatArray(property.Key);
                    values[instance.InstanceIndex] = values[newInstanceCount];
                    bucket.Properties.SetFloatArray(property.Key, values);
                }

                foreach (var property in vectorMaterialProperties)
                {
                    Vector4[] values = bucket.Properties.GetVectorArray(property.Key);
                    values[instance.InstanceIndex] = values[newInstanceCount];
                    bucket.Properties.SetVectorArray(property.Key, values);
                }

                foreach (var property in matrixMaterialProperties)
                {
                    Matrix4x4[] values = bucket.Properties.GetMatrixArray(property.Key);
                    values[instance.InstanceIndex] = values[newInstanceCount];
                    bucket.Properties.SetMatrixArray(property.Key, values);
                }

                // Update the parallel update delegate.
                bucket.ParallelUpdates[instance.InstanceIndex] = bucket.ParallelUpdates[newInstanceCount];
            }

            bucket.InstanceCount = newInstanceCount;

            --InstanceCount;
        }

        private int AllocateInstanceBucketIndex()
        {
            for (int i = 0; i < instanceBuckets.Count; ++i)
            {
                if (instanceBuckets[i].InstanceCount < instanceBuckets[i].Matricies.Length)
                {
                    return i;
                }
            }

            instanceBuckets.Add(new InstanceBucket(floatMaterialProperties,
                                                   vectorMaterialProperties,
                                                   matrixMaterialProperties));

            return (instanceBuckets.Count - 1);
        }

        private int AllocateInstanceIndex(int instanceBucketIndex)
        {
            int instanceIndex = instanceBuckets[instanceBucketIndex].InstanceCount;
            ++instanceBuckets[instanceBucketIndex].InstanceCount;

            return instanceIndex;
        }
    }
}

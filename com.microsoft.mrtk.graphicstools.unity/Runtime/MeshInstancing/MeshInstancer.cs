// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

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
        /// TODO
        /// </summary>
        [Header("Visuals")]
        [Tooltip("TODO")]
        public Mesh InstanceMesh = null;

        /// <summary>
        /// TODO
        /// </summary>
        [Tooltip("TODO")]
        public int InstanceSubMeshIndex = 0;

        /// <summary>
        /// TODO
        /// </summary>
        [Tooltip("TODO")]
        public Material InstanceMaterial = null;

        /// <summary>
        /// TODO
        /// </summary>
        [Tooltip("TODO")]
        public UnityEngine.Rendering.ShadowCastingMode ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        /// <summary>
        /// TODO
        /// </summary>
        [Tooltip("TODO")]
        public bool RecieveShadows = false;

        [Serializable]
        private class FloatMaterialProperty
        {
            public string Name = null;
            public float DefaultValue = 0.0f;
        }

        [SerializeField, Tooltip("TODO")]
        private FloatMaterialProperty[] FloatMaterialProperties = new FloatMaterialProperty[0];

        [Serializable]
        private class VectorMaterialProperty
        {
            public string Name = null;
            public Vector4 DefaultValue = Vector3.zero;
        }

        [SerializeField, Tooltip("TODO")]
        private VectorMaterialProperty[] VectorMaterialProperties = new VectorMaterialProperty[0];

        [Serializable]
        private class MatrixMaterialProperty
        {
            public string Name = null;
            public Matrix4x4 DefaultValue = Matrix4x4.identity;
        }

        [SerializeField, Tooltip("TODO")]
        private MatrixMaterialProperty[] MatrixMaterialProperties = new MatrixMaterialProperty[0];

        /// <summary>
        /// TODO
        /// </summary>
        [Header("Physics")]
        [Tooltip("TODO")]
        public bool RaycastInstances = false;

        /// <summary>
        /// TODO
        /// </summary>
        [Serializable]
        public struct Box
        {
            public Vector3 Center;
            public Vector3 Size;
        }

        /// <summary>
        /// TODO
        /// </summary>
        public struct RaycastHit
        {
            public Instance Instance;
            public float Distance;
            public Vector3 Point;
            public Vector3 Direction;
        }

        /// <summary>
        /// TODO
        /// </summary>
        public Box BoxCollider = new Box() { Center = Vector3.zero, Size = Vector3.one };

        /// <summary>
        /// TODO
        /// </summary>
        public Ray RayCollider { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public List<RaycastHit> RaycastHits { get; private set; }

        /// <summary>
        /// TODO
        /// </summary>
        public delegate void ParallelUpdate(float deltaTime, Instance instance);

        /// <summary>
        /// TODO
        /// </summary>
        [Header("Diagnostics")]
        [Tooltip("TODO")]
        public bool DisplayUpdateTime = false;

        /// <summary>
        /// TODO
        /// </summary>
        [Tooltip("TODO")]
        public bool DisableParallelUpdate = false;

        /// <summary>
        /// TODO
        /// </summary>
        public class Instance
        {
            /// <summary>
            /// TODO
            /// </summary>
            public int InstanceIndex { get; set; }

            /// <summary>
            /// TODO
            /// </summary>
            public int InstanceBucketIndex { get; private set; }

            /// <summary>
            /// TODO
            /// </summary>
            public System.Object UserData { get; set; }

            /// <summary>
            /// TODO
            /// </summary>
            public Vector3 Position
            {
                get { return Transformation.GetColumn(3); }
                set { Transformation = Matrix4x4.TRS(value, Rotation, LossyScale); }
            }

            /// <summary>
            /// TODO
            /// </summary>
            public Vector3 LocalPosition
            {
                get { return LocalTransformation.GetColumn(3); }
                set { LocalTransformation = Matrix4x4.TRS(value, LocalRotation, LocalScale); }
            }

            /// <summary>
            /// TODO
            /// </summary>
            public Quaternion Rotation
            {
                get { Matrix4x4 matrix = Transformation; return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1)); }
                set { Transformation = Matrix4x4.TRS(Position, value, LossyScale); }
            }

            /// <summary>
            /// TODO
            /// </summary>
            public Quaternion LocalRotation
            {
                get { Matrix4x4 matrix = LocalTransformation; return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1)); }
                set { LocalTransformation = Matrix4x4.TRS(LocalPosition, value, LocalScale); }
            }

            /// <summary>
            /// TODO
            /// </summary>
            public Vector3 LossyScale
            {
                get { Matrix4x4 matrix = Transformation; return new Vector3(matrix.GetColumn(0).magnitude, matrix.GetColumn(1).magnitude, matrix.GetColumn(2).magnitude); }
            }

            /// <summary>
            /// TODO
            /// </summary>
            public Vector3 LocalScale
            {
                get { Matrix4x4 matrix = LocalTransformation; return new Vector3(matrix.GetColumn(0).magnitude, matrix.GetColumn(1).magnitude, matrix.GetColumn(2).magnitude); }
                set { LocalTransformation = Matrix4x4.TRS(LocalPosition, LocalRotation, value); }
            }

            /// <summary>
            /// TODO
            /// </summary>
            public Matrix4x4 Transformation
            {
                get { return meshInstancer.transform.localToWorldMatrix * LocalTransformation; }
                set { LocalTransformation = meshInstancer.transform.worldToLocalMatrix * value; }
            }

            /// <summary>
            /// TODO
            /// </summary>
            public Matrix4x4 LocalTransformation
            {
                get { return meshInstancer.instanceBuckets[InstanceBucketIndex].Matricies[InstanceIndex]; }
                set { meshInstancer.instanceBuckets[InstanceBucketIndex].Matricies[InstanceIndex] = value; }
            }

            /// <summary>
            /// TODO
            /// </summary>
            public bool Destroyed
            {
                get { return (meshInstancer == null); }
            }

            private MeshInstancer meshInstancer;

            private Instance() { }

            /// <summary>
            /// TODO
            /// </summary>
            public Instance(int instanceIndex, int instanceBucketIndex, MeshInstancer instancer)
            {
                InstanceIndex = instanceIndex;
                InstanceBucketIndex = instanceBucketIndex;
                meshInstancer = instancer;
            }

            /// <summary>
            /// TODO
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
            /// TODO
            /// </summary>
            public void SetFloat(string name, float value)
            {
                SetFloat(Shader.PropertyToID(name), value);
            }

            /// <summary>
            /// TODO
            /// </summary>
            public void SetFloat(int nameID, float value)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to SetFloat on a destroyed MeshInstancer instance."); return; }

                meshInstancer.SetFloat(this, nameID, value);
            }

            /// <summary>
            /// TODO
            /// </summary>
            public float GetFloat(string name)
            {
                return GetFloat(Shader.PropertyToID(name));
            }

            /// <summary>
            /// TODO
            /// </summary>
            public float GetFloat(int nameID)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to GetFloat on a destroyed MeshInstancer instance."); return 0.0f; }

                return meshInstancer.GetFloat(this, nameID);
            }

            /// <summary>
            /// TODO
            /// </summary>
            public void SetVector(string name, Vector4 value)
            {
                SetVector(Shader.PropertyToID(name), value);
            }

            /// <summary>
            /// TODO
            /// </summary>
            public void SetVector(int nameID, Vector4 value)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to SetVector on a destroyed MeshInstancer instance."); return; }

                meshInstancer.SetVector(this, nameID, value);
            }

            /// <summary>
            /// TODO
            /// </summary>
            public Vector4 GetVector(string name)
            {
                return GetVector(Shader.PropertyToID(name));
            }

            /// <summary>
            /// TODO
            /// </summary>
            public Vector4 GetVector(int nameID)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to GetVector on a destroyed MeshInstancer instance."); return Vector4.zero; }

                return meshInstancer.GetVector(this, nameID);
            }

            /// <summary>
            /// TODO
            /// </summary>
            public void SetMatrix(string name, Matrix4x4 value)
            {
                SetMatrix(Shader.PropertyToID(name), value);
            }

            /// <summary>
            /// TODO
            /// </summary>
            public void SetMatrix(int nameID, Matrix4x4 value)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to SetMatrix on a destroyed MeshInstancer instance."); return; }

                meshInstancer.SetMatrix(this, nameID, value);
            }

            /// <summary>
            /// TODO
            /// </summary>
            public Matrix4x4 GetMatrix(string name)
            {
                return GetMatrix(Shader.PropertyToID(name));
            }

            /// <summary>
            /// TODO
            /// </summary>
            public Matrix4x4 GetMatrix(int nameID)
            {
                if (Destroyed) { Debug.LogWarning("Attempting to v on a destroyed MeshInstancer instance."); return Matrix4x4.identity; }

                return meshInstancer.GetMatrix(this, nameID);
            }

            /// <summary>
            /// TODO
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
            public List<RaycastHit> RaycastHits = new List<RaycastHit>(UNITY_MAX_INSTANCE_COUNT);

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
                    Properties.SetFloatArray(property.Key, Enumerable.Repeat(property.Value, UNITY_MAX_INSTANCE_COUNT).ToArray());
                }
            }

            public void RegisterMaterialPropertiesVector(List<KeyValuePair<int, Vector4>> materialProperties)
            {
                foreach (var property in materialProperties)
                {
                    Properties.SetVectorArray(property.Key, Enumerable.Repeat(property.Value, UNITY_MAX_INSTANCE_COUNT).ToArray());
                }
            }

            public void RegisterMaterialPropertiesMatrix(List<KeyValuePair<int, Matrix4x4>> materialProperties)
            {
                foreach (var property in materialProperties)
                {
                    Properties.SetMatrixArray(property.Key, Enumerable.Repeat(property.Value, UNITY_MAX_INSTANCE_COUNT).ToArray());
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
                RaycastHits.Clear();

                for (int i = firstIndex; i < lastIndex; ++i)
                {
                    // Apply any per instance logic.
                    ParallelUpdates[i]?.Invoke(deltaTime, Instances[i]);

                    // Calculate the final transformation matrix.
                    matrixScratchBuffer[i] = localToWorld * Matricies[i];

                    // Perform a ray cast against the current instance.
                    RaycastHit hitInfo;

                    if (RaycastBox(matrixScratchBuffer[i], boxMin, boxMax, ray, out hitInfo))
                    {
                        hitInfo.Instance = Instances[i];
                        hitInfo.Point = ray.origin + ray.direction * hitInfo.Distance;
                        hitInfo.Direction = -ray.direction;
                        RaycastHits.Add(hitInfo);
                    }
                }
            }

            public void Draw(Mesh mesh, int submeshIndex, Material material, UnityEngine.Rendering.ShadowCastingMode shadowCastingMode, bool recieveShadows)
            {
                Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrixScratchBuffer, InstanceCount, Properties, shadowCastingMode, recieveShadows);
            }

            private bool RaycastBox(Matrix4x4 localToWorld, Vector3 boxMin, Vector3 boxMax, Ray ray, out RaycastHit hitInfo)
            {
                // Fast, Branch-less Ray/Bounding Box Intersections.
                // https://tavianator.com/fast-branchless-raybounding-box-intersections/

                hitInfo = new RaycastHit();

                // Transform the ray into the instance's local space.
                Matrix4x4 localToWorldInverse = localToWorld.inverse;
                ray.origin = localToWorldInverse.MultiplyPoint(ray.origin);
                ray.direction = localToWorldInverse.MultiplyVector(ray.direction);
                Vector3 rayDirectionInverse = new Vector3(1.0f / ray.direction.x, 1.0f / ray.direction.y, 1.0f / ray.direction.z);

                float t1 = (boxMin.x - ray.origin.x) * rayDirectionInverse.x;
                float t2 = (boxMax.x - ray.origin.x) * rayDirectionInverse.x;

                float tMin = Math.Min(t1, t2);
                float tMax = Math.Max(t1, t2);

                for (int i = 1; i < 3; ++i)
                {
                    t1 = (boxMin[i] - ray.origin[i]) * rayDirectionInverse[i];
                    t2 = (boxMax[i] - ray.origin[i]) * rayDirectionInverse[i];

                    tMin = Math.Max(tMin, Math.Min(t1, t2));
                    tMax = Math.Min(tMax, Math.Max(t1, t2));
                }

                bool hit = tMax > Math.Max(tMin, 0.0f);

                if (hit)
                {
                    // TODO - [Cameron-Micka] Calculate correct intersection time for non-uniformly scaled boxes.
                    hitInfo.Distance = (tMin < 0) ? tMax : tMin;
                }

                return hit;
            }
        }

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
            // Register all properties specified in the component properties.
            foreach (FloatMaterialProperty property in FloatMaterialProperties)
            {
                RegisterMaterialProperty(property.Name, property.DefaultValue);
            }

            foreach (VectorMaterialProperty property in VectorMaterialProperties)
            {
                RegisterMaterialProperty(property.Name, property.DefaultValue);
            }

            foreach (MatrixMaterialProperty property in MatrixMaterialProperties)
            {
                RegisterMaterialProperty(property.Name, property.DefaultValue);
            }

            RaycastHits = new List<RaycastHit>();
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void LateUpdate()
        {
            RaycastHits.Clear();

            UpdateBuckets();

            foreach (var bucket in instanceBuckets)
            {
                // Draw each instance bucket.
                bucket.Draw(InstanceMesh, InstanceSubMeshIndex, InstanceMaterial, ShadowCastingMode, RecieveShadows);

                // Collect the aggregate raycast hits.
                if (RaycastInstances)
                {
                    RaycastHits.AddRange(bucket.RaycastHits);
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

                string label = string.Format("MeshInstancer Update: {0:f2} ms", averageElapsedMilliseconds);
                GUI.Label(new Rect(10.0f, Screen.height - 24.0f, Screen.height, 128.0f), label);
            }
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
            processorCount = 1;
#endif
            // We are on a single processor machine, so best to not multi-thread.
            if (processorCount == 1 || DisableParallelUpdate)
            {
                foreach (InstanceBucket bucket in instanceBuckets)
                {
                    if (RaycastInstances)
                    {
                        bucket.UpdateJobRaycast(deltaTime, localToWorld, BoxCollider, RayCollider, 0, bucket.InstanceCount);
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

                Parallel.For(0, instanceBuckets.Count * processorsPerBucket, (i, state) =>
                {
                    int bucketIndex = i / processorsPerBucket;
                    InstanceBucket bucket = instanceBuckets[bucketIndex];

                    int iteration = i % processorsPerBucket;
                    int firstIndex = iteration * count;

                    if (firstIndex < bucket.InstanceCount)
                    {
                        // Ensure the whole bucket is processed for the last iteration.
                        int lastIndex = (iteration == (processorsPerBucket - 1)) ? bucket.InstanceCount : firstIndex + count;

                        if (RaycastInstances)
                        {
                            bucket.UpdateJobRaycast(deltaTime, localToWorld, BoxCollider, RayCollider, firstIndex, lastIndex);
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
                        bucket.UpdateJobRaycast(deltaTime, localToWorld, BoxCollider, RayCollider, 0, bucket.InstanceCount);
                    }
                    else
                    {
                        bucket.UpdateJob(deltaTime, localToWorld, 0, bucket.InstanceCount);
                    }
                });
            }

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
        /// TODO
        /// </summary>
        public Instance Instantiate(Vector3 position, bool instantiateInWorldSpace = false)
        {
            return Instantiate(Matrix4x4.TRS(position, Quaternion.identity, Vector3.one), instantiateInWorldSpace);
        }

        /// <summary>
        /// TODO
        /// </summary>
        public Instance Instantiate(Vector3 position, Quaternion rotation, bool instantiateInWorldSpace = false)
        {
            return Instantiate(Matrix4x4.TRS(position, rotation, Vector3.one), instantiateInWorldSpace);
        }

        /// <summary>
        /// TODO
        /// </summary>
        public Instance Instantiate(Vector3 position, Quaternion rotation, Vector3 scale, bool instantiateInWorldSpace = false)
        {
            return Instantiate(Matrix4x4.TRS(position, rotation, scale), instantiateInWorldSpace);
        }

        /// <summary>
        /// TODO
        /// </summary>
        public Instance Instantiate(Matrix4x4 transformation, bool instantiateInWorldSpace = false)
        {
            int instanceBucketIndex = AllocateInstanceBucketIndex();
            int instanceIndex = AllocateInstanceIndex(instanceBucketIndex);

            InstanceBucket bucket = instanceBuckets[instanceBucketIndex];
            bucket.Instances[instanceIndex] = new Instance(instanceIndex, instanceBucketIndex, this);
            bucket.Matricies[instanceIndex] = (instantiateInWorldSpace) ? transform.worldToLocalMatrix * transformation : transformation;

            return bucket.Instances[instanceIndex];
        }

        /// <summary>
        /// TODO
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
        /// TODO
        /// </summary>
        public bool RegisterMaterialProperty(string name, float defaultValue)
        {
            return RegisterMaterialPropertyCommonFloat(Shader.PropertyToID(name), defaultValue);
        }

        /// <summary>
        /// TODO
        /// </summary>
        public bool RegisterMaterialProperty(int nameID, float defaultValue)
        {
            return RegisterMaterialPropertyCommonFloat(nameID, defaultValue);
        }

        /// <summary>
        /// TODO
        /// </summary>
        public bool RegisterMaterialProperty(string name, Vector4 defaultValue)
        {
            return RegisterMaterialPropertyCommonVector(Shader.PropertyToID(name), defaultValue);
        }

        /// <summary>
        /// TODO
        /// </summary>
        public bool RegisterMaterialProperty(int nameID, Vector4 defaultValue)
        {
            return RegisterMaterialPropertyCommonVector(nameID, defaultValue);
        }

        /// <summary>
        /// TODO
        /// </summary>
        public bool RegisterMaterialProperty(string name, Matrix4x4 defaultValue)
        {
            return RegisterMaterialPropertyCommonMatrix(Shader.PropertyToID(name), defaultValue);
        }

        /// <summary>
        /// TODO
        /// </summary>
        public bool RegisterMaterialProperty(int nameID, Matrix4x4 defaultValue)
        {
            return RegisterMaterialPropertyCommonMatrix(nameID, defaultValue);
        }

        private bool RegisterMaterialPropertyCommonFloat(int nameID, float defaultValue)
        {
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

        private bool RegisterMaterialPropertyCommonVector(int nameID, Vector4 defaultValue)
        {
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

        private bool RegisterMaterialPropertyCommonMatrix(int nameID, Matrix4x4 defaultValue)
        {
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

                return;
            }

            // If this is not the last instance, move the last instance to the place of the recently destroyed instance.
            if (newInstanceCount != instance.InstanceIndex)
            {
                // Update the transformation and instance reference.
                bucket.Matricies[instance.InstanceIndex] = bucket.Matricies[newInstanceCount];
                bucket.Instances[instance.InstanceIndex] = bucket.Instances[newInstanceCount];
                bucket.Instances[instance.InstanceIndex].InstanceIndex = instance.InstanceIndex;
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

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using UnityEditor;
using UnityEngine;
using MaterialValue = System.Tuple<object, bool>;
using MaterialSettings = System.Collections.Generic.Dictionary<string, System.Tuple<object, bool>>;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// A custom inspector for BaseMeshOutline. Used for create or fix and outline material.
    /// </summary>
    [CustomEditor(typeof(BaseMeshOutline), true)]
    public class BaseMeshOutlineInspector : UnityEditor.Editor
    {
        private BaseMeshOutline instance;
        private SerializedProperty m_Script;
        private SerializedProperty outlineMaterial;
        private SerializedProperty outlineWidth;
        private SerializedProperty useStencilOutline;
        private SerializedProperty stencilWriteMaterial;
        private SerializedProperty outlineMargin;

        private readonly MaterialSettings defaultOutlineMaterialSettings = new MaterialSettings()
        {
            { "_Mode", new MaterialValue(5.0f, true) },
            { "_CustomMode", new MaterialValue(0.0f, true) },
            { "_ZWrite", new MaterialValue(0.0f, true) },

            { "_Color", new MaterialValue(Color.green, false) },

            { "_DirectionalLight", new MaterialValue((float)LightMode.Unlit, false) },
            { "_DirectionalLightProxy", new MaterialValue(0.0f, false) },
            { "_DIRECTIONAL_LIGHT", new MaterialValue(false, false) },

            { "_VertexExtrusion", new MaterialValue(1.0f, true) },
            { "_VERTEX_EXTRUSION", new MaterialValue(true, true) },

            { "_VertexExtrusionSmoothNormals", new MaterialValue(1.0f, true) },
            { "_VERTEX_EXTRUSION_SMOOTH_NORMALS", new MaterialValue(true, true) },
        };

        private readonly MaterialSettings defaultOutlineWithStencilMaterialSettings = new MaterialSettings()
        {
            { "_Mode", new MaterialValue(5.0f, true) },
            { "_CustomMode", new MaterialValue(0.0f, true) },
            { "_ZWrite", new MaterialValue(1.0f, true) },

            { "_Color", new MaterialValue(Color.green, false) },

            { "_DirectionalLight", new MaterialValue((float)LightMode.Unlit, false) },
            { "_DirectionalLightProxy", new MaterialValue(0.0f, false) },
            { "_DIRECTIONAL_LIGHT", new MaterialValue(false, false) },

            { "_VertexExtrusion", new MaterialValue(1.0f, true) },
            { "_VERTEX_EXTRUSION", new MaterialValue(true, true) },

            { "_VertexExtrusionSmoothNormals", new MaterialValue(1.0f, true) },
            { "_VERTEX_EXTRUSION_SMOOTH_NORMALS", new MaterialValue(true, true) },

            { "_EnableStencil", new MaterialValue(1.0f, true) },
            { "_STENCIL", new MaterialValue(true, true) },
            { "_StencilReference", new MaterialValue(1.0f, true) },
            { "_StencilComparison", new MaterialValue((float)UnityEngine.Rendering.CompareFunction.NotEqual, true) },
            { "_StencilOperation", new MaterialValue((float)UnityEngine.Rendering.StencilOp.Keep, true) },
        };

        private readonly MaterialSettings defaultStencilMaterialSettings = new MaterialSettings()
        {
            { "_Mode", new MaterialValue(5.0f, true) },
            { "_CustomMode", new MaterialValue(0.0f, true) },
            { "_ZWrite", new MaterialValue(0.0f, true) },
            { "_ColorWriteMask", new MaterialValue(0.0f, true) },

            { "_DirectionalLight", new MaterialValue((float)LightMode.Unlit, false) },
            { "_DirectionalLightProxy", new MaterialValue(0.0f, false) },
            { "_DIRECTIONAL_LIGHT", new MaterialValue(false, false) },

            { "_VertexExtrusion", new MaterialValue(1.0f, true) },
            { "_VERTEX_EXTRUSION", new MaterialValue(true, true) },

            { "_VertexExtrusionSmoothNormals", new MaterialValue(1.0f, true) },
            { "_VERTEX_EXTRUSION_SMOOTH_NORMALS", new MaterialValue(true, true) },

            { "_EnableStencil", new MaterialValue(1.0f, true) },
            { "_STENCIL", new MaterialValue(true, true) },
            { "_StencilReference", new MaterialValue(1.0f, true) },
            { "_StencilComparison", new MaterialValue((float)UnityEngine.Rendering.CompareFunction.Always, true) },
            { "_StencilOperation", new MaterialValue((float)UnityEngine.Rendering.StencilOp.Replace, true) },
        };

        private void OnEnable()
        {
            instance = target as BaseMeshOutline;
            m_Script = serializedObject.FindProperty("m_Script");
            outlineMaterial = serializedObject.FindProperty(nameof(outlineMaterial));
            outlineWidth = serializedObject.FindProperty(nameof(outlineWidth));
            useStencilOutline = serializedObject.FindProperty(nameof(useStencilOutline));
            stencilWriteMaterial = serializedObject.FindProperty(nameof(stencilWriteMaterial));
            outlineMargin = serializedObject.FindProperty(nameof(outlineMargin));
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            DrawReadonlyPropertyField(m_Script);

            EditorGUI.BeginChangeCheck();

            // Draw other properties
            DrawPropertiesExcluding(serializedObject, nameof(m_Script), 
                                                      nameof(outlineMaterial), 
                                                      nameof(outlineWidth), 
                                                      nameof(useStencilOutline), 
                                                      nameof(stencilWriteMaterial), 
                                                      nameof(outlineMargin));

            EditorGUILayout.PropertyField(outlineMaterial);

            var outlineMaterialSettings = useStencilOutline.boolValue ? defaultOutlineWithStencilMaterialSettings : defaultOutlineMaterialSettings;

            VerifyMaterial(outlineMaterial, instance.OutlineMaterial, outlineMaterialSettings, "Outline.mat");

            EditorGUILayout.PropertyField(outlineWidth);

            EditorGUILayout.PropertyField(useStencilOutline);

            if (useStencilOutline.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(stencilWriteMaterial);
                EditorGUILayout.PropertyField(outlineMargin);
                EditorGUI.indentLevel--;

                VerifyMaterial(stencilWriteMaterial, instance.StencilWriteMaterial, defaultStencilMaterialSettings, "OutlineStencilWrite.mat");
            }
            else
            {
                if (Camera.main && Camera.main.clearFlags == CameraClearFlags.Skybox)
                {
                    EditorGUILayout.HelpBox("The main camera is using a skybox, for outlines to appear it is recommended stencil outlines be enabled.", MessageType.Info);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                instance.ApplyOutlineMaterial();
                instance.ApplyOutlineWidth();
            }
        }

        private static void VerifyMaterial(SerializedProperty property, Material material, MaterialSettings materialSettings, string postfix)
        {
            if (material == null)
            {
                EditorGUILayout.HelpBox($"{property.displayName} field is empty, please create or select a material.", MessageType.Warning);

                if (GUILayout.Button("Create New Material"))
                {
                    property.objectReferenceValue = CreateNewMaterial(materialSettings, postfix);
                }
            }
            else if (!IsCorrectMaterial(material, materialSettings))
            {
                EditorGUILayout.HelpBox("Material may not be configured correctly, please check or reset to default.", MessageType.Info);

                if (GUILayout.Button("Update Material Settings to Default"))
                {
                    ForceUpdateToDefaultMaterial(material, materialSettings, false);
                }
            }
        }

        private static Material CreateNewMaterial(MaterialSettings materialSettings, string postfix)
        {
            var material = new Material(StandardShaderUtility.GraphicsToolsStandardShader);
            ForceUpdateToDefaultMaterial(material, materialSettings, true);
            AssetDatabase.CreateAsset(material, AssetDatabase.GenerateUniqueAssetPath($"Assets/{Selection.activeGameObject.name}{postfix}"));

            return material;
        }

        private static void ForceUpdateToDefaultMaterial(Material material, MaterialSettings materialSettings, bool isNewMaterial)
        {
            if (!StandardShaderUtility.IsUsingGraphicsToolsStandardShader(material))
            {
                material.shader = StandardShaderUtility.GraphicsToolsStandardShader;
            }

            foreach (var pair in materialSettings)
            {
                // Apply the setting if it is required or we are creating a new material.
                if (pair.Value.Item2 || isNewMaterial)
                {
                    var typeName = pair.Value.Item1.GetType().Name;
                    switch (typeName)
                    {
                        case nameof(System.Single):
                            {
                                material.SetFloat(pair.Key, (float)pair.Value.Item1);
                            }
                            break;
                        case nameof(System.Boolean):
                            {
                                var val = (bool)pair.Value.Item1;
                                if (val)
                                {
                                    material.EnableKeyword(pair.Key);
                                }
                                else
                                {
                                    material.DisableKeyword(pair.Key);
                                }
                            }
                            break;
                        case nameof(Color):
                            {
                                material.SetColor(pair.Key, (Color)pair.Value.Item1);
                            }
                            break;
                        default:
                            Debug.Log($"{pair.Key} of type {typeName} was not handled.");
                            break;
                    }
                }
            }
        }

        private static bool IsCorrectMaterial(Material material, MaterialSettings materialSettings)
        {
            if (!StandardShaderUtility.IsUsingGraphicsToolsStandardShader(material))
            {
                return false;
            }

            return materialSettings.All(x =>
            {
                // If the item isn't required the material is correct.
                if (!x.Value.Item2)
                {
                    return true;
                }

                switch (x.Value.Item1.GetType().Name)
                {
                    case nameof(System.Single):
                        {
                            return material.GetFloat(x.Key) == (float)x.Value.Item1;
                        }
                    case nameof(System.Boolean):
                        {
                            var val = (bool)x.Value.Item1;
                            if (val)
                            {
                                return material.IsKeywordEnabled(x.Key);
                            }
                            else
                            {
                                return !material.IsKeywordEnabled(x.Key);
                            }
                        }
                    case nameof(Color):
                        {
                            return material.GetColor(x.Key) == (Color)x.Value.Item1;
                        }
                    default:
                        // Default return value
                        return false;
                }
            });
        }

        private static void DrawReadonlyPropertyField(SerializedProperty property, params GUILayoutOption[] options)
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(property, options);
            GUI.enabled = true;
        }
    }
}

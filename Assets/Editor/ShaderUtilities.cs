// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CSharp;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// TODO
    /// </summary>
    public class ShaderUtilities
    {
        private static readonly string ClassBody =
 @"// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools
{{
    /// <summary>
    /// This class was auto generated via Assets > Graphics Tools > Generate Shader Behaviour.
    /// Use Mecanim to animate fields on this class to drive materials on Unity UI renderers.
    /// Version=0.1.0
    /// </summary>
    public class {0} : BaseShaderBehaviour
    {{{1}

        /// <inheritdoc/>
        public override void InitializeFromMaterial(Material material)
        {{{2}
        }}

        /// <inheritdoc/>
        public override void ApplyToMaterial(Material material)
        {{{3}
        }}
    }}
}}
";

        private static readonly string PropertyBody = @"        public {0} {1} = {2};";
        private static readonly string PropertyBodyRange = @"        [Range({3}, {4})] public {0} {1} = {2};";
        private static readonly string FromMaterialBody = @"            {0} = material.{1}({2});";
        private static readonly string FromMaterialBodyCast = @"            {0} = ({3})material.{1}({2});";
        private static readonly string ToMaterialBody = @"            material.{0}({1}, {2});";
        private static readonly string ToMaterialBodyCast = @"            material.{0}({1}, ({3}){2});";
        private static readonly string FloatPostfix = "f";

        /// <summary>
        /// TODO
        /// </summary>
        [MenuItem("Assets/Graphics Tools/Generate Shader Behaviour")]
        private static void GenerateShaderMonobehaviour()
        {
            Shader shader = Selection.activeObject as Shader;

            if (shader == null)
            {
                Debug.LogWarning("The active selection is not a shader.");
                return;
            }

            // Generate the properties and methods.
            string properties = string.Empty;
            string fromMaterial = string.Empty;
            string toMaterial = string.Empty;
            int count = ShaderUtil.GetPropertyCount(shader);

            for (int i = 0; i < count; ++i)
            {
                string name = ShaderUtil.GetPropertyName(shader, i);
                ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(shader, i);

                if (type == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    TextureDimension dimension = ShaderUtil.GetTexDim(shader, i);
                    string typeName = TextureDimensionToTypeName(dimension);
                    properties += Environment.NewLine;
                    properties += string.Format(PropertyBody, typeName, name, "null");
 
                    fromMaterial += Environment.NewLine;
                    fromMaterial += string.Format(FromMaterialBodyCast, name, ShaderPropertyTypeToGettor(type), Shader.PropertyToID(name), typeName);

                    toMaterial += Environment.NewLine;
                    toMaterial += string.Format(ToMaterialBodyCast, ShaderPropertyTypeToSettor(type), Shader.PropertyToID(name), name, typeName);
                }
                else 
                {
                    string defaultValue, minValue = null, maxValue = null;
                    if (type == ShaderUtil.ShaderPropertyType.Float)
                    {
                        defaultValue = ShaderUtil.GetRangeLimits(shader, i, 0).ToString() + FloatPostfix;
                    }
                    else if (type == ShaderUtil.ShaderPropertyType.Range)
                    {
                        defaultValue = ShaderUtil.GetRangeLimits(shader, i, 0).ToString() + FloatPostfix;
                        minValue = ShaderUtil.GetRangeLimits(shader, i, 1).ToString() + FloatPostfix;
                        maxValue = ShaderUtil.GetRangeLimits(shader, i, 2) + FloatPostfix;
                    }
                    else
                    {
                        defaultValue = ShaderPropertyTypeToDefault(type);
                    }

                    properties += Environment.NewLine;

                    if (minValue == null || maxValue == null)
                    {
                        properties += string.Format(PropertyBody, ShaderPropertyTypeToTypeName(type), name, defaultValue);
                    }
                    else
                    {
                        properties += string.Format(PropertyBodyRange, ShaderPropertyTypeToTypeName(type), name, defaultValue, minValue, maxValue);
                    }

                    fromMaterial += Environment.NewLine;
                    fromMaterial += string.Format(FromMaterialBody, name, ShaderPropertyTypeToGettor(type), Shader.PropertyToID(name));

                    toMaterial += Environment.NewLine;
                    toMaterial += string.Format(ToMaterialBody, ShaderPropertyTypeToSettor(type), Shader.PropertyToID(name), name);
                }
            }

            try
            {
                string directory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(shader));
                string className = SanitizeIdentifier(shader.name) + "ShaderBehaviour";
                string path = Path.Combine(directory, string.Format("{0}.cs", className));

                string classText = string.Format(ClassBody, className, properties, fromMaterial, toMaterial);

                File.WriteAllText(path, classText);

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        [MenuItem("Assets/Graphics Tools/Generate Shader Behaviour", true)]
        private static bool ValidateGenerateShaderMonobehaviour()
        {
            return Selection.activeObject.GetType() == typeof(Shader);
        }

        /// <summary>
        /// TODO
        /// </summary>
        private static string ShaderPropertyTypeToTypeName(ShaderUtil.ShaderPropertyType type)
        {
            switch (type)
            {
                case ShaderUtil.ShaderPropertyType.Color: return "Color";
                case ShaderUtil.ShaderPropertyType.Vector: return "Vector3";
                default:
                case ShaderUtil.ShaderPropertyType.Float:
                case ShaderUtil.ShaderPropertyType.Range: return "float";
                case ShaderUtil.ShaderPropertyType.TexEnv: return "Texture";
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private static string ShaderPropertyTypeToGettor(ShaderUtil.ShaderPropertyType type)
        {
            switch (type)
            {
                case ShaderUtil.ShaderPropertyType.Color: return "GetColor";
                case ShaderUtil.ShaderPropertyType.Vector: return "GetVector";
                default:
                case ShaderUtil.ShaderPropertyType.Float:
                case ShaderUtil.ShaderPropertyType.Range: return "GetFloat";
                case ShaderUtil.ShaderPropertyType.TexEnv: return "GetTexture";
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private static string ShaderPropertyTypeToSettor(ShaderUtil.ShaderPropertyType type)
        {
            switch (type)
            {
                case ShaderUtil.ShaderPropertyType.Color: return "SetColor";
                case ShaderUtil.ShaderPropertyType.Vector: return "SetVector";
                default:
                case ShaderUtil.ShaderPropertyType.Float:
                case ShaderUtil.ShaderPropertyType.Range: return "SetFloat";
                case ShaderUtil.ShaderPropertyType.TexEnv: return "SetTexture";
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private static string TextureDimensionToTypeName(TextureDimension dimension)
        {
            switch (dimension)
            {
                default:
                case TextureDimension.Tex2D: return "Texture2D";
                case TextureDimension.Tex3D: return "Texture3D";
                case TextureDimension.Cube: return "Cubemap";
                case TextureDimension.Tex2DArray: return "Texture2DArray";
                case TextureDimension.CubeArray: return "CubemapArray";
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private static string ShaderPropertyTypeToDefault(ShaderUtil.ShaderPropertyType type)
        {
            switch (type)
            {
                case ShaderUtil.ShaderPropertyType.Color: return "Color.white";
                case ShaderUtil.ShaderPropertyType.Vector: return "Vector3.zero";
                default:
                case ShaderUtil.ShaderPropertyType.Float:
                case ShaderUtil.ShaderPropertyType.Range: return "0.0f";
                case ShaderUtil.ShaderPropertyType.TexEnv: return "null";
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private static string SanitizeIdentifier(string input)
        {
            string className = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
            bool isValid = CSharpCodeProvider.CreateProvider("C#").IsValidIdentifier(className);

            if (!isValid)
            {
                // Remove invalid characters.
                Regex regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
                className = regex.Replace(className, string.Empty);

                // Ensure class/variable name begins with a letter or underscore.
                if (!char.IsLetter(className, 0))
                {
                    className = className.Insert(0, "_");
                }
            }

            return RemoveWhitespace(className);
        }

        /// <summary>
        /// TODO
        /// </summary>
        private static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
#define PATCHING_ACTIVE
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class NonSrpShaderAssetGenerator : AssetPostprocessor
{

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
#if PATCHING_ACTIVE
        bool assetDBDirty = false;
        foreach (string str in importedAssets)
        {
            if (str.EndsWith(".shader") && !str.EndsWith("NonSrp.shader"))
            {
                string resultFile = copyAndPatchShader(str);
                if (resultFile != null)
                {
                    assetDBDirty = true;
                }
            }   
        }
        if (assetDBDirty) { AssetDatabase.Refresh(); }
#endif
    }

    static string copyAndPatchShader(string path)
    {
        if (!File.Exists(path)) { return null; }

        bool isPatched = false;
        
        string shaderText = File.ReadAllText(path);
        string sourceDirectory = Path.GetDirectoryName(path);
        string targetDirectory = sourceDirectory + "/NonSRP";

        // Only work with shaders from Graphics Tools
        string nameDeclaration = Regex.Match(shaderText, " *Shader *\"Graphics Tools\\/.*\"").Value;
        if (nameDeclaration.Length <= 0 ) { return null; }
        
        string outputPath = Path.Combine( sourceDirectory, Path.GetFileNameWithoutExtension(path) +"NonSrp" +  Path.GetExtension(path));
        
        // Put "_NON_SRP" befor StandardProgram include (in Standard and StandardCanvas)
        MatchCollection matchesStandardProgram = Regex.Matches(shaderText, "(?<tab> *)(#include_with_pragmas \"GraphicsToolsStandardProgram.hlsl\")");
        if (matchesStandardProgram.Count > 0)
        {
            string tab = matchesStandardProgram[1].Groups["tab"].Value;
            shaderText = Regex.Replace(shaderText, matchesStandardProgram[0].Value,  tab + "#define _NON_SRP\r\n" + matchesStandardProgram[0].Value);
            isPatched = true;
        }

        // Remove CBUFFER statements
        MatchCollection matchCBuff = Regex.Matches(shaderText, " *(CBUFFER_START\\(UnityPerMaterial\\))| *(CBUFFER_END)");
        if (matchCBuff.Count > 0)
        {
            shaderText = Regex.Replace(shaderText, " *(CBUFFER_START\\(UnityPerMaterial\\))| *(CBUFFER_END)", "");
            isPatched = true;
        }

        // Rename shader, put it under NonSrp directory, write to file
        if (isPatched)
        {
            Match shaderDirAndName = Regex.Match(nameDeclaration, "(?<dir>.*\\/)(?<name>.*)\"");
            string newNameDeclaration = shaderDirAndName.Groups["dir"].Value + "Non-SRP/" + shaderDirAndName.Groups["name"].Value + "NonSrp\"" ;
            shaderText = Regex.Replace(shaderText, nameDeclaration, newNameDeclaration);
            
            File.WriteAllText(outputPath, shaderText);
            return outputPath;
        }
        else { return null; }

    }

}

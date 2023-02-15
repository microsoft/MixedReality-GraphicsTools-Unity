// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Unity.Rendering.Universal;
using UnityEditor.ShaderGraph;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    class GraphicsToolsUniversalLitSubTarget : UniversalSubTarget
    {
        protected override ShaderUtils.ShaderID shaderID => ShaderUtils.ShaderID.SG_Lit;

        private UniversalLitSubTarget proxy = new UniversalLitSubTarget();

        public GraphicsToolsUniversalLitSubTarget()
        {
            displayName = " Graphics Tools Lit";
        }

        public override void GetActiveBlocks(ref TargetActiveBlockContext context)
        {
            proxy.GetActiveBlocks(ref context);
        }

        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<string> registerUndo)
        {
            proxy.GetPropertiesGUI(ref context, onChange, registerUndo);
        }

        public override bool IsActive()
        {
            return proxy.IsActive();
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if HAS_VFX_GRAPH
namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    class GraphicsToolsVFXShaderGraphLitGUI : GraphicsToolsShaderGraphLitGUI
    {
        protected override uint materialFilter => uint.MaxValue & ~(uint)Expandable.SurfaceInputs;
    }

    class GraphicsToolsVFXShaderGraphUnlitGUI : GraphicsToolsShaderGraphUnlitGUI
    {
        protected override uint materialFilter => uint.MaxValue & ~(uint)Expandable.SurfaceInputs;
    }
}
#endif

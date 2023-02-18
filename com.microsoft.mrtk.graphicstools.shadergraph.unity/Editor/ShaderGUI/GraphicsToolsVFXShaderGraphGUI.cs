// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if HAS_VFX_GRAPH
namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Material GUI extension to the GraphicsToolsUniversalLitSubTargetwith VFX Graph.
    /// </summary>
    class GraphicsToolsVFXShaderGraphLitGUI : GraphicsToolsShaderGraphLitGUI
    {
        protected override uint materialFilter => uint.MaxValue & ~(uint)Expandable.SurfaceInputs;
    }

    /// <summary>
    /// Material GUI extension to the GraphicsToolsUniversalUnlitSubTarget with VFX Graph.
    /// </summary>
    class GraphicsToolsVFXShaderGraphUnlitGUI : GraphicsToolsShaderGraphUnlitGUI
    {
        protected override uint materialFilter => uint.MaxValue & ~(uint)Expandable.SurfaceInputs;
    }
}
#endif // HAS_VFX_GRAPH

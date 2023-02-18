// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if GT_USE_URP
using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEditor.ShaderGraph;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Utility class to help set alpha blending states for GraphicsToolsUniversalLitSubTarget and GraphicsToolsUniversalUnlitSubTarget.
    /// </summary>
    static class GraphicsToolsCoreRenderStates
    {
        /// <summary>
        /// Name of the alpha blending shader properties. (Matches other shaders in Graphics Tools.)
        /// </summary>
        public static class Property
        {
            public static readonly string SrcBlendAlpha = "_SrcBlendAlpha";
            public static readonly string DstBlendAlpha = "_DstBlendAlpha";
        }

        /// <summary>
        /// Shaderlab property names.
        /// </summary>
        public static class Uniforms
        {
            public static readonly string SrcBlendAlpha = "[" + Property.SrcBlendAlpha + "]";
            public static readonly string DstBlendAlpha = "[" + Property.DstBlendAlpha + "]";
        }

        /// <summary>
        /// Used by lit/unlit subtargets when allowMaterialOverride is false.
        /// </summary>
        public static readonly RenderStateCollection MaterialControlledRenderStateAlpha = new RenderStateCollection
        {
            { RenderState.ZTest(CoreRenderStates.Uniforms.zTest) },
            { RenderState.ZWrite(CoreRenderStates.Uniforms.zWrite) },
            { RenderState.Cull(CoreRenderStates.Uniforms.cullMode) },
            { RenderState.Blend(CoreRenderStates.Uniforms.srcBlend, CoreRenderStates.Uniforms.dstBlend, Uniforms.SrcBlendAlpha, Uniforms.DstBlendAlpha) },
        };

        /// <summary>
        /// Fork of CoreRenderStates.UberSwitchedRenderState to support configurable source and destination alpha blending state.
        /// </summary>
        public static RenderStateCollection UberSwitchedRenderState(UniversalTarget target, bool overrideBlendAlpha, Blend alphaSrc, Blend alphaDst)
        {
            if (target.allowMaterialOverride)
                return overrideBlendAlpha ? MaterialControlledRenderStateAlpha : CoreRenderStates.MaterialControlledRenderState;
            else
            {
                var result = new RenderStateCollection();

                result.Add(RenderState.ZTest(target.zTestMode.ToString()));

                if (target.zWriteControl == ZWriteControl.Auto)
                {
                    if (target.surfaceType == SurfaceType.Opaque)
                        result.Add(RenderState.ZWrite(ZWrite.On));
                    else
                        result.Add(RenderState.ZWrite(ZWrite.Off));
                }
                else if (target.zWriteControl == ZWriteControl.ForceEnabled)
                    result.Add(RenderState.ZWrite(ZWrite.On));
                else
                    result.Add(RenderState.ZWrite(ZWrite.Off));

                result.Add(RenderState.Cull(CoreRenderStates.RenderFaceToCull(target.renderFace)));

                if (target.surfaceType == SurfaceType.Opaque)
                {
                    result.Add(RenderState.Blend(Blend.One, Blend.Zero));
                }
                else
                {
                    if (overrideBlendAlpha)
                    {
                        switch (target.alphaMode)
                        {
                            case AlphaMode.Alpha:
                                result.Add(RenderState.Blend(Blend.SrcAlpha, Blend.OneMinusSrcAlpha, alphaSrc, alphaDst));
                                break;
                            case AlphaMode.Premultiply:
                                result.Add(RenderState.Blend(Blend.One, Blend.OneMinusSrcAlpha, alphaSrc, alphaDst));
                                break;
                            case AlphaMode.Additive:
                                result.Add(RenderState.Blend(Blend.SrcAlpha, Blend.One, alphaSrc, alphaDst));
                                break;
                            case AlphaMode.Multiply:
                                result.Add(RenderState.Blend(Blend.DstColor, Blend.Zero, alphaSrc, alphaDst));
                                break;
                        }
                    }
                    else
                    {
                        switch (target.alphaMode)
                        {
                            case AlphaMode.Alpha:
                                result.Add(RenderState.Blend(Blend.SrcAlpha, Blend.OneMinusSrcAlpha, Blend.One, Blend.OneMinusSrcAlpha));
                                break;
                            case AlphaMode.Premultiply:
                                result.Add(RenderState.Blend(Blend.One, Blend.OneMinusSrcAlpha, Blend.One, Blend.OneMinusSrcAlpha));
                                break;
                            case AlphaMode.Additive:
                                result.Add(RenderState.Blend(Blend.SrcAlpha, Blend.One, Blend.One, Blend.One));
                                break;
                            case AlphaMode.Multiply:
                                result.Add(RenderState.Blend(Blend.DstColor, Blend.Zero));
                                break;
                        }
                    }
                }

                return result;
            }
        }
    }
}
#endif // GT_USE_URP

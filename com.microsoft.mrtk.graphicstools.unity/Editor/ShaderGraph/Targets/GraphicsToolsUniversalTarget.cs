// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEditor.ShaderGraph;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// TODO
    /// </summary>
    static class GraphicsToolsCoreRenderStates
    {
        /// <summary>
        /// TODO
        /// </summary>
        public static class Property
        {
            public static readonly string SrcBlendAlpha = "_SrcBlendAlpha";
            public static readonly string DstBlendAlpha = "_DstBlendAlpha";
        }

        /// <summary>
        /// TODO
        /// </summary>
        public static class Uniforms
        {
            public static readonly string SrcBlendAlpha = "[" + Property.SrcBlendAlpha + "]";
            public static readonly string DstBlendAlpha = "[" + Property.DstBlendAlpha + "]";
        }

        // used by lit/unlit subtargets
        public static readonly RenderStateCollection MaterialControlledRenderStateAlpha = new RenderStateCollection
        {
            { RenderState.ZTest(CoreRenderStates.Uniforms.zTest) },
            { RenderState.ZWrite(CoreRenderStates.Uniforms.zWrite) },
            { RenderState.Cull(CoreRenderStates.Uniforms.cullMode) },
            { RenderState.Blend(CoreRenderStates.Uniforms.srcBlend, CoreRenderStates.Uniforms.dstBlend, Uniforms.SrcBlendAlpha, Uniforms.DstBlendAlpha) },
        };

        /// <summary>
        /// TODO
        /// </summary>
        public static RenderStateCollection UberSwitchedRenderState(UniversalTarget target, bool alphaBlendOne)
        {
            if (target.allowMaterialOverride)
                return alphaBlendOne ? MaterialControlledRenderStateAlpha : CoreRenderStates.MaterialControlledRenderState;
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
                    if (alphaBlendOne)
                    {
                        switch (target.alphaMode)
                        {
                            case AlphaMode.Alpha:
                                result.Add(RenderState.Blend(Blend.SrcAlpha, Blend.OneMinusSrcAlpha, Blend.One, Blend.One));
                                break;
                            case AlphaMode.Premultiply:
                                result.Add(RenderState.Blend(Blend.One, Blend.OneMinusSrcAlpha, Blend.One, Blend.One));
                                break;
                            case AlphaMode.Additive:
                                result.Add(RenderState.Blend(Blend.SrcAlpha, Blend.One, Blend.One, Blend.One));
                                break;
                            case AlphaMode.Multiply:
                                result.Add(RenderState.Blend(Blend.DstColor, Blend.Zero, Blend.One, Blend.One));
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

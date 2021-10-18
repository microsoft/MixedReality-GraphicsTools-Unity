# Clipping primitive

The `ClippingPrimitive` behaviors allow for performant `plane`, `sphere`, and `box` shape clipping with the ability to specify which side of the primitive to clip against (inside or outside) when used with Graphics Tools shaders.

> [!NOTE]
> `ClippingPrimitive`s utilize [clip/discard](https://developer.download.nvidia.com/cg/clip.html) instructions within shaders and disable Unity's ability to batch clipped renderers. Take these performance implications in mind when utilizing clipping primitives.

`ClippingPlane.cs`, `ClippingSphere.cs`, and `ClippingBox.cs` can be used to easily control clipping primitive properties. Use these components with the following shaders to leverage clipping scenarios.

- *Graphics Tools/Standard*

## Advanced Usage

By default only one `ClippingPrimitive` can clip a [renderer](https://docs.unity3d.com/ScriptReference/Renderer.html) at a time. If your project requires more than one `ClippingPrimitive` to influence a [renderer](https://docs.unity3d.com/ScriptReference/Renderer.html)  the sample code below demonstrates how to achieve this.

> [!NOTE]
> Having multiple `ClippingPrimitive`s clip a [renderer](https://docs.unity3d.com/ScriptReference/Renderer.html) will increase pixel shader instructions and will impact performance. Please profile these changes within your project.

*How to have two different `ClippingPrimitive`s clip a render. For example a `ClippingSphere` and `ClippingBox` at the same time:*

```C#
// Within GraphicsToolsStandardProgram.cginc (or another Graphics Tools shader) change:

#pragma multi_compile _ _CLIPPING_PLANE _CLIPPING_SPHERE _CLIPPING_BOX

// to:

#pragma multi_compile _ _CLIPPING_PLANE
#pragma multi_compile _ _CLIPPING_SPHERE
#pragma multi_compile _ _CLIPPING_BOX
```

> [!NOTE]
> The above change will incur additional shader compilation time.

*How to have two of the same `ClippingPrimitive`s clip a render. For example two `ClippingBoxes`at the same time:*

```C#
// 1) Add the below MonoBehaviour to your project:

[ExecuteInEditMode]
public class SecondClippingBox : ClippingBox
{
    /// <inheritdoc />
    protected override string Keyword
    {
        get { return "_CLIPPING_BOX2"; }
    }

    /// <inheritdoc />
    protected override string ClippingSideProperty
    {
        get { return "_ClipBoxSide2"; }
    }

    /// <inheritdoc />
    protected override void Initialize()
    {
        base.Initialize();

        clipBoxSizeID = Shader.PropertyToID("_ClipBoxSize2");
        clipBoxInverseTransformID = Shader.PropertyToID("_ClipBoxInverseTransform2");
    }
}

// 2) Within GraphicsToolsStandardProgram.cginc (or another Graphics Tools shader) add the following multi_compile pragma:

#pragma multi_compile _ _CLIPPING_BOX2

// 3) In the same shader change:

#if defined(_CLIPPING_PLANE) || defined(_CLIPPING_SPHERE) || defined(_CLIPPING_BOX)

// to:

#if defined(_CLIPPING_PLANE) || defined(_CLIPPING_SPHERE) || defined(_CLIPPING_BOX) || defined(_CLIPPING_BOX2)

// 4) In the same shader add the following shader variables:

#if defined(_CLIPPING_BOX2)
    fixed _ClipBoxSide2;
    float4 _ClipBoxSize2;
    float4x4 _ClipBoxInverseTransform2;
#endif

// 5) In the same shader change:

#if defined(_CLIPPING_BOX)
    primitiveDistance = min(primitiveDistance, PointVsBox(i.worldPosition.xyz, _ClipBoxSize.xyz, _ClipBoxInverseTransform) * _ClipBoxSide);
#endif

// to:

#if defined(_CLIPPING_BOX)
    primitiveDistance = min(primitiveDistance, PointVsBox(i.worldPosition.xyz, _ClipBoxSize.xyz, _ClipBoxInverseTransform) * _ClipBoxSide);
#endif
#if defined(_CLIPPING_BOX2)
    primitiveDistance = min(primitiveDistance, PointVsBox(i.worldPosition.xyz, _ClipBoxSize2.xyz, _ClipBoxInverseTransform2) * _ClipBoxSide2);
#endif
```

Finally, add a `ClippingBox` and SecondClippingBox component to your scene and specify the same renderer for both boxes. The renderer should now be clipped by both boxes simultaneously.

## See also

* [Standard Shader](StandardShader.md)
* [Material Instance](MaterialInstance.md)
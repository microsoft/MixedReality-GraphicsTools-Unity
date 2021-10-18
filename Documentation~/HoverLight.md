# Hover light

A `HoverLight` is a [Fluent Design System](https://www.microsoft.com/design/fluent/) paradigm that mimics a [point light](https://docs.unity3d.com/Manual/Lighting.html) hovering near the surface of an object. Often used for far away interactions, the application can control the properties of a Hover Light via the `HoverLight` component.

For a material to be influenced by a `HoverLight` the *Graphics Tools/Standard* shader must be used and the *Hover Light* property must be enabled.

> [!Note]
> The *Graphics Tools/Standard* shader supports up to two `HoverLight`s by default, but will scale to support four and then ten as more lights are added to the scene.

## Advanced Usage

Only ten `HoverLight`s can illuminate a [material](https://docs.unity3d.com/ScriptReference/Material.html) at a time. If your project requires more than ten `HoverLight`s to influence a [material](https://docs.unity3d.com/ScriptReference/Material.html) the sample code below demonstrates how to achieve this.

> [!Note]
> Having many `HoverLight`s illuminate a [material](https://docs.unity3d.com/ScriptReference/Material.html) will increase pixel shader instructions and will impact performance. **Please profile these changes within your project.**

*How to increase the number of available `HoverLight`s
 from ten to twelve.*

```C#
// 1) Within GraphicsToolsStandardProgram.cginc change:

#if defined(_HOVER_LIGHT_HIGH)
#define HOVER_LIGHT_COUNT 10

// to:

#if defined(_HOVER_LIGHT_HIGH)
#define HOVER_LIGHT_COUNT 12

// 2) Within HoverLight.cs change:

private const int hoverLightCountHigh = 10;

// to:

private const int hoverLightCountHigh = 12;
```

> [!NOTE]
> If Unity logs a warning similar to below then you must restart Unity before your changes will take effect.
>
> `Property (_HoverLightData) exceeds previous array size (24 vs 20). Cap to previous >size.`

## See also

* [Standard Shader](StandardShader.md)

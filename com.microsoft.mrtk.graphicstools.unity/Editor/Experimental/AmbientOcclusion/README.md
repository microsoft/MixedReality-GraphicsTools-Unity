# Ambient occlusion (AO)

Calculates [ambient occlusion](https://en.wikipedia.org/wiki/Ambient_occlusion) using ray-casts (and stores it per-vertex for use with the Graphics Tools Standard shader.

Using AO with a `Mesh` is a two part process. The first is to cast rays and store how often we hit nearby objects. This is done for every vertex. The second part is to read that vertex metadata in the shader, and integrate it with the lighting.

## Usage

1) From the menu `Window -> Graphics tools -> Ambient occlusion`
2) Select a GameObject with a MeshFilter on it.
3) Press the Apply button

## Motivation

Ambient occlusion adds realism to a scene by providing important clues to viewers about an object's relationship to other objects.

However, as a post-process it may be expensive, and not well suited for mobile, VR, or other "low end" platforms. Baking global illumination to textures can cumbersome and relies on well-formed UVs (when not using lightmaps).

As an alternative, this tool will bake occlusion information into the mesh vertices. The Graphics Tools Standard shader then uses this information to integrate the lighting.

## Implementation Notes

- The quality of the AO solution is very dependent on the vertex count and normals of the mesh.
- Editor UI built with UI Toolkit using UXML and USS, backed by ScriptableObject settings

## FAQ

### Why is samples visulization not showing any hits?

- Is there a [mesh] collider on the object you're trying to hit?
- Are the normals facing the right way? 
- Check that `MaxSampleDistance` is reaching the objects you expect to be occluded by.

### Why is the AO noisy?

- Increase your `vertex samples`
- Decrease your `MaxSampleDistance`, the further out you to the more likely you'll hit stuff randomly without having to increase your vertex sample count.

### Shouldn't this area be darker?

- Check that `MaxSampleDistance` is reaching the objects you expect to be occluded by.

### How can I toggle this effect on and off?

- Uncheck `Vertex ambient occlusion` in the material, this is the fastest way. It ignores the data, but still sends it to the GPU.

### Why do my upgraded material's textures look weird?

- They may have come from GLTF/GLB - try setting standad-shader.tiling.y to -1

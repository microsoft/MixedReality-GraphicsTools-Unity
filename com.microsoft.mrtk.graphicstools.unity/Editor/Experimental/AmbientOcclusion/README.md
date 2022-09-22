# Ambient occlusion

Calculates [ambient occlusion](https://en.wikipedia.org/wiki/Ambient_occlusion) using ray-casts (AO) and stores it per-vertex for use with the Graphics Tools Standard shader.

Using AO with a `Mesh` is a two part process. The first is to cast rays and store how often we hit nearby objects. This is done for every vertex. The second part is to read that vertex metadata in the shader, and integrate it with the lighting.

## Usage

1) Add `AmbientOcclusion` to a GameObject that contains a `MeshFilter` and press `Gather samples` in the inspector.
2) Create a new material and choose `Graphics tools/Standard` as the shader
3) Check the `Vertex ambient occlusion` is enabled for the material the inspector
4) Assign new material to GameObject's `MeshRenderer`

## Motivation

Realtime ambient occlusion adds realism to a scene and provides import clues to viewers about an object's relationship to other objects.

However, as a post-process it is expensive and not well suited for VR and low-end platforms. Baking global illumination to textures is also a cumbersome workflow and relies on sane UVs.

As an alternative, we bake this occlusion information into the mesh vertices, which is evaluated per vertex and passed to fragment shader as part of rasterization.

## Implementation Notes

- The fidelity of the AO solution is very dependent on the vertex count and normals of the mesh.

## FAQ

### Why is samples visulization not showing any hits?

- Is there a [mesh] collider on the object you're trying to hit?
- Are the normals facing the right way? 
- Check that `MaxSampleDistance` is reaching the objects you expect to be occluded by.

### Why is the AO noisy?

- Increase your `vertex samples`

### Shouldn't this area be darker?

- Check that `MaxSampleDistance` is reaching the objects you expect to be occluded by.

### How can I toggle this effect on and off?

- Uncheck `Use veretex color` in the material, this is the fastest way. It ignores the data, but still sends it to the GPU.
- Disable the component. The will restore mesh to it's original state before `AmbientOcclusion` modified it. Note, this will cause `Gather samples` to evaluate.

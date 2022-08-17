# Ambient occlusion

Calculates [ambient occlusion](https://en.wikipedia.org/wiki/Ambient_occlusion) using ray-casts (AO) and stores it per-vertex for use with the [standard shader]()

## Usage

1) Add `AmbientOcclusion` to a GameObject that contains a `MeshFilter` and press `Gather samples` in the inspector.
2) Create a new material and choose `Graphics tools/Standard` as the shader
3) Check the `Vertex ambient occlusion` is enabled for the material the inspector
4) Assign new material to GameObject's `MeshRenderer`

## FAQ

*My samples aren't generating hits!*

- Is there a [mesh] collider on the object you're trying to hit?
- Are the normals facing the right way? 
- Check that `MaxSampleDistance` is reaching the objects you expect to be occluded by.

*The AO is noisy!*

- Increase your `vertex samples`

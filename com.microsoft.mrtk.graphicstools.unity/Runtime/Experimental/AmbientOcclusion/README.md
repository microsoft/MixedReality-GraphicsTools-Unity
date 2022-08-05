# Ambient occlusion

Calculates [ambient occlusion](https://en.wikipedia.org/wiki/Ambient_occlusion) (AO) and writes it to vertex color for use with the [standard shader]()

## Components

Name | Description
---- | ---
AmbientOcclusion | Writes AO to vertex color 
MeshGizmo | Visualizes mesh normals and point numbers for technical analysis in editor scene view

# FAQ

*My samples aren't generating hits!*

- Is there a collider on the object you're trying to hit?
- Are the normals facing the right way? 
- What is the `MaxSampleDistance` in the `Sampler`?

*The vertex has the wrong color!*

- Check the geometry normals using the `ShowMeshNormals` component. It may be it's pointing in a unexpected direction.
- Is your collider appropriate - ie a Mesh vs a Box?

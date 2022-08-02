# Sampler toolkit

## Components

Name | Description
---- | ---
Sampler | Samples scene hemisphere and generates statistics from ray hits
ShowMeshNormals | Visualizes in the Scene window
AmbientOcclusion | Uses data from the `Sampler` to set mesh vertex color

# FAQ

*My samples aren't generating hits!*

- Is there a collider on the object you're trying to hit?
- Are the normals facing the right way? 
- What is the `MaxSampleDistance` in the `Sampler`?

*The vertex has the wrong color!*

- Check the geometry normals using the `ShowMeshNormals` component. It may be it's pointing in a unexpected direction.
- Is your collider appropriate - ie a Mesh vs a Box?

*My vertex color isn't updating*

- Did you add a Unity event to the 'Samples updated' in the `Sampler` component? It should call the `ApplyVertexColor` method.
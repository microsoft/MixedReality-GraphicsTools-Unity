# FAQ

*My samples aren't generating hits!*

- Is there a mesh collider on the object you're trying to hit?
- Are the normals facing the right way? 

*The vertex has the wrong color!*

- Check the geometry normals using the `ShowMeshNormals` component. It may be it's pointing in a unexpected direction.

*My vertex color isn't updating*

- Did you add a Unity event to the 'Samples updated' in the `Sampler` component? It should call the `ApplyVertexColor` method.
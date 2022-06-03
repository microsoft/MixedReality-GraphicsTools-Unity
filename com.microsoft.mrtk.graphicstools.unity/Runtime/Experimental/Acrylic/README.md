Note: Requires com.unity.render-pipelines.universal 10.6.0 or above

Use the acrylic assets to create a blurred bg effect on backplates similar to the acrylic material
in Windows 11.  These assets require use of the Universal Render Pipeline.

Set up:  a given acrylic layer should have its own render layer (or set of layers) containing the backplate
and all UI objects for that layer.  Assign the render layer(s) to the Render Layers property of the acrylic layer in 
the acrylic layer manager.  The layers are used to exclude foreground elements from the blurred bg map.

Acrylic Helper:

Assign this component to every backplate that uses an acrylic material.  This component notifies the acrylic layer 
manager whenever the object is enabled or disabled.  The acrylic shader currently supports 2 layers (0 and 1).

Acrylic Layer Manager:

Manages creating and updating the blurred background maps used by acrylic materials.  The manager works with the Acrylic Helper 
component to limit the additional processing performed to those acrylic layers that are currently being used in the scene.
A scene should have one copy of the manager.

The manager supports two methods of capturing a background image:

1.  Copy framebuffer:  records a downsampled copy of the current framebuffer during URP rendering using a set of renderer feature passes
2.  Render to texture:  renders a separate reduced-resolution image of the background objects

The captured image is then blurred based on the per-layer blur settings.  If the blur map is not updated every frame, the blur process is applied
during the next frame after the capture.

There's also an option to gradually blend in an updated map over several frames (if the map is not updated every frame).

Editor options:

Use the Retain in Editor option to leave the acrylic layers enabled when the app is not playing.
This option only works with the Copy Framebuffer capture method.

Platform-specific settings:

When building for Quest with Multiview stereo, use the following:

Capture Method = Copy Framebuffer
Filter Method = Kawase
Update Period = 1

Additional reading:
https://www.intel.com/content/www/us/en/developer/articles/technical/an-investigation-of-fast-real-time-gpu-based-image-blur-algorithms.html
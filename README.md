# OpenGL 4 Skull Renderer - Effects Version

This is a small C# / WinForms renderer using OpenGL directly through Win32 / WGL interop. It does not use OpenTK, SharpGL, Silk.NET, GLFW, SDL, ImGui, or any other external graphics library.

This version extends the original textured OBJ viewer with a more visual shader pipeline and a small scene around the model.

[![GitHub Sponsors](https://img.shields.io/github/sponsors/makarov-mm?style=flat&logo=github)](https://github.com/sponsors/makarov-mm)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)
![C#](https://img.shields.io/badge/C%23-239120)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0A66C2?logo=linkedin&logoColor=white)](https://www.linkedin.com/in/makarov-mm/)
[![Threads](https://img.shields.io/badge/Threads-000000?logo=threads&logoColor=white)](https://www.threads.net/@m.m.makarov)
[![Instagram](https://img.shields.io/badge/Instagram-E4405F?logo=instagram&logoColor=white)](https://www.instagram.com/m.m.makarov/)
![OpenGL](https://img.shields.io/badge/OpenGL-5586A4?logo=opengl&logoColor=white)
![Windows](https://img.shields.io/badge/Windows-0078D6?logo=windows&logoColor=white)

## Added effects

- Tangent-space normal mapping generated at runtime from the diffuse texture
- Fresnel-based reflective material effect
- Procedural environment reflection in the fragment shader
- Stronger specular highlights
- Cyan rim lighting on silhouettes
- Animated scanning energy bands across the model
- Reflective mirror-floor pass using a second mirrored draw of the model
- Procedural glowing grid floor
- Darker cinematic background
- Keyboard toggles for comparing the original and enhanced look

This is not hardware ray tracing. It is still classic rasterization, but the shader uses reflection vectors, Fresnel weighting, procedural environment lighting, and a mirrored render pass to create a more advanced visual result while keeping the project compact.

## Screenshot

![Screenshot](screenshots/screenshot1.png)

## Controls

- Left mouse drag — rotate camera/model
- Mouse wheel — zoom
- Space — toggle auto-rotation
- E — toggle shader effects
- M — toggle mirror floor / reflection pass
- R — reset camera
- Esc — close

## Technical Details

- Language: C#
- UI: WinForms
- Graphics API: OpenGL 4 Core Profile
- Interop: Win32 / WGL / GDI through P/Invoke
- Model format: Wavefront OBJ
- Texture loading: System.Drawing
- No third-party libraries

## How to Run

Open the solution in Visual Studio and build/run the WinForms project. x64 is recommended.

The project expects these files to be copied to the output directory:

- `objects/skull.obj`
- `objects/skull.jpg`
- `shaders/skull.vert`
- `shaders/skull.frag`

The `.csproj` already contains `CopyToOutputDirectory` entries for them.

## License

MIT License

Copyright (c) 2026 Mykhailo Makarov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Support

If you found this project interesting or useful, you can support my work:

[![GitHub Sponsors](https://img.shields.io/github/sponsors/makarov-mm?style=flat&logo=github)](https://github.com/sponsors/makarov-mm)

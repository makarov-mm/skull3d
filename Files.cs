namespace Skull3d;

public static class Files
{
    public static string Model => Path.Combine(AppContext.BaseDirectory, "objects", "skull.obj");
    public static string TextureDiffuse => Path.Combine(AppContext.BaseDirectory, "objects", "skull.jpg");
    public static string ShaderVertex => Path.Combine(AppContext.BaseDirectory, "shaders", "skull.vert");
    public static string ShaderFragment => Path.Combine(AppContext.BaseDirectory, "shaders", "skull.frag");
}

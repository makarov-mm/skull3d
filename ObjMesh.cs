namespace Skull3d;

public sealed class ObjMesh(ObjVertex[] vertices, string? diffuseTextureFile)
{
    public ObjVertex[] Vertices { get; } = vertices;
    public string? DiffuseTextureFile { get; } = diffuseTextureFile;
}

using System.Drawing.Imaging;
using System.Numerics;

namespace Skull3d;

public static class Texture
{
    public static uint LoadTextureFromFile(string path)
    {
        using Bitmap original = new Bitmap(path);
        using Bitmap bmp = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);

        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.DrawImage(original, 0, 0, original.Width, original.Height);
        }

        byte[] rgba = ReadRgba(bmp);
        return UploadRgbaTexture(bmp.Width, bmp.Height, rgba, mipmaps: true);
    }

    public static uint CreateNormalMapFromFile(string path, float strength = 3.0f)
    {
        using Bitmap original = new Bitmap(path);
        using Bitmap bmp = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);

        using (Graphics gfx = Graphics.FromImage(bmp))
        {
            gfx.DrawImage(original, 0, 0, original.Width, original.Height);
        }

        byte[] rgba = ReadRgba(bmp);
        int w = bmp.Width;
        int h = bmp.Height;
        float[] lum = new float[w * h];

        for (int i = 0; i < w * h; i++)
        {
            int p = i * 4;
            lum[i] = (rgba[p + 0] * 0.2126f + rgba[p + 1] * 0.7152f + rgba[p + 2] * 0.0722f) / 255.0f;
        }

        byte[] normalRgba = new byte[w * h * 4];

        for (int y = 0; y < h; y++)
        {
            int ym = Math.Max(y - 1, 0);
            int yp = Math.Min(y + 1, h - 1);

            for (int x = 0; x < w; x++)
            {
                int xm = Math.Max(x - 1, 0);
                int xp = Math.Min(x + 1, w - 1);

                float dx = (lum[y * w + xp] - lum[y * w + xm]) * strength;
                float dy = (lum[yp * w + x] - lum[ym * w + x]) * strength;

                Vector3 n = Vector3.Normalize(new Vector3(-dx, -dy, 1.0f));

                int p = (y * w + x) * 4;
                normalRgba[p + 0] = ToByte(n.X * 0.5f + 0.5f);
                normalRgba[p + 1] = ToByte(n.Y * 0.5f + 0.5f);
                normalRgba[p + 2] = ToByte(n.Z * 0.5f + 0.5f);
                normalRgba[p + 3] = 255;
            }
        }

        return UploadRgbaTexture(w, h, normalRgba, mipmaps: true);
    }

    private static byte ToByte(float v) => (byte)Math.Clamp((int)(v * 255.0f + 0.5f), 0, 255);

    private static unsafe byte[] ReadRgba(Bitmap bmp)
    {
        Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        BitmapData data = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            byte[] rgba = new byte[bmp.Width * bmp.Height * 4];
            byte* srcBase = (byte*)data.Scan0;
            int dstIndex = 0;

            for (int y = 0; y < bmp.Height; y++)
            {
                byte* srcRow = srcBase + y * data.Stride;

                for (int x = 0; x < bmp.Width; x++)
                {
                    byte b = srcRow[x * 4 + 0];
                    byte g = srcRow[x * 4 + 1];
                    byte r = srcRow[x * 4 + 2];
                    byte a = srcRow[x * 4 + 3];

                    rgba[dstIndex + 0] = r;
                    rgba[dstIndex + 1] = g;
                    rgba[dstIndex + 2] = b;
                    rgba[dstIndex + 3] = a;
                    dstIndex += 4;
                }
            }

            return rgba;
        }
        finally
        {
            bmp.UnlockBits(data);
        }
    }

    private static unsafe uint UploadRgbaTexture(int width, int height, byte[] rgba, bool mipmaps)
    {
        GL.GenTextures(1, out uint tex);
        GL.BindTexture(GL.GL_TEXTURE_2D, tex);
        GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, GL.GL_REPEAT);
        GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, GL.GL_REPEAT);
        GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, mipmaps ? GL.GL_LINEAR_MIPMAP_LINEAR : GL.GL_LINEAR);
        GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_LINEAR);

        fixed (byte* p = rgba)
        {
            GL.TexImage2D(
                GL.GL_TEXTURE_2D,
                0,
                GL.GL_RGBA8,
                width,
                height,
                0,
                GL.GL_RGBA,
                GL.GL_UNSIGNED_BYTE,
                (IntPtr)p);
        }

        if (mipmaps)
        {
            GL.GenerateMipmap(GL.GL_TEXTURE_2D);
        }

        GL.BindTexture(GL.GL_TEXTURE_2D, 0);
        return tex;
    }
}

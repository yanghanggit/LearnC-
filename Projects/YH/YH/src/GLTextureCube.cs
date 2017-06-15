using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using StbSharp;
using System.IO;

namespace YH
{
    public class GLTextureCube
    {
        public GLTextureCube(string right, string left, string top, string bottom, string back, string front)
        {
            mFaces[0] = right;
			mFaces[1] = left;
			mFaces[2] = top;
			mFaces[3] = bottom;
			mFaces[4] = back;
			mFaces[5] = front;

            mTextureCubeId = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindTexture(TextureTarget.TextureCubeMap, mTextureCubeId);

            for (var i = 0; i < mFaces.Length; ++i)
            {
                var texPath = mFaces[i];

				ImageReader loader = new ImageReader();
				texPath = texPath.Replace(@"\\", @"/");
				texPath = texPath.Replace(@"\", @"/");
				bool isPNG = texPath.EndsWith(@".png");

				using (System.IO.Stream stream = File.Open(texPath, FileMode.Open))
				{
					StbSharp.Image image = loader.Read(stream, isPNG ? Stb.STBI_rgb_alpha : Stb.STBI_rgb);
					GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
							  0,
							  isPNG ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb,
							  image.Width,
							  image.Height,
							  0,
							  isPNG ? OpenTK.Graphics.OpenGL.PixelFormat.Rgba : OpenTK.Graphics.OpenGL.PixelFormat.Rgb,
							  PixelType.UnsignedByte,
							  image.Data);
				}
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (float)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (float)TextureWrapMode.ClampToEdge);
			GL.BindTexture(TextureTarget.TextureCubeMap, 0);
		}

        private string[] mFaces = new string[6];
        public int mTextureCubeId = 0;
    }
}

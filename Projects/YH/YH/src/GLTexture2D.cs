using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using StbSharp;
using System.IO;

namespace YH
{
	public class GLTexture2D
	{
		public GLTexture2D(string texPath)
		{
			LoadFromPath(texPath);
		}

		public void LoadFromPath(string texPath)
		{
			ImageReader loader = new ImageReader();
			texPath = texPath.Replace(@"\\", @"/");
			texPath = texPath.Replace(@"\", @"/");


            mIsPNG = texPath.EndsWith(@".png");

			using (System.IO.Stream stream = File.Open(texPath, FileMode.Open))
			{
				StbSharp.Image image = loader.Read(stream, mIsPNG ? Stb.STBI_rgb_alpha : Stb.STBI_rgb);
				mTextureId = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, mTextureId);
				GL.TexImage2D(TextureTarget.Texture2D,
						  0,
                          mIsPNG ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb,
						  image.Width,
						  image.Height,
						  0,
						  mIsPNG ? OpenTK.Graphics.OpenGL.PixelFormat.Rgba : OpenTK.Graphics.OpenGL.PixelFormat.Rgb,
						  PixelType.UnsignedByte,
						  image.Data);
				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
				GL.BindTexture(TextureTarget.Texture2D, 0);
			}
		}

		public int getTextureId()
		{
			return mTextureId;
		}

		private int mTextureId = 0;
        private bool mIsPNG = false;
	}
}

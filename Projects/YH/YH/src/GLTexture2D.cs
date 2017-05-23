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
			using (System.IO.Stream stream = File.Open(texPath, FileMode.Open))
			{
				StbSharp.Image image = loader.Read(stream, Stb.STBI_rgb_alpha);
				mTextureId = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, mTextureId);
				GL.TexImage2D(TextureTarget.Texture2D,
						  0,
						  PixelInternalFormat.Rgba,
						  image.Width,
						  image.Height,
						  0,
						  OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
						  PixelType.UnsignedByte,
						  image.Data);
				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
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
	}
}

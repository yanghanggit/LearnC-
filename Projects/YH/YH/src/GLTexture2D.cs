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
        public GLTexture2D(string texPath, bool wrapRepeat = true, bool gammaCorrection = false)
		{
			LoadFromPath(texPath, wrapRepeat, gammaCorrection);
		}

		public void LoadFromPath(string texPath, bool repeatOrClampToEdge, bool gammaCorrection)
		{
			ImageReader loader = new ImageReader();
			texPath = texPath.Replace(@"\\", @"/");
			texPath = texPath.Replace(@"\", @"/");

            bool isPng = texPath.EndsWith(@".png");

			using (System.IO.Stream stream = File.Open(texPath, FileMode.Open))
			{
				StbSharp.Image image = loader.Read(stream, isPng ? Stb.STBI_rgb_alpha : Stb.STBI_rgb);
				mTextureId = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, mTextureId);
				
                if (gammaCorrection)
                {
					//glTexImage2D(GL_TEXTURE_2D, 0, gammaCorrection ? GL_SRGB : GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, image);
					GL.TexImage2D(TextureTarget.Texture2D,
								  0,
                                  isPng ? PixelInternalFormat.SrgbAlpha : PixelInternalFormat.Srgb,
								  image.Width,
								  image.Height,
								  0,
                                  isPng ? OpenTK.Graphics.OpenGL.PixelFormat.Rgba : OpenTK.Graphics.OpenGL.PixelFormat.Rgb,
								  PixelType.UnsignedByte,
								  image.Data);
                }
                else 
                {
					GL.TexImage2D(TextureTarget.Texture2D,
								  0,
								  isPng ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb,
								  image.Width,
								  image.Height,
								  0,
								  isPng ? OpenTK.Graphics.OpenGL.PixelFormat.Rgba : OpenTK.Graphics.OpenGL.PixelFormat.Rgb,
								  PixelType.UnsignedByte,
								  image.Data);
                }

				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                GL.TexParameter(TextureTarget.Texture2D,
                                TextureParameterName.TextureWrapS,
                                repeatOrClampToEdge ? (int)TextureWrapMode.Repeat : (int)TextureWrapMode.ClampToEdge);
                
                GL.TexParameter(TextureTarget.Texture2D, 
                                TextureParameterName.TextureWrapT, 
                                repeatOrClampToEdge ? (int)TextureWrapMode.Repeat : (int)TextureWrapMode.ClampToEdge);
                
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				
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

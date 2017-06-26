﻿﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace YH
{
	public class GLDepthMapFramebuffer
	{
        public enum Type
        {
            TEXTURE_2D = 0,
            TEXTURE_CUBE
        }

        public GLDepthMapFramebuffer(int w, int h, Vector4 borderColor, Type type)
		{
            if (type == Type.TEXTURE_2D)
            {
                build2D(w, h, borderColor);
            }
            else if (type == Type.TEXTURE_CUBE)
			{
				buildCube(w, h, borderColor);
			}
		}

        private void buildCube(int w, int h, Vector4 borderColor)
        {
			mWidth = w;
			mHeight = h;

			mDepthMapFramebufferId = GL.GenFramebuffer();

            mDepthMap = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, mDepthMap);

			for (int i = 0; i < 6; ++i)
            {
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                              0, 
                              PixelInternalFormat.DepthComponent,
                              w, h, 
                              0,
                              PixelFormat.DepthComponent,
                              PixelType.Float,
                              IntPtr.Zero);

            }
			

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
			GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mDepthMapFramebufferId);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, mDepthMap, 0);
			GL.DrawBuffer(DrawBufferMode.None);
			GL.ReadBuffer(ReadBufferMode.None);
			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("ERROR::FRAMEBUFFER:: GLDepthMapFramebuffer is not complete!");
			}
			else
			{

			}

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private void build2D(int w, int h, Vector4 borderColor)
        {
			mWidth = w;
			mHeight = h;

			mDepthMapFramebufferId = GL.GenFramebuffer();

			mDepthMap = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, mDepthMap);

			GL.TexImage2D(TextureTarget.Texture2D,
						  0,
						  PixelInternalFormat.DepthComponent,
						  w, h,
						  0,
						  PixelFormat.DepthComponent,
						  PixelType.Float,
						  IntPtr.Zero);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

			float[] color = { borderColor.X, borderColor.Y, borderColor.Z, borderColor.W };
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, color);
			GL.BindTexture(TextureTarget.Texture2D, 0);

			//
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, mDepthMapFramebufferId);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, mDepthMap, 0);
			GL.DrawBuffer(DrawBufferMode.None);
			GL.ReadBuffer(ReadBufferMode.None);

			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("ERROR::FRAMEBUFFER:: GLDepthMapFramebuffer is not complete!");
			}
			else
			{

			}

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public int mDepthMapFramebufferId = 0;
        public int mDepthMap = 0;
        public int mWidth = 0;
		public int mHeight = 0;
	}
}

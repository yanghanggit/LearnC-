﻿﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class GLDepthMapFramebuffer
	{
        public GLDepthMapFramebuffer(int w, int h, Vector4 borderColor)
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

        public readonly int mDepthMapFramebufferId = 0;
        public readonly int mDepthMap = 0;
        public readonly int mWidth = 0;
		public readonly int mHeight = 0;
	}
}

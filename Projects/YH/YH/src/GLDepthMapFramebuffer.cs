﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class GLDepthMapFramebuffer
	{
        public GLDepthMapFramebuffer(int w, int h, Vector4 borderColor)
		{
            width = w;
            height = h;

            depthMapFBO = GL.GenFramebuffer();

            depthMap = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, depthMap);

            GL.TexImage2D(TextureTarget.Texture2D,
                          0, 
                          PixelInternalFormat.DepthComponent, 
                          w, h, 
                          0, 
                          PixelFormat.DepthComponent, 
                          PixelType.Float,
                          IntPtr.Zero);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            float[] color = { borderColor.X, borderColor.Y, borderColor.Z, borderColor.W };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, color);                    
			GL.BindTexture(TextureTarget.Texture2D, 0);

            //
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, depthMapFBO);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthMap, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

        public readonly int depthMapFBO = 0;
        public readonly int depthMap = 0;
        public readonly int width = 0;
		public readonly int height = 0;
	}
}

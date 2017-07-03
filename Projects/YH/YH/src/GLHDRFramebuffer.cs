using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace YH
{
    public class GLHDRFramebuffer
    {
        public GLHDRFramebuffer(int w, int h)
        {
            mHDRFBO = GL.GenFramebuffer();

			mColorBuffer = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, mColorBuffer);
            GL.TexImage2D(TextureTarget.Texture2D,
							0,
							PixelInternalFormat.Rgba16f,
							w, h,
							0,
							PixelFormat.Rgba,
							PixelType.Float,
							IntPtr.Zero);
			

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
			GL.BindTexture(TextureTarget.Texture2D, 0);

            mRboDepth = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, mRboDepth);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, w, h);
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mHDRFBO);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, 
                                    FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2D,
                                    mColorBuffer,
                                    0);
            
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                                       FramebufferAttachment.DepthAttachment,
                                       RenderbufferTarget.Renderbuffer,
                                       mRboDepth);


			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("ERROR::FRAMEBUFFER:: GLHDRFramebuffer is not complete!");
			}
			else
			{

			}

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

		}

		public int mHDRFBO = 0;
		public int mColorBuffer = 0;
        public int mRboDepth = 0;
		public int mWidth = 0;
		public int mHeight = 0;
    }
}

using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace YH
{
    public class GLFramebuffer
    {
        public GLFramebuffer(int width, int height)
        {
            mFrameBufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mFrameBufferId);

            mColorAttachment0 = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, mColorAttachment0);
            GL.TexImage2D(TextureTarget.Texture2D,
                          0,
                          PixelInternalFormat.Rgb, 
                          width, height,
                          0, 
                          OpenTK.Graphics.OpenGL.PixelFormat.Rgb,
                          PixelType.UnsignedByte,
                          IntPtr.Zero);
            
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                                    FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2D,
                                    mColorAttachment0, 
                                    0);
            
            mDepthAndStencilAttachment = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, mDepthAndStencilAttachment);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, mDepthAndStencilAttachment);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
           
            CheckFramebuffer();
		}

        private void CheckFramebuffer()
        {
            if (mFrameBufferId > 0)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, mFrameBufferId);

                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                {
                    Console.WriteLine("ERROR::FRAMEBUFFER:: Framebuffer is not complete!");
                }
                else 
                {
                    mIsValid = true;
                }

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }

		public bool IsValid()
		{
			return mIsValid;
		}

        public int mFrameBufferId = 0;
        public int mColorAttachment0 = 0;
        public int mDepthAndStencilAttachment = 0;
        private bool mIsValid = false;
    }
}

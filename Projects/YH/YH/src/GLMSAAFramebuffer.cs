using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;



namespace YH.src
{
    public class GLMSAAFramebuffer
    {
        public GLMSAAFramebuffer(int w, int h, int samples)
        {
			mWidth = w;
			mHeight = h;

            mTextureColorBufferMultiSampled = GenerateMultiSampleTexture(samples, w, h);

            mFramebufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, mFramebufferId);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                                    FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2DMultisample,
                                    mTextureColorBufferMultiSampled,
                                    0);


            int rbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer,
                                              samples,
                                              RenderbufferStorage.Depth24Stencil8, 
                                              w,
                                              h);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                                       FramebufferAttachment.DepthStencilAttachment,
                                       RenderbufferTarget.Renderbuffer, 
                                       rbo);
            
			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("ERROR::FRAMEBUFFER:: GLMSAAFramebuffer is not complete!");
			}


			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);


		}

        private int GenerateMultiSampleTexture(int samples, int w, int h)
		{
            int texture = GL.GenTexture();
			
            GL.BindTexture(TextureTarget.Texture2DMultisample, texture);

            GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample,
                                     samples,
                                     PixelInternalFormat.Rgb, 
                                     w, h, true);
            
			GL.BindTexture(TextureTarget.Texture2DMultisample, 0);
			return texture;
		}


        public int mFramebufferId = 0;
        private int mTextureColorBufferMultiSampled = 0;
        public int mWidth = 0;
		public int mHeight = 0;
	}
}

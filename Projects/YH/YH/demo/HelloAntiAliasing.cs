﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloAntiAliasing : Application
	{
		public HelloAntiAliasing() : base("HelloAntiAliasing")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mSphere = new Sphere();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);
			
            //mLightShader = new GLProgram(@"Resources/colors.vs", @"Resources/colors.frag");
            mShader = new GLProgram(@"Resources/anti_aliasing.vs", @"Resources/anti_aliasing.frag");

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Gray);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Multisample);

            InitMSAAFramebuffer(wnd.Width, wnd.Height, 4);
		}

        private void InitMSAAFramebuffer(int w, int h, int samples)
        {
            mMSAAFramebuffer = GL.GenBuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mMSAAFramebuffer);

            int textureColorBufferMultiSampled = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DMultisample, textureColorBufferMultiSampled);
			GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, samples, PixelInternalFormat.Rgb, w, h, true);
			GL.BindTexture(TextureTarget.Texture2DMultisample, 0);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                                    FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2DMultisample,
                                    textureColorBufferMultiSampled, 
                                    0);


            int rbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, samples, RenderbufferStorage.Depth24Stencil8, w, h);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
									  FramebufferAttachment.DepthStencilAttachment,
									  RenderbufferTarget.Renderbuffer,
									  rbo);
            
			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("ERROR::FRAMEBUFFER:: Framebuffer is not complete!");
			}

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
            if (mUseMSAA)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, mMSAAFramebuffer);
            }
            else 
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																 (float)wnd.Width / (float)wnd.Height,
																 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

            Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
               
			mShader.Use();

			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);
            GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

			if (mUseMSAA)
			{
				GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, mMSAAFramebuffer);
				GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
				GL.BlitFramebuffer(0, 0, wnd.Width, wnd.Height,
								   0, 0, wnd.Width, wnd.Height,
								   ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
			}







			/*
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
                                                                  (float)wnd.Width / (float)wnd.Height, 
                                                                  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			mLightShader.Use();

			GL.UniformMatrix4(mLightShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mLightShader.GetUniformLocation("view"), false, ref view);

			GL.Uniform3(mLightShader.GetUniformLocation("objectColor"), 1.0f, 0.5f, 0.31f);
			GL.Uniform3(mLightShader.GetUniformLocation("lightColor"), 1.0f, 0.5f, 1.0f);

			Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
			GL.UniformMatrix4(mLightShader.GetUniformLocation("model"), false, ref model);

			mCube.Draw();
			*/
		}

        public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (e.Key == OpenTK.Input.Key.C)
			{
				mUseMSAA = !mUseMSAA;
			}
		}

		private Cube mCube = null;
		private Sphere mSphere = null;
		private Camera mCamera = null;
        private GLProgram mShader = null;
        private int mMSAAFramebuffer = 0;
        private bool mUseMSAA = false;
	}
}

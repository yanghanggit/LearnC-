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
            mCamera.MovementSpeed /= 2.0f;

			mCameraController = new CameraController(mAppName, mCamera);
			
            //mLightShader = new GLProgram(@"Resources/colors.vs", @"Resources/colors.frag");
            mShader = new GLProgram(@"Resources/anti_aliasing.vs", @"Resources/anti_aliasing.frag");

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Gray);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Multisample);

            framebuffer = new GLMSAAFramebuffer(wnd.Width, wnd.Height, 4);


            mCubeTexture = new GLTexture2D(@"Resources/Texture/wall.jpg");
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
            if (mUseMSAA)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer.mFramebufferId);
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

			GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());
			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);
            GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

			if (mUseMSAA)
			{
                //GL.BindFramebuffer(FramebufferTarget., 0);
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, framebuffer.mFramebufferId);
                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

                GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
                GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

                GL.BlitFramebuffer(0, 0, wnd.Width, wnd.Height,
								   0, 0, wnd.Width, wnd.Height,
                                   ClearBufferMask.ColorBufferBit | ClearBufferMask.ColorBufferBit, 
                                   BlitFramebufferFilter.Linear);
			}
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
        private bool mUseMSAA = true;
        private GLMSAAFramebuffer framebuffer = null;
        private GLTexture2D mCubeTexture = null;
	}
}

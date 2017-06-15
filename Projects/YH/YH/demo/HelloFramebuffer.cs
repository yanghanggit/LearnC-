﻿﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Collections.Generic;

namespace YH
{
	public class HelloFramebuffer : Application
	{
		public HelloFramebuffer() : base("HelloFramebuffer")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mFloor = new Floor();
            mQuad = new Quad();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

            mShader = new GLProgram(@"Resources/framebuffers.vs", @"Resources/framebuffers.frag");
            mScreenShader = new GLProgram(@"Resources/framebuffers_screen.vs", @"Resources/framebuffers_screen.frag");
            mFramebuffer = new GLFramebuffer(wnd.Width, wnd.Height);

			mCubeTexture = new GLTexture2D(@"Resources/Texture/container.jpg");
			mFloorTexture = new GLTexture2D(@"Resources/Texture/metal.png");
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
            if (mUseFramebuffer)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, mFramebuffer.mFrameBufferId);
            }

			GL.Enable(EnableCap.DepthTest);
			GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																  (float)wnd.Width / (float)wnd.Height,
																  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

			mShader.Use();

			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);

            GL.BindTexture(TextureTarget.Texture2D, mFloorTexture.getTextureId());
			model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
            mFloor.Draw();

			GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());
			model = Matrix4.CreateTranslation(-1.0f, 0.0f, -1.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(2.0f, 0.0f, 0.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			if (mUseFramebuffer)
			{
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

				GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
				GL.Clear(ClearBufferMask.ColorBufferBit);
				GL.Disable(EnableCap.DepthTest);

				mScreenShader.Use();
				GL.BindTexture(TextureTarget.Texture2D, mFramebuffer.mColorAttachment0);
				mQuad.Draw();
				GL.BindTexture(TextureTarget.Texture2D, 0);
			}
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.Plus)
			{

			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{

			}
			else if (e.Key == OpenTK.Input.Key.C)
			{
                mUseFramebuffer = !mUseFramebuffer;
			}
		}

		private Cube mCube = null;
		private Floor mFloor = null;
        private Quad mQuad = null;

		private Camera mCamera = null;

		private GLTexture2D mCubeTexture = null;
		private GLTexture2D mFloorTexture = null;

		private GLProgram mShader = null; 
		private GLProgram mScreenShader = null; 
        private GLFramebuffer mFramebuffer = null;
        private bool mUseFramebuffer = false;
	}
}

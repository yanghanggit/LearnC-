﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloAdvancedLight : Application
	{
		public HelloAdvancedLight() : base("HelloAdvancedLight")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mSphere = new Sphere();
            mFloor = new Floor();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			//mLightShader = new GLProgram(@"Resources/colors.vs", @"Resources/colors.frag");
			mShader = new GLProgram(@"Resources/advanced_lighting.vs", @"Resources/advanced_lighting.frag");

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.ClearColor(Color.Black);
			GL.Enable(EnableCap.DepthTest);
			//GL.Enable(EnableCap.Multisample);

			//framebuffer = new GLMSAAFramebuffer(wnd.Width, wnd.Height, 4);


			//mCubeTexture = new GLTexture2D(@"Resources/Texture/wall.jpg");

			 mFloorTexture = new GLTexture2D(@"Resources/Texture/wood.png");
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
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
            GL.Uniform3(mShader.GetUniformLocation("lightPos"), mLightPosition);
            GL.Uniform3(mShader.GetUniformLocation("viewPos"), mCamera.Position);
            GL.Uniform1(mShader.GetUniformLocation("blinn"), 1);
            GL.BindTexture(TextureTarget.Texture2D, mFloorTexture.getTextureId());

            mFloor.Draw();
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (e.Key == OpenTK.Input.Key.C)
			{
				
			}
		}

		private Cube mCube = null;
		private Sphere mSphere = null;
		private Camera mCamera = null;
		private GLProgram mShader = null;
        private GLTexture2D mFloorTexture = null;
        private Floor mFloor = null;
        private Vector3 mLightPosition = new Vector3(0, 0, 0);
	}
}

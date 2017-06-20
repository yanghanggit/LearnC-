﻿using System;
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
			
            mLightShader = new GLProgram(@"Resources/colors.vs", @"Resources/colors.frag");

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Gray);
            GL.Enable(EnableCap.DepthTest);
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

			mLightShader.Use();

			GL.UniformMatrix4(mLightShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mLightShader.GetUniformLocation("view"), false, ref view);

			GL.Uniform3(mLightShader.GetUniformLocation("objectColor"), 1.0f, 0.5f, 0.31f);
			GL.Uniform3(mLightShader.GetUniformLocation("lightColor"), 1.0f, 0.5f, 1.0f);

			Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
			GL.UniformMatrix4(mLightShader.GetUniformLocation("model"), false, ref model);

			mCube.Draw();
		}

		private Cube mCube = null;
		private Sphere mSphere = null;
		private Camera mCamera = null;
		private GLProgram mLightShader = null;
	}
}

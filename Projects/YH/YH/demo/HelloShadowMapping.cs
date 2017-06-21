﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Collections.Generic;

namespace YH
{
	public class HelloShadowMapping : Application
	{
		public HelloShadowMapping() : base("HelloShadowMapping")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mFloor = new Floor();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);
			mShader = new GLProgram(@"Resources/advanced.vs", @"Resources/advanced.frag");
			mCubeTexture = new GLTexture2D(@"Resources/Texture/wood.png");
			mFloorTexture = new GLTexture2D(@"Resources/Texture/metal.png");

			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Less);
			GL.ClearColor(Color.Gray);
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
                                                                  (float)wnd.Width / (float)wnd.Height,
                                                                  0.1f, 100.0f);
            
			var view = mCamera.GetViewMatrix();

            mShader.Use();

			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);
            RenderScene(mShader);

            /*
			mShader.Use();

			GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());

			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);

			Matrix4 model = Matrix4.CreateTranslation(-1.0f, 0.0f, -1.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(2.0f, 0.0f, 0.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			GL.BindTexture(TextureTarget.Texture2D, mFloorTexture.getTextureId());
			model = Matrix4.CreateTranslation(0.0f, -0.5f, 0.0f);
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mFloor.Draw();
			*/
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

        void RenderScene(GLProgram shader)
		{
            Matrix4 model = Matrix4.CreateTranslation(0, 0, 0);

            GL.BindTexture(TextureTarget.Texture2D, mFloorTexture.getTextureId());

            model = Matrix4.CreateTranslation(0.0f, -0.5f, 0.0f);
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mFloor.Draw();

            GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());

			model = Matrix4.CreateTranslation(0.0f, 1.5f, 0.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

			model = Matrix4.CreateTranslation(2.0f, 0.0f, 1.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(-1.0f, 0.3f, 2.0f);
            model = Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.0f, 1.0f), 60.0f) * model;
            model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();
		}

		private Cube mCube = null;
		private Floor mFloor = null;
		private Camera mCamera = null;
		private GLProgram mShader = null;
		private GLTexture2D mCubeTexture = null;
		private GLTexture2D mFloorTexture = null;
	}
}

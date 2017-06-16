﻿﻿﻿﻿﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloUniformBufferObjects : Application
	{
		public HelloUniformBufferObjects() : base("HelloUniformBufferObjects")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mSphere = new Sphere();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

            mShaderRed = new GLProgram(@"Resources/uniform_buffers.vs", @"Resources/red.frag");
            mShaderGreen = new GLProgram(@"Resources/uniform_buffers.vs", @"Resources/green.frag");
            mShaderBlue = new GLProgram(@"Resources/uniform_buffers.vs", @"Resources/blue.frag");
            mShaderYellow = new GLProgram(@"Resources/uniform_buffers.vs", @"Resources/yellow.frag");

			//
			GL.UniformBlockBinding(mShaderRed.mProgram, GL.GetUniformBlockIndex(mShaderRed.mProgram, "Matrices"), 0);
			GL.UniformBlockBinding(mShaderGreen.mProgram, GL.GetUniformBlockIndex(mShaderGreen.mProgram, "Matrices"), 0);
			GL.UniformBlockBinding(mShaderBlue.mProgram, GL.GetUniformBlockIndex(mShaderBlue.mProgram, "Matrices"), 0);
			GL.UniformBlockBinding(mShaderYellow.mProgram, GL.GetUniformBlockIndex(mShaderYellow.mProgram, "Matrices"), 0);

			//
            mUboMatrices = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, mUboMatrices);
            GL.BufferData(BufferTarget.UniformBuffer, 2 * sizeof(float) * 4 * 4, IntPtr.Zero, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 0, mUboMatrices, IntPtr.Zero, 2 * sizeof(float) * 4 * 4);

            //
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom), 
                                                                  (float)wnd.Width / (float)wnd.Height,
                                                                  0.1f, 100.0f);

            GL.BindBuffer(BufferTarget.UniformBuffer, mUboMatrices);
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, sizeof(float) * 4 * 4, ref projection);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(Color.Black);
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			var view = mCamera.GetViewMatrix();
            Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

			GL.BindBuffer(BufferTarget.UniformBuffer, mUboMatrices);
            GL.BufferSubData(BufferTarget.UniformBuffer, (IntPtr)(sizeof(float) * 4 * 4), sizeof(float) * 4 * 4, ref view);
			GL.BindBuffer(BufferTarget.UniformBuffer, 0);

			mShaderRed.Use();
            model = Matrix4.CreateTranslation(-0.75f, 0.75f, 0.0f);
            model = Matrix4.CreateScale(0.5f) * model;
            GL.UniformMatrix4(mShaderRed.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

			mShaderGreen.Use();
			model = Matrix4.CreateTranslation(0.75f, 0.75f, 0.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(mShaderGreen.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			mShaderBlue.Use();
			model = Matrix4.CreateTranslation(-0.75f, -0.75f, 0.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(mShaderBlue.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			mShaderYellow.Use();
			model = Matrix4.CreateTranslation(0.75f, -0.75f, 0.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(mShaderYellow.GetUniformLocation("model"), false, ref model);
			mCube.Draw();
		}

		private Cube mCube = null;
		private Sphere mSphere = null;
		private Camera mCamera = null;
        private GLProgram mShaderRed = null;
		private GLProgram mShaderGreen = null;
		private GLProgram mShaderBlue = null;
		private GLProgram mShaderYellow = null;
        private int mUboMatrices = 0;
	}
}

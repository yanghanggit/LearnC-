﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloDepthTesting : Application
	{
		public HelloDepthTesting() : base("HelloDepthTesting")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);
			mShader = new GLProgram(@"Resources/advanced.vs", @"Resources/advanced.frag");
		    mCubeTexture = new GLTexture2D(@"Resources/Texture/wall.jpg"); ;
		    mFloorTexture = new GLTexture2D(@"Resources/Texture/metal.png"); ;
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Gray);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Enable(EnableCap.DepthTest);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)wnd.Width / (float)wnd.Height, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();
		}

		private Cube mCube = null;
		private Camera mCamera = null;
		private GLProgram mShader = null;
        private GLTexture2D mCubeTexture = null;
		private GLTexture2D mFloorTexture = null;
	}


}

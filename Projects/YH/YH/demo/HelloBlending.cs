﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Collections.Generic;

namespace YH
{
    public class IntegerDecreaseComparer : IComparer<float>
    {
        public int Compare(float x, float y)
        {
            return (x < y) ? 1 : -1;
        }
    }

	public class HelloBlending : Application
	{
		public HelloBlending() : base("HelloBlending")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mFloor = new Floor();
			mBillboard = new Billboard();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			mShader = new GLProgram(@"Resources/discard.vs", @"Resources/discard.frag");

			mCubeTexture = new GLTexture2D(@"Resources/Texture/marble.jpg");
			mFloorTexture = new GLTexture2D(@"Resources/Texture/metal.png");
			mTransparentTexture = new GLTexture2D(@"Resources/Texture/window.png", false);

			//
			GL.Enable(EnableCap.DepthTest);
			GL.ClearColor(Color.Gray);

            //
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}

		public override void Update(double dt)
		{
			base.Update(dt);

            mSortedDistance.Clear();
            foreach (var pos in mWindowsPositions)
            {
                float distance = (mCamera.Position - pos).LengthFast;
                mSortedDistance.Add(distance, pos);
            }
		}

		public override void Draw(double dt, Window wnd)
		{
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

			GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());
			model = Matrix4.CreateTranslation(-1.0f, 0.0f, -1.0f);
			//model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(2.0f, 0.0f, 0.0f);
			//model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			GL.BindTexture(TextureTarget.Texture2D, mFloorTexture.getTextureId());
			model = Matrix4.CreateTranslation(0.0f, -0.5f, 0.0f);
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mFloor.Draw();

			GL.BindTexture(TextureTarget.Texture2D, mTransparentTexture.getTextureId());
            foreach (var itr in mSortedDistance)
            {
				model = Matrix4.CreateTranslation(itr.Value);
				GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
				mBillboard.Draw();
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
				
			}
		}

		private Cube mCube = null;
		private Floor mFloor = null;
		private Billboard mBillboard = null;

		private Camera mCamera = null;

		private GLTexture2D mCubeTexture = null;
		private GLTexture2D mFloorTexture = null;
		private GLTexture2D mTransparentTexture = null;

		private GLProgram mShader = null;

		Vector3[] mWindowsPositions = {
			new Vector3(-1.5f,  0.0f, -0.48f),
			new Vector3( 1.5f,  0.0f,  0.51f),
			new Vector3( 0.0f,  0.0f,  0.7f),
			new Vector3(-0.3f,  0.0f, -2.3f),
			new Vector3( 0.5f,  0.0f, -0.6f)
		};

        private SortedDictionary<float, Vector3> mSortedDistance = new SortedDictionary<float, Vector3>(new IntegerDecreaseComparer());
	}
}

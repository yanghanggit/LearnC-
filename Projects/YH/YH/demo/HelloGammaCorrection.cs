﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloGammaCorrection : Application
	{
		public HelloGammaCorrection() : base("HelloGammaCorrection")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mFloor = new Floor();
			mSphere = new Sphere();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 3.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			mShader = new GLProgram(@"Resources/gamma_correction.vs", @"Resources/gamma_correction.frag");
			mLampShader = new GLProgram(@"Resources/lamp.vs", @"Resources/lamp.frag");

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Black);
			GL.Enable(EnableCap.DepthTest);

			mFloorTexture = new GLTexture2D(@"Resources/Texture/wood.png");

			// Load textures
			floorTexture = new GLTexture2D(@"Resources/Texture/wood.png");//loadTexture(FileSystem::getPath("resources/textures/wood.png").c_str(), false);
			floorTextureGammaCorrected = new GLTexture2D(@"Resources/Texture/wood.png", true , true);//loadTexture(FileSystem::getPath("resources/textures/wood.png").c_str(), true);
	    }

		public override void Update(double dt)
		{
			base.Update(dt);

			mLightPosition.X = 1.0f + (float)Math.Sin((float)mTotalRuningTime) * 2.0f;
			mLightPosition.Y = (float)Math.Sin((float)mTotalRuningTime / 2.0f) * 1.0f;
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																 (float)wnd.Width / (float)wnd.Height,
																 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			Matrix4 model = Matrix4.CreateTranslation(0.0f, -0.5f, 0.0f);

			mShader.Use();

            GL.BindTexture(TextureTarget.Texture2D,
                           gammaEnabled ? floorTextureGammaCorrected.getTextureId() : floorTexture.getTextureId());

			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);

			// Set light uniforms
			GL.Uniform3(mShader.GetUniformLocation("lightPositions"), ref lightPositions[0]);
			GL.Uniform3(mShader.GetUniformLocation("lightColors"), ref lightColors[0]);
			GL.Uniform3(mShader.GetUniformLocation("viewPos"), mCamera.Position);
            GL.Uniform1(mShader.GetUniformLocation("gamma"), gammaEnabled ? 1 : 0);

		
			mFloor.Draw();

			//
			//mLampShader.Use();
			//GL.UniformMatrix4(mLampShader.GetUniformLocation("projection"), false, ref projection);
			//GL.UniformMatrix4(mLampShader.GetUniformLocation("view"), false, ref view);
			//model = Matrix4.CreateTranslation(mLightPosition);
			//model = Matrix4.CreateScale(0.2f) * model;
			//GL.UniformMatrix4(mLampShader.GetUniformLocation("model"), false, ref model);
			//mSphere.Draw();
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.Plus)
			{
				mMaterialShinness *= 2.0f;
				mMaterialShinness = mMaterialShinness >= 256.0f ? 256.0f : mMaterialShinness;
			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{
				mMaterialShinness /= 2.0f;
				mMaterialShinness = mMaterialShinness <= 2.0f ? 2.0f : mMaterialShinness;
			}
			else if (e.Key == OpenTK.Input.Key.C)
			{
				mUseBlinn = !mUseBlinn;
			}
		}

		private Camera mCamera = null;
		private GLProgram mShader = null;
		private GLTexture2D mFloorTexture = null;
		private Floor mFloor = null;
		private Vector3 mLightPosition = new Vector3(0, 1, 0);
		private bool mUseBlinn = true;
		private GLProgram mLampShader = null;
		private Sphere mSphere = null;
		private float mMaterialShinness = 32.0f;

		// Light sources
		Vector3[] lightPositions = {
			new Vector3(-3.0f, 0.0f, 0.0f),
			new Vector3(-1.0f, 0.0f, 0.0f),
			new Vector3( 1.0f, 0.0f, 0.0f),
			new Vector3( 3.0f, 0.0f, 0.0f)
	    };

		Vector3[] lightColors = {
			new Vector3(0.25f),
			new Vector3(0.50f),
			new Vector3(0.75f),
			new Vector3(1.00f)
	    };

		// Load textures
        private GLTexture2D floorTexture = null;//loadTexture(FileSystem::getPath("resources/textures/wood.png").c_str(), false);
		private GLTexture2D floorTextureGammaCorrected = null;//loadTexture(FileSystem::getPath("resources/textures/wood.png").c_str(), true);
		private bool gammaEnabled = false;

	}
}

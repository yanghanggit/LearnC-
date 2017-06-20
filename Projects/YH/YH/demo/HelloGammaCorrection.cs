﻿﻿﻿﻿﻿using System;
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

			// Load textures
			mFloorTexture = new GLTexture2D(@"Resources/Texture/wood.png");
			mFloorTextureGammaCorrected = new GLTexture2D(@"Resources/Texture/wood.png", true , true);

			//
			var rd = new Random(System.DateTime.Now.Millisecond);
			for (var i = 0; i < mLightColors.Length; ++i)
			{
				mLightColors[i] = new Vector3((float)rd.NextDouble(), (float)rd.NextDouble(), (float)rd.NextDouble());
			}
	    }

		public override void Update(double dt)
		{
			base.Update(dt);
			for (var i = 0; i < mLightColors.Length; ++i)
			{
                if (i == 0)
                {
                    mLightColors[i].X = ((float)Math.Cos((float)mTotalRuningTime));
                }
				else if (i == 1)
				{
					mLightColors[i].Y = ((float)Math.Cos((float)mTotalRuningTime));
				}
				else if (i == 2)
				{
					mLightColors[i].Z = ((float)Math.Cos((float)mTotalRuningTime));
				}
			}
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
                           mGammaEnabled ? mFloorTextureGammaCorrected.getTextureId() : mFloorTexture.getTextureId());

			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);

			// Set light uniforms
            GL.Uniform3(mShader.GetUniformLocation("lightPositions[0]"), mLightPositions[0]);
            GL.Uniform3(mShader.GetUniformLocation("lightPositions[1]"), mLightPositions[1]);
            GL.Uniform3(mShader.GetUniformLocation("lightPositions[2]"), mLightPositions[2]);
            GL.Uniform3(mShader.GetUniformLocation("lightPositions[3]"), mLightPositions[3]);

            //
			GL.Uniform3(mShader.GetUniformLocation("lightColors[0]"), mLightColors[0]);
			GL.Uniform3(mShader.GetUniformLocation("lightColors[1]"), mLightColors[1]);
			GL.Uniform3(mShader.GetUniformLocation("lightColors[2]"), mLightColors[2]);
			GL.Uniform3(mShader.GetUniformLocation("lightColors[3]"), mLightColors[3]);

            //
			GL.Uniform3(mShader.GetUniformLocation("viewPos"), mCamera.Position);
            GL.Uniform1(mShader.GetUniformLocation("gamma"), mGammaEnabled ? 1 : 0);
            GL.Uniform1(mShader.GetUniformLocation("shinness"), mMaterialShinness);

		    //
			mFloor.Draw();

			//
			mLampShader.Use();
			GL.UniformMatrix4(mLampShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mLampShader.GetUniformLocation("view"), false, ref view);

            for (var i = 0; i < mLightPositions.Length; ++i)
            {
                var pos = mLightPositions[i];
                model = Matrix4.CreateTranslation(pos);
                model = Matrix4.CreateScale(0.2f) * model;
                GL.UniformMatrix4(mLampShader.GetUniformLocation("model"), false, ref model);

                var color = mLightColors[i];
                GL.Uniform3(mLampShader.GetUniformLocation("set_color"), color);
         
                mSphere.Draw();
            }
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
				mGammaEnabled = !mGammaEnabled;
			}
		}

		private Camera mCamera = null;
		private GLProgram mShader = null;
		private Floor mFloor = null;
		private GLProgram mLampShader = null;
		private Sphere mSphere = null;
		private float mMaterialShinness = 32.0f;

		//Light sources
		Vector3[] mLightPositions = {
			new Vector3(-3.0f, 1.0f, 0.0f),
			new Vector3(-1.0f, 1.0f, 0.0f),
			new Vector3( 1.0f, 1.0f, 0.0f),
			new Vector3( 3.0f, 1.0f, 0.0f)
	    };

		Vector3[] mLightColors = {
			new Vector3(0.25f),
			new Vector3(0.50f),
			new Vector3(0.75f),
			new Vector3(1.00f)
	    };

		// Load textures
        private GLTexture2D mFloorTexture = null;
		private GLTexture2D mFloorTextureGammaCorrected = null;
		private bool mGammaEnabled = true;

	}
}

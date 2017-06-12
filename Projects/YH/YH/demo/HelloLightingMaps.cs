﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloLightingMaps : Application
	{
		public HelloLightingMaps() : base("HelloLightingMaps")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			mLightShader = new GLProgram(@"Resources/lighting_maps.vs", @"Resources/lighting_maps.frag");

			mLocLightModel = mLightShader.GetUniformLocation("model");
			mLocLightView = mLightShader.GetUniformLocation("view");
			mLocLightProjection = mLightShader.GetUniformLocation("projection");

            mLocLightViewPos = mLightShader.GetUniformLocation("viewPos");

			mLocLightPos = mLightShader.GetUniformLocation("light.position");
			mLocLightAmbient = mLightShader.GetUniformLocation("light.ambient");
			mLocLightDiffuse = mLightShader.GetUniformLocation("light.diffuse");
			mLocLightSpecular = mLightShader.GetUniformLocation("light.specular");

			mLocMaterialDiffuse = mLightShader.GetUniformLocation("material.diffuse");
			mLocMaterialSpecular = mLightShader.GetUniformLocation("material.specular");
            mLocMaterialEmission = mLightShader.GetUniformLocation("material.emission");
			mLocMaterialShininess = mLightShader.GetUniformLocation("material.shininess");

            mLocLightReverseSpecular = mLightShader.GetUniformLocation("reverse_specular");
            mLocMaterialUseEmissionMap = mLightShader.GetUniformLocation("use_emission_map");

			//
			mLampShader = new GLProgram(@"Resources/lamp.vs", @"Resources/lamp.frag");
			mLocLampModel = mLampShader.GetUniformLocation("model");
			mLocLampView = mLampShader.GetUniformLocation("view");
			mLocLampProjection = mLampShader.GetUniformLocation("projection");

			//
			mDiffuseMap = new GLTexture2D(@"Resources/Texture/container2.png");
			mSpecularMap = new GLTexture2D(@"Resources/Texture/container2_specular.png");
            mColorSpecularMap = new GLTexture2D(@"Resources/Texture/lighting_maps_specular_color.png");
            mEmissionMap = new GLTexture2D(@"Resources/Texture/matrix.jpg");
		}

		public override void Update(double dt)
		{
			base.Update(dt);

			mLightPos.X = 1.0f + (float)Math.Sin((float)mTotalRuningTime) * 2.0f;
			mLightPos.Y = (float)Math.Sin((float)mTotalRuningTime / 2.0f) * 1.0f;
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Enable(EnableCap.DepthTest);

            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom), (float)wnd.Width / (float)wnd.Height, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			do
			{
				mLightShader.Use();

                //
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D, mDiffuseMap.getTextureId());
				GL.Uniform1(mLocMaterialDiffuse, 0);

                //
				GL.ActiveTexture(TextureUnit.Texture1);

                if (mUseColorSpecularTexture)
                {
                    GL.BindTexture(TextureTarget.Texture2D, mColorSpecularMap.getTextureId());        

                }
                else 
                {
                    GL.BindTexture(TextureTarget.Texture2D, mSpecularMap.getTextureId());
                }
				
				GL.Uniform1(mLocMaterialSpecular, 1);

                //
				GL.ActiveTexture(TextureUnit.Texture2);
				GL.BindTexture(TextureTarget.Texture2D, mEmissionMap.getTextureId());
				GL.Uniform1(mLocMaterialEmission, 2);

                //
				GL.Uniform3(mLocLightPos, mLightPos.X, mLightPos.Y, mLightPos.Z);
				GL.Uniform3(mLocLightViewPos, mCamera.Position.X, mCamera.Position.Y, mCamera.Position.Z);

                //
				GL.Uniform3(mLocLightAmbient, 0.2f, 0.2f, 0.2f);
				GL.Uniform3(mLocLightDiffuse, 0.5f, 0.5f, 0.5f);
				GL.Uniform3(mLocLightSpecular, 1.0f, 1.0f, 1.0f);

                GL.Uniform1(mLocMaterialShininess, 32.0f);

                GL.Uniform1(mLocLightReverseSpecular, mReverseSpecular ? 1 : 0);
                GL.Uniform1(mLocMaterialUseEmissionMap, mUseEmissionMap ? 1 : 0);

				GL.UniformMatrix4(mLocLightProjection, false, ref projection);
				GL.UniformMatrix4(mLocLightView, false, ref view);

				Matrix4 model = Matrix4.CreateTranslation(0, 0, 0);
				model = Matrix4.CreateScale(0.5f) * model;
				GL.UniformMatrix4(mLocLightModel, false, ref model);
				mCube.Draw();
			}
			while (false);

			do
            {
				mLampShader.Use();

				GL.UniformMatrix4(mLocLampProjection, false, ref projection);
				GL.UniformMatrix4(mLocLampView, false, ref view);

				Matrix4 model = Matrix4.CreateTranslation(mLightPos.X, mLightPos.Y, mLightPos.Z);
				model = Matrix4.CreateScale(0.2f) * model;
				GL.UniformMatrix4(mLocLampModel, false, ref model);

				mCube.Draw();

			}
			while (false);
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.R)
			{
                mReverseSpecular = !mReverseSpecular;
			}
		    else if (e.Key == OpenTK.Input.Key.U)
			{
				mUseColorSpecularTexture = !mUseColorSpecularTexture;
			}
            else if (e.Key == OpenTK.Input.Key.C)
			{
				mUseEmissionMap = !mUseEmissionMap; 
			}
		}

		private Cube mCube = null;
		private Camera mCamera = null;

		//
		private GLProgram mLightShader = null;

		private int mLocLightModel = -1;
		private int mLocLightView = -1;
		private int mLocLightProjection = -1;
		private int mLocLightPos = -1;
		private int mLocLightViewPos = -1;
		private int mLocLightAmbient = -1;
		private int mLocLightDiffuse = -1;
		private int mLocLightSpecular = -1;
        private int mLocLightReverseSpecular = -1;

        //
		private int mLocMaterialDiffuse = -1;
		private int mLocMaterialSpecular = -1;
		private int mLocMaterialEmission = -1;
		private int mLocMaterialShininess = -1;
        private int mLocMaterialUseEmissionMap = -1;

		//
		private GLProgram mLampShader = null;
		private int mLocLampModel = -1;
		private int mLocLampView = -1;
		private int mLocLampProjection = -1;

        //
		private GLTexture2D mDiffuseMap = null;
		private GLTexture2D mSpecularMap = null;
        private GLTexture2D mColorSpecularMap = null;
		private GLTexture2D mEmissionMap = null;

		//
		private Vector3 mLightPos = new Vector3(1.2f, 1.0f, 2.0f);

        //
        private bool mReverseSpecular = false;
        private bool mUseColorSpecularTexture = false;
        private bool mUseEmissionMap = false;
	}
}

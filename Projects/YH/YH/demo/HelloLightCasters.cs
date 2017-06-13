﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloLightCasters : Application
	{
		public HelloLightCasters() : base("HelloLightCastersDirection")
		{
            
		}

        public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 3.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			mLightShader = new GLProgram(@"Resources/light_casters.vs", @"Resources/light_casters.frag");

			mDiffuseMap = new GLTexture2D(@"Resources/Texture/container2.png");
			mSpecularMap = new GLTexture2D(@"Resources/Texture/container2_specular.png");
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.ClearColor(Color.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Enable(EnableCap.DepthTest);

            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
                                                                  (float)wnd.Width / (float)wnd.Height, 
                                                                  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			do
			{
				mLightShader.Use();

				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D, mDiffuseMap.getTextureId());
				GL.Uniform1(mLightShader.GetUniformLocation("material.diffuse"), 0);

				GL.ActiveTexture(TextureUnit.Texture1);
				GL.BindTexture(TextureTarget.Texture2D, mSpecularMap.getTextureId());
				GL.Uniform1(mLightShader.GetUniformLocation("material.specular"), 1);

				GL.UniformMatrix4(mLightShader.GetUniformLocation("projection"), false, ref projection);
				GL.UniformMatrix4(mLightShader.GetUniformLocation("view"), false, ref view);

                GL.Uniform3(mLightShader.GetUniformLocation("viewPos"), mCamera.Position.X, mCamera.Position.Y, mCamera.Position.Z);

                GL.Uniform3(mLightShader.GetUniformLocation("light.position"), mCamera.Position.X, mCamera.Position.Y, mCamera.Position.Z);
				GL.Uniform3(mLightShader.GetUniformLocation("light.direction"), mCamera.Front.X, mCamera.Front.Y, mCamera.Front.Z);

                GL.Uniform1(mLightShader.GetUniformLocation("light.cutOff"), (float)Math.Cos(MathHelper.DegreesToRadians(12.5f * mCutOffScale)));
                GL.Uniform1(mLightShader.GetUniformLocation("light.outerCutOff"), (float)Math.Cos(MathHelper.DegreesToRadians(17.5f * mCutOffScale)));

                GL.Uniform1(mLightShader.GetUniformLocation("material.shininess"), 32.0f);

                GL.Uniform3(mLightShader.GetUniformLocation("light.ambient"), 0.1f, 0.1f, 0.1f);
				GL.Uniform3(mLightShader.GetUniformLocation("light.diffuse"), 0.8f, 0.8f, 0.8f);
				GL.Uniform3(mLightShader.GetUniformLocation("light.specular"), 1.0f, 1.0f, 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("light.constant"), 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("light.linear"), 0.09f);
				GL.Uniform1(mLightShader.GetUniformLocation("light.quadratic"), 0.032f);

				Vector3 axis = new Vector3(1.0f, 0.3f, 0.5f);

				for (int i = 0; i < mCubePositions.Length; ++i)
				{ 
					Matrix4 model = Matrix4.CreateTranslation(mCubePositions[i]);
					float angle = 20.0f * i;
					model = Matrix4.CreateScale(0.5f) * model;
					model = Matrix4.CreateFromAxisAngle(axis, angle) * model;
					GL.UniformMatrix4(mLightShader.GetUniformLocation("model"), false, ref model);
					mCube.Draw();
				}
			}
			while (false);
		}

        public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
            base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.Plus)
			{
                const float maxValue = 2.0f;
                mCutOffScale += 0.1f;
                mCutOffScale = mCutOffScale >= maxValue ? maxValue : mCutOffScale;
			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{
                const float minValue = 0.3f;
				mCutOffScale -= 0.1f;
                mCutOffScale = mCutOffScale <= minValue ? minValue : mCutOffScale;
			}
			else if (e.Key == OpenTK.Input.Key.P)
			{
				
			}
		}

        //
		private Cube mCube = null;
		private Camera mCamera = null;

		//
		private GLProgram mLightShader = null;

        //
		Vector3[] mCubePositions = {
		new Vector3( 0.0f,  0.0f,  0.0f),
		new Vector3( 2.0f,  5.0f, -15.0f),
		new Vector3(-1.5f, -2.2f, -2.5f),
		new Vector3(-3.8f, -2.0f, -12.3f),
		new Vector3( 2.4f, -0.4f, -3.5f),
		new Vector3(-1.7f,  3.0f, -7.5f),
		new Vector3( 1.3f, -2.0f, -2.5f),
		new Vector3( 1.5f,  2.0f, -2.5f),
		new Vector3( 1.5f,  0.2f, -1.5f),
		new Vector3(-1.3f,  1.0f, -1.5f) 
		};

		private GLTexture2D mDiffuseMap = null;
		private GLTexture2D mSpecularMap = null;
        private float mCutOffScale = 1.0f;
	}
}

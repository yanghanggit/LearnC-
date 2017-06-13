using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloMultipleLights : Application
	{
		public HelloMultipleLights() : base("HelloMultipleLights")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 3.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			mLightShader = new GLProgram(@"Resources/multiple_lights.vs", @"Resources/multiple_lights.frag");
            mLampShader = new GLProgram(@"Resources/lamp.vs", @"Resources/lamp.frag");

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
                GL.Uniform1(mLightShader.GetUniformLocation("material.shininess"), 32.0f);

				// Directional light
				GL.Uniform3(mLightShader.GetUniformLocation("dirLight.direction"), -0.2f, -1.0f, -0.3f);
				GL.Uniform3(mLightShader.GetUniformLocation("dirLight.ambient"), 0.05f, 0.05f, 0.05f);
				GL.Uniform3(mLightShader.GetUniformLocation("dirLight.diffuse"), 0.4f, 0.4f, 0.4f);
				GL.Uniform3(mLightShader.GetUniformLocation("dirLight.specular"), 0.5f, 0.5f, 0.5f);

				// Point light 1
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[0].position"), pointLightPositions[0].X, pointLightPositions[0].Y, pointLightPositions[0].Z);
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[0].ambient"), 0.05f, 0.05f, 0.05f);
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[0].diffuse"), 0.8f, 0.8f, 0.8f);
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[0].specular"), 1.0f, 1.0f, 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("pointLights[0].constant"), 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("pointLights[0].linear"), 0.09f);
				GL.Uniform1(mLightShader.GetUniformLocation("pointLights[0].quadratic"), 0.032f);
				// Point light 2
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[1].position"), pointLightPositions[1].X, pointLightPositions[1].Y, pointLightPositions[1].Z);
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[1].ambient"), 0.05f, 0.05f, 0.05f);
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[1].diffuse"), 0.8f, 0.8f, 0.8f);
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[1].specular"), 1.0f, 1.0f, 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("pointLights[1].constant"), 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("pointLights[1].linear"), 0.09f);
				GL.Uniform1(mLightShader.GetUniformLocation("pointLights[1].quadratic"), 0.032f);
				// Point light 3
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[2].position"), pointLightPositions[2].X, pointLightPositions[2].Y, pointLightPositions[2].Z);
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[2].ambient"), 0.05f, 0.05f, 0.05f);
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[2].diffuse"), 0.8f, 0.8f, 0.8f);
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[2].specular"), 1.0f, 1.0f, 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("pointLights[2].constant"), 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("pointLights[2].linear"), 0.09f);
				GL.Uniform1(mLightShader.GetUniformLocation("pointLights[2].quadratic"), 0.032f);
				// Point light 4
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[3].position"), pointLightPositions[3].X, pointLightPositions[3].Y, pointLightPositions[3].Z);
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[3].ambient"), 0.05f, 0.05f, 0.05f);
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[3].diffuse"), 0.8f, 0.8f, 0.8f);
				GL.Uniform3(mLightShader.GetUniformLocation("pointLights[3].specular"), 1.0f, 1.0f, 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("pointLights[3].constant"), 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("pointLights[3].linear"), 0.09f);
				GL.Uniform1(mLightShader.GetUniformLocation("pointLights[3].quadratic"), 0.032f);
				// SpotLight
                GL.Uniform3(mLightShader.GetUniformLocation("spotLight.position"), mCamera.Position.X, mCamera.Position.Y, mCamera.Position.Z);
				GL.Uniform3(mLightShader.GetUniformLocation("spotLight.direction"), mCamera.Front.X, mCamera.Front.Y, mCamera.Front.Z);
				GL.Uniform3(mLightShader.GetUniformLocation("spotLight.ambient"), 0.0f, 0.0f, 0.0f);
				GL.Uniform3(mLightShader.GetUniformLocation("spotLight.diffuse"), 1.0f, 1.0f, 1.0f);
				GL.Uniform3(mLightShader.GetUniformLocation("spotLight.specular"), 1.0f, 1.0f, 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("spotLight.constant"), 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("spotLight.linear"), 0.09f);
				GL.Uniform1(mLightShader.GetUniformLocation("spotLight.quadratic"), 0.032f);
				GL.Uniform1(mLightShader.GetUniformLocation("spotLight.cutOff"), (float)Math.Cos(MathHelper.DegreesToRadians(12.5f)));
				GL.Uniform1(mLightShader.GetUniformLocation("spotLight.outerCutOff"), (float)Math.Cos(MathHelper.DegreesToRadians(15.0f)));

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

            do
            {
                mLampShader.Use();

                GL.UniformMatrix4(mLampShader.GetUniformLocation("projection"), false, ref projection);
                GL.UniformMatrix4(mLampShader.GetUniformLocation("view"), false, ref view);

                for (var i = 0; i < 4; i++)
                {
                    Matrix4 model = Matrix4.CreateTranslation(pointLightPositions[i]);
                    model = Matrix4.CreateScale(0.2f) * model;
                    GL.UniformMatrix4(mLampShader.GetUniformLocation("model"), false, ref model);
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
        private GLProgram mLampShader = null;

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

		Vector3[] pointLightPositions = {
		new Vector3( 0.7f,  0.2f,  2.0f),
		new Vector3( 2.3f, -3.3f, -4.0f),
		new Vector3(-4.0f,  2.0f, -12.0f),
		new Vector3( 0.0f,  0.0f, -3.0f)
	    };

		private GLTexture2D mDiffuseMap = null;
		private GLTexture2D mSpecularMap = null;
		private float mCutOffScale = 1.0f;
	}
}

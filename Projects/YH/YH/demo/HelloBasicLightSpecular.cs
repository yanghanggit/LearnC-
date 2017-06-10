using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloBasicLightSpecular : Application
	{
		public HelloBasicLightSpecular() : base("HelloBasicLightSpecular")
		{
		}

		public override void Start()
		{
			base.Start();

			mCube = new Cube();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			//
			mLightShader = new GLProgram(@"Resources/basic_lighting.vs", @"Resources/basic_lighting.frag");
			mLocLightModel = mLightShader.GetUniformLocation("model");
			mLocLightView = mLightShader.GetUniformLocation("view");
			mLocLightProjection = mLightShader.GetUniformLocation("projection");
			mLocLightObjectColor = mLightShader.GetUniformLocation("objectColor");
			mLocLightColor = mLightShader.GetUniformLocation("lightColor");
			mLocLightPos = mLightShader.GetUniformLocation("lightPos");
			mLocLightViewPos = mLightShader.GetUniformLocation("viewPos");
            mLocLightShininess = mLightShader.GetUniformLocation("shininess");
			mLocUseWorldSpace = mLightShader.GetUniformLocation("worldspace");

            //
			mLampShader = new GLProgram(@"Resources/lamp.vs", @"Resources/lamp.frag");
			mLocLampModel = mLampShader.GetUniformLocation("model");
			mLocLampView = mLampShader.GetUniformLocation("view");
			mLocLampProjection = mLampShader.GetUniformLocation("projection");
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, int w, int h)
		{
			GL.Viewport(0, 0, w, h);
            GL.ClearColor(Color.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Enable(EnableCap.DepthTest);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)w / (float)h, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

            mLightPos.X = 1.0f + (float)Math.Sin((float)mTotalRuningTime)  * 2.0f;
			mLightPos.Y = (float)Math.Sin((float)mTotalRuningTime / 2.0f) * 1.0f;

			do
			{
				mLightShader.Use();

				GL.UniformMatrix4(mLocLightProjection, false, ref projection);
				GL.UniformMatrix4(mLocLightView, false, ref view);

				GL.Uniform3(mLocLightObjectColor, 1.0f, 0.5f, 0.31f);
				GL.Uniform3(mLocLightColor, 1.0f, 1.0f, 1.0f);
				GL.Uniform3(mLocLightPos, mLightPos.X, mLightPos.Y, mLightPos.Z);
				GL.Uniform3(mLocLightViewPos, mCamera.Position.X, mCamera.Position.Y, mCamera.Position.Z);
                GL.Uniform1(mLocLightShininess, mShininess);

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
			if (e.Key == OpenTK.Input.Key.Plus)
			{
                mShininess *= 2.0f;
				mShininess = mShininess >= 256.0f ? 256.0f : mShininess;
			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{
                mShininess /= 2.0f;
                mShininess = mShininess <= 2.0f ? 2.0f : mShininess;
			}
            else if (e.Key == OpenTK.Input.Key.C)
			{
			}
		}

		private Cube mCube = null;

		private Camera mCamera = null;

		//
		private GLProgram mLightShader = null;
		private int mLocLightModel = -1;
		private int mLocLightView = -1;
		private int mLocLightProjection = -1;
		private int mLocLightObjectColor = -1;
		private int mLocLightColor = -1;
		private int mLocLightPos = -1;
		private int mLocLightViewPos = -1;
        private int mLocLightShininess = -1;
		private int mLocUseWorldSpace = -1;


		//
		private GLProgram mLampShader = null;
		private int mLocLampModel = -1;
		private int mLocLampView = -1;
		private int mLocLampProjection = -1;	

        //
        private Vector3 mLightPos = new Vector3(1.2f, 1.0f, 2.0f);
        private float mShininess = 256.0f;
	}
}

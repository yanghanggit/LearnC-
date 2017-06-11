using System;
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

			//
			mLightShader = new GLProgram(@"Resources/basic_lighting_phong.vs", @"Resources/basic_lighting_phong.frag");
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

			//
			mProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f),
			(float)wnd.Width / (float)wnd.Height, 0.1f, 100.0f);

			//
			mCubeModel = Matrix4.CreateTranslation(0, 0, 0);
			mCubeModel = Matrix4.CreateScale(0.5f) * mCubeModel;
		}

		public override void Update(double dt)
		{
			base.Update(dt);

			mView = mCamera.GetViewMatrix();
			mLightPos.X = 1.0f + (float)Math.Sin((float)mTotalRuningTime) * 2.0f;
			mLightPos.Y = (float)Math.Sin((float)mTotalRuningTime / 2.0f) * 1.0f;
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Enable(EnableCap.DepthTest);
			DrawCube();
			DrawLamp();
		}

		private void DrawCube()
		{
			mLightShader.Use();

			GL.UniformMatrix4(mLocLightProjection, false, ref mProjection);
			GL.UniformMatrix4(mLocLightView, false, ref mView);

			if (mUseWorldSpace)
			{
				GL.Uniform3(mLocLightObjectColor, 1.0f, 0.5f, 0.31f);
			}
			else
			{
				GL.Uniform3(mLocLightObjectColor, 0.5f, 0.31f, 1.0f);
			}

			GL.Uniform3(mLocLightColor, 1.0f, 1.0f, 1.0f);
			GL.Uniform3(mLocLightPos, mLightPos.X, mLightPos.Y, mLightPos.Z);
			GL.Uniform3(mLocLightViewPos, mCamera.Position.X, mCamera.Position.Y, mCamera.Position.Z);
			GL.Uniform1(mLocLightShininess, mShininess);
			GL.Uniform1(mLocUseWorldSpace, mUseWorldSpace ? 1 : 0);
			GL.UniformMatrix4(mLocLightModel, false, ref mCubeModel);

			mCube.Draw();
		}

		private void DrawLamp()
		{
			mLampShader.Use();

			GL.UniformMatrix4(mLocLampProjection, false, ref mProjection);
			GL.UniformMatrix4(mLocLampView, false, ref mView);

			Matrix4 model = Matrix4.CreateTranslation(mLightPos.X, mLightPos.Y, mLightPos.Z);
			model = Matrix4.CreateScale(0.2f) * model;
			GL.UniformMatrix4(mLocLampModel, false, ref model);

			mCube.Draw();
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
				mUseWorldSpace = !mUseWorldSpace;
			}
		}

		//
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
		private bool mUseWorldSpace = true;
		private Matrix4 mProjection;
		private Matrix4 mView;
		private Matrix4 mCubeModel;

	}
}

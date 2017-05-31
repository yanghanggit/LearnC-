using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloColorScene : Application
	{
		public HelloColorScene() : base("HelloColorScene")
		{
		}

		public override void Start()
		{
			base.Start();

			mCube = new Cube();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 10.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			//
			//mLampShader = new GLProgram(@"Resources/lamp.vs", @"Resources/lamp.frag");
			//mLocLampModel = mLampShader.GetUniformLocation("model");
			//mLocLampView  = mLampShader.GetUniformLocation("view");
			//mLocLampProjection  = mLampShader.GetUniformLocation("projection");

			//
			mLightShader = new GLProgram(@"Resources/colors.vs", @"Resources/colors.frag");
			mLocLightModel = mLightShader.GetUniformLocation("model");
			mLocLightView = mLightShader.GetUniformLocation("view");
			mLocLightProjection = mLightShader.GetUniformLocation("projection");
			mLocLightObjectColor = mLightShader.GetUniformLocation("objectColor");
			mLocLightColor = mLightShader.GetUniformLocation("lightColor");
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, int w, int h)
		{
			GL.Viewport(0, 0, w, h);
			GL.ClearColor(Color.Gray);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Enable(EnableCap.DepthTest);

			mLightShader.Use();
			GL.Uniform3(mLocLightObjectColor, 1.0f, 0.5f, 0.31f);
			GL.Uniform3(mLocLightColor, 1.0f, 0.5f, 1.0f);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)w / (float)h, 0.1f, 100.0f);
			GL.UniformMatrix4(mLocLightProjection, false, ref projection);

			var view = mCamera.GetViewMatrix();
			GL.UniformMatrix4(mLocLightView, false, ref view);

			Matrix4 model = new Matrix4();
			GL.UniformMatrix4(mLocLightModel, false, ref model);

			mCube.Draw();
		}

		private Cube mCube = null;

		Camera mCamera = null;	

		//
		private GLProgram mLightShader = null;
		private int mLocLightModel = -1;
		private int mLocLightView = -1;
		private int mLocLightProjection = -1;
		private int mLocLightObjectColor = -1;
		private int mLocLightColor = -1;
	}
}

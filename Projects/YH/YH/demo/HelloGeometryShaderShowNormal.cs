using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloGeometryShaderShowNormal : Application
	{
		public HelloGeometryShaderShowNormal() : base("HelloGeometryShaderShowNormal")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);
			mCubeTexture = new GLTexture2D(@"Resources/Texture/wall.jpg");

			mShader = new GLProgram(@"Resources/explode.vs", @"Resources/explode.frag", @"Resources/explode.gs");

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Gray);
			GL.Enable(EnableCap.DepthTest);
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																  (float)wnd.Width / (float)wnd.Height,
																  0.1f, 100.0f);

			var view = mCamera.GetViewMatrix();
			Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

			mShader.Use();
			GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());
			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);
			model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
			//model = Matrix4.CreateScale(0.5f) * model;

			Vector3 axis = new Vector3(1.0f, 0.3f, 0.5f);
			float angle = (float)mTotalRuningTime;
			model = Matrix4.CreateFromAxisAngle(axis, angle) * model;

			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			GL.Uniform1(mShader.GetUniformLocation("time"), (float)mTotalRuningTime);
			mCube.Draw();
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

		private Cube mCube = null;
		private Camera mCamera = null;
		private GLTexture2D mCubeTexture = null;
		private GLProgram mShader = null;
	}

}


using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloGeometryShaderExplode : Application
	{
		public HelloGeometryShaderExplode() : base("HelloGeometryShaderExplode")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 3.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			mProgram = new GLProgram(@"Resources/explode.vs", @"Resources/explode.frag", @"Resources/explode.gs");

            mCube = new Cube();

            mDiffuseMap = new GLTexture2D(@"Resources/Texture/container2.png");

            GL.ClearColor(Color.Gray);
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																 (float)wnd.Width / (float)wnd.Height,
																 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

            mProgram.Use();

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, mDiffuseMap.getTextureId());

			Matrix4 model = Matrix4.CreateTranslation(0, 0, 0);
            GL.UniformMatrix4(mProgram.GetUniformLocation("model"), false, ref model);
            GL.Uniform1(mProgram.GetUniformLocation("time"), (float)mTotalRuningTime);
			mCube.Draw();
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

		private GLProgram mProgram = null;
        private Cube mCube = null;
		private Camera mCamera = null;
        private GLTexture2D mDiffuseMap = null;
	}
}

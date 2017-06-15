using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Collections.Generic;

namespace YH
{
	public class HelloBlending1 : Application
	{
		public HelloBlending1() : base("HelloBlending1")
		{
            
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mPlane = new Plane();
            mBillboard = new Billboard();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			mShader = new GLProgram(@"Resources/discard.vs", @"Resources/discard.frag");

			mCubeTexture = new GLTexture2D(@"Resources/Texture/marble.jpg");
			mFloorTexture = new GLTexture2D(@"Resources/Texture/metal.png");
            mTransparentTexture = new GLTexture2D(@"Resources/Texture/grass.png", false);
			
			//
			GL.Enable(EnableCap.DepthTest);
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

			Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

			mShader.Use();
			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);

            GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());
			model = Matrix4.CreateTranslation(-1.0f, 0.0f, -1.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

			model = Matrix4.CreateTranslation(2.0f, 0.0f, 0.0f);
			model = Matrix4.CreateScale(0.5f) * model;
            GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

            GL.BindTexture(TextureTarget.Texture2D, mFloorTexture.getTextureId());
			model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
            GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
            mPlane.Draw();

            GL.BindTexture(TextureTarget.Texture2D, mTransparentTexture.getTextureId());
            foreach(var p in mVegetationPositions)
            {
                model = Matrix4.CreateTranslation(p);
                GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
                mBillboard.Draw();
            }
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.Plus)
			{

			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{

			}
			else if (e.Key == OpenTK.Input.Key.C)
			{

            }
			else if (e.Key == OpenTK.Input.Key.Space)
			{

			}
		}

		private Cube mCube = null;
		private Plane mPlane = null;
        private Billboard mBillboard = null;

		private Camera mCamera = null;

		private GLTexture2D mCubeTexture = null;
		private GLTexture2D mFloorTexture = null;
		private GLTexture2D mTransparentTexture = null;

		private GLProgram mShader = null;

		Vector3[] mVegetationPositions = {
			new Vector3(-1.5f,  0.0f, -0.48f),
			new Vector3( 1.5f,  0.0f,  0.51f),
			new Vector3( 0.0f,  0.0f,  0.7f),
			new Vector3(-0.3f,  0.0f, -2.3f),
			new Vector3( 0.5f,  0.0f, -0.6f)
		};
	}
}

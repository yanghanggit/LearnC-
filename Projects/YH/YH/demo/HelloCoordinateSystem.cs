using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;


namespace YH
{
	public class HelloCoordinateSystem : Application
	{
		public HelloCoordinateSystem() : base("HelloCoordinateSystem")
		{
		}

		public override void Start()
		{
			base.Start();

			mProgram = new GLProgram(@"Resources/coordinate_systems.vs", @"Resources/coordinate_systems.fs");
			mLocation1 = mProgram.GetUniformLocation("ourTexture1");
			mLocation2 = mProgram.GetUniformLocation("ourTexture2");
			mModelLoc = mProgram.GetUniformLocation("model");
			mViewLoc = mProgram.GetUniformLocation("view");
			mProjectionlLoc = mProgram.GetUniformLocation("projection");

			mCube = new Cube();

			mTexture1 = new GLTexture2D(@"Resources/Texture/wood.png");
			mTexture2 = new GLTexture2D(@"Resources/Texture/awesomeface.png");

			mView = Matrix4.CreateTranslation(0.0f, 0.0f, -5.0f);
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

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, mTexture1.getTextureId());
			GL.Uniform1(mLocation1, 0);

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, mTexture2.getTextureId());
			GL.Uniform1(mLocation2, 1);

			mProgram.Use();

			mProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)w / (float)h, 0.1f, 100.0f);

			GL.UniformMatrix4(mViewLoc, false, ref mView);
			GL.UniformMatrix4(mProjectionlLoc, false, ref mProjection);

			Vector3 axis = new Vector3(1.0f, 0.3f, 0.5f);
			for (int i = 0; i < mPositions.Length; ++i)
			{
				Matrix4 model = new Matrix4();
				model = Matrix4.CreateTranslation(mPositions[i]);

				float angle = (i + 1) * (float)mTotalRuningTime;
				model = Matrix4.CreateFromAxisAngle(axis, angle) * model;

				GL.UniformMatrix4(mModelLoc, false, ref model);
				mCube.Draw();
			}
		}

		private GLProgram mProgram = null;
		private GLTexture2D mTexture1 = null;
		private GLTexture2D mTexture2 = null;
		private int mLocation1 = -1;
		private int mLocation2 = -1;
		private Cube mCube = null;
		private Matrix4 mView = new Matrix4();
		private Matrix4 mProjection = new Matrix4();
		private int mModelLoc = -1;
		private int mViewLoc = -1;
		private int mProjectionlLoc = -1;
		private Vector3[] mPositions = {
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

	}
}

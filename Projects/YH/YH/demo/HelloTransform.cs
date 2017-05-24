using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloTransform : Application
	{
		public HelloTransform() : base("HelloTransform")
		{
		}

		public override void Start()
		{
			base.Start();

			mProgram = new GLProgram(@"Resources/transform.vs", @"Resources/transform.fs");
			mLocation1 = mProgram.GetUniformLocation("ourTexture1");
			mLocation2 = mProgram.GetUniformLocation("ourTexture2");
			mLocationOfTransform = mProgram.GetUniformLocation("transform");

			mSimpleRectangle = new SimpleTextureRectangle();
			mTexture1 = new GLTexture2D(@"Resources/Texture/wood.png");
			mTexture2 = new GLTexture2D(@"Resources/Texture/awesomeface.png");
		}

		public override void Update()
		{
			//Vector3 pos = new Vector3(0.0f,0.0f,0.0f);
			//Matrix4 m4x4 = Matrix4.CreateTranslation(pos);
			//Matrix4.Translation(pos);
		}

		public override void Draw(int w, int h)
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

			Matrix4 transForm = Matrix4.CreateTranslation(mPosition);
			GL.UniformMatrix4(mLocationOfTransform, false, ref transForm);

			mSimpleRectangle.Draw();
		}

		private GLProgram mProgram = null;
		private SimpleTextureRectangle mSimpleRectangle = null;
		private GLTexture2D mTexture1 = null;
		private GLTexture2D mTexture2 = null;
		private int mLocation1 = -1;
		private int mLocation2 = -1;
		private Vector3 mPosition = new Vector3(0.0f, 0.5f, 0.0f);
		private int mLocationOfTransform = -1;
	}
}

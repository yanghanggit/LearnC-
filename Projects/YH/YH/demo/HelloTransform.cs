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

        public override void Start(Window wnd)
		{
			base.Start(wnd);

			mProgram = new GLProgram(@"Resources/transform.vs", @"Resources/transform.fs");
			mLocation1 = mProgram.GetUniformLocation("ourTexture1");
			mLocation2 = mProgram.GetUniformLocation("ourTexture2");
			mLocationOfTransform = mProgram.GetUniformLocation("transform");

			mSimpleRectangle = new SimpleTextureRectangle();
			mTexture1 = new GLTexture2D(@"Resources/Texture/wood.png");
			mTexture2 = new GLTexture2D(@"Resources/Texture/awesomeface.png");
		}

		public override void Update(double dt)
		{
			base.Update(dt);

			mRotation = (float)(-3.14 * base.mTotalRuningTime * 2.0f);

			mPosition += (mMoveDirection * (float)dt * 2.0f);

			bool cond1 = mMoveDirection.X > 0 && mPosition.X > 1.0;
			bool cond2 = mMoveDirection.X < 0 && mPosition.X < -1.0;
			if (cond1 || cond2)
			{
				mMoveDirection.X *= -1;
			}
		}

        public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
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
			transForm = Matrix4.CreateRotationZ(mRotation) * transForm;
			GL.UniformMatrix4(mLocationOfTransform, false, ref transForm);

			mSimpleRectangle.Draw();
		}

		private GLProgram mProgram = null;
		private SimpleTextureRectangle mSimpleRectangle = null;
		private GLTexture2D mTexture1 = null;
		private GLTexture2D mTexture2 = null;
		private int mLocation1 = -1;
		private int mLocation2 = -1;
		private Vector3 mPosition = new Vector3(0.5f, -0.5f, 0.0f);
		private int mLocationOfTransform = -1;
		private float mRotation = 0.0f;
		private Vector3 mMoveDirection = new Vector3(1.1f, 0.0f, 0.0f);
	}
}

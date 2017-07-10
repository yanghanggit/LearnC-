using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace YH
{
	//=============================================================================================
	public class HelloFont : Application
	{
		public HelloFont() : base("HelloFont")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);
			mProgram = new GLProgram(@"Resources/testshader.vert", @"Resources/testshader.frag");
			mSimpleRectangle = new SimpleRectangle();

			GL.ClearColor(Color.Gray);
			mTestPosition = wnd.Width / 2;
			mMoveSpeed = wnd.Width / 10;
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.Clear(ClearBufferMask.ColorBufferBit);

			mProgram.Use();
			GL.Uniform1(mProgram.GetUniformLocation("point_size"), mPointSize);
			GL.Uniform1(mProgram.GetUniformLocation("test_frag_coord"), mTestFragCoord ? 1 : 0);
			GL.Uniform1(mProgram.GetUniformLocation("test_middle"), mTestPosition);


			mSimpleRectangle.Draw();
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.Plus)
			{
				mPointSize += 10.0f;
				mTestPosition += mMoveSpeed;
			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{
				mPointSize -= 10.0f;
				mPointSize = mPointSize > 20.0f ? mPointSize : 20.0f;

				mTestPosition -= mMoveSpeed;
				mTestPosition = mTestPosition > 0.0f ? mTestPosition : 0.0f;
			}
			else if (e.Key == OpenTK.Input.Key.C)
			{
				if (mSimpleRectangle != null)
				{
					mSimpleRectangle.mDrawPoints = !mSimpleRectangle.mDrawPoints;

					if (mSimpleRectangle.mDrawPoints)
					{
						GL.Enable(EnableCap.ProgramPointSize);
					}
					else
					{
						GL.Disable(EnableCap.ProgramPointSize);
					}
				}
			}
			else if (e.Key == OpenTK.Input.Key.B)
			{
				mTestFragCoord = !mTestFragCoord;
			}
		}

		private GLProgram mProgram = null;
		private SimpleRectangle mSimpleRectangle = null;
		private float mPointSize = 100.0f;
		private bool mTestFragCoord = false;
		private float mTestPosition = 0.0f;
		private float mMoveSpeed = 0.0f;

	}
}


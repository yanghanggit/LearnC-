﻿
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace YH
{
	//=============================================================================================
	public class HelloTriangle : Application
	{
		public HelloTriangle() : base("HelloTriangle")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);
			mProgram = new GLProgram(@"Resources/testshader.vert", @"Resources/testshader.frag");
			mSimpleRectangle = new SimpleRectangle();

            GL.Enable(EnableCap.ProgramPointSize);
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Gray);
			GL.Clear(ClearBufferMask.ColorBufferBit);

			mProgram.Use();
            GL.Uniform1(mProgram.GetUniformLocation("point_size"), mPointSize);

			mSimpleRectangle.Draw();
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.Plus)
			{
                mPointSize += 10.0f;
			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{
                mPointSize -= 10.0f;
                mPointSize = mPointSize > 20.0f ? mPointSize : 20.0f;
			}
			else if (e.Key == OpenTK.Input.Key.C)
			{
                if (mSimpleRectangle != null)
                {
                    mSimpleRectangle.mDrawPoints = !mSimpleRectangle.mDrawPoints;
			    }
			}
		}

		private GLProgram mProgram = null;
        private SimpleRectangle mSimpleRectangle = null;
        private float mPointSize = 100.0f;
	}
}

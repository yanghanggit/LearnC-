
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
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Gray);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			mProgram.Use();
			mSimpleRectangle.Draw();
		}

		private GLProgram mProgram = null;
		private SimpleRectangle mSimpleRectangle = null;	
	}
}

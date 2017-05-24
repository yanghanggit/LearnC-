
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

		public override void Start()
		{
			base.Start();
			mProgram = new GLProgram(@"Resources/testshader.vert", @"Resources/testshader.frag");
			mSimpleRectangle = new SimpleRectangle();
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

			mProgram.Use();
			mSimpleRectangle.Draw();
		}

		private GLProgram mProgram = null;
		private SimpleRectangle mSimpleRectangle = null;	
	}
}

using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace YH
{
	//=============================================================================================
	public class HelloTexture2D : Application
	{
		public HelloTexture2D() : base("HelloTexture2D")
		{

		}

		public override void Start()
		{
			base.Start();
			mProgram = new GLProgram(@"Resources/testshader.vert", @"Resources/testshader.frag");
			mSimpleRectangle = new SimpleRectangle();
		}

		public override void Update()
		{

		}

		public override void Draw(int w, int h)
		{
			GL.Viewport(0, 0, w, h);
			GL.ClearColor(Color.Yellow);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			mProgram.Use();
			mSimpleRectangle.Draw();
		}

		private GLProgram mProgram = null;
		private SimpleRectangle mSimpleRectangle = null;	
	}
}

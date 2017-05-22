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
			base.Start(); //texture
			mProgram = new GLProgram(@"Resources/texture.vert", @"Resources/texture.frag");
			mSimpleRectangle = new SimpleTextureRectangle();
		}

		public override void Update()
		{

		}

		public override void Draw(int w, int h)
		{
			GL.Viewport(0, 0, w, h);
			GL.ClearColor(Color.Gray);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			mProgram.Use();
			mSimpleRectangle.Draw();
		}

		private GLProgram mProgram = null;
		private SimpleTextureRectangle mSimpleRectangle = null;	
	}
}

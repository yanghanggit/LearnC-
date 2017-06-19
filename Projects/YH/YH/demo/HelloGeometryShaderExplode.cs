using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace YH
{
	public class HelloGeometryShaderExplode : Application
	{
		public HelloGeometryShaderExplode() : base("HelloGeometryShaderExplode")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mProgram = new GLProgram(@"Resources/geometry_shader.vs", @"Resources/geometry_shader.frag", @"Resources/geometry_shader.gs");
			createVAO();
			GL.ClearColor(Color.Black);
		}

		private void createVAO()
		{
			if (mVAO > 0)
			{
				return;
			}

			float[] vertices = {
			-0.5f,  0.5f, 1.0f, 0.0f, 0.0f, // Top-left
            0.5f,  0.5f, 0.0f, 1.0f, 0.0f, // Top-right
            0.5f, -0.5f, 0.0f, 0.0f, 1.0f, // Bottom-right
            -0.5f, -0.5f, 1.0f, 1.0f, 0.0f  // Bottom-left
            };

			mVAO = GL.GenVertexArray();
			int vbo = GL.GenBuffer();

			GL.BindVertexArray(mVAO);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (2 * sizeof(float)));

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0);
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
			GL.BindVertexArray(mVAO);
			GL.DrawArrays(PrimitiveType.Points, 0, 4);
			GL.BindVertexArray(0);
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

		private GLProgram mProgram = null;
		private int mVAO = 0;
	}
}

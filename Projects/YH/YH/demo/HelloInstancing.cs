using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloInstancing : Application
	{
		public HelloInstancing() : base("HelloInstancing")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);
	
            mProgram = new GLProgram(@"Resources/instancing_test.vs", @"Resources/instancing_test.fs");

            CreateVAO();

            InitTranslations();

            GL.ClearColor(Color.Black);
		}

        private void InitTranslations()
        {
			//
			mTranslations = new Vector2[100];
			int index = 0;
			float offset = 0.1f;
			for (int y = -10; y < 10; y += 2)
			{
				for (int x = -10; x < 10; x += 2)
				{
					Vector2 translation = new Vector2();
					translation.X = (float)x / 10.0f + offset;
					translation.Y = (float)y / 10.0f + offset;
					mTranslations[index++] = translation;
				}
			}
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
			for (int i = 0; i < 100; ++i)
			{
                var location = mProgram.GetUniformLocation("offsets[" + i + "]");
                GL.Uniform2(location, mTranslations[i]);
			}

            GL.BindVertexArray(mVAO);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, 100);
            GL.BindVertexArray(0);
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
            base.OnKeyUp(e);
		}

		private void CreateVAO()
		{
			if (mVAO > 0)
			{
				return;
			}

			float[] vertices = {
				//  ---位置---   ------颜色-------
				-0.05f,  0.05f,  1.0f, 0.0f, 0.0f,
				0.05f, -0.05f,  0.0f, 1.0f, 0.0f,
				-0.05f, -0.05f,  0.0f, 0.0f, 1.0f,

				-0.05f,  0.05f,  1.0f, 0.0f, 0.0f,
				0.05f, -0.05f,  0.0f, 1.0f, 0.0f,
				0.05f,  0.05f,  0.0f, 1.0f, 1.0f
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

		private GLProgram mProgram = null;
        private int mVAO = 0;
        private Vector2[] mTranslations = null;
	}
}

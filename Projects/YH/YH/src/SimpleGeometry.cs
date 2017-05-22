using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace YH
{
	public class SimpleGeometry
	{
		public SimpleGeometry(string name)
		{
			mName = name;
		}

		public virtual void Draw()
		{

		}

		public string mName = "SimpleGeometry";	
	}

	//=============================================================================================
	public class SimpleRectangle : SimpleGeometry
	{
		public SimpleRectangle() : base("SimpleRectangle")
		{

		}

		public override void Draw()
		{
			if (mVAO <= 0)
			{
				build();
			}

			if (mVAO > 0)
			{
				GL.BindVertexArray(mVAO);
				GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
				GL.BindVertexArray(0);
			}
		}

		private void build()
		{
			float[] vertices =
			{
				 0.5f,  0.5f, 0.0f,  // Top Right
			     0.5f, -0.5f, 0.0f,  // Bottom Right
			    -0.5f, -0.5f, 0.0f,  // Bottom Left
			    -0.5f,  0.5f, 0.0f   // Top Left 
			};

			int[] indices =
			{  // Note that we start from 0!
			    0, 1, 3,  // First Triangle
			    1, 2, 3   // Second Triangle
			};

			mVAO = GL.GenVertexArray();
			mVBO = GL.GenBuffer();
			mEBO = GL.GenBuffer();

			//
			GL.BindVertexArray(mVAO);
			GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, mEBO);
			GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticDraw);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
			GL.EnableVertexAttribArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0);
		}

		private int mVAO = 0;
		private int mVBO = 0;
		private int mEBO = 0;	
	}

	//=============================================================================================
	public class SimpleTextureRectangle : SimpleGeometry
	{
		public SimpleTextureRectangle() : base("SimpleTextureRectangle")
		{

		}

		public override void Draw()
		{
			if (mVAO <= 0)
			{
				build();
			}

			if (mVAO > 0)
			{
				GL.BindVertexArray(mVAO);
				GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
				GL.BindVertexArray(0);
			}
		}

		private void build()
		{
			float[] vertices =
			{
				// Positions          // Colors           // Texture Coords
		         0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f, // Top Right
		         0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,   1.0f, 0.0f, // Bottom Right
		        -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f, // Bottom Left
		        -0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f  // Top Left 
			};

			int[] indices =
			{  // Note that we start from 0!
				0, 1, 3, // First Triangle
				1, 2, 3  // Second Triangle
			};

			mVAO = GL.GenVertexArray();
			mVBO = GL.GenBuffer();
			mEBO = GL.GenBuffer();

			//
			GL.BindVertexArray(mVAO);
			GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, mEBO);
			GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticDraw);


			// Position attribute
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
			GL.EnableVertexAttribArray(0);

			// Color attribute
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (3 * sizeof(float)));
			GL.EnableVertexAttribArray(1);

			// TexCoord attribute
			GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (6 * sizeof(float)));
			GL.EnableVertexAttribArray(2);

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0); // Unbind VAO
		}

		private int mVAO = 0;
		private int mVBO = 0;
		private int mEBO = 0;	
	}
}

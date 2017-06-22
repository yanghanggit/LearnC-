﻿﻿﻿﻿﻿﻿
using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Collections.Generic;

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

		public virtual void DrawInstance(int amount)
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
                PrimitiveType pt = mDrawPoints ? PrimitiveType.Points : PrimitiveType.Triangles;
				GL.DrawElements(pt, 6, DrawElementsType.UnsignedInt, 0);
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
        public bool mDrawPoints = false;
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
	//=============================================================================================
	public class Cube : SimpleGeometry
	{
		public Cube() : base("Cube")
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
				GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
				GL.BindVertexArray(0);
			}
		}

		public override void DrawInstance(int amount)
		{
            if (mVAO > 0)
            {
				GL.BindVertexArray(mVAO);
				GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 36, amount);
				GL.BindVertexArray(0);
            }
		}

		public void build()
		{
			float[] vertices = 
			{
                // Back face
				-1.0f * mRadius, -1.0f * mRadius, -1.0f * mRadius,    0.0f, 0.0f,             0.0f, 0.0f, -1.0f,             // Bottom-left
				1.0f * mRadius, 1.0f * mRadius, -1.0f * mRadius,      1.0f, 1.0f,             0.0f, 0.0f, -1.0f,             // top-right
				1.0f * mRadius, -1.0f * mRadius, -1.0f * mRadius,     1.0f, 0.0f,             0.0f, 0.0f, -1.0f,             // bottom-right
				1.0f * mRadius, 1.0f * mRadius, -1.0f * mRadius,      1.0f, 1.0f,             0.0f, 0.0f, -1.0f,             // top-right
				-1.0f * mRadius, -1.0f * mRadius, -1.0f * mRadius,    0.0f, 0.0f,             0.0f, 0.0f, -1.0f,             // bottom-left
				-1.0f * mRadius, 1.0f * mRadius, -1.0f * mRadius,     0.0f, 1.0f,             0.0f, 0.0f, -1.0f,             // top-left
				// Front face
				-1.0f * mRadius, -1.0f * mRadius, 1.0f * mRadius,     0.0f, 0.0f,             0.0f, 0.0f, 1.0f,              // bottom-left
				1.0f * mRadius, -1.0f * mRadius, 1.0f * mRadius,      1.0f, 0.0f,             0.0f, 0.0f, 1.0f,              // bottom-right
				1.0f * mRadius, 1.0f * mRadius, 1.0f * mRadius,       1.0f, 1.0f,             0.0f, 0.0f, 1.0f,              // top-right
				1.0f * mRadius, 1.0f * mRadius, 1.0f * mRadius,       1.0f, 1.0f,             0.0f, 0.0f, 1.0f,              // top-right
				-1.0f * mRadius, 1.0f * mRadius, 1.0f * mRadius,      0.0f, 1.0f,             0.0f, 0.0f, 1.0f,              // top-left
				-1.0f * mRadius, -1.0f * mRadius, 1.0f * mRadius,     0.0f, 0.0f,             0.0f, 0.0f, 1.0f,              // bottom-left
				// Left face
				-1.0f * mRadius, 1.0f * mRadius, 1.0f * mRadius,      1.0f, 0.0f,             -1.0f, 0.0f, 0.0f,             // top-right
				-1.0f * mRadius, 1.0f * mRadius, -1.0f * mRadius,     1.0f, 1.0f,             -1.0f, 0.0f, 0.0f,             // top-left
				-1.0f * mRadius, -1.0f * mRadius, -1.0f * mRadius,    0.0f, 1.0f,             -1.0f, 0.0f, 0.0f,             // bottom-left
				-1.0f * mRadius, -1.0f * mRadius, -1.0f * mRadius,    0.0f, 1.0f,             -1.0f, 0.0f, 0.0f,             // bottom-left
				-1.0f * mRadius, -1.0f * mRadius, 1.0f * mRadius,     0.0f, 0.0f,             -1.0f, 0.0f, 0.0f,             // bottom-right
				-1.0f * mRadius, 1.0f * mRadius, 1.0f * mRadius,      1.0f, 0.0f,             -1.0f, 0.0f, 0.0f,             // top-right
				// Right face
				1.0f * mRadius, 1.0f * mRadius, 1.0f * mRadius,       1.0f, 0.0f,             1.0f, 0.0f, 0.0f,              // top-left
				1.0f * mRadius, -1.0f * mRadius, -1.0f * mRadius,     0.0f, 1.0f,             1.0f, 0.0f, 0.0f,              // bottom-right
				1.0f * mRadius, 1.0f * mRadius, -1.0f * mRadius,      1.0f, 1.0f,             1.0f, 0.0f, 0.0f,              // top-right
				1.0f * mRadius, -1.0f * mRadius, -1.0f * mRadius,     0.0f, 1.0f,             1.0f, 0.0f, 0.0f,              // bottom-right
				1.0f * mRadius, 1.0f * mRadius, 1.0f * mRadius,       1.0f, 0.0f,             1.0f, 0.0f, 0.0f,              // top-left
				1.0f * mRadius, -1.0f * mRadius, 1.0f * mRadius,      0.0f, 0.0f,             1.0f, 0.0f, 0.0f,              // bottom-left
				// Bottom face
				-1.0f * mRadius, -1.0f * mRadius, -1.0f * mRadius,    0.0f, 1.0f,             0.0f, -1.0f, 0.0f,             // top-right
				1.0f * mRadius, -1.0f * mRadius, -1.0f * mRadius,     1.0f, 1.0f,             0.0f, -1.0f, 0.0f,             // top-leftå
				1.0f * mRadius, -1.0f * mRadius, 1.0f * mRadius,      1.0f, 0.0f,             0.0f, -1.0f, 0.0f,             // bottom-left
				1.0f * mRadius, -1.0f * mRadius, 1.0f * mRadius,      1.0f, 0.0f,             0.0f, -1.0f, 0.0f,             // bottom-left
				-1.0f * mRadius, -1.0f * mRadius, 1.0f * mRadius,     0.0f, 0.0f,             0.0f, -1.0f, 0.0f,             // bottom-right
				-1.0f * mRadius, -1.0f * mRadius, -1.0f * mRadius,    0.0f, 1.0f,             0.0f, -1.0f, 0.0f,             // top-right
				// Top face
				-1.0f * mRadius, 1.0f * mRadius, -1.0f * mRadius,     0.0f, 1.0f,             0.0f, 1.0f, 0.0f,              // top-leftå
				1.0f * mRadius, 1.0f * mRadius, 1.0f * mRadius,       1.0f, 0.0f,             0.0f, 1.0f, 0.0f,              // bottom-right
				1.0f * mRadius, 1.0f * mRadius, -1.0f * mRadius,      1.0f, 1.0f,             0.0f, 1.0f, 0.0f,              // top-right
				1.0f * mRadius, 1.0f * mRadius, 1.0f * mRadius,       1.0f, 0.0f,             0.0f, 1.0f, 0.0f,              // bottom-right
				-1.0f * mRadius, 1.0f * mRadius, -1.0f * mRadius,     0.0f, 1.0f,             0.0f, 1.0f, 0.0f,              // top-left
				-1.0f * mRadius, 1.0f * mRadius, 1.0f * mRadius,      0.0f, 0.0f,             0.0f, 1.0f, 0.0f,              // bottom-left


                /*
				-1.0f, -1.0f, -1.0f,  0.0f, 0.0f, 0.0f,  0.0f, -1.0f,
				 1.0f,  1.0f, -1.0f,  1.0f, 1.0f, 0.0f,  0.0f, -1.0f,
				 1.0f, -1.0f, -1.0f,  1.0f, 0.0f, 0.0f,  0.0f, -1.0f,
				 1.0f,  1.0f, -1.0f,  1.0f, 1.0f, 0.0f,  0.0f, -1.0f,
				-1.0f, -1.0f, -1.0f,  0.0f, 0.0f, 0.0f,  0.0f, -1.0f,
				-1.0f,  1.0f, -1.0f,  0.0f, 1.0f, 0.0f,  0.0f, -1.0f,

				-1.0f, -1.0f,  1.0f,  0.0f, 0.0f, 0.0f,  0.0f,  1.0f,
				 1.0f, -1.0f,  1.0f,  1.0f, 0.0f, 0.0f,  0.0f,  1.0f,
				 1.0f,  1.0f,  1.0f,  1.0f, 1.0f, 0.0f,  0.0f,  1.0f,
				 1.0f,  1.0f,  1.0f,  1.0f, 1.0f, 0.0f,  0.0f,  1.0f,
				-1.0f,  1.0f,  1.0f,  0.0f, 1.0f, 0.0f,  0.0f,  1.0f,
				-1.0f, -1.0f,  1.0f,  0.0f, 0.0f, 0.0f,  0.0f,  1.0f,

				-1.0f,  1.0f,  1.0f,  1.0f, 0.0f, -1.0f,  0.0f,  0.0f,
				-1.0f,  1.0f, -1.0f,  1.0f, 1.0f, -1.0f,  0.0f,  0.0f,
				-1.0f, -1.0f, -1.0f,  0.0f, 1.0f, -1.0f,  0.0f,  0.0f,
				-1.0f, -1.0f, -1.0f,  0.0f, 1.0f, -1.0f,  0.0f,  0.0f,
				-1.0f, -1.0f,  1.0f,  0.0f, 0.0f, -1.0f,  0.0f,  0.0f,
				-1.0f,  1.0f,  1.0f,  1.0f, 0.0f, -1.0f,  0.0f,  0.0f,

				1.0f,  1.0f,  1.0f,  1.0f, 0.0f, 1.0f,  0.0f,  0.0f,
				1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 1.0f,  0.0f,  0.0f,
				1.0f,  1.0f, -1.0f,  1.0f, 1.0f, 1.0f,  0.0f,  0.0f,
				1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 1.0f,  0.0f,  0.0f,
				1.0f,  1.0f,  1.0f,  1.0f, 0.0f, 1.0f,  0.0f,  0.0f,
				1.0f, -1.0f,  1.0f,  0.0f, 0.0f, 1.0f,  0.0f,  0.0f,

				-1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 0.0f, -1.0f,  0.0f,
				 1.0f, -1.0f, -1.0f,  1.0f, 1.0f, 0.0f, -1.0f,  0.0f,
				 1.0f, -1.0f,  1.0f,  1.0f, 0.0f, 0.0f, -1.0f,  0.0f,
				 1.0f, -1.0f,  1.0f,  1.0f, 0.0f, 0.0f, -1.0f,  0.0f,
				-1.0f, -1.0f,  1.0f,  0.0f, 0.0f, 0.0f, -1.0f,  0.0f,
				-1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 0.0f, -1.0f,  0.0f,

				-1.0f,  1.0f, -1.0f,  0.0f, 1.0f, 0.0f,  1.0f,  0.0f,
				 1.0f,  1.0f,  1.0f,  1.0f, 0.0f, 0.0f,  1.0f,  0.0f,
				 1.0f,  1.0f, -1.0f,  1.0f, 1.0f, 0.0f,  1.0f,  0.0f,
				 1.0f,  1.0f,  1.0f,  1.0f, 0.0f, 0.0f,  1.0f,  0.0f,
				-1.0f,  1.0f, -1.0f,  0.0f, 1.0f, 0.0f,  1.0f,  0.0f,
                -1.0f,  1.0f,  1.0f,  0.0f, 0.0f,  0.0f,  1.0f,  0.0f
                */
			};

			mVAO = GL.GenVertexArray();
			mVBO = GL.GenBuffer();

			//
			GL.BindVertexArray(mVAO);
			GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

			// Position attribute
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

			// Color attribute
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (3 * sizeof(float)));

			GL.EnableVertexAttribArray(2);
			GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (5 * sizeof(float)));

			//
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0); // Unbind VAO
		}

        public int mVAO = 0;
		public int mVBO = 0;
        private float mRadius = 0.5f;
	}

	//=============================================================================================
	public class Floor : SimpleGeometry
	{
		public Floor() : base("Floor")
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
				GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
				GL.BindVertexArray(0);
			}
		}

		private void build()
		{
			float[] vertices =
			{
				// Positions          // Texture Coords  // Normals         
				8.0f, 0.0f,  8.0f,  5.0f, 0.0f, 0.0f, 1.0f, 0.0f,
				-8.0f, 0.0f,  8.0f,  0.0f, 0.0f, 0.0f, 1.0f, 0.0f,
				-8.0f, 0.0f, -8.0f,  0.0f, 5.0f, 0.0f, 1.0f, 0.0f,

				8.0f, 0.0f,  8.0f,  5.0f, 0.0f, 0.0f, 1.0f, 0.0f,
				-8.0f, 0.0f, -8.0f,  0.0f, 5.0f, 0.0f, 1.0f, 0.0f,
				8.0f, 0.0f, -8.0f,  5.0f, 5.0f, 0.0f, 1.0f, 0.0f,
			};

			mVAO = GL.GenVertexArray();
			mVBO = GL.GenBuffer();

			//
			GL.BindVertexArray(mVAO);
			GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

			// Position attribute
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

			// Color attribute
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (3 * sizeof(float)));


			GL.EnableVertexAttribArray(2);
			GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (5 * sizeof(float)));

			//
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0); // Unbind VAO
		}

		private int mVAO = 0;
		private int mVBO = 0;
	}

	//=============================================================================================
	public class Billboard : SimpleGeometry
	{
		public Billboard() : base("Billboard")
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
				GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
				GL.BindVertexArray(0);
			}
		}

		private void build()
		{
			float[] vertices =
			{
				// Positions         // Texture Coords (swapped y coordinates because texture is flipped upside down)
				0.0f,  0.5f,  0.0f,  0.0f,  0.0f,
				0.0f, -0.5f,  0.0f,  0.0f,  1.0f,
				1.0f, -0.5f,  0.0f,  1.0f,  1.0f,

				0.0f,  0.5f,  0.0f,  0.0f,  0.0f,
				1.0f, -0.5f,  0.0f,  1.0f,  1.0f,
				1.0f,  0.5f,  0.0f,  1.0f,  0.0f
			};

			mVAO = GL.GenVertexArray();
			mVBO = GL.GenBuffer();

			//
			GL.BindVertexArray(mVAO);
			GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

			// Position attribute
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

			// Color attribute
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (3 * sizeof(float)));

			//
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0); // Unbind VAO
		}

		private int mVAO = 0;
		private int mVBO = 0;
	}

	//=============================================================================================
	public class Quad : SimpleGeometry
	{
		public Quad() : base("Quad")
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
				GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
				GL.BindVertexArray(0);
			}
		}

		private void build()
		{
			float[] vertices =
			{
				// Positions   // TexCoords
				-1.0f,  1.0f,  0.0f, 1.0f,
				-1.0f, -1.0f,  0.0f, 0.0f,
				1.0f, -1.0f,  1.0f, 0.0f,

				-1.0f,  1.0f,  0.0f, 1.0f,
				1.0f, -1.0f,  1.0f, 0.0f,
				1.0f,  1.0f,  1.0f, 1.0f
			};

			mVAO = GL.GenVertexArray();
			mVBO = GL.GenBuffer();

			GL.BindVertexArray(mVAO);
			GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (2 * sizeof(float)));

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0);
		}

		private int mVAO = 0;
		private int mVBO = 0;
	}

	//=============================================================================================
	public class Sphere : SimpleGeometry
	{
		public Sphere() : base("Sphere")
		{
            
		}

		public override void Draw()
		{
            if (vao <= 0)
            {
                build();
            }

            if (vao > 0)
            {
                GL.BindVertexArray(vao);
                GL.DrawElements(PrimitiveType.Triangles, indexData.Count, DrawElementsType.UnsignedInt, 0);
				GL.BindVertexArray(0);
            }
		}

		private void build()
		{
			var vertexPositionData = new List<float>();
            var textureCoordData = new List<float>();
			var normalData = new List<float>();
            			
			for (var latNumber = 0; latNumber <= latitudeBands; latNumber++)
			{
				var theta = latNumber * Math.PI / latitudeBands;
				var sinTheta = Math.Sin(theta);
				var cosTheta = Math.Cos(theta);
			
                for (var longNumber = 0; longNumber <= longitudeBands; longNumber++)
				{
					var phi = longNumber * 2 * Math.PI / longitudeBands;
                    var sinPhi = Math.Sin(phi);
                    var cosPhi = Math.Cos(phi);
					var x = cosPhi * sinTheta;
					var y = cosTheta;
					var z = sinPhi * sinTheta;
					var u = 1 - (longNumber / longitudeBands);
					var v = latNumber / latitudeBands;

                    normalData.Add((float)x);
					normalData.Add((float)y);
					normalData.Add((float)z);

					textureCoordData.Add(u);
					textureCoordData.Add(v);

					vertexPositionData.Add((float)radius * (float)x);
					vertexPositionData.Add((float)radius * (float)y);
					vertexPositionData.Add((float)radius * (float)z);
				}
			}

            indexData = new List<int>();
			for (var latNumber = 0; latNumber < latitudeBands; latNumber++)
			{
				for (var longNumber = 0; longNumber < longitudeBands; longNumber++)
				{
					var first = (latNumber * (longitudeBands + 1)) + longNumber;
					var second = first + longitudeBands + 1;
					indexData.Add(first);
					indexData.Add(second);
					indexData.Add(first + 1);
					indexData.Add(second);
					indexData.Add(second + 1);
					indexData.Add(first + 1);
				}
			}


            if (true)
            {
				vao = GL.GenVertexArray();
				GL.BindVertexArray(vao);

                batchVBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, batchVBO);

                GL.BufferData(BufferTarget.ArrayBuffer, 
                              sizeof(float) * (vertexPositionData.Count + textureCoordData.Count + normalData.Count),
                              IntPtr.Zero,
                              BufferUsageHint.StaticDraw);

				GL.BufferSubData(BufferTarget.ArrayBuffer,
                                 (IntPtr)0,
                                 sizeof(float) * vertexPositionData.Count, 
                                 vertexPositionData.ToArray());
                
                GL.BufferSubData(BufferTarget.ArrayBuffer,
                                 (IntPtr)(sizeof(float) * vertexPositionData.Count),
                                 sizeof(float) * textureCoordData.Count, 
                                 textureCoordData.ToArray());
                
				GL.BufferSubData(BufferTarget.ArrayBuffer, 
                                 (IntPtr)(sizeof(float) * vertexPositionData.Count + sizeof(float) * textureCoordData.Count),
                                 sizeof(float) * normalData.Count, 
                                 normalData.ToArray());
                
				GL.EnableVertexAttribArray(0);
				GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

				GL.EnableVertexAttribArray(1);
				GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), (sizeof(float) * vertexPositionData.Count));

				GL.EnableVertexAttribArray(2);
				GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (sizeof(float) * vertexPositionData.Count + sizeof(float) * textureCoordData.Count));


                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

		
				vertexIndexBuffer = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexIndexBuffer);
				GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indexData.Count, indexData.ToArray(), BufferUsageHint.StaticDraw);

				GL.BindVertexArray(0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }
            else 
            {
				//
				vao = GL.GenVertexArray();
				GL.BindVertexArray(vao);

				//
				vertexPositionBuffer = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, vertexPositionBuffer);
				GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertexPositionData.Count, vertexPositionData.ToArray(), BufferUsageHint.StaticDraw);
				GL.EnableVertexAttribArray(0);
				GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

				//
				vertexTextureCoordBuffer = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, vertexTextureCoordBuffer);
				GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * textureCoordData.Count, textureCoordData.ToArray(), BufferUsageHint.StaticDraw);
				GL.EnableVertexAttribArray(1);
				GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

				//
				vertexNormalBuffer = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, vertexNormalBuffer);
				GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * normalData.Count, normalData.ToArray(), BufferUsageHint.StaticDraw);
				GL.EnableVertexAttribArray(2);
				GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

				//
				vertexIndexBuffer = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexIndexBuffer);
				GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indexData.Count, indexData.ToArray(), BufferUsageHint.StaticDraw);

		
				GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }
		}

        private int latitudeBands = 30;
		private int longitudeBands = 30;
		private int radius = 2;
		private int vertexPositionBuffer = 0;
		private int vertexNormalBuffer = 0;
		private int vertexTextureCoordBuffer = 0;
		private int vertexIndexBuffer = 0;
        private int vao = 0;
        private List<int> indexData = null;
        private int batchVBO = 0;
	}

	//=============================================================================================
	public class Skybox : SimpleGeometry
	{
		public Skybox() : base("Skybox")
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
				GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
				GL.BindVertexArray(0);
			}
		}

		private void build()
		{
			float[] vertices =
			{
				// Positions          
		        -1.0f, 1.0f, -1.0f,
				-1.0f, -1.0f, -1.0f,
				1.0f, -1.0f, -1.0f,
				1.0f, -1.0f, -1.0f,
				1.0f, 1.0f, -1.0f,
				-1.0f, 1.0f, -1.0f,

				-1.0f, -1.0f, 1.0f,
				-1.0f, -1.0f, -1.0f,
				-1.0f, 1.0f, -1.0f,
				-1.0f, 1.0f, -1.0f,
				-1.0f, 1.0f, 1.0f,
				-1.0f, -1.0f, 1.0f,

				1.0f, -1.0f, -1.0f,
				1.0f, -1.0f, 1.0f,
				1.0f, 1.0f, 1.0f,
				1.0f, 1.0f, 1.0f,
				1.0f, 1.0f, -1.0f,
				1.0f, -1.0f, -1.0f,

				-1.0f, -1.0f, 1.0f,
				-1.0f, 1.0f, 1.0f,
				1.0f, 1.0f, 1.0f,
				1.0f, 1.0f, 1.0f,
				1.0f, -1.0f, 1.0f,
				-1.0f, -1.0f, 1.0f,

				-1.0f, 1.0f, -1.0f,
				1.0f, 1.0f, -1.0f,
				1.0f, 1.0f, 1.0f,
				1.0f, 1.0f, 1.0f,
				-1.0f, 1.0f, 1.0f,
				-1.0f, 1.0f, -1.0f,

				-1.0f, -1.0f, -1.0f,
				-1.0f, -1.0f, 1.0f,
				1.0f, -1.0f, -1.0f,
				1.0f, -1.0f, -1.0f,
				-1.0f, -1.0f, 1.0f,
				1.0f, -1.0f, 1.0f
			};

			mVAO = GL.GenVertexArray();
			mVBO = GL.GenBuffer();

			GL.BindVertexArray(mVAO);
			GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0); 
		}

		private int mVAO = 0;
		private int mVBO = 0;
	}
}


using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;


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

		private void build()
		{
			float[] vertices = 
			{
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
				5.0f,  -0.5f,  5.0f,  2.0f, 0.0f,
				-5.0f, -0.5f,  5.0f,  0.0f, 0.0f,
				-5.0f, -0.5f, -5.0f,  0.0f, 2.0f,

				5.0f,  -0.5f,  5.0f,  2.0f, 0.0f,
				-5.0f, -0.5f, -5.0f,  0.0f, 2.0f,
				5.0f,  -0.5f, -5.0f,  2.0f, 2.0f
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
		public Sphere(float cx, float cy, float cz, float r, int p) : base("Sphere")
		{
			this.cx = cx;
			this.cy = cy;
			this.cz = cz;
			this.r = r;
			this.p = p;

			this.vertices = new float[p * 6 + 6];
            this.normals = new float[p * 6 + 6];
            this.texCoords = new float[p * 4 + 4];
		}

		public override void Draw()
		{
            if (!buildFinished)
            {
                buildFinished = true;
                build();
            }

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, this.vertices);

			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, this.texCoords);

			GL.EnableVertexAttribArray(2);
			GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, this.normals);
			
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, (this.p + 1) * 2);
		}

		private void build()
		{
            double theta1 = 0.0, theta2 = 0.0, theta3 = 0.0;
			double ex = 0.0, ey = 0.0, ez = 0.0;
			double px = 0.0, py = 0.0, pz = 0.0;
			//GLfloat vertices[p * 6 + 6], normals[p * 6 + 6], texCoords[p * 4 + 4];

			if (r < 0)
				r = -r;

			if (p < 0)
				p = -p;

			for (int i = 0; i < p / 2; ++i)
			{
				theta1 = i * (Math.PI * 2) / p - Math.PI;
				theta2 = (i + 1) * (Math.PI * 2) / p - Math.PI;

				for (int j = 0; j <= p; ++j)
				{
					theta3 = j * (Math.PI * 2) / p;

                    ex = Math.Cos(theta2) * Math.Cos(theta3);
                    ey = Math.Sin(theta2);
					ez = Math.Cos(theta2) * Math.Sin(theta3);
					px = cx + r * ex;
					py = cy + r * ey;
					pz = cz + r * ez;

					vertices[(6 * j) + (0 % 6)] = (float)px;
					vertices[(6 * j) + (1 % 6)] = (float)py;
					vertices[(6 * j) + (2 % 6)] = (float)pz;

					normals[(6 * j) + (0 % 6)] = (float)ex;
					normals[(6 * j) + (1 % 6)] = (float)ey;
					normals[(6 * j) + (2 % 6)] = (float)ez;

					texCoords[(4 * j) + (0 % 4)] = -(j / (float)p);
					texCoords[(4 * j) + (1 % 4)] = 2 * (i + 1) / (float)p;


					ex = Math.Cos(theta1) * Math.Cos(theta3);
					ey = Math.Sin(theta1);
					ez = Math.Cos(theta1) * Math.Sin(theta3);
					px = cx + r * ex;
					py = cy + r * ey;
					pz = cz + r * ez;

					vertices[(6 * j) + (3 % 6)] = (float)px;
					vertices[(6 * j) + (4 % 6)] = (float)py;
					vertices[(6 * j) + (5 % 6)] = (float)pz;

					normals[(6 * j) + (3 % 6)] = (float)ex;
					normals[(6 * j) + (4 % 6)] = (float)ey;
					normals[(6 * j) + (5 % 6)] = (float)ez;

					texCoords[(4 * j) + (2 % 4)] = -(j / (float)p);
					texCoords[(4 * j) + (3 % 4)] = 2 * i / (float)p;
				}
			}
		}

        private float cx = 0.0f;
        private float cy = 0.0f; 
        private float cz = 0.0f;
        private float r = 0.5f;
        private int p = 256;
        private float[] vertices = null;
        private float[] normals = null;
        private float[] texCoords = null;
        private bool buildFinished = false;
	}
}

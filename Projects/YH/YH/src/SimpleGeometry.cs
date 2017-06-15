﻿﻿﻿
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
                GL.DrawElements(PrimitiveType.Triangles, _indexData.Length, DrawElementsType.UnsignedInt, 0);
				GL.BindVertexArray(0);
            }
		}

		private void build()
		{
			var vertexPositionData = new List<float>();
			var normalData = new List<float>();
			var textureCoordData = new List<float>();

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

            _vertexPositionData = new float[vertexPositionData.Count];
            for (int i = 0; i < vertexPositionData.Count; ++i)
            {
                _vertexPositionData[i] = vertexPositionData[i];
            }

			_normalData = new float[normalData.Count];
			for (int i = 0; i < normalData.Count; ++i)
			{
				_normalData[i] = normalData[i];
			}

			_textureCoordData = new float[textureCoordData.Count];
			for (int i = 0; i < textureCoordData.Count; ++i)
			{
				_textureCoordData[i] = textureCoordData[i];
			}

            var indexData = new List<int>();
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

			_indexData = new int[indexData.Count];
			for (int i = 0; i < indexData.Count; ++i)
			{
				_indexData[i] = indexData[i];
			}

            mVAO = GL.GenVertexArray();
            GL.BindVertexArray(mVAO);

			//
			vertexPositionBuffer = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexPositionBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * _vertexPositionData.Length, _vertexPositionData, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
			
			//
			vertexTextureCoordBuffer = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexTextureCoordBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * _textureCoordData.Length, _textureCoordData, BufferUsageHint.StaticDraw);
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

			//
			vertexNormalBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexNormalBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * _normalData.Length, _normalData , BufferUsageHint.StaticDraw);
			GL.EnableVertexAttribArray(2);
			GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);

            //
			vertexIndexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexIndexBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * _indexData.Length, _indexData, BufferUsageHint.StaticDraw);

            //
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			GL.BindVertexArray(0);
		}


        private int latitudeBands = 30;
		private int longitudeBands = 30;
		private int radius = 2;
		private int vertexPositionBuffer = 0;
		private int vertexNormalBuffer = 0;
		private int vertexTextureCoordBuffer = 0;
		private int vertexIndexBuffer = 0;
        private int mVAO = 0;

        private float[] _vertexPositionData = null;
		private float[] _normalData = null;
		private float[] _textureCoordData = null;
        private int[] _indexData = null;
	}
}

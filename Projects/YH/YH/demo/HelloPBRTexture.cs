
using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using System.Collections.Generic;

namespace YH
{
	public class HelloPBRTexture : Application
	{
		public HelloPBRTexture() : base("HelloPBRTexture")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Black);
			GL.Enable(EnableCap.DepthTest);

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 20.0f), new Vector3(0.0f, 1.0f, 0.0f), -90.0f, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

            /*
			mShader = new GLProgram(@"Resources/1.1.pbr.vs", @"Resources/1.1.pbr.fs");
			mShader.Use();
			GL.Uniform3(mShader.GetUniformLocation("albedo"), 0.5f, 0.0f, 0.0f);
			GL.Uniform1(mShader.GetUniformLocation("ao"), 1.0f);

			mProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
															(float)wnd.Width / (float)wnd.Height,
															0.1f, 100.0f);

			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref mProjection);
			*/
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            /*
			mShader.Use();
			var view = mCamera.GetViewMatrix();
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);
			GL.Uniform3(mShader.GetUniformLocation("camPos"), mCamera.Position);


			var model = Matrix4.CreateTranslation(0, 0, 0);

			const int nrRows = 7;
			const int nrColumns = 7;
			const float spacing = 2.5f;

			for (int row = 0; row < nrRows; ++row)
			{
				GL.Uniform1(mShader.GetUniformLocation("metallic"), (float)row / (float)nrRows);

				for (int col = 0; col < nrColumns; ++col)
				{
					float v = (float)col / (float)nrColumns;
					v = (v < 0.05f) ? 0.05f : ((v > 1.0f) ? 1.0f : v);
					GL.Uniform1(mShader.GetUniformLocation("roughness"), v);

					model = Matrix4.CreateTranslation(
						(float)(col - (nrColumns / 2)) * spacing,
						(float)(row - (nrRows / 2)) * spacing,
						0.0f);

					GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);

					RenderSphere();
				}
			}

			for (int i = 0; i < mLightPositions.Length; ++i)
			{
				Vector3 newPos = Vector3.Add(mLightPositions[i], new Vector3((float)Math.Sin(mTotalRuningTime * 2.0f), (float)Math.Sin(mTotalRuningTime * 2.0f), 0.0f));

				GL.Uniform3(mShader.GetUniformLocation("lightPositions[" + i + "]"), newPos);
				GL.Uniform3(mShader.GetUniformLocation("lightColors[" + i + "]"), mLightColors[i]);

				model = Matrix4.CreateTranslation(newPos);
				model = Matrix4.CreateScale(0.5f) * model;
				GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);

				RenderSphere();
			}
			*/
		}

		private void RenderSphere()
		{
			if (mSphereVAO == 0)
			{
				mSphereVAO = GL.GenVertexArray();

				int vbo = GL.GenBuffer();
				int ebo = GL.GenBuffer();

				List<Vector3> positions = new List<Vector3>();
				List<Vector2> uv = new List<Vector2>();
				List<Vector3> normals = new List<Vector3>();
				List<int> indices = new List<int>();

				const int X_SEGMENTS = 64;
				const int Y_SEGMENTS = 64;
				const float PI = 3.14159265359f;

				for (int y = 0; y <= Y_SEGMENTS; ++y)
				{
					for (int x = 0; x <= X_SEGMENTS; ++x)
					{
						float xSegment = (float)x / (float)X_SEGMENTS;
						float ySegment = (float)y / (float)Y_SEGMENTS;

						float xPos = (float)Math.Cos(xSegment * 2.0f * PI) * (float)Math.Sin(ySegment * PI);
						float yPos = (float)Math.Cos(ySegment * PI);
						float zPos = (float)Math.Sin(xSegment * 2.0f * PI) * (float)Math.Sin(ySegment * PI);

						positions.Add(new Vector3(xPos, yPos, zPos));
						uv.Add(new Vector2(xSegment, ySegment));
						normals.Add(new Vector3(xPos, yPos, zPos));
					}
				}

				bool oddRow = false;
				for (int y = 0; y < Y_SEGMENTS; ++y)
				{
					if (!oddRow)
					{
						for (int x = 0; x <= X_SEGMENTS; ++x)
						{
							indices.Add(y * (X_SEGMENTS + 1) + x);
							indices.Add((y + 1) * (X_SEGMENTS + 1) + x);
						}
					}
					else
					{
						for (int x = X_SEGMENTS; x >= 0; --x)
						{
							indices.Add((y + 1) * (X_SEGMENTS + 1) + x);
							indices.Add(y * (X_SEGMENTS + 1) + x);
						}
					}
					oddRow = !oddRow;
				}

				mIndexCount = indices.Count;

				List<float> data = new List<float>();

				for (int i = 0; i < positions.Count; ++i)
				{
					data.Add(positions[i].X);
					data.Add(positions[i].Y);
					data.Add(positions[i].Z);

					if (uv.Count > 0)
					{
						data.Add(uv[i].X);
						data.Add(uv[i].Y);
					}

					if (normals.Count > 0)
					{
						data.Add(normals[i].X);
						data.Add(normals[i].Y);
						data.Add(normals[i].Z);
					}
				}

				GL.BindVertexArray(mSphereVAO);
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
				GL.BufferData(BufferTarget.ArrayBuffer, data.Count * sizeof(float), data.ToArray(), BufferUsageHint.StaticDraw);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
				GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);

				const int stride = (3 + 2 + 3) * sizeof(float);
				GL.EnableVertexAttribArray(0);
				GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, IntPtr.Zero);

				GL.EnableVertexAttribArray(1);
				GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, (IntPtr)(3 * sizeof(float)));

				GL.EnableVertexAttribArray(2);
				GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, (IntPtr)(5 * sizeof(float)));

				GL.BindVertexArray(0);
			}

			GL.BindVertexArray(mSphereVAO);
			GL.DrawElements(PrimitiveType.TriangleStrip, mIndexCount, DrawElementsType.UnsignedInt, 0);
			GL.BindVertexArray(0);
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

		public override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyDown(e);
		}


		private Camera mCamera = null;
		//private GLProgram mShader = null;

		//private Vector3[] mLightPositions = {
		//	new Vector3(-10.0f,  10.0f, 10.0f),
		//	new Vector3( 10.0f,  10.0f, 10.0f),
		//	new Vector3(-10.0f, -10.0f, 10.0f),
		//	new Vector3( 10.0f, -10.0f, 10.0f),
		//};

		//private Vector3[] mLightColors = {
		//	new Vector3(300.0f, 300.0f, 300.0f),
		//	new Vector3(0.0f, 300.0f, 300.0f),
		//	new Vector3(300.0f, 0.0f, 300.0f),
		//	new Vector3(300.0f, 300.0f, 0.0f)
		//};

		//private Matrix4 mProjection = new Matrix4();
		private int mSphereVAO = 0;
		private int mIndexCount = 0;
	}
}

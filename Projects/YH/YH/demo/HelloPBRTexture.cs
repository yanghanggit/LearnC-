
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

			//
			mAlbedo = new GLTexture2D(@"Resources/Texture/rustediron1-alt2-Unreal-Engine/rustediron2_basecolor.png");
			mNormal = new GLTexture2D(@"Resources/Texture/rustediron1-alt2-Unreal-Engine/rustediron2_normal.png");
			mMetallic = new GLTexture2D(@"Resources/Texture/rustediron1-alt2-Unreal-Engine/rustediron2_metallic.png");
			mRoughness = new GLTexture2D(@"Resources/Texture/rustediron1-alt2-Unreal-Engine/rustediron2_roughness.png");
			mAO = new GLTexture2D(@"Resources/Texture/rustediron1-alt2-Unreal-Engine/rustediron2_basecolor.png");

			mShader = new GLProgram(@"Resources/pbr.vs", @"Resources/pbr.frag");

			// set material texture uniforms
			mShader.Use();
			GL.Uniform1(mShader.GetUniformLocation("albedoMap"), 0);
			GL.Uniform1(mShader.GetUniformLocation("normalMap"), 1);
			GL.Uniform1(mShader.GetUniformLocation("metallicMap"), 2);
			GL.Uniform1(mShader.GetUniformLocation("roughnessMap"), 3);
			GL.Uniform1(mShader.GetUniformLocation("aoMap"), 4);


			// projection setup
			mProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
															(float)wnd.Width / (float)wnd.Height,
															0.1f, 100.0f);

			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref mProjection);
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			const int nrRows = 7;
			const int nrColumns = 7;
            const float spacing = 2.5f;

			// configure view matrix
			mShader.Use();
			var view = mCamera.GetViewMatrix();
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);

			// setup relevant shader uniforms
			GL.Uniform3(mShader.GetUniformLocation("camPos"), mCamera.Position);

            // set material
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, mAlbedo.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, mNormal.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(TextureTarget.Texture2D, mMetallic.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture3);
			GL.BindTexture(TextureTarget.Texture2D, mRoughness.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture4);
			GL.BindTexture(TextureTarget.Texture2D, mAO.getTextureId());



			// render rows*column number of spheres with material properties defined by textures (they all have the same material properties)
            var model = Matrix4.CreateTranslation(0, 0, 0);
			for (int row = 0; row < nrRows; ++row)
			{
				for (int col = 0; col < nrColumns; ++col)
				{
					model = Matrix4.CreateTranslation(
					    (float)(col - (nrColumns / 2)) * spacing,
					    (float)(row - (nrRows / 2)) * spacing,
					    0.0f);
                    
					GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
					RenderSphere();
				}
			}

            Vector3 newPos = new Vector3();
			for (int i = 0; i < mLightPositions.Length; ++i)
			{
                newPos.X = mLightPositions[i].X + (float)Math.Sin((float)mTotalRuningTime) * 2.0f * 2.0f;
                newPos.Y = (float)Math.Sin((float)mTotalRuningTime / 2.0f) * 1.0f * 2.0f + mLightPositions[i].Y;
                newPos.Z = (float)Math.Sin((float)mTotalRuningTime) * 5.0f;

				GL.Uniform3(mShader.GetUniformLocation("lightPositions[" + i + "]"), newPos);
				GL.Uniform3(mShader.GetUniformLocation("lightColors[" + i + "]"), mLightColors[i]);

				model = Matrix4.CreateTranslation(newPos);
				model = Matrix4.CreateScale(0.5f) * model;
				GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);

				RenderSphere();
			}
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

		private Vector3[] mLightPositions = {
			new Vector3(0.0f, 0.0f, 10.0f)
		};

		private Vector3[] mLightColors = {
			new Vector3(150.0f, 150.0f, 150.0f)
		};

		private Matrix4 mProjection = new Matrix4();
		private int mSphereVAO = 0;
		private int mIndexCount = 0;


        //
        private GLTexture2D mAlbedo = null;
		private GLTexture2D mNormal = null;
		private GLTexture2D mMetallic = null;
		private GLTexture2D mRoughness = null;
		private GLTexture2D mAO = null;
        private GLProgram mShader = null;
	}
}

﻿﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Collections.Generic;

namespace YH
{
	public class HelloNormalMapping : Application
	{
		public HelloNormalMapping() : base("HelloNormalMapping")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mFloor = new Floor();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);
			mShader = new GLProgram(@"Resources/advanced.vs", @"Resources/advanced.frag");
			mCubeTexture = new GLTexture2D(@"Resources/Texture/wall.jpg");
			mFloorTexture = new GLTexture2D(@"Resources/Texture/metal.png");

			//
			//mDepthFunction.Add(DepthFunction.Less);
			//mDepthFunction.Add(DepthFunction.Always);
			//mDepthFunction.Add(DepthFunction.Never);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.ClearColor(Color.Gray);

            //
            shader = new GLProgram(@"Resources/normal_mapping.vs", @"Resources/normal_mapping.frag");
			shader.Use();
			GL.Uniform1(shader.GetUniformLocation("diffuseMap"), 0);
			GL.Uniform1(shader.GetUniformLocation("normalMap"), 1);

            //
			diffuseMap = new GLTexture2D(@"Resources/Texture/11869.jpg");
			normalMap = new GLTexture2D(@"Resources/Texture/11870.jpg");
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			//if (mUseDepthTest)
			//{
			//	GL.Enable(EnableCap.DepthTest);
			//}
			//else
			//{
			//	GL.Disable(EnableCap.DepthTest);
			//}

			//GL.DepthFunc(mDepthFunction[mDepthFuncIndex]);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom), (float)wnd.Width / (float)wnd.Height, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			mShader.Use();

			GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());

			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);

			Matrix4 model = Matrix4.CreateTranslation(-1.0f, 0.0f, -1.0f);
			//model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(2.0f, 0.0f, 0.0f);
			//model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			GL.BindTexture(TextureTarget.Texture2D, mFloorTexture.getTextureId());
			model = Matrix4.CreateTranslation(0.0f, -0.5f, 0.0f);
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mFloor.Draw();
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (e.Key == OpenTK.Input.Key.Plus)
			{
				//++mDepthFuncIndex;
				//mDepthFuncIndex = mDepthFuncIndex >= mDepthFunction.Count ? 0 : mDepthFuncIndex;
			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{
				//--mDepthFuncIndex;
				//mDepthFuncIndex = mDepthFuncIndex < 0 ? 0 : mDepthFuncIndex;
			}
			else if (e.Key == OpenTK.Input.Key.C)
			{
				//mUseDepthTest = !mUseDepthTest;
			}
			else if (e.Key == OpenTK.Input.Key.Space)
			{
				//mUseDepthTest = true;
				//mDepthFuncIndex = 0;
			}
		}

		private void RenderQuad()
		{
			if (quadVAO == 0)
			{
				// positions
                Vector3 pos1 = new Vector3(-1.0f, 1.0f, 0.0f);
				Vector3 pos2 = new Vector3(-1.0f, -1.0f, 0.0f);
				Vector3 pos3 = new Vector3(1.0f, -1.0f, 0.0f);
				Vector3 pos4 = new Vector3(1.0f, 1.0f, 0.0f);
				// texture coordinates
                Vector2 uv1 = new Vector2(0.0f, 1.0f);
				Vector2 uv2 = new Vector2(0.0f, 0.0f);
				Vector2 uv3 = new Vector2(1.0f, 0.0f);
				Vector2 uv4 = new Vector2(1.0f, 1.0f);
				// normal vector
                Vector3 nm = new Vector3(0.0f, 0.0f, 1.0f);

				// calculate tangent/bitangent vectors of both triangles
				Vector3 tangent1, bitangent1;
				Vector3 tangent2, bitangent2;
				// - triangle 1
				Vector3 edge1 = pos2 - pos1;
				Vector3 edge2 = pos3 - pos1;
				Vector2 deltaUV1 = uv2 - uv1;
				Vector2 deltaUV2 = uv3 - uv1;

                float f = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);

				tangent1.X = f * (deltaUV2.Y * edge1.X - deltaUV1.Y * edge2.X);
				tangent1.Y = f * (deltaUV2.Y * edge1.Y - deltaUV1.Y * edge2.Y);
				tangent1.Z = f * (deltaUV2.Y * edge1.Z - deltaUV1.Y * edge2.Z);
                tangent1 = Vector3.Normalize(tangent1);//glm::normalize(tangent1);

				bitangent1.X = f * (-deltaUV2.X * edge1.X + deltaUV1.X * edge2.X);
				bitangent1.Y = f * (-deltaUV2.X * edge1.Y + deltaUV1.X * edge2.Y);
				bitangent1.Z = f * (-deltaUV2.X * edge1.Z + deltaUV1.X * edge2.Z);
				bitangent1 = Vector3.Normalize(bitangent1);//glm::normalize(bitangent1);

				// - triangle 2
				edge1 = pos3 - pos1;
				edge2 = pos4 - pos1;
				deltaUV1 = uv3 - uv1;
				deltaUV2 = uv4 - uv1;

				f = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);

				tangent2.X = f * (deltaUV2.Y * edge1.X - deltaUV1.Y * edge2.X);
				tangent2.Y = f * (deltaUV2.Y * edge1.Y - deltaUV1.Y * edge2.Y);
				tangent2.Z = f * (deltaUV2.Y * edge1.Z - deltaUV1.Y * edge2.Z);
				tangent2 = Vector3.Normalize(tangent2);//glm::normalize(tangent2);


				bitangent2.X = f * (-deltaUV2.X * edge1.X + deltaUV1.X * edge2.X);
				bitangent2.Y = f * (-deltaUV2.X * edge1.Y + deltaUV1.X * edge2.Y);
				bitangent2.Z = f * (-deltaUV2.X * edge1.Z + deltaUV1.X * edge2.Z);
				bitangent2 = Vector3.Normalize(bitangent2);//glm::normalize(bitangent2);


                float[] quadVertices = {
		            // Positions            // normal         // TexCoords  // Tangent                          // Bitangent
		            pos1.X, pos1.Y, pos1.Z, nm.X, nm.Y, nm.Z, uv1.X, uv1.Y, tangent1.X, tangent1.Y, tangent1.Z, bitangent1.X, bitangent1.Y, bitangent1.Z,
					pos2.X, pos2.Y, pos2.Z, nm.X, nm.Y, nm.Z, uv2.X, uv2.Y, tangent1.X, tangent1.Y, tangent1.Z, bitangent1.X, bitangent1.Y, bitangent1.Z,
					pos3.X, pos3.Y, pos3.Z, nm.X, nm.Y, nm.Z, uv3.X, uv3.Y, tangent1.X, tangent1.Y, tangent1.Z, bitangent1.X, bitangent1.Y, bitangent1.Z,

					pos1.X, pos1.Y, pos1.Z, nm.X, nm.Y, nm.Z, uv1.X, uv1.Y, tangent2.X, tangent2.Y, tangent2.Z, bitangent2.X, bitangent2.Y, bitangent2.Z,
					pos3.X, pos3.Y, pos3.Z, nm.X, nm.Y, nm.Z, uv3.X, uv3.Y, tangent2.X, tangent2.Y, tangent2.Z, bitangent2.X, bitangent2.Y, bitangent2.Z,
					pos4.X, pos4.Y, pos4.Z, nm.X, nm.Y, nm.Z, uv4.X, uv4.Y, tangent2.X, tangent2.Y, tangent2.Z, bitangent2.X, bitangent2.Y, bitangent2.Z
				};

				quadVAO = GL.GenVertexArray();
				quadVBO = GL.GenBuffer();

				//
				GL.BindVertexArray(quadVAO);
				GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
				GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * quadVertices.Length, quadVertices, BufferUsageHint.StaticDraw);

				GL.EnableVertexAttribArray(0);
				GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 0);

				GL.EnableVertexAttribArray(1);
				GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), (3 * sizeof(float)));

				GL.EnableVertexAttribArray(2);
				GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 14 * sizeof(float), (6 * sizeof(float)));

				GL.EnableVertexAttribArray(3);
				GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), (8 * sizeof(float)));

				GL.EnableVertexAttribArray(4);
				GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), (11 * sizeof(float)));

				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GL.BindVertexArray(0); // Unbind VAO
			}
			GL.BindVertexArray(quadVAO);			
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
		}

		private Cube mCube = null;
		private Floor mFloor = null;
		private Camera mCamera = null;
		private GLProgram mShader = null;
		private GLTexture2D mCubeTexture = null;
		private GLTexture2D mFloorTexture = null;
		//private bool mUseDepthTest = true;
		//private int mDepthFuncIndex = 0;
		//private List<DepthFunction> mDepthFunction = new List<DepthFunction>();

		// RenderQuad() Renders a 1x1 quad in NDC
		private int quadVAO = 0;
		private int quadVBO = 0;
        private GLProgram shader = null;///("normal_mapping.vs", "normal_mapping.frag");

		private GLTexture2D diffuseMap = null;
		private GLTexture2D normalMap = null;
		


	}
}

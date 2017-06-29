﻿﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

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

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.ClearColor(Color.Gray);

            //
            mShader = new GLProgram(@"Resources/normal_mapping.vs", @"Resources/normal_mapping.frag");
			mShader.Use();
			GL.Uniform1(mShader.GetUniformLocation("diffuseMap"), 0);
			GL.Uniform1(mShader.GetUniformLocation("normalMap"), 1);

            //11867， 11868
            //11869， 11870
            if (false)
            {
				mDiffuseMap = new GLTexture2D(@"Resources/Texture/11867.jpg");
				mNormalMap = new GLTexture2D(@"Resources/Texture/11868.jpg");
            }
            else 
            {
				mDiffuseMap = new GLTexture2D(@"Resources/Texture/11869.jpg");
				mNormalMap = new GLTexture2D(@"Resources/Texture/11870.jpg");
            }
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            mShader.Use();

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom), (float)wnd.Width / (float)wnd.Height, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);

            Matrix4 model = Matrix4.CreateFromAxisAngle(Vector3.Normalize(new Vector3(1.0f, 0.0f, 1.0f)), (float)mTotalRuningTime * -0.1f);//glm::rotate(model, (GLfloat)glfwGetTime() * -0.1f, glm::normalize(glm::vec3(1.0, 0.0, 1.0))); // Rotates the quad to show normal mapping works in all directions
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);

			GL.Uniform3(mShader.GetUniformLocation("lightPos"), mLightPos);
            GL.Uniform3(mShader.GetUniformLocation("viewPos"), mCamera.Position);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, mDiffuseMap.getTextureId());

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, mNormalMap.getTextureId());

			RenderQuad();


            model = Matrix4.CreateTranslation(mLightPos);
            model = Matrix4.CreateScale(0.1f) * model;									  
            GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			RenderQuad();
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

		private void RenderQuad()
		{
			if (mQuadVAO == 0)
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

				mQuadVAO = GL.GenVertexArray();
				mQuadVBO = GL.GenBuffer();

				//
				GL.BindVertexArray(mQuadVAO);
				GL.BindBuffer(BufferTarget.ArrayBuffer, mQuadVBO);
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
			GL.BindVertexArray(mQuadVAO);			
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
		}

		private Camera mCamera = null;
		private int mQuadVAO = 0;
		private int mQuadVBO = 0;
        private GLProgram mShader = null;
		private GLTexture2D mDiffuseMap = null;
		private GLTexture2D mNormalMap = null;
        private Vector3 mLightPos = new Vector3(0.5f, 1.0f, 0.3f);
	}
}

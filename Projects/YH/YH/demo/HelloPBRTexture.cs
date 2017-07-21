
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
			//GLuint albedo = loadTexture(FileSystem::getPath("resources/textures/pbr/rusted_iron/albedo.png").c_str());
			//GLuint normal = loadTexture(FileSystem::getPath("resources/textures/pbr/rusted_iron/normal.png").c_str());
			//GLuint metallic = loadTexture(FileSystem::getPath("resources/textures/pbr/rusted_iron/metallic.png").c_str());
			//GLuint roughness = loadTexture(FileSystem::getPath("resources/textures/pbr/rusted_iron/roughness.png").c_str());
			//GLuint ao = loadTexture(FileSystem::getPath("resources/textures/pbr/rusted_iron/ao.png").c_str());
			albedo = new GLTexture2D(@"Resources/Texture/rustediron1-alt2-Unreal-Engine/rustediron2_basecolor.png");
			normal = new GLTexture2D(@"Resources/Texture/rustediron1-alt2-Unreal-Engine/rustediron2_normal.png");
			metallic = new GLTexture2D(@"Resources/Texture/rustediron1-alt2-Unreal-Engine/rustediron2_metallic.png");
			roughness = new GLTexture2D(@"Resources/Texture/rustediron1-alt2-Unreal-Engine/rustediron2_roughness.png");
			ao = new GLTexture2D(@"Resources/Texture/rustediron1-alt2-Unreal-Engine/rustediron2_basecolor.png");

			//Shader shader("pbr.vs", "pbr.frag");
			shader = new GLProgram(@"Resources/pbr.vs", @"Resources/pbr.frag");

			// set material texture uniforms
			//shader.Use();
			//glUniform1i(glGetUniformLocation(shader.Program, "albedoMap"), 0);
			//glUniform1i(glGetUniformLocation(shader.Program, "normalMap"), 1);
			//glUniform1i(glGetUniformLocation(shader.Program, "metallicMap"), 2);
			//glUniform1i(glGetUniformLocation(shader.Program, "roughnessMap"), 3);
			//glUniform1i(glGetUniformLocation(shader.Program, "aoMap"), 4);
			shader.Use();
			GL.Uniform1(shader.GetUniformLocation("albedoMap"), 0);
			GL.Uniform1(shader.GetUniformLocation("normalMap"), 1);
			GL.Uniform1(shader.GetUniformLocation("metallicMap"), 2);
			GL.Uniform1(shader.GetUniformLocation("roughnessMap"), 3);
			GL.Uniform1(shader.GetUniformLocation("aoMap"), 4);


			// projection setup
			//glm::mat4 projection = glm::perspective(camera.Zoom, (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
			//glUniformMatrix4fv(glGetUniformLocation(shader.Program, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
			projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
															(float)wnd.Width / (float)wnd.Height,
															0.1f, 100.0f);

			GL.UniformMatrix4(shader.GetUniformLocation("projection"), false, ref projection);
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
			//shader.Use();
			//glm::mat4 view = camera.GetViewMatrix();
			//glUniformMatrix4fv(glGetUniformLocation(shader.Program, "view"), 1, GL_FALSE, glm::value_ptr(view));
			shader.Use();
			var view = mCamera.GetViewMatrix();
			GL.UniformMatrix4(shader.GetUniformLocation("view"), false, ref view);

			// setup relevant shader uniforms
			//glUniform3fv(glGetUniformLocation(shader.Program, "camPos"), 1, &camera.Position[0]);
			GL.Uniform3(shader.GetUniformLocation("camPos"), mCamera.Position);


            // set material
            //glActiveTexture(GL_TEXTURE0);
            //glBindTexture(GL_TEXTURE_2D, albedo);
            //glActiveTexture(GL_TEXTURE1);
            //glBindTexture(GL_TEXTURE_2D, normal);
            //glActiveTexture(GL_TEXTURE2);
            //glBindTexture(GL_TEXTURE_2D, metallic);
            //glActiveTexture(GL_TEXTURE3);
            //glBindTexture(GL_TEXTURE_2D, roughness);
            //glActiveTexture(GL_TEXTURE4);
            //glBindTexture(GL_TEXTURE_2D, ao);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, albedo.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, normal.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(TextureTarget.Texture2D, metallic.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture3);
			GL.BindTexture(TextureTarget.Texture2D, roughness.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture4);
			GL.BindTexture(TextureTarget.Texture2D, ao.getTextureId());



			// render rows*column number of spheres with material properties defined by textures (they all have the same material properties)
			//glm::mat4 model;
            var model = Matrix4.CreateTranslation(0, 0, 0);
			for (int row = 0; row < nrRows; ++row)
			{
				for (int col = 0; col < nrColumns; ++col)
				{
					//model = glm::mat4();
					//model = glm::translate(model, glm::vec3(
					//	(float)(col - (nrColumns / 2)) * spacing,
					//	(float)(row - (nrRows / 2)) * spacing,
					//	0.0f
					//));
					model = Matrix4.CreateTranslation(
					    (float)(col - (nrColumns / 2)) * spacing,
					    (float)(row - (nrRows / 2)) * spacing,
					    0.0f);
                    
					//glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
					GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
					RenderSphere();
				}
			}


			// render light source (simply re-render sphere at light positions)
			// this looks a bit off as we use the same shader, but it'll make their positions obvious and 
			// keeps the codeprint small.
			//for (unsigned int i = 0; i < sizeof(lightPositions) / sizeof(lightPositions[0]); ++i)
			//{
			//	glm::vec3 newPos = lightPositions[i] + glm::vec3(sin(glfwGetTime() * 5.0) * 5.0, 0.0, 0.0);
			//	newPos = lightPositions[i];
			//	glUniform3fv(glGetUniformLocation(shader.Program, ("lightPositions[" + std::to_string(i) + "]").c_str()), 1, &newPos[0]);
			//	glUniform3fv(glGetUniformLocation(shader.Program, ("lightColors[" + std::to_string(i) + "]").c_str()), 1, &lightColors[i][0]);

			//	model = glm::mat4();
			//	model = glm::translate(model, newPos);
			//	model = glm::scale(model, glm::vec3(0.5f));
			//	glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
			//	renderSphere();
			//}
			for (int i = 0; i < lightPositions.Length; ++i)
			{
                Vector3 newPos = new Vector3();//Vector3.Add(lightPositions[i], new Vector3((float)Math.Sin(mTotalRuningTime * 5.0f), 0.0f, 0.0f));

                newPos.X = lightPositions[i].X + (float)Math.Sin((float)mTotalRuningTime) * 2.0f * 2.0f;
                newPos.Y = (float)Math.Sin((float)mTotalRuningTime / 2.0f) * 1.0f * 2.0f + lightPositions[i].Y;
                newPos.Z = lightPositions[i].Z + (float)Math.Sin((float)mTotalRuningTime) * 2.0f;


				GL.Uniform3(shader.GetUniformLocation("lightPositions[" + i + "]"), newPos);
				GL.Uniform3(shader.GetUniformLocation("lightColors[" + i + "]"), lightColors[i]);

				model = Matrix4.CreateTranslation(newPos);
				model = Matrix4.CreateScale(0.5f) * model;
				GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);

				RenderSphere();
			}
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


		//glm::vec3 lightPositions[] = {
		//    glm::vec3(0.0, 0.0f, 10.0f),
		//};
		//glm::vec3 lightColors[] = {
		//    glm::vec3(150.0f, 150.0f, 150.0f)
		//};
		private Vector3[] lightPositions = {
			new Vector3(0.0f, 0.0f, 10.0f)
		};

		private Vector3[] lightColors = {
			new Vector3(150.0f, 150.0f, 150.0f)
		};

		private Matrix4 projection = new Matrix4();
		private int mSphereVAO = 0;
		private int mIndexCount = 0;


        //
        private GLTexture2D albedo = null;
		private GLTexture2D normal = null;
		private GLTexture2D metallic = null;
		private GLTexture2D roughness = null;
		private GLTexture2D ao = null;
        private GLProgram shader = null;
	}
}

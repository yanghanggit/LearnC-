
﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using System.Collections.Generic;

namespace YH
{
	public class HelloPBRLighting : Application
	{
		public HelloPBRLighting() : base("HelloPBRLighting")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.ClearColor(Color.Black);
			GL.Enable(EnableCap.DepthTest);

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), -90.0f, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			// build and compile shaders
			// -------------------------
			//Shader shader("1.1.pbr.vs", "1.1.pbr.fs");
			//shader.use();
			//shader.setVec3("albedo", 0.5f, 0.0f, 0.0f);
			//shader.setFloat("ao", 1.0f);
            shader = new GLProgram(@"Resources/1.1.pbr.vs", @"Resources/1.1.pbr.fs");
			shader.Use();
            GL.Uniform3(shader.GetUniformLocation("albedo"), 0.5f, 0.0f, 0.0f);
            GL.Uniform1(shader.GetUniformLocation("ao"), 1.0f);

			// initialize static shader uniforms before rendering
			// --------------------------------------------------
			//glm::mat4 projection = glm::perspective(camera.Zoom, (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
			//shader.use();
			//shader.setMat4("projection", projection);
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

			//shader.use();
			//glm::mat4 view = camera.GetViewMatrix();
			//shader.setMat4("view", view);
			//shader.setVec3("camPos", camera.Position);
			shader.Use();
            var view = mCamera.GetViewMatrix();
			GL.UniformMatrix4(shader.GetUniformLocation("view"), false, ref view);
            GL.Uniform3(shader.GetUniformLocation("camPos"), mCamera.Position);


			// render rows*column number of spheres with varying metallic/roughness values scaled by rows and columns respectively
			//glm::mat4 model;
			var model = Matrix4.CreateTranslation(0, 0, 0);

            for (int row = 0; row < nrRows; ++row)
            {
                //shader.setFloat("metallic", (float)row / (float)nrRows);
                GL.Uniform1(shader.GetUniformLocation("metallic"), (float)row / (float)nrRows);

                for (int col = 0; col < nrColumns; ++col)
                {
                    // we clamp the roughness to 0.025 - 1.0 as perfectly smooth surfaces (roughness of 0.0) tend to look a bit off
                    // on direct lighting.
                    //shader.setFloat("roughness", glm::clamp((float)col / (float)nrColumns, 0.05f, 1.0f));
                    float v = (float)col / (float)nrColumns;
					v = (v < 0.05f) ? 0.05f : ((v > 1.0f) ? 1.0f : v);
					GL.Uniform1(shader.GetUniformLocation("roughness"), v);

					//model = Matrix4.CreateTranslation(0, 0, 0);
					//model = glm::translate(model, glm::vec3(
					//	(float)(col - (nrColumns / 2)) * spacing,
					//	(float)(row - (nrRows / 2)) * spacing,
					//	0.0f
					//));
					model = Matrix4.CreateTranslation(
                        (float)(col - (nrColumns / 2)) * spacing,
						(float)(row - (nrRows / 2)) * spacing,
						0.0f);
                    
					//shader.setMat4("model", model);
					GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);

					renderSphere();
				}
			}


			// render light source (simply re-render sphere at light positions)
			// this looks a bit off as we use the same shader, but it'll make their positions obvious and 
			// keeps the codeprint small.
			//for (unsigned int i = 0; i < sizeof(lightPositions) / sizeof(lightPositions[0]); ++i)
            for (int i = 0; i < lightPositions.Length; ++i)
			{
                //glm::vec3 newPos = lightPositions[i] + glm::vec3(sin(glfwGetTime() * 5.0) * 5.0, 0.0, 0.0);
                Vector3 newPos = lightPositions[i] + new Vector3((float)Math.Sin(mTotalRuningTime * 5.0f), 0.0f, 0.0f);

				newPos = lightPositions[i];
				//shader.setVec3("lightPositions[" + std::to_string(i) + "]", newPos);
				GL.Uniform3(shader.GetUniformLocation("lightPositions[" + i + "]"), newPos);
				//shader.setVec3("lightColors[" + std::to_string(i) + "]", lightColors[i]);
				GL.Uniform3(shader.GetUniformLocation("lightColors[" + i + "]"), lightColors[i]);

                //model = glm::mat4();
                //model = glm::translate(model, newPos);
                //model = glm::scale(model, glm::vec3(0.5f));
                //shader.setMat4("model", model);
                model = Matrix4.CreateTranslation(newPos);
                model = Matrix4.CreateScale(0.5f) * model;
				GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);

                //
				renderSphere();
			}
		}

        private int sphereVAO = 0;
		private int indexCount = 0;

        private void renderSphere()
        {
			if (sphereVAO == 0)
			{
				//glGenVertexArrays(1, &sphereVAO);
                sphereVAO = GL.GenVertexArray();

                //unsigned int vbo, ebo;
                //glGenBuffers(1, &vbo);
                //glGenBuffers(1, &ebo);
                int vbo = GL.GenBuffer();
                int ebo = GL.GenBuffer();

                //std::vector<glm::vec3> positions;
                //std::vector<glm::vec2> uv;
                //std::vector<glm::vec3> normals;
                //std::vector < unsigned int> indices;
                List<Vector3> positions = new List<Vector3>();
				List<Vector2> uv = new List<Vector2>();
				List<Vector3> normals = new List<Vector3>();
				List<int> indices = new List<int>();


				//const unsigned int X_SEGMENTS = 64;
				//const unsigned int Y_SEGMENTS = 64;
				//const float PI = 3.14159265359;
				const int X_SEGMENTS = 64;
				const int Y_SEGMENTS = 64;
                const float PI = 3.14159265359f;

				for (int y = 0; y <= Y_SEGMENTS; ++y)
				{
					for (int x = 0; x <= X_SEGMENTS; ++x)
					{
						float xSegment = (float)x / (float)X_SEGMENTS;
						float ySegment = (float)y / (float)Y_SEGMENTS;

                        //float xPos = std::cos(xSegment * 2.0f * PI) * std::sin(ySegment * PI);
                        float xPos = (float)Math.Cos(xSegment * 2.0f * PI) * (float)Math.Sin(ySegment * PI);

                        //float yPos = std::cos(ySegment * PI);
                        float yPos = (float)Math.Cos(ySegment * PI);

						//float zPos = std::sin(xSegment * 2.0f * PI) * std::sin(ySegment * PI);
                        float zPos = (float)Math.Sin(xSegment * 2.0f * PI) * (float)Math.Sin(ySegment * PI);


						//positions.push_back(glm::vec3(xPos, yPos, zPos));
                        positions.Add(new Vector3(xPos, yPos, zPos));
						//uv.push_back(glm::vec2(xSegment, ySegment));
                        uv.Add(new Vector2(xSegment, ySegment));
						//normals.push_back(glm::vec3(xPos, yPos, zPos));
                        normals.Add(new Vector3(xPos, yPos, zPos));
					}
				}

				bool oddRow = false;
				for (int y = 0; y < Y_SEGMENTS; ++y)
				{
					if (!oddRow) // even rows: y == 0, y == 2; and so on
					{
						for (int x = 0; x <= X_SEGMENTS; ++x)
						{
							//indices.push_back(y * (X_SEGMENTS + 1) + x);
                            indices.Add(y * (X_SEGMENTS + 1) + x);
							//indices.push_back((y + 1) * (X_SEGMENTS + 1) + x);
							indices.Add((y + 1) * (X_SEGMENTS + 1) + x);
						}
					}
					else
					{
						for (int x = X_SEGMENTS; x >= 0; --x)
						{
							//indices.push_back((y + 1) * (X_SEGMENTS + 1) + x);
                            indices.Add((y + 1) * (X_SEGMENTS + 1) + x);
							//indices.push_back(y * (X_SEGMENTS + 1) + x);
                            indices.Add(y * (X_SEGMENTS + 1) + x);
						}
					}
					oddRow = !oddRow;
				}

                //indexCount = indices.size();
                indexCount = indices.Count;

                //std::vector<float> data;
                List<float> data = new List<float>();

				//for (int i = 0; i < positions.size(); ++i)    
                for (int i = 0; i < positions.Count; ++i)
				{
					//data.push_back(positions[i].x);
					//data.push_back(positions[i].y);
					//data.push_back(positions[i].z);
                    data.Add(positions[i].X);
					data.Add(positions[i].Y);
					data.Add(positions[i].Z);

					//if (uv.size() > 0)
					//{
					//	data.push_back(uv[i].x);
					//	data.push_back(uv[i].y);
					//}
                    if (uv.Count > 0)
					{
						data.Add(uv[i].X);
						data.Add(uv[i].Y);
					}

					//if (normals.size() > 0)
					//{
					//	data.push_back(normals[i].x);
					//	data.push_back(normals[i].y);
					//	data.push_back(normals[i].z);
					//}
                    if (normals.Count > 0)
					{
						data.Add(normals[i].X);
						data.Add(normals[i].Y);
						data.Add(normals[i].Z);
					}
				}

				//glBindVertexArray(sphereVAO);
                GL.BindVertexArray(sphereVAO);
				//glBindBuffer(GL_ARRAY_BUFFER, vbo);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
				//glBufferData(GL_ARRAY_BUFFER, data.size() * sizeof(float), &data[0], GL_STATIC_DRAW);
                GL.BufferData(BufferTarget.ArrayBuffer, data.Count * sizeof(float), data.ToArray(), BufferUsageHint.StaticDraw);
				//glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
				//glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.size() * sizeof(unsigned int), &indices[0], GL_STATIC_DRAW);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);

				var stride = (3 + 2 + 3) * sizeof(float);
				//glEnableVertexAttribArray(0);
				//glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, stride, (void*)0);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, IntPtr.Zero);
                //glEnableVertexAttribArray(1);
				//glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, stride, (void*)(3 * sizeof(float)));
				GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, (IntPtr)(3 * sizeof(float)));
				//glEnableVertexAttribArray(2);
				//glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, stride, (void*)(5 * sizeof(float)));
				GL.EnableVertexAttribArray(2);
				GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, (IntPtr)(5 * sizeof(float)));
				GL.BindVertexArray(0);
			}

			//glBindVertexArray(sphereVAO);
            GL.BindVertexArray(sphereVAO);
			//glDrawElements(GL_TRIANGLE_STRIP, indexCount, GL_UNSIGNED_INT, 0);
            GL.DrawElements(PrimitiveType.TriangleStrip, indexCount, DrawElementsType.UnsignedInt, 0);
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
		private GLProgram shader = null;

		/*
        // lights
        // ------
        glm::vec3 lightPositions[] = {
            glm::vec3(-10.0f,  10.0f, 10.0f),
            glm::vec3( 10.0f,  10.0f, 10.0f),
            glm::vec3(-10.0f, -10.0f, 10.0f),
            glm::vec3( 10.0f, -10.0f, 10.0f),
        };
        glm::vec3 lightColors[] = {
            glm::vec3(300.0f, 300.0f, 300.0f),
            glm::vec3(300.0f, 300.0f, 300.0f),
            glm::vec3(300.0f, 300.0f, 300.0f),
            glm::vec3(300.0f, 300.0f, 300.0f)
        };
        */
		private Vector3[] lightPositions = {
			new Vector3(-10.0f,  10.0f, 10.0f),
			new Vector3( 10.0f,  10.0f, 10.0f),
			new Vector3(-10.0f, -10.0f, 10.0f),
			new Vector3( 10.0f, -10.0f, 10.0f),
	    };

        private Vector3[] lightColors = {
			new Vector3(300.0f, 300.0f, 300.0f),
			new Vector3(300.0f, 300.0f, 300.0f),
			new Vector3(300.0f, 300.0f, 300.0f),
			new Vector3(300.0f, 300.0f, 300.0f)
	    };

        private Matrix4 projection = new Matrix4();

		private const int nrRows = 7;
		private const int nrColumns = 7;
		private const float spacing = 2.5f;
	}
}

using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloMaterials : Application
	{
		public HelloMaterials() : base("HelloMaterials")
		{
		}

		public override void Start()
		{
			base.Start();

			mCube = new Cube();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			//
			mLightShader = new GLProgram(@"Resources/materials.vs", @"Resources/materials.frag");

			mLocLightModel = mLightShader.GetUniformLocation("model");
			mLocLightView = mLightShader.GetUniformLocation("view");
			mLocLightProjection = mLightShader.GetUniformLocation("projection");

			mLocLightPos = mLightShader.GetUniformLocation("light.position");
			mLocLightViewPos = mLightShader.GetUniformLocation("viewPos");

			mLocLightAmbient = mLightShader.GetUniformLocation("light.ambient");
			mLocLightDiffuse = mLightShader.GetUniformLocation("light.diffuse");
			mLocLightSpecular = mLightShader.GetUniformLocation("light.specular");

			mLocMaterialAmbient = mLightShader.GetUniformLocation("material.ambient");
			mLocMaterialDiffuse = mLightShader.GetUniformLocation("material.diffuse");
			mLocMaterialSpecular = mLightShader.GetUniformLocation("material.specular");
			mLocMaterialShininess = mLightShader.GetUniformLocation("material.shininess");




			//glUniform3f(glGetUniformLocation(lightingShader.Program, "light.ambient"),  ambientColor.x, ambientColor.y, ambientColor.z);

			//glUniform3f(glGetUniformLocation(lightingShader.Program, "light.diffuse"),  diffuseColor.x, diffuseColor.y, diffuseColor.z);

			//glUniform3f(glGetUniformLocation(lightingShader.Program, "light.specular"), 1.0f, 1.0f, 1.0f);

			//glUniform3f(glGetUniformLocation(lightingShader.Program, "material.ambient"),   1.0f, 0.5f, 0.31f);

			//glUniform3f(glGetUniformLocation(lightingShader.Program, "material.diffuse"),   1.0f, 0.5f, 0.31f);

			//glUniform3f(glGetUniformLocation(lightingShader.Program, "material.specular"),  0.5f, 0.5f, 0.5f); // Specular doesn't have full effect on this object's material

			//glUniform1f(glGetUniformLocation(lightingShader.Program, "material.shininess"), 32.0f);

			// Create camera transformations
			//glm::mat4 view;
			//view = camera.GetViewMatrix();
			//glm::mat4 projection = glm::perspective(camera.Zoom, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.1f, 100.0f);
			//// Get the uniform locations
			//GLint modelLoc = glGetUniformLocation(lightingShader.Program, "model");
			//GLint viewLoc = glGetUniformLocation(lightingShader.Program, "view");
			//GLint projLoc = glGetUniformLocation(lightingShader.Program, "projection");
			//// Pass the matrices to the shader
			//glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));

			//glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(projection));

			//// Draw the container (using container's vertex attributes)
			//glBindVertexArray(containerVAO);
			//glm::mat4 model;

			//glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));

			//glDrawArrays(GL_TRIANGLES, 0, 36);

			//glBindVertexArray(0);






			//
			mLampShader = new GLProgram(@"Resources/lamp.vs", @"Resources/lamp.frag");
			mLocLampModel = mLampShader.GetUniformLocation("model");
			mLocLampView = mLampShader.GetUniformLocation("view");
			mLocLampProjection = mLampShader.GetUniformLocation("projection");
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, int w, int h)
		{
			GL.Viewport(0, 0, w, h);
			GL.ClearColor(Color.Gray);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Enable(EnableCap.DepthTest);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)w / (float)h, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);

			do
			{
				mLightShader.Use();

				GL.UniformMatrix4(mLocLightProjection, false, ref projection);
				GL.UniformMatrix4(mLocLightView, false, ref view);
				/*
				GL.Uniform3(mLocLightObjectColor, 1.0f, 0.5f, 0.31f);
				GL.Uniform3(mLocLightColor, 1.0f, 1.0f, 1.0f);
				GL.Uniform3(mLocLightPos, lightPos.X, lightPos.Y, lightPos.Z);
				GL.Uniform3(mLocLightViewPos, mCamera.Position.X, mCamera.Position.Y, mCamera.Position.Z);

				Matrix4 model = Matrix4.CreateTranslation(0, 0, 0);
				model = Matrix4.CreateScale(0.5f) * model;
				GL.UniformMatrix4(mLocLightModel, false, ref model);
				*/

				mCube.Draw();
			}
			while (false);

			do
			{
				mLampShader.Use();

				GL.UniformMatrix4(mLocLampProjection, false, ref projection);
				GL.UniformMatrix4(mLocLampView, false, ref view);

				Matrix4 model = Matrix4.CreateTranslation(lightPos.X, lightPos.Y, lightPos.Z);
				model = Matrix4.CreateScale(0.2f) * model;
				GL.UniformMatrix4(mLocLampModel, false, ref model);

				mCube.Draw();
			}
			while (false);
		}

		private Cube mCube = null;

		private Camera mCamera = null;

		//
		private GLProgram mLightShader = null;

		private int mLocLightModel = -1;
		private int mLocLightView = -1;
		private int mLocLightProjection = -1;

		private int mLocLightPos = -1;
		private int mLocLightViewPos = -1;

		private int mLocLightAmbient = -1;
		private int mLocLightDiffuse = -1;
		private int mLocLightSpecular = -1;

		private int mLocMaterialAmbient = -1;
		private int mLocMaterialDiffuse = -1;
		private int mLocMaterialSpecular = -1;
		private int mLocMaterialShininess = -1;

		//
		private GLProgram mLampShader = null;
		private int mLocLampModel = -1;
		private int mLocLampView = -1;
		private int mLocLampProjection = -1;	
	}
}

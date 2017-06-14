﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Collections.Generic;

namespace YH
{
	public class HelloStencilTesting : Application
	{
		public HelloStencilTesting() : base("HelloStencilTesting")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mPlane = new Plane();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

            shader = new GLProgram(@"Resources/stencil_testing.vs", @"Resources/stencil_testing.frag");
            shaderSingleColor = new GLProgram(@"Resources/stencil_testing.vs", @"Resources/stencil_single_color.frag");

			mCubeTexture = new GLTexture2D(@"Resources/Texture/marble.jpg");
			mFloorTexture = new GLTexture2D(@"Resources/Texture/metal.png");
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.ClearColor(Color.Gray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
           
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Enable(EnableCap.StencilTest);
            GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom), (float)wnd.Width / (float)wnd.Height, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

            shaderSingleColor.Use();
            Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
			GL.UniformMatrix4(shaderSingleColor.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shaderSingleColor.GetUniformLocation("view"), false, ref view);

            shader.Use();
			GL.UniformMatrix4(shader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shader.GetUniformLocation("view"), false, ref view);

            GL.StencilMask(0x00);
            GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            GL.BindTexture(TextureTarget.Texture2D, mFloorTexture.getTextureId());
            mPlane.Draw();

			/*
			// Set uniforms
			//shaderSingleColor.Use();
			//glm::mat4 model;
			//glm::mat4 view = camera.GetViewMatrix();
			//glm::mat4 projection = glm::perspective(camera.Zoom, (float)screenWidth / (float)screenHeight, 0.1f, 100.0f);
			//glUniformMatrix4fv(glGetUniformLocation(shaderSingleColor.Program, "view"), 1, GL_FALSE, glm::value_ptr(view));
			//glUniformMatrix4fv(glGetUniformLocation(shaderSingleColor.Program, "projection"), 1, GL_FALSE, glm::value_ptr(projection));


			//shader.Use();
			//glUniformMatrix4fv(glGetUniformLocation(shader.Program, "view"), 1, GL_FALSE, glm::value_ptr(view));
			//glUniformMatrix4fv(glGetUniformLocation(shader.Program, "projection"), 1, GL_FALSE, glm::value_ptr(projection));


			// Draw floor as normal, we only care about the containers. The floor should NOT fill the stencil buffer so we set its mask to 0x00
			//glStencilMask(0x00);
			// Floor
			//glBindVertexArray(planeVAO);
			//glBindTexture(GL_TEXTURE_2D, floorTexture);
			//model = glm::mat4();
			//glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
			//glDrawArrays(GL_TRIANGLES, 0, 6);
			//glBindVertexArray(0);

			// == =============
			// 1st. Render pass, draw objects as normal, filling the stencil buffer
			glStencilFunc(GL_ALWAYS, 1, 0xFF);
			glStencilMask(0xFF);
			// Cubes
			glBindVertexArray(cubeVAO);
			glBindTexture(GL_TEXTURE_2D, cubeTexture);
			model = glm::translate(model, glm::vec3(-1.0f, 0.0f, -1.0f));
			glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
			glDrawArrays(GL_TRIANGLES, 0, 36);
			model = glm::mat4();
			model = glm::translate(model, glm::vec3(2.0f, 0.0f, 0.0f));
			glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
			glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);

			// == =============
			// 2nd. Render pass, now draw slightly scaled versions of the objects, this time disabling stencil writing.
			// Because stencil buffer is now filled with several 1s. The parts of the buffer that are 1 are now not drawn, thus only drawing 
			// the objects' size differences, making it look like borders.
			glStencilFunc(GL_NOTEQUAL, 1, 0xFF);
			glStencilMask(0x00);
			glDisable(GL_DEPTH_TEST);
			shaderSingleColor.Use();
			GLfloat scale = 1.1;
			// Cubes
			glBindVertexArray(cubeVAO);
			glBindTexture(GL_TEXTURE_2D, cubeTexture);
			model = glm::mat4();
			model = glm::translate(model, glm::vec3(-1.0f, 0.0f, -1.0f));
			model = glm::scale(model, glm::vec3(scale, scale, scale));
			glUniformMatrix4fv(glGetUniformLocation(shaderSingleColor.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
			glDrawArrays(GL_TRIANGLES, 0, 36);
			model = glm::mat4();
			model = glm::translate(model, glm::vec3(2.0f, 0.0f, 0.0f));
			model = glm::scale(model, glm::vec3(scale, scale, scale));
			glUniformMatrix4fv(glGetUniformLocation(shaderSingleColor.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
			glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);
			glStencilMask(0xFF);
			glEnable(GL_DEPTH_TEST);

            */








			//mShader.Use();

			//GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());

			//GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			//GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);

			//Matrix4 model = Matrix4.CreateTranslation(-1.0f, 0.0f, -1.0f);
			//model = Matrix4.CreateScale(0.5f) * model;
			//GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			//mCube.Draw();

			//model = Matrix4.CreateTranslation(2.0f, 0.0f, 0.0f);
			//model = Matrix4.CreateScale(0.5f) * model;
			//GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			//mCube.Draw();

			//GL.BindTexture(TextureTarget.Texture2D, mFloorTexture.getTextureId());
			//model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
			//GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			//mPlane.Draw();
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

		private Cube mCube = null;
		private Plane mPlane = null;
		private Camera mCamera = null;
		//private GLProgram mShader = null;
		private GLTexture2D mCubeTexture = null;
		private GLTexture2D mFloorTexture = null;
		//private bool mUseDepthTest = true;
		//private int mDepthFuncIndex = 0;
		//private List<DepthFunction> mDepthFunction = new List<DepthFunction>();

		// Setup and compile our shaders
        private GLProgram shader = null;//("stencil_testing.vs", "stencil_testing.frag");
		private GLProgram shaderSingleColor = null;//("stencil_testing.vs", "stencil_single_color.frag");



		

	}
}

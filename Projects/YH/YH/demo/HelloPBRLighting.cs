
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

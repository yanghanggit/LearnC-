﻿﻿﻿﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Collections.Generic;

namespace YH
{
	public class HelloAsteroidsInstanced : Application
	{
		public HelloAsteroidsInstanced() : base("HelloAsteroidsInstanced")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
            mCubeInstanced = new Cube();
			mSphere = new Sphere();
			mSkybox = new Skybox();
			mFloor = new Floor();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			shader = new GLProgram(@"Resources/planet.vs", @"Resources/planet.frag");
			skyboxShader = new GLProgram(@"Resources/skybox.vs", @"Resources/skybox.frag");
            instanceShader = new GLProgram(@"Resources/instanced_asteroids.vs", @"Resources/instanced_asteroids.frag");

		    mGLTextureCube = new GLTextureCube(
				@"Resources/Texture/skybox/right.jpg",
				@"Resources/Texture/skybox/left.jpg",
				@"Resources/Texture/skybox/top.jpg",
				@"Resources/Texture/skybox/bottom.jpg",
				@"Resources/Texture/skybox/back.jpg",
				@"Resources/Texture/skybox/front.jpg"
			);

            GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Less);
            GL.ClearColor(Color.Gray);

            InitModelMatrices();
		}

        private void InitModelMatrices()
        {
            var rd = new Random(System.DateTime.Now.Millisecond);
            const float radius = 150.0f;
            const float offset = 25.0f;
            var amount = modelMatrices.Length;
   
            for (int i = 0; i < modelMatrices.Length; i++)
			{
				//glm::mat4 model;
                Matrix4 model = Matrix4.CreateTranslation(0, 0, 0);
				// 1. Translation: Randomly displace along circle with radius 'radius' in range [-offset, offset]
				float angle = (float)i / (float)amount * 360.0f;

                float displacement = ((float)rd.NextDouble() % (int)(2 * offset * 100)) / 100.0f - offset;
                float x = (float)Math.Sin(angle) * radius + displacement;
				
                displacement = ((float)rd.NextDouble() % (int)(2 * offset * 100)) / 100.0f - offset;
				float y = -2.5f + displacement * 0.4f; // Keep height of asteroid field smaller compared to width of x and z
				
                displacement = ((float)rd.NextDouble() % (int)(2 * offset * 100)) / 100.0f - offset;
                float z = (float)Math.Cos(angle) * radius + displacement;

                model = Matrix4.CreateTranslation(x, y, z);//glm::translate(model, glm::vec3(x, y, z));

                // 2. Scale: Scale between 0.05 and 0.25f
                float scale = ((float)rd.NextDouble() % 20) / 100.0f + 0.05f;
                model = Matrix4.CreateScale(scale);//glm::scale(model, glm::vec3(scale));

				// 3. Rotation: add random rotation around a (semi)randomly picked rotation axis vector
                float rotAngle = ((float)rd.NextDouble() % 360);
                model = Matrix4.CreateFromAxisAngle(new Vector3(0.4f, 0.6f, 0.8f), rotAngle);//glm::rotate(model, rotAngle, glm::vec3(0.4f, 0.6f, 0.8f));

				// 4. Now add to list of matrices
				modelMatrices[i] = model;
			}

            mCubeInstanced.build();

            GL.BindVertexArray(mCubeInstanced.mVAO);

			int buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
			GL.BufferData(BufferTarget.ArrayBuffer, amount * sizeof(float) * 4 * 4, modelMatrices, BufferUsageHint.StaticDraw);

			// Set attribute pointers for matrix (4 times vec4)
			GL.EnableVertexAttribArray(3);
			GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4 * 4, (IntPtr)(0 * sizeof(float) * 4));

			GL.EnableVertexAttribArray(4);
			GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4 * 4, (IntPtr)(1 * sizeof(float) * 4));

			GL.EnableVertexAttribArray(5);
			GL.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4 * 4, (IntPtr)(2 * sizeof(float) * 4));

			GL.EnableVertexAttribArray(6);
			GL.VertexAttribPointer(6, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4 * 4, (IntPtr)(3 * sizeof(float) * 4));

			GL.VertexAttribDivisor(3, 1);
			GL.VertexAttribDivisor(4, 1);
			GL.VertexAttribDivisor(5, 1);
			GL.VertexAttribDivisor(6, 1);

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0);
        }

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom), (float)wnd.Width / (float)wnd.Height, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();
			Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

			// Draw scene as normal
			shader.Use();

			GL.BindTexture(TextureTarget.TextureCubeMap, mGLTextureCube.mTextureCubeId);

			GL.UniformMatrix4(shader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shader.GetUniformLocation("view"), false, ref view);
			GL.Uniform3(shader.GetUniformLocation("cameraPos"), mCamera.Position);


			model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			//mSphere.Draw();

			//model = Matrix4.CreateTranslation(0.0f, 2.0f, 0.0f);
			//GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			//mCube.Draw();


            if (true)
            {
				// Draw meteorites
				instanceShader.Use();

				GL.UniformMatrix4(instanceShader.GetUniformLocation("projection"), false, ref projection);
				GL.UniformMatrix4(instanceShader.GetUniformLocation("view"), false, ref view);
                mCubeInstanced.DrawInstance(modelMatrices.Length);

            }

            if (false)
            {
				// Draw skybox as last
				GL.DepthFunc(DepthFunction.Equal);
				skyboxShader.Use();
				var skyView = new Matrix4(new Matrix3(view));
				GL.UniformMatrix4(skyboxShader.GetUniformLocation("view"), false, ref skyView);
				GL.UniformMatrix4(skyboxShader.GetUniformLocation("projection"), false, ref projection);
				GL.BindTexture(TextureTarget.TextureCubeMap, mGLTextureCube.mTextureCubeId);
				mSkybox.Draw();
				GL.DepthFunc(DepthFunction.Less);
            }
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

		private Cube mCube = null;
        private Cube mCubeInstanced = null;
		private Sphere mSphere = null;
		private Floor mFloor = new Floor();
		private Camera mCamera = null;
		private GLProgram shader = null;
		private GLProgram skyboxShader = null;
		private Skybox mSkybox = null;
		private GLTextureCube mGLTextureCube = null;
		//private int mRatioIndex = 0;
        private Matrix4[] modelMatrices = new Matrix4[5];
        private GLProgram instanceShader = null;
	}

}

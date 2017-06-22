﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
 
namespace YH
{
	public class HelloShadowMapping : Application
	{
		public HelloShadowMapping() : base("HelloShadowMapping")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mFloor = new Floor();
            mQuad = new Quad();
            mSphere = new Sphere();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);
			mShader = new GLProgram(@"Resources/advanced.vs", @"Resources/advanced.frag");
            //mCubeTexture = new GLTexture2D(@"Resources/Texture/wall.jpg");
			mFloorTexture = new GLTexture2D(@"Resources/Texture/wood.png");

			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Less);
            GL.ClearColor(Color.Black);

            mDepthFramebuffer = new GLDepthMapFramebuffer(1024, 1024, new Vector4(1, 1, 1, 1));

            mSimpleDepthShader = new GLProgram(@"Resources/shadow_mapping_depth.vs", @"Resources/shadow_mapping_depth.frag");
			mDebugDepthQuad = new GLProgram(@"Resources/debug_quad.vs", @"Resources/debug_quad_depth.frag");
            mLampShader = new GLProgram(@"Resources/lamp.vs", @"Resources/lamp.frag");
            mShadowMapping = new GLProgram(@"Resources/shadow_mapping.vs", @"Resources/shadow_mapping.frag");
		}

		public override void Update(double dt)
		{
			base.Update(dt);

            mLightPos.Z = (float)Math.Cos((float)mTotalRuningTime);// * 2.0f;
		}

		public override void Draw(double dt, Window wnd)
		{
			const float near_plane = 0.1f;
			const float far_plane = 100.0f;

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																	  (float)wnd.Width / (float)wnd.Height,
																	  near_plane, far_plane);

			var view = mCamera.GetViewMatrix();
			
            Matrix4 lightView = mUseCameraViewAsLightView ? 
                view : Matrix4.LookAt(mLightPos, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));

            //
            Matrix4 lightSpaceMatrix = lightView * projection;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mDepthFramebuffer.mDepthMapFramebufferId);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Viewport(0, 0, mDepthFramebuffer.mWidth, mDepthFramebuffer.mHeight);
            mSimpleDepthShader.Use();
			GL.UniformMatrix4(mSimpleDepthShader.GetUniformLocation("lightSpaceMatrix"), false, ref lightSpaceMatrix);
            RenderScene(mSimpleDepthShader, false);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            //
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            mShadowMapping.Use();
			GL.UniformMatrix4(mShadowMapping.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShadowMapping.GetUniformLocation("view"), false, ref view);
            GL.Uniform3(mShadowMapping.GetUniformLocation("lightPos"), mLightPos);
            GL.Uniform3(mShadowMapping.GetUniformLocation("viewPos"), mCamera.Position);
            GL.UniformMatrix4(mShadowMapping.GetUniformLocation("lightSpaceMatrix"), false, ref lightSpaceMatrix);
            GL.Uniform1(mShadowMapping.GetUniformLocation("shadows"), 1);

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, mFloorTexture.getTextureId());

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, mDepthFramebuffer.mDepthMap);

			RenderScene(mShadowMapping, true);

			// 2. Render scene as normal
			//glViewport(0, 0, SCR_WIDTH, SCR_HEIGHT);
			//glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			//shader.Use();
			//glm::mat4 projection = glm::perspective(camera.Zoom, (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
			//glm::mat4 view = camera.GetViewMatrix();
			//glUniformMatrix4fv(glGetUniformLocation(shader.Program, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
			//glUniformMatrix4fv(glGetUniformLocation(shader.Program, "view"), 1, GL_FALSE, glm::value_ptr(view));
			//// Set light uniforms
			//glUniform3fv(glGetUniformLocation(shader.Program, "lightPos"), 1, &lightPos[0]);
			//glUniform3fv(glGetUniformLocation(shader.Program, "viewPos"), 1, &camera.Position[0]);
			//glUniformMatrix4fv(glGetUniformLocation(shader.Program, "lightSpaceMatrix"), 1, GL_FALSE, glm::value_ptr(lightSpaceMatrix));
			//// Enable/Disable shadows by pressing 'SPACE'
			//glUniform1i(glGetUniformLocation(shader.Program, "shadows"), shadows);
			//glActiveTexture(GL_TEXTURE0);
			//glBindTexture(GL_TEXTURE_2D, woodTexture);
			//glActiveTexture(GL_TEXTURE1);
			//glBindTexture(GL_TEXTURE_2D, depthMap);
			//RenderScene(shader);

			//
			mLampShader.Use();
			GL.UniformMatrix4(mLampShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mLampShader.GetUniformLocation("view"), false, ref view);
			Matrix4 model = Matrix4.CreateTranslation(mLightPos);
			model = Matrix4.CreateScale(0.2f) * model;
			GL.UniformMatrix4(mLampShader.GetUniformLocation("model"), false, ref model);
			mSphere.Draw();
			
            if (mShowDepthMap)
            {
                GL.Viewport(0, 0, wnd.Width/2, wnd.Height/2);
                mDebugDepthQuad.Use();
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mDepthFramebuffer.mDepthMap);
                GL.Uniform1(mDebugDepthQuad.GetUniformLocation("near_plane"), near_plane);
                GL.Uniform1(mDebugDepthQuad.GetUniformLocation("far_plane"), far_plane);

				mQuad.Draw();
            }
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.C)
			{
				mShowDepthMap = !mShowDepthMap;
			}
            else if (e.Key == OpenTK.Input.Key.U)
			{
				mUseCameraViewAsLightView = !mUseCameraViewAsLightView;
			}
		}

        void RenderScene(GLProgram shader, bool useTexture)
		{
            Matrix4 model = Matrix4.CreateTranslation(0, 0, 0);
            model = Matrix4.CreateTranslation(0.0f, -0.5f, 0.0f);
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mFloor.Draw();

            //GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());
			model = Matrix4.CreateTranslation(0.0f, 1.5f, 0.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

			model = Matrix4.CreateTranslation(2.0f, 0.0f, 1.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(-1.0f, 0.3f, 2.0f);
            model = Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.0f, 1.0f), 60.0f) * model;
            model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();
		}

		private Cube mCube = null;
		private Floor mFloor = null;
        private Quad mQuad = null;
        private Sphere mSphere = null;

		private Camera mCamera = null;
		private GLProgram mShader = null;
		//private GLTexture2D mCubeTexture = null;
		private GLTexture2D mFloorTexture = null;
        private GLDepthMapFramebuffer mDepthFramebuffer = null;
        private Vector3 mLightPos = new Vector3(-2.0f, 4.0f, -1.0f);

        private bool mShowDepthMap = false;
        private bool mUseCameraViewAsLightView = false;

        private GLProgram mSimpleDepthShader = null;
		private GLProgram mDebugDepthQuad = null;
        private GLProgram mLampShader = null;
		private GLProgram mShadowMapping = null;

		

	}
}

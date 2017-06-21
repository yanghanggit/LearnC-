﻿using System;
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

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);
			mShader = new GLProgram(@"Resources/advanced.vs", @"Resources/advanced.frag");
            mCubeTexture = new GLTexture2D(@"Resources/Texture/wall.jpg");
			mFloorTexture = new GLTexture2D(@"Resources/Texture/wood.png");

			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Less);
            GL.ClearColor(Color.Black);

            depthMapFBO = new GLDepthMapFramebuffer(1024, 1024, new Vector4(1, 1, 1, 1));

            simpleDepthShader = new GLProgram(@"Resources/shadow_mapping_depth.vs", @"Resources/shadow_mapping_depth.frag");
			debugDepthQuad = new GLProgram(@"Resources/debug_quad.vs", @"Resources/debug_quad_depth.frag");
		}

		public override void Update(double dt)
		{
			base.Update(dt);

            //lightPos.Z = (float)Math.Cos((float)mTotalRuningTime) * 2.0f;
		}

		public override void Draw(double dt, Window wnd)
		{
			//=================================================
			const float near_plane = 1.0f;
            const float far_plane = 7.5f;
            Matrix4 lightProjection = Matrix4.CreateOrthographic(wnd.Width, wnd.Height, near_plane, far_plane);
            Matrix4 lightView = Matrix4.LookAt(lightPos, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            Matrix4 lightSpaceMatrix = lightProjection * lightView;
			
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, depthMapFBO.depthMapFBO);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Viewport(0, 0, depthMapFBO.width, depthMapFBO.height);
            simpleDepthShader.Use();
			GL.UniformMatrix4(simpleDepthShader.GetUniformLocation("lightSpaceMatrix"), false, ref lightSpaceMatrix);
            RenderScene(simpleDepthShader);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //=================================================
            if (true)
            {
				GL.Viewport(0, 0, wnd.Width, wnd.Height);

				var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																	  (float)wnd.Width / (float)wnd.Height,
																	  0.1f, 100.0f);

				var view = mCamera.GetViewMatrix();

				mShader.Use();
				GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
				GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);
                RenderScene(mShader);
            }
			
            if (showDepthMap)
            {
                GL.Viewport(0, 0, wnd.Width / 2, wnd.Height / 2);

                debugDepthQuad.Use();
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, depthMapFBO.depthMap);
                GL.Uniform1(debugDepthQuad.GetUniformLocation("near_plane"), near_plane);
                GL.Uniform1(debugDepthQuad.GetUniformLocation("far_plane"), far_plane);

				mQuad.Draw();
            }
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.C)
			{
				showDepthMap = !showDepthMap;
			}
		}

        void RenderScene(GLProgram shader)
		{
            Matrix4 model = Matrix4.CreateTranslation(0, 0, 0);

            GL.BindTexture(TextureTarget.Texture2D, mFloorTexture.getTextureId());

            model = Matrix4.CreateTranslation(0.0f, -0.5f, 0.0f);
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mFloor.Draw();

            GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());

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
		private Camera mCamera = null;
		private GLProgram mShader = null;
		private GLTexture2D mCubeTexture = null;
		private GLTexture2D mFloorTexture = null;
        private GLDepthMapFramebuffer depthMapFBO = null;
        private Vector3 lightPos = new Vector3(-2.0f, 4.0f, -1.0f);

        private bool showDepthMap = false;


        private GLProgram simpleDepthShader = null;
		private GLProgram debugDepthQuad = null;


		

	}
}

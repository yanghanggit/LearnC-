﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
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

            mLightPos.Z = (float)Math.Cos((float)mTotalRuningTime) * 2.0f;
		}

		public override void Draw(double dt, Window wnd)
		{
			const float near_plane = 0.1f;
            const float far_plane = 7.5f;

            var lightProjection = Matrix4.CreateOrthographic(10, 10, near_plane, far_plane);
			//lightProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom), (float)wnd.Width / (float)wnd.Height, near_plane, far_plane);
			var lightView = Matrix4.LookAt(mLightPos, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            var lightSpaceMatrix = lightView  * lightProjection;

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom), (float)wnd.Width / (float)wnd.Height, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

            if (mUseCameraViewAsLightView)
            {
                lightSpaceMatrix = view * projection;
            }

		    //
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

			GL.Uniform1(mShadowMapping.GetUniformLocation("diffuseTexture"), 0);
			GL.Uniform1(mShadowMapping.GetUniformLocation("shadowMap"), 1);

			RenderScene(mShadowMapping, true);

			//
			mLampShader.Use();
			GL.UniformMatrix4(mLampShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mLampShader.GetUniformLocation("view"), false, ref view);
			Matrix4 model = Matrix4.CreateTranslation(mLightPos);
			model = Matrix4.CreateScale(0.2f) * model;
			GL.UniformMatrix4(mLampShader.GetUniformLocation("model"), false, ref model);
			mSphere.Draw();
			
            //
            if (mShowDepthMap)
            {
                GL.Viewport(0, 0, wnd.Width/2, wnd.Height/2);
                GL.Enable(EnableCap.ScissorTest);
                GL.Scissor(0, 0, wnd.Width / 2, wnd.Height / 2);
                mDebugDepthQuad.Use();
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mDepthFramebuffer.mDepthMap);
                GL.Uniform1(mDebugDepthQuad.GetUniformLocation("near_plane"), near_plane);
                GL.Uniform1(mDebugDepthQuad.GetUniformLocation("far_plane"), far_plane);
				mQuad.Draw();
                GL.Disable(EnableCap.ScissorTest);
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

			model = Matrix4.CreateTranslation(0.0f, 1.5f, 0.0f);
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

			model = Matrix4.CreateTranslation(2.0f, 0.0f, 1.0f);
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(-1.0f, 0.3f, 2.0f);
            model = Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.0f, 1.0f), 60.0f) * model;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();
		}

		private Cube mCube = null;
		private Floor mFloor = null;
        private Quad mQuad = null;
        private Sphere mSphere = null;

		private Camera mCamera = null;
		private GLProgram mShader = null;
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

﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Collections.Generic;

namespace YH
{
	public class HelloPointShadows : Application
	{
		public HelloPointShadows() : base("HelloPointShadows")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mSphere = new Sphere();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

            //
            mGLDepthMapFramebuffer = new GLDepthMapFramebuffer(1024, 1024, 
                                                               new Vector4(1, 1, 1, 1),
                                                               GLDepthMapFramebuffer.Type.TEXTURE_CUBE);
             
			//
			float aspect = (float)1024 / (float)1024;
			mShadowProj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), aspect, mNear, mFar);

            //
            mShadowTransforms = CalcShadowTransforms(mLightPos, 1024, 1024, mShadowProj);

            mSimpleDepthShader = new GLProgram(@"Resources/point_shadows_depth.vs", @"Resources/point_shadows_depth.frag", @"Resources/point_shadows_depth.gs");
			mShader = new GLProgram(@"Resources/point_shadows.vs", @"Resources/point_shadows.frag");
            mWoodTexture = new GLTexture2D(@"Resources/Texture/wood.png");

			mLampShader = new GLProgram(@"Resources/lamp.vs", @"Resources/lamp.frag");

			mShader.Use();
            GL.Uniform1(mShader.GetUniformLocation("diffuseTexture"), 0);
            GL.Uniform1(mShader.GetUniformLocation("depthMap"), 1);

			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Less);
            GL.ClearColor(Color.Black);
		}

		public override void Update(double dt)
		{
			base.Update(dt);

            if (mLightMove)
            {
				mLightPos.X = 1.0f + (float)Math.Sin((float)mTotalRuningTime);
				mLightPos.Y = (float)Math.Sin((float)mTotalRuningTime / 2.0f);
				mShadowTransforms = CalcShadowTransforms(mLightPos, 1024, 1024, mShadowProj);
            }
		}

        void DrawDepthMap(double dt, Window wnd)
        {
            GL.Viewport(0, 0, mGLDepthMapFramebuffer.mWidth, mGLDepthMapFramebuffer.mHeight);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mGLDepthMapFramebuffer.mDepthMapFramebufferId);
            GL.Clear(ClearBufferMask.DepthBufferBit);

			mSimpleDepthShader.Use();
            for (int i = 0; i < mShadowTransforms.Count; ++i)
            {
                Matrix4 mat = mShadowTransforms[i];
                string location = "shadowTransforms[" + i + "]";
				GL.UniformMatrix4(mSimpleDepthShader.GetUniformLocation(location),
                                  false, 
                                  ref mat);
			}
			
            //
			GL.Uniform1(mSimpleDepthShader.GetUniformLocation("far_plane"), mFar);
            GL.Uniform3(mSimpleDepthShader.GetUniformLocation("lightPos"), mLightPos);
			RenderScene(mSimpleDepthShader);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

		public override void Draw(double dt, Window wnd)
		{
            DrawDepthMap(dt, wnd);

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			mShader.Use();
			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
                                                                  (float)wnd.Width / (float)wnd.Height,
                                                                  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();
			Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);
			GL.Uniform3(mShader.GetUniformLocation("lightPos"), mLightPos);
            GL.Uniform3(mShader.GetUniformLocation("viewPos"), mCamera.Position);

			//const float mFar = 25.0f;
			GL.Uniform1(mShader.GetUniformLocation("far_plane"), mFar);
			GL.Uniform1(mShader.GetUniformLocation("shadows"), 1);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, mWoodTexture.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.TextureCubeMap, mGLDepthMapFramebuffer.mDepthMap);

			RenderScene(mShader);

			do
			{
				mLampShader.Use();
				GL.UniformMatrix4(mLampShader.GetUniformLocation("projection"), false, ref projection);
				GL.UniformMatrix4(mLampShader.GetUniformLocation("view"), false, ref view);
				model = Matrix4.CreateTranslation(mLightPos.X, mLightPos.Y, mLightPos.Z);
				model = Matrix4.CreateScale(0.1f) * model;
				GL.UniformMatrix4(mLampShader.GetUniformLocation("model"), false, ref model);
                mSphere.Draw();
			}
			while (false);
		}

        List<Matrix4> CalcShadowTransforms(Vector3 lightPos, int shadowWidth, int shadowHeight, Matrix4 shadowProj)
        {
            if (shadowWidth == 0 || shadowHeight == 0)
            {
                return new List<Matrix4>();
            }

			List<Matrix4> shadowTransforms = new List<Matrix4>();
            shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, -1.0f, 0.0f)) * shadowProj);
            return shadowTransforms;
		}

        void RenderScene(GLProgram sh)
        {
            Matrix4 model = Matrix4.CreateTranslation(0, 0, 0);

            model = Matrix4.CreateScale(10.0f);
            GL.UniformMatrix4(sh.GetUniformLocation("model"), false, ref model);
            GL.Disable(EnableCap.CullFace);
			GL.Uniform1(sh.GetUniformLocation("reverse_normals"), 1);
            mCube.Draw();
			GL.Uniform1(sh.GetUniformLocation("reverse_normals"), 0);
            GL.Enable(EnableCap.CullFace);

            //
            model = Matrix4.CreateTranslation(4.0f, -3.5f, 0.0f);
            GL.UniformMatrix4(sh.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

            //
			model = Matrix4.CreateTranslation(2.0f, 3.0f, 1.0f);
            model = Matrix4.CreateScale(1.5f) * model;
			GL.UniformMatrix4(sh.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			//
			model = Matrix4.CreateTranslation(-3.0f, -1.0f, 0.0f);
			GL.UniformMatrix4(sh.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			//
			model = Matrix4.CreateTranslation(-1.5f, 1.0f, 1.5f);
			GL.UniformMatrix4(sh.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

            //
            model = Matrix4.CreateTranslation(-1.5f, 2.0f, -3.0f);
            model = Matrix4.CreateFromAxisAngle(Vector3.Normalize(new Vector3(1.0f, 0.0f, 1.0f)), 60.0f) * model;
            model = Matrix4.CreateScale(1.5f) * model;
			GL.UniformMatrix4(sh.GetUniformLocation("model"), false, ref model);
			mCube.Draw();
        }

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.C)
			{
				mLightMove = !mLightMove;
			}
		}

		private Cube mCube = null;
		private Sphere mSphere = null;
		private Camera mCamera = null;
        private List<Matrix4> mShadowTransforms = new List<Matrix4>();
        private GLDepthMapFramebuffer mGLDepthMapFramebuffer = null;
        private Vector3 mLightPos = new Vector3(0, 0, 0);
        private GLProgram mSimpleDepthShader = null;
		private GLProgram mShader = null;
		private GLTexture2D mWoodTexture = null;
        private GLProgram mLampShader = null;
		private const float mNear = 1.0f;
		private const float mFar = 25.0f;
        private Matrix4 mShadowProj = Matrix4.Zero;
        private bool mLightMove = false;
	}
}

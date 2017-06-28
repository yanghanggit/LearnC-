﻿﻿﻿using System;
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
			mSkybox = new Skybox();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			mShader = new GLProgram(@"Resources/cubemaps.vs", @"Resources/cubemaps.frag");
			mSkyboxShader = new GLProgram(@"Resources/skybox.vs", @"Resources/skybox.frag");

			mGLTextureCube = new GLTextureCube(
				@"Resources/Texture/skybox/right.jpg",
				@"Resources/Texture/skybox/left.jpg",
				@"Resources/Texture/skybox/top.jpg",
				@"Resources/Texture/skybox/bottom.jpg",
				@"Resources/Texture/skybox/back.jpg",
				@"Resources/Texture/skybox/front.jpg"
			);

            //
            mGLDepthMapFramebuffer = new GLDepthMapFramebuffer(1024, 1024, 
                                                               new Vector4(1, 1, 1, 1),
                                                               GLDepthMapFramebuffer.Type.TEXTURE_CUBE);

            mShadowTransforms = BuildShadowTransforms(mLightPos, 1024, 1024);

            simpleDepthShader = new GLProgram(@"Resources/point_shadows_depth.vs", @"Resources/point_shadows_depth.frag", @"Resources/point_shadows_depth.gs");
            //Shader simpleDepthShader("point_shadows_depth.vs", "point_shadows_depth.frag", "point_shadows_depth.gs");
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

        void DrawDepthMap(double dt, Window wnd)
        {
            GL.Viewport(0, 0, mGLDepthMapFramebuffer.mWidth, mGLDepthMapFramebuffer.mHeight);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mGLDepthMapFramebuffer.mDepthMapFramebufferId);
            GL.Clear(ClearBufferMask.DepthBufferBit);

			simpleDepthShader.Use();
            for (int i = 0; i < mShadowTransforms.Count; ++i)
            {
                Matrix4 mat = mShadowTransforms[i];
                string location = "shadowTransforms[" + i + "]";
				GL.UniformMatrix4(simpleDepthShader.GetUniformLocation(location),
                                  false, 
                                  ref mat);
			}
			
            const float far = 25.0f;
			GL.Uniform1(mShader.GetUniformLocation("far_plane"), far);
            GL.Uniform3(mShader.GetUniformLocation("lightPos"), mLightPos);
			RenderScene(simpleDepthShader);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

		public override void Draw(double dt, Window wnd)
		{
            DrawDepthMap(dt, wnd);

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Gray);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Less);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom), (float)wnd.Width / (float)wnd.Height, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();
			Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

			// Draw scene as normal
			mShader.Use();

			GL.BindTexture(TextureTarget.TextureCubeMap, mGLTextureCube.mTextureCubeId);
			GL.Uniform1(mShader.GetUniformLocation("ratio"), ParseRatio(mRatioIndex));

			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);
			GL.Uniform3(mShader.GetUniformLocation("cameraPos"), mCamera.Position);

            RenderScene(mShader);

			// Draw skybox as last
			GL.DepthFunc(DepthFunction.Equal);
			mSkyboxShader.Use();
			var skyView = new Matrix4(new Matrix3(view));
			GL.UniformMatrix4(mSkyboxShader.GetUniformLocation("view"), false, ref skyView);
			GL.UniformMatrix4(mSkyboxShader.GetUniformLocation("projection"), false, ref projection);
			GL.BindTexture(TextureTarget.TextureCubeMap, mGLTextureCube.mTextureCubeId);
			mSkybox.Draw();
			GL.DepthFunc(DepthFunction.Less);
		}

        List<Matrix4> BuildShadowTransforms(Vector3 lightPos, int shadowWidth, int shadowHeight)
        {
            if (shadowWidth == 0 || shadowHeight == 0)
            {
                return new List<Matrix4>();
            }

            float aspect = (float)shadowWidth / (float)shadowHeight;
			const float near = 1.0f;
			const float far = 25.0f;

			var shadowProj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), aspect, near, far);


			List<Matrix4> shadowTransforms = new List<Matrix4>();
            shadowTransforms.Add(shadowProj * Matrix4.LookAt(lightPos, lightPos + new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f)));
			shadowTransforms.Add(shadowProj * Matrix4.LookAt(lightPos, lightPos + new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f)));
			shadowTransforms.Add(shadowProj * Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f)));
			shadowTransforms.Add(shadowProj * Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f)));
			shadowTransforms.Add(shadowProj * Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f)));
			shadowTransforms.Add(shadowProj * Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, -1.0f, 0.0f)));
            return shadowTransforms;
		}

        void RenderScene(GLProgram shader)
        {
            Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();
        }

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (e.Key == OpenTK.Input.Key.Plus)
			{
				++mRatioIndex;
			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{
				--mRatioIndex;
				mRatioIndex = mRatioIndex >= 0 ? mRatioIndex : 0;
			}
			else if (e.Key == OpenTK.Input.Key.C)
			{
			}
		}

		float ParseRatio(int index)
		{
			float[] ratios = {
				0.0f, //nothing
                //1.00f,//空气
                1.33f,//水
                1.309f,//冰
                1.52f,//玻璃
                2.42f,//宝石
            };

			int mod = index % ratios.Length;
			return ratios[mod];
		}

		private Cube mCube = null;
		private Sphere mSphere = null;
		private Camera mCamera = null;
		private GLProgram mShader = null;
		private GLProgram mSkyboxShader = null;
		private Skybox mSkybox = null;
		private GLTextureCube mGLTextureCube = null;
		private int mRatioIndex = 0;
        private List<Matrix4> mShadowTransforms = new List<Matrix4>();
        private GLDepthMapFramebuffer mGLDepthMapFramebuffer = null;
        private Vector3 mLightPos = new Vector3(0, 0, 0);
        private GLProgram simpleDepthShader = null;
	}
}

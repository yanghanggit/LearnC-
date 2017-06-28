﻿﻿﻿﻿using System;
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

            //
            mGLDepthMapFramebuffer = new GLDepthMapFramebuffer(1024, 1024, 
                                                               new Vector4(1, 1, 1, 1),
                                                               GLDepthMapFramebuffer.Type.TEXTURE_CUBE);

            mShadowTransforms = BuildShadowTransforms(mLightPos, 1024, 1024);

            simpleDepthShader = new GLProgram(@"Resources/point_shadows_depth.vs", @"Resources/point_shadows_depth.frag", @"Resources/point_shadows_depth.gs");
			shader = new GLProgram(@"Resources/point_shadows.vs", @"Resources/point_shadows.frag");
            woodTexture = new GLTexture2D(@"Resources/Texture/wood.png");

            shader.Use();
            GL.Uniform1(shader.GetUniformLocation("diffuseTexture"), 0);
            GL.Uniform1(shader.GetUniformLocation("depthMap"), 1);

			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Less);
            GL.ClearColor(Color.Gray);
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
			GL.Uniform1(simpleDepthShader.GetUniformLocation("far_plane"), far);
            GL.Uniform3(simpleDepthShader.GetUniformLocation("lightPos"), mLightPos);
			RenderScene(simpleDepthShader);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

		public override void Draw(double dt, Window wnd)
		{
            DrawDepthMap(dt, wnd);

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			shader.Use();
			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
                                                                  (float)wnd.Width / (float)wnd.Height,
                                                                  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();
			Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

			GL.UniformMatrix4(shader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shader.GetUniformLocation("view"), false, ref view);
			GL.Uniform3(shader.GetUniformLocation("lightPos"), mLightPos);
            GL.Uniform3(shader.GetUniformLocation("viewPos"), mCamera.Position);

			const float far = 25.0f;
			GL.Uniform1(shader.GetUniformLocation("far_plane"), far);
			GL.Uniform1(shader.GetUniformLocation("shadows"), 1);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, woodTexture.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.TextureCubeMap, mGLDepthMapFramebuffer.mDepthMap);

			RenderScene(shader);
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
            Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.5f, 0.0f);
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();
        }

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (e.Key == OpenTK.Input.Key.Plus)
			{

			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{
				
			}
			else if (e.Key == OpenTK.Input.Key.C)
			{
			}
		}

		private Cube mCube = null;
		private Sphere mSphere = null;
		private Camera mCamera = null;
		private Skybox mSkybox = null;
        private List<Matrix4> mShadowTransforms = new List<Matrix4>();
        private GLDepthMapFramebuffer mGLDepthMapFramebuffer = null;
        private Vector3 mLightPos = new Vector3(0, 0, 0);
        private GLProgram simpleDepthShader = null;
		private GLProgram shader = null;
		private GLTexture2D woodTexture = null;
	}
}

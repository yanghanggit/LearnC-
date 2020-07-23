﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using System.Collections.Generic;


namespace YH
{
	public class HelloBloom : Application
	{
		public HelloBloom() : base("HelloBloom")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

            //
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.DepthTest);

            //
			mCube = new Cube();
			mSphere = new Sphere();
            mQuad = new Quad();

            //
			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), -90.0f, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

            //
            mShader = new GLProgram(@"Resources/bloom.vs", @"Resources/bloom.frag");
            mShaderLight = new GLProgram(@"Resources/bloom.vs", @"Resources/light_box.frag");
            mShaderBlur = new GLProgram(@"Resources/blur.vs", @"Resources/blur.frag");
            mShaderBloomFinal = new GLProgram(@"Resources/bloom_final.vs", @"Resources/bloom_final.frag");

			// Set samplers
			mShaderBloomFinal.Use();
            GL.Uniform1(mShaderBloomFinal.GetUniformLocation("scene"), 0);
            GL.Uniform1(mShaderBloomFinal.GetUniformLocation("bloomBlur"), 1);

			//
			mLightPositions.Add(new Vector3(0.0f, 0.8f, 1.5f)); // back light
			mLightPositions.Add(new Vector3(-4.0f, 0.5f, -3.0f));
			mLightPositions.Add(new Vector3(3.0f, 0.5f, 1.0f));
			mLightPositions.Add(new Vector3(-.8f, 2.4f, -1.0f));
		
            //
			mLightColors.Add(new Vector3(5.0f, 5.0f, 5.0f));
			mLightColors.Add(new Vector3(5.5f, 0.0f, 0.0f));
			mLightColors.Add(new Vector3(0.0f, 0.0f, 15.0f));
			mLightColors.Add(new Vector3(0.0f, 1.5f, 0.0f));

			// Load textures
			mWoodTexture = new GLTexture2D(@"Resources/Texture/wood.png"); 
			mContainerTexture = new GLTexture2D(@"Resources/Texture/container2.png"); 

            //
            BuildHDRFramebuffer(wnd.Width, wnd.Height);

            //
            BuildPingPongFramebuffer(wnd.Width, wnd.Height);
		}

        private void BuildHDRFramebuffer(int w, int h)
        {
            // Set up floating point framebuffer to render scene to
            mHDRFBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mHDRFBO);


            GL.GenTextures(2, mColorBuffers);

            for (var i = 0; i < mColorBuffers.Length; i++)
			{
                GL.BindTexture(TextureTarget.Texture2D, mColorBuffers[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, mColorBuffers[i], 0);
			}

            int rboDepth = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, w, h);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rboDepth);

            DrawBuffersEnum[] attachments = {DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1};
            GL.DrawBuffers(2, attachments);

			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("ERROR::FRAMEBUFFER:: BuildHDRFramebuffer is not complete!");
			}
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

        private void BuildPingPongFramebuffer(int w, int h)
        {
            GL.GenFramebuffers(2, mPingpongFBO);
            GL.GenTextures(2, mPingpongColorbuffers);


            for (var i = 0; i < mPingpongFBO.Length; i++)
			{
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, mPingpongFBO[i]);
                GL.BindTexture(TextureTarget.Texture2D, mPingpongColorbuffers[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, mPingpongColorbuffers[i], 0);
				if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
				{
					Console.WriteLine("ERROR::FRAMEBUFFER:: BuildPingPongFramebuffer is not complete!");
				}
			}
        }

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mHDRFBO);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																  (float)wnd.Width / (float)wnd.Height,
																  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			var model = Matrix4.CreateTranslation(0, 0, 0);

			mShader.Use();
			GL.UniformMatrix4(mShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, mWoodTexture.getTextureId());

            for (int i = 0; i < mLightPositions.Count; i++)
			{
           		GL.Uniform3(mShader.GetUniformLocation("lights[" + i + "].Position"), mLightPositions[i]);
                GL.Uniform3(mShader.GetUniformLocation("lights[" + i + "].Color"), mLightColors[i]);
			}
			GL.Uniform3(mShader.GetUniformLocation("viewPos"), mCamera.Position);

            model = Matrix4.CreateTranslation(0.0f, -1.0f, 0.0f);
            model = Matrix4.CreateScale(25.0f, 1.0f, 25.0f) * model;
            GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

            GL.BindTexture(TextureTarget.Texture2D, mContainerTexture.getTextureId());

            model = Matrix4.CreateTranslation(0.0f, 1.5f, 0.0f);
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();
	
            model = Matrix4.CreateTranslation(2.0f, 0.0f, 1.0f);
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

            model = Matrix4.CreateTranslation(-1.0f, -1.0f, 2.0f);
            model = Matrix4.CreateFromAxisAngle(Vector3.Normalize(new Vector3(1.0f, 0.0f, 1.0f)), 60.0f) * model;
            model = Matrix4.CreateScale(2.0f) * model;
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();
			
			model = Matrix4.CreateTranslation(0.0f, 2.7f, 4.0f);
            model = Matrix4.CreateFromAxisAngle(Vector3.Normalize(new Vector3(1.0f, 0.0f, 1.0f)), 23.0f) * model;
            model = Matrix4.CreateScale(2.5f) * model;
            GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(-2.0f, 1.0f, -3.0f);
			model = Matrix4.CreateFromAxisAngle(Vector3.Normalize(new Vector3(1.0f, 0.0f, 1.0f)), 124.0f) * model;
			model = Matrix4.CreateScale(2.0f) * model;
			GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();
      
			model = Matrix4.CreateTranslation(-3.0f, 0.0f, 0.0f);
            GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

            // - finally show all the light sources as bright cubes
			mShaderLight.Use();
			GL.UniformMatrix4(mShaderLight.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShaderLight.GetUniformLocation("view"), false, ref view);

            for (int i = 0; i < mLightPositions.Count; i++)
			{
                model = Matrix4.CreateTranslation(mLightPositions[i]);
				model = Matrix4.CreateScale(0.5f) * model;
                GL.UniformMatrix4(mShaderLight.GetUniformLocation("model"), false, ref model);
                GL.Uniform3(mShaderLight.GetUniformLocation("lightColor"), mLightColors[i]);
                mCube.Draw();
			}
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			//==============================================================================================

			// 2. Blur bright fragments w/ two-pass Gaussian Blur 
			bool horizontal = true, first_iteration = true;
			int amount = 10;
			mShaderBlur.Use();
			for (int i = 0; i < amount; i++)
			{
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, mPingpongFBO[horizontal ? 1 : 0]);
				GL.Uniform1(mShaderBlur.GetUniformLocation("horizontal"), horizontal ? 1 : 0);
                GL.BindTexture(TextureTarget.Texture2D, first_iteration ? mColorBuffers[1] : mPingpongColorbuffers[horizontal ? 0 : 1]);
                mQuad.Draw();
				horizontal = !horizontal;
				if (first_iteration)
                {
                    first_iteration = false;
                }
					
			}
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

			//==============================================================================================

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			mShaderBloomFinal.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, mColorBuffers[0]);
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, mPingpongColorbuffers[horizontal ? 0 : 1]);
			GL.Uniform1(mShaderBloomFinal.GetUniformLocation("bloom"), mBloom ? 1 : 0);
			GL.Uniform1(mShaderBloomFinal.GetUniformLocation("exposure"), mExposure);
            mQuad.Draw();
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

            if (e.Key == OpenTK.Input.Key.C)
			{
                mBloom = !mBloom;
			}
		}

		public override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Key == OpenTK.Input.Key.Plus)
			{
				mExposure += (float)(2.0 * mDeltaTime);
			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{
				mExposure -= (float)(2.0 * mDeltaTime);
				mExposure = (mExposure >= 0.0f ? mExposure : 0.0f);
			}
		}

		private Camera mCamera = null;
		private Cube mCube = null;
		private Sphere mSphere = null;
        private Quad mQuad = null;
		private GLProgram mShader = null;
		private GLProgram mShaderLight = null;
		private GLProgram mShaderBlur = null;
		private GLProgram mShaderBloomFinal = null;
        private GLTexture2D mWoodTexture = null;
        private GLTexture2D mContainerTexture = null;
		private List<Vector3> mLightPositions = new List<Vector3>();
		private List<Vector3> mLightColors = new List<Vector3>();
        private int mHDRFBO = 0;
        private int[] mPingpongFBO = {0, 0};
		private int[] mPingpongColorbuffers = {0, 0};
        private int[] mColorBuffers = {0, 0};
        private bool mBloom = true;
        private float mExposure = 1.0f;
	}
}

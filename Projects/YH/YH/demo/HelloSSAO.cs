﻿using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using System.Collections.Generic;

namespace YH
{
	public class HelloSSAO : Application
	{
		public HelloSSAO() : base("HelloSSAO")
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
            mCamera.MovementSpeed *= 2.0f;

            //
			mCameraController = new CameraController(mAppName, mCamera);

			//
			mShaderGeometryPass = new GLProgram(@"Resources/ssao_geometry.vs", @"Resources/ssao_geometry.frag"); 
			mShaderLightingPass = new GLProgram(@"Resources/ssao.vs", @"Resources/ssao_lighting.frag"); 
			mShaderSSAO = new GLProgram(@"Resources/ssao.vs", @"Resources/ssao.frag"); 
			mShaderSSAOBlur = new GLProgram(@"Resources/ssao.vs", @"Resources/ssao_blur.frag"); 

			// Set samplers
			mShaderLightingPass.Use();
			GL.Uniform1(mShaderLightingPass.GetUniformLocation("gPosition"), 0);
			GL.Uniform1(mShaderLightingPass.GetUniformLocation("gNormal"), 1);
			GL.Uniform1(mShaderLightingPass.GetUniformLocation("gAlbedo"), 2);
			GL.Uniform1(mShaderLightingPass.GetUniformLocation("ssao"), 3);

            //
			mShaderSSAO.Use();
			GL.Uniform1(mShaderSSAO.GetUniformLocation("gPosition"), 0);
			GL.Uniform1(mShaderSSAO.GetUniformLocation("gNormal"), 1);
			GL.Uniform1(mShaderSSAO.GetUniformLocation("texNoise"), 2);

            //
			BuildGBuffer(wnd.Width, wnd.Height);

            //
            BuildSSAOBuffer(wnd.Width, wnd.Height);

            //
            InitSampleKernel();

            //
            InitNoiseTexture();
		}

		private void BuildGBuffer(int w, int h)
		{
            gBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);

			gPosition = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, gPosition);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, gPosition, 0);
			
            gNormal = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, gNormal);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, gNormal, 0);
			
			gAlbedo = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, gAlbedo);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, w, h, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, TextureTarget.Texture2D, gAlbedo, 0);
			
			DrawBuffersEnum[] attachments = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2 };
			GL.DrawBuffers(3, attachments);
			
			var rbo = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, w, h);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);
			
			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("ERROR::FRAMEBUFFER:: BuildGBuffer is not complete!");
			}

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

        private void BuildSSAOBuffer(int w, int h)
        {
            ssaoFBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ssaoFBO);
			
            ssaoColorBuffer = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ssaoColorBuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ssaoColorBuffer, 0);
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("SSAO Framebuffer not complete!");
			}

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            ssaoBlurFBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ssaoBlurFBO);
            ssaoColorBufferBlur = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ssaoColorBufferBlur);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ssaoColorBufferBlur, 0);
			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("SSAO Blur Framebuffer not complete!");
			}
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private float lerp(float a, float b, float f)
		{
			return a + f * (b - a);
		}

        private void InitSampleKernel()
        {
            var rd = new Random(System.DateTime.Now.Millisecond);
			for (var i = 0; i < 64; ++i)
			{
                Vector3 sample = new Vector3((float)rd.NextDouble() * 2.0f - 1.0f, (float)rd.NextDouble() * 2.0f - 1.0f, (float)rd.NextDouble());
                sample = Vector3.Normalize(sample);
                sample *= (float)rd.NextDouble();
                float scale = (float)i / 64.0f;
                scale = lerp(0.1f, 1.0f, scale * scale);
				sample *= scale;
                ssaoKernel.Add(sample);
		    }
        }

        private void InitNoiseTexture()
        {
            var rd = new Random(System.DateTime.Now.Millisecond);
			for (var i = 0; i < 16; i++)
			{
				Vector3 noise = new Vector3((float)rd.NextDouble() * 2.0f - 1.0f, (float)rd.NextDouble() * 2.0f - 1.0f, (float)rd.NextDouble());
                ssaoNoise.Add(noise);
			}

            noiseTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, noiseTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb32f, 4, 4, 0, PixelFormat.Rgb, PixelType.Float, ssaoNoise.ToArray());
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																  (float)wnd.Width / (float)wnd.Height,
																  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			var model = Matrix4.CreateTranslation(0, 0, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
  
			mShaderGeometryPass.Use();
			GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("view"), false, ref view);

            model = Matrix4.CreateTranslation(10.0f, 0.0f, 0.0f);
            model = Matrix4.CreateScale(new Vector3(1.0f, 20.0f, 20.0f)) * model;
            GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

            model = Matrix4.CreateTranslation(-10.0f, 0.0f, 0.0f);
            model = Matrix4.CreateScale(new Vector3(1.0f, 20.0f, 20.0f)) * model;
            GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

            model = Matrix4.CreateTranslation(0.0f, 0.0f, 10.0f);
            model = Matrix4.CreateScale(new Vector3(20.0f, 20.0f, 1.0f)) * model;
            GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

			model = Matrix4.CreateTranslation(0.0f, 0.0f, -10.0f);
			model = Matrix4.CreateScale(new Vector3(20.0f, 20.0f, 1.0f)) * model;
			GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(0.0f, 10.0f, 0.0f);
			model = Matrix4.CreateScale(new Vector3(20.0f, 1.0f, 20.0f)) * model;
			GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(0.0f, -1.0f, 0.0f);
			model = Matrix4.CreateScale(new Vector3(20.0f, 1.0f, 20.0f)) * model;
			GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
			GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(1.0f, 0.0f, 0.5f);
			GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(0.5f, 1.0f, 0.25f);
			GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("model"), false, ref model);
			mCube.Draw();


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ssaoFBO);
			GL.Clear(ClearBufferMask.ColorBufferBit);
			mShaderSSAO.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, gPosition);
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, gNormal);
			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(TextureTarget.Texture2D, noiseTexture);
			// Send kernel + rotation 
			for (var i = 0; i < 64; ++i)
            {
				GL.Uniform3(mShaderSSAO.GetUniformLocation("samples[" + i + "]"), ssaoKernel[i]);
			}
			GL.UniformMatrix4(mShaderSSAO.GetUniformLocation("projection"), false, ref projection);
			mQuad.Draw();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            //
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ssaoBlurFBO);
            GL.Clear(ClearBufferMask.ColorBufferBit);
			mShaderSSAOBlur.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, ssaoColorBuffer);
            mQuad.Draw();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			mShaderLightingPass.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, gPosition);
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, gNormal);
			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(TextureTarget.Texture2D, gAlbedo);
			GL.ActiveTexture(TextureUnit.Texture3);
			GL.BindTexture(TextureTarget.Texture2D, ssaoColorBufferBlur);

            //
            Vector4 tmp = (new Vector4(mLightPos.X, mLightPos.Y, mLightPos.Z, 1.0f)) * view;
            Vector3 lightPosView = new Vector3(tmp.X, tmp.Y, tmp.Z);
			GL.Uniform3(mShaderLightingPass.GetUniformLocation("light.Position"), lightPosView);
			GL.Uniform3(mShaderLightingPass.GetUniformLocation("light.Color"), mLightColor);

            //const float constant = 1.0f;
            const float linear = 0.09f;
            const float quadratic = 0.032f;
			GL.Uniform1(mShaderLightingPass.GetUniformLocation("light.Linear"), linear);
			GL.Uniform1(mShaderLightingPass.GetUniformLocation("light.Quadratic"), quadratic);
			GL.Uniform1(mShaderLightingPass.GetUniformLocation("draw_mode"), draw_mode);
            mQuad.Draw();
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

            if (e.Key == OpenTK.Input.Key.Number1)
			{
				draw_mode = 1;
			}
			else if (e.Key == OpenTK.Input.Key.Number2)
			{
				draw_mode = 2;
			}
			else if (e.Key == OpenTK.Input.Key.Number3)
			{
				draw_mode = 3;
			}
			else if (e.Key == OpenTK.Input.Key.Number4)
			{
				draw_mode = 4;
			}
			else if (e.Key == OpenTK.Input.Key.Number5)
			{
				draw_mode = 5;
			}
		}

		public override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Key == OpenTK.Input.Key.Plus)
			{
			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{

			}
		}

		private Camera mCamera = null;
		private Cube mCube = null;
		private Sphere mSphere = null;
		private Quad mQuad = null;

        private GLProgram mShaderGeometryPass = null;
		private GLProgram mShaderLightingPass = null;
		private GLProgram mShaderSSAO = null;
		private GLProgram mShaderSSAOBlur = null;

        private Vector3 mLightPos = new Vector3(2.0f, 4.0f, -2.0f);
		private Vector3 mLightColor = new Vector3(0.2f, 0.2f, 0.7f);

        private int gBuffer = 0;
        private int gPosition = 0;
		private int gNormal = 0;
		private int gAlbedo = 0;
        private int ssaoFBO = 0;
        private int ssaoBlurFBO = 0;
        private int ssaoColorBuffer = 0;
		private int ssaoColorBufferBlur = 0;

		private List<Vector3> ssaoKernel = new List<Vector3>();
	    private List<Vector3> ssaoNoise = new List<Vector3>();
        private int noiseTexture = 0;
		private int draw_mode = 1;
	}
}

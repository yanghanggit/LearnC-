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
			mCameraController = new CameraController(mAppName, mCamera);

			//private GLProgram shaderGeometryPass = null;//("ssao_geometry.vs", "ssao_geometry.frag");
			//private GLProgram shaderLightingPass = null;//("ssao.vs", "ssao_lighting.frag");
			//private GLProgram shaderSSAO = null;//("ssao.vs", "ssao.frag");
			//private GLProgram shaderSSAOBlur = null;//("ssao.vs", "ssao_blur.frag");
			shaderGeometryPass = new GLProgram(@"Resources/ssao_geometry.vs", @"Resources/ssao_geometry.frag"); //("g_buffer.vs", "g_buffer.frag");
			shaderLightingPass = new GLProgram(@"Resources/ssao.vs", @"Resources/ssao_lighting.frag"); //("g_buffer.vs", "g_buffer.frag");
			shaderSSAO = new GLProgram(@"Resources/ssao.vs", @"Resources/ssao.frag"); //("g_buffer.vs", "g_buffer.frag");
			shaderSSAOBlur = new GLProgram(@"Resources/ssao.vs", @"Resources/ssao_blur.frag"); //("g_buffer.vs", "g_buffer.frag");

			// Set samplers
			shaderLightingPass.Use();
			GL.Uniform1(shaderLightingPass.GetUniformLocation("gPosition"), 0);
			GL.Uniform1(shaderLightingPass.GetUniformLocation("gNormal"), 1);
			GL.Uniform1(shaderLightingPass.GetUniformLocation("gAlbedo"), 2);
			GL.Uniform1(shaderLightingPass.GetUniformLocation("ssao"), 3);

            //
			shaderSSAO.Use();
			GL.Uniform1(shaderSSAO.GetUniformLocation("gPosition"), 0);
			GL.Uniform1(shaderSSAO.GetUniformLocation("gNormal"), 1);
			GL.Uniform1(shaderSSAO.GetUniformLocation("texNoise"), 2);

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
			// Set up G-Buffer
			// 3 textures:
			// 1. Positions (RGB)
			// 2. Color (RGB) 
			// 3. Normals (RGB) 
			//GLuint gBuffer;
			//glGenFramebuffers(1, &gBuffer);
			//glBindFramebuffer(GL_FRAMEBUFFER, gBuffer);
            gBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);
            //GLuint gPosition, gNormal, gAlbedo;
			// - Position buffer
			//glGenTextures(1, &gPosition);
			//glBindTexture(GL_TEXTURE_2D, gPosition);
			//glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB16F, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGB, GL_FLOAT, NULL);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
			//glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, gPosition, 0);
			gPosition = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, gPosition);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, gPosition, 0);
			// - Normal color buffer
			//glGenTextures(1, &gNormal);
			//glBindTexture(GL_TEXTURE_2D, gNormal);
			//glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB16F, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGB, GL_FLOAT, NULL);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
			//glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT1, GL_TEXTURE_2D, gNormal, 0);
            gNormal = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, gNormal);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, gNormal, 0);
			// - Albedo color buffer
			//glGenTextures(1, &gAlbedo);
			//glBindTexture(GL_TEXTURE_2D, gAlbedo);
			//glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGBA, GL_FLOAT, NULL);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
			//glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT2, GL_TEXTURE_2D, gAlbedo, 0);
			gAlbedo = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, gAlbedo);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, w, h, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, TextureTarget.Texture2D, gAlbedo, 0);
			// - Tell OpenGL which color attachments we'll use (of this framebuffer) for rendering 
			//GLuint attachments[3] = { GL_COLOR_ATTACHMENT0, GL_COLOR_ATTACHMENT1, GL_COLOR_ATTACHMENT2 };
			//glDrawBuffers(3, attachments);
			DrawBuffersEnum[] attachments = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2 };
			GL.DrawBuffers(3, attachments);
			// - Create and attach depth buffer (renderbuffer)
			//GLuint rboDepth;
			//glGenRenderbuffers(1, &rboDepth);
			//glBindRenderbuffer(GL_RENDERBUFFER, rboDepth);
			//glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT, SCR_WIDTH, SCR_HEIGHT);
			//glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, rboDepth);
			var rbo = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, w, h);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);
			// - Finally check if framebuffer is complete
			//if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
			//std::cout << "GBuffer Framebuffer not complete!" << std::endl;
			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("ERROR::FRAMEBUFFER:: BuildGBuffer is not complete!");
			}

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

        private void BuildSSAOBuffer(int w, int h)
        {
			// Also create framebuffer to hold SSAO processing stage 
			//GLuint ssaoFBO, ssaoBlurFBO;
			//glGenFramebuffers(1, &ssaoFBO); 
            //glGenFramebuffers(1, &ssaoBlurFBO);
            ssaoFBO = GL.GenFramebuffer();
			//glBindFramebuffer(GL_FRAMEBUFFER, ssaoFBO);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ssaoFBO);
			//GLuint ssaoColorBuffer, ssaoColorBufferBlur;
			// - SSAO color buffer
			//glGenTextures(1, &ssaoColorBuffer);
            ssaoColorBuffer = GL.GenTexture();
			//glBindTexture(GL_TEXTURE_2D, ssaoColorBuffer);
            GL.BindTexture(TextureTarget.Texture2D, ssaoColorBuffer);
			//glTexImage2D(GL_TEXTURE_2D, 0, GL_RED, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGB, GL_FLOAT, NULL);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            //glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, ssaoColorBuffer, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ssaoColorBuffer, 0);
			//if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
				//std::cout << "SSAO Framebuffer not complete!" << std::endl;
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("SSAO Framebuffer not complete!");
			}

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            ssaoBlurFBO = GL.GenFramebuffer();
            // - and blur stage
            //glBindFramebuffer(GL_FRAMEBUFFER, ssaoBlurFBO);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ssaoBlurFBO);
            //glGenTextures(1, &ssaoColorBufferBlur);
            ssaoColorBufferBlur = GL.GenTexture();
			//glBindTexture(GL_TEXTURE_2D, ssaoColorBufferBlur);
            GL.BindTexture(TextureTarget.Texture2D, ssaoColorBufferBlur);
            //glTexImage2D(GL_TEXTURE_2D, 0, GL_RED, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGB, GL_FLOAT, NULL);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            //glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, ssaoColorBufferBlur, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ssaoColorBufferBlur, 0);
			//if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
				//std::cout << "SSAO Blur Framebuffer not complete!" << std::endl;
			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("SSAO Blur Framebuffer not complete!");
			}


			//glBindFramebuffer(GL_FRAMEBUFFER, 0);
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private float lerp(float a, float b, float f)
		{
			return a + f * (b - a);
		}

        private void InitSampleKernel()
        {
            var rd = new Random(System.DateTime.Now.Millisecond);
			// Sample kernel
			//std::uniform_real_distribution<GLfloat> randomFloats(0.0, 1.0); // generates random floats between 0.0 and 1.0
			//std::default_random_engine generator;
			//std::vector<glm::vec3> ssaoKernel;
			for (var i = 0; i < 64; ++i)
			{
				//glm::vec3 sample(randomFloats(generator) * 2.0 - 1.0, randomFloats(generator) * 2.0 - 1.0, randomFloats(generator));
                Vector3 sample = new Vector3((float)rd.NextDouble() * 2.0f - 1.0f, (float)rd.NextDouble() * 2.0f - 1.0f, (float)rd.NextDouble());
				//sample = glm::normalize(sample);
                sample = Vector3.Normalize(sample);
                //sample *= randomFloats(generator);
                sample *= (float)rd.NextDouble();
                //GLfloat scale = GLfloat(i) / 64.0;
                float scale = (float)i / 64.0f;
                // Scale samples s.t. they're more aligned to center of kernel
                scale = lerp(0.1f, 1.0f, scale * scale);
				sample *= scale;
				//ssaoKernel.push_back(sample);
                ssaoKernel.Add(sample);
		    }
        }

        private void InitNoiseTexture()
        {
            var rd = new Random(System.DateTime.Now.Millisecond);
			// Noise texture
			//std::vector<glm::vec3> ssaoNoise;
			
			for (var i = 0; i < 16; i++)
			{
				//glm::vec3 noise(randomFloats(generator) * 2.0 - 1.0, randomFloats(generator) * 2.0 - 1.0, 0.0f); // rotate around z-axis (in tangent space)
				Vector3 noise = new Vector3((float)rd.NextDouble() * 2.0f - 1.0f, (float)rd.NextDouble() * 2.0f - 1.0f, (float)rd.NextDouble());
                //ssaoNoise.push_back(noise);
                ssaoNoise.Add(noise);
			}

            //glGenTextures(1, &noiseTexture);
            noiseTexture = GL.GenTexture();
			//glBindTexture(GL_TEXTURE_2D, noiseTexture);
            GL.BindTexture(TextureTarget.Texture2D, noiseTexture);
			//glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB32F, 4, 4, 0, GL_RGB, GL_FLOAT, &ssaoNoise[0]);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb32f, 4, 4, 0, PixelFormat.Rgb, PixelType.Float, ssaoNoise.ToArray());

			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
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


			//
			// 1. Geometry Pass: render scene's geometry/color data into gbuffer
			//glBindFramebuffer(GL_FRAMEBUFFER, gBuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);
			//glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
   //         glm::mat4 projection = glm::perspective(glm::radians(camera.Zoom), (GLfloat)SCR_WIDTH / (GLfloat)SCR_HEIGHT, 0.1f, 50.0f);
			//glm::mat4 view = camera.GetViewMatrix();
			//glm::mat4 model;
			shaderGeometryPass.Use();
			//glUniformMatrix4fv(glGetUniformLocation(shaderGeometryPass.Program, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
			//glUniformMatrix4fv(glGetUniformLocation(shaderGeometryPass.Program, "view"), 1, GL_FALSE, glm::value_ptr(view));
			GL.UniformMatrix4(shaderGeometryPass.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shaderGeometryPass.GetUniformLocation("view"), false, ref view);
            // Background cubes
            // Note that AO doesn't work too well on flat surfaces so simply scaling the cube as the background room wouldn't work
            // as the resulting faces of the cube are completely flat.
            //model = glm::translate(model, glm::vec3(10.0f, 0.0f, 0.0f));
            //model = glm::scale(model, glm::vec3(1.0f, 20.0f, 20.0f));
            model = Matrix4.CreateTranslation(10.0f, 0.0f, 0.0f);
            model = Matrix4.CreateScale(new Vector3(1.0f, 20.0f, 20.0f)) * model;
			//glUniformMatrix4fv(glGetUniformLocation(shaderGeometryPass.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
            GL.UniformMatrix4(shaderGeometryPass.GetUniformLocation("model"), false, ref model);
			//RenderCube();
            mCube.Draw();

   //         model = glm::mat4();
			//model = glm::translate(model, glm::vec3(-10.0f, 0.0f, 0.0f));
			//model = glm::scale(model, glm::vec3(1.0f, 20.0f, 20.0f));
			//glUniformMatrix4fv(glGetUniformLocation(shaderGeometryPass.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
			//RenderCube();
            model = Matrix4.CreateTranslation(-10.0f, 0.0f, 0.0f);
            model = Matrix4.CreateScale(new Vector3(1.0f, 20.0f, 20.0f)) * model;
            GL.UniformMatrix4(shaderGeometryPass.GetUniformLocation("model"), false, ref model);
            mCube.Draw();


   //         model = glm::mat4();
			//model = glm::translate(model, glm::vec3(0.0f, 0.0f, 10.0f));
			//model = glm::scale(model, glm::vec3(20.0f, 20.0f, 1.0f));
			//glUniformMatrix4fv(glGetUniformLocation(shaderGeometryPass.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
			//RenderCube();
            model = Matrix4.CreateTranslation(0.0f, 0.0f, 10.0f);
            model = Matrix4.CreateScale(new Vector3(20.0f, 20.0f, 1.0f)) * model;
            GL.UniformMatrix4(shaderGeometryPass.GetUniformLocation("model"), false, ref model);
            mCube.Draw();


   //         model = glm::mat4();
			//model = glm::translate(model, glm::vec3(0.0f, 0.0f, -10.0f));
			//model = glm::scale(model, glm::vec3(20.0f, 20.0f, 1.0f));
			//glUniformMatrix4fv(glGetUniformLocation(shaderGeometryPass.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
			//RenderCube();
			model = Matrix4.CreateTranslation(0.0f, 0.0f, -10.0f);
			model = Matrix4.CreateScale(new Vector3(20.0f, 20.0f, 1.0f)) * model;
			GL.UniformMatrix4(shaderGeometryPass.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			//model = glm::mat4();
			//model = glm::translate(model, glm::vec3(0.0f, 10.0f, 0.0f));
			//model = glm::scale(model, glm::vec3(20.0f, 1.0f, 20.0f));
			//glUniformMatrix4fv(glGetUniformLocation(shaderGeometryPass.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
			//RenderCube();
			model = Matrix4.CreateTranslation(0.0f, 10.0f, 0.0f);
			model = Matrix4.CreateScale(new Vector3(20.0f, 1.0f, 20.0f)) * model;
			GL.UniformMatrix4(shaderGeometryPass.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

            // Floor cube
   //         model = glm::mat4();
			//model = glm::translate(model, glm::vec3(0.0, -1.0f, 0.0f));
			//model = glm::scale(model, glm::vec3(20.0f, 1.0f, 20.0f));
			//glUniformMatrix4fv(glGetUniformLocation(shaderGeometryPass.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
			//RenderCube();
			model = Matrix4.CreateTranslation(0.0f, -1.0f, 0.0f);
			model = Matrix4.CreateScale(new Vector3(20.0f, 1.0f, 20.0f)) * model;
			GL.UniformMatrix4(shaderGeometryPass.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

            // Nanosuit model on the floor
			//model = glm::mat4();
			//model = glm::translate(model, glm::vec3(0.0f, 0.0f, 5.0));
			//model = glm::rotate(model, glm::radians(-90.0f), glm::vec3(1.0, 0.0, 0.0));
			//model = glm::scale(model, glm::vec3(0.5f));
			//glUniformMatrix4fv(glGetUniformLocation(shaderGeometryPass.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
			//nanosuit.Draw(shaderGeometryPass);
			//glBindFramebuffer(GL_FRAMEBUFFER, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
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


        private GLProgram shaderGeometryPass = null;//("ssao_geometry.vs", "ssao_geometry.frag");
		private GLProgram shaderLightingPass = null;//("ssao.vs", "ssao_lighting.frag");
		private GLProgram shaderSSAO = null;//("ssao.vs", "ssao.frag");
		private GLProgram shaderSSAOBlur = null;//("ssao.vs", "ssao_blur.frag");

        private Vector3 lightPos = new Vector3(2.0f, 4.0f, -2.0f);
		private Vector3 lightColor = new Vector3(0.2f, 0.2f, 0.7f);

        private int gBuffer = 0;
        private int gPosition = 0, gNormal = 0, gAlbedo = 0;
        private int ssaoFBO = 0, ssaoBlurFBO = 0;
        private int ssaoColorBuffer = 0, ssaoColorBufferBlur = 0;
        private List<Vector3> ssaoKernel = new List<Vector3>();
	    private List<Vector3> ssaoNoise = new List<Vector3>();
        private int noiseTexture = 0;
	}
}

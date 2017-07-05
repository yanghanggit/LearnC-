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

			shaderSSAO.Use();
			GL.Uniform1(shaderSSAO.GetUniformLocation("gPosition"), 0);
			GL.Uniform1(shaderSSAO.GetUniformLocation("gNormal"), 1);
			GL.Uniform1(shaderSSAO.GetUniformLocation("texNoise"), 2);


			/*
			//
			mDiffuseMap = new GLTexture2D(@"Resources/Texture/container2.png");
			mSpecularMap = new GLTexture2D(@"Resources/Texture/container2_specular.png");

			//
			mShaderGeometryPass = new GLProgram(@"Resources/g_buffer.vs", @"Resources/g_buffer.frag"); //("g_buffer.vs", "g_buffer.frag");
			mShaderLightingPass = new GLProgram(@"Resources/deferred_shading.vs", @"Resources/deferred_shading.frag");// ("deferred_shading.vs", "deferred_shading.frag");
			mShaderLightBox = new GLProgram(@"Resources/deferred_light_box.vs", @"Resources/deferred_light_box.frag"); //("deferred_light_box.vs", "deferred_light_box.frag");

			// Set samplers
			mShaderLightingPass.Use();
			GL.Uniform1(mShaderLightingPass.GetUniformLocation("gPosition"), 0);
			GL.Uniform1(mShaderLightingPass.GetUniformLocation("gNormal"), 1);
			GL.Uniform1(mShaderLightingPass.GetUniformLocation("gAlbedoSpec"), 2);

			//
			mObjectPositions.Add(new Vector3(-3.0f, -3.0f, -3.0f));
			mObjectPositions.Add(new Vector3(0.0f, -3.0f, -3.0f));
			mObjectPositions.Add(new Vector3(3.0f, -3.0f, -3.0f));
			mObjectPositions.Add(new Vector3(-3.0f, -3.0f, 0.0f));
			mObjectPositions.Add(new Vector3(0.0f, -3.0f, 0.0f));
			mObjectPositions.Add(new Vector3(3.0f, -3.0f, 0.0f));
			mObjectPositions.Add(new Vector3(-3.0f, -3.0f, 3.0f));
			mObjectPositions.Add(new Vector3(0.0f, -3.0f, 3.0f));
			mObjectPositions.Add(new Vector3(3.0f, -3.0f, 3.0f));


			// - Colors
			const int NR_LIGHTS = 32;
			var rd = new Random(13);
			for (var i = 0; i < NR_LIGHTS; i++)
			{
				// Calculate slightly random offsets
				float xPos = (float)(((rd.Next() % 100) / 100.0) * 6.0 - 3.0);
				float yPos = (float)(((rd.Next() % 100) / 100.0) * 6.0 - 4.0 * 1.2);
				float zPos = (float)(((rd.Next() % 100) / 100.0) * 6.0 - 3.0);
				mLightPositions.Add(new Vector3(xPos, yPos, zPos));

				// Also calculate random color
				float rColor = (float)(((rd.Next() % 100) / 200.0f) + 0.5); // Between 0.5 and 1.0
				float gColor = (float)(((rd.Next() % 100) / 200.0f) + 0.5); // Between 0.5 and 1.0
				float bColor = (float)(((rd.Next() % 100) / 200.0f) + 0.5); // Between 0.5 and 1.0
				mLightColors.Add(new Vector3(rColor, gColor, bColor));
			}
            */

			BuildGBuffer(wnd.Width, wnd.Height);

            BuildSSAOBuffer(wnd.Width, wnd.Height);

            InitSampleKernel();

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


			/*
			mGBuffer = GL.GenFramebuffer();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, mGBuffer);

			mGPosition = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, mGPosition);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, mGPosition, 0);

			mGNormal = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, mGNormal);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, mGNormal, 0);

			mGAlbedoSpec = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, mGAlbedoSpec);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, w, h, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, TextureTarget.Texture2D, mGAlbedoSpec, 0);

			DrawBuffersEnum[] attachments = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2 };
			GL.DrawBuffers(3, attachments);

			//
			var rbo = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, w, h);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);

			//
			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("ERROR::FRAMEBUFFER:: BuildGBuffer is not complete!");
			}

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			*/
		}

        private void BuildSSAOBuffer(int w, int h)
        {
			// Also create framebuffer to hold SSAO processing stage 
			//GLuint ssaoFBO, ssaoBlurFBO;
			//glGenFramebuffers(1, &ssaoFBO); 
            //glGenFramebuffers(1, &ssaoBlurFBO);
            ssaoFBO = GL.GenFramebuffer();
            ssaoBlurFBO = GL.GenFramebuffer();
			//glBindFramebuffer(GL_FRAMEBUFFER, ssaoFBO);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ssaoFBO);
			//GLuint ssaoColorBuffer, ssaoColorBufferBlur;
			// - SSAO color buffer
			//glGenTextures(1, &ssaoColorBuffer);
            ssaoColorBuffer = GL.GenTexture();
			//glBindTexture(GL_TEXTURE_2D, ssaoColorBuffer);
            GL.BindTexture(TextureTarget.Texture2D, ssaoColorBuffer);
			//glTexImage2D(GL_TEXTURE_2D, 0, GL_RED, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGB, GL_FLOAT, NULL);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.CompressedRed, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
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

            // - and blur stage
            //glBindFramebuffer(GL_FRAMEBUFFER, ssaoBlurFBO);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ssaoBlurFBO);
            //glGenTextures(1, &ssaoColorBufferBlur);
            ssaoColorBufferBlur = GL.GenTexture();
			//glBindTexture(GL_TEXTURE_2D, ssaoColorBufferBlur);
            GL.BindTexture(TextureTarget.Texture2D, ssaoColorBufferBlur);
            //glTexImage2D(GL_TEXTURE_2D, 0, GL_RED, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGB, GL_FLOAT, NULL);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.CompressedRed, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
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

            /*
			GL.PolygonMode(MaterialFace.FrontAndBack, mWireframe ? PolygonMode.Line : PolygonMode.Fill);
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, mGBuffer);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			mShaderGeometryPass.Use();
			GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("view"), false, ref view);

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, mDiffuseMap.getTextureId());
			GL.Uniform1(mShaderGeometryPass.GetUniformLocation("texture_diffuse1"), 0);

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, mSpecularMap.getTextureId());
			GL.Uniform1(mShaderGeometryPass.GetUniformLocation("texture_specular1"), 1);

			for (var i = 0; i < mObjectPositions.Count; i++)
			{
				model = Matrix4.CreateTranslation(mObjectPositions[i]);
				GL.UniformMatrix4(mShaderGeometryPass.GetUniformLocation("model"), false, ref model);
				mCube.Draw();
			}
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			mShaderLightingPass.Use();
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, mGPosition);

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, mGNormal);

			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(TextureTarget.Texture2D, mGAlbedoSpec);

			const float constant = 1.0f;
			const float linear = 0.7f;
			const float quadratic = 1.8f;
			const float lightThreshold = 5.0f;

			for (var i = 0; i < mLightPositions.Count; i++)
			{
				GL.Uniform3(mShaderLightingPass.GetUniformLocation("lights[" + i + "].Position"), mLightPositions[i]);
				GL.Uniform3(mShaderLightingPass.GetUniformLocation("lights[" + i + "].Color"), mLightColors[i]);
				GL.Uniform1(mShaderLightingPass.GetUniformLocation("lights[" + i + "].Linear"), linear);
				GL.Uniform1(mShaderLightingPass.GetUniformLocation("lights[" + i + "].Quadratic"), quadratic);


				float maxBrightness = Math.Max(Math.Max(mLightColors[i].X, mLightColors[i].Y), mLightColors[i].Z);
				float radius = (-linear + (float)(Math.Sqrt(linear * linear - 4 * quadratic * (constant - (256.0 / lightThreshold) * maxBrightness)))) / (2 * quadratic);
				GL.Uniform1(mShaderLightingPass.GetUniformLocation("lights[" + i + "].Radius"), radius);
			}
			GL.Uniform3(mShaderLightingPass.GetUniformLocation("viewPos"), mCamera.Position);
			GL.Uniform1(mShaderLightingPass.GetUniformLocation("draw_mode"), mDrawMode);
			mQuad.Draw();

			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, mGBuffer);
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
			GL.BlitFramebuffer(0, 0, wnd.Width, wnd.Height,
							   0, 0, wnd.Width, wnd.Height,
							   ClearBufferMask.DepthBufferBit,
							   BlitFramebufferFilter.Nearest);

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

			mShaderLightBox.Use();
			GL.UniformMatrix4(mShaderLightBox.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mShaderLightBox.GetUniformLocation("view"), false, ref view);

			for (var i = 0; i < mLightPositions.Count; i++)
			{
				model = Matrix4.CreateTranslation(mLightPositions[i]);
				model = Matrix4.CreateScale(0.1f) * model;
				GL.UniformMatrix4(mShaderLightBox.GetUniformLocation("model"), false, ref model);
				GL.Uniform3(mShaderLightBox.GetUniformLocation("lightColor"), mLightColors[i]);
				mSphere.Draw();
			}
            */
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			//if (e.Key == OpenTK.Input.Key.C)
			//{
			//	mWireframe = !mWireframe;
			//}
			//else if (e.Key == OpenTK.Input.Key.Number1)
			//{
			//	mDrawMode = 1;
			//}
			//else if (e.Key == OpenTK.Input.Key.Number2)
			//{
			//	mDrawMode = 2;
			//}
			//else if (e.Key == OpenTK.Input.Key.Number3)
			//{
			//	mDrawMode = 3;
			//}
			//else if (e.Key == OpenTK.Input.Key.Number4)
			//{
			//	mDrawMode = 4;
			//}
			//else if (e.Key == OpenTK.Input.Key.Number5)
			//{
			//	mDrawMode = 5;
			//}
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


        //private List<Vector3> mLightPositions = new List<Vector3>();
        //private List<Vector3> mLightColors = new List<Vector3>();
        //private GLProgram mShaderGeometryPass = null;
        //private GLProgram mShaderLightingPass = null;
        //private GLProgram mShaderLightBox = null;
        //private List<Vector3> mObjectPositions = new List<Vector3>();
        //private int mGBuffer = 0;

        //// Options
        //private int mDrawMode = 1;
        //private bool mWireframe = false;

        //private GLTexture2D mDiffuseMap = null;
        //private GLTexture2D mSpecularMap = null;

        //private int mGPosition = 0;
        //private int mGNormal = 0;
        //private int mGAlbedoSpec = 0;

        private int gBuffer = 0;
        private int gPosition = 0, gNormal = 0, gAlbedo = 0;
        private int ssaoFBO = 0, ssaoBlurFBO = 0;
        private int ssaoColorBuffer = 0, ssaoColorBufferBlur = 0;
        private List<Vector3> ssaoKernel = new List<Vector3>();
	    private List<Vector3> ssaoNoise = new List<Vector3>();
        private int noiseTexture = 0;
	}
}

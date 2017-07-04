using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using System.Collections.Generic;

namespace YH
{
	public class HelloDeferredShading : Application
	{
		public HelloDeferredShading() : base("HelloDeferredShading")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			//
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.ClearColor(Color.White);
			GL.Enable(EnableCap.DepthTest);

			//
			mCube = new Cube();
			mSphere = new Sphere();
			mQuad = new Quad();

			//
			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), -90.0f, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

            //
			mDiffuseMap = new GLTexture2D(@"Resources/Texture/container2.png");
			mSpecularMap = new GLTexture2D(@"Resources/Texture/container2_specular.png");

			//
			//mShader = new GLProgram(@"Resources/bloom.vs", @"Resources/bloom.frag");
			//mShaderLight = new GLProgram(@"Resources/bloom.vs", @"Resources/light_box.frag");
			//mShaderBlur = new GLProgram(@"Resources/blur.vs", @"Resources/blur.frag");
			//mShaderBloomFinal = new GLProgram(@"Resources/bloom_final.vs", @"Resources/bloom_final.frag");
            //
			shaderGeometryPass = new GLProgram(@"Resources/g_buffer.vs", @"Resources/g_buffer.frag"); //("g_buffer.vs", "g_buffer.frag");
			shaderLightingPass = new GLProgram(@"Resources/deferred_shading.vs", @"Resources/deferred_shading.frag");// ("deferred_shading.vs", "deferred_shading.frag");
			shaderLightBox = new GLProgram(@"Resources/deferred_light_box.vs", @"Resources/deferred_light_box.frag"); //("deferred_light_box.vs", "deferred_light_box.frag");


			// Set samplers
			shaderLightingPass.Use();
			GL.Uniform1(shaderLightingPass.GetUniformLocation("gPosition"), 0);
			GL.Uniform1(shaderLightingPass.GetUniformLocation("gNormal"), 1);
			GL.Uniform1(shaderLightingPass.GetUniformLocation("gAlbedoSpec"), 2);

            //
            objectPositions.Add(new Vector3(-3.0f, -3.0f, -3.0f));
			objectPositions.Add(new Vector3(0.0f, -3.0f, -3.0f));
			objectPositions.Add(new Vector3(3.0f, -3.0f, -3.0f));
			objectPositions.Add(new Vector3(-3.0f, -3.0f, 0.0f));
			objectPositions.Add(new Vector3(0.0f, -3.0f, 0.0f));
			objectPositions.Add(new Vector3(3.0f, -3.0f, 0.0f));
			objectPositions.Add(new Vector3(-3.0f, -3.0f, 3.0f));
			objectPositions.Add(new Vector3(0.0f, -3.0f, 3.0f));
			objectPositions.Add(new Vector3(3.0f, -3.0f, 3.0f));


			// - Colors
			const int NR_LIGHTS = 32;
			//std::vector<glm::vec3> lightPositions;
			//std::vector<glm::vec3> lightColors;
			//srand(13);
            var rd = new Random(13);
			for (var i = 0; i < NR_LIGHTS; i++)
			{
                // Calculate slightly random offsets
                float xPos = (float)(((rd.NextDouble() % 100) / 100.0) * 6.0 - 3.0);
                float yPos = (float)(((rd.NextDouble() % 100) / 100.0) * 6.0 - 4.0);
                float zPos = (float)(((rd.NextDouble() % 100) / 100.0) * 6.0 - 3.0);
                //lightPositions.Add(new Vector3(xPos, yPos, zPos));
                lightPositions.Add(new Vector3(0.0f, 0.0f, 0.0f));

				// Also calculate random color
				float rColor = (float)(((rd.NextDouble() % 100) / 200.0f) + 0.5); // Between 0.5 and 1.0
				float gColor = (float)(((rd.NextDouble() % 100) / 200.0f) + 0.5); // Between 0.5 and 1.0
				float bColor = (float)(((rd.NextDouble() % 100) / 200.0f) + 0.5); // Between 0.5 and 1.0
				lightColors.Add(new Vector3(rColor, gColor, bColor));
			}

			BuildGBuffer(wnd.Width, wnd.Height);




			//// Set samplers
			//mShaderBloomFinal.Use();
			//GL.Uniform1(mShaderBloomFinal.GetUniformLocation("scene"), 0);
			//GL.Uniform1(mShaderBloomFinal.GetUniformLocation("bloomBlur"), 1);

			////
			//mLightPositions.Add(new Vector3(0.0f, 0.5f, 1.5f)); // back light
			//mLightPositions.Add(new Vector3(-4.0f, 0.5f, -3.0f));
			//mLightPositions.Add(new Vector3(3.0f, 0.5f, 1.0f));
			//mLightPositions.Add(new Vector3(-.8f, 2.4f, -1.0f));

			////
			//mLightColors.Add(new Vector3(5.0f, 5.0f, 5.0f));
			//mLightColors.Add(new Vector3(5.5f, 0.0f, 0.0f));
			//mLightColors.Add(new Vector3(0.0f, 0.0f, 15.0f));
			//mLightColors.Add(new Vector3(0.0f, 1.5f, 0.0f));

			//// Load textures
			//mWoodTexture = new GLTexture2D(@"Resources/Texture/wood.png");
			//mContainerTexture = new GLTexture2D(@"Resources/Texture/container2.png");

			////
			//BuildHDRFramebuffer(wnd.Width, wnd.Height);

			////
			//BuildPingPongFramebuffer(wnd.Width, wnd.Height);
		}

        private void BuildGBuffer(int w, int h)
        {
			//GLuint gBuffer;
			//glGenFramebuffers(1, &gBuffer);
            gBuffer = GL.GenFramebuffer();
            //glBindFramebuffer(GL_FRAMEBUFFER, gBuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);

			// - Position color buffer
			//glGenTextures(1, &gPosition);
			//glBindTexture(GL_TEXTURE_2D, gPosition);
			//glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB16F, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGB, GL_FLOAT, NULL);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
			//glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, gPosition, 0);
            gPosition = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, gPosition);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
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

			// - Color + Specular color buffer
			//glGenTextures(1, &gAlbedoSpec);
			//glBindTexture(GL_TEXTURE_2D, gAlbedoSpec);
			//glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGBA, GL_UNSIGNED_BYTE, NULL);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
			//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
			//glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT2, GL_TEXTURE_2D, gAlbedoSpec, 0);
            gAlbedoSpec = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, gAlbedoSpec);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, w, h, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, TextureTarget.Texture2D, gAlbedoSpec, 0);

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
            int rboDepth = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, w, h);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rboDepth);

			// - Finally check if framebuffer is complete
			//if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
				//std::cout << "Framebuffer not complete!" << std::endl;
			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("ERROR::FRAMEBUFFER:: BuildGBuffer is not complete!");
			}

			//glBindFramebuffer(GL_FRAMEBUFFER, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
		//private void BuildHDRFramebuffer(int w, int h)
		//{
		//	// Set up floating point framebuffer to render scene to
		//	mHDRFBO = GL.GenFramebuffer();
		//	GL.BindFramebuffer(FramebufferTarget.Framebuffer, mHDRFBO);


		//	GL.GenTextures(2, mColorBuffers);

		//	for (var i = 0; i < mColorBuffers.Length; i++)
		//	{
		//		GL.BindTexture(TextureTarget.Texture2D, mColorBuffers[i]);
		//		GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
		//		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
		//		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
		//		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
		//		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
		//		GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, mColorBuffers[i], 0);
		//	}

		//	int rboDepth = GL.GenRenderbuffer();
		//	GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
		//	GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, w, h);
		//	GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rboDepth);

		//	DrawBuffersEnum[] attachments = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1 };
		//	GL.DrawBuffers(2, attachments);

		//	if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
		//	{
		//		Console.WriteLine("ERROR::FRAMEBUFFER:: BuildHDRFramebuffer is not complete!");
		//	}
		//	GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		//}

		//private void BuildPingPongFramebuffer(int w, int h)
		//{
		//	GL.GenFramebuffers(2, mPingpongFBO);
		//	GL.GenTextures(2, mPingpongColorbuffers);


		//	for (var i = 0; i < mPingpongFBO.Length; i++)
		//	{
		//		GL.BindFramebuffer(FramebufferTarget.Framebuffer, mPingpongFBO[i]);
		//		GL.BindTexture(TextureTarget.Texture2D, mPingpongColorbuffers[i]);
		//		GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
		//		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
		//		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
		//		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
		//		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
		//		GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, mPingpongColorbuffers[i], 0);
		//		if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
		//		{
		//			Console.WriteLine("ERROR::FRAMEBUFFER:: BuildPingPongFramebuffer is not complete!");
		//		}
		//	}
		//}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			// 1. Geometry Pass: render scene's geometry/color data into gbuffer
			//glPolygonMode(GL_FRONT_AND_BACK, wireframe ? GL_LINE : GL_FILL);
			GL.PolygonMode(MaterialFace.FrontAndBack, wireframe ? PolygonMode.Line : PolygonMode.Fill);
			// 1. Geometry Pass: render scene's geometry/color data into gbuffer
			//glBindFramebuffer(GL_FRAMEBUFFER, gBuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);
			//glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			//glm::mat4 projection = glm::perspective(camera.Zoom, (GLfloat)SCR_WIDTH / (GLfloat)SCR_HEIGHT, 0.1f, 100.0f);
			//glm::mat4 view = camera.GetViewMatrix();
			//glm::mat4 model;
			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																  (float)wnd.Width / (float)wnd.Height,
																  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			var model = Matrix4.CreateTranslation(0, 0, 0);


			shaderGeometryPass.Use();
			//glUniformMatrix4fv(glGetUniformLocation(shaderGeometryPass.Program, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
			//glUniformMatrix4fv(glGetUniformLocation(shaderGeometryPass.Program, "view"), 1, GL_FALSE, glm::value_ptr(view));
			GL.UniformMatrix4(shaderGeometryPass.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shaderGeometryPass.GetUniformLocation("view"), false, ref view);

			//
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, mDiffuseMap.getTextureId());
			GL.Uniform1(shaderGeometryPass.GetUniformLocation("texture_diffuse1"), 0);

			GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, mSpecularMap.getTextureId());
			GL.Uniform1(shaderGeometryPass.GetUniformLocation("texture_specular1"), 1);

            //
            for (var i = 0; i < objectPositions.Count; i++)
			{
				//model = glm::mat4();
				//model = glm::translate(model, objectPositions[i]);
				//model = glm::scale(model, glm::vec3(0.25f));
                model = Matrix4.CreateTranslation(objectPositions[i]);
                //model = Matrix4.CreateScale(1.0f, 6.0f, 1.0f) * model;
				//glUniformMatrix4fv(glGetUniformLocation(shaderGeometryPass.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
                GL.UniformMatrix4(shaderGeometryPass.GetUniformLocation("model"), false, ref model);
				//cyborg.Draw(shaderGeometryPass);
                mCube.Draw();
			}
			//glBindFramebuffer(GL_FRAMEBUFFER, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			//glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);





			// 2. Lighting Pass: calculate lighting by iterating over a screen filled quad pixel-by-pixel using the gbuffer's content.
			//glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			shaderLightingPass.Use();
			//glActiveTexture(GL_TEXTURE0);
			//glBindTexture(GL_TEXTURE_2D, gPosition);
			//glActiveTexture(GL_TEXTURE1);
			//glBindTexture(GL_TEXTURE_2D, gNormal);
			//glActiveTexture(GL_TEXTURE2);
			//glBindTexture(GL_TEXTURE_2D, gAlbedoSpec);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, gPosition);

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, gNormal);

			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(TextureTarget.Texture2D, gAlbedoSpec);


            // Also send light relevant uniforms
            const float constant = 1.0f; // Note that we don't send this to the shader, we assume it is always 1.0 (in our case)
            const float linear = 0.7f;
            const float quadratic = 1.8f;
            const float lightThreshold = 5.0f; // 5 / 256

			for (var i = 0; i < lightPositions.Count; i++)
			{
				//glUniform3fv(glGetUniformLocation(shaderLightingPass.Program, ("lights[" + std::to_string(i) + "].Position").c_str()), 1, &lightPositions[i][0]);
				//glUniform3fv(glGetUniformLocation(shaderLightingPass.Program, ("lights[" + std::to_string(i) + "].Color").c_str()), 1, &lightColors[i][0]);
				// Update attenuation parameters and calculate radius
				GL.Uniform3(shaderLightingPass.GetUniformLocation("lights[" + i + "].Position"), lightPositions[i]);
				GL.Uniform3(shaderLightingPass.GetUniformLocation("lights[" + i + "].Color"), lightColors[i]);

				//glUniform1f(glGetUniformLocation(shaderLightingPass.Program, ("lights[" + std::to_string(i) + "].Linear").c_str()), linear);
				//glUniform1f(glGetUniformLocation(shaderLightingPass.Program, ("lights[" + std::to_string(i) + "].Quadratic").c_str()), quadratic);
                GL.Uniform1(shaderLightingPass.GetUniformLocation("lights[" + i + "].Linear"), linear);
                GL.Uniform1(shaderLightingPass.GetUniformLocation("lights[" + i + "].Quadratic"), quadratic);

                // Then calculate radius of light volume/sphere
                //const GLfloat maxBrightness = std::fmaxf(std::fmaxf(lightColors[i].r, lightColors[i].g), lightColors[i].b);
                float maxBrightness = Math.Max(Math.Max(lightColors[i].X, lightColors[i].Y), lightColors[i].Z); 
                //GLfloat radius = (-linear + static_cast<float>(std::sqrt(linear * linear - 4 * quadratic * (constant - (256.0 / lightThreshold) * maxBrightness)))) / (2 * quadratic);
                float radius = (-linear + (float)(Math.Sqrt(linear * linear - 4 * quadratic * (constant - (256.0 / lightThreshold) * maxBrightness)))) / (2 * quadratic);
				//glUniform1f(glGetUniformLocation(shaderLightingPass.Program, ("lights[" + std::to_string(i) + "].Radius").c_str()), radius);
			    GL.Uniform1(shaderLightingPass.GetUniformLocation("lights[" + i + "].Radius"), radius);
            }
			//glUniform3fv(glGetUniformLocation(shaderLightingPass.Program, "viewPos"), 1, &camera.Position[0]);
			GL.Uniform3(shaderLightingPass.GetUniformLocation("viewPos"), mCamera.Position);
            //glUniform1i(glGetUniformLocation(shaderLightingPass.Program, "draw_mode"), draw_mode);
			GL.Uniform1(shaderLightingPass.GetUniformLocation("draw_mode"), draw_mode);
            //RenderQuad();
            mQuad.Draw();



			// 2.5. Copy content of geometry's depth buffer to default framebuffer's depth buffer
			//glBindFramebuffer(GL_READ_FRAMEBUFFER, gBuffer);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, gBuffer);
			//glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0); // Write to default framebuffer
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
													   // blit to default framebuffer. Note that this may or may not work as the internal formats of both the FBO and default framebuffer have to match.
													   // the internal formats are implementation defined. This works on all of my systems, but if it doesn't on yours you'll likely have to write to the      
													   // depth buffer in another stage (or somehow see to match the default framebuffer's internal format with the FBO's internal format).
			//glBlitFramebuffer(0, 0, SCR_WIDTH, SCR_HEIGHT, 0, 0, SCR_WIDTH, SCR_HEIGHT, GL_DEPTH_BUFFER_BIT, GL_NEAREST);
			GL.BlitFramebuffer( 0, 0, wnd.Width, wnd.Height,
							    0, 0, wnd.Width, wnd.Height,
                                ClearBufferMask.DepthBufferBit,
                               BlitFramebufferFilter.Nearest);
            
            //glBindFramebuffer(GL_FRAMEBUFFER, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


			// 3. Render lights on top of scene, by blitting
			shaderLightBox.Use();
			//glUniformMatrix4fv(glGetUniformLocation(shaderLightBox.Program, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
			//glUniformMatrix4fv(glGetUniformLocation(shaderLightBox.Program, "view"), 1, GL_FALSE, glm::value_ptr(view));
			GL.UniformMatrix4(shaderLightBox.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shaderLightBox.GetUniformLocation("view"), false, ref view);

            for (var i = 0; i < lightPositions.Count; i++)
			{
				//model = glm::mat4();
				//model = glm::translate(model, lightPositions[i]);
                model = Matrix4.CreateTranslation(lightPositions[i]);
				//model = glm::scale(model, glm::vec3(0.25f));
                //model = Matrix4.CreateScale(0.25f) * model;
				//glUniformMatrix4fv(glGetUniformLocation(shaderLightBox.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
                GL.UniformMatrix4(shaderLightBox.GetUniformLocation("model"), false, ref model);
				//glUniform3fv(glGetUniformLocation(shaderLightBox.Program, "lightColor"), 1, &lightColors[i][0]);
                GL.Uniform3(shaderLightBox.GetUniformLocation("lightColor"), lightColors[i]);
				//RenderCube();
                mSphere.Draw();
			}
            /*
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
			*/
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.C)
			{
                wireframe = !wireframe;
				//mBloom = !mBloom;
			}
		}

		public override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Key == OpenTK.Input.Key.Plus)
			{
				//mExposure += (float)(2.0 * mDeltaTime);
			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{
				//mExposure -= (float)(2.0 * mDeltaTime);
				//mExposure = (mExposure >= 0.0f ? mExposure : 0.0f);
			}
		}

		private Camera mCamera = null;
		private Cube mCube = null;
		private Sphere mSphere = null;
		private Quad mQuad = null;
		//private GLProgram mShader = null;
		//private GLProgram mShaderLight = null;
		//private GLProgram mShaderBlur = null;
		//private GLProgram mShaderBloomFinal = null;
		//private GLTexture2D mWoodTexture = null;
		//private GLTexture2D mContainerTexture = null;

		//std::vector<glm::vec3> lightPositions;
		//std::vector<glm::vec3> lightColors;


		private List<Vector3> lightPositions = new List<Vector3>();
		private List<Vector3> lightColors = new List<Vector3>();
		//private int mHDRFBO = 0;
		//private int[] mPingpongFBO = { 0, 0 };
		//private int[] mPingpongColorbuffers = { 0, 0 };
		//private int[] mColorBuffers = { 0, 0 };
		//private bool mBloom = true;
		//private float mExposure = 1.0f;


        private GLProgram shaderGeometryPass = null;
        private GLProgram shaderLightingPass = null;
        private GLProgram shaderLightBox = null;
        private List<Vector3> objectPositions = new List<Vector3>();
        private int gBuffer = 0;

		// Options
		private int draw_mode = 1;
		private bool wireframe = false;

		private GLTexture2D mDiffuseMap = null;
		private GLTexture2D mSpecularMap = null;

        private int gPosition = 0; 
        private int gNormal = 0; 
        private int gAlbedoSpec = 0;
    	
	}
}

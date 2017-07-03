using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
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
			GL.ClearColor(0.1f, 0.1f, 0.1f, 0.1f);
			GL.Enable(EnableCap.DepthTest);

            //
			mCube = new Cube();
			mSphere = new Sphere();

            //
			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), 90.0f, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

            //
            shader = new GLProgram(@"Resources/bloom.vs", @"Resources/bloom.frag");
            shaderLight = new GLProgram(@"Resources/bloom.vs", @"Resources/light_box.frag");
            shaderBlur = new GLProgram(@"Resources/blur.vs", @"Resources/blur.frag");
            shaderBloomFinal = new GLProgram(@"Resources/bloom_final.vs", @"Resources/bloom_final.frag");

			// Set samplers
			shaderBloomFinal.Use();
            GL.Uniform1(shaderBloomFinal.GetUniformLocation("scene"), 0);
            GL.Uniform1(shaderBloomFinal.GetUniformLocation("bloomBlur"), 1);

			//
			// Light sources
			// - Positions
			//std::vector<glm::vec3> lightPositions;
			mLightPositions.Add(new Vector3(0.0f, 0.5f, 1.5f)); // back light
			mLightPositions.Add(new Vector3(-4.0f, 0.5f, -3.0f));
			mLightPositions.Add(new Vector3(3.0f, 0.5f, 1.0f));
			mLightPositions.Add(new Vector3(-.8f, 2.4f, -1.0f));
			// - Colors
			//std::vector<glm::vec3> lightColors;
			mLightColors.Add(new Vector3(5.0f, 5.0f, 5.0f));
			mLightColors.Add(new Vector3(5.5f, 0.0f, 0.0f));
			mLightColors.Add(new Vector3(0.0f, 0.0f, 15.0f));
			mLightColors.Add(new Vector3(0.0f, 1.5f, 0.0f));

			// Load textures
			woodTexture = new GLTexture2D(@"Resources/Texture/wood.png"); //loadTexture(FileSystem::getPath("resources/textures/wood.png").c_str());
			containerTexture = new GLTexture2D(@"Resources/Texture/container2.png"); //loadTexture(FileSystem::getPath("resources/textures/container2.png").c_str());

            //
            BuildHDRFramebuffer(wnd.Width, wnd.Height);

            //
            BuildPingPongFramebuffer(wnd.Width, wnd.Height);

			/*
			mCube = new Cube();
			mQuad = new Quad();
			mSphere = new Sphere();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), 90.0f, Camera.PITCH);
			mCamera.MovementSpeed *= 2.0f;

			mCameraController = new CameraController(mAppName, mCamera);

			//
			mHDRFBO = new GLHDRFramebuffer(wnd.Width, wnd.Height);

			//
			mLightingShader = new GLProgram(@"Resources/lighting.vs", @"Resources/lighting.frag");
			mHDRShader = new GLProgram(@"Resources/hdr.vs", @"Resources/hdr.frag");

			//
			mWoodTexture = new GLTexture2D(@"Resources/Texture/wood.png");

			//
			mLightPositions.Add(new Vector3(0.0f, 0.0f, 49.5f));
			mLightPositions.Add(new Vector3(-1.4f, -1.9f, 9.0f));
			mLightPositions.Add(new Vector3(0.0f, -1.8f, 4.0f));
			mLightPositions.Add(new Vector3(0.8f, -1.7f, 6.0f));

			//
			mLightColors.Add(new Vector3(200.0f, 200.0f, 200.0f));
			mLightColors.Add(new Vector3(0.1f, 0.0f, 0.0f));
			mLightColors.Add(new Vector3(0.0f, 0.0f, 0.2f));
			mLightColors.Add(new Vector3(0.0f, 0.1f, 0.0f));

			//
			mLampShader = new GLProgram(@"Resources/lamp.vs", @"Resources/lamp.frag");

			//
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(0.1f, 0.1f, 0.1f, 0.1f);
			GL.Enable(EnableCap.DepthTest);
			*/
		}

        private void BuildHDRFramebuffer(int w, int h)
        {
            // Set up floating point framebuffer to render scene to
            //GLuint hdrFBO;
            //glGenFramebuffers(1, &hdrFBO);
            hdrFBO = GL.GenFramebuffer();
			//glBindFramebuffer(GL_FRAMEBUFFER, hdrFBO);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, hdrFBO);
			// - Create 2 floating point color buffers (1 for normal rendering, other for brightness treshold values)
			//GLuint colorBuffers[2];
			//glGenTextures(2, colorBuffers);
            int[] colorBuffers = {0, 0};
            GL.GenTextures(2, colorBuffers);

            for (var i = 0; i < colorBuffers.Length; i++)
			{
				//glBindTexture(GL_TEXTURE_2D, colorBuffers[i]);
                GL.BindTexture(TextureTarget.Texture2D, colorBuffers[i]);
                //glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB16F, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGB, GL_FLOAT, NULL);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
				//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
				//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
				//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);  // We clamp to the edge as the blur filter would otherwise sample repeated texture values!
				//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                // attach texture to framebuffer
				//glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0 + i, GL_TEXTURE_2D, colorBuffers[i], 0);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, colorBuffers[i], 0);
			}

			// - Create and attach depth buffer (renderbuffer)
			//GLuint rboDepth;
			//glGenRenderbuffers(1, &rboDepth);
            int rboDepth = GL.GenRenderbuffer();
			//glBindRenderbuffer(GL_RENDERBUFFER, rboDepth);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
			//glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT, SCR_WIDTH, SCR_HEIGHT);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, w, h);
			//glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, rboDepth);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rboDepth);

			// - Tell OpenGL which color attachments we'll use (of this framebuffer) for rendering 
			//GLuint attachments[2] = { GL_COLOR_ATTACHMENT0, GL_COLOR_ATTACHMENT1 };
            DrawBuffersEnum[] attachments = {DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1};
            //glDrawBuffers(2, attachments);
            GL.DrawBuffers(2, attachments);

			// - Finally check if framebuffer is complete
			//if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
			//	std::cout << "Framebuffer not complete!" << std::endl;
			//glBindFramebuffer(GL_FRAMEBUFFER, 0);
			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Console.WriteLine("ERROR::FRAMEBUFFER:: BuildHDRFramebuffer is not complete!");
			}
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

        private void BuildPingPongFramebuffer(int w, int h)
        {
			// Ping pong framebuffer for blurring
			//GLuint pingpongFBO[2];
			//GLuint pingpongColorbuffers[2];
			//glGenFramebuffers(2, pingpongFBO);
			//glGenTextures(2, pingpongColorbuffers);

            GL.GenFramebuffers(2, pingpongFBO);
            GL.GenTextures(2, pingpongColorbuffers);


            for (var i = 0; i < pingpongFBO.Length; i++)
			{
				//glBindFramebuffer(GL_FRAMEBUFFER, pingpongFBO[i]);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, pingpongFBO[i]);
				//glBindTexture(GL_TEXTURE_2D, pingpongColorbuffers[i]);
                GL.BindTexture(TextureTarget.Texture2D, pingpongColorbuffers[i]);


				//glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB16F, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGB, GL_FLOAT, NULL);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);



				//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
				//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
				//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE); // We clamp to the edge as the blur filter would otherwise sample repeated texture values!
				//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                //glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, pingpongColorbuffers[i], 0);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, pingpongColorbuffers[i], 0);


				// Also check if framebuffers are complete (no need for depth buffer)
				//if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
				//std::cout << "Framebuffer not complete!" << std::endl;

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
            /*
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, mHDRFBO.mHDRFBO);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																  (float)wnd.Width / (float)wnd.Height,
																  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			var model = Matrix4.CreateTranslation(0, 0, 0);

			mLightingShader.Use();
			GL.UniformMatrix4(mLightingShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mLightingShader.GetUniformLocation("view"), false, ref view);

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, mWoodTexture.getTextureId());

			for (var i = 0; i < mLightPositions.Count; ++i)
			{
				GL.Uniform3(mLightingShader.GetUniformLocation("lights[" + i + "].Position"), mLightPositions[i]);
				GL.Uniform3(mLightingShader.GetUniformLocation("lights[" + i + "].Color"), mLightColors[i]);
			}
			GL.Uniform3(mLightingShader.GetUniformLocation("viewPos"), mCamera.Position);
			model = Matrix4.CreateTranslation(0.0f, 0.0f, 25.0f);
			model = Matrix4.CreateScale(5.0f, 5.0f, 55.0f) * model;
			GL.UniformMatrix4(mLightingShader.GetUniformLocation("model"), false, ref model);
			GL.Uniform1(mLightingShader.GetUniformLocation("inverse_normals"), 1);
			mCube.Draw();


			//
			mLampShader.Use();
			GL.UniformMatrix4(mLampShader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(mLampShader.GetUniformLocation("view"), false, ref view);

			for (var i = 0; i < mLightPositions.Count; ++i)
			{
				var pos = mLightPositions[i];
				model = Matrix4.CreateTranslation(pos);
				model = Matrix4.CreateScale(0.1f) * model;
				GL.UniformMatrix4(mLampShader.GetUniformLocation("model"), false, ref model);

				var color = mLightColors[i];
				GL.Uniform3(mLampShader.GetUniformLocation("set_color"), color);

				mSphere.Draw();
			}

			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

			//
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			mHDRShader.Use();

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, mHDRFBO.mColorBuffer);
			GL.Uniform1(mHDRShader.GetUniformLocation("hdr"), mUseHDR ? 1 : 0);
			GL.Uniform1(mHDRShader.GetUniformLocation("exposure"), mExposure);
			mQuad.Draw();
			*/
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.H)
			{
				//mUseHDR = !mUseHDR;
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
		private GLProgram shader = null;//("bloom.vs", "bloom.frag");
		private GLProgram shaderLight = null;//("bloom.vs", "light_box.frag");
		private GLProgram shaderBlur = null;//("blur.vs", "blur.frag");
		private GLProgram shaderBloomFinal = null;//("bloom_final.vs", "bloom_final.frag");
        private GLTexture2D woodTexture = null;
        private GLTexture2D containerTexture = null;
		



		//

		//private Camera mCamera = null;
		//private GLHDRFramebuffer mHDRFBO = null;
		//private GLProgram mLightingShader = null;
		//private GLProgram mHDRShader = null;
		//private GLTexture2D mWoodTexture = null;
		private List<Vector3> mLightPositions = new List<Vector3>();
		private List<Vector3> mLightColors = new List<Vector3>();
        //private float mExposure = 1.0f;
        //private bool mUseHDR = true;
        //private GLProgram mLampShader = null;

        private int hdrFBO = 0;
        private int[] pingpongFBO = {0, 0};
		private int[] pingpongColorbuffers = { 0, 0 };
	}
}

using System;
using OpenTK.Graphics.OpenGL;
//using System.Drawing;
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
            mQuad = new Quad();

            //
			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), -90.0f, Camera.PITCH);
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
			mLightPositions.Add(new Vector3(0.0f, 0.5f, 1.5f)); // back light
			mLightPositions.Add(new Vector3(-4.0f, 0.5f, -3.0f));
			mLightPositions.Add(new Vector3(3.0f, 0.5f, 1.0f));
			mLightPositions.Add(new Vector3(-.8f, 2.4f, -1.0f));
		
            //
			mLightColors.Add(new Vector3(5.0f, 5.0f, 5.0f));
			mLightColors.Add(new Vector3(5.5f, 0.0f, 0.0f));
			mLightColors.Add(new Vector3(0.0f, 0.0f, 15.0f));
			mLightColors.Add(new Vector3(0.0f, 1.5f, 0.0f));

			// Load textures
			woodTexture = new GLTexture2D(@"Resources/Texture/wood.png"); 
			containerTexture = new GLTexture2D(@"Resources/Texture/container2.png"); 

            //
            BuildHDRFramebuffer(wnd.Width, wnd.Height);

            //
            BuildPingPongFramebuffer(wnd.Width, wnd.Height);
		}

        private void BuildHDRFramebuffer(int w, int h)
        {
            // Set up floating point framebuffer to render scene to
            hdrFBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, hdrFBO);


            GL.GenTextures(2, colorBuffers);

            for (var i = 0; i < colorBuffers.Length; i++)
			{
                GL.BindTexture(TextureTarget.Texture2D, colorBuffers[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, colorBuffers[i], 0);
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
            GL.GenFramebuffers(2, pingpongFBO);
            GL.GenTextures(2, pingpongColorbuffers);


            for (var i = 0; i < pingpongFBO.Length; i++)
			{
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, pingpongFBO[i]);
                GL.BindTexture(TextureTarget.Texture2D, pingpongColorbuffers[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, pingpongColorbuffers[i], 0);
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
            //glBindFramebuffer(GL_FRAMEBUFFER, hdrFBO);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, hdrFBO);
            //===============================================================================================
            //GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																  (float)wnd.Width / (float)wnd.Height,
																  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			var model = Matrix4.CreateTranslation(0, 0, 0);

			shader.Use();
			GL.UniformMatrix4(shader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shader.GetUniformLocation("view"), false, ref view);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, woodTexture.getTextureId());

            for (int i = 0; i < mLightPositions.Count; i++)
			{
           		GL.Uniform3(shader.GetUniformLocation("lights[" + i + "].Position"), mLightPositions[i]);
                GL.Uniform3(shader.GetUniformLocation("lights[" + i + "].Color"), mLightColors[i]);
			}
			GL.Uniform3(shader.GetUniformLocation("viewPos"), mCamera.Position);

            model = Matrix4.CreateTranslation(0.0f, -1.0f, 0.0f);
            model = Matrix4.CreateScale(25.0f, 1.0f, 25.0f) * model;
            GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

            GL.BindTexture(TextureTarget.Texture2D, containerTexture.getTextureId());

            model = Matrix4.CreateTranslation(0.0f, 1.5f, 0.0f);
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();
	
            model = Matrix4.CreateTranslation(2.0f, 0.0f, 1.0f);
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

            model = Matrix4.CreateTranslation(-1.0f, -1.0f, 2.0f);
            model = Matrix4.CreateFromAxisAngle(Vector3.Normalize(new Vector3(1.0f, 0.0f, 1.0f)), 60.0f) * model;
            model = Matrix4.CreateScale(2.0f) * model;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();
			
			model = Matrix4.CreateTranslation(0.0f, 2.7f, 4.0f);
            model = Matrix4.CreateFromAxisAngle(Vector3.Normalize(new Vector3(1.0f, 0.0f, 1.0f)), 23.0f) * model;
            model = Matrix4.CreateScale(2.5f) * model;
            GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(-2.0f, 1.0f, -3.0f);
			model = Matrix4.CreateFromAxisAngle(Vector3.Normalize(new Vector3(1.0f, 0.0f, 1.0f)), 124.0f) * model;
			model = Matrix4.CreateScale(2.0f) * model;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();
      
			model = Matrix4.CreateTranslation(-3.0f, 0.0f, 0.0f);
            GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

            // - finally show all the light sources as bright cubes
			shaderLight.Use();
			GL.UniformMatrix4(shaderLight.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shaderLight.GetUniformLocation("view"), false, ref view);

            for (int i = 0; i < mLightPositions.Count; i++)
			{
                model = Matrix4.CreateTranslation(mLightPositions[i]);
				model = Matrix4.CreateScale(0.5f) * model;
                GL.UniformMatrix4(shaderLight.GetUniformLocation("model"), false, ref model);
                GL.Uniform3(shaderLight.GetUniformLocation("lightColor"), mLightColors[i]);
                mCube.Draw();
			}
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			//==============================================================================================

			// 2. Blur bright fragments w/ two-pass Gaussian Blur 
			bool horizontal = true, first_iteration = true;
			int amount = 10;
			shaderBlur.Use();
			for (int i = 0; i < amount; i++)
			{
				//glBindFramebuffer(GL_FRAMEBUFFER, pingpongFBO[horizontal]);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, pingpongFBO[horizontal ? 1 : 0]);
				//glUniform1i(glGetUniformLocation(shaderBlur.Program, "horizontal"), horizontal);
				GL.Uniform1(shaderBlur.GetUniformLocation("horizontal"), horizontal ? 1 : 0);
                //glBindTexture(GL_TEXTURE_2D, first_iteration ? colorBuffers[1] : pingpongColorbuffers[!horizontal]);  // bind texture of other framebuffer (or scene if first iteration)
                GL.BindTexture(TextureTarget.Texture2D, first_iteration ? colorBuffers[1] : pingpongColorbuffers[horizontal ? 0 : 1]);
                mQuad.Draw();
				horizontal = !horizontal;
				if (first_iteration)
                {
                    first_iteration = false;
                }
					
			}
			//glBindFramebuffer(GL_FRAMEBUFFER, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			//==============================================================================================

			// 2. Now render floating point color buffer to 2D quad and tonemap HDR colors to default framebuffer's (clamped) color range
			//glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			shaderBloomFinal.Use();
			//glActiveTexture(GL_TEXTURE0);
			//glBindTexture(GL_TEXTURE_2D, colorBuffers[0]);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, colorBuffers[0]);


			//glBindTexture(GL_TEXTURE_2D, pingpongColorbuffers[!horizontal]);
			//glActiveTexture(GL_TEXTURE1);
			//glBindTexture(GL_TEXTURE_2D, pingpongColorbuffers[!horizontal]);

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, pingpongColorbuffers[horizontal ? 0 : 1]);


			//glUniform1i(glGetUniformLocation(shaderBloomFinal.Program, "bloom"), bloom);
			//glUniform1f(glGetUniformLocation(shaderBloomFinal.Program, "exposure"), exposure);

			GL.Uniform1(shaderBloomFinal.GetUniformLocation("bloom"), mBloom ? 1 : 0);
			GL.Uniform1(shaderBloomFinal.GetUniformLocation("exposure"), mExposure);
			//RenderQuad();
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
		private GLProgram shader = null;
		private GLProgram shaderLight = null;
		private GLProgram shaderBlur = null;
		private GLProgram shaderBloomFinal = null;
        private GLTexture2D woodTexture = null;
        private GLTexture2D containerTexture = null;
		private List<Vector3> mLightPositions = new List<Vector3>();
		private List<Vector3> mLightColors = new List<Vector3>();
        private int hdrFBO = 0;
        private int[] pingpongFBO = {0, 0};
		private int[] pingpongColorbuffers = {0, 0};
        private int[] colorBuffers = { 0, 0 };
        private bool mBloom = true;
        private float mExposure = 1.0f;
	}
}

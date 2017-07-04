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
            GL.ClearColor(Color.Black);

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
            var rd = new Random(13);
            //rd.Next();
            //var a = rd.NextDouble();
			for (var i = 0; i < NR_LIGHTS; i++)
			{
                // Calculate slightly random offsets
                float xPos = (float)(((rd.Next() % 100) / 100.0) * 6.0 - 3.0);
                float yPos = (float)(((rd.Next() % 100) / 100.0) * 6.0 - 4.0);
                float zPos = (float)(((rd.Next() % 100) / 100.0) * 6.0 - 3.0);
                lightPositions.Add(new Vector3(xPos, yPos, zPos));
                //lightPositions.Add(new Vector3(0.0f, 0.0f, 0.0f));

				// Also calculate random color
				float rColor = (float)(((rd.Next() % 100) / 200.0f) + 0.5); // Between 0.5 and 1.0
				float gColor = (float)(((rd.Next() % 100) / 200.0f) + 0.5); // Between 0.5 and 1.0
				float bColor = (float)(((rd.Next() % 100) / 200.0f) + 0.5); // Between 0.5 and 1.0
				lightColors.Add(new Vector3(rColor, gColor, bColor));
                //lightColors.Add(new Vector3(1, 0, 0));
			}

			BuildGBuffer(wnd.Width, wnd.Height);
		}

        private void BuildGBuffer(int w, int h)
        {
            gBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);

			// - Position color buffer
            gPosition = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, gPosition);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, gPosition, 0);
            //GL.BindTexture(TextureTarget.Texture2D, 0);

			// - Normal color buffer
			gNormal = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, gNormal);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, w, h, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, gNormal, 0);
			//GL.BindTexture(TextureTarget.Texture2D, 0);

			// - Color + Specular color buffer
			gAlbedoSpec = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, gAlbedoSpec);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, w, h, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, TextureTarget.Texture2D, gAlbedoSpec, 0);
			//GL.BindTexture(TextureTarget.Texture2D, 0);

			// - Tell OpenGL which color attachments we'll use (of this framebuffer) for rendering
            DrawBuffersEnum[] attachments = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2 };
			GL.DrawBuffers(3, attachments);

            // - Create and attach depth buffer (renderbuffer)
   //         int rboDepth = GL.GenRenderbuffer();
   //         GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
   //         GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, w, h);
   //         GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rboDepth);
			//GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

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
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
		
		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
            GL.Enable(EnableCap.DepthTest);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																  (float)wnd.Width / (float)wnd.Height,
																  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			var model = Matrix4.CreateTranslation(0, 0, 0);


            if (true)
            {
                // 1. Geometry Pass: render scene's geometry/color data into gbuffer
                GL.PolygonMode(MaterialFace.FrontAndBack, wireframe ? PolygonMode.Line : PolygonMode.Fill);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                //
                shaderGeometryPass.Use();
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
                    model = Matrix4.CreateTranslation(objectPositions[i]);
                    GL.UniformMatrix4(shaderGeometryPass.GetUniformLocation("model"), false, ref model);
                    mCube.Draw();
                }
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }

            if (true)
            {
                //GL.Disable(EnableCap.DepthTest);
				// 2. Lighting Pass: calculate lighting by iterating over a screen filled quad pixel-by-pixel using the gbuffer's content.
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
				shaderLightingPass.Use();
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
					GL.Uniform3(shaderLightingPass.GetUniformLocation("lights[" + i + "].Position"), lightPositions[i]);
					GL.Uniform3(shaderLightingPass.GetUniformLocation("lights[" + i + "].Color"), lightColors[i]);
					GL.Uniform1(shaderLightingPass.GetUniformLocation("lights[" + i + "].Linear"), linear);
					GL.Uniform1(shaderLightingPass.GetUniformLocation("lights[" + i + "].Quadratic"), quadratic);


					float maxBrightness = Math.Max(Math.Max(lightColors[i].X, lightColors[i].Y), lightColors[i].Z);
					float radius = (-linear + (float)(Math.Sqrt(linear * linear - 4 * quadratic * (constant - (256.0 / lightThreshold) * maxBrightness)))) / (2 * quadratic);
					GL.Uniform1(shaderLightingPass.GetUniformLocation("lights[" + i + "].Radius"), radius);
				}
				GL.Uniform3(shaderLightingPass.GetUniformLocation("viewPos"), mCamera.Position);
				GL.Uniform1(shaderLightingPass.GetUniformLocation("draw_mode"), draw_mode);
				mQuad.Draw();
            }

            if (true)
            {
				// 2.5. Copy content of geometry's depth buffer to default framebuffer's depth buffer
				GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, gBuffer);
				GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
				GL.BlitFramebuffer(0, 0, wnd.Width, wnd.Height,
                                   0, 0, wnd.Width, wnd.Height,
                                   ClearBufferMask.DepthBufferBit,
								   BlitFramebufferFilter.Nearest);

				GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }

            GL.Disable(EnableCap.DepthTest);
			// 3. Render lights on top of scene, by blitting
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			shaderLightBox.Use();
			GL.UniformMatrix4(shaderLightBox.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shaderLightBox.GetUniformLocation("view"), false, ref view);

            for (var i = 0; i < lightPositions.Count; i++)
            {
                model = Matrix4.CreateTranslation(lightPositions[i]);
                model = Matrix4.CreateScale(0.1f) * model;
                GL.UniformMatrix4(shaderLightBox.GetUniformLocation("model"), false, ref model);
                GL.Uniform3(shaderLightBox.GetUniformLocation("lightColor"), lightColors[i]);
                mSphere.Draw();
			}
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.C)
			{
                wireframe = !wireframe;
			}
			else if (e.Key == OpenTK.Input.Key.Number1)
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
		private List<Vector3> lightPositions = new List<Vector3>();
		private List<Vector3> lightColors = new List<Vector3>();
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

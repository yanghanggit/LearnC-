﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
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

			BuildGBuffer(wnd.Width, wnd.Height);
		}

        private void BuildGBuffer(int w, int h)
        {
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
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == OpenTK.Input.Key.C)
			{
                mWireframe = !mWireframe;
			}
			else if (e.Key == OpenTK.Input.Key.Number1)
			{
				mDrawMode = 1;
			}
			else if (e.Key == OpenTK.Input.Key.Number2)
			{
				mDrawMode = 2;
			}
			else if (e.Key == OpenTK.Input.Key.Number3)
			{
				mDrawMode = 3;
			}
			else if (e.Key == OpenTK.Input.Key.Number4)
			{
				mDrawMode = 4;
			}
			else if (e.Key == OpenTK.Input.Key.Number5)
			{
				mDrawMode = 5;
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
		private List<Vector3> mLightPositions = new List<Vector3>();
		private List<Vector3> mLightColors = new List<Vector3>();
        private GLProgram mShaderGeometryPass = null;
        private GLProgram mShaderLightingPass = null;
        private GLProgram mShaderLightBox = null;
        private List<Vector3> mObjectPositions = new List<Vector3>();
        private int mGBuffer = 0;

		// Options
		private int mDrawMode = 1;
		private bool mWireframe = false;

		private GLTexture2D mDiffuseMap = null;
		private GLTexture2D mSpecularMap = null;

        private int mGPosition = 0; 
        private int mGNormal = 0; 
        private int mGAlbedoSpec = 0;
	}
}

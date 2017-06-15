﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Collections.Generic;

namespace YH
{
	public class HelloFramebuffer : Application
	{
		public HelloFramebuffer() : base("HelloFramebuffer")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mFloor = new Floor();
            mQuad = new Quad();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);


			// Setup and compile our shaders
			//Shader shader("framebuffers.vs", "framebuffers.frag");
			//Shader screenShader("framebuffers_screen.vs", "framebuffers_screen.frag");
            shader = new GLProgram(@"Resources/framebuffers.vs", @"Resources/framebuffers.frag");
            screenShader = new GLProgram(@"Resources/framebuffers_screen.vs", @"Resources/framebuffers_screen.frag");
            framebuffer = new GLFramebuffer(wnd.Width, wnd.Height);

			mCubeTexture = new GLTexture2D(@"Resources/Texture/container.jpg");
			mFloorTexture = new GLTexture2D(@"Resources/Texture/metal.png");

			//


			//
			//GL.Enable(EnableCap.Blend);
			//GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			//GLuint framebuffer;
			//glGenFramebuffers(1, &framebuffer);
			//glBindFramebuffer(GL_FRAMEBUFFER, framebuffer);
			//// Create a color attachment texture
			//GLuint textureColorbuffer = generateAttachmentTexture(false, false);
			//glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, textureColorbuffer, 0);
			//// Create a renderbuffer object for depth and stencil attachment (we won't be sampling these)
			//GLuint rbo;
			//glGenRenderbuffers(1, &rbo);
			//glBindRenderbuffer(GL_RENDERBUFFER, rbo);
			//glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, screenWidth, screenHeight); // Use a single renderbuffer object for both a depth AND stencil buffer.
			//glBindRenderbuffer(GL_RENDERBUFFER, 0);
			//glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, rbo); // Now actually attach it
			//																							  // Now that we actually created the framebuffer and added all attachments we want to check if it is actually complete now
			//if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
			//	cout << "ERROR::FRAMEBUFFER:: Framebuffer is not complete!" << endl;
			//glBindFramebuffer(GL_FRAMEBUFFER, 0);
		}

		public override void Update(double dt)
		{
			base.Update(dt);

			//mSortedDistance.Clear();
			//foreach (var pos in mWindowsPositions)
			//{
			//	float distance = (mCamera.Position - pos).LengthFast;
			//	mSortedDistance.Add(distance, pos);
			//}
		}

		public override void Draw(double dt, Window wnd)
		{
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer.mFrameBufferId);

			GL.Enable(EnableCap.DepthTest);
			GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
																  (float)wnd.Width / (float)wnd.Height,
																  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

			shader.Use();

			GL.UniformMatrix4(shader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shader.GetUniformLocation("view"), false, ref view);

            GL.BindTexture(TextureTarget.Texture2D, mFloorTexture.getTextureId());
			model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mFloor.Draw();

			GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());
			model = Matrix4.CreateTranslation(-1.0f, 0.0f, -1.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

			model = Matrix4.CreateTranslation(2.0f, 0.0f, 0.0f);
			model = Matrix4.CreateScale(0.5f) * model;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			mCube.Draw();

   //         GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


   //         GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
   //         GL.Clear(ClearBufferMask.ColorBufferBit);
   //         GL.Disable(EnableCap.DepthTest);

   //         screenShader.Use();
   //         GL.BindTexture(TextureTarget.Texture2D, framebuffer.mColorAttachment0);
   //         mQuad.Draw();
			//GL.BindTexture(TextureTarget.Texture2D, 0);
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
		private Floor mFloor = null;
        private Quad mQuad = null;

		private Camera mCamera = null;

		private GLTexture2D mCubeTexture = null;
		private GLTexture2D mFloorTexture = null;

		//Vector3[] mWindowsPositions = {
		//	new Vector3(-1.5f,  0.0f, -0.48f),
		//	new Vector3( 1.5f,  0.0f,  0.51f),
		//	new Vector3( 0.0f,  0.0f,  0.7f),
		//	new Vector3(-0.3f,  0.0f, -2.3f),
		//	new Vector3( 0.5f,  0.0f, -0.6f)
		//};

		private GLProgram shader = null; 
		private GLProgram screenShader = null; 
        private GLFramebuffer framebuffer = null;
	}
}

﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
//using System.Collections.Generic;

namespace YH
{
	public class HelloSkyBox : Application
	{
		public HelloSkyBox() : base("HelloSkyBox")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

            mSkybox = new Skybox();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

            shader = new GLProgram(@"Resources/cubemaps.vs", @"Resources/cubemaps.frag");
            skyboxShader = new GLProgram(@"Resources/skybox.vs", @"Resources/skybox.frag");

            mGLTextureCube = new GLTextureCube(
	            @"Resources/Texture/skybox/right.jpg",
	            @"Resources/Texture/skybox/left.jpg",
	            @"Resources/Texture/skybox/top.jpg",
	            @"Resources/Texture/skybox/bottom.jpg",
	            @"Resources/Texture/skybox/back.jpg",
	            @"Resources/Texture/skybox/front.jpg"
            );
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Gray);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

	
			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom), (float)wnd.Width / (float)wnd.Height, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			// Draw skybox as last
            GL.DepthFunc(DepthFunction.Equal); 
			skyboxShader.Use();

            var skyView = new Matrix4(new Matrix3(view));

            GL.UniformMatrix4(skyboxShader.GetUniformLocation("view"), false, ref skyView);
            GL.UniformMatrix4(skyboxShader.GetUniformLocation("projection"), false, ref projection);

            GL.BindTexture(TextureTarget.TextureCubeMap, mGLTextureCube.mTextureCubeId);

            mSkybox.Draw();

            GL.DepthFunc(DepthFunction.Less);
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

		private Camera mCamera = null;
        private GLProgram shader = null;
		private GLProgram skyboxShader = null;
        private Skybox mSkybox = null;
        private GLTextureCube mGLTextureCube = null;
	}
}

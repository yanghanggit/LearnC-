﻿﻿﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Collections.Generic;

namespace YH
{
	public class HelloStencilTesting : Application
	{
		public HelloStencilTesting() : base("HelloStencilTesting")
		{
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
			mPlane = new Plane();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

            shader = new GLProgram(@"Resources/stencil_testing.vs", @"Resources/stencil_testing.frag");
            shaderSingleColor = new GLProgram(@"Resources/stencil_testing.vs", @"Resources/stencil_single_color.frag");

			mCubeTexture = new GLTexture2D(@"Resources/Texture/marble.jpg");
			mFloorTexture = new GLTexture2D(@"Resources/Texture/metal.png");

			//
			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Less);

            //
			GL.Enable(EnableCap.StencilTest);
            GL.StencilFunc(StencilFunction.Notequal, 1, 0xFFFFFFFF);
			GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
   
            //
            GL.ClearColor(Color.Gray);
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
           
			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
                                                                  (float)wnd.Width / (float)wnd.Height,
                                                                  0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();
			Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

            GL.StencilMask(0x00000000);

			GL.StencilFunc(StencilFunction.Always, 1, 0xFFFFFFFF);
			GL.StencilMask(0xFFFFFFFF);

            GL.Enable(EnableCap.DepthTest);

            shader.Use();
			GL.UniformMatrix4(shader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shader.GetUniformLocation("view"), false, ref view);

            GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());

            model = Matrix4.CreateTranslation(-1.0f, 0.0f, -1.0f);
            GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

            if (false)
            {
                GL.BindTexture(TextureTarget.Texture2D, mCubeTexture.getTextureId());

                GL.Disable(EnableCap.DepthTest);
                GL.StencilFunc(StencilFunction.Notequal, 1, 0xFFFFFFFF);
				GL.StencilMask(0x00000000);
				
				shaderSingleColor.Use();

                const float scale = 1.1f;

				GL.UniformMatrix4(shaderSingleColor.GetUniformLocation("projection"), false, ref projection);
				GL.UniformMatrix4(shaderSingleColor.GetUniformLocation("view"), false, ref view);

				model = Matrix4.CreateTranslation(-1.0f, 0.0f, -1.0f);
				model = Matrix4.CreateScale(scale) * model;
				GL.UniformMatrix4(shaderSingleColor.GetUniformLocation("model"), false, ref model);
				mCube.Draw();

				GL.StencilMask(0xFFFFFFFF);
				GL.Enable(EnableCap.DepthTest);
            }
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
			else if (e.Key == OpenTK.Input.Key.Space)
			{

			}
		}

		private Cube mCube = null;
		private Plane mPlane = null;
		private Camera mCamera = null;
		private GLTexture2D mCubeTexture = null;
		private GLTexture2D mFloorTexture = null;
        private GLProgram shader = null;
		private GLProgram shaderSingleColor = null;
	}
}

using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Collections.Generic;


namespace YH
{
	public class HelloHDR : Application
	{
		public HelloHDR() : base("HelloHDR")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();
            mQuad = new Quad();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

            //
            hdrFBO = new GLHDRFramebuffer(wnd.Width, wnd.Height);

            //
			shader = new GLProgram(@"Resources/lighting.vs", @"Resources/lighting.frag");
			hdrShader = new GLProgram(@"Resources/hdr.vs", @"Resources/hdr.frag");

			//
			woodTexture = new GLTexture2D(@"Resources/Texture/wood.png");

            //
            lightPositions.Add(new Vector3(0.0f, 0.0f, 49.5f));
			lightPositions.Add(new Vector3(-1.4f, -1.9f, 9.0f));
			lightPositions.Add(new Vector3(0.0f, -1.8f, 4.0f));
			lightPositions.Add(new Vector3(0.8f, -1.7f, 6.0f));
		
            //
			lightColors.Add(new Vector3(200.0f, 200.0f, 200.0f));
			lightColors.Add(new Vector3(0.1f, 0.0f, 0.0f));
			lightColors.Add(new Vector3(0.0f, 0.0f, 0.2f));
			lightColors.Add(new Vector3(0.0f, 0.1f, 0.0f));

            //
            GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.ClearColor(Color.Gray);
			GL.Enable(EnableCap.DepthTest);
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, hdrFBO.mHDRFBO);
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

            for (var i = 0; i < lightPositions.Count; ++i)
			{
                GL.Uniform3(shader.GetUniformLocation("lights[" + i + "].Position"), lightPositions[i]);
                GL.Uniform3(shader.GetUniformLocation("lights[" + i + "].Color"), lightColors[i]);
			}
            GL.Uniform3(shader.GetUniformLocation("viewPos"), mCamera.Position);
            model = Matrix4.CreateTranslation(0.0f, 0.0f, 25.0f);
            model = Matrix4.CreateScale(5.0f, 5.0f, 55.0f) * model;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
			GL.Uniform1(shader.GetUniformLocation("inverse_normals"), 1);
			mCube.Draw();

			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


			//
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			hdrShader.Use();

			GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, hdrFBO.mColorBuffer);
			GL.Uniform1(hdrShader.GetUniformLocation("hdr"), 1);
			GL.Uniform1(hdrShader.GetUniformLocation("exposure"), 1.0f);
			mQuad.Draw();
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
		}

		//
		private Cube mCube = null;
        private Quad mQuad = null;
		private Camera mCamera = null;
        private GLHDRFramebuffer hdrFBO = null;
        private GLProgram shader = null;
        private GLProgram hdrShader = null;
		private GLTexture2D woodTexture = null;
        private List<Vector3> lightPositions = new List<Vector3>();
        private List<Vector3> lightColors = new List<Vector3>();
	}
}

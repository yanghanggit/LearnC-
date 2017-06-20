﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

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

            mCube = new Cube();
            mSphere = new Sphere();
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
            Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

			// Draw scene as normal
			shader.Use();

			GL.BindTexture(TextureTarget.TextureCubeMap, mGLTextureCube.mTextureCubeId);
            GL.Uniform1(shader.GetUniformLocation("ratio"), ParseRatio(mRatioIndex));

            GL.UniformMatrix4(shader.GetUniformLocation("projection"), false, ref projection);
			GL.UniformMatrix4(shader.GetUniformLocation("view"), false, ref view);
            GL.Uniform3(shader.GetUniformLocation("cameraPos"), mCamera.Position);
           
			
            model = Matrix4.CreateTranslation(0.0f, -2.0f, 0.0f);
            GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mSphere.Draw();

			model = Matrix4.CreateTranslation(0.0f, 2.0f, 0.0f);
            GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref model);
            mCube.Draw();

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
                ++mRatioIndex;
			}
			else if (e.Key == OpenTK.Input.Key.Minus)
			{
                --mRatioIndex;
                mRatioIndex = mRatioIndex >= 0 ? mRatioIndex : 0;
			}
			else if (e.Key == OpenTK.Input.Key.C)
			{
			}
		}

        float ParseRatio(int index)
        {
            float[] ratios = {
                0.0f, //nothing
                //1.00f,//空气
				1.33f,//水
				1.309f,//冰
				1.52f,//玻璃
				2.42f,//宝石
            };

            int mod = index % ratios.Length;
            return ratios[mod];
        }

        private Cube mCube = null;
        private Sphere mSphere = null;
		private Camera mCamera = null;
        private GLProgram shader = null;
		private GLProgram skyboxShader = null;
        private Skybox mSkybox = null;
        private GLTextureCube mGLTextureCube = null;
        private int mRatioIndex = 0;
	}
}

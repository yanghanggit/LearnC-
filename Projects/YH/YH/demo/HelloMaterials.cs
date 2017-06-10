using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloMaterials : Application
	{
		public HelloMaterials() : base("HelloMaterials")
		{
		}

        public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			//
			mLightShader = new GLProgram(@"Resources/materials.vs", @"Resources/materials.frag");

			mLocLightModel = mLightShader.GetUniformLocation("model");
			mLocLightView = mLightShader.GetUniformLocation("view");
			mLocLightProjection = mLightShader.GetUniformLocation("projection");

			mLocLightPos = mLightShader.GetUniformLocation("light.position");
			mLocLightViewPos = mLightShader.GetUniformLocation("viewPos");

			mLocLightAmbient = mLightShader.GetUniformLocation("light.ambient");
			mLocLightDiffuse = mLightShader.GetUniformLocation("light.diffuse");
			mLocLightSpecular = mLightShader.GetUniformLocation("light.specular");

			mLocMaterialAmbient = mLightShader.GetUniformLocation("material.ambient");
			mLocMaterialDiffuse = mLightShader.GetUniformLocation("material.diffuse");
			mLocMaterialSpecular = mLightShader.GetUniformLocation("material.specular");
			mLocMaterialShininess = mLightShader.GetUniformLocation("material.shininess");

			//
			mLampShader = new GLProgram(@"Resources/lamp.vs", @"Resources/lamp.frag");
			mLocLampModel = mLampShader.GetUniformLocation("model");
			mLocLampView = mLampShader.GetUniformLocation("view");
			mLocLampProjection = mLampShader.GetUniformLocation("projection");
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

			var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)wnd.Width / (float)wnd.Height, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);

			do
			{
				mLightShader.Use();

				GL.UniformMatrix4(mLocLightProjection, false, ref projection);
				GL.UniformMatrix4(mLocLightView, false, ref view);

				GL.Uniform3(mLocLightPos, lightPos.X, lightPos.Y, lightPos.Z);
				GL.Uniform3(mLocLightViewPos, mCamera.Position.X, mCamera.Position.Y, mCamera.Position.Z);

				Vector3 lightColor = new Vector3(0.0f, 0.0f, 0.0f);
				lightColor.X = (float)Math.Sin(mTotalRuningTime * 2.0);
				lightColor.Y = (float)Math.Sin(mTotalRuningTime * 0.7);
				lightColor.Z = (float)Math.Sin(mTotalRuningTime * 1.3);

				Vector3 diffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
				diffuseColor = lightColor * (new Vector3(0.5f, 0.5f, 0.5f));

				Vector3 ambientColor = new Vector3(0.0f, 0.0f, 0.0f);
				ambientColor = diffuseColor * (new Vector3(0.2f, 0.2f, 0.2f));

				GL.Uniform3(mLocLightAmbient,  ambientColor.X, ambientColor.Y, ambientColor.Z);
				GL.Uniform3(mLocLightDiffuse,  diffuseColor.X, diffuseColor.Y, diffuseColor.Z);
				GL.Uniform3(mLocLightSpecular, 1.0f, 1.0f, 1.0f);
				GL.Uniform3(mLocMaterialAmbient, 1.0f, 0.5f, 0.31f);
				GL.Uniform3(mLocMaterialDiffuse, 1.0f, 0.5f, 0.31f);
				GL.Uniform3(mLocMaterialSpecular, 0.5f, 0.5f, 0.5f); // Specular doesn't have full effect on this object's material
				GL.Uniform1(mLocMaterialShininess, 32.0f);

				Matrix4 model = Matrix4.CreateTranslation(0, 0, 0);
				model = Matrix4.CreateScale(0.5f) * model;
				GL.UniformMatrix4(mLocLightModel, false, ref model);

				mCube.Draw();
			}
			while (false);

			do
			{
				mLampShader.Use();

				GL.UniformMatrix4(mLocLampProjection, false, ref projection);
				GL.UniformMatrix4(mLocLampView, false, ref view);

				Matrix4 model = Matrix4.CreateTranslation(lightPos.X, lightPos.Y, lightPos.Z);
				model = Matrix4.CreateScale(0.2f) * model;
				GL.UniformMatrix4(mLocLampModel, false, ref model);

				mCube.Draw();
			}
			while (false);
		}

		private Cube mCube = null;

		private Camera mCamera = null;

		//
		private GLProgram mLightShader = null;

		private int mLocLightModel = -1;
		private int mLocLightView = -1;
		private int mLocLightProjection = -1;

		private int mLocLightPos = -1;
		private int mLocLightViewPos = -1;

		private int mLocLightAmbient = -1;
		private int mLocLightDiffuse = -1;
		private int mLocLightSpecular = -1;

		private int mLocMaterialAmbient = -1;
		private int mLocMaterialDiffuse = -1;
		private int mLocMaterialSpecular = -1;
		private int mLocMaterialShininess = -1;

		//
		private GLProgram mLampShader = null;
		private int mLocLampModel = -1;
		private int mLocLampView = -1;
		private int mLocLampProjection = -1;	
	}
}

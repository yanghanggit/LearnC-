using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class HelloLightCasters : Application
	{
		public HelloLightCasters() : base("HelloLightCasters")
		{
		}

        public override void Start(Window wnd)
		{
			base.Start(wnd);

			mCube = new Cube();

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), Camera.YAW, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			mLightShader = new GLProgram(@"Resources/light_casters.vs", @"Resources/light_casters.frag");

			mLocLightModel = mLightShader.GetUniformLocation("model");
			mLocLightView = mLightShader.GetUniformLocation("view");
			mLocLightProjection = mLightShader.GetUniformLocation("projection");

			mLocLightPos = mLightShader.GetUniformLocation("light.position");
			mLocLightSpotdirLoc = mLightShader.GetUniformLocation("light.direction");
			mLocLightSpotCutOffLoc = mLightShader.GetUniformLocation("light.cutOff");
			mLocLightSpotOuterCutOffLoc = mLightShader.GetUniformLocation("light.outerCutOff");

			mLocLightViewPos = mLightShader.GetUniformLocation("viewPos");

			mLocLightAmbient = mLightShader.GetUniformLocation("light.ambient");
			mLocLightDiffuse = mLightShader.GetUniformLocation("light.diffuse");
			mLocLightSpecular = mLightShader.GetUniformLocation("light.specular");

			mLocLightConstant = mLightShader.GetUniformLocation("light.constant");
			mLocLightLinear = mLightShader.GetUniformLocation("light.linear");
			mLocLightQuadratic = mLightShader.GetUniformLocation("light.quadratic");

			//mLocMaterialAmbient = mLightShader.GetUniformLocation("material.ambient");
			mLocMaterialDiffuse = mLightShader.GetUniformLocation("material.diffuse");
			mLocMaterialSpecular = mLightShader.GetUniformLocation("material.specular");
			mLocMaterialShininess = mLightShader.GetUniformLocation("material.shininess");

			//
			mLampShader = new GLProgram(@"Resources/lamp.vs", @"Resources/lamp.frag");
			mLocLampModel = mLampShader.GetUniformLocation("model");
			mLocLampView = mLampShader.GetUniformLocation("view");
			mLocLampProjection = mLampShader.GetUniformLocation("projection");

			//
			mDiffuseMap = new GLTexture2D(@"Resources/Texture/container2.png");
			mSpecularMap = new GLTexture2D(@"Resources/Texture/container2_specular.png");
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Enable(EnableCap.DepthTest);

            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom), (float)wnd.Width / (float)wnd.Height, 0.1f, 100.0f);
			var view = mCamera.GetViewMatrix();

			//

			do
			{
				mLightShader.Use();

				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D, mDiffuseMap.getTextureId());
				GL.Uniform1(mLightShader.GetUniformLocation("material.diffuse"), 0);

				GL.ActiveTexture(TextureUnit.Texture1);
				GL.BindTexture(TextureTarget.Texture2D, mSpecularMap.getTextureId());
				GL.Uniform1(mLightShader.GetUniformLocation("material.specular"), 1);

				GL.UniformMatrix4(mLocLightProjection, false, ref projection);
				GL.UniformMatrix4(mLocLightView, false, ref view);

				GL.Uniform3(mLightShader.GetUniformLocation("light.position"), mCamera.Position.X, mCamera.Position.Y, mCamera.Position.Z);
				GL.Uniform3(mLightShader.GetUniformLocation("viewPos"), mCamera.Position.X, mCamera.Position.Y, mCamera.Position.Z);

               	GL.Uniform3(mLightShader.GetUniformLocation("light.direction"), mCamera.Front.X, mCamera.Front.Y, mCamera.Front.Z);
				GL.Uniform1(mLightShader.GetUniformLocation("light.cutOff"), Math.Cos(MathHelper.DegreesToRadians(12.5f)));
                GL.Uniform1(mLightShader.GetUniformLocation("light.outerCutOff"), Math.Cos(MathHelper.DegreesToRadians(17.5f)));

				GL.Uniform3(mLightShader.GetUniformLocation("light.ambient"), 0.1f, 0.1f, 0.1f);
				GL.Uniform3(mLightShader.GetUniformLocation("light.diffuse"), 0.8f, 0.8f, 0.8f);
				GL.Uniform3(mLightShader.GetUniformLocation("light.specular"), 1.0f, 1.0f, 1.0f);

                //GL.Uniform3(mLocLightAmbient, 1.0f, 0.5f, 0.31f);
				//GL.Uniform3(mLocMaterialDiffuse, 1.0f, 0.5f, 0.31f);
				//GL.Uniform3(mLocMaterialSpecular, 0.5f, 0.5f, 0.5f); // Specular doesn't have full effect on this object's material
				GL.Uniform1(mLightShader.GetUniformLocation("light.constant"), 1.0f);
				GL.Uniform1(mLightShader.GetUniformLocation("light.linear"), 0.09f);
				GL.Uniform1(mLightShader.GetUniformLocation("light.quadratic"), 0.032f);
				GL.Uniform1(mLightShader.GetUniformLocation("material.shininess"), 32.0f);

				Vector3 axis = new Vector3(1.0f, 0.3f, 0.5f);

				for (int i = 0; i < mCubePositions.Length; ++i)
				{ 
					Matrix4 model = Matrix4.CreateTranslation(mCubePositions[i]);
					//model = glm::translate(model, cubePositions[i]);
					float angle = 20.0f * i;
					model = Matrix4.CreateScale(0.5f) * model;
					model = Matrix4.CreateFromAxisAngle(axis, angle) * model;
					GL.UniformMatrix4(mLocLightModel, false, ref model);
					mCube.Draw();
				}
			}
			while (false);

            do
            {
				mLampShader.Use();

				GL.UniformMatrix4(mLocLampProjection, false, ref projection);
				GL.UniformMatrix4(mLocLampView, false, ref view);

                Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);
				Matrix4 model = Matrix4.CreateTranslation(lightPos.X, lightPos.Y, lightPos.Z);
				model = Matrix4.CreateScale(0.2f) * model;
				GL.UniformMatrix4(mLocLampModel, false, ref model);

				mCube.Draw();

            }
            while (false);
		}

        public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
            base.OnKeyUp(e);
    //        if (e.Key == OpenTK.Input.Key.Plus)
    //        {
    //            mSpotCutOffScale += 0.5f;
    //            mSpotOuterCutOffScale += 0.5f;
    //        }
    //        else if (e.Key == OpenTK.Input.Key.Minus)
    //        {
    //            mSpotCutOffScale -= 0.5f;
    //            mSpotCutOffScale = mSpotCutOffScale < 0.0f ? 0.0f : mSpotCutOffScale;

				//mSpotOuterCutOffScale -= 0.5f;
				//mSpotOuterCutOffScale = mSpotOuterCutOffScale < 0.0f ? 0.0f : mSpotOuterCutOffScale;
            //}
		}

		private Cube mCube = null;
		private Camera mCamera = null;

		//
		private GLProgram mLightShader = null;

		private int mLocLightModel = -1;
		private int mLocLightView = -1;
		private int mLocLightProjection = -1;

		private int mLocLightPos = -1;
		private int mLocLightSpotdirLoc = -1;
		private int mLocLightSpotCutOffLoc = -1;
		private int mLocLightSpotOuterCutOffLoc = -1;

		private int mLocLightViewPos = -1;

		private int mLocLightAmbient = -1;
		private int mLocLightDiffuse = -1;
		private int mLocLightSpecular = -1;

		private int mLocLightConstant = -1;
		private int mLocLightLinear = -1;
		private int mLocLightQuadratic = -1;

		//private int mLocMaterialAmbient = -1;
		private int mLocMaterialDiffuse = -1;
		private int mLocMaterialSpecular = -1;
		private int mLocMaterialShininess = -1;

          
		//
		private GLProgram mLampShader = null;
		private int mLocLampModel = -1;
		private int mLocLampView = -1;
		private int mLocLampProjection = -1;	

		//
		Vector3[] mCubePositions = {
		new Vector3( 0.0f,  0.0f,  0.0f),
		new Vector3( 2.0f,  5.0f, -15.0f),
		new Vector3(-1.5f, -2.2f, -2.5f),
		new Vector3(-3.8f, -2.0f, -12.3f),
		new Vector3( 2.4f, -0.4f, -3.5f),
		new Vector3(-1.7f,  3.0f, -7.5f),
		new Vector3( 1.3f, -2.0f, -2.5f),
		new Vector3( 1.5f,  2.0f, -2.5f),
		new Vector3( 1.5f,  0.2f, -1.5f),
		new Vector3(-1.3f,  1.0f, -1.5f) 
		};

		private GLTexture2D mDiffuseMap = null;
		private GLTexture2D mSpecularMap = null;

  //      private float mSpotCutOffScale = 1.0f;
		//private float mSpotOuterCutOffScale = 1.0f;
	}
}

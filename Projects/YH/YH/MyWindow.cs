using System;
using OpenTK;
using OpenTK.Graphics;

namespace YH
{
	//=============================================================================================
	public class MyWindow : OpenTK.GameWindow
	{
		public MyWindow(int w, int h) : base
		(
		w, // initial width
		h, // initial height
		GraphicsMode.Default,
		"OpenTK.GameWindow",  // initial title
		GameWindowFlags.Default,
		DisplayDevice.Default,
		4, // OpenGL major version
		0, // OpenGL minor version
		GraphicsContextFlags.ForwardCompatible)
		{
			//mCurrentApplication = new HelloTriangle();
			//mCurrentApplication = new HelloTexture2D();
			//mCurrentApplication = new HelloTransform();
			//mCurrentApplication = new HelloCoordinateSystem();
			//mCurrentApplication = new HelloCamera();
			//mCurrentApplication = new HelloColorScene();
			//mCurrentApplication = new HelloBasicLightSpecular();
			//mCurrentApplication = new HelloMaterials();
			mCurrentApplication = new HelloLightCasters();

			Title = mCurrentApplication.mAppName;
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			if (!mCurrentApplication.isStarted())
			{
				mCurrentApplication.Start();
			}

			mCurrentApplication.Update(e.Time);
			mCurrentApplication.Draw(e.Time, base.Width, base.Height);

            SwapBuffers();
		}

		protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (mCurrentApplication != null)
			{
				mCurrentApplication.OnKeyDown(e);
			}
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);

			if (mCurrentApplication != null)
			{
				mCurrentApplication.OnKeyPress(e);
			}
		}

		protected override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (mCurrentApplication != null)
			{
				mCurrentApplication.OnKeyUp(e);
			}
		}

		protected override void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
		{
			base.OnMouseMove(e);

			if (mCurrentApplication != null)
			{
				mCurrentApplication.OnMouseMove(e);
			}
		}

		private Application mCurrentApplication;
	}

	 
}


	 


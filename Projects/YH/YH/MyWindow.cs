
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
// new HelloCamera()
			mCurrentApplication = new HelloCamera();//new HelloCoordinateSystem();//new HelloTransform();//new HelloTexture2D();//new HelloTriangle();
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

		private Application mCurrentApplication;
	}

	 
}


	 


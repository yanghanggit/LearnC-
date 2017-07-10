using OpenTK.Graphics.OpenGL;
using System.Drawing;
using SharpFont;

namespace YH
{
	public class HelloFont : Application
	{
		public HelloFont() : base("HelloFont")
		{
            
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

            GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.ClearColor(Color.Black);

            mLib = new Library();
            var face = mLib.NewFace(@"Resources/Font/test.ttf", 0);


            //int a = 0;
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
		}


        Library mLib = null;
	}
}


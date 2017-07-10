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
            face.SetCharSize(0, 46, 0, 96);

            face.LoadChar('a', LoadFlags.Render, LoadTarget.Mono);
            var w = face.Glyph.Bitmap.Width;
            var h = face.Glyph.Bitmap.Rows;
            var buffer = face.Glyph.Bitmap.Buffer;
               // FontFace.SetCharSize(0, size, 0, 96);
            int a = 0;

            mLib.Dispose();
            face.Dispose();
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



using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using System.Collections.Generic;
using SharpFont;

namespace YH
{
	class Character
	{
		public int TextureID = 0;   
        public Size Size = new Size(0, 0);    
        public Vector2 Bearing = new Vector2(0, 0); 
		public int Advance = 0;
	};

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
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

			shader = new GLProgram(@"Resources/text.vs", @"Resources/text.frag");

			const float near_plane = 0.1f;
			const float far_plane = 1000.0f;
            Matrix4 projection = Matrix4.CreateOrthographic(wnd.Width, wnd.Height, near_plane, far_plane);
			shader.Use();
			GL.UniformMatrix4(shader.GetUniformLocation("projection"), false, ref projection);

			var library = new Library();

            var face = new Face(library, @"Resources/Font/test.ttf");
            face.SetPixelSizes(46, 46);

            for (uint c = 0; c < 128; ++c)
			{
                face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
                int texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexImage2D(TextureTarget.Texture2D,
                              0,
                              PixelInternalFormat.R8,
                              face.Glyph.Bitmap.Width,
                              face.Glyph.Bitmap.Rows,
                              0,
                              PixelFormat.Red, 
                              PixelType.UnsignedByte,
                              face.Glyph.Bitmap.Buffer);

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				
                Character character = new Character();
                character.TextureID = texture;
                character.Size = new Size(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows);
                character.Bearing = new Vector2(face.Glyph.BitmapLeft, face.Glyph.BitmapTop);
                character.Advance = face.Glyph.Advance.X.ToInt32();
                mCharacters.Add(c, character);
			}

            GL.BindTexture(TextureTarget.Texture2D, 0);

            face.Dispose();
			library.Dispose();

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0);
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

			RenderText(shader, "This is sample text", 0.0f, 0.0f, 1.0f, new Vector3(0.5f, 0.8f, 0.2f));
			RenderText(shader, "(C) LearnOpenGL.com", 540.0f, 570.0f, 0.5f, new Vector3(0.3f, 0.7f, 0.9f));
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

        public void RenderText(GLProgram sh, string text, float x, float y, float scale, Vector3 color)
		{
			sh.Use();
            GL.Uniform3(sh.GetUniformLocation("textColor"), color.X, color.Y, color.Z);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(VAO);

            foreach (var c in text)
			{
                if (!mCharacters.ContainsKey(c))
                {
                    continue;
                }

                Character ch = mCharacters[c];
                float xpos = x + ch.Bearing.X * scale;
                float ypos = y - (ch.Size.Height - ch.Bearing.Y) * scale;
                float w = ch.Size.Width * scale;
                float h = ch.Size.Height * scale;
			
				float[] vertices = {
                     xpos,     ypos + h,   0.0f, 0.0f,
                     xpos,     ypos,       0.0f, 1.0f,
                     xpos + w, ypos,       1.0f, 1.0f,

                     xpos,     ypos + h,   0.0f, 0.0f,
                     xpos + w, ypos,       1.0f, 1.0f,
                     xpos + w, ypos + h,   1.0f, 0.0f
				};

                GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * vertices.Length, vertices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                var adv = (ch.Advance * scale);
                x += adv;
            }

            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
		}

        private Dictionary<uint, Character> mCharacters = new Dictionary<uint, Character>();
        private int VAO = 0, VBO = 0;
        private GLProgram shader = null;
	}
}


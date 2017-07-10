﻿﻿//using OpenTK.Graphics.OpenGL;
//using System.Drawing;


using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using System.Collections.Generic;
using SharpFont;


namespace YH
{
	/// Holds all state information relevant to a character as loaded using FreeType
	class Character
	{
		public int TextureID = 0;   // ID handle of the glyph texture
        public Size Size = new Size(0, 0);    // Size of glyph
        public Vector2 Bearing = new Vector2(0, 0);  // Offset from baseline to left/top of glyph
		public int Advance = 0;    // Horizontal offset to advance to next glyph
	};

	public class HelloFont : Application
	{
		public HelloFont() : base("HelloFont")
		{
            
		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

            mQuad = new Quad();

			mScreenShader = new GLProgram(@"Resources/framebuffers_screen.vs", @"Resources/framebuffers_screen.frag");

			GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.ClearColor(Color.Black);
            GL.Disable(EnableCap.DepthTest);

			// Set OpenGL options
			//glEnable(GL_CULL_FACE);
            GL.Enable(EnableCap.CullFace);
			//glEnable(GL_BLEND);
            GL.Enable(EnableCap.Blend);
			//glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			// Disable byte-alignment restriction
			//glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);


			// Compile and setup the shader
			//Shader shader("shaders/text.vs", "shaders/text.frag");
			shader = new GLProgram(@"Resources/text.vs", @"Resources/text.frag");

			const float near_plane = 0.1f;
			const float far_plane = 1000.0f;
            Matrix4 projection = Matrix4.CreateOrthographic(wnd.Width, wnd.Height, near_plane, far_plane);//glm::ortho(0.0f, static_cast<GLfloat>(WIDTH), 0.0f, static_cast<GLfloat>(HEIGHT));
			shader.Use();
			//glUniformMatrix4fv(glGetUniformLocation(shader.Program, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
			GL.UniformMatrix4(shader.GetUniformLocation("projection"), false, ref projection);

			mLibrary = new Library();

            //var face = mLibrary.NewFace(@"Resources/Font/test.ttf", 0);

            var face = new Face(mLibrary, @"Resources/Font/test.ttf");
            //face.SetCharSize(46, 46, 96, 96);
            face.SetPixelSizes(32, 32);

			//face.LoadChar('a', LoadFlags.Render, LoadTarget.Mono);
			//var w = face.Glyph.Bitmap.Width;
			//var h = face.Glyph.Bitmap.Rows;
			//var buffer = face.Glyph.Bitmap.Buffer;
			// FontFace.SetCharSize(0, size, 0, 96);
			//int a = 0;

			// Load first 128 characters of ASCII set
            for (uint c = 0; c < 128; c++)
			{
                //var c = testChar;
                //face.LoadChar();
                //face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
                face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
                //face.Glyph.RenderGlyph(RenderMode.Normal);
                if (face.Glyph.Bitmap.Width <= 0)
                {
                    continue;
                }




				// Load character glyph 
				//if (FT_Load_Char(face, c, FT_LOAD_RENDER))
				//{
				//	std::cout << "ERROR::FREETYTPE: Failed to load Glyph" << std::endl;
				//	continue;
				//}
				// Generate texture
                int texture = GL.GenTexture();
				//glGenTextures(1, &texture);
				//glBindTexture(GL_TEXTURE_2D, texture);
                GL.BindTexture(TextureTarget.Texture2D, texture);
                //glTexImage2D(
                //	GL_TEXTURE_2D,
                //	0,
                //	GL_RED,
                //	face->glyph->bitmap.width,
                //	face->glyph->bitmap.rows,
                //	0,
                //	GL_RED,
                //	GL_UNSIGNED_BYTE,
                //	face->glyph->bitmap.buffer
                //);
                GL.TexImage2D(TextureTarget.Texture2D,
                              0,
                              PixelInternalFormat.R8,
                              face.Glyph.Bitmap.Width,
                              face.Glyph.Bitmap.Rows,
                              0,
                              PixelFormat.Red, 
                              PixelType.UnsignedByte,
                              face.Glyph.Bitmap.Buffer);
                
				// Set texture options
				//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
				//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
				//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
				//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				// Now store character for later use
				//Character character = {
				//	texture,
				//	glm::ivec2(face->glyph->bitmap.width, face->glyph->bitmap.rows),
				//	glm::ivec2(face->glyph->bitmap_left, face->glyph->bitmap_top),
				//	face->glyph->advance.x
				//};
				//Characters.insert(std::pair<GLchar, Character>(c, character));
                Character character = new Character();
                character.TextureID = texture;
                character.Size = new Size(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows);
                character.Bearing = new Vector2(face.Glyph.BitmapLeft, face.Glyph.BitmapTop);
                character.Advance = (int)face.Glyph.Advance.X;
                mCharacters.Add(c, character);
			}
			//glBindTexture(GL_TEXTURE_2D, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            face.Dispose();
			mLibrary.Dispose();

            // Configure VAO/VBO for texture quads
            //glGenVertexArrays(1, &VAO);
            VAO = GL.GenVertexArray();
            //glGenBuffers(1, &VBO);
            VBO = GL.GenBuffer();
            //glBindVertexArray(VAO);
            GL.BindVertexArray(VAO);
            //glBindBuffer(GL_ARRAY_BUFFER, VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            //glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * 6 * 4, NULL, GL_DYNAMIC_DRAW);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            //glEnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(0);
			//glVertexAttribPointer(0, 4, GL_FLOAT, GL_FALSE, 4 * sizeof(GLfloat), 0);
			GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
			//glBindBuffer(GL_ARRAY_BUFFER, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			//glBindVertexArray(0);
			GL.BindVertexArray(0);
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);
			// Clear the colorbuffer
			//glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			//glClear(GL_COLOR_BUFFER_BIT);
            //RenderText(shader, "This is sample text", 25.0f, 25.0f, 1.0f, new Vector3(0.5f, 0.8f, 0.2f));
			//RenderText(shader, "(C) LearnOpenGL.com", 540.0f, 570.0f, 0.5f, new Vector3(0.3f, 0.7f, 0.9f));
			//RenderText(shader, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", 0.5f, 0.5f, 1.0f, new Vector3(1.0f, 1.0f, 1.0f));


            var ch = mCharacters['A'];
			mScreenShader.Use();
			GL.Uniform1(mScreenShader.GetUniformLocation("post_processing"), 0);
            GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);
			mQuad.Draw();
			GL.BindTexture(TextureTarget.Texture2D, 0);
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

        public void RenderText(GLProgram sh, string text, float x, float y, float scale, Vector3 color)
		{
			// Activate corresponding render state  
			sh.Use();
			//glUniform3f(glGetUniformLocation(shader.Program, "textColor"), color.x, color.y, color.z);
            GL.Uniform3(sh.GetUniformLocation("textColor"), color.X, color.Y, color.Z);
			//glActiveTexture(GL_TEXTURE0);
            GL.ActiveTexture(TextureUnit.Texture0);
			//glBindVertexArray(VAO);
            GL.BindVertexArray(VAO);

            // Iterate through all characters
            //std::string::const_iterator c;
            //for (c = text.begin(); c != text.end(); c++)
            foreach (var c in text)
			{
                Character ch = mCharacters[c];//Characters[c];
                float xpos = x + ch.Bearing.X * scale;
                float ypos = y - (ch.Size.Height - ch.Bearing.Y) * scale;
                float w = ch.Size.Width * scale;
                float h = ch.Size.Height * scale;
				// Update VBO for each character
				
				//GLfloat vertices[6][4] = {
		        //    { xpos,     ypos + h,   0.0, 0.0 },            
		        //    { xpos,     ypos,       0.0, 1.0 },
		        //    { xpos + w, ypos,       1.0, 1.0 },

		        //    { xpos,     ypos + h,   0.0, 0.0 },
		        //    { xpos + w, ypos,       1.0, 1.0 },
		        //    { xpos + w, ypos + h,   1.0, 0.0 }           
		        //};
                float[] vertices = {
                     xpos,     ypos + h,   0.0f, 0.0f,
                     xpos,     ypos,       0.0f, 1.0f,
                     xpos + w, ypos,       1.0f, 1.0f,

                     xpos,     ypos + h,   0.0f, 0.0f,
                     xpos + w, ypos,       1.0f, 1.0f,
                     xpos + w, ypos + h,   1.0f, 0.0f
				};

				// Render glyph texture over quad
				//glBindTexture(GL_TEXTURE_2D, ch.TextureID);
                GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);
				// Update content of VBO memory
				//glBindBuffer(GL_ARRAY_BUFFER, VBO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                //glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(vertices), vertices); // Be sure to use glBufferSubData and not glBufferData
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * vertices.Length, vertices);
				//glBindBuffer(GL_ARRAY_BUFFER, 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				// Render quad
				//glDrawArrays(GL_TRIANGLES, 0, 6);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
				// Now advance cursors for next glyph (note that advance is number of 1/64 pixels)
				x += (ch.Advance >> 6) * scale; // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))
			}
            //glBindVertexArray(0);
            GL.BindVertexArray(0);
			//glBindTexture(GL_TEXTURE_2D, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
		}

        private Library mLibrary = null;
        private Dictionary<uint, Character> mCharacters = new Dictionary<uint, Character>();
        private int VAO = 0, VBO = 0;
        private GLProgram shader = null;
        private Quad mQuad = null;
        private GLProgram mScreenShader = null;
        //private char testChar = 'A';
	}
}


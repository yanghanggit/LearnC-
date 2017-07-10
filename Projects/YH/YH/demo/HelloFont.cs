﻿//using OpenTK.Graphics.OpenGL;
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

            GL.Viewport(0, 0, wnd.Width, wnd.Height);
            GL.ClearColor(Color.Blue);

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
			const float far_plane = 7.5f;
            Matrix4 projection = Matrix4.CreateOrthographic(wnd.Width, wnd.Height, near_plane, far_plane);//glm::ortho(0.0f, static_cast<GLfloat>(WIDTH), 0.0f, static_cast<GLfloat>(HEIGHT));
			shader.Use();
			//glUniformMatrix4fv(glGetUniformLocation(shader.Program, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
			GL.UniformMatrix4(shader.GetUniformLocation("projection"), false, ref projection);

			mLibrary = new Library();

            var face = mLibrary.NewFace(@"Resources/Font/test.ttf", 0);
            face.SetCharSize(0, 46, 0, 96);

			//face.LoadChar('a', LoadFlags.Render, LoadTarget.Mono);
			//var w = face.Glyph.Bitmap.Width;
			//var h = face.Glyph.Bitmap.Rows;
			//var buffer = face.Glyph.Bitmap.Buffer;
			// FontFace.SetCharSize(0, size, 0, 96);
			//int a = 0;

			// Load first 128 characters of ASCII set
            for (uint c = 0; c < 128; c++)
			{
                face.LoadChar(c, LoadFlags.Render, LoadTarget.Mono);
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
                mCharacters.Add((char)c, character);
			}
			//glBindTexture(GL_TEXTURE_2D, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            face.Dispose();
			mLibrary.Dispose();



			// Configure VAO/VBO for texture quads
			//glGenVertexArrays(1, &VAO);
			//glGenBuffers(1, &VBO);
			//glBindVertexArray(VAO);
			//glBindBuffer(GL_ARRAY_BUFFER, VBO);
			//glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * 6 * 4, NULL, GL_DYNAMIC_DRAW);
			//glEnableVertexAttribArray(0);
			//glVertexAttribPointer(0, 4, GL_FLOAT, GL_FALSE, 4 * sizeof(GLfloat), 0);
			//glBindBuffer(GL_ARRAY_BUFFER, 0);
			//glBindVertexArray(0);

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

        private Library mLibrary = null;
        private Dictionary<char, Character> mCharacters = new Dictionary<char, Character>();
        private int VAO = 0, VBO = 0;
        private GLProgram shader = null;
	}
}


﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace YH
{
	//=============================================================================================
	public class HelloTexture2D : Application
	{
		public HelloTexture2D() : base("HelloTexture2D")
		{

		}

		public override void Start()
		{
			base.Start(); 

			mProgram = new GLProgram(@"Resources/texture.vert", @"Resources/texture.frag");
			mSimpleRectangle = new SimpleTextureRectangle();
			mTexture1 = new GLTexture2D(@"Resources/Texture/wood.png");
			mTexture2 = new GLTexture2D(@"Resources/Texture/awesomeface.png");
		}

		public override void Update()
		{

		}

		public override void Draw(int w, int h)
		{
			GL.Viewport(0, 0, w, h);
			GL.ClearColor(Color.Gray);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, mTexture1.getTextureId());
			GL.Uniform1(mProgram.GetUniformLocation("ourTexture1"), 0);

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, mTexture2.getTextureId());
			GL.Uniform1(mProgram.GetUniformLocation("ourTexture2"), 1);

			mProgram.Use();
			mSimpleRectangle.Draw();
		}

		private GLProgram mProgram = null;
		private SimpleTextureRectangle mSimpleRectangle = null;
		private GLTexture2D mTexture1 = null;
		private GLTexture2D mTexture2 = null;
	}
}

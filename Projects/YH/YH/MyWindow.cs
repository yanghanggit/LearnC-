using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace YH
{
	//=============================================================================================
	public class SimpleGeometry
	{
		public SimpleGeometry(string name)
		{
			mName = name;
		}

		public virtual void Draw()
		{ 
		
		}

		public string mName = "SimpleGeometry";
	}
	//=============================================================================================
	public class SimpleRectangle : SimpleGeometry
	{
		public SimpleRectangle() : base("SimpleRectangle")
		{
			
		}

		public override void Draw()
		{
			if (mVAO <= 0)
			{
				build();
			}

			if (mVAO > 0)
			{ 
				GL.BindVertexArray(mVAO);
				GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
				GL.BindVertexArray(0);
			}
		}

		private void build()
		{ 
			float[] vertices =
			{
				 0.5f,  0.5f, 0.0f,  // Top Right
			     0.5f, -0.5f, 0.0f,  // Bottom Right
			    -0.5f, -0.5f, 0.0f,  // Bottom Left
			    -0.5f,  0.5f, 0.0f   // Top Left 
			};

			int[] indices =
			{  // Note that we start from 0!
			    0, 1, 3,  // First Triangle
			    1, 2, 3   // Second Triangle
			};

			mVAO = GL.GenVertexArray();
			mVBO = GL.GenBuffer();
			mEBO = GL.GenBuffer();

			//
			GL.BindVertexArray(mVAO);
			GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, mEBO);
			GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticDraw);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
			GL.EnableVertexAttribArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0);
		}

		private int mVAO = 0;
		private int mVBO = 0;
		private int mEBO = 0;
	}
	//=============================================================================================
	public class GLShader
	{
		public GLShader(ShaderType shaderType, string path)
		{
			mShaderId = GL.CreateShader(shaderType);
			string shaderSource = System.IO.File.ReadAllText(path);
			GL.ShaderSource(mShaderId, shaderSource);
			GL.CompileShader(mShaderId);

			string error = GL.GetShaderInfoLog(mShaderId);
			if (error.Length > 0)
			{
				mIsValid = false;
				Console.WriteLine(error);
			}
			else
			{
				mIsValid = true;
			}
		}

		public int getShaderId()
		{
			return mShaderId;
		}

		public bool isValid()
		{
			return mIsValid;
		}

		private int mShaderId = 0;
		private bool mIsValid = false;
	}
	//=============================================================================================
	public class GLProgram
	{
		public GLProgram(string vertPath, string fragPath)
		{
			mVertexShader = new GLShader(ShaderType.VertexShader, vertPath);
			mFragmentShader = new GLShader(ShaderType.FragmentShader, fragPath);

			mProgram = GL.CreateProgram();
			GL.AttachShader(mProgram, mVertexShader.getShaderId());
			GL.AttachShader(mProgram, mFragmentShader.getShaderId());
			GL.LinkProgram(mProgram);

			string error = GL.GetProgramInfoLog(mProgram);
			if (error.Length > 0)
			{
				mIsValid = false;
				Console.WriteLine(error);
			}
			else
			{
				mIsValid = true;
			}
		}

		public void Use()
		{
			GL.UseProgram(mProgram);
		}

		public int GetUniformLocation(string name)
		{
			return GL.GetUniformLocation(mProgram, name);
		}

		public bool isValid()
		{
			return mIsValid;
		}

		private int mProgram = 0;
		private GLShader mVertexShader = null;
		private GLShader mFragmentShader = null;
		private bool mIsValid = false;
	}
	//=============================================================================================
	public class Application
	{ 
		public Application(string appName)
		{
			mAppName = appName;
		}

		public virtual void Start()
		{
			mStarted = true;		
		}

		public bool isStarted()
		{
			return mStarted;
		}

		public virtual void Update()
		{
			
		}

		public virtual void Draw(int w, int h)
		{

		}

		private bool mStarted = false;
		public readonly string mAppName = "Application";
	}
	//=============================================================================================
	public class HelloTriangle : Application
	{
		public HelloTriangle() : base("HelloTriangle")
		{

		}

		public override void Start()
		{
			base.Start();
			mProgram = new GLProgram(@"Resources/testshader.vert", @"Resources/testshader.frag");
			mSimpleRectangle = new SimpleRectangle();
		}

		public override void Update()
		{
			
		}

		public override void Draw(int w, int h)
		{
			GL.Viewport(0, 0, w, h);
			GL.ClearColor(Color.Blue);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			mProgram.Use();
			mSimpleRectangle.Draw();
		}

		private GLProgram mProgram = null;
		private SimpleRectangle mSimpleRectangle = null;
	}
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
			mCurrentApplication = new HelloTriangle();
			Title = mCurrentApplication.mAppName;
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			if (!mCurrentApplication.isStarted())
			{
				mCurrentApplication.Start();
			}

			mCurrentApplication.Update();
			mCurrentApplication.Draw(base.Width, base.Height);

            SwapBuffers();
		}

		private Application mCurrentApplication;
	}

	 
}


	 


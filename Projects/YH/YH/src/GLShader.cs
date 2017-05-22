using System;
using OpenTK.Graphics.OpenGL;

namespace YH
{
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
}

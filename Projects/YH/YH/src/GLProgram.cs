using System;
using OpenTK.Graphics.OpenGL;

namespace YH
{
	public class GLProgram
	{
        public GLProgram(string vertPath, string fragPath, string geomPath = "")
		{
			mVertexShader = new GLShader(ShaderType.VertexShader, vertPath);
			mFragmentShader = new GLShader(ShaderType.FragmentShader, fragPath);

            if (geomPath.Length > 0)
            {
                mGeometryShader = new GLShader(ShaderType.GeometryShader, geomPath);        
            }   

			mProgram = GL.CreateProgram();
			GL.AttachShader(mProgram, mVertexShader.getShaderId());
			GL.AttachShader(mProgram, mFragmentShader.getShaderId());

            if (mGeometryShader != null)
            {
                GL.AttachShader(mProgram, mGeometryShader.getShaderId());
            }

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

		public bool IsValid()
		{
			return mIsValid;
		}

        public int mProgram = 0;
		private GLShader mVertexShader = null;
		private GLShader mFragmentShader = null;
        private GLShader mGeometryShader = null;
		private bool mIsValid = false;	
	}
}

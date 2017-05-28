using System;
using OpenTK.Input;

namespace YH
{
	public class InputSystem
	{
		public InputSystem()
		{
		}

		public void Update()
		{ 
			if (OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.W))
			{
				Console.WriteLine("W");
				//transform.position += 0.1f * transform.right;
			}

			if(OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.S))
			{
				Console.WriteLine("S");
				//transform.position -= 0.1f * transform.right;
			}

			if (OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.A))
			{
				Console.WriteLine("A");
				//transform.position += 0.1f * transform.up;
			}

			if(OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.D))
			{
				Console.WriteLine("D");
				//transform.position -= 0.1f * transform.up;			
			}

			//if (OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.J))
			//{
			//	transform.rotation = (Matrix4.CreateRotationX(10.0f * Time.DeltaTime) * Matrix4.CreateFromQuaternion(transform.rotation)).ExtractRotation();
			//}

			//if (OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.K))
			//{
			//	transform.rotation = (Matrix4.CreateRotationY(10.0f * Time.DeltaTime) * Matrix4.CreateFromQuaternion(transform.rotation)).ExtractRotation();
			//}
		}
	}
}

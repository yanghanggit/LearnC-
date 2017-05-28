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
			KeyboardState state = OpenTK.Input.Keyboard.GetState();
			if (!state.IsAnyKeyDown)
			{
				return;
			}

			if (state.IsKeyDown(Key.W))
			{
				Console.WriteLine("Key.W");
			}

			if(state.IsKeyDown(Key.S))
			{
				Console.WriteLine("Key.S");
			}

			if (state.IsKeyDown(Key.A))
			{
				Console.WriteLine("Key.A");
			}

			if(state.IsKeyDown(Key.D))
			{
				Console.WriteLine("Key.D");
			}
		}
	}
}

using System;
using OpenTK.Input;
using System.Collections.Generic;

namespace YH
{
	public class Keyboard
	{
		public delegate void ProcessDelegate(Key k);

		public Keyboard(string name)
		{
			mName = name;

			mRegisterKeys.Add(Key.W);
			mRegisterKeys.Add(Key.S);
			mRegisterKeys.Add(Key.A);
			mRegisterKeys.Add(Key.D);
		}

		public void Capture()
		{
			KeyboardState state = OpenTK.Input.Keyboard.GetState();
			if (state.IsAnyKeyDown)
			{
				foreach (var item in mRegisterKeys)
				{
					if (state.IsKeyDown(item))
					{
						KeyDownEvent(item);
						mKeyDown.Add(item);
					}
				}
			}

			mKeyDown.RemoveWhere(delegate(Key k)
			{
				if (state.IsKeyUp(k))
				{
					KeyUpEvent(k);
					return true;
				}
				return false;
			});
		}

		private void KeyDownEvent(Key k)
		{
			Console.WriteLine("KeyDownEvent: " + k.ToString());

			if (!mKeyDownEvents.ContainsKey(k))
			{
				return;
			}

			mKeyDownEvents[k](k);
		}

		private void KeyUpEvent(Key k)
		{ 
			Console.WriteLine("KeyUpEvent: " + k.ToString());

			if (!mKeyUpEvents.ContainsKey(k))
			{
				return;
			}

			mKeyUpEvents[k](k);
		}

		private Dictionary<Key, ProcessDelegate> mKeyDownEvents = new Dictionary<Key, ProcessDelegate>();
		private Dictionary<Key, ProcessDelegate> mKeyUpEvents = new Dictionary<Key, ProcessDelegate>();
		private List<Key> mRegisterKeys = new List<Key>();
		private HashSet<Key> mKeyDown = new HashSet<Key>();
		public readonly string mName;
	}
}

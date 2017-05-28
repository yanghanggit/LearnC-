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
		}

		public void RegisterKeyEvent(Key k, ProcessDelegate down, ProcessDelegate up)
		{
			RegisterKeyDown(k, down);
            RegisterKeyUp(k, up);
		}

		public void RegisterKeyDown(Key k, ProcessDelegate del)
		{
			if (del != null)
			{
				mKeyDownEvents[k] = del;
			}
		}

		public void RegisterKeyUp(Key k, ProcessDelegate del)
		{
			if (del != null)
			{
				mKeyUpEvents[k] = del;
			}
		}

		public void Capture()
		{
			KeyboardState state = OpenTK.Input.Keyboard.GetState();
			if (state.IsAnyKeyDown)
			{
				foreach (var item in mKeyDownEvents)
				{
					if (state.IsKeyDown(item.Key))
					{
						//Console.WriteLine("KeyDownEvent: " + item.Key.ToString());
						item.Value(item.Key);
						mKeyDown.Add(item.Key);
					}
				}
			}

			mKeyDown.RemoveWhere(delegate(Key k)
			{
				if (state.IsKeyUp(k))
				{
					//Console.WriteLine("KeyUpEvent: " + k.ToString());
					if (mKeyUpEvents.ContainsKey(k))
					{
						mKeyUpEvents[k](k);
						return true;
					}
				}
				return false;
			});
		}

		private Dictionary<Key, ProcessDelegate> mKeyDownEvents = new Dictionary<Key, ProcessDelegate>();
		private Dictionary<Key, ProcessDelegate> mKeyUpEvents = new Dictionary<Key, ProcessDelegate>();
		private HashSet<Key> mKeyDown = new HashSet<Key>();
		public readonly string mName;
	}
}

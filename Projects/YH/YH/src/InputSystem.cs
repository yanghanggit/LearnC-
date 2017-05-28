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

			//RegisterKeyEvent(Key.W, this.test, this.test);
			//RegisterKeyEvent(Key.S, this.test, this.test);
			//RegisterKeyEvent(Key.A, this.test, this.test);
			//RegisterKeyEvent(Key.D, this.test, this.test);
		}

		//void test(Key k)
		//{ 
		//	Console.WriteLine("test: " + k.ToString());	
		//}

		public void RegisterKeyEvent(Key k, ProcessDelegate down, ProcessDelegate up)
		{
			RegisterKeyDown(k, down);
            RegisterKeyUp(k, up);
		}

		public void RegisterKeyDown(Key k, ProcessDelegate del)
		{
			mKeyDownEvents[k] = del;
		}

		public void RegisterKeyUp(Key k, ProcessDelegate del)
		{
			mKeyUpEvents[k] = del;
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

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
//			var mouseStat = OpenTK.Input.Mouse.GetState();
//mouseStat.
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

	public class Mouse
	{
		public delegate void ProcessDelegate(int offsetX, int offsetY, int curX, int curY);

		public Mouse(string name)
		{
			mName = name;
		}

		public void RegisterMouseMoveEvent(string name, ProcessDelegate del)
		{
			if (del != null)
			{
				mMouseMoveEvents[name] = del;
			}
		}

		public void Capture()
		{
			MouseState state = OpenTK.Input.Mouse.GetState();

			if (mCurrentMouseX < 0)
			{
				mCurrentMouseX = state.X;
			}

			if (mCurrentMouseY < 0)
			{
				mCurrentMouseY = state.Y;
			}

			int offsetX = state.X - mCurrentMouseX;
			int offsetY = state.Y - mCurrentMouseY;
			mCurrentMouseX = state.X;
			mCurrentMouseY = state.Y;

			foreach (var evt in mMouseMoveEvents)
			{
				if (!state.IsAnyButtonDown)
				{
					continue;
				}
				evt.Value(offsetX, offsetY, mCurrentMouseX, mCurrentMouseY);
			}
		}

		private Dictionary<string, ProcessDelegate> mMouseMoveEvents = new Dictionary<string, ProcessDelegate>();
		int mCurrentMouseX = -1;
		int mCurrentMouseY = -1;
		public readonly string mName;	
	}



}

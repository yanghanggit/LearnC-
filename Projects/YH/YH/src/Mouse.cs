using System;
using OpenTK.Input;
using System.Collections.Generic;

namespace YH
{
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

			Console.WriteLine("mouse state = " + state);

			foreach (var evt in mMouseMoveEvents)
			{
				if (!state.IsAnyButtonDown)
				{
					//continue;
				}
				evt.Value(offsetX, offsetY, mCurrentMouseX, mCurrentMouseY);
			}
		}

		private Dictionary<string, ProcessDelegate> mMouseMoveEvents = new Dictionary<string, ProcessDelegate>();
		private int mCurrentMouseX = -1;
		private int mCurrentMouseY = -1;
		public readonly string mName;
	}
}

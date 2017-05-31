﻿
using System;
using OpenTK;
using OpenTK.Graphics;

namespace YH
{
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

		public virtual void Update(double dt)
		{
			mTotalRuningTime += dt;
			if (mCameraController != null)
			{
				mCameraController.Capture(dt);
			}
		}

		public virtual void Draw(double dt, int w, int h)
		{

		}

		public virtual void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			if (mCameraController != null)
			{
				mCameraController.OnKeyDown(e);
			}
		}

		public virtual void OnKeyPress(KeyPressEventArgs e)
		{
			if (mCameraController != null)
			{
				mCameraController.OnKeyPress(e);
			}
		}

		public virtual void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			if (mCameraController != null)
			{
				mCameraController.OnKeyUp(e);
			}
		}

		public virtual void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
		{
			if (mCameraController != null)
			{
				mCameraController.OnMouseMove(e);
			}
		}

		private bool mStarted = false;
		public readonly string mAppName = "Application";
		protected double mTotalRuningTime = 0;
		protected CameraController mCameraController = null;
	}
}

using System;
using OpenTK;
using System.Collections.Generic;

namespace YH
{
	public class CameraController
	{
		public CameraController(string name, Camera cam)
		{
			mName = name;

			mCamera = cam;

			//
			mKeyState.Add(OpenTK.Input.Key.W, false);
			mKeyState.Add(OpenTK.Input.Key.A, false);
			mKeyState.Add(OpenTK.Input.Key.S, false);
			mKeyState.Add(OpenTK.Input.Key.D, false);
			mKeyState.Add(OpenTK.Input.Key.Q, false);
			mKeyState.Add(OpenTK.Input.Key.E, false);
		}

		public void Capture(double dt)
		{
			float fdt = (float)dt;

			mCamera.Yaw   += mMouseOffsetX * fdt;
			mCamera.Pitch -= mMouseOffsetY * fdt;

			if (mConstrainPitch)
			{ 
				// Make sure that when pitch is out of bounds, screen doesn't get flipped
				if (mCamera.Pitch > 89.0f)
				{ 
	                mCamera.Pitch = 89.0f;
				}

				if (mCamera.Pitch < -89.0f)
				{ 
	                mCamera.Pitch = -89.0f;
				}
			}

			if (mKeyState[OpenTK.Input.Key.W])
			{
				MoveForward(fdt);
			}

			if (mKeyState[OpenTK.Input.Key.S])
			{
                MoveBack(fdt);
			}

			if (mKeyState[OpenTK.Input.Key.A])
			{
                MoveLeft(fdt);
			}

			if (mKeyState[OpenTK.Input.Key.D])
			{
                MoveRight(fdt);
			}

			if (mKeyState[OpenTK.Input.Key.Q])
			{
                MoveUp(fdt);
			}

			if (mKeyState[OpenTK.Input.Key.E])
			{
                MoveDown(fdt);
			}

			mMouseOffsetX = 0.0f;
			mMouseOffsetY = 0.0f;

			mCamera.updateCameraVectors();
		}

		private void HandleMove()
		{ 
		
		}

		public Camera GetCamera()
		{
			return mCamera;
		}

		public void OnKeyPress(KeyPressEventArgs e)
		{
			//mKeyState[e.Key] = true;
		}

		public void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			mKeyState[e.Key] = false;
		}

		public void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			mKeyState[e.Key] = true;
		}

		public void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
		{
			if (!e.Mouse.IsAnyButtonDown)
			{
				//return;
			}

			mMouseOffsetX = (float)e.XDelta * mCamera.MouseSensitivity;
			mMouseOffsetY = (float)e.YDelta * mCamera.MouseSensitivity;
		}

		private void MoveForward(float dt)
		{
			Console.WriteLine("MoveForward");
			mCamera.Position += mCamera.Front * mCamera.MovementSpeed * dt;
		}

		private void MoveBack(float dt)
		{
			Console.WriteLine("MoveBack");
			mCamera.Position -= mCamera.Front * mCamera.MovementSpeed * dt;
		}

		private void MoveLeft(float dt)
		{
			Console.WriteLine("MoveLeft");
			mCamera.Position -= mCamera.Right * mCamera.MovementSpeed * dt;
		}

		private void MoveRight(float dt)
		{
			Console.WriteLine("MoveRight");
			mCamera.Position += mCamera.Right * mCamera.MovementSpeed * dt;
		}

		private void MoveUp(float dt)
		{
			Console.WriteLine("MoveUp");
			mCamera.Position += mCamera.Up * mCamera.MovementSpeed * dt;
		}

		private void MoveDown(float dt)
		{
			Console.WriteLine("MoveDown");
			mCamera.Position -= mCamera.Up * mCamera.MovementSpeed * dt;
		}

		public readonly string mName;
		private Camera mCamera = null;
		private bool mConstrainPitch = true;
		private Dictionary<OpenTK.Input.Key, bool> mKeyState = new Dictionary<OpenTK.Input.Key, bool>();
		float mMouseOffsetX = 0.0f;
		float mMouseOffsetY = 0.0f;
	};
}

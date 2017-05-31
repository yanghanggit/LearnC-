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

			//
			mMouse = new Mouse(name + "'s mouse");
			mMouse.RegisterMouseMoveEvent("CameraMouseMove", this.MouseMove);
		}

		public void Capture(double dt)
		{
			float fdt = (float)dt;

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
			
		}

		private void MoveForward(float dt)
		{
			Console.WriteLine("MoveForward");
			mCamera.Position += mCamera.Front * mCamera.MovementSpeed * dt;
			mCamera.updateCameraVectors();
		}

		private void MoveBack(float dt)
		{
			Console.WriteLine("MoveBack");
			mCamera.Position -= mCamera.Front * mCamera.MovementSpeed * dt;
			mCamera.updateCameraVectors();
		}

		private void MoveLeft(float dt)
		{
			Console.WriteLine("MoveLeft");

			mCamera.Position -= mCamera.Right * mCamera.MovementSpeed * dt;
			mCamera.updateCameraVectors();
		}

		private void MoveRight(float dt)
		{
			Console.WriteLine("MoveRight");
			mCamera.Position += mCamera.Right * mCamera.MovementSpeed * dt;
			mCamera.updateCameraVectors();
		}

		private void MoveUp(float dt)
		{
			Console.WriteLine("MoveUp");
			mCamera.Position += mCamera.Up * mCamera.MovementSpeed * dt;
			mCamera.updateCameraVectors();
		}

		private void MoveDown(float dt)
		{
			Console.WriteLine("MoveDown");
			mCamera.Position -= mCamera.Up * mCamera.MovementSpeed * dt;
			mCamera.updateCameraVectors();
		}

		private void MouseMove(int offsetX, int offsetY, int curX, int curY)
		{
			float ox = (float)offsetX * mCamera.MouseSensitivity;
			float oy = (float)offsetY * mCamera.MouseSensitivity;

			mCamera.Yaw   += ox;
			mCamera.Pitch += oy;

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

			mCamera.updateCameraVectors();
		}

		public readonly string mName;
		private Camera mCamera = null;
		//private Keyboard mKeyboard = null;
		//private float mDeltaTime = 0.0f;
		private Mouse mMouse = null;
		private bool mConstrainPitch = false;
		private Dictionary<OpenTK.Input.Key, bool> mKeyState = new Dictionary<OpenTK.Input.Key, bool>();
	};
}

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

			//重置用
			mSaveFront = new Vector3(mCamera.Front);
			mSavePosition = new Vector3(mCamera.Position);
			mSaveWorldUp = new Vector3(mCamera.WorldUp);
			mSaveYaw = mCamera.Yaw;
			mSavePitch = mCamera.Pitch;

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

			bool move = false;
			if (mKeyState[OpenTK.Input.Key.W])
			{
				move = true;
				MoveForward(fdt);
			}

			if (mKeyState[OpenTK.Input.Key.S])
			{
				move = true;
                MoveBack(fdt);
			}

			if (mKeyState[OpenTK.Input.Key.A])
			{
				move = true;
                MoveLeft(fdt);
			}

			if (mKeyState[OpenTK.Input.Key.D])
			{
				move = true;
                MoveRight(fdt);
			}

			if (mKeyState[OpenTK.Input.Key.Q])
			{
				move = true;
                MoveUp(fdt);
			}

			if (mKeyState[OpenTK.Input.Key.E])
			{
				move = true;
                MoveDown(fdt);
			}

			if (move)
			{ 
				mCamera.updateCameraVectors();
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

			if (e.Key == OpenTK.Input.Key.Space)
			{ 
				mCamera.Front = mSaveFront;
				mCamera.Position = mSavePosition;
				mCamera.WorldUp = mSaveWorldUp;
				mCamera.Yaw = mSaveYaw;
				mCamera.Pitch = mSavePitch;
				mCamera.updateCameraVectors();
			}
		}

		public void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			mKeyState[e.Key] = true;
		}

		public void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
		{
			if (!e.Mouse.IsAnyButtonDown)
			{
				return;
			}

			float ox = (float)e.XDelta * mCamera.MouseSensitivity;
			float oy = (float)e.YDelta * mCamera.MouseSensitivity;

			mCamera.Yaw   += ox;
			mCamera.Pitch -= oy;

			if (mConstrainPitch)
			{ 
				// Make sure that when pitch is out of bounds, screen doesn't get flipped
				if (mCamera.Pitch > 89.0f)
				{ 
	                mCamera.Pitch = 89.0f;
				}

				if (mCamera.Pitch< -89.0f)
				{ 
	                mCamera.Pitch = -89.0f;
				}
			}

			mCamera.updateCameraVectors();
		}

		private void MoveForward(float dt)
		{
			//Console.WriteLine("MoveForward");
			mCamera.Position += mCamera.Front * mCamera.MovementSpeed * dt;
		}

		private void MoveBack(float dt)
		{
			//Console.WriteLine("MoveBack");
			mCamera.Position -= mCamera.Front * mCamera.MovementSpeed * dt;
		}

		private void MoveLeft(float dt)
		{
			//Console.WriteLine("MoveLeft");
			mCamera.Position -= mCamera.Right * mCamera.MovementSpeed * dt;
		}

		private void MoveRight(float dt)
		{
			//Console.WriteLine("MoveRight");
			mCamera.Position += mCamera.Right * mCamera.MovementSpeed * dt;
		}

		private void MoveUp(float dt)
		{
			//Console.WriteLine("MoveUp");
			mCamera.Position += mCamera.Up * mCamera.MovementSpeed * dt;
		}

		private void MoveDown(float dt)
		{
			//Console.WriteLine("MoveDown");
			mCamera.Position -= mCamera.Up * mCamera.MovementSpeed * dt;
		}

		public readonly string mName;
		private Camera mCamera = null;
		private bool mConstrainPitch = true;
		private Dictionary<OpenTK.Input.Key, bool> mKeyState = new Dictionary<OpenTK.Input.Key, bool>();

		Vector3 mSaveFront = new Vector3();
		Vector3 mSavePosition = new Vector3();
		Vector3 mSaveWorldUp = new Vector3();
		float mSaveYaw = 0.0f;
		float mSavePitch = 0.0f;
	};
}

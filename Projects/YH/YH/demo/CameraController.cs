using System;
namespace YH
{
	public class CameraController
	{
		public CameraController(string name, Camera cam)
		{
			mName = name;

			mCamera = cam;

			mKeyboard = new Keyboard(name + "'s keyboard");
			mKeyboard.RegisterKeyEvent(OpenTK.Input.Key.W, this.MoveForward, null);
			mKeyboard.RegisterKeyEvent(OpenTK.Input.Key.S, this.MoveBack, null);
			mKeyboard.RegisterKeyEvent(OpenTK.Input.Key.A, this.MoveLeft, null);
			mKeyboard.RegisterKeyEvent(OpenTK.Input.Key.D, this.MoveRight, null);
			mKeyboard.RegisterKeyEvent(OpenTK.Input.Key.Q, this.MoveUp, null);
			mKeyboard.RegisterKeyEvent(OpenTK.Input.Key.E, this.MoveDown, null);

			mMouse = new Mouse(name + "'s mouse");
			mMouse.RegisterMouseMoveEvent("CameraMouseMove", this.MouseMove);
		}

		public void Capture(double dt)
		{
			mDeltaTime = (float)dt;

			mKeyboard.Capture();
			mMouse.Capture();
			mCamera.updateCameraVectors();
		}

		public Keyboard GetKeyboard()
		{
			return mKeyboard;
		}

		public Camera GetCamera()
		{
			return mCamera;
		}

		private void MoveForward(OpenTK.Input.Key k)
		{
			Console.WriteLine("MoveForward");
			mCamera.Position += mCamera.Front * mCamera.MovementSpeed * mDeltaTime;
			mCamera.updateCameraVectors();
		}

		private void MoveBack(OpenTK.Input.Key k)
		{
			Console.WriteLine("MoveBack");
			mCamera.Position -= mCamera.Front * mCamera.MovementSpeed * mDeltaTime;
			mCamera.updateCameraVectors();
		}

		private void MoveLeft(OpenTK.Input.Key k)
		{
			Console.WriteLine("MoveLeft");

			mCamera.Position -= mCamera.Right * mCamera.MovementSpeed * mDeltaTime;
			mCamera.updateCameraVectors();
		}

		private void MoveRight(OpenTK.Input.Key k)
		{
			Console.WriteLine("MoveRight");
			mCamera.Position += mCamera.Right * mCamera.MovementSpeed * mDeltaTime;
			mCamera.updateCameraVectors();
		}

		private void MoveUp(OpenTK.Input.Key k)
		{
			Console.WriteLine("MoveUp");
			mCamera.Position += mCamera.Up * mCamera.MovementSpeed * mDeltaTime;
			mCamera.updateCameraVectors();
		}

		private void MoveDown(OpenTK.Input.Key k)
		{
			Console.WriteLine("MoveDown");
			mCamera.Position -= mCamera.Up * mCamera.MovementSpeed * mDeltaTime;
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
		private Keyboard mKeyboard = null;
		private float mDeltaTime = 0.0f;
		private Mouse mMouse = null;
		private bool mConstrainPitch = false;
	};
}

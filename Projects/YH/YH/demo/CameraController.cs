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
		}

		public void Capture(double dt)
		{
			mDeltaTime = (float)dt;
			mKeyboard.Capture();

			if (mCameraChanged)
			{
				mCameraChanged = false;
				mCamera.updateCameraVectors();
			}
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
			mCameraChanged = true;
		}

		private void MoveBack(OpenTK.Input.Key k)
		{
			Console.WriteLine("MoveBack");

			mCamera.Position -= mCamera.Front * mCamera.MovementSpeed * mDeltaTime;
			mCameraChanged = true;
		}

		private void MoveLeft(OpenTK.Input.Key k)
		{
			Console.WriteLine("MoveLeft");

			mCamera.Position -= mCamera.Right * mCamera.MovementSpeed * mDeltaTime;
			mCameraChanged = true;
		}

		private void MoveRight(OpenTK.Input.Key k)
		{
			Console.WriteLine("MoveRight");

			mCamera.Position += mCamera.Right * mCamera.MovementSpeed * mDeltaTime;
			mCameraChanged = true;
		}

		public readonly string mName;
		private Camera mCamera = null;
		private Keyboard mKeyboard = null;
		private float mDeltaTime = 0.0f;
		private bool mCameraChanged = false;
	};
}

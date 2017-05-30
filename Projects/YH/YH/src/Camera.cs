using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace YH
{
	public class Camera
	{
		public Camera(Vector3 position, Vector3 up, float yaw, float pitch) 
		{
			Front = new Vector3(0.0f, 0.0f, -1.0f);
			MovementSpeed = SPEED;
			MouseSensitivity = SENSITIVTY;
			Zoom = ZOOM;

			Position = position;
			WorldUp = up;
			Yaw = yaw;
			Pitch = pitch;

			updateCameraVectors();
		}

		public void updateCameraVectors()
		{
			// Calculate the new Front vector
			//Vector3 front = new (0.0, 0.0, 0.0);
			//double a = Math.Cos(MathHelper.DegreesToRadians(Yaw)) * Math.Cos(MathHelper.DegreesToRadians(Pitch));
			var x = Math.Cos(MathHelper.DegreesToRadians(Yaw)) * Math.Cos(MathHelper.DegreesToRadians(Pitch));
			var y = Math.Sin(MathHelper.DegreesToRadians(Pitch));
			var z = Math.Sin(MathHelper.DegreesToRadians(Yaw)) * Math.Cos(MathHelper.DegreesToRadians(Pitch));
			Vector3 front = new Vector3((float)x, (float)y, (float)z);

			Front = Vector3.Normalize(front);
			// Also re-calculate the Right and Up vector
			Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));  // Normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
			Up    = Vector3.Normalize(Vector3.Cross(Right, Front));
		}

		public Matrix4 GetViewMatrix() 
		{
			return Matrix4.LookAt(Position, Position + Front, Up);
		}

		static public readonly float YAW = -90.0f;
		static public readonly float PITCH = 0.0f;
		static public readonly float SPEED = 3.0f;
		static public readonly float SENSITIVTY = 0.25f;
		static public readonly float ZOOM = 45.0f;

		// Camera Attributes
		public Vector3 Position = new Vector3();
		public Vector3 Front = new Vector3();
		public Vector3 Up = new Vector3();
		public Vector3 Right = new Vector3();
		public Vector3 WorldUp = new Vector3();
		// Eular Angles
		public float Yaw = 0.0f;
		public float Pitch = 0.0f;
		// Camera options
		public float MovementSpeed = 0.0f;
		public float MouseSensitivity = 0.0f;
		public float Zoom = 0.0f;
	}
}

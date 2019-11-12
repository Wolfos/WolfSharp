using System;
using System.Numerics;
using WolfSharp.Core;

namespace WolfSharp.Components
{
	public class Camera : Component
	{
		public static Camera Instance;
		
		private float _clipMin = 0.1f;
		private float _clipMax = 100;
		private float _fieldOfView = 60;
		private float _aspectRatio;
		
		public Matrix4x4 Projection;
		public Matrix4x4 View;

		public float ClipMin
		{
			get { return _clipMin; }
			set
			{
				_clipMin = value;
				UpdateProjection();
			}
		}
		public float ClipMax
		{
			get { return _clipMax; }
			set
			{
				_clipMax = value;
				UpdateProjection();
			}
		}
		public float FieldOfView
		{
			get { return _fieldOfView; }
			set
			{
				_fieldOfView = value;
				UpdateProjection();
			}
		}
		public float AspectRatio
		{
			get { return _aspectRatio; }
			set
			{
				_aspectRatio = value;
				UpdateProjection();
			}
		}

		public override void Added()
		{
			Instance = this;
			_aspectRatio = (float)Engine.WindowWidth / Engine.WindowHeight;
			UpdateProjection();
		}

		public override void Update()
		{
			Matrix4x4.Invert(Transform.Matrix, out var matrix);
			var forward = Vector3.TransformNormal(-Vector3.UnitZ + Transform.LocalPosition, matrix);
			var up = Vector3.TransformNormal(Vector3.UnitY, matrix);
			View = Matrix4x4.CreateLookAt(Transform.Position, forward, up);
		}

		private void UpdateProjection()
		{
			Projection = Matrix4x4.CreatePerspectiveFieldOfView(
				(float)Math.PI * FieldOfView / 180.0f,
				AspectRatio,
				ClipMin,
				ClipMax);
		}


		Vector3 ScreenToWorldPosition(Vector2 screenPosition)
		{
			throw new NotImplementedException();
		}
	}
}
using System.Numerics;
using WolfSharp.Core;

namespace WolfSharp.Components
{
	public class Transform : Component
	{
		public Transform Parent;
		
		public Vector3 LocalPosition;
		public Vector3 Position
		{
			get
			{
				if (Parent == null) return LocalPosition;
				return Vector3.Transform(LocalPosition, Matrix);
			}
		}

		public Quaternion LocalRotation;
		public Quaternion Rotation
		{
			get
			{
				if (Parent == null) return LocalRotation;
				return Parent.Rotation * LocalRotation; // TODO: Test this
			}
		}

		public Vector3 LocalScale;
		public Vector3 Scale
		{
			get
			{
				if (Parent == null) return LocalScale;
				return Parent.Scale * LocalScale;
			}
		}
		
		public Matrix4x4 Matrix
		{
			get
			{
				// TODO: Cache these, they only need to be updated if they change
				var translate = Matrix4x4.CreateTranslation(LocalPosition * -Vector3.UnitZ);
				var rotate = Matrix4x4.CreateFromQuaternion(LocalRotation);
				var scale = Matrix4x4.CreateScale(LocalScale);

				var localMatrix = translate * rotate * scale;
				if (Parent == null) return localMatrix;
				return localMatrix * Parent.Matrix;
			}
		}
		
		public Vector3 Forward => Vector3.TransformNormal(Vector3.UnitZ, Matrix);
		public Vector3 Up => Vector3.TransformNormal(Vector3.UnitY, Matrix);


		public override void Added()
		{
			LocalPosition = Vector3.Zero;
			LocalScale = Vector3.One;
			LocalRotation = Quaternion.Identity;
		}

		public void Translate(Vector3 direction, Space space = Space.Global)
		{
			if (space == Space.Global)
			{
				LocalPosition += Vector3.TransformNormal(direction, Matrix);
			}
			else if(space == Space.Local)
			{
				LocalPosition += direction;
			}
		}

		// TODO: Extend with Euler angles input
		public void Rotate(Vector3 axis, float angle)
		{
			var rotateBy = Quaternion.CreateFromAxisAngle(axis, angle);
			LocalRotation *= rotateBy;
		}
	}
}
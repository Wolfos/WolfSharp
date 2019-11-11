using WolfSharp.Components;

namespace WolfSharp.Core
{
	public abstract class Component
	{
		public GameObject GameObject;
		protected Transform Transform => GameObject.Transform;

		/// <summary>
		/// Called when the component gets added to the GameObject
		/// </summary>
		public virtual void Added() { }
		/// <summary>
		/// Called once every frame, before drawing
		/// </summary>
		public virtual void Update() { }
		/// <summary>
		/// Called once every frame, intended for rendering
		/// </summary>
		public virtual void Draw(dynamic commandList) { }
		/// <summary>
		/// Called once every frame, after drawing
		/// </summary>
		public virtual void LateUpdate() { }
		/// <summary>
		/// Called when the GameObject was destroyed, or the Component removed
		/// TODO: Implement
		/// </summary>
		public virtual void Destroyed() { }
	}
}
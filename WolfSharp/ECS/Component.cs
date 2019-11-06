namespace WolfSharp.ECS
{
	public abstract class Component
	{
		protected GameObject gameObject;

		public virtual void Added() { }
		public virtual void Update() { }
		public virtual void LateUpdate() { }
		public virtual void Destroyed() { }
	}
}
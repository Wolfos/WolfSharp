using System.Collections.Generic;
using System.Linq;

namespace WolfSharp.ECS
{
	public class GameObject
	{
		private readonly List<Component> components;

		public GameObject()
		{
			components = new List<Component>();
		}

		public void Update()
		{
			foreach (var c in components) c.Update();
		}
		
		public void LateUpdate()
		{
			foreach (var c in components) c.LateUpdate();
		}

		public void AddComponent<T>() where T : Component, new()
		{
			var c = new T();
			components.Add(c);
			c.Added();
		}

		public T GetComponent<T>() where T : Component
		{
			return components.Find(c => c is T) as T;
		}

		public void RemoveComponent<T>() where T : Component
		{
			components.Remove(GetComponent<T>());
		}
	}
}
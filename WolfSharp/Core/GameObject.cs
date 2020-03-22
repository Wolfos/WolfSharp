using System.Collections.Generic;
using WolfSharp.Components;

namespace WolfSharp.Core
{
	// TODO: Write unit tests for this class
	public class GameObject
	{
		private readonly List<Component> _components;
		public readonly Transform Transform;
		public string Name;

		public GameObject(string name = "GameObject")
		{
			_components = new List<Component>();
			Transform = AddComponent<Transform>();
			Name = name;
		}

		public void Update()
		{
			foreach (var c in _components) c.Update();
		}
		
		public void PreDraw()
		{
			foreach (var c in _components) c.PreDraw();
		}
		
		public void LateUpdate()
		{
			foreach (var c in _components) c.LateUpdate();
		}

		public T AddComponent<T>() where T : Component, new()
		{
			var component = new T {GameObject = this};
			_components.Add(component);
			component.Added();
			
			return component;
		}
		
		public T GetComponent<T>() where T : Component
		{
			return _components.Find(c => c is T) as T;
		}

		public void RemoveComponent<T>() where T : Component
		{
			GetComponent<T>().Destroy();
			_components.Remove(GetComponent<T>());
		}

		public void Destroy()
		{
			foreach (var c in _components) c.Destroy();
		}
	}
}
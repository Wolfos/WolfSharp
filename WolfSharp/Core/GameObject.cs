using System.Collections.Generic;
using WolfSharp.Components;

namespace WolfSharp.Core
{
	// TODO: Write unit tests for this class
	public class GameObject
	{
		private readonly List<Component> _components;
		public readonly Transform Transform;

		public GameObject()
		{
			_components = new List<Component>();
			Transform = AddComponent<Transform>();
		}

		public void Update()
		{
			foreach (var c in _components) c.Update();
		}

		public void Draw(dynamic commandList)
		{
			foreach (var c in _components) c.Draw(commandList);
		}
		
		public void LateUpdate()
		{
			foreach (var c in _components) c.LateUpdate();
		}

		public T AddComponent<T>() where T : Component, new()
		{
			var component = new T();
			component.GameObject = this;
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
			_components.Remove(GetComponent<T>());
		}
	}
}
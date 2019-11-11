using System;
using System.Collections.Generic;
using WolfSharp.Components;

namespace WolfSharp.Core
{
	public class Scene
	{
		public static Scene ActiveScene => Engine.Scene;
		public readonly Camera Camera;
		private List<GameObject> _gameObjects;

		public Scene()
		{
			_gameObjects = new List<GameObject>();
			
			var camera = new GameObject();
			Camera = camera.AddComponent<Camera>();
			AddGameObject(camera);
		}

		public void AddGameObject(GameObject gameObject)
		{
			_gameObjects.Add(gameObject);
		}
		
		public void Update()
		{
			foreach(var g in _gameObjects) g.Update();
		}

		public void Draw(dynamic commandList)
		{
			foreach(var g in _gameObjects) g.Draw(commandList);
		}
		
		public void LateUpdate()
		{
			foreach(var g in _gameObjects) g.LateUpdate();
		}

		public void Render()
		{
			throw new NotImplementedException();
		}
	}
}
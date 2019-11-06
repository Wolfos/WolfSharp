using System;
using System.Collections.Generic;

namespace WolfSharp.ECS
{
	public class Scene
	{
		private List<GameObject> gameObjects;

		public Scene()
		{
			gameObjects = new List<GameObject>();
		}

		public void AddGameObject(GameObject gameObject)
		{
			gameObjects.Add(gameObject);
		}
		
		public void Update()
		{
			foreach(var g in gameObjects) g.Update();
		}
		
		public void LateUpdate()
		{
			foreach(var g in gameObjects) g.LateUpdate();
		}

		public void Render()
		{
			throw new NotImplementedException();
		}
	}
}
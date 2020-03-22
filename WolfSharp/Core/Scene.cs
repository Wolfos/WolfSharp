using System;
using System.Collections.Generic;
using System.Linq;
using WolfSharp.Components;

namespace WolfSharp.Core
{
	public class Scene
	{
		public static Scene ActiveScene => Engine.Scene;
		public readonly Camera Camera;
		public readonly List<GameObject> GameObjects;
		public readonly Queue<GameObject> AddQueue;

		public Scene()
		{
			GameObjects = new List<GameObject>();
			AddQueue = new Queue<GameObject>();
			
			var camera = new GameObject("Camera");
			Camera = camera.AddComponent<Camera>();
			AddGameObject(camera);
		}

		/// Adds the GameObject to the scene at the start of the next frame
		public void AddGameObject(GameObject gameObject)
		{
			AddQueue.Enqueue(gameObject);
		}
		
		public void Update()
		{
			while (AddQueue.Any())
			{
				GameObjects.Add(AddQueue.Dequeue());
			}
			foreach(var g in GameObjects) g.Update();
		}

		public void PreDraw()
		{
			foreach(var g in GameObjects) g.PreDraw();
		}
		
		public void LateUpdate()
		{
			foreach(var g in GameObjects) g.LateUpdate();
		}

		public void Destroy()
		{
			foreach(var g in GameObjects) g.Destroy();
		}
	}
}
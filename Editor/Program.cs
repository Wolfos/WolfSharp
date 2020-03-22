using Editor.UI;
using WolfSharp.Core;
using WolfSharp.UI;

namespace Editor
{
	class Program
	{
		static void Main(string[] args)
		{
			var engine = new Engine(1280, 720, "WolfEngine Editor");
			var scene = new Scene();
			Engine.Scene = scene;
			Editor.Scene = new Scene();

			var canvas = new GameObject("Canvas");
			canvas.AddComponent<Canvas>();
			canvas.AddComponent<EditorGUI>();

			scene.AddGameObject(canvas);

			engine.MainLoop();
		}
	}
}
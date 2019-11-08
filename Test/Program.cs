using System;
using WolfSharp.Core;
using WolfSharp.ECS;

namespace Test
{
    class Program
    {
        private class TestComponent : Component
        {
            public override void Added()
            {
                Console.WriteLine("Added");
            }

            public override void Update()
            {
                Console.WriteLine("Update");
            }

            public override void LateUpdate()
            {
                Console.WriteLine("LateUpdate");
            }
        }
        static void Main(string[] args)
        {
            var engine = new Engine(800, 600, "WolfEngine Test program");
            var scene = new Scene();
            engine.scene = scene;

//            var gameObject = new GameObject();
//            gameObject.AddComponent<TestComponent>();
//            scene.AddGameObject(gameObject);
            
            engine.MainLoop();
        }
    }
}
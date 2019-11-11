using System.Numerics;
using WolfSharp.Components;
using WolfSharp.Core;
using WolfSharp.Rendering;

namespace Test
{
    class Program
    {
        private class TestComponent : Component
        {

            public override void Update()
            {
            }
            
        }
        static void Main(string[] args)
        {
            var engine = new Engine(1280, 720, "WolfEngine Test program");
            var scene = new Scene();
            Engine.Scene = scene;
            
            var shader = new Shader("WolfSharp/Assets/Shaders/Sprite");
            var texture = new Texture2D("WolfSharp/Assets/Sprites/Test.png", Renderer.GraphicsDevice.Aniso4xSampler);
            var mesh = new Mesh();
            mesh.CreateQuad();
            
            Camera.Instance.GameObject.AddComponent<TestComponent>();

            var gameObject = new GameObject();
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.Mesh = mesh;
            meshRenderer.Shader = shader;
            meshRenderer.Texture = texture;
            //gameObject.AddComponent<TestComponent>();
            
            scene.AddGameObject(gameObject);
            
            gameObject.Transform.Translate(Vector3.UnitZ * 3);
            
            engine.MainLoop();
        }
    }
}
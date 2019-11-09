using System;
using System.Diagnostics;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;
using WolfSharp.ECS;
using WolfSharp.Rendering;
using Shader = WolfSharp.Rendering.Shader;

namespace WolfSharp.Core
{
	public class Engine
	{
		public int maxFPS = 60;
		public int windowWidth { get; private set; }
		public int windowHeight { get; private set; }
		public readonly string windowTitle;

		public Scene scene;
		
		private Sdl2Window window;

		private RenderObject renderObject;

		private long currentFrameTime;
		private long previousFrameTime;
		private bool quit;

		public Engine(int windowWidth, int windowHeight, string windowTitle = "WolfEngine")
		{
			this.windowWidth = windowWidth;
			this.windowHeight = windowHeight;
			this.windowTitle = windowTitle;

			InitGraphics();
			CreateResources(); // TODO: Extract
		}

		public void MainLoop()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			while (!quit)
			{
				currentFrameTime = stopwatch.ElapsedTicks;
				float deltaTime = (currentFrameTime - previousFrameTime) / Stopwatch.Frequency;

				window.PumpEvents();
				//TODO: Update input
				//TODO: Handle resize events
				//TODO: Handle quit events

				scene?.Update();
				Draw(deltaTime); // TODO: Draw scene
				scene?.LateUpdate();

				previousFrameTime = currentFrameTime;

				// Framerate limiter
				while (maxFPS != -1 && stopwatch.ElapsedTicks - currentFrameTime < Stopwatch.Frequency / maxFPS)
				{
				}
			}
			
			Renderer.Exit();
		}

		private void InitGraphics()
		{
			var windowCi = new WindowCreateInfo
			{
				X = 100,
				Y = 100,
				WindowWidth = windowWidth,
				WindowHeight = windowHeight,
				WindowTitle = windowTitle
			};

			window = VeldridStartup.CreateWindow(ref windowCi);
			
			Renderer.Initialize(window);
		}

		private void Draw(float deltaTime)
		{
			var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
				1.0f,
				(float) window.Width / window.Height,
				0.5f,
				100f);

			var viewMatrix = Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY);

			var modelMatrix = Matrix4x4.Identity;

			renderObject.mvp = modelMatrix * viewMatrix * projectionMatrix;
			
			Renderer.Draw();
		}

		private void CreateResources()
		{
			var shader = new Shader("WolfSharp/Assets/Shaders/Sprite");
			var texture = new Texture2D("WolfSharp/Assets/Sprites/Test.png");
			var mesh = new Mesh();
			mesh.CreateQuad();
			
			renderObject = new RenderObject(shader, texture, mesh, Renderer.GraphicsDevice.Aniso4xSampler);
			Renderer.AddRenderObject(renderObject);
		}
	}
}
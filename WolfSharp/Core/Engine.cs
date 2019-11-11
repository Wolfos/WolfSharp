using System.Diagnostics;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using WolfSharp.Rendering;
using Shader = WolfSharp.Rendering.Shader;

namespace WolfSharp.Core
{
	public class Engine
	{
		public int MaxFps = 60;
		public static int WindowWidth;
		public static int WindowHeight;
		public static float DeltaTime;
		public readonly string WindowTitle;

		public static Scene Scene;
		
		private Sdl2Window _window;

		private RenderObject _renderObject;

		private long _currentFrameTime;
		private long _previousFrameTime;
		private bool _quit;

		public Engine(int windowWidth, int windowHeight, string windowTitle = "WolfEngine")
		{
			WindowWidth = windowWidth;
			WindowHeight = windowHeight;
			WindowTitle = windowTitle;

			InitGraphics();
		}

		public void MainLoop()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			while (!_quit)
			{
				_currentFrameTime = stopwatch.ElapsedTicks;
				DeltaTime = (float)(_currentFrameTime - _previousFrameTime) / Stopwatch.Frequency;

				_window.PumpEvents();
				//TODO: Update input
				//TODO: Handle resize events
				//TODO: Handle quit events

				Scene?.Update();
				Renderer.Draw();
				Scene?.LateUpdate();

				_previousFrameTime = _currentFrameTime;

				// Framerate limiter
				while (MaxFps != -1 && stopwatch.ElapsedTicks - _currentFrameTime < Stopwatch.Frequency / MaxFps)
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
				WindowWidth = WindowWidth,
				WindowHeight = WindowHeight,
				WindowTitle = WindowTitle
			};

			_window = VeldridStartup.CreateWindow(ref windowCi);
			
			Renderer.Initialize(_window);
		}

		private void Draw()
		{
			var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
				1.0f,
				(float) _window.Width / _window.Height,
				0.5f,
				100f);

			var viewMatrix = Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY);

			var modelMatrix = Matrix4x4.Identity;

			_renderObject.MVP = modelMatrix * viewMatrix * projectionMatrix;
			
			Renderer.Draw();
		}
	}
}
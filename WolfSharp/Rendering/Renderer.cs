using System;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;
using WolfSharp.Core;

namespace WolfSharp.Rendering
{
	public class Renderer
	{
		// Both of these are thread safe
		public static GraphicsDevice GraphicsDevice { get; private set; }
		public static DisposeCollectorResourceFactory ResourceFactory { get; private set; }

		private static CommandList _commandList;

		public static void Initialize(Sdl2Window window)
		{
			// ReSharper disable once UseObjectOrCollectionInitializer
			var options = new GraphicsDeviceOptions(
				debug: false,
				swapchainDepthFormat: PixelFormat.R16_UNorm,
				syncToVerticalBlank: true,
				resourceBindingModel: ResourceBindingModel.Improved,
				preferDepthRangeZeroToOne: true,
				preferStandardClipSpaceYDirection: true);
#if DEBUG
			options.Debug = true;
#endif
			GraphicsDevice = VeldridStartup.CreateGraphicsDevice(window, options);
			ResourceFactory = new DisposeCollectorResourceFactory(GraphicsDevice.ResourceFactory);
			_commandList = ResourceFactory.CreateCommandList();

			Console.WriteLine($"Initialized Veldrid with {GraphicsDevice.BackendType} backend");
		}

		public static void Draw()
		{
			_commandList.Begin();
			_commandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
			_commandList.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
			_commandList.ClearDepthStencil(1);

			Scene.ActiveScene.Draw(_commandList);
			
			_commandList.End();
			GraphicsDevice.SubmitCommands(_commandList);
			GraphicsDevice.SwapBuffers();
			GraphicsDevice.WaitForIdle();
		}

		public static void Exit()
		{
			ResourceFactory.DisposeCollector.DisposeAll();
			GraphicsDevice.Dispose();
		}
	}
}
using System;
using System.Collections.Generic;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;

namespace WolfSharp.Rendering
{
	public class Renderer
	{
		// Both of these are thread safe
		public static GraphicsDevice GraphicsDevice { get; private set; }
		public static DisposeCollectorResourceFactory ResourceFactory { get; private set; }

		private static CommandList commandList;
		
		// TODO: Make this thread-safe maybe?
		private static List<RenderObject> renderObjects;
		
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
			commandList = ResourceFactory.CreateCommandList();
			renderObjects = new List<RenderObject>();
			
			Console.WriteLine($"Initialized Veldrid with {GraphicsDevice.BackendType} backend");
		}

		public static void AddRenderObject(RenderObject renderObject)
		{
			lock (renderObjects)
			{
				renderObjects.Add(renderObject);
			}
		}
		
		public static void RemoveRenderObject(RenderObject renderObject)
		{
			lock (renderObjects)
			{
				renderObjects.Remove(renderObject);
			}
		}

		public static void Draw()
		{
			commandList.Begin();
			commandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
			commandList.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
			commandList.ClearDepthStencil(1);

			lock (renderObjects)
			{
				foreach (var renderObject in renderObjects)
				{
					renderObject.Draw(commandList);
				}
			}
			
			commandList.End();
			GraphicsDevice.SubmitCommands(commandList);
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
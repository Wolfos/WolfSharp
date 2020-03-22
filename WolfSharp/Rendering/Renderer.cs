using System;
using System.Collections.Generic;
using System.Linq;
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

		private static CommandList _commandList;
		private static Dictionary<RenderPass, List<RenderObject>> _renderPasses;
		private static IEnumerable<RenderPass> _allPasses; // List of all RenderPasses in the RenderPass enum

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
			
			_renderPasses = new Dictionary<RenderPass, List<RenderObject>>();
			_allPasses = Enum.GetValues(typeof(RenderPass)).Cast<RenderPass>();
			foreach (var pass in _allPasses)
			{
				_renderPasses.Add(pass, new List<RenderObject>());
			}

			Console.WriteLine($"Initialized Veldrid with {GraphicsDevice.BackendType} backend");
		}

		public static void AddRenderObject(RenderObject renderObject, RenderPass renderPass)
		{
			_renderPasses[renderPass].Add(renderObject);
		}

		public static void RemoveRenderObject(RenderObject renderObject, RenderPass renderPass)
		{
			_renderPasses[renderPass].Remove(renderObject);
		}

		public static void Draw()
		{
			_commandList.Begin();
			_commandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
			_commandList.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
			_commandList.ClearDepthStencil(1);
			
			foreach (var pass in _allPasses)
			{
				_commandList.PushDebugGroup(pass.ToString());
				foreach (var renderObject in _renderPasses[pass])
				{
					renderObject.Draw(_commandList);
				}
				_commandList.PopDebugGroup();
			}

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
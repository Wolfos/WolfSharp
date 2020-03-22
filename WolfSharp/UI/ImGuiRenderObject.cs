using Veldrid;
using WolfSharp.Core;
using WolfSharp.Rendering;

namespace WolfSharp.UI
{
	public class ImGuiRenderObject : RenderObject
	{
		private ImGuiRenderer _imGuiRenderer;
		
		public ImGuiRenderObject()
		{
			var width = Engine.WindowWidth;
			var height = Engine.WindowHeight;

			var graphicsDevice = Renderer.GraphicsDevice;
			_imGuiRenderer = new ImGuiRenderer(graphicsDevice, graphicsDevice.SwapchainFramebuffer.OutputDescription,
				width, height, ColorSpaceHandling.Linear);
		}
		
		public void Resize(int width, int height) => _imGuiRenderer.WindowResized(width, height);

		public void Update()
		{
			_imGuiRenderer.Update(Engine.DeltaTime, Input.Snapshot);
		}

		public override void Draw(CommandList commandList)
		{
			_imGuiRenderer.Render(Renderer.GraphicsDevice, commandList);
		}
		
		public void Destroy()
		{
			_imGuiRenderer.Dispose();
		}
	}
}
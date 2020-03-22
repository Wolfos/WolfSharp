using Veldrid;
using WolfSharp.Core;
using WolfSharp.Rendering;

namespace WolfSharp.UI
{
	public class Canvas : Component
	{
		private ImGuiRenderObject _renderObject;

		public override void Added()
		{
			_renderObject = new ImGuiRenderObject();
			Renderer.AddRenderObject(_renderObject, RenderPass.UI);
		}

		public override void Update()
		{
			_renderObject.Update();
		}

		public override void Destroy()
		{
			Renderer.RemoveRenderObject(_renderObject, RenderPass.UI);
			_renderObject.Destroy();
		}
		
	}
}
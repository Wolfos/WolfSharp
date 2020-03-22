using Veldrid;
using WolfSharp.Core;
using WolfSharp.Rendering;
using Shader = WolfSharp.Rendering.Shader;

namespace WolfSharp.Components
{
	public class MeshRenderer : Component
	{
		private MeshRenderObject _renderObject;

		public Mesh Mesh {
			get => _renderObject.Mesh;
			set => _renderObject.Mesh = value;
		}

		public Shader Shader
		{
			get { return _renderObject.Shader; }
			set
			{
				_renderObject.Shader = value;
				_renderObject.UpdateResources();
			}
		}

		public Texture2D Texture
		{
			get { return _renderObject.Texture; }
			set
			{
				_renderObject.Texture = value;
				_renderObject.UpdateResources();
			}
		}

		public override void Added()
		{
			_renderObject = new MeshRenderObject();

			Renderer.AddRenderObject(_renderObject, RenderPass.Main);
		}

		public override void PreDraw()
		{
			_renderObject.MVP = Transform.Matrix * Camera.Instance.View * Camera.Instance.Projection;
		}

		public override void Destroy()
		{
			Renderer.RemoveRenderObject(_renderObject, RenderPass.Main);
		}
	}
}
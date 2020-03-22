using System.Collections.Generic;
using System.Numerics;
using Veldrid;

namespace WolfSharp.Rendering
{
	public class MeshRenderObject : RenderObject
	{
		public Shader Shader;
		public Texture2D Texture;
		public Mesh Mesh;

		// ReSharper disable once InconsistentNaming
		public Matrix4x4 MVP;
		
		private DeviceBuffer _mvpBuffer;
		private List<ResourceSet> _resources;
		

		public MeshRenderObject()
		{
			_mvpBuffer = Renderer.ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
			_resources = new List<ResourceSet>();
		}

		public void UpdateResources()
		{
			if (Shader == null || Texture == null) return;
			
			foreach (var resource in _resources)
			{
				resource.Dispose();
			}
			
			// TODO: Make generic, together with Shader
			_resources.Add(Renderer.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
				Shader.MatrixLayout,
				_mvpBuffer)));

			_resources.Add(Renderer.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
				Shader.TextureLayout,
				Texture.TextureView,
				Texture.Sampler)));
		}

		public override void Draw(CommandList commandList)
		{
			commandList.UpdateBuffer(_mvpBuffer, 0, ref MVP);
			commandList.SetVertexBuffer(0, Mesh.VertexBuffer);
			commandList.SetIndexBuffer(Mesh.IndexBuffer, IndexFormat.UInt16);
			commandList.SetPipeline(Shader.Pipeline);
			for (uint i = 0; i < _resources.Count; i++)
			{
				commandList.SetGraphicsResourceSet(i, _resources[(int)i]);
			}
			
			commandList.DrawIndexed((uint)Mesh.Indices.Count, 1, 0, 0, 0);
		}
	}
}
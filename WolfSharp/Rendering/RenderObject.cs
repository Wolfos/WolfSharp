using System.Collections.Generic;
using System.Numerics;
using Veldrid;

namespace WolfSharp.Rendering
{
	public class RenderObject
	{
		public readonly Shader shader;
		public readonly Texture2D texture;
		public readonly Sampler textureSampler;
		public readonly Mesh mesh;

		public Matrix4x4 mvp;
		
		private readonly DeviceBuffer mvpBuffer;
		
		public readonly List<ResourceSet> resources;
		

		public RenderObject(Shader shader, Texture2D texture, Mesh mesh, Sampler textureSampler)
		{
			this.shader = shader;
			this.texture = texture;
			this.mesh = mesh;
			this.textureSampler = textureSampler;

			mvpBuffer = Renderer.ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
			resources = new List<ResourceSet>();
			
			// TODO: Make generic, together with Shader
			resources.Add(Renderer.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
				shader.matrixLayout,
				mvpBuffer)));

			resources.Add(Renderer.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
				shader.textureLayout,
				texture.textureView,
				textureSampler)));
		}

		public void Draw(CommandList commandList)
		{
			commandList.UpdateBuffer(mvpBuffer, 0, ref mvp);
			commandList.SetVertexBuffer(0, mesh.vertexBuffer);
			commandList.SetIndexBuffer(mesh.indexBuffer, IndexFormat.UInt16);
			commandList.SetPipeline(shader.pipeline);
			for (uint i = 0; i < resources.Count; i++)
			{
				commandList.SetGraphicsResourceSet(i, resources[(int)i]);
			}
			
			commandList.DrawIndexed((uint)mesh.indices.Count, 1, 0, 0, 0);
		}
	}
}
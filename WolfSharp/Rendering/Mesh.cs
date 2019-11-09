using System.Collections.Generic;
using System.Numerics;
using Veldrid;

namespace WolfSharp.Rendering
{
	public struct Vertex
	{
		public const uint SizeInBytes = 20;
		public Vector3 Position;
		public Vector2 Uv;

		public Vertex(Vector3 position, Vector2 uv)
		{
			Position = position;
			Uv = uv;
		}
	}
	
	public class Mesh
	{
		public List<Vertex> vertices;
		public List<ushort> indices;

		public DeviceBuffer vertexBuffer;
		public DeviceBuffer indexBuffer;

		public Mesh()
		{
			vertices = new List<Vertex>();
			indices = new List<ushort>();
		}

		public void UpdateBuffers()
		{
			vertexBuffer =
				Renderer.ResourceFactory.CreateBuffer(new BufferDescription((uint) vertices.Count * Vertex.SizeInBytes,
					BufferUsage.VertexBuffer));
			indexBuffer =
				Renderer.ResourceFactory.CreateBuffer(new BufferDescription((uint) indices.Count * sizeof(ushort),
					BufferUsage.IndexBuffer));
			
			Renderer.GraphicsDevice.UpdateBuffer(vertexBuffer, 0, vertices.ToArray());
			Renderer.GraphicsDevice.UpdateBuffer(indexBuffer, 0, indices.ToArray());
		}
		
		public void CreateQuad()
		{
			vertices.Add(new Vertex(new Vector3(-1, 1, 0), new Vector2(0, 0)));
			vertices.Add(new Vertex(new Vector3(1, 1, 0), new Vector2(1, 0)));
			vertices.Add(new Vertex(new Vector3(-1, -1, 0), new Vector2(0, 1)));
			vertices.Add(new Vertex(new Vector3(1, -1, 0), new Vector2(1, 1)));
			
			indices.AddRange(new ushort[]{0, 1, 2, 3, 2, 1});
			UpdateBuffers();
		}
	}
}
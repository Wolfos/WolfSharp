using System.Collections.Generic;
using System.Numerics;
using Veldrid;

namespace WolfSharp.Rendering
{
	// TODO: UVs should be separate from Vertices
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
		public List<Vertex> Vertices;
		public List<ushort> Indices;

		public DeviceBuffer VertexBuffer;
		public DeviceBuffer IndexBuffer;

		public Mesh()
		{
			Vertices = new List<Vertex>();
			Indices = new List<ushort>();
		}

		public void UpdateBuffers()
		{
			VertexBuffer =
				Renderer.ResourceFactory.CreateBuffer(new BufferDescription((uint) Vertices.Count * Vertex.SizeInBytes,
					BufferUsage.VertexBuffer));
			IndexBuffer =
				Renderer.ResourceFactory.CreateBuffer(new BufferDescription((uint) Indices.Count * sizeof(ushort),
					BufferUsage.IndexBuffer));
			
			Renderer.GraphicsDevice.UpdateBuffer(VertexBuffer, 0, Vertices.ToArray());
			Renderer.GraphicsDevice.UpdateBuffer(IndexBuffer, 0, Indices.ToArray());
		}
		
		public void CreateQuad()
		{
			Vertices.Add(new Vertex(new Vector3(-1, 1, 0), new Vector2(0, 0)));
			Vertices.Add(new Vertex(new Vector3(1, 1, 0), new Vector2(1, 0)));
			Vertices.Add(new Vertex(new Vector3(-1, -1, 0), new Vector2(0, 1)));
			Vertices.Add(new Vertex(new Vector3(1, -1, 0), new Vector2(1, 1)));
			
			Indices.AddRange(new ushort[]{0, 1, 2, 3, 2, 1});
			UpdateBuffers();
		}
	}
}
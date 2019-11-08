using System.IO;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace WolfSharp.Rendering
{
	public class Shader
	{
		public readonly ShaderSetDescription shaderSet;
		
		public Shader(string path, GraphicsDevice graphicsDevice, ResourceFactory factory)
		{
			var vertexCode = File.ReadAllText($"{path}.vert");
			var fragmentCode = File.ReadAllText($"{path}.frag");

			shaderSet = new ShaderSetDescription(
				new[]
				{
					new VertexLayoutDescription(
						new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
						new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
				},
				factory.CreateFromSpirv(
					new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexCode), "main"),
					new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentCode), "main")));
		}
	}
}
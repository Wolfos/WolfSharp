using System.Collections.Generic;
using System.IO;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace WolfSharp.Rendering
{
	public class Shader
	{
		public readonly Pipeline Pipeline;
		public readonly ResourceLayout MatrixLayout;
		public readonly ResourceLayout TextureLayout;

		public Shader(string path)
		{
			var vertexCode = File.ReadAllText($"{path}.vert");
			var fragmentCode = File.ReadAllText($"{path}.frag");

			// TODO: Make generic, together with RenderObject
			// Probably requires waiting for Veldrid.SPIRV update
			var shaderSet = new ShaderSetDescription(
				new[]
				{
					new VertexLayoutDescription(
						new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
						new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
				},
				Renderer.ResourceFactory.CreateFromSpirv(
					new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexCode), "main"),
					new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentCode), "main")));

			var matrixDescription = new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("MVP", ResourceKind.UniformBuffer, ShaderStages.Vertex));

			var textureDescription = new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly,
					ShaderStages.Fragment),
				new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler,
					ShaderStages.Fragment));

			MatrixLayout = Renderer.ResourceFactory.CreateResourceLayout(matrixDescription);

			TextureLayout = Renderer.ResourceFactory.CreateResourceLayout(textureDescription);

			var pipelineDescription = new GraphicsPipelineDescription
			{
				BlendState = BlendStateDescription.SingleOverrideBlend,
				DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
				RasterizerState = new RasterizerStateDescription(
					cullMode: FaceCullMode.None,
					fillMode: PolygonFillMode.Solid,
					frontFace: FrontFace.Clockwise,
					depthClipEnabled: true,
					scissorTestEnabled: false),
				PrimitiveTopology = PrimitiveTopology.TriangleList,
				ResourceLayouts = new[] {MatrixLayout, TextureLayout},
				ShaderSet = shaderSet,
				Outputs = Renderer.GraphicsDevice.SwapchainFramebuffer.OutputDescription
			};
			
			Pipeline = Renderer.ResourceFactory.CreateGraphicsPipeline(pipelineDescription);
		}
	}
}
using System;
using System.Diagnostics;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;
using WolfSharp.ECS;
using WolfSharp.Rendering;
using Shader = WolfSharp.Rendering.Shader;

namespace WolfSharp.Core
{
	public class Engine
	{
		private struct Vertex
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

		public int maxFPS = 60;
		public int windowWidth { get; private set; }
		public int windowHeight { get; private set; }
		public readonly string windowTitle;

		public Scene scene;

		private GraphicsDevice graphicsDevice;
		private Sdl2Window window;

		private CommandList commandList;

		private DeviceBuffer vertexBuffer;
		private DeviceBuffer indexBuffer;
		private DeviceBuffer worldBuffer;
		private DeviceBuffer projectionBuffer;
		private DeviceBuffer viewBuffer;

		private ResourceSet projectionViewSet;
		private ResourceSet worldTextureSet;

		private Shader shader;
		private Pipeline pipeline;
		private Texture2D texture;

		private long currentFrameTime;
		private long previousFrameTime;
		private bool quit;

		private float rotation;

		private ResourceFactory factory;

		public Engine(int windowWidth, int windowHeight, string windowTitle = "WolfEngine")
		{
			this.windowWidth = windowWidth;
			this.windowHeight = windowHeight;
			this.windowTitle = windowTitle;

			InitGraphics();
			CreateResources(); // TODO: Extract
		}

		public void MainLoop()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			while (!quit)
			{
				currentFrameTime = stopwatch.ElapsedTicks;
				float deltaTime = (currentFrameTime - previousFrameTime) / Stopwatch.Frequency;

				window.PumpEvents();
				//TODO: Update input
				//TODO: Handle resize events
				//TODO: Handle quit events

				scene?.Update();
				Draw(deltaTime); // TODO: Draw scene
				scene?.LateUpdate();

				previousFrameTime = currentFrameTime;

				// Framerate limiter
				while (maxFPS != -1 && stopwatch.ElapsedTicks - currentFrameTime < Stopwatch.Frequency / maxFPS)
				{
				}
			}
		}

		private void InitGraphics()
		{
			var windowCi = new WindowCreateInfo
			{
				X = 100,
				Y = 100,
				WindowWidth = windowWidth,
				WindowHeight = windowHeight,
				WindowTitle = windowTitle
			};

			window = VeldridStartup.CreateWindow(ref windowCi);
//			var options = new GraphicsDeviceOptions(
//					debug: false,
//					swapchainDepthFormat: PixelFormat.R16_UNorm,
//					syncToVerticalBlank: true,
//					resourceBindingModel: ResourceBindingModel.Improved,
//					preferDepthRangeZeroToOne: true,
//					preferStandardClipSpaceYDirection: true);
#if DEBUG
			//options.Debug = true;
#endif
			graphicsDevice = VeldridStartup.CreateGraphicsDevice(window);

			Console.WriteLine($"Initialized graphics with {graphicsDevice.BackendType.ToString()} backend");
		}

		private void Draw(float deltaTime)
		{
			//this.rotation += deltaTime;
			commandList.Begin();

//			commandList.UpdateBuffer(projectionBuffer, 0, Matrix4x4.CreatePerspectiveFieldOfView(
//				1.0f,
//				(float)window.Width / window.Height,
//				0.5f,
//				100f));
//
//			commandList.UpdateBuffer(viewBuffer, 0,
//				Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY));
//			
//			Matrix4x4 rotation =
//				Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, this.rotation)
//				* Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, this.rotation);
//			commandList.UpdateBuffer(worldBuffer, 0, ref rotation);

			commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
			commandList.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
			//commandList.ClearDepthStencil(1);
			commandList.SetVertexBuffer(0, vertexBuffer);
			commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
			commandList.SetPipeline(pipeline);
			//commandList.SetGraphicsResourceSet(0, projectionViewSet);
			commandList.SetGraphicsResourceSet(0, worldTextureSet);

			commandList.DrawIndexed(6, 1, 0, 0, 0);

			commandList.End();
			graphicsDevice.SubmitCommands(commandList);
			graphicsDevice.SwapBuffers();
			//graphicsDevice.WaitForIdle();
		}

		private void CreateResources()
		{
			factory = graphicsDevice.ResourceFactory;

//			projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
//			viewBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
//			worldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

			Vertex[] vertices =
			{
//				new Vertex(new Vector2(-.75f, .75f), RgbaFloat.Red),
//				new Vertex(new Vector2(.75f, .75f), RgbaFloat.Green),
//				new Vertex(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
//				new Vertex(new Vector2(.75f, -.75f), RgbaFloat.Yellow)
				new Vertex(new Vector3(-.75f, .75f, 0), new Vector2(0, 0)),
				new Vertex(new Vector3(.75f, .75f, 0), new Vector2(1, 0)),
				new Vertex(new Vector3(-.75f, -.75f, 0), new Vector2(0, 1)),
				new Vertex(new Vector3(.75f, -.75f, 0), new Vector2(1, 1))
			};

			ushort[] indices = {0, 1, 2, 3, 2, 1};

//			var vertices = GetCubeVertices();
//			var indices = GetCubeIndices();
			vertexBuffer =
				factory.CreateBuffer(new BufferDescription((uint)vertices.Length * Vertex.SizeInBytes, BufferUsage.VertexBuffer));
			indexBuffer =
				factory.CreateBuffer(new BufferDescription((uint)indices.Length * sizeof(ushort), BufferUsage.IndexBuffer));

			graphicsDevice.UpdateBuffer(vertexBuffer, 0, vertices);
			graphicsDevice.UpdateBuffer(indexBuffer, 0, indices);

			shader = new Shader("WolfSharp/Assets/Shaders/Sprite", graphicsDevice, factory);
			texture = new Texture2D("WolfSharp/Assets/Sprites/Test.png", graphicsDevice, factory);

//			var projViewLayout = factory.CreateResourceLayout(
//				new ResourceLayoutDescription(
//					new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
//					new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
//
			var worldTextureLayout = factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					//new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
					new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
					new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
//			
//			projectionViewSet = factory.CreateResourceSet(new ResourceSetDescription(
//				projViewLayout,
//				projectionBuffer,
//				viewBuffer));
//
			worldTextureSet = factory.CreateResourceSet(new ResourceSetDescription(
				worldTextureLayout,
				//worldBuffer,
				texture.textureView,
				graphicsDevice.Aniso4xSampler));

//			var pipelineDescription = new GraphicsPipelineDescription
//			{
//				BlendState = BlendStateDescription.SingleOverrideBlend,
//				DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
//				RasterizerState = RasterizerStateDescription.Default,
//				PrimitiveTopology = PrimitiveTopology.TriangleList,
//				ResourceLayouts = new []{projViewLayout, worldTextureLayout},
//				ShaderSet = shader.shaderSet,
//				Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription
//			};
			var pipelineDescription = new GraphicsPipelineDescription
			{
				BlendState = BlendStateDescription.SingleOverrideBlend,
				DepthStencilState = new DepthStencilStateDescription(
					depthTestEnabled: true,
					depthWriteEnabled: true,
					comparisonKind: ComparisonKind.LessEqual),
				RasterizerState = new RasterizerStateDescription(
					cullMode: FaceCullMode.Back,
					fillMode: PolygonFillMode.Solid,
					frontFace: FrontFace.Clockwise,
					depthClipEnabled: true,
					scissorTestEnabled: false),
				PrimitiveTopology = PrimitiveTopology.TriangleList,
				ResourceLayouts = new []{worldTextureLayout},
				ShaderSet = shader.shaderSet,
				Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription
			};


			pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
			commandList = factory.CreateCommandList();
		}

		private static VertexPositionTexture[] GetCubeVertices()
		{
			VertexPositionTexture[] vertices =
			{
				// Top
				new VertexPositionTexture(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
				new VertexPositionTexture(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
				new VertexPositionTexture(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(1, 1)),
				new VertexPositionTexture(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(0, 1)),
				// Bottom                                                             
				new VertexPositionTexture(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(0, 0)),
				new VertexPositionTexture(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(1, 0)),
				new VertexPositionTexture(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(1, 1)),
				new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
				// Left                                                               
				new VertexPositionTexture(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
				new VertexPositionTexture(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(1, 0)),
				new VertexPositionTexture(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(1, 1)),
				new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
				// Right                                                              
				new VertexPositionTexture(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(0, 0)),
				new VertexPositionTexture(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
				new VertexPositionTexture(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(1, 1)),
				new VertexPositionTexture(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(0, 1)),
				// Back                                                               
				new VertexPositionTexture(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
				new VertexPositionTexture(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
				new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(1, 1)),
				new VertexPositionTexture(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
				// Front                                                              
				new VertexPositionTexture(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(0, 0)),
				new VertexPositionTexture(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(1, 0)),
				new VertexPositionTexture(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(1, 1)),
				new VertexPositionTexture(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(0, 1)),
			};

			return vertices;
		}

		private static ushort[] GetCubeIndices()
		{
			ushort[] indices =
			{
				0, 1, 2, 0, 2, 3,
				4, 5, 6, 4, 6, 7,
				8, 9, 10, 8, 10, 11,
				12, 13, 14, 12, 14, 15,
				16, 17, 18, 16, 18, 19,
				20, 21, 22, 20, 22, 23,
			};

			return indices;
		}
	}
}
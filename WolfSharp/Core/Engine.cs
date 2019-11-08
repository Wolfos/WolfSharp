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
		private DeviceBuffer mvpBuffer;

		private ResourceSet mvpSet;
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
			var options = new GraphicsDeviceOptions(
					debug: false,
					swapchainDepthFormat: PixelFormat.R16_UNorm,
					syncToVerticalBlank: true,
					resourceBindingModel: ResourceBindingModel.Improved,
					preferDepthRangeZeroToOne: true,
					preferStandardClipSpaceYDirection: true);
#if DEBUG
			options.Debug = true;
#endif
			graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, options);

			Console.WriteLine($"Initialized graphics with {graphicsDevice.BackendType.ToString()} backend");
		}

		private void Draw(float deltaTime)
		{
			rotation += deltaTime;
			commandList.Begin();

			var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
				1.0f,
				(float) window.Width / window.Height,
				0.5f,
				100f);

			var viewMatrix = Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY);

			var rotationMatrix = Matrix4x4.Identity;

			var mvp = rotationMatrix * viewMatrix * projectionMatrix;
			commandList.UpdateBuffer(mvpBuffer, 0, ref mvp);

			commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
			commandList.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
			commandList.ClearDepthStencil(1);
			commandList.SetVertexBuffer(0, vertexBuffer);
			commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
			commandList.SetPipeline(pipeline);
			commandList.SetGraphicsResourceSet(0, mvpSet);
			commandList.SetGraphicsResourceSet(1, worldTextureSet);

			commandList.DrawIndexed(6, 1, 0, 0, 0);

			commandList.End();
			graphicsDevice.SubmitCommands(commandList);
			graphicsDevice.SwapBuffers();
			graphicsDevice.WaitForIdle();
		}

		private void CreateResources()
		{
			factory = new DisposeCollectorResourceFactory(graphicsDevice.ResourceFactory);

			mvpBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

			Vertex[] vertices =
			{
				new Vertex(new Vector3(-.75f, .75f, 0), new Vector2(0, 0)),
				new Vertex(new Vector3(.75f, .75f, 0), new Vector2(1, 0)),
				new Vertex(new Vector3(-.75f, -.75f, 0), new Vector2(0, 1)),
				new Vertex(new Vector3(.75f, -.75f, 0), new Vector2(1, 1))
			};

			ushort[] indices = {0, 1, 2, 3, 2, 1};

			vertexBuffer =
				factory.CreateBuffer(new BufferDescription((uint) vertices.Length * Vertex.SizeInBytes,
					BufferUsage.VertexBuffer));
			indexBuffer =
				factory.CreateBuffer(new BufferDescription((uint) indices.Length * sizeof(ushort),
					BufferUsage.IndexBuffer));

			graphicsDevice.UpdateBuffer(vertexBuffer, 0, vertices);
			graphicsDevice.UpdateBuffer(indexBuffer, 0, indices);

			shader = new Shader("WolfSharp/Assets/Shaders/Sprite", graphicsDevice, factory);
			texture = new Texture2D("WolfSharp/Assets/Sprites/Test.png", graphicsDevice, factory);

			var projViewLayout = factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("MVP", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

			var worldTextureLayout = factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly,
						ShaderStages.Fragment),
					new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler,
						ShaderStages.Fragment)));

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
				ResourceLayouts = new[] {projViewLayout, worldTextureLayout},
				ShaderSet = shader.shaderSet,
				Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription
			};
			
			pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
			
			mvpSet = factory.CreateResourceSet(new ResourceSetDescription(
				projViewLayout,
				mvpBuffer));

			worldTextureSet = factory.CreateResourceSet(new ResourceSetDescription(
				worldTextureLayout,
				texture.textureView,
				graphicsDevice.Aniso4xSampler));
			
			commandList = factory.CreateCommandList();
		}
	}
}
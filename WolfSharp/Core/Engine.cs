using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace WolfSharp.Core
{
    public class Engine
    {
        private struct Vertex
        {
            public Vector2 position;
            public RgbaFloat color;

            public Vertex(Vector2 position, RgbaFloat color)
            {
                this.position = position;
                this.color = color;
            }

            public const uint SizeInBytes = 24;
        }
        
        public int windowWidth { get; private set; }
        public int windowHeight { get; private set; }
        public readonly string windowTitle;

        private GraphicsDevice graphicsDevice;
        
        private CommandList commandList;
        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;
        private Shader[] shaders;
        private Pipeline pipeline;
        
        private const string VertexCode = @"
            #version 450

            layout(location = 0) in vec2 Position;
            layout(location = 1) in vec4 Color;

            layout(location = 0) out vec4 fsin_Color;

            void main()
            {
                gl_Position = vec4(Position, 0, 1);
                fsin_Color = Color;
            }";

        private const string FragmentCode = @"
            #version 450

            layout(location = 0) in vec4 fsin_Color;
            layout(location = 0) out vec4 fsout_Color;

            void main()
            {
                fsout_Color = fsin_Color;
            }";

        public Engine(int windowWidth, int windowHeight, string windowTitle = "WolfEngine")
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.windowTitle = windowTitle;

            InitGraphics();
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

            var window = VeldridStartup.CreateWindow(windowCi);
            graphicsDevice = VeldridStartup.CreateGraphicsDevice(window);

            CreateResources();
            
            while (window.Exists)
            {
                window.PumpEvents();
                Draw();
            }
        }

        private void Draw()
        {
            commandList.Begin();
            
            commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
            
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            commandList.SetPipeline(pipeline);
            commandList.DrawIndexed(
                indexCount: 4,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);
            
            commandList.End();
            graphicsDevice.SubmitCommands(commandList);
            graphicsDevice.SwapBuffers();
        }
        
        private void CreateResources()
        {
            var resourceFactory = graphicsDevice.ResourceFactory;
            
            Vertex[] quadVertices =
            {
                new Vertex(new Vector2(-.75f, .75f), RgbaFloat.Red),
                new Vertex(new Vector2(.75f, .75f), RgbaFloat.Green),
                new Vertex(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
                new Vertex(new Vector2(.75f, -.75f), RgbaFloat.Yellow)
            };
            
            ushort[] quadIndices = { 0, 1, 2, 3 };
            vertexBuffer =
                resourceFactory.CreateBuffer(new BufferDescription(4 * Vertex.SizeInBytes, BufferUsage.VertexBuffer));
            indexBuffer =
                resourceFactory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));
            
            graphicsDevice.UpdateBuffer(vertexBuffer, 0, quadVertices);
            graphicsDevice.UpdateBuffer(indexBuffer, 0, quadIndices);
            
            var vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));
            
            var vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                Encoding.UTF8.GetBytes(VertexCode),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes(FragmentCode),
                "main");

            shaders = resourceFactory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);
            
            var pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            
            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual);
            
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            
            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();

            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: shaders);
            
            pipelineDescription.Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription;
            
            pipeline = resourceFactory.CreateGraphicsPipeline(pipelineDescription);
            commandList = resourceFactory.CreateCommandList();
        }
    }
}
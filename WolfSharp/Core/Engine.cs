using Veldrid;
using Veldrid.StartupUtilities;

namespace WolfSharp.Core
{
    public class Engine
    {
        public int windowWidth { get; private set; }
        public int windowHeight { get; private set; }
        public readonly string windowTitle;

        private GraphicsDevice graphicsDevice;

        private void InitGraphics()
        {
            var windowCI = new WindowCreateInfo
            {
                X = 100,
                Y = 100,
                WindowWidth = windowWidth,
                WindowHeight = windowHeight,
                WindowTitle = windowTitle
            };

            var window = VeldridStartup.CreateWindow(windowCI);
            graphicsDevice = VeldridStartup.CreateGraphicsDevice(window);

            while (window.Exists)
            {
                window.PumpEvents();
            }
        }
        
        public Engine(int windowWidth, int windowHeight, string windowTitle = "WolfEngine")
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.windowTitle = windowTitle;

            InitGraphics();
        }
    }
}
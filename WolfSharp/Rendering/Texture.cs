using Veldrid;
using Veldrid.ImageSharp;

namespace WolfSharp.Rendering
{
	public class Texture2D
	{
		public readonly Texture deviceTexture;
		public readonly TextureView textureView;
		private ImageSharpTexture textureData;
		
		public Texture2D(string path, GraphicsDevice graphicsDevice, ResourceFactory factory)
		{
			 textureData = new ImageSharpTexture(path);
			 deviceTexture = textureData.CreateDeviceTexture(graphicsDevice, factory);
			 textureView = graphicsDevice.ResourceFactory.CreateTextureView(deviceTexture);
		}
	}
}
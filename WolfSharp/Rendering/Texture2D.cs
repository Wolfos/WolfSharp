using Veldrid;
using Veldrid.ImageSharp;

namespace WolfSharp.Rendering
{
	public class Texture2D
	{
		public readonly Texture DeviceTexture;
		public readonly TextureView TextureView;
		public readonly int Width, Height;
		public readonly Sampler Sampler;
		private ImageSharpTexture _textureData;
		
		// TODO: Change how sampler is passed in
		public Texture2D(string path, Sampler sampler)
		{
			Sampler = sampler;
			_textureData = new ImageSharpTexture(path);
			 DeviceTexture = _textureData.CreateDeviceTexture(Renderer.GraphicsDevice, Renderer.ResourceFactory);
			 TextureView = Renderer.ResourceFactory.CreateTextureView(DeviceTexture);
			 
			 Width = (int)_textureData.Width;
			 Height = (int) _textureData.Height;
		}
	}
}
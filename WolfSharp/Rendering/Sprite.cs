using System;
using System.Collections.Generic;
using Veldrid;
using Rectangle = System.Drawing.Rectangle;

namespace WolfSharp.Rendering
{
	public class Sprite
	{
		public readonly Texture2D Texture;
		public readonly List<Vertex> Vertices;

		/// <summary>
		/// Creates a new sprite from an image file
		/// </summary>
		/// <param name="path">The path to the image</param>
		/// <param name="sampler">The texture sampler to use</param>
		/// <param name="sourceRectangle">The part of the image to use (in pixels)</param>
		public Sprite(string path, Sampler sampler, Rectangle sourceRectangle) : this(new Texture2D(path, sampler), sourceRectangle)
		{
		}
		
		public Sprite(Texture2D texture, Rectangle sourceRectangle)
		{
			Texture = texture;

			// TODO: Implement
			throw new NotImplementedException();
		}
	}
}
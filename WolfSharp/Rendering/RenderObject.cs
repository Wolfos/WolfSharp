using Veldrid;

namespace WolfSharp.Rendering
{
	public abstract class RenderObject
	{
		public abstract void Draw(CommandList commandList);
	}
}
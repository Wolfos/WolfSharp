using Veldrid;
using Veldrid.Sdl2;

namespace WolfSharp.Core
{
	public class Input
	{
		public static InputSnapshot Snapshot { get; private set; }

		public static void Update(InputSnapshot snapshot, Sdl2Window window)
		{
			Snapshot = snapshot;
		}
	}
}
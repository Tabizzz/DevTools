global using static DevTools.Nets.Nets;
using KokoLib;

namespace DevTools.Nets;

internal static class Nets
{
	public static INPCs NPCs => Net<INPCs>.Proxy;

}

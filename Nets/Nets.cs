global using static DevTools.Nets.Nets;
using KokoLib;

namespace DevTools.Nets;

internal static class Nets
{
	public static INpCs NpCs => Net<INpCs>.Proxy;

}

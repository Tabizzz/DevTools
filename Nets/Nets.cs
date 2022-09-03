global using static DevTools.Nets.Nets;
using KokoLib;

namespace DevTools.Nets;

internal static class Nets
{
	public static INpCs Npcs => Net<INpCs>.Proxy;

	public static IAppLog AppLogNet => Net<IAppLog>.Proxy;

	public static ITileFinder TileFinderNet => Net<ITileFinder>.Proxy;

	public static ITimeStep TimeStepNet => Net<ITimeStep>.Proxy;

	public static IDevTools DTN => Net<IDevTools>.Proxy;
}

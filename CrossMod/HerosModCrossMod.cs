using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DevTools.CrossMod;

internal class HerosModCrossMod : ILoadable
{
	public static bool HerosModAvaliable;
	public static Mod HerosMod;

	public const string ItemEditorPermission = "devtools.itemeditor";
	static bool _ItemAdmin;

	public const string NpcExplorerPermission = "devtools.npcexplorer";
	static bool _NpcExplorer;

	public const string ServerLogPermission = "devtools.serverlog";
	static bool _ServerLog;

	public const string WorldInfoPermission = "devtools.worldinfo";
	static bool _WorldInfo;

	public const string TileFinderPermission = "devtools.tilefinder";
	static bool _TileFinder;

	public static bool ItemEditor => 
		Main.netMode == NetmodeID.SinglePlayer || !HerosModAvaliable || (HerosModAvaliable && _ItemAdmin);

	public static bool NpcExplorer =>
		Main.netMode == NetmodeID.SinglePlayer || !HerosModAvaliable || (HerosModAvaliable && _NpcExplorer);

	public static bool ServerLogs =>
		Main.netMode == NetmodeID.MultiplayerClient && (!HerosModAvaliable || (HerosModAvaliable && _ServerLog));

	public static bool WorldInfo =>
		Main.netMode == NetmodeID.SinglePlayer || !HerosModAvaliable || (HerosModAvaliable && _WorldInfo);

	public static bool TileFinder =>
		Main.netMode == NetmodeID.SinglePlayer || !HerosModAvaliable || (HerosModAvaliable && _TileFinder);

	internal static void AddPermissions()
	{
		if (!HerosModAvaliable) return;
		HerosMod.Call("AddPermission", ItemEditorPermission, "Item Editor", (bool b) => { _ItemAdmin = b; });
		HerosMod.Call("AddPermission", ServerLogPermission, "Server Logs", (bool b) => { _ServerLog = b; });
		HerosMod.Call("AddPermission", NpcExplorerPermission, "Npc Explorer", (bool b) => { _NpcExplorer = b; });
		HerosMod.Call("AddPermission", WorldInfoPermission, "World Info", (bool b) => { _WorldInfo = b; });
		HerosMod.Call("AddPermission", TileFinderPermission, "Tile Finder", (bool b) => { _TileFinder = b; });

	}

	public void Load(Mod mod)
	{
		HerosModAvaliable = ModLoader.TryGetMod("HEROsMod", out HerosMod);
	}

	public void Unload()
	{
		HerosMod = null;
		HerosModAvaliable =
		_ItemAdmin =
		_ServerLog =
		_NpcExplorer = false;
	}
}

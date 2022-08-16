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
	public const string ServerLogPermission = "devtools.serverlog";
	static bool _ServerLog;

	public static bool ItemEditor => 
		Main.netMode == NetmodeID.SinglePlayer || !HerosModAvaliable || (HerosModAvaliable && _ItemAdmin);

	public static bool ServerLogs =>
		Main.netMode == NetmodeID.MultiplayerClient && (!HerosModAvaliable || (HerosModAvaliable && _ServerLog));

	internal static void AddPermissions()
	{
		if (!HerosModAvaliable) return;
		HerosMod.Call("AddPermission", ItemEditorPermission, "Item Editor", (bool b) => { _ItemAdmin = b; });
		HerosMod.Call("AddPermission", ServerLogPermission, "Server Logs", (bool b) => { _ServerLog = b; });
	}

	public void Load(Mod mod)
	{
		HerosModAvaliable = ModLoader.TryGetMod("HEROsMod", out HerosMod);
	}

	public void Unload()
	{
		HerosMod = null;
		HerosModAvaliable = false;
		_ItemAdmin = false;
	}
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DevTools.CrossMod;

internal class HerosModCrossMod : ILoadable
{
	public static bool HerosModAvaliable;
	static Mod _HerosMod;

	static bool _ItemAdmin;

	public static bool ItemEditor => 
		Main.netMode == NetmodeID.SinglePlayer || !HerosModAvaliable || (HerosModAvaliable && _ItemAdmin);

	internal static void AddPermissions()
	{
		if (!HerosModAvaliable) return;
		_HerosMod.Call("AddPermission", "itemadminperm", "Item Admin", (bool b) => { _ItemAdmin = b; });
	}

	public void Load(Mod mod)
	{
		HerosModAvaliable = ModLoader.TryGetMod("HEROsMod", out _HerosMod);
	}

	public void Unload()
	{
		_HerosMod = null;
		HerosModAvaliable = false;
		_ItemAdmin = false;
	}
}

using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DevTools.CrossMod;

internal class HerosModCrossMod : ILoadable
{
	public static bool HerosModAvaliable;
	static Mod HerosMod;

	static bool itemAdmin;

	public static bool ItemEditor => 
		Main.netMode == NetmodeID.SinglePlayer || !HerosModAvaliable || (HerosModAvaliable && itemAdmin);

	internal static void AddPermissions()
	{
		if (!HerosModAvaliable) return;
		var a = false;
		var b = (bool b) => { a = b; };
		HerosMod.Call("AddPermission", "itemadminperm", "Item Admin", (Action<bool>)ChangeItemAdminPerm);
	}

	public static void ChangeItemAdminPerm(bool newperm)
	{
		itemAdmin = newperm;
	}

	public void Load(Mod mod)
	{
		HerosModAvaliable = ModLoader.TryGetMod("HEROsMod", out HerosMod);
	}

	public void Unload()
	{
		HerosMod = null;
		HerosModAvaliable = false;
		itemAdmin = false;
	}
}

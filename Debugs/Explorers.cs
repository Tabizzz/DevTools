using DevTools.CrossMod;
using DevTools.Editors;
using Terraria;
using Terraria.ModLoader;
namespace DevTools.Debugs;

internal class Editors : IGui
{
	public void Gui()
	{
		if (Main.gameMenu || !TreeNode("Editors"))	return;

		if (HerosModCrossMod.ItemEditor)
			Checkbox("Item Editor", ref ItemEditor.Open);
		else
			TextWrapped("You don't have enough permissions to use Item Editor in this server.");

		TreePop();
	}

	public void Load(Mod mod)
	{
		InfoWindow.Debugs.Add(this);
	}

	public void Unload()
	{
		InfoWindow.Debugs.Remove(this);
	}
}
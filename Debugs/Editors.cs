using DevTools.Explorers;
using Terraria;
using Terraria.ModLoader;
namespace DevTools.Debugs;

internal class Explorers : IGui
{
	public void Gui()
	{
		if (Main.gameMenu || !TreeNode("Explorers"))	return;

		Checkbox("NPC Explorer", ref NpcExplorer.Open);

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
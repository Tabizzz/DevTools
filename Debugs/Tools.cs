using DevTools;
using Terraria.ModLoader;
using Terraria;
using DevTools.Tools;

internal class Tools : IGui
{
	public void Gui()
	{
		if (Main.gameMenu || !TreeNode("Tools")) return;

		Checkbox("Hitboxes", ref Hitboxes.Open);

		Checkbox("Tile Finder", ref TileFinder.Open);

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
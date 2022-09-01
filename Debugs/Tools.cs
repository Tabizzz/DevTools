using DevTools;
using Terraria.ModLoader;
using Terraria;
using DevTools.Tools;
using DevTools.CrossMod;

internal class Tools : IGui
{
	public void Gui()
	{
		if (Main.gameMenu || !TreeNode("Tools")) return;

		Checkbox("Hitboxes", ref Hitboxes.Open);

		if(HerosModCrossMod.TileFinder)
		Checkbox("Tile Finder", ref TileFinder.Open);

		if (HerosModCrossMod.WorldInfo)
			Checkbox("World Info", ref WorldInfo.Open);

		Checkbox("ColorBlind simulator", ref ColorBlind.Open);

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
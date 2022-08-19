using Terraria.ModLoader;
using ImGuiNET;
using Terraria;
using Microsoft.Xna.Framework;

namespace DevTools.Explorers;

public class NpcExplorerSelector : IGui<ImDrawListPtr>
{
	public static bool NpcSelector = true;
	public void Gui(ImDrawListPtr drawList)
	{
		if(NpcExplorer.HasHitbox && NpcSelector)
		{
			var npc = Main.npc[NpcExplorer.Selected];
			drawList.AddHitBox(npc.getRect(), Color.Red, Color.Yellow);
		}
		NpcExplorer.HasHitbox = false;
	}

	public void Load(Mod mod)
	{
		InfoWindow.Fronts.Add(this);
	}

	public void Unload()
	{
		InfoWindow.Fronts.Remove(this);
	}
}

using Terraria.ModLoader;
using ImGuiNET;
using Terraria;
using Microsoft.Xna.Framework;

namespace DevTools.Explorers;

public class NPCExplorerSelector : IGui<ImDrawListPtr>
{

	public void Gui(ImDrawListPtr drawList)
	{
		if(NPCExplorer.has_hitbox)
		{
			NPCExplorer.has_hitbox = false;
			var npc = Main.npc[NPCExplorer.selected];
			drawList.AddHitBox(npc.getRect(), Color.Red, Color.Yellow);
		}
	
	}


	public void Load(Mod mod)
	{
		InfoWindow.fronts.Add(this);
	}

	public void Unload()
	{
		InfoWindow.fronts.Remove(this);
	}
}

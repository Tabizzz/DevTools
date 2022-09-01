using ImGuiNET;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace DevTools.Tools;

internal class Hitboxes :  IGui
{
	public static bool Open;
	public static bool Npc;
	public static bool Projectiles;
	public static bool Players;

	public void Gui()
	{
		if(!Open) return;
		if (Begin("Hitboxes", ref Open))
		{
			Checkbox("NPC", ref Npc);
			SameLine();
			Checkbox("Projectiles", ref Projectiles);
			SameLine();
			Checkbox("Players", ref Players);
		}
		End();
	}

	public void Load(Mod mod)
	{
		InfoWindow.Guis.Add(this);
	}

	public void Unload()
	{
		InfoWindow.Guis.Remove(this);
	}
}

public class hitboxDraw : IGui<ImDrawListPtr>
{
	public void Gui(ImDrawListPtr drawList)
	{
		if (Hitboxes.Npc)
			foreach (var npc in Main.npc.SkipLast(1))
				if (npc.active)
					drawList.AddHitBox(npc.getRect(), Color.Aqua, Color.GreenYellow);

		if (Hitboxes.Projectiles)
			foreach (var npc in Main.projectile.SkipLast(1))
				if (npc.active)
					drawList.AddHitBox(npc.getRect(), Color.Orange, Color.IndianRed);
		if (Hitboxes.Players)
			foreach (var npc in Main.player.SkipLast(1))
				if (npc.active)
					drawList.AddHitBox(npc.getRect(), Color.Purple, Color.BlueViolet);
	}

	public void Load(Mod mod)
	{
		InfoWindow.Backs.Add(this);
	}

	public void Unload()
	{
		InfoWindow.Backs.Remove(this);
	}
}
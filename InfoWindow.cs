using ImGUI;
using ImGuiNET;
using System.Collections.Generic;

namespace DevTools;

internal class InfoWindow : ModImGui
{
	public static List<IGui> debugs = new();
	public static List<IGui> guis = new();
	public static List<IGui<ImDrawListPtr>> backs = new();
	public static List<IGui<ImDrawListPtr>> fronts = new();

	public override void DebugGUI()
	{
		if (!BeginChild(Mod.Name)) return;
		if (!CollapsingHeader(Mod.DisplayName)) return;

		debugs.ForEach(d=>d.Gui());

		EndChild();
	}

	public override void CustomGUI()
	{
		guis.ForEach(d => d.Gui());
	}

	public override void BackgroundDraw(ImDrawListPtr drawList)
	{
		backs.ForEach(d=> d.Gui(drawList));
		/*
		foreach (var npc in Main.npc.SkipLast(1).Where(n=>n.active))
		{
			var rect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
			
			if (rect.Intersects(npc.getRect()))
			{
				var orig = npc.position - Main.screenPosition;
				var end = orig + npc.Size;

				drawList.AddRect(orig.Convert(), end.Convert(), Color.Yellow.PackedValue);
				renderhitbox++;
			}
		}
		foreach (var npc in Main.projectile.SkipLast(1).Where(n => n.active))
		{
			var rect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);

			if (rect.Intersects(npc.getRect()))
			{
				var orig = npc.position - Main.screenPosition;
				var end = orig + npc.Size;
				drawList.AddRect(orig.Convert(), end.Convert(), Color.Yellow.PackedValue);
				renderhitbox++;
			}
		}*/
	}

	public override void ForeroundDraw(ImDrawListPtr drawList)
	{
		fronts.ForEach(d => d.Gui(drawList));
	}
}

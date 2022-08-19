using DevTools.CrossMod;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DevTools.Tools;

internal class TileFinder : IGui
{
	public static bool Open;
	internal static int tileId;
	internal static bool finding;
	internal static bool find;
	internal static Vector2 findPos;
	internal static long findTime;
	internal static int findid;
	internal static bool closest;
	private bool TileFrameX;
	private bool TileFrameY;
	private int iTileFrameX;
	private int iTileFrameY;
	internal static Vector2 findStyle;

	public void Gui()
	{
		if (Main.gameMenu || !Open || !HerosModCrossMod.TileFinder ||  !Begin("Tile Finder", ref Open)) return;

		TextColored(Color.Red.ToVector4().Convert(), "Warning!");
		TextWrapped("This tool can be slow");

		if (finding) BeginDisabled();
		
		InputInt("Tile ID", ref tileId);
		if (tileId < 0) tileId = 0;
		if (tileId >= TileLoader.TileCount) tileId = TileLoader.TileCount - 1;

		Checkbox("Check Style", ref TileFrameX);
		if (TileFrameX)
		{
			SameLine();
			InputInt("Style", ref iTileFrameX);
		}

		Checkbox("Check Alternate", ref TileFrameY);
		if (TileFrameY)
		{
			InputInt("Alternate", ref iTileFrameY);
		}


		var needfind = Button("Find Tile");
		
		SameLine();
		Checkbox("Find closest", ref closest);

		if (finding)
		{
			EndDisabled();
			Text($"Finding tile {findid}...");
		}

		if(find)
		{
			var world = findPos.ToWorldCoordinates();
			TextWrapped($"Tile {findid} find at position {findPos.X}, {findPos.Y}. {(int)(Vector2.Distance(Main.player[Main.myPlayer].position, world) /16)} tiles from you");
			if (Main.tileFrameImportant[findid])
			TextWrapped($"Style: {findStyle.X}, Alternate: {findStyle.Y}");

			if (Button("Teleport"))
				Main.player[Main.myPlayer].position = world - (new Vector2(0, 16 * 3));
		}
		if(findTime > 0)
			TextWrapped($"Last find time: {findTime}ms");

		End(); 

		if(needfind) StartFind();
	}

	private void StartFind()
	{
		finding = true;
		find = false;
		findid = tileId;
		findTime = 0;
		if(TileFrameX || TileFrameY)
			TileFinderNet.FindStyle(closest, tileId, TileFrameX, TileFrameY, iTileFrameX, iTileFrameY);
		else
			TileFinderNet.Find(closest, tileId);

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

public class TileFinderSelector : IGui<ImDrawListPtr>
{
	public static bool TileSelector = true;

	public void Gui(ImDrawListPtr drawList)
	{
		if (TileFinder.find && TileSelector)
		{
			var world = TileFinder.findPos.ToWorldCoordinates(0,0);	
			drawList.AddHitBox(new Rectangle((int)world.X, (int)world.Y, 16, 16), Color.Black, Color.White);
		}
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

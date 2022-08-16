﻿using System;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.ModLoader;
namespace DevTools.Debugs;

internal class WorldInfo : IGui
{
	readonly int[] pos = new int[2];

	public void Gui()
	{
		if (Main.gameMenu || !TreeNode("World Info")) return;

		InputText("World name", ref Main.worldName, 100);

		Checkbox("Hard Mode", ref Main.hardMode);
		Text("Expert Mode: " + Main.expertMode);
		Checkbox("Raining", ref Main.raining);
		Checkbox("Blood Moon", ref Main.bloodMoon);

		Text("Time");
		Indent();
		Text("Update Count: " + Main.GameUpdateCount);
		Text("Current Time: " + (Main.dayTime ? "Day" : "Night"));
		
		var ftime = (float)Main.time;
		SliderFloat("Time: ", ref ftime, 0, Main.dayTime ? 54000 : 32400);
		Main.time = ftime;

		var day = (float)Main.dayRate;
		InputFloat("Day Rate", ref day);
		Main.dayRate = day;

		Unindent();

		Text("World Spawn");
		Indent();
		pos[0] = Main.spawnTileX;
		pos[1] = Main.spawnTileY;
		var position = new Span<int>(pos);
		InputInt2("Pos", ref MemoryMarshal.GetReference(position));
		Main.spawnTileX = position[0];
		Main.spawnTileY = position[1];

		if (Button("Teleport"))
		{
			Main.player[Main.myPlayer].position = new Vector2(Main.spawnTileX, Main.spawnTileY).ToWorldCoordinates() - new Vector2(0, Main.player[Main.myPlayer].Size.Y);
		}
		SameLine();
		if (Button("Set Here"))
		{
			var tile = Main.player[Main.myPlayer].position.ToTileCoordinates();
			Main.spawnTileX = tile.X;
			Main.spawnTileY = tile.Y;
		}
		Unindent();

		Text("Town NPCs");
		Indent();
		Checkbox("Bunny", ref NPC.boughtBunny);
		Checkbox("Cat", ref NPC.boughtCat);
		Checkbox("Dog", ref NPC.boughtCat);
		Checkbox("Angler", ref NPC.savedAngler);
		Checkbox("Bartender", ref NPC.savedBartender);
		Checkbox("Goblin", ref NPC.savedGoblin);
		Checkbox("Golfer", ref NPC.savedGolfer);
		Checkbox("Mech", ref NPC.savedMech);
		Checkbox("Stylist", ref NPC.savedStylist);
		Checkbox("TaxCollector", ref NPC.savedTaxCollector);
		Checkbox("Wizard", ref NPC.savedWizard);
		Unindent();
		
		Text("Down Bosses");
		Indent();
		Checkbox("Boss1", ref NPC.downedBoss1);
		Checkbox("Boss2", ref NPC.downedBoss2);
		Checkbox("Boss3", ref NPC.downedBoss3);

		Unindent();

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

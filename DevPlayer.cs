using DevTools.Utils;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace DevTools;

internal class DevPlayer : ModPlayer
{
	public override void ProcessTriggers(TriggersSet triggersSet)
	{
		if(DevTools.EntityCodeKey.JustPressed)
		{
			if (Main.HoverItem is { IsAir: false, ModItem: not null })
			{
				DevTools.Instance.Logger.Info($"Click on item {Main.HoverItem.ModItem.DisplayName}");
				Decompiler.AddType(Main.HoverItem.ModItem);
			}
			else if (!string.IsNullOrEmpty(Main.hoverItemName))
			{
				// Try to find the matching item in the player's hot-bar
				for (var i = 0; i < 10; i++)
				{
					var item = Main.LocalPlayer.inventory[i];
					if (!(item.HoverName == Main.hoverItemName && item.ModItem is not null))
					{
						continue;
					}
					Decompiler.AddType(item.ModItem);
				}
			}

			foreach (var npc in Main.npc.SkipLast(1).Where(n=>n.active&&n.ModNPC!=null))
			{
				if(npc.Hitbox.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y))
				{
					Decompiler.AddType(npc.ModNPC);
				}
			}
			foreach (var npc in Main.projectile.SkipLast(1).Where(n => n.active && n.ModProjectile != null))
			{
				if (npc.Hitbox.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y))
				{
					Decompiler.AddType(npc.ModProjectile);
				}
			}

			var tileP = Main.MouseWorld.ToTileCoordinates();
			var tile = Framing.GetTileSafely(tileP);
			if(tile.HasTile && ModContent.GetModTile(tile.TileType) is ModTile mtile)
			{
				Decompiler.AddType(mtile);
			}
		}
	}
}

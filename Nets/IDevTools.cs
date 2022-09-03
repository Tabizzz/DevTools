using KokoLib;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace DevTools.Nets;

[Broadcast]
public interface IDevTools
{
	void HardMode(bool hardMode);
	void SetWorldSpawn(Point tile);

	private class Impl : ModHandler<IDevTools>, IDevTools
	{
		public override IDevTools Handler => this;

		public void HardMode(bool hardMode)
		{
			Main.hardMode = hardMode;
		}

		public void SetWorldSpawn(Point tile)
		{
			Main.spawnTileX = tile.X;
			Main.spawnTileY = tile.Y;
		}
	}
}
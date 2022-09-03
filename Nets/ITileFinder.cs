using DevTools.Tools;
using KokoLib;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace DevTools.Nets;

public interface ITileFinder
{
	void Find(bool closest, int tile);

	void FindStyle(bool closest, int tile, bool x, bool y, int sx, int sy);

	void FindResult(Vector2 position, int ms, Vector2 style);

	void FindError();


	private class Impl : ModHandler<ITileFinder>, ITileFinder
	{
		public override ITileFinder Handler => this;

		public void Find(bool closest, int tile)
		{
			if(Main.netMode is NetmodeID.Server or NetmodeID.SinglePlayer)
			{
				var t = new Thread(FindThread);
				t.Priority = ThreadPriority.Lowest;
				t.IsBackground = true;
				t.Start((closest, tile, WhoAmI, false, false, 0, 0));
			}
		}

		public void FindStyle(bool closest, int tile, bool x, bool y, int sx, int sy)
		{
			if (Main.netMode is NetmodeID.Server or NetmodeID.SinglePlayer)
			{
				var t = new Thread(FindThread);
				t.Priority = ThreadPriority.Lowest;
				t.IsBackground = true;
				t.Start((closest, tile, WhoAmI, x, y, sx, sy));
			}
		}

		private void FindThread(object state)
		{
			var pos = Vector2.Zero;
			var style = Vector2.Zero;
			if (state is (bool closest, int tile, int client, bool x, bool y, int sx, int sy))
			{
				var p = new Point(sx, sy);
				var sw = Stopwatch.StartNew();
				bool find;
				if (closest)
				{
					var start = Main.player[client].position.ToTileCoordinates();
					find = SpiralFind(Main.maxTilesX, Main.maxTilesY, start.X, start.Y, ref pos, ref style, p, x, y, tile);
				}
				else
				{
					find = NormalFind(tile, ref pos, ref style, p, x, y);
				}

				sw.Stop();
				Net.ToClient = client;
				if (find) TileFinderNet.FindResult(pos, (int)sw.ElapsedMilliseconds, style);
				else TileFinderNet.FindError();
			}
		}

		private bool SpiralFind(int X, int Y, int oX, int oY, ref Vector2 findPos, ref Vector2 style, Point p, bool fx, bool fy, int tileid)
		{
			int x, y, dx, dy;
			x = y = dx = 0;
			dy = -1;
			int t = Math.Min(Math.Max(X, Y), 40000);
			int maxI = t * t;

			for (int i = 0; i < maxI; i++)
			{
				if ((-X / 2 <= x) && (x <= X / 2) && (-Y / 2 <= y) && (y <= Y / 2))
				{
					if (Checktile(x + oX, y + oY, ref findPos, ref style, fx, fy, p, tileid)) return true;
				}
				if ((x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1 - y)))
				{
					t = dx;
					dx = -dy;
					dy = t;
				}
				x += dx;
				y += dy;
			}
			return false;
		}

		private bool Checktile(int x, int y, ref Vector2 findPos, ref Vector2 style, bool fx, bool fy, Point p, int tileid)
		{
			var tile = Framing.GetTileSafely(x, y);
			if (tile.HasTile && tile.TileType == tileid)
			{
				var b = true;
				int sty = 0, alt = 0;
				TileObjectData.GetTileInfo(tile, ref sty, ref alt);


				if (fx && !(sty == p.X)) b = false;
				if (fy && !(alt == p.Y)) b = false;

				if (b)
				{
					findPos = new Vector2(x, y);
					style = new(sty, alt);
					return true;
				}
			}
			return false;
		}

		private bool NormalFind(int tileId, ref Vector2 findPos, ref Vector2 style, Point p, bool x, bool y)
		{
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					if (Checktile(i, j, ref findPos, ref style, x, y, p, tileId)) return true;
				}

			}

			return false;
		}

		public void FindResult(Vector2 position, int ms, Vector2 style)
		{
			if (Main.netMode is NetmodeID.MultiplayerClient or NetmodeID.SinglePlayer)
			{
				TileFinder.finding = false;
				TileFinder.find = true;
				TileFinder.findPos = position;
				TileFinder.findTime = ms;
				TileFinder.findStyle = style;
			}
		}

		public void FindError()
		{
			if (Main.netMode is NetmodeID.MultiplayerClient or NetmodeID.SinglePlayer)
			{
				TileFinder.finding = false;
			}
		}
	}
}

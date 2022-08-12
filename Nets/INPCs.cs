using KokoLib;
using Terraria;
using Terraria.ID;
namespace DevTools.Nets;

public interface INpCs
{
	void Move(NPC n, Vector2 vector2);
	void Heal(NPC n);

	private class Imp : ModHandler<INpCs>, INpCs
	{
		public override INpCs Handler => this;

		public void Move(NPC n, Vector2 vector2)
		{
			n.position = vector2;
			if(Main.netMode == NetmodeID.Server)
				NpCs.Move(n, vector2);
		}

		public void Heal(NPC n)
		{
			n.life = n.lifeMax;
			if (Main.netMode == NetmodeID.Server)
				NpCs.Heal(n);
		}
	}
}
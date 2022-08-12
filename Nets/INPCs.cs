using KokoLib;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DevTools;

public interface INPCs
{
	void Move(NPC n, Vector2 vector2);
	void Heal(NPC n);

	private class Imp : ModHandler<INPCs>, INPCs
	{
		public override INPCs Handler => this;

		public void Move(NPC n, Vector2 vector2)
		{
			n.position = vector2;
			if(Main.netMode == NetmodeID.Server)
				NPCs.Move(n, vector2);
		}

		public void Heal(NPC n)
		{
			n.life = n.lifeMax;
			if (Main.netMode == NetmodeID.Server)
				NPCs.Heal(n);
		}
	}
}
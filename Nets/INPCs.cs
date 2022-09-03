using KokoLib;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
namespace DevTools.Nets;

[Broadcast]
public interface INpCs
{
	void Move(NPC n, Vector2 vector2);
	void Heal(NPC n);
	void Disable(NPC n);

	private class Imp : ModHandler<INpCs>, INpCs
	{
		public override INpCs Handler => this;

		public void Move(NPC n, Vector2 vector2)
		{
			n.position = vector2;
		}

		public void Heal(NPC n)
		{
			n.life = n.lifeMax;
		}

		public void Disable(NPC n)
		{
			n.active = false;
		}
	}
}
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using ImGUI.Renderer;
using ImGUI.Utils;
using Terraria.GameContent.Bestiary;
using System.Linq;
using Terraria.DataStructures;
using System;
using Microsoft.Xna.Framework;

namespace DevTools.Explorers;

public class NPCExplorer : IGui
{
	public static int selected = -1;
	private static int npc_texture_frame = 1;
	private static bool animate_npc_texture;
	private static int frame_timer;
	public static bool open = false;
	internal static bool has_hitbox;

	public void Gui()
	{
		if (!open) return;
		ImGuiUtils.SimpleLayout(ref open, ref Main.npc, "NPC", ref selected,
		n => n.active,
		n => n.GivenOrTypeName,
		n =>
		{
			TabDescription(n);
			TabAI(n);
			TabBuffs(n);
			TabTexture(n);
			EndTabBar();
			has_hitbox = true;
		},
		Buttons
		);
	}

	private void TabBuffs(NPC n)
	{
		if (BeginTabItem("Buffs"))
		{
			for (int i = 0; i < n.buffType.Length; i++)
			{
				if(n.buffType[i] > 0)
				{
					var tex = TextureBinder.buff[n.buffType[i]];
					Image(tex.ptr, new ImVect2(20, 20));
					SameLine();
					var colo = Main.debuff[n.buffType[i]] ? Color.IndianRed.ToVector4() : Color.Green.ToVector4();
					TextColored(colo.Convert() ,$"{Lang.GetBuffName(n.buffType[i])}({n.buffType[i]}):");
					Indent();
					TextWrapped(Lang.GetBuffDescription(n.buffType[i]));
					TextWrapped($"Index: {i}");
					TextWrapped($"Time: {n.buffTime[i]} ({MathF.Round(n.buffTime[i]/ 60f, 1):F1}s)");
					if(Button("remove"))
					{
						n.buffTime[i] = 0;
					}
					Unindent();
					Separator();
				}
			}
			EndTabItem();
		}
	}

	private void TabTexture(NPC n)
	{
		if (BeginTabItem("Texture"))
		{
			var texture = TextureBinder.npcs[n.type];

			Image(texture.ptr, texture.Transform(100), texture.Uv0(npc_texture_frame), texture.Uv1(npc_texture_frame));
			Separator();
			SliderInt("Frame", ref npc_texture_frame, 1, texture.frames, $"{npc_texture_frame} / {texture.frames}");
			Checkbox("Animate", ref animate_npc_texture);
			if (animate_npc_texture)
			{
				frame_timer++;
				if (frame_timer > 5)
				{
					frame_timer = 0;
					npc_texture_frame++;
					if (npc_texture_frame > texture.frames)
					{
						npc_texture_frame = 1;
					}
				}
			}
			EndTabItem();
		}
	}

	private void TabAI(NPC n)
	{
		if (BeginTabItem("AI"))
		{
			TextWrapped("aiStyle: ");
			SameLine();
			TextWrapped(n.aiStyle.ToString());

			TextWrapped("aiAction: ");
			SameLine();
			TextWrapped(n.aiAction.ToString());

			TextWrapped("ai: ");
			Indent();

			for (int i = 0; i < n.ai.Length; i++)
			{
				TextWrapped($"ai[{i}]: ");
				SameLine();
				TextWrapped(n.ai[i].ToString());
			}

			Unindent();

			TextWrapped("target: ");
			SameLine();

			if (n.HasValidTarget)
			{
				TextWrapped(n.TranslatedTargetIndex + "(" + (n.HasPlayerTarget ? "Player" : "NPC") + ")");
			}
			else
			{
				TextWrapped(n.target.ToString());
			}

			EndTabItem();
		}
	}

	private void TabDescription(NPC n)
	{
		if (BeginTabItem("Description"))
		{
			TextWrapped("Fullname: ");
			SameLine();
			TextWrapped(n.FullName);

			TextWrapped("GivenName: ");
			SameLine();
			TextWrapped(n.GivenName);

			TextWrapped("type: ");
			SameLine();
			TextWrapped(n.type.ToString());

			TextWrapped("netId: ");
			SameLine();
			TextWrapped(n.netID.ToString());

			TextWrapped("life: ");
			SameLine();
			TextWrapped(n.life + "/" + n.lifeMax);

			VectorWrapped("position", n.position);

			VectorWrapped("velocity", n.velocity, true);

			

			EndTabItem();
		}
	}

	private void Buttons(NPC n)
	{
		if (n.type != NPCID.TargetDummy)
		{
			if (Button("Teleport to you"))
			{
				n.position = Main.player[Main.myPlayer].Center - new Vector2(0, n.Size.Y);
			}
			SameLine();
		}
		if (Button("Teleport to"))
		{
			Main.player[Main.myPlayer].position = n.Center - new Vector2(0, Main.player[Main.myPlayer].Size.Y);
		}
		SameLine();
		if (Button("Disable"))
		{
			n.active = false;
		}
	}

	public void Load(Mod mod)
	{
		InfoWindow.guis.Add(this);
	}

	public void Unload()
	{
		InfoWindow.guis.Remove(this);
	}
}
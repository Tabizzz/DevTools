using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ImGUI.Renderer;
using DevTools.Utils;
using System;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace DevTools.Explorers;

public class NPCExplorer : IGui
{
	public static int selected = -1;
	private static int npc_texture_frame = 1;
	private static bool animate_npc_texture;
	private static int frame_timer;
	public static bool open = false;
	internal static bool has_hitbox;
	private int current_add_buff = 1;
	private int current_add_buff_time = 60;
	private bool ignore_immune;
	private ImVect2 imvect2;

	public void Gui()
	{
		if (!open) return;
		ImGuiUtils.SimpleLayout(ref open, ref Main.npc, "NPC Explorer", ref selected,
		n => n.active,
		n => n.GivenOrTypeName,
		n =>
		{
			TabDescription(n);
			TabDetails(n);
			TabAI(n);
			TabBuffs(n);
			TabTexture(n);
			TabSound(n);
			EndTabBar();
			has_hitbox = true;
		},
		2,
		Buttons
		);
	}

	private void TabDetails(NPC n)
	{
		if (BeginTabItem("Details"))
		{
			TextWrapped("base damage: ");
			SameLine();
			TextWrapped(n.defDamage.ToString());

			InputInt("damage", ref n.damage);

			TextWrapped("base defense: ");
			SameLine();
			TextWrapped(n.defDefense.ToString());

			InputInt("defense", ref n.defense);

			InputFloat("value", ref n.value);

			InputInt("rarity", ref n.rarity);
			SetNextItemWidth(CalcItemWidth() - 50);
			InputFloat("knockBackResist", ref n.knockBackResist);

			InputFloat("scale", ref n.scale);

			Checkbox("townNPC", ref n.townNPC);

			Checkbox("noGravity", ref n.noGravity);

			InputFloat("npcSlots", ref n.npcSlots);

			TextWrapped($"boss: {n.boss}");

			Checkbox("netAlways", ref n.netAlways);

			EndTabItem();
		}
	}

	private void TabSound(NPC n)
	{
		if (BeginTabItem("Sounds"))
		{
			SoundStyle sound;
			if(n.HitSound.HasValue)
			{
				sound = n.HitSound.Value;
				ShowSoundStats(sound);
			}
			else
			{
				TextWrapped("npc dont have a HitSound");
			}
			Separator();
			
			if(n.DeathSound.HasValue)
			{
				sound = n.DeathSound.Value;
				ShowSoundStats(sound);
			}
			else
			{
				TextWrapped("npc dont have a DeathSound");
			}
			EndTabItem();
		}
	}

	private static void ShowSoundStats(SoundStyle sound)
	{
		var s = sound.Identifier?.ToString();
		TextUnformatted("Identifier: ");
		if (!string.IsNullOrWhiteSpace(s))
		{
			SameLine();
			TextWrapped(s);
		}

		s = sound.SoundPath?.ToString();
		TextUnformatted("SoundPath: ");
		if (!string.IsNullOrWhiteSpace(s))
		{
			SameLine();
			TextWrapped(s);
		}

		if (Button("Play"))
		{
			var old = sound.Volume;
			sound.Volume = 1;
			SoundEngine.PlaySound(sound);
			sound.Volume = old;
		}
	}

	private void TabBuffs(NPC n)
	{
		if (BeginTabItem("Buffs"))
		{
			if(TreeNode("Add Buff"))
			{
				InputInt("Buff ID", ref current_add_buff);
				if (current_add_buff < 1) current_add_buff = 1;
				if (current_add_buff >= BuffLoader.BuffCount) current_add_buff = BuffLoader.BuffCount - 1;
				InputInt("Time", ref current_add_buff_time);
				if (current_add_buff_time <= 0) current_add_buff_time = 1;
				var tex = TextureBinder.buff[current_add_buff];
				Image(tex.ptr, new ImVect2(20, 20));
				SameLine();
				TextWrapped(Lang.GetBuffName(current_add_buff));
				TextWrapped(Lang.GetBuffDescription(current_add_buff).Replace("%", "%%"));
				if (n.buffImmune[current_add_buff])
				{
					TextColored(Color.Red.ToVector4().Convert(), "npc is immune to this buff");
					Checkbox("Ignore immune", ref ignore_immune);
					if (ignore_immune)
						n.buffImmune[current_add_buff] = false;
				}
				else
				{
					ignore_immune = false;
				}
				if((!n.buffImmune[current_add_buff] || ignore_immune) && Button("Add"))
				{
					n.AddBuff(current_add_buff, current_add_buff_time);
				}
				if(ignore_immune)
				{
					n.buffImmune[current_add_buff] = true;
				}
				TreePop();
			}
			if (TreeNode("Current Buffs"))
			{
				for (int i = 0; i < n.buffType.Length; i++)
				{
					if (n.buffType[i] > 0)
					{

						var tex = TextureBinder.buff[n.buffType[i]];
						Image(tex.ptr, new ImVect2(20, 20));
						SameLine();
						var colo = Main.debuff[n.buffType[i]] ? Color.IndianRed.ToVector4() : Color.Green.ToVector4();
						TextColored(colo.Convert(), $"{Lang.GetBuffName(n.buffType[i])}({n.buffType[i]}):");
						Indent();
						TextWrapped(Lang.GetBuffDescription(n.buffType[i]));
						TextWrapped($"Index: {i}");
						TextWrapped($"Time: {n.buffTime[i]} ({MathF.Round(n.buffTime[i] / 60f, 1):F1}s)");
						if (Button("remove"))
						{
							n.buffTime[i] = 0;
						}
						Unindent();
						Separator();
					}
				}
				TreePop();
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

			SliderInt("life", ref n.life, 0, n.lifeMax, n.life + "/" + n.lifeMax);

			imvect2 = n.position.Convert();			
			InputFloat2("position", ref imvect2);
			n.position = imvect2.Convert();

			imvect2 = n.velocity.Convert();
			InputFloat2("velocity", ref imvect2);
			n.velocity = imvect2.Convert();


			imvect2 = n.Size.Convert();
			InputFloat2("Size (hitbox)", ref imvect2);
			n.Size = imvect2.Convert();

			TextWrapped("reallife: ");
			SameLine();
			TextWrapped(n.realLife.ToString());
			if(n.realLife != -1)
			{
				if(Button("View"))
				{
					selected = n.realLife;
				}
			}



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
		if (Button("Healt"))
		{
			n.life = n.lifeMax;
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
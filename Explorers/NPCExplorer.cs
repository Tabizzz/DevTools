using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ImGUI.Renderer;
using DevTools.Utils;
using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using System.Reflection;
using DevTools.CrossMod;

namespace DevTools.Explorers;

public class NpcExplorer : IGui
{
	public static int Selected = -1;

	static int _NpcTextureFrame = 1;

	internal static bool AnimateNpcTexture;

	static int _FrameTimer;
	public static bool Open;
	internal static bool HasHitbox;

	int current_add_buff = 1;

	internal static int current_add_buff_time = 60;

	bool ignore_immune;

	ImVect2 imvect2;

	internal static bool f_public = true;

	internal static bool f_private;

	internal static bool f_instance = true;

	internal static bool f_static;

	internal static bool f_editable = true;

	internal static bool f_readonly;

	public void Gui()
	{
		if (Main.gameMenu || !Open || !HerosModCrossMod.NpcExplorer) return;

		ImGuiUtils.SimpleLayout(ref Open, ref Main.npc, "NPC Explorer", ref Selected,
		n => n.active,
		n => n.GivenOrTypeName,
		n =>
		{
			TabDescription(n);
			TabDetails(n);
			TabAi(n);
			TabBuffs(n);
			TabTexture(n);
			TabSound(n);
			TabFields(n);
			EndTabBar();
			HasHitbox = true;
		},
		2,
		Buttons,
		Options
		);
	}

	private void Options()
	{
		MenuItem("Mark Selected", null, ref NpcExplorerSelector.NpcSelector);
	}

	void TabFields(NPC i)
	{
		if (BeginTabItem("Fields"))
		{
			TextWrapped("this is a class field editor, here you can see all the fields of the NPC class, this tab is made for testing purposes only, what you change here will not be saved unless it is a property found in the other tabs");
			var flags = BindingFlags.Default;

			if (Button("Options"))
				OpenPopup("FieldOptions");

			if (BeginPopup("FieldOptions"))
			{
				MenuItem("Public", null, ref f_public);
				MenuItem("Private", null, ref f_private);
				MenuItem("Instance", null, ref f_instance);
				MenuItem("Static", null, ref f_static);
				MenuItem("Editable", null, ref f_editable);
				MenuItem("Readonly", null, ref f_readonly);
				EndPopup();
			}

			if (f_public) flags |= BindingFlags.Public;
			if (f_instance) flags |= BindingFlags.Instance;
			if (f_private) flags |= BindingFlags.NonPublic;
			if (f_static) flags |= BindingFlags.Static;

			Separator();
			foreach (var item in i.GetType().GetFields(flags))
			{
				var editable =
					f_editable &&
					(item.FieldType == ImGuiUtils.Bool || item.FieldType == ImGuiUtils.Int || item.FieldType == ImGuiUtils.Int) &&
					!item.IsInitOnly && !item.IsLiteral;

				var readon =
					f_readonly &&
					(item.IsInitOnly || item.IsLiteral);

				if (editable || readon)
					ImGuiUtils.FieldEdit(item, i);
			}
			EndTabItem();
		}
	}

	void TabDetails(NPC n)
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

	void TabSound(NPC n)
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

	static void ShowSoundStats(SoundStyle sound)
	{
		var s = sound.Identifier;
		TextUnformatted("Identifier: ");
		if (!string.IsNullOrWhiteSpace(s))
		{
			SameLine();
			TextWrapped(s);
		}

		s = sound.SoundPath;
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

	void TabBuffs(NPC n)
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
				for (var i = 0; i < n.buffType.Length; i++)
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

	void TabTexture(NPC n)
	{
		if (BeginTabItem("Texture"))
		{
			var texture = TextureBinder.npcs[n.type];

			Image(texture.ptr, texture.Transform(100), texture.Uv0(_NpcTextureFrame), texture.Uv1(_NpcTextureFrame));
			Separator();
			SliderInt("Frame", ref _NpcTextureFrame, 1, texture.frames, $"{_NpcTextureFrame} / {texture.frames}");
			Checkbox("Animate", ref AnimateNpcTexture);
			if (AnimateNpcTexture)
			{
				_FrameTimer++;
				if (_FrameTimer > 5)
				{
					_FrameTimer = 0;
					_NpcTextureFrame++;
					if (_NpcTextureFrame > texture.frames)
					{
						_NpcTextureFrame = 1;
					}
				}
			}
			EndTabItem();
		}
	}

	void TabAi(NPC n)
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

			for (var i = 0; i < n.ai.Length; i++)
			{
				TextWrapped($"ai[{i}]: ");
				SameLine();
				TextWrapped(n.ai[i].ToString(CultureInfo.InvariantCulture));
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

	void TabDescription(NPC n)
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
					Selected = n.realLife;
				}
			}



			EndTabItem();
		}
	}

	void Buttons(NPC n)
	{
		if (n.type != NPCID.TargetDummy)
		{
			if (Button("Teleport to you"))
			{
				Npcs.Move(n, Main.player[Main.myPlayer].Center - new Vector2(0, n.Size.Y));
			}
			SameLine();
		}
		if (Button("Teleport to"))
		{
			Main.player[Main.myPlayer].position = n.Center - new Vector2(0, Main.player[Main.myPlayer].Size.Y);
		}
		if (Button("Healt"))
		{
			Npcs.Heal(n);
		}
		SameLine();
		if (Button("Disable"))
		{
			n.active = false;
		}
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
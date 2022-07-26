﻿ using Terraria;
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
using DevTools.Viewers;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;
using System.Collections.Generic;

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

	Vector2 Vector2;

	internal static bool f_public = true;

	internal static bool f_private;

	internal static bool f_instance = true;

	internal static bool f_static;

	internal static bool f_editable = true;

	internal static bool f_readonly;
	private int Selected_global;

	public void Gui()
	{
		if (Main.gameMenu || !Open || !HerosModCrossMod.NpcExplorer)
		{
			_FrameTimer = 0; return;
		}

		ImGuiUtils.SimpleLayout(ref Open, ref Main.npc, "NPC Explorer", ref Selected,
		n => n.active,
		n => n.GivenOrTypeName,
		n =>
		{
			TabDescription(n);
			TabDetails(n);
			TabAi(n);
			TabBuffs(n);
			TabMisc(n);
			if (n.ModNPC is ModNPC npc)
			{
				TabModNPC(npc);
			}
			TabGlobals(n);
			TabFields(n);
			EndTabBar();
			HasHitbox = true;
		},
		2,
		Buttons,
		Options
		);
	}

	private void TabModNPC(ModNPC mnpc)
	{
		if (BeginTabItem("Mod NPC"))
		{
			TextWrapped("this is a mod npc field editor, here you can see all the fields of the ModNPC class, this tab is made for testing purposes only, what you change here will not be saved unless it is a property found in the other tabs");
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
			if (Button("Show code"))
			{
				Decompiler.AddType(mnpc);
			}
			var t = mnpc.GetType();

			foreach (var item in t.GetFields(flags))
			{
				var editable =
					f_editable &&
					(item.FieldType == ImGuiUtils.Bool || item.FieldType == ImGuiUtils.Int || item.FieldType == ImGuiUtils.Int) &&
					!item.IsInitOnly && !item.IsLiteral;

				var readon =
					f_readonly &&
					(item.IsInitOnly || item.IsLiteral);

				if (editable || readon)
					ImGuiUtils.FieldEdit(item, mnpc);
			}
			
			EndTabItem();
		}
	}

	private void TabGlobals(NPC npc)
	{
		var l = npc.Globals.Length;
		if (l > 0 && BeginTabItem("Global NPCS"))
		{
			TextWrapped("Choose a global npc to see");

			if (BeginCombo("Global npc", npc.Globals[Selected_global].Instance.Name))
			{
				for (int i = 0; i < l; i++)
				{
					var current = npc.Globals[i].Instance;
					if (Selectable(current.Name, i == Selected_global))
						Selected_global = i;
				}

				EndCombo();
			}
			var global = npc.Globals[Selected_global].Instance;

			TextWrapped($"Global npc from: {global.Mod.Name}");


			if (Selected_global < 0) Selected_global = 0;
			if (Selected_global >= l) Selected_global = l - 1;

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
			if (Button("Show code"))
			{
				Decompiler.AddType(global);
			}
			var t = global.GetType();
			if (global is UnloadedGlobalNPC unloaded)
			{
				var data = (List<TagCompound>)t.GetField("data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global);

				TextWrapped("you can see the unloaded data in the tag viewer");
				if (Button("Open in tag viewer"))
					TagViewer.OpenTag(data, t => t.GetString("name"));
			}
			/*else if (global is SavePreview save)
			{
				var tag = ItemIO.Save(npc);
				TextWrapped("you can see what the item will save in tag viewer");
				if (Button("Open in tag viewer"))
					TagViewer.OpenTag(tag);
			}*/
			else
			{

				foreach (var gitem in t.GetFields(flags))
				{
					var editable =
						f_editable &&
						(gitem.FieldType == ImGuiUtils.Bool || gitem.FieldType == ImGuiUtils.Int || gitem.FieldType == ImGuiUtils.Int) &&
						!gitem.IsInitOnly && !gitem.IsLiteral;

					var readon =
						f_readonly &&
						(gitem.IsInitOnly || gitem.IsLiteral);

					if (editable || readon)
						ImGuiUtils.FieldEdit(gitem, global);
				}
			}


			EndTabItem();
		}
	}

	private void TabMisc(NPC n)
	{
		if (BeginTabItem("Misc"))
		{
			TabTexture(n);
			Separator();
			TabSound(n);
			EndTabItem();
		}
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
		SoundStyle sound;
		if (n.HitSound.HasValue)
		{
			sound = n.HitSound.Value;
			ShowSoundStats(sound);
		}
		else
		{
			TextWrapped("npc dont have a HitSound");
		}

		if (n.DeathSound.HasValue)
		{
			sound = n.DeathSound.Value;
			ShowSoundStats(sound);
		}
		else
		{
			TextWrapped("npc dont have a DeathSound");
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
				Image(tex.ptr, new Vector2(20, 20));
				SameLine();
				TextWrapped(Lang.GetBuffName(current_add_buff));
				TextWrapped(Lang.GetBuffDescription(current_add_buff).Replace("%", "%%"));
				if (n.buffImmune[current_add_buff])
				{
					TextColored(Color.Red.ToVector4(), "npc is immune to this buff");
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
						Image(tex.ptr, new Vector2(20, 20));
						SameLine();
						var colo = Main.debuff[n.buffType[i]] ? Color.IndianRed.ToVector4() : Color.Green.ToVector4();
						TextColored(colo, $"{Lang.GetBuffName(n.buffType[i])}({n.buffType[i]}):");
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
		var texture = TextureBinder.npcs[n.type];

		Image(texture.ptr, texture.Transform(100), texture.Uv0(_NpcTextureFrame), texture.Uv1(_NpcTextureFrame));
		Separator();
		SliderInt("Frame", ref _NpcTextureFrame, 1, texture.frames, $"{_NpcTextureFrame} / {texture.frames}");
		Checkbox("Animate", ref AnimateNpcTexture);
		if (AnimateNpcTexture)
		{
			_FrameTimer++;
			_NpcTextureFrame = _FrameTimer / 5 % texture.frames + 1;
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

			InputFloat2("position", ref n.position);

			InputFloat2("velocity", ref n.velocity);

			Vector2 = n.Size;
			InputFloat2("Size (hitbox)", ref Vector2);
			n.Size = Vector2;

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
			Npcs.Disable(n);
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
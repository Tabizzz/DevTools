using DevTools.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DevTools.Editors;

internal class ItemEditor : IGui
{
	public static bool open;
	public static int selected;
	private static Dictionary<int, List<FieldInfo>> fields = new();
	private bool f_public = true;
	private bool f_instance = true;
	private bool f_private;
	private bool f_static;

	public void Gui()
	{
		if (!open) return;
		var p = Main.player[Main.myPlayer];
		ImGuiUtils.SimpleLayout(ref open, ref p.inventory, "Item Editor", ref selected,
		i => !i.IsAir,
		i => i.Name,
		i =>
		{
			TabDescription(i);
			TabDetails(i);
			TabFields(i);
		},
		1,
		i =>
		{
			if(Button("Set Defaults"))
			{
				i.SetDefaults(i.type);
			}
			SameLine();
			if (Button("Remove"))
			{
				i.stack = 0;
			}
		});
	}

	private void TabFields(Item i)
	{
		if (BeginTabItem("Fields"))
		{
			TextWrapped("this is a class field editor, here you can see all the fields of the Item class, this tab is made for testing purposes only, what you change here will not be saved unless it is a property found in the other tabs");
			BindingFlags flags = BindingFlags.Default;

			if (Button("Options"))
				OpenPopup("FieldOptions");

			if(BeginPopup("FieldOptions"))
			{
				MenuItem("Public", null, ref f_public);
				MenuItem("Private", null, ref f_private);
				MenuItem("Instance", null, ref f_instance);
				MenuItem("Static", null, ref f_static);
				EndPopup();
			}

			if (f_public) flags |= BindingFlags.Public;
			if (f_instance) flags |= BindingFlags.Instance;
			if (f_private) flags |= BindingFlags.NonPublic;
			if (f_static) flags |= BindingFlags.Static;

			Separator();
			foreach (var item in i.GetType().GetFields(flags))
			{
				ImGuiUtils.FieldEdit(item, i);
			}
			EndTabItem();
		}
	}

	private void TabDetails(Item i)
	{
		if (BeginTabItem("Details"))
		{
			SliderInt("useStyle", ref i.useStyle, 0, 14);
			Checkbox("useTurn", ref i.useTurn);
			Checkbox("autoReuse", ref i.autoReuse);
			SliderInt("holdStyle", ref i.holdStyle, 0, 6);
			InputInt("useAnimation", ref i.useAnimation);
			InputInt("useTime", ref i.useTime);
			InputInt("reuseDelay", ref i.reuseDelay);

			InputInt("damage", ref i.damage);

			SliderFloat("knockBack", ref i.knockBack, 0, 20);

			InputInt("shoot", ref i.shoot);

			InputFloat("shootSpeed", ref i.shootSpeed);

			Checkbox("noMelee", ref i.noMelee);

			InputInt("defense", ref i.defense);
			InputInt("crit", ref i.crit);

			EndTabItem();
		}
	}

	private void TabDescription(Item i)
	{
		if(BeginTabItem("Description"))
		{
			TextWrapped("Name: " + i.Name);
			SliderInt("stack", ref i.stack, 1, i.maxStack);
			InputInt("maxStack", ref i.maxStack);
			if (i.maxStack < 1) i.maxStack = 1;

			Checkbox("uniqueStack", ref i.uniqueStack);
			Checkbox("favorited", ref i.favorited);

			Checkbox("questItem", ref i.questItem);

			InputInt("value", ref i.value);

			Checkbox("consumable", ref i.consumable);

			if(i.rare < -1)	i.rare = 1 + (-i.rare);
			SliderInt("rare", ref i.rare, -1, ItemRarityID.Count + 2);
			if (i.rare >= ItemRarityID.Count) i.rare = -(i.rare - 1);

			Checkbox("accessory", ref i.accessory);

			Checkbox("noUseGraphic", ref i.noUseGraphic);

			Checkbox("expert", ref i.expert);
			Checkbox("expertOnly", ref i.expertOnly);
			Checkbox("mech", ref i.mech);
			TextWrapped("material: " + i.material);

			GetAndPrintTooltips(i);

			

			EndTabItem();
		}
	}
	private static void GetAndPrintTooltips(Item item)
	{
		if (TreeNode("Tooltips"))
		{
			Item hoverItem = item.Clone();
			int yoyoLogo = -1;
			int researchLine = -1;
			hoverItem.tooltipContext = 0;
			float knockBack = hoverItem.knockBack;
			float num = 1f;
			if (hoverItem.CountsAsClass(DamageClass.Melee) && Main.player[Main.myPlayer].kbGlove)
				num += 1f;

			if (Main.player[Main.myPlayer].kbBuff)
				num += 0.5f;

			if (num != 1f)
				hoverItem.knockBack *= num;

			if (hoverItem.CountsAsClass(DamageClass.Ranged) && Main.player[Main.myPlayer].shroomiteStealth)
				hoverItem.knockBack *= 1f + (1f - Main.player[Main.myPlayer].stealth) * 0.5f;

			int num2 = 30;
			int numLines = 1;
			string[] array = new string[num2];
			bool[] array2 = new bool[num2];
			bool[] array3 = new bool[num2];
			for (int i = 0; i < num2; i++)
			{
				array2[i] = false;
				array3[i] = false;
			}
			string[] tooltipNames = new string[num2];

			Main.MouseText_DrawItemTooltip_GetLinesInfo(hoverItem, ref yoyoLogo, ref researchLine, knockBack, ref numLines, array, array2, array3, tooltipNames);

			List<TooltipLine> lines = ItemLoader.ModifyTooltips(hoverItem, ref numLines, tooltipNames, ref array, ref array2, ref array3, ref yoyoLogo, out Color?[] _);
			foreach (var line in lines)
			{
					TextWrapped(line.Text.Replace("%", "%%"));
			}


			TreePop();
		}

	}
	public void Load(Mod mod)
	{
		var t = typeof(Item);
		var descriptions = new List<FieldInfo>();

		descriptions.Add(t.GetField("stack"));
		descriptions.Add(t.GetField("uniqueStack"));
		descriptions.Add(t.GetField("favorited"));
		descriptions.Add(t.GetField("questItem"));
		descriptions.Add(t.GetField("value"));
		descriptions.Add(t.GetField("consumable"));
		descriptions.Add(t.GetField("rare"));
		descriptions.Add(t.GetField("maxStack"));
		descriptions.Add(t.GetField("accessory"));
		descriptions.Add(t.GetField("noUseGraphic"));
		descriptions.Add(t.GetField("expert"));
		descriptions.Add(t.GetField("expertOnly"));
		descriptions.Add(t.GetField("mech"));

		fields.Add(0, descriptions);

		var details = new List<FieldInfo>();

		details.Add(t.GetField("useStyle"));
		details.Add(t.GetField("useTurn"));
		details.Add(t.GetField("autoReuse"));
		details.Add(t.GetField("holdStyle"));
		details.Add(t.GetField("useAnimation"));
		details.Add(t.GetField("useTime"));
		details.Add(t.GetField("reuseDelay"));
		details.Add(t.GetField("damage"));
		details.Add(t.GetField("knockBack"));
		details.Add(t.GetField("shoot"));
		details.Add(t.GetField("shootSpeed"));
		details.Add(t.GetField("noMelee"));
		details.Add(t.GetField("defense"));
		details.Add(t.GetField("crit"));


		fields.Add(1, details);

		InfoWindow.guis.Add(this);
	}

	public void Unload()
	{
		InfoWindow.guis.Remove(this);
		fields.Clear();
	}

	private class ItemEditorGlobalItem : GlobalItem
	{
		public override void LoadData(Item item, TagCompound tag)
		{
			var ie = tag.GetCompound("ItemEditor");
			foreach (var list in fields)
			{
				foreach (var field in list.Value)
				{
					var name = field.Name;
					if(ie.ContainsKey(name))
					{
						if(field.FieldType == typeof(float))
						{
							field.SetValue(item, ie.GetFloat(name));
						}
						else if (field.FieldType == typeof(int))
						{
							field.SetValue(item, ie.GetInt(name));
						}
						else if (field.FieldType == typeof(bool))
						{
							field.SetValue(item, ie.GetBool(name));
						}
					}
				}
			}
			if (ie.Count > 0)
			{
				item.StatsModifiedBy.Add(Mod);
			}
		}

		public override void SaveData(Item item, TagCompound tag)
		{
			var ie = tag.GetCompound("ItemEditor");
			var def = new Item(item.type);

			foreach (var list in fields)
			{
				foreach (var field in list.Value)
				{
					var name = field.Name;
					
					var f1 = field.GetValue(item);
					var f2 = field.GetValue(def);
					Save(ie, name, f1, f2);
				}
			}

			tag.Set("ItemEditor", ie);
		}

		private void Save(TagCompound tag, string v, object damage1, object damage2)
		{
			if(!damage1.Equals(damage2))
			{
				tag.Set(v, damage1);
			}
		}
	}
}

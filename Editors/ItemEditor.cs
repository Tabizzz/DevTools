using DevTools.CrossMod;
using DevTools.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Default;
using DevTools.Tools;

namespace DevTools.Editors;

internal class ItemEditor : IGui
{
	public static bool Open;
	public static int Selected;
	public static int Selected_global;

	static readonly Dictionary<int, List<FieldInfo>> Fields = new();

	internal static bool f_public = true;

	internal static bool f_instance = true;

	internal static bool f_private;

	internal static bool f_static;

	internal static bool f_editable = true;

	internal static bool f_readonly;

	public void Gui()
	{
		if ( Main.gameMenu || !Open || !HerosModCrossMod.ItemEditor) return;
		var p = Main.player[Main.myPlayer];
		
		ImGuiUtils.SimpleLayout(ref Open, ref p.inventory, "Item Editor", ref Selected,
		i => !i.IsAir,
		i => i.Name,
		i =>
		{
			TabDescription(i);
			TabDetails(i);
			if(i.ModItem is ModItem item)
			{
				TabModItem(item);
			}
			TabGlobals(i);
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

	private void TabGlobals(Item item)
	{
		var l = item.Globals.Length;
		if (l > 0 && BeginTabItem("Global Items"))
		{
			TextWrapped("Choose a global item to see");

			if(BeginCombo("Global Item", item.Globals[Selected_global].Instance.Name))
			{
				for (int i = 0; i < l; i++)
				{
					var current = item.Globals[i].Instance;
					if (Selectable(current.Name, i == Selected_global))
						Selected_global = i;
				}

				EndCombo();
			}
			var global = item.Globals[Selected_global].Instance;

			TextWrapped($"Global item from: {global.Mod.Name}");

			
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
			var t = global.GetType();
			if (global is UnloadedGlobalItem unloaded)
			{
				var data = (List<TagCompound>)t.GetField("data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global);

				TextWrapped("you can see the unloaded data in the tag viewer");
				if(Button("Open in tag viewer"))
					TagViewer.OpenTag(data, t => t.GetString("name"));
			}
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

	private void TabModItem(ModItem mitem)
	{
		if (BeginTabItem("Mod Item"))
		{
			TextWrapped("this is a mod item field editor, here you can see all the fields of the ModItem class, this tab is made for testing purposes only, what you change here will not be saved unless it is a property found in the other tabs");
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
			var t = mitem.GetType();
			foreach (var item in mitem.GetType().GetFields(flags))
			{
				var editable =
					f_editable &&
					(item.FieldType == ImGuiUtils.Bool || item.FieldType == ImGuiUtils.Int || item.FieldType == ImGuiUtils.Int) &&
					!item.IsInitOnly && !item.IsLiteral;

				var readon =
					f_readonly &&
					(item.IsInitOnly || item.IsLiteral);

				if (editable || readon)
					ImGuiUtils.FieldEdit(item, mitem);
			}	
			EndTabItem();
		}
	}

	void TabFields(Item i)
	{
		if (BeginTabItem("Fields"))
		{
			TextWrapped("this is a class field editor, here you can see all the fields of the Item class, this tab is made for testing purposes only, what you change here will not be saved unless it is a property found in the other tabs");
			var flags = BindingFlags.Default;

			if (Button("Options"))
				OpenPopup("FieldOptions");

			if(BeginPopup("FieldOptions"))
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
					(item.FieldType == ImGuiUtils.Bool || item.FieldType == ImGuiUtils.Int	|| item.FieldType == ImGuiUtils.Int) &&
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

	void TabDetails(Item i)
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

	void TabDescription(Item i)
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
	static void GetAndPrintTooltips(Item item)
	{
		if (TreeNode("Tooltips"))
		{
			var hoverItem = item.Clone();
			var yoyoLogo = -1;
			var researchLine = -1;
			hoverItem.tooltipContext = 0;
			var knockBack = hoverItem.knockBack;
			var num = 1f;
			if (hoverItem.CountsAsClass(DamageClass.Melee) && Main.player[Main.myPlayer].kbGlove)
				num += 1f;

			if (Main.player[Main.myPlayer].kbBuff)
				num += 0.5f;

			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (num != 1f)
				hoverItem.knockBack *= num;

			if (hoverItem.CountsAsClass(DamageClass.Ranged) && Main.player[Main.myPlayer].shroomiteStealth)
				hoverItem.knockBack *= 1f + (1f - Main.player[Main.myPlayer].stealth) * 0.5f;

			var num2 = 30;
			var numLines = 1;
			var array = new string[num2];
			var array2 = new bool[num2];
			var array3 = new bool[num2];
			for (var i = 0; i < num2; i++)
			{
				array2[i] = false;
				array3[i] = false;
			}
			var tooltipNames = new string[num2];

			Main.MouseText_DrawItemTooltip_GetLinesInfo(hoverItem, ref yoyoLogo, ref researchLine, knockBack, ref numLines, array, array2, array3, tooltipNames);

			var lines = ItemLoader.ModifyTooltips(hoverItem, ref numLines, tooltipNames, ref array, ref array2, ref array3, ref yoyoLogo, out var _);
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

		descriptions.Add(t.GetField("uniqueStack"));
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

		Fields.Add(0, descriptions);

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


		Fields.Add(1, details);

		InfoWindow.Guis.Add(this);
	}

	public void Unload()
	{
		InfoWindow.Guis.Remove(this);
		Fields.Clear();
	}

	class ItemEditorGlobalItem : GlobalItem
	{
		public override void LoadData(Item item, TagCompound tag)
		{
			var ie = tag.GetCompound("ItemEditor");

			foreach (var list in Fields)
			{
				foreach (var field in list.Value)
				{
					var name = field.Name;
					if(ie.ContainsKey(name))
					{						
						if (field.FieldType == typeof(float))
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

			foreach (var list in Fields)
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

		void Save(TagCompound tag, string v, object damage1, object damage2)
		{
			if(!damage1.Equals(damage2))
			{
				tag.Set(v, damage1);
			}
		}
	}
}

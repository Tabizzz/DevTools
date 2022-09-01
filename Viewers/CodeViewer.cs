using ImGuiNET;
using ImGuiNET.Extras;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace DevTools.Viewers;

public class CodeViewer : IGui
{
	public static bool Open;

	public static Dictionary<string, TextEditor> editors;
	public static Dictionary<string, string> names;

	internal static void Add(string name, string v, string code)
	{
		Open = true;
		if(editors.ContainsKey(v))
		{
			editors[v].Text = code;
			return;
		}
		var editor = new TextEditor();
		editor.Text = code.Replace("\\n", $"\\n\" + {Environment.NewLine}\"");
		editor.ReadOnly = true;
		editors.Add(v, editor);
		names.Add(v, name);
	}

	public void Gui()
	{
		if(!Open || editors.Count == 0) { return; }

		SetNextWindowSize(Vector2.One * 500, ImGuiCond.FirstUseEver);
		if(!Begin("Code Viewer"))
		{
			End();
			return;
		}

		if(BeginTabBar("##tabs", ImGuiTabBarFlags.AutoSelectNewTabs | ImGuiTabBarFlags.TabListPopupButton))
		{
			var i = 0;
			foreach (var item in editors.ToArray())
			{
				var name = names[item.Key];
				var open = true;
				PushID(item.Key);
				if(BeginTabItem(name, ref open))
				{
					var pos = item.Value.CursorPosition;
					Text($"{pos.mLine,6}:{pos.mColumn,-6} {item.Value.TotalLines,6} lines  | C# | {item.Key}");
					PushFont(GetIO().Fonts.Fonts[0]);
					item.Value.Render(name);
					PopFont();
					EndTabItem();
				}
				PopID();
				if(!open)
				{
					editors.Remove(item.Key);
					names.Remove(item.Key);
					item.Value.Dispose();
				}
			}
			EndTabBar();
		}
		End();
	}

	public void Load(Mod mod)
	{
		InfoWindow.Guis.Add(this);
		editors = new();
		names = new();
	}

	public void Unload()
	{
		InfoWindow.Guis.Remove(this);
		foreach (var item in editors)
		{
			item.Value.Dispose();
		}
		editors.Clear();
		names.Clear();
	}
}

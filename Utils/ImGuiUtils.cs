using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace DevTools.Utils;

public class ImGuiUtils
{
	public static readonly Type Float = typeof(float);
	private static readonly Type Bool = typeof(bool);
	private static readonly Type Int = typeof(int);

	public static void FieldEdit(FieldInfo field, object target = null)
	{
		PushItemWidth(150);

		if(field.IsInitOnly || field.IsLiteral)
		{
			TextWrapped(field.Name + ": " + field.GetValue(target));
		}
		else if(field.FieldType == Float)
		{
			var Ref = (float)field.GetValue(target);
			InputFloat(field.Name, ref Ref);
			field.SetValue(target, Ref);
		}
		else if (field.FieldType == Bool)
		{
			var Ref = (bool)field.GetValue(target);
			Checkbox(field.Name, ref Ref);
			field.SetValue(target, Ref);

		}
		else if (field.FieldType == Int)
		{
			var Ref = (int)field.GetValue(target);
			InputInt(field.Name, ref Ref);
			field.SetValue(target, Ref);

		}
		else
		{
			PopItemWidth();
			TextWrapped(field.Name + ": " + field.GetValue(target));
		}
		
	}

	public static void Overlay(ref int corner, ref bool open, Action content)
	{
		var window_flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav;

		if (corner != -1)
		{
			const float PAD = 10.0f;
			var viewport = GetMainViewport();
			var work_pos = viewport.WorkPos; // Use work area to avoid menu-bar/task-bar, if any!
			var work_size = viewport.WorkSize;
			ImVect2 window_pos = default, window_pos_pivot = default;
			window_pos.X = ((corner & 1) == 1) ? (work_pos.X + work_size.X - PAD) : (work_pos.X + PAD);
			window_pos.Y = ((corner & 2) == 2) ? (work_pos.Y + work_size.Y - PAD) : (work_pos.Y + PAD);
			window_pos_pivot.X = ((corner & 1) == 1) ? 1.0f : 0.0f;
			window_pos_pivot.Y = ((corner & 2) == 2) ? 1.0f : 0.0f;
			SetNextWindowPos(window_pos, ImGuiCond.FirstUseEver, window_pos_pivot);
			SetNextWindowViewport(viewport.ID);
			window_flags |= ImGuiWindowFlags.NoMove;
		}
		SetNextWindowBgAlpha(0.35f); // Transparent background

		if (Begin("Actives overlay", ref open, window_flags))
		{
			
			content();

			if (BeginPopupContextWindow())
			{
				if (MenuItem("Custom", null, corner == -1)) corner = -1;
				if (MenuItem("Top-left", null, corner == 0)) corner = 0;
				if (MenuItem("Top-right", null, corner == 1)) corner = 1;
				if (MenuItem("Bottom-left", null, corner == 2)) corner = 2;
				if (MenuItem("Bottom-right", null, corner == 3)) corner = 3;
				if (open && MenuItem("Close")) open = false;
				EndPopup();
			}
		}
		End();
	}

	public static void SimpleLayout<T>(ref bool open, ref T[] npc, string name, ref int selected, Func<T, bool> active, Func<T, string> display, Action<T> tabs, int buttonLines, Action<T> buttons)
	{
		int select = selected;
		SetNextWindowSize(new ImVect2(500, 500), ImGuiCond.FirstUseEver);
		if (Begin($"{name}", ref open, ImGuiWindowFlags.MenuBar))
		{
			if (BeginMenuBar())
			{
				if (BeginMenu("Options"))
				{
					if (MenuItem("Close")) open = false;
					EndMenu();
				}
				EndMenuBar();
			}

			{
				BeginChild("left pane", new ImVect2(200, 0), true);
				for (int i = 0; i < npc.Length - 1; i++)
				{
					if (active(npc[i]))
						if (Selectable($"{display(npc[i])}({i})", selected == i))
							selected = i;
				}

				EndChild();
			}
			SameLine();
			if(selected >= 0 && selected < npc.Length - 1 && active(npc[selected]))
			// Right
			{
				BeginGroup();
				select = selected;

				BeginChild("item view", new ImVect2(0, -GetFrameHeightWithSpacing() * buttonLines));
				Text($"{name}: {select}");
				Separator();
				if (BeginTabBar("##Tabs", ImGuiTabBarFlags.TabListPopupButton))
				{
					tabs(npc[select]);
				}

				EndChild();
				buttons(npc[select]);
				EndGroup();
			}
			else
			{
				TextWrapped("Choose an item from the left panel");
			}
		}
		End();
	}

	public static void VectorWrapped(string v, Microsoft.Xna.Framework.Vector2 position, bool Length = false)
	{
		TextWrapped($"{v}: ");
		Indent();

		TextWrapped("X: ");
		SameLine();
		TextWrapped(position.X.ToString());

		TextWrapped("Y: ");
		SameLine();
		TextWrapped(position.Y.ToString());

		if (Length)
		{
			TextWrapped("Length: ");
			SameLine();
			TextWrapped(position.Length().ToString());
		}

		Unindent();
	}
}

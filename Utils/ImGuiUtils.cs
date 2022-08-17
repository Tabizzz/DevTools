using ImGuiNET;
using System;
using System.Globalization;
using System.Reflection;

namespace DevTools.Utils;

public class ImGuiUtils
{
	internal static readonly Type Float = typeof(float);
	internal static readonly Type Bool = typeof(bool);
	internal static readonly Type Int = typeof(int);

	public static void FieldEdit(FieldInfo field, object target = null)
	{
		PushItemWidth(150);

		if(field.IsInitOnly || field.IsLiteral)
		{
			TextWrapped(field.Name + ": " + field.GetValue(target));
		}
		else if(field.FieldType == Float)
		{
			var @ref = (float)field.GetValue(target)!;
			InputFloat(field.Name, ref @ref);
			field.SetValue(target, @ref);
		}
		else if (field.FieldType == Bool)
		{
			var @ref = (bool)field.GetValue(target)!;
			Checkbox(field.Name, ref @ref);
			field.SetValue(target, @ref);

		}
		else if (field.FieldType == Int)
		{
			var @ref = (int)field.GetValue(target)!;
			InputInt(field.Name, ref @ref);
			field.SetValue(target, @ref);

		}
		else
		{
			PopItemWidth();
			TextWrapped(field.Name + ": " + field.GetValue(target));
		}
		
	}

	public static void Overlay(ref int corner, ref bool open, Action content)
	{
		var windowFlags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav;

		if (corner != -1)
		{
			const float pad = 10.0f;
			var viewport = GetMainViewport();
			var workPos = viewport.WorkPos; // Use work area to avoid menu-bar/task-bar, if any!
			var workSize = viewport.WorkSize;
			ImVect2 windowPos = default, windowPosPivot = default;
			windowPos.X = ((corner & 1) == 1) ? (workPos.X + workSize.X - pad) : (workPos.X + pad);
			windowPos.Y = ((corner & 2) == 2) ? (workPos.Y + workSize.Y - pad) : (workPos.Y + pad);
			windowPosPivot.X = ((corner & 1) == 1) ? 1.0f : 0.0f;
			windowPosPivot.Y = ((corner & 2) == 2) ? 1.0f : 0.0f;
			SetNextWindowPos(windowPos, ImGuiCond.FirstUseEver, windowPosPivot);
			SetNextWindowViewport(viewport.ID);
			windowFlags |= ImGuiWindowFlags.NoMove;
		}
		SetNextWindowBgAlpha(0.35f); // Transparent background

		if (Begin("Actives overlay", ref open, windowFlags))
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

	public static void SimpleLayout<T>(ref bool open, ref T[] npc, string name, ref int selected, Func<T, bool> active, Func<T, string> display, Action<T> tabs, int buttonLines, Action<T> buttons, Action Options = null)
	{
		SetNextWindowSize(new ImVect2(500, 500), ImGuiCond.FirstUseEver);
		if (Begin($"{name}", ref open, ImGuiWindowFlags.MenuBar))
		{
			if (BeginMenuBar())
			{
				if (BeginMenu("Options"))
				{
					if (Options != null) Options();

					if (MenuItem("Close")) open = false;
					EndMenu();
				}
				EndMenuBar();
			}

			{
				BeginChild("left pane", new ImVect2(200, 0), true);
				for (var i = 0; i < npc.Length - 1; i++)
				{
					if (active(npc[i]) && Selectable($"{display(npc[i])}({i})", selected == i))
						selected = i;
				}

				EndChild();
			}
			SameLine();
			if(selected >= 0 && selected < npc.Length - 1 && active(npc[selected]))
			// Right
			{
				BeginGroup();
				var select = selected;

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

	public static void VectorWrapped(string v, Vector2 position, bool length = false)
	{
		TextWrapped($"{v}: ");
		Indent();

		TextWrapped("X: ");
		SameLine();
		TextWrapped(position.X.ToString(CultureInfo.InvariantCulture));

		TextWrapped("Y: ");
		SameLine();
		TextWrapped(position.Y.ToString(CultureInfo.InvariantCulture));

		if (length)
		{
			TextWrapped("Length: ");
			SameLine();
			TextWrapped(position.Length().ToString(CultureInfo.InvariantCulture));
		}

		Unindent();
	}
}

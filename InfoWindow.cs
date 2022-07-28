using ImGUI;
using ImGuiNET;
using System.Collections.Generic;

namespace DevTools;

internal class InfoWindow : ModImGui
{
	public static List<IGui> debugs = new();
	public static List<IGui> guis = new();
	public static List<IGui<ImDrawListPtr>> backs = new();
	public static List<IGui<ImDrawListPtr>> fronts = new();
	public static List<object> reports = new();

	public override void DebugGUI()
	{
		if (!BeginChild(Mod.Name)) return;
		if (!CollapsingHeader(Mod.DisplayName)) return;

		debugs.ForEach(d=>d.Gui());

		lock (reports)
		{
			if (Button("Clear Reports"))
			{
				reports.Clear();
			}

			foreach (var item in reports)
			{
				if(item is string str)
				{
					TextWrapped(str);
				}
				else if(item is int n)
				{
					if(n == 1)
					{ Indent(); }
					else if (n == -1)
					{ Unindent(); }
				}
			}
		}

		EndChild();
	}

	public static void AddReport(object obj)
	{
		lock(reports)
		{
			reports.Add(obj);
		}
	}

	public override void CustomGUI()
	{
		guis.ForEach(d => d.Gui());
	}

	public override void BackgroundDraw(ImDrawListPtr drawList)
	{
		backs.ForEach(d=> d.Gui(drawList));
	}

	public override void ForeroundDraw(ImDrawListPtr drawList)
	{
		fronts.ForEach(d => d.Gui(drawList));
	}
}

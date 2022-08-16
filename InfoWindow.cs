using DevTools.CrossMod;
using ImGUI;
using ImGuiNET;
using System.Collections.Generic;
using System.Diagnostics;

namespace DevTools;

internal class InfoWindow : ModImGui
{
	public static List<IGui> Debugs = new();
	public static List<IGui> Guis = new();
	public static List<IGui<ImDrawListPtr>> Backs = new();
	public static List<IGui<ImDrawListPtr>> Fronts = new();
	public static List<object> Reports = new();

	public override void DebugGUI()
	{
		if (!BeginChild(Mod.Name)) return;
		if (!CollapsingHeader(Mod.DisplayName)) return;

		Debugs.ForEach(d=>d.Gui());

		if (HerosModCrossMod.ServerLogs)
			Checkbox("Server Logs", ref ServerAppLog.Open);

		if (Debugger.IsAttached)
		{
			if(HerosModCrossMod.ServerLogs) SameLine();
			lock (Reports)
			{
				if (Button("Clear Reports"))
				{
					Reports.Clear();
				}

				foreach (var item in Reports)
				{
					if (item is string str)
					{
						TextWrapped(str);
					}
					else if (item is int n)
					{
						if (n == 1)
						{ Indent(); }
						else if (n == -1)
						{ Unindent(); }
					}
				}
			}
		}

		EndChild();
	}

	public static void AddReport(object obj)
	{
		lock(Reports)
		{
			Reports.Add(obj);
		}
	}

	public override void CustomGUI()
	{
		Guis.ForEach(d => d.Gui());
	}

	public override void BackgroundDraw(ImDrawListPtr drawList)
	{
		Backs.ForEach(d=> d.Gui(drawList));
	}

	public override void ForeroundDraw(ImDrawListPtr drawList)
	{
		Fronts.ForEach(d => d.Gui(drawList));
	}
}

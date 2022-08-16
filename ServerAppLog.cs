using DevTools.CrossMod;
using ImGUI;
using ImGuiNET;
using System.Collections.Generic;


namespace DevTools;

internal class ServerAppLog : ModImGui
{
	public static bool Open;

	static bool _AutoScrollLogs = true;
	internal static int position;
	internal static readonly List<string> Logs = new();

	public override void CustomGUI()
	{
		if(!HerosModCrossMod.ServerLogs) Open = false;

		if (!Open || !Begin("Server Logs", ref Open))
			return;

		// Options menu
		if (BeginPopup("LogOptions"))
		{
			Checkbox("Auto-scroll", ref _AutoScrollLogs);
			EndPopup();
		}

		if (Button("Options"))
			OpenPopup("LogOptions");

		SameLine();
		var clear = Button("Clear");
		SameLine();
		var copy = Button("Copy");
		SameLine();
		var getLog = Button("Request Logs");

		Separator();
		BeginChild("scrolling", ImVect2.Zero, false, ImGuiWindowFlags.HorizontalScrollbar);

		if (clear)
		{
			Logs.Clear();
			position = 0;
		}
		if (copy)
			LogToClipboard();

		PushStyleVar(ImGuiStyleVar.ItemSpacing, ImVect2.Zero);

		foreach (var log in Logs.ToArray())
		{
			TextUnformatted(log);
		}

		PopStyleVar();

		if (_AutoScrollLogs && GetScrollY() >= GetScrollMaxY())
			SetScrollHereY(1.0f);

		EndChild();

		End();
		if (getLog)
			AppLogs.Request(position);
		
	}
}

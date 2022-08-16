global using static ImGuiNET.ImGui;
using DevTools.CrossMod;
using Terraria.ModLoader;

namespace DevTools;

public class DevTools : Mod
{
	public override void Load()
	{
		if (!ImGUI.ImGUI.CanGui)
		{
			log4net.Config.BasicConfigurator.Configure(new ServerLogAppender());
		}
	}
	public override void PostSetupContent()
	{
		HerosModCrossMod.AddPermissions();
	}
}
global using static ImGuiNET.ImGui;
using DevTools.CrossMod;
using Terraria.ModLoader;

namespace DevTools;

public class DevTools : Mod
{
	public override void PostSetupContent()
	{
		HerosModCrossMod.AddPermissions();
	}
}
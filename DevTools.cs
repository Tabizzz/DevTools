global using static ImGuiNET.ImGui;
using DevTools.CrossMod;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace DevTools;

public class DevTools : Mod
{
	public static readonly string PreferenceFolder = Path.Combine(Main.SavePath, "config");
	public static readonly string PreferencePath = Path.Combine(PreferenceFolder, "devtools.json");
	internal static DevTools Instance;

	public static ModKeybind EntityCodeKey;

	public override void Load()
	{
		Instance = this;
		EntityCodeKey = KeybindLoader.RegisterKeybind(this, "Show entity code", Keys.F2);
		if (!ImGUI.ImGUI.CanGui)
		{
			log4net.Config.BasicConfigurator.Configure(new ServerLogAppender());
		}
		else
		{
			if(File.Exists(PreferencePath))
			{
				JsonConvert.DeserializeObject<Preferences>(File.ReadAllText(PreferencePath));
			}
			Main.instance.Exiting += Instance_Exiting;
		}
	}

	private void Instance_Exiting(object sender, System.EventArgs e)
	{
		Unload();
	}

	public override void PostSetupContent()
	{
		HerosModCrossMod.AddPermissions();
	}

	public override void Unload()
	{
		Instance = null;
		EntityCodeKey = null;
		var json = JsonConvert.SerializeObject(new Preferences());

		Directory.CreateDirectory(PreferenceFolder);
		File.WriteAllText(PreferencePath, json);
	}
}
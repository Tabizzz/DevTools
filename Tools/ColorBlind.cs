using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DevTools.Tools;

internal class ColorBlind : IGui
{
	internal static bool Open = true;

	internal static Vector3 topRow = Vector3.UnitX;
	internal static Vector3 midRow = Vector3.UnitY;
	internal static Vector3 botRow = Vector3.UnitZ;

	public void Gui()
	{
		if (!Open) return;
		if (Begin("ColorBlind", ref Open))
		{
			TextWrapped("This is a color blindness simulation tool, you can change the transformation matrix to get different results");
			var inuse = Filters.Scene["DevTools:ColorBlind"].IsInUse();
			Checkbox("Is in use", ref inuse);
			if(Button("Activate"))
			{
				Filters.Scene["DevTools:ColorBlind"].GetShader().UseColor(Color.White);
				Filters.Scene.Activate("DevTools:ColorBlind", Main.LocalPlayer.position);
			}
			SameLine();
			if (Button("Deactivate"))
			{
				Filters.Scene["DevTools:ColorBlind"].Deactivate();
			}
			Separator();
			Text("Transformation matrix");
			InputFloat3("##top", ref topRow);
			InputFloat3("##mid", ref midRow);
			InputFloat3("##row", ref botRow);
			if(Button("Apply"))
			{
				SetShader();
			}
			Separator();
			Text("Sample matrices");
			if(Button("Protanope"))
			{
				topRow = Vector3.UnitY * 2.02344f + Vector3.UnitZ * -2.52581f;
				midRow = Vector3.UnitY;
				botRow = Vector3.UnitZ;
				SetShader();
			}
			SameLine();
			TextWrapped("reds are greatly reduced (1%% men)");
			if (Button("Deuteranope"))
			{
				midRow = Vector3.UnitX * 0.494207f + Vector3.UnitZ * 1.24827f;
				topRow = Vector3.UnitX;
				botRow = Vector3.UnitZ;
				SetShader();
			}
			SameLine();
			TextWrapped("greens are greatly reduced (1%% men)");
			if (Button("Tritanope"))
			{
				botRow = Vector3.UnitX * -0.395913f + Vector3.UnitY * 0.801109f;
				topRow = Vector3.UnitX;
				midRow = Vector3.UnitY;
				SetShader();
			}
			SameLine();
			TextWrapped("blues are greatly reduced (0.003%% population)");
		}
		End();
	}

	public void Load(Mod mod)
	{
		if(Main.netMode is not NetmodeID.Server)
		{
			var newEffect = new Ref<Effect>(mod.Assets.Request<Effect>("fx/ColorBlind", AssetRequestMode.ImmediateLoad).Value);

			Filters.Scene["DevTools:ColorBlind"] = new Filter(new ScreenShaderData(newEffect, "ColorBlind"), EffectPriority.VeryHigh);
			topRow = Vector3.UnitX;
			midRow = Vector3.UnitY;
			botRow = Vector3.UnitZ;
			SetShader();
		}
		InfoWindow.Guis.Add(this);
	}

	private static void SetShader()
	{
		var shader = Filters.Scene["DevTools:ColorBlind"].GetShader();
		
		shader.Shader.Parameters["topRow"].SetValue(topRow);
		shader.Shader.Parameters["midRow"].SetValue(midRow);
		shader.Shader.Parameters["botRow"].SetValue(botRow);
	}

	public void Unload()
	{
		InfoWindow.Guis.Remove(this);
		if (Main.netMode is not NetmodeID.Server)
		{

		}
	}
}
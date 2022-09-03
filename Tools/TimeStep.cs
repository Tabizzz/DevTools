using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace DevTools.Tools;

public class TimeStep : IGui
{
	public static bool Open;
	internal static bool worldPause;
	internal static bool projPause;
	private static int stepTicks;
	static int currentSteps = 0;

	public void Gui()
	{
		if(!Open) return;

		if (Begin("Time Step", ref Open))
		{
			InputInt("Step ticks", ref stepTicks);
			if (stepTicks < 1)
				stepTicks = 1;
			if (Button("Step"))
				TimeStepNet.DoStep(stepTicks);
			Separator();
			var pre = worldPause;
			Checkbox("Pause world update", ref pre);
			if(pre != worldPause)
			{
				TimeStepNet.WorldPause(pre);
			}
			pre = projPause;
			Checkbox("Pause projectile update", ref pre);
			if (pre != projPause)
			{
				TimeStepNet.ProjPause(pre);
			}
		}
		End();
	}

	public static void DoStep(int numSteps)
	{
		if (currentSteps < 0) currentSteps = 0;
		currentSteps += numSteps;
	}

	public void Load(Mod mod)
	{
		InfoWindow.Guis.Add(this);
		On.Terraria.Main.DoUpdateInWorld += Main_DoUpdateInWorld;
		IL.Terraria.Main.DoUpdateInWorld += Main_DoUpdateInWorld1;
	}

	private void Main_DoUpdateInWorld1(MonoMod.Cil.ILContext il)
	{
		var c = new ILCursor(il);
		var projL = c.DefineLabel();

		if(c.TryGotoNext(MoveType.Before, i=>i.MatchCall(typeof(SystemLoader), nameof(SystemLoader.PreUpdateProjectiles))))
		{
			c.EmitDelegate(() => projPause && currentSteps < 0);
			c.Emit(OpCodes.Brtrue_S, projL);
			if (c.TryGotoNext(MoveType.After, i => i.MatchCall(typeof(SystemLoader), nameof(SystemLoader.PostUpdateProjectiles))))
			{
				c.MarkLabel(projL);
			}
		}
	}

	private void Main_DoUpdateInWorld(On.Terraria.Main.orig_DoUpdateInWorld orig, Terraria.Main self, System.Diagnostics.Stopwatch sw)
	{
		currentSteps--;
		if(!worldPause || currentSteps >= 0)
		{
			orig(self, sw);
		}
	}

	public void Unload()
	{
		InfoWindow.Guis.Remove(this);
		On.Terraria.Main.DoUpdateInWorld -= Main_DoUpdateInWorld;
		IL.Terraria.Main.DoUpdateInWorld -= Main_DoUpdateInWorld1;

	}
}

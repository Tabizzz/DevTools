using DevTools.Tools;
using KokoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace DevTools.Nets;

public interface ITimeStep
{
	void DoStep(int stepTicks);
	void ProjPause(bool pre);
	void WorldPause(bool pre);

	private class Impl : ModHandler<ITimeStep>, ITimeStep
	{
		public override ITimeStep Handler => this;

		public void DoStep(int stepTicks)
		{
			TimeStep.DoStep(stepTicks);

			if (Main.netMode == NetmodeID.Server)
			{
				TimeStepNet.DoStep(stepTicks);
			}
		}

		public void ProjPause(bool pre)
		{
			TimeStep.projPause = pre;

			if (Main.netMode == NetmodeID.Server)
			{
				TimeStepNet.ProjPause(pre);
			}
		}

		public void WorldPause(bool pre)
		{
			TimeStep.worldPause = pre;

			if (Main.netMode == NetmodeID.Server)
			{
				TimeStepNet.WorldPause(pre);
			}
		}
	}
}

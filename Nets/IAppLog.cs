using DevTools.CrossMod;
using KokoLib;
using Terraria;
using Terraria.ID;

namespace DevTools.Nets;

public interface IAppLog
{
	public void Request(int start);
	public void Logs(string[] logs);

	private class Impl : ModHandler<IAppLog>, IAppLog
	{
		public override IAppLog Handler => this;

		public void Logs(string[] logs)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				ServerAppLog.Logs.AddRange(logs);
				ServerAppLog.position += logs.Length;
			}
		}

		public void Request(int start)
		{
			if(Main.netMode == NetmodeID.Server && start < ServerAppLog.Logs.Count)
			{
				// send the response to the same client
				Net.ToClient = WhoAmI;
				var canSend = true;

				if(HerosModCrossMod.HerosModAvaliable)
				{
					canSend = (bool)HerosModCrossMod.HerosMod.Call("HasPermission", WhoAmI, HerosModCrossMod.ServerLogPermission);
				}
				if (canSend)
					AppLogNet.Logs(ServerAppLog.Logs.ToArray()[start..]);
				else
					AppLogNet.Logs(new string[] { "You dont have permission to get server logs" });
			}
		}
	}
}

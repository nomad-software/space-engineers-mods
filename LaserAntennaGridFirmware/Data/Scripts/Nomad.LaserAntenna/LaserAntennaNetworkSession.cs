using System;
using System.Collections.Generic;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.Input;
using VRage.Utils;

namespace Nomad.LaserAntennaGridFirmware
{
	[MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
	public class LaserAntennaNetworkSession : MySessionComponentBase
	{
		internal static readonly ushort CHANNEL_ID = 20614;

		protected List<IMyPlayer> players = null;

		public override void BeforeStart()
		{
			MyAPIGateway.Multiplayer.RegisterMessageHandler(LaserAntennaNetworkSession.CHANNEL_ID, this.ReceivePacket);
			MyLog.Default.WriteLineAndConsole(String.Format(">>>> Laser antenna grid firmware network registered"));
		}

		protected override void UnloadData()
		{
			MyAPIGateway.Multiplayer.UnregisterMessageHandler(LaserAntennaNetworkSession.CHANNEL_ID, this.ReceivePacket);
			MyLog.Default.WriteLineAndConsole(String.Format(">>>> Laser antenna grid firmware network unregistered"));
		}

		protected void ReceivePacket(byte[] packet)
		{
			try
			{
				var settings = MyAPIGateway.Utilities.SerializeFromBinary<LaserAntennaSettings>(packet);

				IMyLaserAntenna antenna = (IMyLaserAntenna) MyAPIGateway.Entities.GetEntityById(settings.NetworkLaserAntennaId);
				if (antenna == null)
				{
					return;
				}

				LaserAntennaGridFirmware logic = antenna.GameLogic.GetAs<LaserAntennaGridFirmware>();
				if (logic != null)
				{
					logic.Settings.ShowLaser = settings.ShowLaser;
					logic.Settings.LaserColor = settings.LaserColor;
					logic.Settings.GroupGridOnConnect = settings.GroupGridOnConnect;
				}

				LaserAntennaGridFirmware targetlogic = logic.GetTargetLogic();
				if (targetlogic != null)
				{
					targetlogic.Settings.ShowLaser = settings.ShowLaser;
					targetlogic.Settings.LaserColor = settings.LaserColor;
					targetlogic.Settings.GroupGridOnConnect = settings.GroupGridOnConnect;
				}

				this.relayPacketToOtherPlayers(packet, settings.NetworkSenderId);
			}
			catch(Exception e)
			{
				MyLog.Default.WriteLineAndConsole(String.Format("Exception: {0}", e.Message));
				MyLog.Default.WriteLineAndConsole(String.Format("StackTrace: {0}", e.StackTrace));
			}
		}

		protected void relayPacketToOtherPlayers(byte[] packet, ulong sender)
		{
			if (! MyAPIGateway.Multiplayer.IsServer)
			{
				return; // Don't relay if you're not the server.
			}

			if(this.players == null)
			{
				this.players = new List<IMyPlayer>(MyAPIGateway.Session.SessionSettings.MaxPlayers);
			}
			else
			{
				this.players.Clear();
			}

			MyAPIGateway.Players.GetPlayers(this.players);

			foreach(IMyPlayer player in this.players)
			{
				if(player.IsBot)
				{
					continue;
				}

				if(player.SteamUserId == MyAPIGateway.Multiplayer.ServerId)
				{
					continue;
				}

				if(player.SteamUserId == sender)
				{
					continue;
				}

				MyAPIGateway.Multiplayer.SendMessageTo(LaserAntennaNetworkSession.CHANNEL_ID, packet, player.SteamUserId);
			}

			this.players.Clear();
		}
	}
}

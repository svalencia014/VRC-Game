using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace VRC_Game.Server
{
	public class FSDServer
	{
		public static TcpClient Client;
		public static StreamWriter writer;
		public static string Callsign;

		//define events
		public static event EventHandler<ConnectedEventArgs> Connected;
		public event EventHandler<FlightPlanRequestedEventArgs> FlightPlanRequested;
		public event EventHandler<MessageSentEventArgs> MessageSent;
		public event EventHandler<AtcUpdatedEventArgs> AtcUpdated;
		public event EventHandler<AtcLoggedOffEventArgs> AtcLoggedOff;
		
		public async void Start()
		{
			TcpListener server = new(IPAddress.Any, 6809);
			server.Start();
			await AcceptAndProcess(server);
			
		}
		
		public async Task AcceptAndProcess(TcpListener server)
		{
			while (true)
			{
				Client = server.AcceptTcpClient();
				try
				{
					var stream = Client.GetStream();
					StreamReader reader = new(stream);
					writer = new(stream) { AutoFlush = true };
					await Send($"$DISERVER:CLIENT:VATSIM FSD V3.13:3ef36a24");
					
					while (true)
					{
						var info = await reader.ReadLineAsync();

						if (await ProcessLine(info))
						{
							break;
						}
					}
				} catch (IOException)
				{
					Disconnect();
				}
			}
		}

		public static async Task Send(string message)
		{
			if (writer != null)
			{
				await writer.WriteLineAsync(message);
			}
		}

		public static async Task Disconnect()
		{
			writer = null;

		}

		public async Task<bool> ProcessLine(string info)
		{
			if (info == null) throw new ArgumentNullException(nameof(info));
			
			if (info.StartsWith("%" + Callsign))
			{
				//Position update
			}

			if (info.StartsWith($"$DA{Callsign}:SERVER"))
			{
				Disconnect();

				return true;
			}
			
			if (info.StartsWith("#AM"))
			{
				//Modify flight plan
			}

			if (info.StartsWith("#AP"))
			{
				//Not going to happen ever
				// I will pay you $100 if this happens
				//AP = New Pilot Connection
				//No ATC Client should send AP Command
			}

			if (info.StartsWith("$AX"))
			{
				//Get METAR
				
			}
			
			if (info.StartsWith("$CQ"))
			{
				//$CQJAX_GND:JAX_TWR:RN - Real Name
				//Expect: $CRJAX_TWR_JAX_GND:RN:<Name>:<location><rating>

				//$CQJAX_GND:@21900:VC:AF1:2677 - Set Squawk
				//$CQJAX_GND:SERVER:FP:AF1 - Request FPLAN
				
				//$CQJAX_GND:@21900:DR:AF1 - Assume/Track Callsign
				//#TMJAX_GND:FP:AF1 - Release/Drop Callsign
			}

			if (info.StartsWith("$CR"))
			{
				//idk what does does
			}
			
			if (info.StartsWith("$HO") || info.StartsWith("#PC"))
			{
				//$HOMCO_E_APP:MCO_E_TWR:AF1 - Hand off
				//#PCMCO_E_APP:MCO_E_TWR:CCP:ST:AF1:1::::::::: - Point Out
			}

			if (info.StartsWith($"$TM{Callsign}"))
			{
				//#TMJAX_GND:@21900:Hello - Send Message
			}

			if (info.StartsWith("#AA"))
			{
				//ATC Login
				var tokens = info.Substring("#AA".Length).Split(':');
				Callsign = tokens[0];
				var to = tokens[1];
				var realName = tokens[2];
				var certificate = tokens[3];
				var password = tokens[4];
				var rating = tokens[5];
				var lat = tokens.Length > 9 ? double.Parse(tokens[9]) : (double?)null;
				var lng = tokens.Length > 10 ? double.Parse(tokens[10]) : (double?)null;

				await Send($"$TMVRCGame:{Callsign}:Connected");

				Connected?.Invoke(this, new(Callsign, realName, certificate, rating, lat, lng));
			}			
			
			return false;
		}
	}
}
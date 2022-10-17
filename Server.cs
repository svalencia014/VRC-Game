using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using VRC_Game.Controllers;

namespace VRC_Game.Server
{
	public class FSDServer
	{
		public static TcpClient Client;
		public static StreamWriter writer;
		public static string Callsign;
		public static StreamWriter logger;

		//define events
		public static event EventHandler<ConnectedEventArgs> Connected;
		public static event EventHandler<FlightPlanRequestedEventArgs> FlightPlanRequested;
		public static event EventHandler<MessageSentEventArgs> MessageSent;
		public static event EventHandler<AtcMessageSentEventArgs> AtcMessageSent;
        public static event EventHandler<AtcUpdatedEventArgs> AtcUpdated;
		public static event EventHandler<AtcLoggedOffEventArgs> AtcLoggedOff;
		
		public async void Start(string path)
		{
			TcpListener server = new(IPAddress.Any, 6809);
			//Setup log file
			LogFile.Create();
			server.Start();
            LogFile.Log("Server Started");            

            await AcceptAndProcess(server);
			
		}

		public Task AcceptAndProcess(TcpListener server)
		{
			while (true)
			{
				Client = server.AcceptTcpClient();
				Task.Run(() => HandleConnection(Client));
            }
		}


		public async void HandleConnection(TcpClient client)
		{
			var stream = client.GetStream();
			StreamReader reader = new(stream);
			writer = new(stream) { AutoFlush = true };
			await Send($"$DISERVER:CLIENT:VATSIM FSD V3.13:3ef36a24");
			LogFile.Log("Connected!");

			while (true)
			{
				var info = await reader.ReadLineAsync();

				if (await ProcessLine(info))
				{
					break;
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

		public void Disconnect()
		{
			writer = null;
			LogFile.Log("Disconnected!");

			AtcLoggedOff?.Invoke(this, new(Callsign));
		}

		public async Task<bool> ProcessLine(string info)
		{
            if (info == null) 
			{ 
				LogFile.Log("Argument Null Exception"); 
				throw new ArgumentNullException(nameof(info)); 
			}

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
                
                var tokens = info.Substring("$CQ".Length).Split(new char[] { ':' },4);
				var sender = tokens[0];
				var recipient = tokens[1];
				var command = tokens[2];
                var data = tokens.Length == 4 ? tokens[3] : null;
                
                if (recipient == "SERVER")
				{
					switch (command)
					{
						case "ATC":
							await Send($"$CRSERVER:{Callsign}:ATC:Y:{data}");
							break;
						case "CAPS":
							await Send($"$CRSERVER:{Callsign}:CAPS:ATCINFO=1:SECPOS=1");
							break;
                            
						case "IP":
							var ipep = (IPEndPoint)Client.Client.RemoteEndPoint;
							var ipa = ipep.Address;
							await Send($"$CRSERVER:{Callsign}:IP:{ipa}");
							await Send($"$CQSERVER:{Callsign}:CAPS");
							break;

						case "FP":
							FlightPlanRequested?.Invoke(this, new FlightPlanRequestedEventArgs(data));
							break;
                    }
				} else
				{
					if (command == "RN")
					{
						await Send($"CR{recipient}:{Callsign}:RN:VRC Game Simulated Controller:{recipient}:11");
					}
                    AtcMessageSent?.Invoke(this, new AtcMessageSentEventArgs(recipient, info));
                }
            }

			if (info.StartsWith("$CR"))
			{
				//Server responding to a $CQ.
                //Server will never recieve this
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

				await Send($"#TMVRCGame:{Callsign}:Connected");
                LogFile.Log($"{ Callsign} connected");

                Connected?.Invoke(this, new(Callsign, realName, certificate, rating, lat, lng));
			}			
			
			return false;
		}
	}

	public class LogFile
	{
        public static readonly string path = "C:\\VRCGame\\Logs\\" + DateTime.Now.ToString("yyyyddMM-HHmmss") + ".txt";

        public static void Create()
		{
			StreamWriter file;
			if (!File.Exists(path))
			{
				if (!Directory.Exists("C:\\VRCGame\\Logs\\"))
				{
					Directory.CreateDirectory("C:\\VRCGame\\Logs\\");
				}
				file = File.CreateText(path);
			}
            else
			{
				file = new(path, true);
			}
			file.WriteLine(DateTime.Now + ": Log file Created");
			file.Close();
		}
		
		public static void Log(string text)
		{
			StreamWriter file;
			if (!File.Exists(path))
			{
				Create();
			}
			file = new(path, true);
			file.WriteLine(DateTime.Now + ": " + text);
			file.Close();
		}
	}
}
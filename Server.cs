using System.Net;
using System.Text;
using System.Net.Sockets;
using VRC_Game;

#pragma warning disable 8618
#pragma warning disable 8602

namespace VRC_Game
{
    public class FSDServer
    {
        public static TcpClient Client;
        public static NetworkStream Stream;
        public static StreamReader Reader;
        public static Byte[] bytes = new Byte[256];
        public static Controller Player;
        private static TcpListener _server;
        public static Random rand = new();

        public static void Start()
        {
            _server = new TcpListener(IPAddress.Parse("127.0.0.1"), 6809);
            _server.Start();
            Console.WriteLine("_server Started! Please connect to localhost or 127.0.0.1!");

            while (true)
            {
                Client = _server.AcceptTcpClient();
                Stream = Client.GetStream();
                Send("$DISERVER:CLIENT:VATSIM FSD v3.13:abcdef12");
                Console.WriteLine("Client Connected!");
                int i = 0;
                while (Client.Connected)
                {
                    while ((i = Stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        String data = Encoding.ASCII.GetString(bytes, 0, i);
                        if (data == null) break;
                        String[] dataLines = data.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
                        foreach (var dataLine in dataLines)
                        {
                            ProcessData(dataLine);
                        }
                    }
                }
            }
        }

        public static void Send(string text)
        {
            byte[] msg = Encoding.UTF8.GetBytes($"{text}\r\n");
            Stream.Write(msg, 0, msg.Length);
        }

        private static void ProcessData(string data)
        {
            if (data.StartsWith("%"))
            {
                //Position Update
                var tokens = data["%".Length..].Split(':');
                var from = tokens[0];
                var freq = tokens[1];

                if (from == Player.Callsign && freq != Player.ShortFrequency)
                {
                    Player.Frequency = $"1{freq.Substring(0,2)}.{freq[2..]}";
                    Player.ShortFrequency = freq;
                    Console.WriteLine($"{Player.Callsign} changed to {Player.Frequency}");
                }

                //Ignore for now
                return;
            }
            
            if (data.StartsWith("$ID"))
            {
                //Client Authentication Packet
                var info = data["$ID".Length..].Split(':');
                Player = new Controller() { Callsign = info[0], Frequency = "199.998", ShortFrequency = "99998" };
                Console.WriteLine($"Created new Player with callsign {Player.Callsign} on {Player.Frequency}");
                return;
            }
            
            if (data.StartsWith("#AA"))
            {
                //ATC Logon
                var tokens = data["#AA".Length..].Split(':');
                var from = tokens[0];
                var realName = tokens[2];

                if (from == Player.Callsign)
                {
                    Send($"#TMserver:{Player.Callsign}:Connected to VRC-Game.");
                    Send($"#TMserver:{Player.Callsign}:VRC-Game Version 0.0.1");
                    Send($"$CRSERVER:{Player.Callsign}:ATC:Y:{Player.Callsign}");
                    Send($"$CRSERVER:{Player.Callsign}:IP:127.0.0.1");
                    Send($"$ZCSERVER:{Player.Callsign}:84b0829fc89d9d7848");
                    Console.WriteLine($"{Player.Callsign} Logged on!");
                }
                else
                {
                    Send($"#TMserver:{from}:Invalid Callsign");
                    Client.Close();
                }

                return;
            }
            
            if (data.StartsWith("#TM"))
            {
                //message
                var tokens = data["#TM".Length..].Split(':');
                var to = tokens[1];
                var message = tokens[2];

                if (to == $"@{Player.ShortFrequency}")
                {
                    Console.WriteLine($"Recieved {message} on {Player.Frequency}");
                    ProcessCommand(message);
                }
            }
            else if (data.StartsWith("#DA"))
            {
                //ATC Logoff
                Console.WriteLine($"{Player.Callsign} disconnected");
            }
        }

        private static void ProcessCommand(string command)
        {
            if (command.StartsWith("add"))
            {
                Console.WriteLine("Add Aircraft Command Ran");
                
                //Define Add command in documentation first
                //Temp Syntax: add type rwy altitude heading
                string[] tokens = command.Substring("add".Length).Split(' ');
                string type = tokens[1].ToUpper();
                string rwy = tokens[2];
                int alt = int.Parse(tokens[3]) + Program.MainAirport.Elevation;
                int heading = int.Parse(tokens[4]);
                string callsign = Aircraft.GenerateCallsign("ga");
                Console.WriteLine($"Adding a {type} as {callsign} at {alt} feet");
                double[] runwayData = DataQuery.runwayQuery(rwy);
                double lat = runwayData[0];
                double lng = runwayData[1];                
                Aircraft.CreateAirplane(callsign, alt, heading, lat, lng, type);
                Send($"#TMserver:@{Player.ShortFrequency}:Added {type} {callsign}");
            }
        }
    }
}

using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
using VRC_Game;
using System.Security.Cryptography;
using System.Diagnostics;
using System.ComponentModel;

namespace VRC_Game
{
    public class FSDSserver
    {
        public static TcpClient? Client;
        public static NetworkStream? Stream;
        public static StreamReader? Reader;
        public static Byte[] bytes = new Byte[256];
        public static Controller? Player;
        public static TcpListener? Server;
        public static Random rand = new();

        public static async void Start()
        {
            Server = new(IPAddress.Parse("127.0.0.1"), 6809);
            Server.Start();
            Console.WriteLine("Server Started! Please connect to localhost or 127.0.0.1!");

            while (true)
            {
                Client = Server.AcceptTcpClient();
                Stream = Client.GetStream();
                await Send("$DISERVER:CLIENT:VATSIM FSD v3.13:abcdef12");
                Console.WriteLine("Client Connected!");
                while (Client.Connected)
                {
                    int i;
                    while ((i = Stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        String Data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        String[] DataArray = Data.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        Console.WriteLine($"Recieved {DataArray.Length} lines of data.");
                        int line = 0;
                        for (line = 0; line < DataArray.Length; line++)
                        {
                            ProcessData(DataArray[line]);
                        }
                    }
                }
            }
        }

        public static async Task Send(string text)
        {
            if (Stream == null)
            {
                Console.WriteLine("Stream was null");
                return;
            }
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(text + "\r\n");
            Stream.Write(msg, 0, msg.Length);
            return;
        }

        public static async Task ProcessData(string Data)
        {
            if (Data == null)
            {
                Console.WriteLine("Data was null");
                return;
            }
            else
            {
                if (Data.StartsWith("%"))
                {
                    //Position Update
                    var tokens = Data.Substring("%".Length).Split(':');
                    var from = tokens[0];
                    var freq = tokens[1];
                    if (from == Player.Callsign)
                    {
                        if (freq != Player.ShortFrequency)
                        {
                            Player.Frequency = "1" + freq.Substring(0,2) + "." + freq.Substring(2);
                            Player.ShortFrequency = freq;
                            Console.WriteLine($"{Player.Callsign} changed to {Player.Frequency}");
                        } else
                        {
                            return;
                        }
                    }
                    //Ignore for now
                    return;
                } else if (Data.StartsWith("$ID"))
                {
                    //Client Authentication Packet
                    var info = Data.Substring("$ID".Length).Split(':');
                    Player = new(info[0], "199.998", "99998");
                    Console.WriteLine($"Created new Player with callsign {Player.Callsign} on {Player.Frequency}");
                    return;
                } else if (Data.StartsWith("#AA"))
                {
                    //ATC Logon
                    var tokens = Data.Substring("#AA".Length).Split(':');
                    var from = tokens[0];
                    var to = tokens[1];
                    var realName = tokens[2];
                    var certificate = tokens[3];
                    var password = tokens[4];
                    var rating = tokens[5];

                    if (from == Player.Callsign)
                    {
                        await Send($"#TMserver:{Player.Callsign}:Connected to VRC-Game.");
                        await Send($"#TMserver:{Player.Callsign}:VRC-Game Version 0.0.1");
                        await Send($"$CRSERVER:{Player.Callsign}:ATC:Y:{Player.Callsign}");
                        await Send($"$CRSERVER:{Player.Callsign}:IP:127.0.0.1");
                        await Send($"$ZCSERVER:{Player.Callsign}:84b0829fc89d9d7848");
                        Console.WriteLine($"{Player.Callsign} Logged on!");
                    } else
                    {
                        await Send($"#TMserver:{from}:Invalid Callsign");
                        Client.Close();
                    }
                    return;
                } else if (Data.StartsWith("#TM"))
                {
                    Console.WriteLine(Data);
                    var tokens = Data.Substring("#TM".Length).Split(':');
                    var from = tokens[0];
                    var to = tokens[1];
                    var message = tokens[2];
                    if (to == $"@{Player.ShortFrequency}")
                    {
                        Console.WriteLine($"Recieved {message} on {Player.Frequency}");
                        ProcessCommand(message);
                    }
                } else if (Data.StartsWith("#DA"))
                {
                    Console.WriteLine($"{Player.Callsign} disconnected");
                }
            }
        }

        public static async Task ProcessCommand(string command)
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
                double lat = runwayQuery(rwy)[0];
                double lng = runwayQuery(rwy)[1];
                Aircraft.CreateAirplane(callsign, alt, heading, lat, lng, type);
                Send($"#TMserver:@{Player.ShortFrequency}:Added {type} {callsign}");
            }
        }

        public static double[] runwayQuery(string runway)
        {
            double[] coordinates = new double[2];
            IEnumerable<double> longitudeQuery =
                from Runway in Program.MainAirport.Runways
                where Runway.ID == runway
                select Runway.Longitude;
            IEnumerable<double> latitudeQuery =
                from Runway in Program.MainAirport.Runways
                where Runway.ID == runway
                select Runway.Latitude;
            foreach (double coord in longitudeQuery)
            {
                coordinates[0] = coord;
            }
            foreach (double coord in latitudeQuery)
            {
                coordinates[1] = coord;
            }

            return coordinates;
        }
    }
}
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace VRC_Game
{
  public class FSDServer
  {
    private static TcpClient? Client;
    private static NetworkStream? Stream;
    private static readonly Byte[] bytes = new Byte[256];
    private static Controller Player = new("DEF_GND", "111.0000");
    private readonly TcpListener _server;
    private Airport MainAirport = new("KDEF", 0, 0, 0);
    private static readonly List<Aircraft> SessionAircraft = new();
    private static readonly List<Controller> SessionControllers = new();

    public FSDServer(string airportFilePath)
    {
      _server = new TcpListener(IPAddress.Parse("127.0.0.1"), 6809);
      VRC_Game.Parser.LoadFile(airportFilePath);
      Console.WriteLine("Aircraft & Controller Lists Ready!");
    }

    public void Start()
    {
      _server.Start();
      Console.WriteLine("Server Started! Please connect to localhost or 127.0.0.1!");

      while (true)
      {
        Client = _server.AcceptTcpClient();
        Stream = Client.GetStream();
        Send("$DISERVER:CLIENT:VATSIM FSD v3.13:abcdef12");
        Console.WriteLine("Client Connected!");
        int i;
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

    private static void Send(string text)
    {
      byte[] msg = Encoding.UTF8.GetBytes($"{text}\r\n");
      Stream?.Write(msg, 0, msg.Length);
    }

    private void ProcessData(string data)
    {
      if (data.StartsWith("%"))
      {
        //Position Update
        var tokens = data["%".Length..].Split(':');
        var from = tokens[0];
        var freq = tokens[1];

        if (from == Player.Callsign && freq != Player.ShortFrequency)
        {
          Player.Frequency = $"1{freq[..2]}.{freq[2..]}";
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
        Player = new Controller(info[0], "199.998", "99998");
        Console.WriteLine($"Created new Player with callsign {Player.Callsign} on {Player.Frequency}");
        return;
      }

      if (data.StartsWith("#AA"))
      {
        //ATC Logon
        var tokens = data["#AA".Length..].Split(':');
        var from = tokens[0];

        if (from == Player.Callsign)
        {
          Send($"#TMserver:{Player.Callsign}:Connected to VRC-Game.");
          Send($"#TMserver:{Player.Callsign}:VRC-Game Version 0.0.1");
          Send($"$CRSERVER:{Player.Callsign}:ATC:Y:{Player.Callsign}");
          Send($"$CRSERVER:{Player.Callsign}:IP:127.0.0.1");
          Send($"$ZCSERVER:{Player.Callsign}:84b0829fc89d9d7848");
          Console.WriteLine($"{Player.Callsign} Logged on!");
          for (int i = 0; i <= SessionControllers.ToArray().Length - 1; i++)
          {
            Controller controller = SessionControllers[i];
            Send($"%{controller.Callsign}:{controller.ShortFrequency}:0:150:12:{controller.Runway[0]}:{controller.Runway[1]}:0");
            Console.WriteLine($"Connected {controller.Callsign} on {controller.Frequency}");
          }
        }
        else
        {
          Send($"#TMserver:{from}:Invalid Callsign");
          Client?.Close();
        }
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
      if (data.StartsWith("#TM"))
      {
        Console.WriteLine(data);
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

    private void ProcessCommand(string command)
    {
      if (command.StartsWith("add"))
      {
        Console.WriteLine("Add Aircraft Command Ran");

        //Define Add command in documentation first
        //Temp Syntax: add type rwy altitude heading
        string[] tokens = command["add".Length..].Split(' ');
        string type = tokens[1].ToUpper();
        string rwy = tokens[2];
        int alt = int.Parse(tokens[3]) + MainAirport.Elevation;
        int heading = int.Parse(tokens[4]);
        Console.WriteLine($"Adding a {type} at {alt} feet");
        double[] runwayData = MainAirport.RunwayQuery(rwy);
        double lat = runwayData[0];
        double lng = runwayData[1];
        Aircraft craft = new(alt, heading, lat, lng, type, "1200", "N");
        Send($"@N:{craft.Callsign}:1200:12:{lat}:{lng}:{alt}:0:400:123");
        Send($"#TMserver:@{Player.ShortFrequency}:Added {type} {craft.Callsign}");
      }
    }


    private void LoadAirportFile(string path)
    {
      Regex airport = new(@"^AIRPORT:(?<icao>.*):(?<alt>.*)$");
      Regex runway = new(@"^RUNWAY:(?<rwy1>.*)/(?<rwy2>.*):(?<lat1>.*):(?<long1>.*):(?<lat2>.*):(?<long2>.*)$");
      Regex controller = new(@"^CONTROLLER:(?<callsign>.*):(?<freq>.*)$");
      if (!File.Exists(path))
      {
        Console.WriteLine("File Not Found");
        Environment.Exit(1);
      }

      string AirportFile = File.ReadAllText(path);
      string[] AirportLines = AirportFile.Split('\n');
      int i;
      for (i = 0; i < AirportLines.Length; i++)
      {
        string line = AirportLines[i];
        if (airport.IsMatch(line))
        {
          Match match = airport.Match(line);
          MainAirport = new Airport(match.Groups["icao"].Value, Int32.Parse(match.Groups["alt"].Value));
        }

        if (runway.IsMatch(line))
        {
          Match match = runway.Match(line);
          MainAirport.AddRunway(match.Groups["rwy1"].Value, match.Groups["rwy2"].Value, Double.Parse(match.Groups["lat1"].Value), Double.Parse(match.Groups["long1"].Value), Double.Parse(match.Groups["lat2"].Value), Double.Parse(match.Groups["long2"].Value));
        }

        if (controller.IsMatch(line))
        {
          Match match = controller.Match(line);
          double[] runwayLocation = MainAirport!.RunwayQuery("08");
          SessionControllers.Add(new Controller(match.Groups["callsign"].Value, match.Groups["freq"].Value, match.Groups["freq"].Value.Replace(".", "")[1..], runwayLocation));
        }
      }
    }
  }
}

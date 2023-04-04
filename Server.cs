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
    private static Controller Player = new("DEF_OBS", "111.000");
    private readonly TcpListener _server;
    public Facility CurrentFacility = new("DEF");
    private static readonly List<Aircraft> SessionAircraft = new();
    private static readonly List<Controller> SessionControllers = new();
    private string ConfigFilePath;

    public FSDServer(string airportFilePath)
    {
      _server = new TcpListener(IPAddress.Parse("127.0.0.1"), 6809);
      Console.WriteLine("Aircraft & Controller Lists Ready!");
      ConfigFilePath = airportFilePath;
    }

    public void Start()
    {
      Parser.LoadFile(ConfigFilePath);
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

        if (from == Player.Callsign && freq != Player.Frequency)
        {
          Player.Frequency = freq;
          Console.WriteLine($"{Player.Callsign} changed to {Player.Frequency}");
        }

        //Ignore for now
        return;
      }

      if (data.StartsWith("$ID"))
      {
        //Client Authentication Packet
        var info = data["$ID".Length..].Split(':');
        Player = new Controller(info[0], "199.998");
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
            Send($"%{controller.Callsign}:{controller.Frequency}:0:150:12:0:0:0");
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

        if (to == $"@{Player.Frequency}")
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

        if (to == $"@{Player.Frequency}")
        {
          Console.WriteLine($"Recieved {message} on 1{Player.Frequency[0..2]}.{Player.Frequency[2..]}");
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
        int alt = int.Parse(tokens[3]) + CurrentFacility.Airports.First().Elevation;
        int heading = int.Parse(tokens[4]);
        Console.WriteLine($"Adding a {type} at {alt} feet");
        double[] runwayData = CurrentFacility.Airports.First().RunwayQuery(rwy);
        double lat = runwayData[0];
        double lng = runwayData[1];
        Aircraft craft = new(alt, heading, lat, lng, type, "1200", "N");
        Send($"@N:{craft.Callsign}:1200:12:{lat}:{lng}:{alt}:0:400:123");
        Send($"#TMserver:@{Player.Frequency}:Added {type} {craft.Callsign}");
      }
    }
    
    public void addController(Controller con)
    {
      con.Frequency = con.Frequency.Replace(".", "").Substring(1);
      SessionControllers.Add(con);
    }
  }
}

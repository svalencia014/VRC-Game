using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
using VRC_Game;

namespace VRC_Game
{
    public class FSDSserver
    {
        public static TcpClient? Client;
        public static NetworkStream? Stream;
        public static StreamReader? Reader;
        public static Byte[] bytes = new Byte[256];
        public static Controller? Player;

        public static async void Start()
        {
            TcpListener Server = new(IPAddress.Parse("127.0.0.1"), 6809);
            Server.Start();
            Console.WriteLine("Server Started! Please connect to localhost or 127.0.0.1!");

            while (true)
            {
                Client = Server.AcceptTcpClient();
                Stream = Client.GetStream();
                await Send("$DISERVER:CLIENT:VATSIM FSD v3.13:abcdef12\r\n");
                Console.WriteLine("Client Connected!");
                while (Client.Connected)
                {
                    int i;
                    Reader = new(Stream);
                    String Data = Reader.ReadToEnd();
                    if (Data.Length == 0)
                    {
                        return;
                    } else
                    { 
                        Console.WriteLine(Data);
                        String[] DataArray = Data.Split(new[] { "\r\n " }, StringSplitOptions.RemoveEmptyEntries);
                        for (i = 0; i < DataArray.Length; i++)
                        {
                            await ProcessData(DataArray[i]);
                        }
                    }
                }
                Console.WriteLine("Client Disconnected!");
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

        public static async Task ProcessData(string data)
        {
            if (data == null)
            {
                Console.WriteLine("Data was null");
                return;
            }
            else
            {
                if (data.StartsWith("%"))
                {
                    //Position Update
                    //Ignore for now
                    return;
                } else if (data.StartsWith("$ID"))
                {
                    //Client Authentication Packet
                    var info = data.Substring("$ID".Length).Split(':');
                    Player = new(info[0], "199.998");
                    Console.WriteLine($"Created new Player with callsign {Player.Callsign} on {Player.Frequency}");
                    return;
                } else if (data.StartsWith("#AA"))
                {
                    //ATC Logon
                    var tokens = data.Substring("#AA".Length).Split(':');
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
                        await Send($"$ZCSERVER:{Player.Callsign}:84b0829fc89d9d7848");
                        Console.WriteLine($"{Player.Callsign} Logged on!");
                    } else
                    {
                        await Send($"#TMserver:{from}:Invalid Callsign");
                        Client.Close();
                    }
                    return;
                }
            }
        }
    }
}
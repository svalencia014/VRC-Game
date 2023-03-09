using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
using VRC_Game;

namespace VRC_Game
{
    public class FSDSserver
    {
        public static TcpClient Client;
        public static NetworkStream Stream;
        public static StreamReader Reader;
        public static Byte[] bytes = new Byte[256];
        public static Controller player;

        public static async void Start()
        {
            TcpListener server = null;
            server = new(IPAddress.Parse("127.0.0.1"), 6809);
            server.Start();
            Console.WriteLine("Server Started! Please connect to localhost or 127.0.0.1!");

            while (true)
            {
                Client = server.AcceptTcpClient();
                Stream = Client.GetStream();
                Send("$DISERVER:CLIENT:VATSIM FSD v3.13:abcdef12\r\n");
            }
        }

        public static async Task Send(string text)
        {
            if (Stream == null)
            {
                Console.WriteLine("Stream was null");
                return;
            }
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(text);
            Stream.Write(msg, 0, msg.Length);
        }

        public static async Task Read()
        {
            int i;
            Reader = new(Stream);
            String data = Reader.ReadToEnd();
            if (data == null)
            {
                Console.WriteLine("Data was null");
                return;
            }
            String[] DataArray = data.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine($"Recieved {DataArray.Length} lines of data");
            if (DataArray.Length == 0)
            {
                return;
            }
            for (i = 0; i < DataArray.Length; i++)
            {
                if (DataArray[i].StartsWith("%"))
                {
                    return;
                }
                else
                {
                    Console.WriteLine($"Recieved: {DataArray[i]}\\r\\n");
                }
                if (DataArray[i].StartsWith("$ID"))
                {
                    var info = DataArray[i].Substring("$ID".Length).Split(':');
                    player = new(info[0], "199.998");
                    Console.WriteLine($"Created new player with callsign: {player.Callsign} on {player.Frequency}");
                }

                if (DataArray[i].StartsWith("#AA"))
                {
                    var tokens = DataArray[i].Substring("#AA".Length).Split(':');
                    var from = tokens[0];
                    var to = tokens[1];
                    var realName = tokens[2];
                    var certificate = tokens[3];
                    var password = tokens[4];
                    var rating = tokens[5];
                    var lat = tokens.Length > 9 ? double.Parse(tokens[9]) : (double?)null;
                    var lng = tokens.Length > 10 ? double.Parse(tokens[10]) : (double?)null;

                    if (from == player.Callsign)
                    {
                        Send($"#TMSERVER:{player.Callsign}:Connected\r\n");
                        Console.WriteLine($"{player.Callsign} connected");
                    }
                    else
                    {
                        Send($"#SERVER:{from}:Invalid Callsign!\r\n");
                    }
                }
            }
        }
    }
}
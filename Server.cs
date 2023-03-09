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


        }
    }
}
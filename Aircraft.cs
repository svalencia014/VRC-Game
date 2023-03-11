using System.Runtime.CompilerServices;

#pragma warning disable 8618

namespace VRC_Game
{
    public class Aircraft
    {
        public string Callsign { get; set; }
        public int Altitude { get; set; }
        public int Heading { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Type { get; set; }
        public string Transponder { get; set; }
        public string Mode { get; set; }

        public Aircraft()
        {

        }

        public static string GenerateCallsign(string type)
        {
            string callsign;
            if (type == "ga")
            {
                callsign = "N";
                string[] callsignParts = new String[5];
                Random rand = new();
                for (int i = 0; i < 5; i ++)
                {
                    callsignParts[i] = rand.Next(9).ToString();
                }
                callsign = callsign + string.Concat(callsignParts);
            } else
            {
                callsign = "N12345";
            }

            return callsign;
        }
        public static void CreateAirplane(string callsign, int alt, int heading, double lat, double lng, string type)
        {
            Program.SessionAircraft.Add(new Aircraft() { Callsign = callsign, Altitude = alt, Heading = heading, Latitude = lat, Longitude = lng, Type = type, Transponder = "1200", Mode = "N" });
            FSDSserver.Send($"@N:{callsign}:1200:12:{lat}:{lng}:{alt}:0:400:123");
        }

    }
}
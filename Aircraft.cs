namespace VRC_Game
{
    public class Aircraft
    {
        public static string Callsign { get; set; }
        public static int Altitude { get; set; }
        public static int Heading { get; set; }
        public static double Latitude { get; set; }
        public static double Longitude { get; set; }
        public static string Type { get; set; }
        public static string Transponder { get; set; }
        public static string Mode { get; set; }

        public Aircraft(string callsign, int altitude, int heading, double lng, double lat, string type)
        {
            Callsign = callsign;
            Altitude = altitude;
            Heading = heading;
            Latitude = lng;
            Longitude = lat;
            Type = type;
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
    }
}
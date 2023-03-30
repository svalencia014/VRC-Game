namespace VRC_Game
{
    public class Controller
    {
        public string Callsign { get; set; }
        public string Frequency { get; set; }
        public string ShortFrequency { get; set; }

        public Controller()
        {

        }

        public static void CreateController(string callsign, string frequency)
        {
            string shortFrequency = frequency.Substring(1);
            double[] runway = DataQuery.runwayQuery("08");
            Program.SessionControllers.Add(new Controller() { Callsign = callsign, Frequency = frequency, ShortFrequency = shortFrequency });
            FSDServer.Send($"%{callsign}:{shortFrequency}:0:150:12:{runway[0]}:{runway[1]}");
        }
    }
}
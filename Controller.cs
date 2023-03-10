namespace VRC_Game
{
    public class Controller
    {
        public string Callsign { get; set; }
        public string Frequency { get; set; }
        public string ShortFrequency { get; set; }

        public Controller(string callsign, string frequency, string shortfrequency)
        {
            Callsign = callsign;
            Frequency = frequency;
            ShortFrequency = shortfrequency;
        }
    }
}
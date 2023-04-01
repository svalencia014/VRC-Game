using System.Runtime.CompilerServices;

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
            string shortFrequency = frequency.Substring(1).Replace(".", "");
            double[] runway = DataQuery.runwayQuery("08");
            Program.SessionControllers.Add(new Controller() { Callsign = callsign, Frequency = frequency, ShortFrequency = shortFrequency });
        }

        public static void LoadControllers()
        {
            double[] runway = DataQuery.runwayQuery("08");
            foreach (var controller in Program.SessionControllers)
            {
                FSDServer.Send($"%{controller.Callsign}:{controller.ShortFrequency}:0:150:12:{runway[0]}:{runway[1]}:0");
                Console.WriteLine($"Created Virtual Controller {controller.Callsign} on {controller.Frequency}");
            }
        }
    }
}
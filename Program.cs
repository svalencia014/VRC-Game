using System.ComponentModel.DataAnnotations;

namespace VRC_Game
{
    public class Program
    {
        public static Airport MainAirport;
        public static void Main(string[] args)
        {
            Console.WriteLine("VRC Game v0.0.1");
            Console.WriteLine("Please Enter the Path of your Airport (.apt) file");
            string Path = Console.ReadLine();
            LoadFile(Path);
            Console.WriteLine("Starting Server...");
            FSDSserver.Start();
        }

        public static void LoadFile(string path)
        {
            string AirportFile = File.ReadAllText(path);
            string[] AirportLines = AirportFile.Split('\n');
            int i;
            for (i = 0; i < AirportLines.Length; i++)
            {
                if (AirportLines[i].StartsWith("AIRPORT"))
                {
                    //airport line
                    string[] line = AirportLines[i].Substring("AIRPORT".Length).Split(':');
                    string name = line[0];
                    int alt = int.Parse(line[1]);
                    MainAirport = new(name, alt);
                } 
                else if (AirportLines[i].StartsWith("RUNWAY"))
                {
                    //Runway Line
                }
                else if (AirportLines[i].StartsWith("CONTROLLER"))
                {
                    //Controller Line
                    //Ignore for now
                }
            }
        }
    }

    public class Airport
    {
        public static string ICAO { get; set; }
        public static int Elevation { get; set; }
        public Runway[] Runways { get; set; }
        public Airport(string icao, int elev)
        {
            ICAO = icao;
            Elevation = elev;
        }

        public void AddRunways()
        {
            
        }
    }
    
    public class Runway
    {
        public Runway()
        {

        }


    }
}
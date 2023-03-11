using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;

#pragma warning disable 8618
#pragma warning disable 8604
#pragma warning disable 8600

namespace VRC_Game
{
    public class Program
    {
        public static Airport MainAirport;
        public static List<Aircraft> SessionAircraft;
        public static void Main(string[] args)
        {
            Console.WriteLine("VRC Game v0.0.1");
            Console.WriteLine("Please Enter the Path of your Airport (.apt) file");
            string Path = Console.ReadLine();
            LoadFile(Path);
            Console.WriteLine("Enter the Path of your Situation (.sit) file or press enter to skip");
            Path = Console.ReadLine();
            if (Path != null)
            {
                //LoadFile(Path)
            }
            Console.WriteLine("Initializing Airport List");
            SessionAircraft = new List<Aircraft>();
            Console.WriteLine("Aircraft list Initialized");
            Console.WriteLine("Starting Server...");
            FSDSserver.Start();
        }

        public static void LoadFile(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("File Not Found");
                Environment.Exit(1);
            }
            if (path.EndsWith(".apt"))
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
                        string name = line[1];
                        int alt = int.Parse(line[2]);
                        MainAirport = new(name, alt);
                        Console.WriteLine($"Created Airport {MainAirport.ICAO} at {MainAirport.Elevation}");
                    } 
                    else if (AirportLines[i].StartsWith("RUNWAY"))
                    {
                        //Runway Line- RUNWAY:RWY/RWY:LAT1:LNG1:LAT2:LNG2
                        string[] line = AirportLines[i].Substring("RUNWAY".Length).Split(':');
                        string name1 = line[1].Substring(0, 2);
                        string name2 = line[1].Substring(3, 2);
                        double latitude1 = double.Parse(line[2]);
                        double longitude1 = double.Parse(line[3]);
                        double latitude2 = double.Parse(line[4]);
                        double longitude2 = double.Parse(line[5]);
                        MainAirport.AddRunway(name1, name2, latitude1, longitude1, latitude2, longitude2);
                        Console.WriteLine($"Created Runway {name1}/{name2} with {name1} at {latitude1},{longitude1} and {name2} at {latitude2},{longitude2}.");
                    }
                    else if (AirportLines[i].StartsWith("CONTROLLER"))
                    {
                        //Controller Line
                        //Ignore for now
                    }
                }
            } else if (path.EndsWith(".sit"))
            {
                //situation file
                //handle later
            }
        }
    }

    public class Airport
    {
        public string ICAO { get; set; }
        public int Elevation { get; set; }
        public List<Runway> Runways { get; set; }
        public Airport(string icao, int elev)
        {
            ICAO = icao;
            Elevation = elev;
            Runways = new List<Runway>();
        }

        public void AddRunway(string name1, string name2, double Rwy1Lat, double Rwy1Lng, double Rwy2Lat, double Rwy2Lng)
        {
            Runways.Add(new Runway() { ID = name1, Latitude = Rwy1Lat, Longitude = Rwy1Lng });
            Runways.Add(new Runway() { ID = name2, Latitude = Rwy2Lat, Longitude = Rwy2Lng });
        }
    }
    
    public class Runway
    {
        public string ID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Runway()
        {

        }

    }
}
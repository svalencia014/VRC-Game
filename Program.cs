using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;

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
                    string name = line[1];
                    int alt = int.Parse(line[2]);
                    MainAirport = new(name, alt);
                    Console.WriteLine($"Created Airport {MainAirport.ICAO} at {MainAirport.Elevation}");
                } 
                else if (AirportLines[i].StartsWith("RUNWAY"))
                {
                    //Runway Line- RUNWAY:RWY/RWY:LAT1:LNG1:LAT2:LNG2
                    string[] line = AirportLines[i].Substring("RUNWAY".Length).Split(':');
                    string name = line[1];
                    double latitude1 = double.Parse(line[2]);
                    double longitude1 = double.Parse(line[3]);
                    double latitude2 = double.Parse(line[4]);
                    double longitude2 = double.Parse(line[5]);
                    MainAirport.AddRunway(name, latitude1, longitude1, latitude2, longitude2);
                    Console.WriteLine($"Created Runway {name} with end 1 at {latitude1},{longitude1} and end 2 at {latitude2},{longitude2}.");
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
        public string ICAO { get; set; }
        public int Elevation { get; set; }
        public List<Runway> Runways { get; set; }
        public Airport(string icao, int elev)
        {
            ICAO = icao;
            Elevation = elev;
            Runways = new List<Runway>();
        }

        public void AddRunway(string name, double Rwy1Lat, double Rwy1Lng, double Rwy2Lat, double Rwy2Lng)
        {
            Runways.Add(new Runway(name, Rwy1Lat, Rwy1Lng, Rwy2Lat, Rwy2Lng));
        }
    }
    
    public class Runway
    {
        public string ID { get; set; }
        public double End1Latitude { get; set; }
        public double End1Longitude { get; set; }
        public double End2Latitude { get; set; }
        public double End2Longitude { get; set; }

        public Runway(string id, double lat1, double lng1, double lat2, double lng2)
        {
            ID = id;
            End1Latitude = lat1;
            End1Longitude = lng1;
            End2Latitude = lat2;
            End2Longitude = lng2;
        }

    }
}
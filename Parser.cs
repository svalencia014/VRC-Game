using System.IO;
using Newtonsoft.Json;

#pragma warning disable 8600
#pragma warning disable 8602

namespace VRC_Game
{
    public class Parser
    {
        public static void LoadFile(string path)
        {
            if (!path.EndsWith(".json")) {
                Console.WriteLine("Invalid File Type! Expected .json");
                Environment.Exit(1);
            } else if (!File.Exists(path)) {
                Console.WriteLine("File Not Found!");
                Environment.Exit(1);
            }

            string FacilityFile = File.ReadAllText(path);
            if (FacilityFile == null) {
                Console.WriteLine("File is Empty!");
                Environment.Exit(1);
            } else {
                dynamic config = JsonConvert.DeserializeObject(FacilityFile);
                string FacilityId = path.Substring(0, 4);
                if (FacilityId.EndsWith(".")) {
                    FacilityId = FacilityId.Substring(0, 3);
                }
                Program.fsdServer.CurrentFacility.ID = FacilityId;
                foreach (var airport in config.airports) {
                    Program.fsdServer.CurrentFacility.Airports.Add(new Airport(airport.ICAO, airport.Latitude, airport.Longitude, airport.Elevation));
                    for (int i = 0; i < airport.Runways.Length; i++) {
                        Program.fsdServer.CurrentFacility.Airports[i].AddRunway(airport.Runways[i].ID, airport.Runways[i].Latitude, airport.Runways[i].Longitude, airport.Runways[i].Heading);
                    }
                }
                foreach (var controller in config.controllers) {
                    Program.fsdServer.addController(new Controller(controller.Callsign, controller.frequency));
                }
                //TODO: Add Controller Parsing
            }
        }
    }
}

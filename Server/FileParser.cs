using System.IO;
using Newtonsoft.Json;

#pragma warning disable 8600
#pragma warning disable 8602


namespace VRC_Game
{
  public class Parser
  {
    public static Facility LoadFile(string path)
    {
      Console.WriteLine("Loading Facility...");
      if (!path.EndsWith(".json"))
      {
        Console.WriteLine("Invalid File Type! Expected .json");
        Environment.Exit(1);
        return null;
      }
      else if (!File.Exists(path))
      {
        Console.WriteLine("File Not Found!");
        Environment.Exit(1);
        return null;
      }

      string FacilityFile = File.ReadAllText(path);
      if (FacilityFile == null)
      {
        Console.WriteLine("File is Empty!");
        Environment.Exit(1);
        return null;
      }
      else
      {
        Console.WriteLine("Parsing Facility...");
        dynamic config = JsonConvert.DeserializeObject(FacilityFile);
        string FacilityId = path.Replace(".json","").Substring(path.LastIndexOf("\\") + 1);
        if (config == null)
        {
            Console.WriteLine("Error loading facility");
            Environment.Exit(1);
            return null;
        }
        else
        {
          Console.WriteLine("Loading Airports...");
          Facility facilityConfig = new Facility(FacilityId);
          foreach (var airport in config.airports) {
            facilityConfig.Airports.Add(new Airport(Convert.ToString(airport.id), Convert.ToDouble(airport.latitude), Convert.ToDouble(airport.longitude), Convert.ToInt32(airport.elevation)));
            int i = config.airports.IndexOf(airport);
            foreach (var rwy in config.airports[i].runways) 
            {
              facilityConfig.Airports[i].Runways.Add(new Runway(Convert.ToString(rwy.id), Convert.ToDouble(rwy.latitude), Convert.ToDouble(rwy.longitude), Convert.ToInt32(rwy.heading)));
            }
          }
          Console.WriteLine("Loading Controllers...");
          foreach (var controller in config.controllers) {
            facilityConfig.Controllers.Add(new Controller(Convert.ToString(controller.callsign), Convert.ToString(controller.frequency)));
          }

          //TODO: Load in list of gates/parking spots
          return facilityConfig;
        }
      }
    }
  }
}

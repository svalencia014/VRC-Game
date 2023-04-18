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
          Facility facilityConfig = new Facility(FacilityId);
          foreach (var airport in config.airports) {
            facilityConfig.Airports.Add(new Airport(Convert.ToString(airport.id), Convert.ToDouble(airport.latitude), Convert.ToDouble(airport.longitude), Convert.ToInt32(airport.elevation)));
            for (int i = 0; i < airport.runways.Count; i++) 
            {
              facilityConfig.Airports[facilityConfig.Airports.Count - 1].Runways.Add(new Runway(Convert.ToString(airport.Runways[i].id), Convert.ToInt32(airport.Runways[i].heading), Convert.ToDouble(airport.Runways[i].latitude), Convert.ToDouble(airport.Runways[i].longitude)));
            }
          }
          foreach (var controller in config.controllers) {
            facilityConfig.Controllers.Add(new Controller(Convert.ToString(controller.callsign), Convert.ToString(controller.frequency)));
          }
          return facilityConfig;
        }
      }
    }
  }
}

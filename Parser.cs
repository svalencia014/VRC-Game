using System.IO;
using Newtonsoft.Json;

#pragma warning disable 8600
#pragma warning disable 8602

namespace VRC_Game
{
  public class Parser
  {
    public static FacilityConfig LoadFile(string path)
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
        
        if (config == null)
        {
            Console.WriteLine("Error loading facility");
            Environment.Exit(1);
            return null;
        }
        else
        {
          FacilityConfig facilityConfig = new FacilityConfig();
          foreach (var airport in config.airports) {
            facilityConfig.airports.Append(new FacilityConfig.Airport(Convert.ToString(airport.id), Convert.ToDouble(airport.latitude), Convert.ToDouble(airport.longitude), Convert.ToInt32(airport.elevation)));
          }
          facilityConfig.controllers = config.controllers;
          return facilityConfig;
        }
      }
    }
  }
}

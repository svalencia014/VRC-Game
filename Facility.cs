#pragma warning disable 8618

namespace VRC_Game 
{
  public class Facility 
  {
    public string ID { get; set; }
    public List<Airport> Airports { get; set; }
    public List<Controller> Controllers { get; set; }

    public Facility(string id) {
      ID = id;
    }
  }
  public class FacilityConfig 
  {
    public Airport[] airports { get; set; }
    public Controllers[] controllers { get; set; }
    public class Airport
    {
      public string ICAO { get; set; }
      public double Latitude { get; set; }
      public double Longitude { get; set; }
      public int Elevation { get; set; }
      public Runway[] Runways { get; set; }
      public Airport(string id, double lat, double lng, int elev) 
      {
        ICAO = id;
        Latitude = lat;
        Longitude = lng;
        Elevation = elev;
      }
    }

    public class Controllers 
    {
      public string Callsign { get; set; }
      public string Frequency { get; set; }
    }
  }
}
namespace VRC_Game
{
  public class Airport
  {
    public string ICAO { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Elevation { get; set; }
    public List<Runway> Runways { get; set; }
    public Airport(string icao, double lat, double lng, int elev)
    {
      ICAO = icao;
      Elevation = elev;
      Latitude = lat;
      Longitude = lng;
      Runways = new List<Runway>();
    }

    public void AddRunway(string name, double Lat, double Lng, int Heading)
    {
      Runways.Add(new Runway(name, Lat, Lng, Heading));
    }

    public double[] RunwayQuery(string runway)
    {
      double[] coordinates = new double[2];
      IEnumerable<double> latitudeQuery =
          from Runway in Runways
          where Runway.ID == runway
          select Runway.Latitude;
      IEnumerable<double> longitudeQuery =
          from Runway in Runways
          where Runway.ID == runway
          select Runway.Longitude;
      foreach (double coord in latitudeQuery)
      {
        coordinates[0] = coord;
      }
      foreach (double coord in longitudeQuery)
      {
        coordinates[1] = coord;
      }

      return coordinates;
    }
  }
}

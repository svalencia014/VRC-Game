namespace VRC_Game
{
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
      Runways.Add(new Runway(name1, Rwy1Lat, Rwy1Lng));
      Runways.Add(new Runway(name2, Rwy2Lat, Rwy2Lng));
    }
  }
}

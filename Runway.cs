namespace VRC_Game
{
  public class Runway
  {
    public string ID { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public Runway(string id, double latitude, double longitude)
    {
      ID = id;
      Latitude = latitude;
      Longitude = longitude;
    }

  }
}

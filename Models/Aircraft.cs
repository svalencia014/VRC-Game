namespace VRC_Game
{
  public class Aircraft
  {
    public string Callsign { get; set; }
    public int Altitude { get; set; }
    public int Heading { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Type { get; set; }
    public string Transponder { get; set; }
    public string Mode { get; set; }

    public Aircraft(int altitude, int heading, double latitude, double longitude, string type, string transponder, string mode)
    {
      Callsign = GenerateCallsign(type);
      Altitude = altitude;
      Heading = heading;
      Latitude = latitude;
      Longitude = longitude;
      Type = type;
      Transponder = transponder;
      Mode = mode;
    }

    private static string GenerateCallsign(string type)
    {
      string callsign;
      callsign = "N";
      string[] callsignParts = new String[5];
      Random rand = new();
      for (int i = 0; i < 5; i++)
      {
        callsignParts[i] = rand.Next(9).ToString();
      }
      callsign += string.Concat(callsignParts);

      return callsign;
    }
  }
}
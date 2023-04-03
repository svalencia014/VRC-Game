namespace VRC_Game
{
  public class Controller
  {
    public string Callsign { get; set; }
    public string Frequency { get; set; }
    public string ShortFrequency { get; set; }
    
    public Controller(string callsign, string frequency, string shortFrequency)
    {
      Callsign = callsign;
      Frequency = frequency;
      ShortFrequency = shortFrequency;
    }
  }
}
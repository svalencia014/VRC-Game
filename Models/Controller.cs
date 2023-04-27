namespace VRC_Game
{
  public class Controller
  {
    public string Callsign { get; set; }
    public string Frequency { get; set; }
    
    public Controller(string callsign, string frequency)
    {
      Callsign = callsign;
      Frequency = frequency;
    }
  }
}
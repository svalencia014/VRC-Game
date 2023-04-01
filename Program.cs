namespace VRC_Game
{
  public class Program
  {
    public static void Main()
    {
      string? airportPath;
      Console.WriteLine("VRC Game v0.0.1");
      Console.WriteLine("Please Enter the Path of your Airport (.apt) file");
      airportPath = Console.ReadLine();
      //Console.WriteLine("Enter the Path of your Situation (.sit) file or press enter to skip");
      //path = Console.ReadLine();
      //if (path != null)
      //{
      //  //LoadFile(Path)
      //}
      Console.WriteLine("Starting Server...");
      if (airportPath != null)
      {
        FSDServer fSDServer = new(airportPath);
        fSDServer.Start();
      }
    }
  }
}
namespace VRC_Game
{
  public class Program
  {
    public static FSDServer? fsdServer;
    public static void Main()
    {
      Console.WriteLine("VRC Game v0.0.1");
      Console.WriteLine("Please Enter the Path of your Facility (.json) file");
      string? airportPath = Console.ReadLine();
      //Console.WriteLine("Enter the Path of your Situation (.sit) file or press enter to skip");
      //path = Console.ReadLine();
      //if (path != null)
      //{
      //  //LoadFile(Path)
      //}
      Console.WriteLine("Starting Server...");
      if (airportPath != null)
      {
        FSDServer fsdServer = new(airportPath);
        if (fsdServer != null)
        {
          fsdServer.Start();
        } else {
          Console.WriteLine("Server Failed to Start!");
          Environment.Exit(2);
        }
      }
    }
  }
}
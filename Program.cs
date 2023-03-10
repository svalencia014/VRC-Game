namespace VRC_Game
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("VRC Game v0.0.1");
            //Load airport file here
            Console.WriteLine("Starting Server...");
            FSDSserver.Start();
        }
    }
}
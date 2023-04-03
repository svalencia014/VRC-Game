using System.IO;
using Newtonsoft.Json;

#pragma warning disable 8600

namespace VRC_Game
{
    public class Parser
    {
        public static void LoadFile(string path)
        {
            if (!path.EndsWith(".json")) {
                Console.WriteLine("Invalid File Type! Expected .json");
                Environment.Exit(1);
            } else if (!File.Exists(path)) {
                Console.WriteLine("File Not Found!");
                Environment.Exit(1);
            }

            string FacilityFile = File.ReadAllText(path);
            if (FacilityFile == null) {
                Console.WriteLine("File is Empty!");
                Environment.Exit(1);
            } else {
                object FacilityConfig = JsonConvert.DeserializeObject(FacilityFile);
                string FacilityId = path.Substring(0, 4);
                if (FacilityId.EndsWith("")) {
                
                }
            }
        }
    }
}

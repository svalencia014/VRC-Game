using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using VRC_Game.Controllers;
using VRC_Game.Server;

namespace VRC_Game.Readers
{
    public class SituationFile
    {
        public class Controller
        {
            public string Callsign { get; set; }
            public string Frequency { get; set; }
            public double Lat { get; set; }
            public double Lng { get; set; }
            
            public Controller(string callsign, string frequency, double lat, double lng)
            {
                Callsign = callsign;
                Frequency = frequency;
                Lat = lat;
                Lng = lng;                
            }
        }
        
       
        public Controller[] GetControllers(string path)
        {
            // Parse Situtation.Txt
            if (!File.Exists(path))
            {
                LogFile.Log("Situation.txt Not Found");
                return null;
            } else
            {
                LogFile.Log("Situation.txt found");
                StreamReader r = new(path);
                string File = r.ReadToEnd();
                string[] Config = File.Split('\n');
                LogFile.Log(Config[0]);
                List<Controller> controllerList = new();
                Controller[] foundControllers = {};
                for (int i = 0; i < Config.Length; i++ )
                {
                    if (i == 0 && Config[i].StartsWith("CONTROLLER"))
                    {
                        //CONTROLLER:CALLSIGN:FREQ

                        string[] Line = Config[i].Split(':');
                        string callsign = Line[1];
                        string frequency = Line[2];
                        Controller SimulatedController = new(callsign, frequency, 0, 0);
                        controllerList.Add(SimulatedController);
                        foundControllers = controllerList.ToArray();
                    }
                    else
                    {
                        LogFile.Log("Error parsing file on line " + i);
                        LogFile.Log("Process Exited with Code (2): Invalid Situtation File");
                        throw new FormatException();
                    }
                }

                if (foundControllers.Length == 0)
                {
                    foundControllers[0].Callsign = "NONE";
                    foundControllers[0].Frequency = "00000";
                }
                LogFile.Log($"Found {foundControllers.Length} Controllers!");
                return foundControllers;
            }
            
        }
    }

    public class AIRAC
    {
        
    }
    
}
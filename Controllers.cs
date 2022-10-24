using System;
using System.IO;
using VRC_Game.Readers;

namespace VRC_Game.Controllers
{
	public class Controller
	{

		public void Create(string path)
		{
			//Pull in info from Parsed File
			SituationFile sit = new();
			SituationFile.Controller[] foundControllers = sit.GetControllers(path);

			//Spawn controllers
			SpawnControllers(foundControllers);
		}

        public static void SpawnControllers(SituationFile.Controller[] foundControllers)
        {
			for (int i = 0; i < foundControllers.Length; i++)
			{
				//Create vars
				char[] callsign = new char[10];
				callsign = foundControllers[i].Callsign.ToString().ToCharArray();
				char[] frequency = new char[5];
				foundControllers[i].Frequency.ToString().ToCharArray();
				double lng = foundControllers[i].Lng;
				double lat = foundControllers[i].Lat;

				//Create pointers
				unsafe
				{
					char* callsignPointer = &callsign;
					char* frequencyPointer = &frequency;
					double* lngPointer = &lng;
					double* latPointer = &lat;
                }
                
            }
        }
    }
}
using System;
using System.IO;
using VRC_Game.Readers;

namespace VRC_Game.Controllers
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

		public void Create(string path)
		{
			//Pull in info from Parsed File
			SituationFile sit = new();
			sit.getControllers(path);
		}
	}
}
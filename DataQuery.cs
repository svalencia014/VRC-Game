namespace VRC_Game
{
    public class DataQuery { 
        public static double[] runwayQuery(string runway)
        {
            double[] coordinates = new double[2];
            IEnumerable<double> latitudeQuery =
                from Runway in Program.MainAirport.Runways
                where Runway.ID == runway
                select Runway.Latitude;
            IEnumerable<double> longitudeQuery =
                from Runway in Program.MainAirport.Runways
                where Runway.ID == runway
                select Runway.Longitude;
            foreach (double coord in latitudeQuery)
            {
                coordinates[0] = coord;
            }
            foreach (double coord in longitudeQuery)
            {
                coordinates[1] = coord;
            }

            return coordinates;
        }
    }
}
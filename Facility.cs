#pragma warning disable 8618

namespace VRC_Game {
    public class Facility {
        public string ID { get; set; }
        public List<Airport> Airports { get; set; }
        public List<Controller> Controllers { get; set; }

        public Facility(string id) {
            ID = id;
        }
    }
}
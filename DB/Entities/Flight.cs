using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DB.Entities
{
    public class Flight : BaseEntity
    {
        public string FlightCode { get; set; } = null!;
        public string DepartureAirport { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public string DestinationAirport { get; set; } = null!;
        public DateTime ArrivalTime { get; set; }
        public short Capacity { get; set; }

        public ICollection<FlightReservation> FlightReservations { get; set; } = new HashSet<FlightReservation>();


        public override string ToString()
        {
            return "Flight: " + base.ToString();
        }
    }
}

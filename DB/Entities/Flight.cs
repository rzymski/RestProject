namespace DB.Entities
{
    public class Flight : BaseEntity
    {
        public string flightCode {  get; set; }
        public string departureAirport { get; set; }
        public DateTime departureTime { get; set; }
        public string destinationAirport { get; set; }
        public DateTime arrivalTime { get; set; }
        public long capacity { get; set; }

        public ICollection<FlightReservation> FlightReservations { get; set; }

        public override string ToString()
        {
            return "Flight: " + base.ToString();
        }
    }
}

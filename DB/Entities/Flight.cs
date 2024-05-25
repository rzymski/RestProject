namespace DB.Entities
{
    public class Flight : BaseEntity
    {
        public string FlightCode {  get; set; }
        public string DepartureAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public string DestinationAirport { get; set; }
        public DateTime ArrivalTime { get; set; }
        public short Capacity { get; set; }

        public ICollection<FlightReservation> FlightReservations { get; set; }

        public override string ToString()
        {
            return "Flight: " + base.ToString();
        }
    }
}

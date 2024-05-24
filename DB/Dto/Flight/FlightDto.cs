using DB.Dto.Base;

namespace DB.Dto.Flight
{
    public class FlightDto : BaseIdDto
    {
        public string FlightCode { get; set; }
        public string DepartureAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public string DestinationAirport { get; set; }
        public DateTime ArrivalTime { get; set; }
        public long Capacity { get; set; }

        public FlightDto() { }
        public FlightDto(int id, string flightCode, string departureAirport, DateTime departureTime, string destinationAirport, DateTime arrivalTime, long capacity) : base(id)
        {
            this.FlightCode = flightCode;
            this.DepartureAirport = departureAirport;
            this.DepartureTime = departureTime;
            this.DestinationAirport = destinationAirport;
            this.ArrivalTime = arrivalTime;
            this.Capacity = capacity;
        }

        public override string ToString() 
        {
            return $"FlightDto {base.ToString()}";
        }
    }
}

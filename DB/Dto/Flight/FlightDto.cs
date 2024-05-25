using DB.Dto.Base;

namespace DB.Dto.Flight
{
    public class FlightDto : BaseIdDto
    {
        public string FlightCode { get; set; } = null!;
        public string DepartureAirport { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public string DestinationAirport { get; set; } = null!;
        public DateTime ArrivalTime { get; set; }
        public short Capacity { get; set; }

        public FlightDto() { }
        public FlightDto(int id, string flightCode, string departureAirport, DateTime departureTime, string destinationAirport, DateTime arrivalTime, short capacity) : base(id)
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

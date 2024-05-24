using DB.Dto.Flight;
using DB.Entities;
using DB.Repositories.Interfaces;
using DB.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace DB.Services
{
    public class FlightService : BaseService<Flight, FlightDto, FlightAddEditDto>, IFlightService
    {
        public FlightService(IFlightRepository repository) : base(repository) {}

        protected override FlightDto MapToDto(Flight flight)
        {
            if (flight == null)
                throw new ArgumentNullException(nameof(flight), "Flight entity cannot be null.");
            return new FlightDto
            {
                Id = flight.Id,
                FlightCode = flight.FlightCode,
                DepartureAirport = flight.DepartureAirport,
                DepartureTime = flight.DepartureTime,
                DestinationAirport = flight.DestinationAirport,
                ArrivalTime = flight.ArrivalTime,
                Capacity = flight.Capacity
            };
        }

        protected override Flight MapAddEditDtoToEntity(FlightAddEditDto dto, Flight flight)
        {
            if (flight == null)
                flight = new Flight();

            flight.FlightCode = dto.FlightCode;
            flight.DepartureAirport = dto.DepartureAirport;
            flight.DepartureTime = dto.DepartureTime;
            flight.DestinationAirport = dto.DestinationAirport;
            flight.ArrivalTime = dto.ArrivalTime;
            flight.Capacity = dto.Capacity;

            return flight;
        }

        public List<FlightDto> GetByParameters(string? departureAirport, string? destinationAirport, DateTime? departureTime, DateTime? arrivalTime)
        {
            var results = baseRepository.GetAll().Where(p =>    (string.IsNullOrEmpty(departureAirport) || p.DepartureAirport.Equals(departureAirport, StringComparison.OrdinalIgnoreCase)) &&
                                                            (string.IsNullOrEmpty(destinationAirport) || p.DestinationAirport.Equals(destinationAirport, StringComparison.OrdinalIgnoreCase)) &&
                                                            (!departureTime.HasValue || p.DepartureTime == departureTime) &&
                                                            (!arrivalTime.HasValue || p.ArrivalTime == arrivalTime)).Select(MapToDto).ToList();
            return results;
        }
    }
}

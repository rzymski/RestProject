using DB.Dto.Flight;
using DB.Entities;
using DB.Repositories.Interfaces;
using DB.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace DB.Services
{
    public class FlightService : BaseService<Flight, FlightDto, FlightAddEditDto>, IFlightService
    {
        private readonly IFlightRepository flightRepository;
        public FlightService(IFlightRepository repository) : base(repository) 
        {
            this.flightRepository = repository;
        }

        public override FlightDto MapToDto(Flight flight)
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

        public override Flight MapAddEditDtoToEntity(FlightAddEditDto dto, Flight? flight = null)
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

        public List<FlightDto> GetByParameters(string? departureAirport, string? destinationAirport, DateTime? departureTime, DateTime? arrivalTime, short? capacity)
        {
            var results = baseRepository.GetAll().Where(p =>    (string.IsNullOrEmpty(departureAirport) || p.DepartureAirport.Equals(departureAirport, StringComparison.OrdinalIgnoreCase)) &&
                                                            (string.IsNullOrEmpty(destinationAirport) || p.DestinationAirport.Equals(destinationAirport, StringComparison.OrdinalIgnoreCase)) &&
                                                            (!departureTime.HasValue || p.DepartureTime == departureTime) &&
                                                            (!arrivalTime.HasValue || p.ArrivalTime == arrivalTime) &&
                                                            (!capacity.HasValue || p.Capacity == capacity)).Select(MapToDto).ToList();
            return results;
        }

        public List<string> GetAllAirports()
        {
            return flightRepository.GetAllAirports();
        }

        public int GetFlightAvailableSeats(int flightId, string? username)
        {
            Flight? flight = flightRepository.GetById(flightId);
            if (flight == null)
                throw new InvalidOperationException($"Flight with id = {flightId} does not exist.");
            return flightRepository.GetFlightAvailableSeats(flightId, username);
        }

        public List<FlightDto> GetAllQualifyingFlights(string? departureAirport, string? destinationAirport, DateTime? departureStartDateRange, DateTime? departureEndDateRange)
        {
            var results = baseRepository.GetAll().Where(p =>    (string.IsNullOrEmpty(departureAirport) || p.DepartureAirport.Equals(departureAirport, StringComparison.OrdinalIgnoreCase)) &&
                                                            (string.IsNullOrEmpty(destinationAirport) || p.DestinationAirport.Equals(destinationAirport, StringComparison.OrdinalIgnoreCase)) &&
                                                            (!departureStartDateRange.HasValue || p.DepartureTime >= departureStartDateRange) &&
                                                            (!departureEndDateRange.HasValue || p.DepartureTime <= departureEndDateRange)).Select(MapToDto).ToList();
            return results;
        }
    }
}

using DB.Dto.Flight;
using DB.Entities;

namespace DB.Services.Interfaces
{
    public interface IFlightService : IBaseService<Flight, FlightDto, FlightAddEditDto>
    {
        List<FlightDto> GetByParameters(string? departureAirport, string? destinationAirport, DateTime? departureTime, DateTime? arrivalTime);
    }
}

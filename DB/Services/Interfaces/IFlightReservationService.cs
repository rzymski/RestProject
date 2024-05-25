using DB.Dto.Flight;
using DB.Dto.FlightReservation;
using DB.Dto.User;
using DB.Entities;

namespace DB.Services.Interfaces
{
    public interface IFlightReservationService : IBaseService<FlightReservation, FlightReservationDto, FlightReservationAddEditDto>
    {
        List<FlightReservationDto> GetByParameters(FlightAddEditDto? flightDto, UserAddEditDto? userDto, long? numberOfReservedSeats);
    }
}

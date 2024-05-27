using DB.Dto.Flight;
using DB.Dto.FlightReservation;
using DB.Dto.User;
using DB.Entities;

namespace DB.Services.Interfaces
{
    public interface IFlightReservationService : IBaseService<FlightReservation, FlightReservationDto, FlightReservationAddEditDto>
    {
        bool ChangeNumberOfReservedSeats(int id, short numberOfReservedSeats);
        List<FlightReservationDto> GetByParameters(int? flightId, int? userId, short? numberOfReservedSeats = null);
        FlightReservationAllFieldsDto GetByIdAllFieldsDtoObject(int id);
    }
}

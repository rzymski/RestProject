using DB.Entities;

namespace DB.Repositories.Interfaces
{
    public interface IFlightReservationRepository : IBaseRepository<FlightReservation>
    {
        void ChangeNumberOfReservedSeats(int id, short numberOfReservedSeats);
    }
}

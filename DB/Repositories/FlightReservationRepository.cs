using DB.Entities;
using DB.Repositories.Interfaces;

namespace DB.Repositories
{
    public class FlightReservationRepository : BaseRepository<FlightReservation>, IFlightReservationRepository
    {
        public FlightReservationRepository(MyDBContext dbContext) : base(dbContext) {}

        public void ChangeNumberOfReservedSeats(int id, short numberOfReservedSeats)
        {
            if (numberOfReservedSeats <= 0)
                throw new InvalidOperationException($"Min numberOfReservedSeats is 1. Actually numberOfReservedSeats = {numberOfReservedSeats}.");
            FlightReservation flightReservation =  _dbContext.Set<FlightReservation>().Single(x => x.Id == id);
            flightReservation.NumberOfReservedSeats = numberOfReservedSeats;
            _dbContext.SaveChanges();
        }
    }
}

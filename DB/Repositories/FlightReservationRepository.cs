using DB.Entities;
using DB.Repositories.Interfaces;

namespace DB.Repositories
{
    public class FlightReservationRepository : BaseRepository<FlightReservation>, IFlightReservationRepository
    {
        public FlightReservationRepository(MyDBContext dbContext) : base(dbContext) {}

        public void ChangeNumberOfReservedSeats(int id, short NumberOfReservedSeats)
        {
            FlightReservation flightReservation =  _dbContext.Set<FlightReservation>().Single(x => x.Id == id);
            flightReservation.NumberOfReservedSeats = NumberOfReservedSeats;
            _dbContext.SaveChanges();
        }
    }
}

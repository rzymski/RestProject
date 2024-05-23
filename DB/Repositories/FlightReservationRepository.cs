using DB.Entities;
using DB.Repositories.Interfaces;

namespace DB.Repositories
{
    public class FlightReservationRepository : BaseRepository<FlightReservation>, IFlightReservationRepository
    {
        public FlightReservationRepository(MyDBContext dbContext) : base(dbContext)
        {
        }
    }
}

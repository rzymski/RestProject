using DB.Entities;
using DB.Repositories.Interfaces;

namespace DB.Repositories
{
    public class FlightRepository : BaseRepository<Flight>, IFlightRepository
    {
        public FlightRepository(MyDBContext dbContext) : base(dbContext) {}
    }
}

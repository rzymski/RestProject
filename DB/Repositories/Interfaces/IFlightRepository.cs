using DB.Entities;

namespace DB.Repositories.Interfaces
{
    public interface IFlightRepository : IBaseRepository<Flight>
    {
        public List<string> GetAllAirports();
        int GetFlightAvailableSeats(int flightId, string? username);
    }
}

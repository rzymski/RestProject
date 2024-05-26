using DB.Entities;
using DB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DB.Repositories
{
    public class FlightRepository : BaseRepository<Flight>, IFlightRepository
    {
        public FlightRepository(MyDBContext dbContext) : base(dbContext) {}

        public List<string> GetAllAirports()
        {
            var departureAirports = _dbContext.Flights.Select(f => f.DepartureAirport);
            var destinationAirports = _dbContext.Flights.Select(f => f.DestinationAirport);
            List<string> allAirports = departureAirports.Concat(destinationAirports).Distinct().ToList();
            return allAirports;
        }

        public int GetFlightAvailableSeats(int flightId)
        {
            Flight flight = _dbContext.Set<Flight>().Single(x => x.Id == flightId);
            var totalReservedSeats = _dbContext.FlightReservations.Where(fr => fr.FlightId == flightId).Sum(fr => fr.NumberOfReservedSeats);
            return flight.Capacity - totalReservedSeats;
        }
    }
}

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

        public int GetFlightAvailableSeats(int flightId, string? username)
        {
            Flight? flight = _dbContext.Flights.Include(f => f.FlightReservations).SingleOrDefault(f => f.Id == flightId);
            if (flight == null)
                throw new InvalidOperationException($"Flight with id = {flightId} does not exist.");
            int totalReservedSeats = flight.FlightReservations.Sum(fr => fr.NumberOfReservedSeats);
            int actuallyReservedSeatsByThisUser = 0;
            if (!string.IsNullOrEmpty(username))
            {
                User? user = _dbContext.Users.SingleOrDefault(u => u.Login == username);
                if (user != null)
                {
                    var userReservation = flight.FlightReservations.SingleOrDefault(fr => fr.UserId == user.Id);
                    if (userReservation != null)
                        actuallyReservedSeatsByThisUser = userReservation.NumberOfReservedSeats;
                }
            }
            return flight.Capacity - totalReservedSeats + actuallyReservedSeatsByThisUser;
        }
    }
}

using DB.Dto.Flight;
using DB.Dto.FlightReservation;
using DB.Dto.User;
using DB.Entities;
using DB.Repositories;
using DB.Repositories.Interfaces;
using DB.Services.Interfaces;

namespace DB.Services
{
    public class FlightReservationService : BaseService<FlightReservation, FlightReservationDto, FlightReservationAddEditDto>, IFlightReservationService
    {
        private readonly IFlightReservationRepository flightReservationRepository;
        private readonly IBaseRepository<Flight> flightRepository;
        private readonly IBaseRepository<User> userRepository;

        public FlightReservationService(IFlightReservationRepository repository, IBaseRepository<Flight> flightRepository, IBaseRepository<User> userRepository) : base(repository) 
        { 
            this.flightReservationRepository = repository;
            this.flightRepository = flightRepository;
            this.userRepository = userRepository;
        }

        public override FlightReservationDto MapToDto(FlightReservation flightReservation)
        {
            if (flightReservation == null)
                throw new ArgumentNullException(nameof(flightReservation), "FlightReservation entity cannot be null.");
            return new FlightReservationDto
            {
                Id = flightReservation.Id,
                FlightId = flightReservation.FlightId,
                UserId = flightReservation.UserId,
                NumberOfReservedSeats = flightReservation.NumberOfReservedSeats
            };
        }

        public override FlightReservation MapAddEditDtoToEntity(FlightReservationAddEditDto dto, FlightReservation? flightReservation)
        {
            if (flightReservation == null)
                flightReservation = new FlightReservation();

            flightReservation.FlightId = dto.FlightId;
            flightReservation.UserId = dto.UserId;
            flightReservation.NumberOfReservedSeats = dto.NumberOfReservedSeats;

            return flightReservation;
        }

        public bool ChangeNumberOfReservedSeats(int id, short numberOfReservedSeats)
        {
            var existingItem = baseRepository.GetById(id);
            if (existingItem == null)
                return false;

            flightReservationRepository.ChangeNumberOfReservedSeats(id, numberOfReservedSeats);
            return true;
        }

        public List<FlightReservationDto> GetByParameters(int? flightId, int? userId, short? numberOfReservedSeats = null)
        {
            var results = baseRepository.GetAll().Where(p => (flightId == null || flightId == p.FlightId) &&
                                                            (userId == null || userId == p.UserId) &&
                                                            (numberOfReservedSeats == null || numberOfReservedSeats == p.NumberOfReservedSeats)).Select(MapToDto).ToList();
            return results;
        }

        public override int Add(FlightReservationAddEditDto item)
        {
            ValidateFlightReservationParameters(item.FlightId, item.UserId);
            return base.Add(item);
        }

        public override List<int> AddList(List<FlightReservationAddEditDto> items)
        {
            foreach (var item in items)
                ValidateFlightReservationParameters(item.FlightId, item.UserId);
            return base.AddList(items);
        }

        public override bool Update(int id, FlightReservationAddEditDto item)
        {
            ValidateFlightReservationParameters(item.FlightId, item.UserId, true);
            return base.Update(id, item);
        }

        private void ValidateFlightReservationParameters(int flightId, int userId, bool update = false)
        {
            bool flightExists = flightRepository.GetById(flightId) != null;
            if (!flightExists)
                throw new InvalidOperationException($"Flight with id = {flightId} does not exist.");
            bool userExists = userRepository.GetById(userId) != null;
            if (!userExists)
                throw new InvalidOperationException($"User with id = {userId} does not exist.");

            List<FlightReservationDto> existingFlightReservations = GetByParameters(flightId, userId);
            if ((!update && existingFlightReservations.Count != 0) || (update && existingFlightReservations.Count != 1))
                throw new InvalidOperationException($"Another flightReservation with flightId = {flightId} and userId = {userId} already exist. Can't add/update another.");
        }

        public FlightReservationAllFieldsDto GetByIdAllFieldsDtoObject(int id)
        {
            var flightReservation = flightReservationRepository.GetById(id);
            if (flightReservation == null)
                throw new InvalidOperationException($"FlightReservation with id = {id} does not exist.");
            var flight = flightRepository.GetById(flightReservation.FlightId);
            if (flight == null)
                throw new InvalidOperationException($"Flight with id = {flightReservation.FlightId} does not exist. It seems that reservation with id = {id} have no correct flight.");
            var user = userRepository.GetById(flightReservation.UserId);
            if (user == null)
                throw new InvalidOperationException($"User with id = {flightReservation.UserId} does not exist. It seems that reservation with id = {id} have no correct user.");
            var flightReservationAllData = new FlightReservationAllFieldsDto
            {
                ReservationId = flightReservation.Id,
                NumberOfReservedSeats = flightReservation.NumberOfReservedSeats,

                FlightId = flight.Id,
                FlightCode = flight.FlightCode,
                DepartureAirport = flight.DepartureAirport,
                DepartureTime = flight.DepartureTime,
                DestinationAirport = flight.DestinationAirport,
                ArrivalTime = flight.ArrivalTime,
                Capacity = flight.Capacity,

                UserId = user.Id,
                Login = user.Login,
                Email = user.Email
            };
            return flightReservationAllData;
        }
    }
}

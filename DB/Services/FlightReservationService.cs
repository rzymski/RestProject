using DB.Dto.Flight;
using DB.Dto.FlightReservation;
using DB.Dto.User;
using DB.Entities;
using DB.Repositories.Interfaces;
using DB.Services.Interfaces;

namespace DB.Services
{
    public class FlightReservationService : BaseService<FlightReservation, FlightReservationDto, FlightReservationAddEditDto>, IFlightReservationService
    {
        private readonly IFlightReservationRepository flightReservationRepository;

        public FlightReservationService(IFlightReservationRepository repository) : base(repository) 
        { 
            this.flightReservationRepository = repository;
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

        public List<FlightReservationDto> GetByParameters(int? flightId, int? userId, short? numberOfReservedSeats)
        {
            var results = baseRepository.GetAll().Where(p => (flightId == null || flightId == p.FlightId) &&
                                                            (userId == null || userId == p.UserId) &&
                                                            (numberOfReservedSeats == null || numberOfReservedSeats == p.NumberOfReservedSeats)).Select(MapToDto).ToList();
            return results;
        }
    }
}

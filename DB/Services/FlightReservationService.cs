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
        private readonly IFlightService flightService;
        private readonly IUserService userService;

        public FlightReservationService(IFlightReservationRepository repository, IFlightService flightService, IUserService userService) : base(repository) 
        { 
            this.flightReservationRepository = repository;
            this.flightService = flightService;
            this.userService = userService;
        }

        public override FlightReservationDto MapToDto(FlightReservation flightReservation)
        {
            if (flightReservation == null)
                throw new ArgumentNullException(nameof(flightReservation), "FlightReservation entity cannot be null.");
            return new FlightReservationDto
            {
                Id = flightReservation.Id,
                //FlightDto = new FlightDto(flightReservation.Flight.Id, flightReservation.Flight.FlightCode, flightReservation.Flight.DepartureAirport, flightReservation.Flight.DepartureTime, flightReservation.Flight.DestinationAirport, flightReservation.Flight.ArrivalTime, flightReservation.Flight.Capacity),
                //UserDto = new UserDto(flightReservation.User.Id, flightReservation.User.Login, flightReservation.User.Password, flightReservation.User.Email),

                // NIE DZIALA BO FLIGHT IS NULL AND USER IS NULL A TYLKO DOSTAJE FLIGHTID I USERID
                //FlightDto = flightService.MapToDto(flightReservation.Flight),
                //UserDto = userService.MapToDto(flightReservation.User),

                FlightDto = flightService.GetByIdDtoObject(flightReservation.FlightId.Value),
                UserDto = userService.GetByIdDtoObject(flightReservation.UserId.Value),

                NumberOfReservedSeats = flightReservation.NumberOfReservedSeats
            };
        }

        public override FlightReservation MapAddEditDtoToEntity(FlightReservationAddEditDto dto, FlightReservation flightReservation)
        {
            if (flightReservation == null)
                flightReservation = new FlightReservation();

            //flightReservation.Flight = new Flight
            //{
            //    Id = dto.FlightDto.Id,
            //    FlightCode = dto.FlightDto.FlightCode,
            //    DepartureAirport = dto.FlightDto.DepartureAirport,
            //    DepartureTime = dto.FlightDto.DepartureTime,
            //    DestinationAirport = dto.FlightDto.DestinationAirport,
            //    ArrivalTime = dto.FlightDto.ArrivalTime,
            //    Capacity = dto.FlightDto.Capacity
            //};

            //flightReservation.User = new User
            //{
            //    Id = dto.UserDto.Id,
            //    Login = dto.UserDto.Login,
            //    Password = dto.UserDto.Password,
            //    Email = dto.UserDto.Email
            //};

            flightReservation.Flight = flightService.MapAddEditDtoToEntity(dto.FlightAddEditDto);
            flightReservation.User = userService.MapAddEditDtoToEntity(dto.UserAddEditDto);

            flightReservation.NumberOfReservedSeats = dto.NumberOfReservedSeats;

            return flightReservation;
        }

        public List<FlightReservationDto> GetByParameters(FlightAddEditDto? flightAddEditDto, UserAddEditDto? userAddEditDto, long? numberOfReservedSeats)
        {
            var results = baseRepository.GetAll().Where(p => (flightAddEditDto == null || flightService.MapAddEditDtoToEntity(flightAddEditDto).Equals(p.Flight)) &&
                                                            (userAddEditDto == null || userService.MapAddEditDtoToEntity(userAddEditDto).Equals(p.User)) &&
                                                            (numberOfReservedSeats == null || numberOfReservedSeats == p.NumberOfReservedSeats)).Select(MapToDto).ToList();
            return results;
        }
    }
}

using DB.Dto.Base;
using DB.Dto.Flight;
using DB.Dto.User;

namespace DB.Dto.FlightReservation
{
    public class FlightReservationDto : BaseIdDto
    {
        public FlightDto FlightDto { get; set; }
        public UserDto UserDto { get; set; }
        public long NumberOfReservedSeats { get; set; }

        public FlightReservationDto() { }
        public FlightReservationDto(int id, FlightDto FlightDto, UserDto UserDto, long NumberOfReservedSeats) : base(id)
        {
            this.FlightDto = FlightDto;
            this.UserDto = UserDto;
            this.NumberOfReservedSeats = NumberOfReservedSeats;
        }

        public override string ToString()
        {
            return $"FlightReservationDto {base.ToString()}";
        }
    }
}

using DB.Dto.Base;
using DB.Dto.Flight;
using DB.Dto.User;

namespace DB.Dto.FlightReservation
{
    public class FlightReservationAddEditDto : BaseDto
    {
        public FlightAddEditDto FlightAddEditDto { get; set; }
        public UserAddEditDto UserAddEditDto { get; set; }
        public long NumberOfReservedSeats { get; set; }

        public FlightReservationAddEditDto() { }
        public FlightReservationAddEditDto(FlightAddEditDto FlightAddEditDto, UserAddEditDto UserAddEditDto, long NumberOfReservedSeats)
        {
            this.FlightAddEditDto = FlightAddEditDto;
            this.UserAddEditDto = UserAddEditDto;
            this.NumberOfReservedSeats = NumberOfReservedSeats;
        }

        public override string ToString()
        {
            return $"FlightReservationAddEditDto {base.ToString()}";
        }
    }
}

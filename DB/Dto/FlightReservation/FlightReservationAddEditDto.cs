using DB.Dto.Base;
using DB.Dto.Flight;
using DB.Dto.User;
using System.ComponentModel.DataAnnotations;

namespace DB.Dto.FlightReservation
{
    public class FlightReservationAddEditDto : BaseDto
    {
        public int FlightId { get; set; }
        public int UserId { get; set; }
        [Range(1, short.MaxValue, ErrorMessage = "Number of reserved seats must be at least 1.")]
        public short NumberOfReservedSeats { get; set; }

        public FlightReservationAddEditDto() { }
        public FlightReservationAddEditDto(int FlightId, int UserId, short NumberOfReservedSeats)
        {
            this.FlightId = FlightId;
            this.UserId = UserId;
            this.NumberOfReservedSeats = NumberOfReservedSeats;
        }

        public override string ToString()
        {
            return $"FlightReservationAddEditDto {base.ToString()}";
        }
    }
}

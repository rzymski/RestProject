using DB.Dto.Base;
using DB.Dto.Flight;
using DB.Dto.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Dto.FlightReservation
{
    public class FlightReservationAllFieldsDto : BaseIdDto
    {
        public int ReservationId { get; set; }
        public short NumberOfReservedSeats { get; set; }

        public int FlightId { get; set; }
        public string FlightCode { get; set; } = null!;
        public string DepartureAirport { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public string DestinationAirport { get; set; } = null!;
        public DateTime ArrivalTime { get; set; }
        public short Capacity { get; set; }

        public int UserId { get; set; }
        public string Login { get; set; } = null!;
        public string? Email { get; set; }

        public override string ToString()
        {
            return $"FlightReservationAllFieldsDto {base.ToString()}";
        }
    }
}

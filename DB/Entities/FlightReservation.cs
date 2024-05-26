using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.Entities
{
    public class FlightReservation : BaseEntity
    {
        public int FlightId { get; set; }
        [ForeignKey("FlightId")]
        public Flight Flight { get; set; } = null!;

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Range(1, short.MaxValue, ErrorMessage = "Number of reserved seats must be at least 1.")]
        public short NumberOfReservedSeats {  get; set; }

        public override string ToString()
        {
            return "FlightReservation: " + base.ToString();
        }
    }
}

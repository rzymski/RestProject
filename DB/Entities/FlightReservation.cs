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

        public short NumberOfReservedSeats {  get; set; }

        public override string ToString()
        {
            return "FlightReservation: " + base.ToString();
        }
    }
}

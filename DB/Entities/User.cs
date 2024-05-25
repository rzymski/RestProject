using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DB.Entities
{
    public class User : BaseEntity
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Email { get; set; }

        public ICollection<FlightReservation> FlightReservations { get; set; } = new HashSet<FlightReservation>();

        public override string ToString()
        {
            return "User: " + base.ToString();
        }
    }
}

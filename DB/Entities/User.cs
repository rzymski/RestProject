using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DB.Entities
{
    public class User : BaseEntity
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }

        public ICollection<FlightReservation> FlightReservations { get; set; }

        public override string ToString()
        {
            return "User: " + base.ToString();
        }
    }
}

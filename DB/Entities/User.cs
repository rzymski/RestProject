namespace DB.Entities
{
    public class User : BaseEntity
    {
        public string login { get; set; }
        public string password { get; set; }
        public string email { get; set; }

        public ICollection<FlightReservation> FlightReservations { get; set; }

        public override string ToString()
        {
            return "User: " + base.ToString();
        }
    }
}

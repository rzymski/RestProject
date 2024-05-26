namespace RestProject.pdfGenerator;

public class FlightReservationPDFData
{
    public long ReservationId { get; set; }
    public int NumberOfReservedSeats { get; set; }
    public string Login { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FlightCode { get; set; } = null!;
    public string DepartureAirport { get; set; } = null!;
    public DateTime DepartureTime { get; set; }
    public string DestinationAirport { get; set; } = null!;
    public DateTime ArrivalTime { get; set; }
}

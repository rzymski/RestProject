using DB.Dto.Flight;
using DB.Dto.FlightReservation;
using DB.Dto.User;
using DB.Entities;
using DB.Services;
using DB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestProject.pdfGenerator;

namespace RestProject.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AirPortController : ControllerBase
    {
        private readonly ILogger<FlightController> logger;
        private readonly IFlightService flightService;
        private readonly IFlightReservationService flightReservationService;
        private readonly IUserService userService;

        public AirPortController(ILogger<FlightController> logger, IFlightService flightService, IFlightReservationService flightReservationService, IUserService userService)
        {
            this.logger = logger;
            this.flightService = flightService;
            this.flightReservationService = flightReservationService;
            this.userService = userService;
        }

        private UserDto? authenticateUser(string username, string password)
        {
            return userService.GetByParameters(username, password).SingleOrDefault();
        }


        [HttpGet]
        public ActionResult<List<FlightDto>> GetFlightsData()
        {
            return Ok(flightService.GetAllDtoList());
        }

        [HttpGet]
        public ActionResult<string> GetAllFlightsWithParameterss([FromQuery] string? departureAirport, [FromQuery] string? destinationAirport, [FromQuery] DateTime? departureTime, [FromQuery] DateTime? arrivalTime, [FromQuery] short? capacity)
        {
            return Ok(flightService.GetByParameters(departureAirport, destinationAirport, departureTime, arrivalTime, capacity));
        }

        [HttpGet]
        public ActionResult<List<string>> GetAvailableAirports()
        {
            return Ok(flightService.GetAllAirports());
        }

        [HttpGet("{id}")]
        public ActionResult CheckFlightReservation([FromRoute] int id)
        {
            var result = flightReservationService.GetByIdDtoObject(id);
            if (result == null)
                return NotFound(new { flightReservation = $"Not found flight reservation  with id = {id}." });
            return Ok(result);
        }


        [HttpGet("{flightId}")]
        public ActionResult GetFlightById([FromRoute] int flightId)
        {
            var result = flightService.GetByIdDtoObject(flightId);
            if (result == null)
                return NotFound(new { flight = $"Not found flight  with id = {flightId}." });
            return Ok(result);
        }

        [HttpPost("{flightId}")]
        public ActionResult ReserveFlight([FromRoute] int flightId, [FromBody] short numberOfReservedSeats, [FromHeader] string username, [FromHeader] string password)
        {
            var user = authenticateUser(username, password);
            if (user == null)
                return NotFound($"Not found user with username: {username} and given password");
            Console.WriteLine($"Wywolano z flightId = {flightId} i numSeats = {numberOfReservedSeats}");
            var existFlightReservation = flightReservationService.GetByParameters(flightId, user.Id).SingleOrDefault();
            if (existFlightReservation == null)
                return Ok(flightReservationService.Add(new FlightReservationAddEditDto(flightId, user.Id, numberOfReservedSeats)));
            else
            {
                flightReservationService.ChangeNumberOfReservedSeats(existFlightReservation.Id, numberOfReservedSeats);
                return Ok(existFlightReservation.Id);
            }
        }

        [HttpDelete]
        public ActionResult CancelFlightReservation([FromRoute] int flightReservationId) 
        {
            var result = flightReservationService.Delete(flightReservationId);
            if (result)
                return NoContent();
            return NotFound();
        }

        [HttpDelete]
        public ActionResult CancelUserReservationInConcreteFlight([FromRoute] int flightId, [FromHeader] string username, [FromHeader] string password)
        {
            var user = authenticateUser(username, password);
            if (user == null)
                return NotFound($"Not found user with username: {username} and given password");
            var flightReservation = flightReservationService.GetByParameters(flightId, user.Id).SingleOrDefault();
            if (flightReservation == null)
                return NotFound($"User with username: {username} don't have reservation on flight with id = {flightId}.");
            var result = flightReservationService.Delete(flightReservation.Id);
            if (result)
                return NoContent();
            return NotFound();
        }


        [HttpPost]
        public ActionResult CreateUser([FromBody] UserAddEditDto user)
        {
            try
            {
                var id = userService.Add(user);
                return Ok(id);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{username}")]
        public ActionResult<List<FlightReservationDto>> GetUserReservations([FromRoute] string username)
        {
            var user = userService.GetByLogin(username);
            if (user == null)
                return NotFound($"Not found user with login = {username}");
            return Ok(flightReservationService.GetByParameters(null, user.Id));
        }

        [HttpGet("{flightId}")]
        public ActionResult<int> GetFlightAvailableSeats([FromRoute] int flightId)
        {
            try
            {
                return Ok(flightService.GetFlightAvailableSeats(flightId));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public void GeneratePdf([FromRoute] int id)
        {
            var fr = flightReservationService.GetByIdDtoObject(id);
            if (fr == null)
                return;
            var f = flightService.GetByIdDtoObject(fr.FlightId);
            var u = userService.GetByIdDtoObject(fr.UserId);
            var reservation = new FlightReservationPDFData
            {
                ReservationId = fr.Id,
                NumberOfReservedSeats = fr.NumberOfReservedSeats,
                Login = u.Login,
                Email = u.Email,
                FlightCode = f.FlightCode,
                DepartureAirport = f.DepartureAirport,
                DepartureTime = f.DepartureTime,
                DestinationAirport = f.DepartureAirport,
                ArrivalTime = f.ArrivalTime
            };
            // Ścieżka do zapisu wygenerowanego PDF
            string pdfPath = Path.Combine("../pdfs", "test2.pdf");
            using (var outputStream = new FileStream(pdfPath, FileMode.Create, FileAccess.Write))
            {
                var pdfGenerator = new PdfGenerator(outputStream, reservation);
                pdfGenerator.SetHeaderFooter("", ""); // brak tekstu w header-ze i footerze
                string relativeImagePath = Path.Combine("../images", "plane.png");
                pdfGenerator.SetImage(relativeImagePath);
                pdfGenerator.Generate();
            }
            Console.WriteLine("PDF został wygenerowany i zapisany jako: " + pdfPath);
        }
    }
}

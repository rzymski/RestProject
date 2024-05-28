using DB.Dto.Flight;
using DB.Dto.FlightReservation;
using DB.Dto.User;
using DB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestProject.pdfGenerator;

namespace RestProject.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AirportController : ControllerBase
    {
        private readonly ILogger<FlightController> logger;
        private readonly IFlightService flightService;
        private readonly IFlightReservationService flightReservationService;
        private readonly IUserService userService;

        public AirportController(ILogger<FlightController> logger, IFlightService flightService, IFlightReservationService flightReservationService, IUserService userService)
        {
            this.logger = logger;
            this.flightService = flightService;
            this.flightReservationService = flightReservationService;
            this.userService = userService;
        }

        [HttpGet]
        public ActionResult<List<FlightDto>> GetFlightsData()
        {
            return Ok(flightService.GetAllDtoList());
        }

        [HttpGet]
        public ActionResult<string> GetAllQualifyingFlights([FromQuery] string? departureAirport, [FromQuery] string? destinationAirport, [FromQuery] DateTime? departureStartDateRange, [FromQuery] DateTime? departureEndDateRange)
        {
            return Ok(flightService.GetAllQualifyingFlights(departureAirport, destinationAirport, departureStartDateRange, departureEndDateRange));
        }

        [HttpGet]
        public ActionResult<List<string>> GetAvailableAirports()
        {
            return Ok(flightService.GetAllAirports());
        }

        [HttpGet("{flightReservationId}")]
        public ActionResult CheckFlightReservation([FromRoute] int flightReservationId)
        {
            var result = flightReservationService.GetByIdAllFieldsDtoObject(flightReservationId);
            if (result == null)
                return NotFound(new { flightReservation = $"Not found flight reservation  with id = {flightReservationId}." });
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
            UserDto? user = HttpContext.Items["CurrentUser"] as UserDto;
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

        [HttpDelete("{flightReservationId}")]
        public ActionResult CancelFlightReservation([FromRoute] int flightReservationId)
        {
            var result = flightReservationService.Delete(flightReservationId);
            if (result)
                return NoContent();
            return NotFound();
        }

        [HttpDelete("{flightId}")]
        public ActionResult CancelUserReservationInConcreteFlight([FromRoute] int flightId, [FromHeader] string username, [FromHeader] string password)
        {
            UserDto? user = HttpContext.Items["CurrentUser"] as UserDto;
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
        public ActionResult<List<FlightReservationAllFieldsDto>> GetUserReservations([FromRoute] string username)
        {
            var user = userService.GetByLogin(username);
            if (user == null)
                return NotFound($"Not found user with login = {username}");
            List<FlightReservationDto> flightReservationsDto = flightReservationService.GetByParameters(null, user.Id);

            List<FlightReservationAllFieldsDto> flightReservationsAllFieldsDto = new List<FlightReservationAllFieldsDto>();
            foreach (var reservationDto in flightReservationsDto)
            {
                FlightReservationAllFieldsDto reservationAllFieldsDto = flightReservationService.GetByIdAllFieldsDtoObject(reservationDto.Id);
                flightReservationsAllFieldsDto.Add(reservationAllFieldsDto);
            }
            return Ok(flightReservationsAllFieldsDto);
            //return Ok(flightReservationService.GetByParameters(null, user.Id));
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
        public ActionResult GeneratePDF([FromRoute] int id)
        {
            var fr = flightReservationService.GetByIdDtoObject(id);
            if (fr == null)
                return NotFound($"Not found flight reservation with id = {id}");

            var f = flightService.GetByIdDtoObject(fr.FlightId);
            var u = userService.GetByIdDtoObject(fr.UserId);
            if (f == null || u == null)
                return NotFound($"Flight reservation have null user = {u} or flight = {f}");

            var reservation = new FlightReservationAllFieldsDto
            {
                ReservationId = fr.Id,
                NumberOfReservedSeats = fr.NumberOfReservedSeats,

                FlightId = f.Id,
                FlightCode = f.FlightCode,
                DepartureAirport = f.DepartureAirport,
                DepartureTime = f.DepartureTime,
                DestinationAirport = f.DestinationAirport,
                ArrivalTime = f.ArrivalTime,

                UserId = u.Id,
                Login = u.Login,
                Email = u.Email
            };

            var pdfGenerator = new PdfGenerator(reservation);
            pdfGenerator.SetHeaderFooter("", ""); // brak tekstu w header-ze i footerze
            string relativeImagePath = Path.Combine("../images", "plane.png");
            pdfGenerator.SetImage(relativeImagePath); // Ustawienie obrazu
            byte[] pdfBytes = pdfGenerator.Generate();

            return File(pdfBytes, "application/pdf", "Reservation.pdf");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GeneratePDFAsynchronouslyAsync([FromRoute] int id)
        {
            var fr = await flightReservationService.GetByIdDtoObjectAsync(id);
            if (fr == null)
                return NotFound($"Not found flight reservation with id = {id}");

            var f = await flightService.GetByIdDtoObjectAsync(fr.FlightId);
            var u = await userService.GetByIdDtoObjectAsync(fr.UserId);
            if (f == null || u == null)
                return NotFound($"Flight reservation have null user = {u} or flight = {f}");

            var reservation = new FlightReservationAllFieldsDto
            {
                ReservationId = fr.Id,
                NumberOfReservedSeats = fr.NumberOfReservedSeats,

                FlightId = f.Id,
                FlightCode = f.FlightCode,
                DepartureAirport = f.DepartureAirport,
                DepartureTime = f.DepartureTime,
                DestinationAirport = f.DestinationAirport,
                ArrivalTime = f.ArrivalTime,

                UserId = u.Id,
                Login = u.Login,
                Email = u.Email
            };

            var pdfGenerator = new PdfGenerator(reservation);
            pdfGenerator.SetHeaderFooter("", ""); // brak tekstu w header-ze i footerze
            string relativeImagePath = Path.Combine("../images", "plane.png");
            pdfGenerator.SetImage(relativeImagePath); // Ustawienie obrazu
            byte[] pdfBytes = await pdfGenerator.GenerateAsync();

            return File(pdfBytes, "application/pdf", "Reservation.pdf");
        }

        [HttpPost]
        public ActionResult<string> Echo([FromHeader] string username, [FromBody] string text)
        {
            Response.Headers["usernameExist"] = (userService.GetByLogin(username) != null).ToString();
            return Ok($"Serwer zwraca otrzymany text: {text}");
        }
    }
}
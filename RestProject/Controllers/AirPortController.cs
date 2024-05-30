using DB.Dto.Flight;
using DB.Dto.FlightReservation;
using DB.Dto.HATEOAS;
using DB.Dto.User;
using DB.Entities;
using DB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestProject.pdfGenerator;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

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
        private LinkGenerator _linkGenerator;

        public AirportController(ILogger<FlightController> logger, IFlightService flightService, IFlightReservationService flightReservationService, IUserService userService, LinkGenerator linkGenerator)
        {
            this.logger = logger;
            this.flightService = flightService;
            this.flightReservationService = flightReservationService;
            this.userService = userService;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public ActionResult<List<FlightDto>> GetFlightsData()
        {
            List<FlightDto> flights = flightService.GetAllDtoList();
            foreach (var flight in flights)
                flight.Links = CreateLinksForFlight(flight);
            return Ok(CreateLinksForFlights(flights));
        }

        [HttpGet]
        public ActionResult<List<FlightDto>> GetAllQualifyingFlights([FromQuery] string? departureAirport, [FromQuery] string? destinationAirport, [FromQuery] DateTime? departureStartDateRange, [FromQuery] DateTime? departureEndDateRange)
        {
            List<FlightDto> flights = flightService.GetAllQualifyingFlights(departureAirport, destinationAirport, departureStartDateRange, departureEndDateRange);
            foreach (var flight in flights)
                flight.Links = CreateLinksForFlight(flight);
            return Ok(CreateLinksForFlights(flights));
        }

        [HttpGet]
        public ActionResult<List<string>> GetAvailableAirports()
        {
            List<string> airtports = flightService.GetAllAirports();
            var links = CreateLinksForFlight(null);
            var result = new
            {
                Airports = airtports,
                Links = links
            };
            return Ok(result);
        }

        [HttpGet("{flightReservationId}")]
        public ActionResult CheckFlightReservation([FromRoute] int flightReservationId)
        {
            try
            {
                var result = flightReservationService.GetByIdAllFieldsDtoObject(flightReservationId);
                if (result == null)
                    return NotFound(new { flightReservation = $"Not found flight reservation  with flightReservationId = {flightReservationId}." });
                result.Links = CreateLinksForFlightReservation(flightReservationService.GetByIdDtoObject(flightReservationId));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{flightId}")]
        public ActionResult GetFlightById([FromRoute] int flightId)
        {
            var result = flightService.GetByIdDtoObject(flightId);
            if (result == null)
                return NotFound(new { flight = $"Not found flight  with flightReservationId = {flightId}." });
            result.Links = CreateLinksForFlight(result);
            return Ok(result);
        }

        [HttpPost("{flightId}")]
        public ActionResult ReserveFlight([FromRoute] int flightId, [FromBody] short numberOfReservedSeats, [FromHeader] string username, [FromHeader] string password)
        {
            UserDto? user = HttpContext.Items["CurrentUser"] as UserDto;
            if (user == null)
                return NotFound($"Not found user with username: {username} and given password");
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
                return NotFound($"User with username: {username} don't have reservation on flight with flightReservationId = {flightId}.");
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
        public ActionResult GetUserReservations([FromRoute] string username)
        {
            var user = userService.GetByLogin(username);
            if (user == null)
                return NotFound($"Not found user with login = {username}");
            List<FlightReservationDto> flightReservationsDto = flightReservationService.GetByParameters(null, user.Id);

            List<FlightReservationAllFieldsDto> flightReservationsAllFieldsDto = new List<FlightReservationAllFieldsDto>();
            foreach (var reservationDto in flightReservationsDto)
            {
                FlightReservationAllFieldsDto reservationAllFieldsDto = flightReservationService.GetByIdAllFieldsDtoObject(reservationDto.Id);
                reservationAllFieldsDto.Links = CreateLinksForFlightReservation(reservationDto);
                flightReservationsAllFieldsDto.Add(reservationAllFieldsDto);
            }

            return Ok(CreateLinksForFlightReservations(flightReservationsAllFieldsDto));
        }

        [HttpGet("{flightId}")]
        public ActionResult GetFlightAvailableSeats([FromRoute] int flightId)
        {
            try
            {
                int availableSeats = flightService.GetFlightAvailableSeats(flightId);
                var links = CreateLinksForFlight(new FlightDto(flightId, "", "", DateTime.MinValue, "", DateTime.MinValue, 0));
                var result = new
                {
                    AvailableSeats = availableSeats,
                    Links = links
                };
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{flightReservationId}")]
        public ActionResult GeneratePDF([FromRoute] int flightReservationId)
        {
            var fr = flightReservationService.GetByIdDtoObject(flightReservationId);
            if (fr == null)
                return NotFound($"Not found flight reservation with flightReservationId = {flightReservationId}");

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

        [HttpGet("{flightReservationId}")]
        public async Task<ActionResult> GeneratePDFAsynchronouslyAsync([FromRoute] int flightReservationId)
        {
            var fr = await flightReservationService.GetByIdDtoObjectAsync(flightReservationId);
            if (fr == null)
                return NotFound($"Not found flight reservation with flightReservationId = {flightReservationId}");

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
        public ActionResult<string> Echo([FromHeader] string? username, [FromBody] string text)
        {
            if (username != null)
                Response.Headers["usernameExist"] = (userService.GetByLogin(username) != null).ToString();
            return Ok($"Serwer zwraca otrzymany text: {text}");
        }


        // HATEOAS
        private List<Link> CreateLinksForFlight(FlightDto? flight, [CallerMemberName] string actionName = "")
        {
            int flightIdValue = flight != null ? flight.Id : 0;
            return new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetFlightById), values: new { flightId = flightIdValue }), actionName == nameof(GetFlightById) ? "self" : "get_flight", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAvailableAirports)), actionName == nameof(GetAvailableAirports) ? "self" : "available_airports", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(ReserveFlight), values: new { flightId = flightIdValue }), "reserve_flight", "POST"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetFlightAvailableSeats), values: new { flightId = flightIdValue }), actionName == nameof(GetFlightAvailableSeats) ? "self" : "available_seats", "GET")
            };
        }

        private LinkCollectionWrapper<FlightDto> CreateLinksForFlights(List<FlightDto> flights, [CallerMemberName] string actionName = "")
        {
            LinkCollectionWrapper<FlightDto> wrapper = new LinkCollectionWrapper<FlightDto>(flights);
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetFlightsData)), actionName == nameof(GetFlightsData) ? "self" : "get_all_flights", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAllQualifyingFlights), values: new { departureAirport = "city", destinationAirport = "city", departureStartDateRange = DateTime.Now.ToString(), departureEndDateRange = DateTime.Now.ToString() }), actionName == nameof(GetAllQualifyingFlights) ? "self" : "get_flights_with_parameters", "GET"));
            return wrapper;
        }


        private List<Link> CreateLinksForFlightReservation(FlightReservationDto? flightReservation, [CallerMemberName] string actionName = "")
        {
            int flightIdValue = flightReservation != null ? flightReservation.FlightId : 0;
            int reservationIdValue = flightReservation != null ? flightReservation.Id : 0;
            return new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(CheckFlightReservation), values: new { flightReservationId = reservationIdValue }), actionName == nameof(CheckFlightReservation) ? "self" : "get_flightReservation", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(ReserveFlight), values: new { flightId = flightIdValue }), "reserve_flight", "POST"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(CancelFlightReservation), values: new { flightReservationId = reservationIdValue }), "cancel_flight_reservation", "DELETE"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(CancelUserReservationInConcreteFlight), values: new { flightId = flightIdValue }), "cancel_user_reservation_in_concrete_flight", "DELETE"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GeneratePDF), values: new { flightReservationId = reservationIdValue }), actionName == nameof(GeneratePDF) ? "self" : "generate_pdf", "GET")
            };
        }

        private LinkCollectionWrapper<FlightReservationAllFieldsDto> CreateLinksForFlightReservations(List<FlightReservationAllFieldsDto> flightReservations, [CallerMemberName] string actionName = "")
        {
            string login = flightReservations.Count != 0 ? flightReservations.First().Login : "username";
            LinkCollectionWrapper<FlightReservationAllFieldsDto> wrapper = new LinkCollectionWrapper<FlightReservationAllFieldsDto>(flightReservations);
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetUserReservations), values: new { username = login }), actionName == nameof(GetUserReservations) ? "self" : "get_user_reservations", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetFlightsData)), actionName == nameof(GetFlightsData) ? "self" : "get_all_flights", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(CreateUser)), "create_user", "POST"));
            return wrapper;
        }

        //private List<Link> CreateLinksForUser(UserDto? user)
        //{
        //    return new List<Link>
        //    {
        //        new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(CreateUser), values: new { }), "add_user", "POST"),
        //        new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(ReserveFlight), values: new { flightId = 0 }), "reserve_flight", "POST")
        //    };
        //}
        //private LinkCollectionWrapper<UserDto> CreateLinksForUsers(List<UserDto> users, string username)
        //{
        //    LinkCollectionWrapper<UserDto> wrapper = new LinkCollectionWrapper<UserDto>(users);
        //    wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetUserReservations), values: new { username = "username" }), "self", "GET"));
        //    wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetFlightsData)), "get_all_flights", "GET"));
        //    return wrapper;
        //}


        //[HttpGet("{flightId}/comments")]
        //public IActionResult GetComments(int flightId)
        //{
        //    return RedirectToAction("GetComments", "Comments", new { flightId = flightId });
        //}
        //[HttpPost("{flightId}/comments")]
        //public IActionResult AddComment(int flightId, [FromBody] string comment)
        //{
        //    return RedirectToAction("AddComment", "Comments", new { flightId = flightId, comment = comment });
        //}
    }


    //[ApiController]
    //[Route("Airport/[controller]")]
    //public class CommentsController : ControllerBase
    //{
    //    [HttpGet("{flightId}/comments")]
    //    public ActionResult<List<string>> GetComments(int flightId)
    //    {
    //        return Ok(new List<string> { "Comment 1", "Comment 2" });
    //    }

    //    [HttpPost("{flightId}/comments")]
    //    public ActionResult<string> AddComment(int flightId, [FromBody] string comment)
    //    {
    //        return Ok("Comment added");
    //    }
    //}
}
using DB.Dto.Flight;
using DB.Dto.FlightReservation;
using DB.Dto.User;
using DB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestProject.Filters;
using RestProject.HATEOAS.Filters;
using RestProject.pdfGenerator;

namespace RestProject.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[ServiceFilter(typeof(HateoasAirportFilter))]
public class AirportController : ControllerBase
{
    private readonly ILogger<AirportController> logger;
    private readonly IFlightService flightService;
    private readonly IFlightReservationService flightReservationService;
    private readonly IUserService userService;

    public AirportController(ILogger<AirportController> logger, IFlightService flightService, IFlightReservationService flightReservationService, IUserService userService)
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
    public ActionResult<List<FlightDto>> GetAllQualifyingFlights([FromQuery] string? departureAirport, [FromQuery] string? destinationAirport, [FromQuery] DateTime? departureStartDateRange, [FromQuery] DateTime? departureEndDateRange)
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
        try
        {
            var result = flightReservationService.GetByIdAllFieldsDtoObject(flightReservationId);
            if (result == null)
                return NotFound(new { flightReservation = $"Not found flight reservation  with id = {flightReservationId}." });
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
            return NotFound(new { flight = $"Not found flight  with id = {flightId}." });
        return Ok(result);
    }

    [HttpPost("{flightId}")]
    [ServiceFilter(typeof(BasicAuthFilter))]
    public ActionResult ReserveFlight([FromRoute] int flightId, [FromBody] short numberOfReservedSeats)
    {
        UserDto? user = HttpContext.Items["UserBasicAuthorization"] as UserDto;
        if (user == null)
            return NotFound($"Not found user with username: {user?.Login} and given password");
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
    [ServiceFilter(typeof(BasicAuthFilter))]
    public ActionResult CancelUserReservationInConcreteFlight([FromRoute] int flightId)
    {
        UserDto? user = HttpContext.Items["UserBasicAuthorization"] as UserDto;
        if (user == null)
            return NotFound($"Not found user with username: {user?.Login} and given password");
        var flightReservation = flightReservationService.GetByParameters(flightId, user.Id).SingleOrDefault();
        if (flightReservation == null)
            return NotFound($"User with username: {user?.Login} don't have reservation on flight with flightId = {flightId}.");
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
            flightReservationsAllFieldsDto.Add(reservationAllFieldsDto);
        }
        return Ok(flightReservationsAllFieldsDto);
        //return Ok(flightReservationService.GetByParameters(null, user.Id));
    }

    [HttpGet("{flightId}")]
    public ActionResult GetFlightAvailableSeats([FromRoute] int flightId, [FromHeader] string? username)
    {
        try
        {
            return Ok(flightService.GetFlightAvailableSeats(flightId, username));
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


    [HttpGet]
    [ServiceFilter(typeof(BasicAuthFilter))]
    public ActionResult<string> ValidateUser()
    {
        return Ok("Udało się zaautoryzowac uzytkownika przez Basic Auth!");
    }


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
/*using DB.Dto.FlightReservation;
using DB.Dto.HATEOAS;
using DB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace RestProject.Controllers.ControllersWithLinks;

//przestarzala wersja gdzie w kontrollerze jest dodawanie HATEOAS zamiast w filtrze

[ApiController]
[Route("[controller]/[action]")]
public class FlightReservationWithLinksController : ControllerBase
{
    private readonly ILogger<FlightReservationWithLinksController> logger;
    private readonly IFlightReservationService flightReservationService;
    private LinkGenerator _linkGenerator;

    public FlightReservationWithLinksController(ILogger<FlightReservationWithLinksController> logger, IFlightReservationService flightReservationService, LinkGenerator linkGenerator)
    {
        this.logger = logger;
        this.flightReservationService = flightReservationService;
        _linkGenerator = linkGenerator;
    }

    [HttpGet("{id}")]
    public ActionResult GetOne([FromRoute] int id)
    {
        var result = flightReservationService.GetByIdDtoObject(id);
        if (result == null)
            return NotFound(new { flightReservation = $"FlightReservation not found with id = {id}." });
        result.Links = CreateLinksForFlightReservation(result.Id);
        return Ok(result);
    }

    [HttpGet]
    public ActionResult<List<FlightReservationDto>> GetList()
    {
        List<FlightReservationDto> flightReservations = flightReservationService.GetAllDtoList();
        foreach (var flightReservation in flightReservations)
            flightReservation.Links = CreateLinksForFlightReservation(flightReservation.Id);
        return Ok(CreateLinksForFlightReservations(flightReservations));
    }

    [HttpPost]
    public ActionResult Add([FromBody] FlightReservationAddEditDto flightReservation)
    {
        try
        {
            var id = flightReservationService.Add(flightReservation);
            return Ok(id);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult AddList([FromBody] List<FlightReservationAddEditDto> flightReservations)
    {
        try
        {
            var ids = flightReservationService.AddList(flightReservations);
            return Ok(ids);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public ActionResult Update([FromRoute] int id, [FromBody] FlightReservationAddEditDto flightReservation)
    {
        try
        {
            var result = flightReservationService.Update(id, flightReservation);
            if (result)
                return NoContent();
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Delete([FromRoute] int id)
    {
        var result = flightReservationService.Delete(id);
        if (result)
            return NoContent();
        return NotFound();
    }

    [HttpPatch("{id}")]
    public ActionResult ChangeNumberOfReservedSeats([FromRoute] int id, [FromBody] short numberOfReservedSeats)
    {
        try
        {
            var result = flightReservationService.ChangeNumberOfReservedSeats(id, numberOfReservedSeats);
            if (result)
                return NoContent();
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public ActionResult<string> GetByValues([FromQuery] int? flightId, [FromQuery] int? userId, [FromQuery] short? numberOfReservedSeats)
    {
        List<FlightReservationDto> flightReservations = flightReservationService.GetByParameters(flightId, userId, numberOfReservedSeats);
        foreach (var flightReservation in flightReservations)
            flightReservation.Links = CreateLinksForFlightReservation(flightReservation.Id);
        return Ok(CreateLinksForFlightReservations(flightReservations));
    }

    [HttpGet("{id}")]
    public ActionResult GetFlightReservationAllData([FromRoute] int id)
    {
        try
        {

            var result = flightReservationService.GetByIdAllFieldsDtoObject(id);
            if (result == null)
                return NotFound(new { flightReservation = $"FlightReservation not found with id = {id}." });
            result.Links = CreateLinksForFlightReservation(id);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetAsynchronouslyFlightReservationAllDataAsync([FromRoute] int id)
    {
        try
        {
            var result = await Task.Run(() => flightReservationService.GetByIdAllFieldsDtoObject(id));
            if (result == null)
                return NotFound(new { flightReservation = $"FlightReservation not found with id = {id}." });
            result.Links = CreateLinksForFlightReservation(id);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    //HATEOAS
    //nie dziala generowanie href dla GetAsynchronouslyFlightReservationAllDataAsync nie wiem czemu
    private List<Link> CreateLinksForFlightReservation(int id, [CallerMemberName] string actionName = "")
    {
        var links = new List<Link>
        {
            new Link(_linkGenerator.GetUriByAction(HttpContext, actionName, values: new { id }), "self", "GET"),
            new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Add), values: new { }), "add_flightReservation", "POST"),
            new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Update), values: new { id }), "update_flightReservation", "PUT"),
            new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Delete), values: new { id }), "delete_flightReservation", "DELETE"),
            new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(ChangeNumberOfReservedSeats), values: new { id }), "edit_flightReservation", "PATCH"),
        };
        if (actionName == nameof(GetOne))
        {
            links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetFlightReservationAllData), values: new { id }), "get_flightReservation", "GET"));
            links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAsynchronouslyFlightReservationAllDataAsync), values: new { id }), "get_flightReservation", "GET"));
        }
        else if (actionName == nameof(GetFlightReservationAllData))
        {
            links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetOne), values: new { id }), "get_flightReservation", "GET"));
            links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAsynchronouslyFlightReservationAllDataAsync), values: new { id }), "get_flightReservation", "GET"));
        }
        else if (actionName == nameof(GetAsynchronouslyFlightReservationAllDataAsync))
        {
            links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetOne), values: new { id }), "get_flightReservation", "GET"));
            links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetFlightReservationAllData), values: new { id }), "get_flightReservation2", "GET"));
        }

        return links;
    }

    private LinkCollectionWrapper<FlightReservationDto> CreateLinksForFlightReservations(List<FlightReservationDto> flightReservations, [CallerMemberName] string actionName = "")
    {
        LinkCollectionWrapper<FlightReservationDto> wrapper = new LinkCollectionWrapper<FlightReservationDto>(flightReservations);
        wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetList), values: new { }), actionName == nameof(GetList) ? "self" : "get_flightReservations", "GET"));
        wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetByValues), values: new { flightId = 0, userId = 0, numberOfReservedSeats = 0 }), actionName == nameof(GetByValues) ? "self" : "get_flightReservations", "GET"));
        wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(AddList), values: new { }), "add_flightReservations", "POST"));
        return wrapper;
    }
}
*/
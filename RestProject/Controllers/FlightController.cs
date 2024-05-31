using DB.Dto.Flight;
using DB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestProject.HATEOAS.Filters;

namespace RestProject.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[ServiceFilter(typeof(HateoasFlightFilter))]
public class FlightController : ControllerBase
{
    private readonly ILogger<FlightController> logger;
    private readonly IFlightService flightService;

    public FlightController(ILogger<FlightController> logger, IFlightService flightService)
    {
        this.logger = logger;
        this.flightService = flightService;
    }

    [HttpGet("{id}")]
    public ActionResult GetOne([FromRoute] int id)
    {
        var result = flightService.GetByIdDtoObject(id);
        if (result == null)
            return NotFound(new { flight = $"Flight not found with id = {id}." });
        return Ok(result);
    }

    [HttpGet]
    public ActionResult<List<FlightDto>> GetList()
    {
        return Ok(flightService.GetAllDtoList());
    }

    [HttpPost]
    public ActionResult Add([FromBody] FlightAddEditDto flight)
    {
        try
        {
            var id = flightService.Add(flight);
            return Ok(id);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult AddList([FromBody] List<FlightAddEditDto> flights)
    {
        try
        {
            var ids = flightService.AddList(flights);
            return Ok(ids);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public ActionResult Update([FromRoute] int id, [FromBody] FlightAddEditDto flight)
    {
        try
        {
            var result = flightService.Update(id, flight);
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
        var result = flightService.Delete(id);
        if (result)
            return NoContent();
        return NotFound();
    }

    [HttpGet]
    public ActionResult<string> GetByValues([FromQuery] string? departureAirport, [FromQuery] string? destinationAirport, [FromQuery] DateTime? departureTime, [FromQuery] DateTime? arrivalTime, [FromQuery] short? capacity)
    {
        return Ok(flightService.GetByParameters(departureAirport, destinationAirport, departureTime, arrivalTime, capacity));
    }
}
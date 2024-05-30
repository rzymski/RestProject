using DB.Dto.Flight;
using DB.Entities;
using DB.Services;
using DB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static iText.IO.Util.IntHashtable;
using DB.Dto.Base;
using DB.Dto.HATEOAS;
using System.Runtime.CompilerServices;

namespace RestProject.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FlightController : ControllerBase
    {
        private readonly ILogger<FlightController> logger;
        private readonly IFlightService flightService;
        private LinkGenerator _linkGenerator;

        public FlightController(ILogger<FlightController> logger, IFlightService flightService, LinkGenerator linkGenerator)
        {
            this.logger = logger;
            this.flightService = flightService;
            _linkGenerator = linkGenerator;
        }

        [HttpGet("{id}")]
        public ActionResult GetOne([FromRoute] int id)
        {
            var result = flightService.GetByIdDtoObject(id);
            if (result == null)
                return NotFound(new { flight = $"Flight not found with id = {id}." });
            result.Links = CreateLinksForFlight(result.Id);
            return Ok(result);
        }

        [HttpGet]
        public ActionResult<List<FlightDto>> GetList()
        {
            List<FlightDto> flights = flightService.GetAllDtoList();
            foreach (var flight in flights)
                flight.Links = CreateLinksForFlight(flight.Id);
            return Ok(CreateLinksForFlights(flights));
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
        public ActionResult GetByValues([FromQuery] string? departureAirport, [FromQuery] string? destinationAirport, [FromQuery] DateTime? departureTime, [FromQuery] DateTime? arrivalTime, [FromQuery] short? capacity)
        {
            List<FlightDto> flights = flightService.GetByParameters(departureAirport, destinationAirport, departureTime, arrivalTime, capacity);
            foreach (var flight in flights)
                flight.Links = CreateLinksForFlight(flight.Id);
            return Ok(CreateLinksForFlights(flights));
        }


        //HATEOAS
        private List<Link> CreateLinksForFlight(int id)
        {
            return new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetOne), values: new { id }), "self", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Add), values: new { }), "add_flight", "POST"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Update), values: new { id }), "update_flight", "PUT"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Delete), values: new { id }), "delete_flight", "DELETE"),
            };
        }

        private LinkCollectionWrapper<FlightDto> CreateLinksForFlights(List<FlightDto> flights, [CallerMemberName] string actionName = "")
        {
            LinkCollectionWrapper<FlightDto> wrapper = new LinkCollectionWrapper<FlightDto>(flights);
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetList), values: new { }), actionName == nameof(GetList) ? "self" : "get_flights", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetByValues), values: new { departureAirport = "city", destinationAirport = "city", departureStartDateRange = DateTime.Now.ToString(), departureEndDateRange = DateTime.Now.ToString(), capacity=0 }), actionName == nameof(GetByValues) ? "self" : "get_flights", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(AddList), values: new { }), "add_flights", "POST"));
            return wrapper;
        }
    }
}

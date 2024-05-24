﻿using DB.Dto.Flight;
using DB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RestProject.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
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
        public IActionResult GetOne([FromRoute] int id)
        {
            var result = flightService.GetByIdDtoObject(id);
            if (result == null)
                return NotFound(new { flight = $"Nie znaleziono rekordu o id = {id}." });
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
            var id = flightService.Add(flight);
            return Ok(id);
        }

        [HttpPost]
        public ActionResult AddList([FromBody] List<FlightAddEditDto> flights)
        {
            var ids = flightService.AddList(flights);
            return Ok(ids);
        }



        [HttpPut("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] FlightAddEditDto flight)
        {
            var result = flightService.Update(id, flight);
            if (result)
                return NoContent();
            return NotFound();
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
        public ActionResult<string> GetByValues([FromQuery] string? departureAirport, [FromQuery] string? destinationAirport, [FromQuery] DateTime? departureTime, [FromQuery] DateTime? arrivalTime)
        {
            return Ok(flightService.GetByParameters(departureAirport, destinationAirport, departureTime, arrivalTime));
        }
    }
}

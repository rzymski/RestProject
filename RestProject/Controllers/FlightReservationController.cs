using DB.Dto.Flight;
using DB.Dto.FlightReservation;
using DB.Dto.User;
using DB.Entities;
using DB.Services;
using DB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RestProject.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FlightReservationController : ControllerBase
    {
        private readonly ILogger<FlightReservationController> logger;
        private readonly IFlightReservationService flightReservationService;

        public FlightReservationController(ILogger<FlightReservationController> logger, IFlightReservationService flightReservationService)
        {
            this.logger = logger;
            this.flightReservationService = flightReservationService;
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var result = flightReservationService.GetByIdDtoObject(id);
            if (result == null)
                return NotFound(new { flightReservation = $"FlightReservation not found with id = {id}." });
            return Ok(result);
        }

        [HttpGet]
        public ActionResult<List<FlightReservationDto>> GetList()
        {
            return Ok(flightReservationService.GetAllDtoList());
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
            return Ok(flightReservationService.GetByParameters(flightId, userId, numberOfReservedSeats));
        }
    }
}

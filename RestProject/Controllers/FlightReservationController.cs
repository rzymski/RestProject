using DB.Dto.Flight;
using DB.Dto.FlightReservation;
using DB.Dto.User;
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
                return NotFound(new { flightReservation = $"Nie znaleziono rekordu o id = {id}." });
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
            var id = flightReservationService.Add(flightReservation);
            return Ok(id);
        }

        [HttpPost]
        public ActionResult AddList([FromBody] List<FlightReservationAddEditDto> flightReservations)
        {
            var ids = flightReservationService.AddList(flightReservations);
            return Ok(ids);
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] FlightReservationAddEditDto flightReservation)
        {
            var result = flightReservationService.Update(id, flightReservation);
            if (result)
                return NoContent();
            return NotFound();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var result = flightReservationService.Delete(id);
            if (result)
                return NoContent();
            return NotFound();
        }

        [HttpPut("{id}")]
        public ActionResult ChangeNumberOfReservedSeats([FromRoute] int id, [FromBody] short numberOfReservedSeats)
        {
            var result = flightReservationService.ChangeNumberOfReservedSeats(id, numberOfReservedSeats);
            if (result)
                return NoContent();
            return NotFound();
        }

        [HttpGet]
        public ActionResult<string> GetByValues([FromQuery] int? flightId, [FromQuery] int? userId, [FromQuery] short? numberOfReservedSeats)
        {
            return Ok(flightReservationService.GetByParameters(flightId, userId, numberOfReservedSeats));
        }
    }
}

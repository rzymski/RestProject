using Microsoft.AspNetCore.Mvc;

namespace RestProject.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FlightController : ControllerBase
    {
        private readonly ILogger<FlightController> logger;

        public FlightController(ILogger<FlightController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("DZIALA");
        }
    }
}

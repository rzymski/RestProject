using DB.Dto.Flight;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using RestProject.HATEOAS.Services;

namespace RestProject.HATEOAS.Filters;

public class HateoasFlightFilter : IActionFilter
{
    private readonly HateoasFlightService _hateoasService;

    public HateoasFlightFilter(HateoasFlightService hateoasService)
    {
        _hateoasService = hateoasService;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // No logic needed before the action executes
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is OkObjectResult okObjectResult)
        {
            string actionName = context.ActionDescriptor.RouteValues["action"]?.ToString() ?? "";
            if (okObjectResult.Value is FlightDto flightDto)
                flightDto.Links = _hateoasService.CreateLinksForFlight(flightDto.Id, actionName);
            else if (okObjectResult.Value is List<FlightDto> flights)
            {
                foreach (var flight in flights)
                    flight.Links = _hateoasService.CreateLinksForFlight(flight.Id, actionName);
                okObjectResult.Value = _hateoasService.CreateLinksForFlights(flights, actionName);
            }
        }
    }
}

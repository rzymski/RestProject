using DB.Dto.FlightReservation;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using RestProject.HATEOAS.Services;

namespace RestProject.HATEOAS.Filters;

public class HateoasFlightReservationFilter : IActionFilter
{
    private readonly HateoasFlightReservationService _hateoasService;

    public HateoasFlightReservationFilter(HateoasFlightReservationService hateoasService)
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
            if (okObjectResult.Value is FlightReservationDto flightReservationDto)
            {
                flightReservationDto.Links = _hateoasService.CreateLinksForFlightReservation(flightReservationDto.Id, actionName);
            }
            else if (okObjectResult.Value is List<FlightReservationDto> flightReservations)
            {
                foreach (var flightReservation in flightReservations)
                {
                    flightReservation.Links = _hateoasService.CreateLinksForFlightReservation(flightReservation.Id, actionName);
                }
                okObjectResult.Value = _hateoasService.CreateLinksForFlightReservations(flightReservations, actionName);
            }
        }
    }
}

using DB.Dto.Flight;
using DB.Dto.FlightReservation;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using RestProject.HATEOAS.Services;
using DB.Entities;

namespace RestProject.HATEOAS.Filters
{
    public class HateoasAirportFilter : IActionFilter
    {
        private readonly HateoasAirportService _hateoasService;

        public HateoasAirportFilter(HateoasAirportService hateoasService)
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
                else if (okObjectResult.Value is FlightReservationDto flightReservationDto)
                    flightReservationDto.Links = _hateoasService.CreateLinksForFlightReservation(flightReservationDto.FlightId, flightReservationDto.Id, actionName);
                else if (okObjectResult.Value is List<FlightReservationAllFieldsDto> flightReservations)
                {
                    foreach (var reservation in flightReservations)
                        reservation.Links = _hateoasService.CreateLinksForFlightReservation(reservation.FlightId, reservation.ReservationId, actionName);
                    okObjectResult.Value = _hateoasService.CreateLinksForFlightReservations(flightReservations, actionName);
                }
                else if (okObjectResult.Value is FlightReservationAllFieldsDto flightReservationAllFieldsDto)
                    flightReservationAllFieldsDto.Links = _hateoasService.CreateLinksForFlightReservation(flightReservationAllFieldsDto.FlightId, flightReservationAllFieldsDto.ReservationId, actionName);
                else if (okObjectResult.Value is List<string> airports)
                {
                    okObjectResult.Value = new
                    {
                        Airports = airports,
                        Links = _hateoasService.CreateLinksForFlight(0, actionName)
                    };
                }
                else if (okObjectResult.Value is int value)
                {
                    okObjectResult.Value = new
                    {
                        Value = value,
                        Links = _hateoasService.CreateLinksForFlight(0, actionName)
                    };
                }
            }
        }
    }
}
